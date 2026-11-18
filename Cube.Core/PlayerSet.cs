using System.Numerics;

namespace Cube.Core;

/// <summary>
/// The players and named NPCs inside the cube this frame (PROJECT.md, "Players": every player character in the
/// object table whose position is inside the cube; "NPCs": every named NPC inside the cube, shown the same way;
/// anyone outside is not shown at all). The game reader offers everyone it finds; this class culls, classifies and
/// names them.
/// </summary>
/// <remarks>
/// <para>
/// Keeping the rules here, not in the game reader, means they are unit tested without the game, and the Debug
/// fake players (#16) go through exactly the same path. The buffer is fixed-size and reused (PROJECT.md,
/// "Efficiency and memory"); a full set drops extras and counts them in <see cref="Dropped"/>.
/// </para>
/// <para>
/// Players and NPCs share one buffer, so they share the culling, ordering and label code downstream, but each has its
/// own cap: a crowd of NPCs can never push a player out, and NPCs can't push the frame past the draw budget
/// (<c>docs/research-notes.md</c>, #73: NPC cap 50).
/// </para>
/// </remarks>
public sealed class PlayerSet
{
    /// <summary>
    /// The game's "no owner" entity id (<c>0xE0000000</c>), which <c>GameObject.OwnerId</c> holds for objects nobody
    /// owns (<c>docs/research-notes.md</c>, #73).
    /// </summary>
    public const uint NoOwner = 0xE0000000;

    /// <summary>The reused buffer: players and NPCs, in the order offered.</summary>
    private readonly NearbyPlayer[] players;

    /// <summary>Most players kept per frame.</summary>
    private readonly int playerCapacity;

    /// <summary>Most NPCs kept per frame.</summary>
    private readonly int npcCapacity;

    /// <summary>Names, cached per slot.</summary>
    private readonly NameCache names;

    /// <summary>The cube for this frame, set by <see cref="Begin"/>.</summary>
    private CubeSpace space;

    /// <summary>How many players (not NPCs) were kept this frame.</summary>
    private int playerCount;

    /// <summary>Creates an empty set.</summary>
    /// <param name="capacity">Most players held per frame.</param>
    /// <param name="names">The name cache; its slot count bounds the slots that can be offered.</param>
    /// <param name="npcCapacity">Most named NPCs held per frame, on top of the players; 0 keeps none.</param>
    public PlayerSet(int capacity, NameCache names, int npcCapacity = 0)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(capacity, 1);
        ArgumentOutOfRangeException.ThrowIfNegative(npcCapacity);
        this.players = new NearbyPlayer[capacity + npcCapacity];
        this.playerCapacity = capacity;
        this.npcCapacity = npcCapacity;
        this.names = names;
    }

    /// <summary>How many players and NPCs are inside the cube this frame.</summary>
    public int Count { get; private set; }

    /// <summary>How many of <see cref="Count"/> are NPCs.</summary>
    public int NpcCount { get; private set; }

    /// <summary>How many players or NPCs inside the cube didn't fit this frame.</summary>
    public int Dropped { get; private set; }

    /// <summary>How many players and NPCs were offered this frame, inside the cube or not.</summary>
    public int Offered { get; private set; }

    /// <summary>The players and NPCs inside the cube this frame.</summary>
    public ReadOnlySpan<NearbyPlayer> Items => this.players.AsSpan(0, this.Count);

    /// <summary>The name cache, for the bridge's checks.</summary>
    public NameCache Names => this.names;

    /// <summary>
    /// The relation that picks a player's colour (PROJECT.md, "Colours"). Self wins over party: the local player
    /// is always drawn in its own colour.
    /// </summary>
    /// <param name="isLocal">Whether this is the local player.</param>
    /// <param name="isParty">The game's party-member flag.</param>
    /// <param name="isAlliance">The game's alliance-member flag.</param>
    public static PlayerRelation Classify(bool isLocal, bool isParty, bool isAlliance) =>
        isLocal ? PlayerRelation.Self : (isParty || isAlliance) ? PlayerRelation.Party : PlayerRelation.Other;

    /// <summary>
    /// Whether an event or battle NPC counts as a named NPC (PROJECT.md, "NPCs"; the filter measured in #73,
    /// <c>docs/research-notes.md</c>).
    /// </summary>
    /// <remarks>
    /// Objects owned by a player (pets, chocobo companions) are left out by both their owner and their battle NPC
    /// sub-kind, so either field being unset still leaves them out. Nameless NPCs are the game's invisible event
    /// helpers. The caller has already checked the object kind is an event or battle NPC.
    /// </remarks>
    /// <param name="ownerId">The object's <c>OwnerId</c>; anything but <see cref="NoOwner"/> means it is owned.</param>
    /// <param name="isPetOrBuddy">Whether it is a battle NPC of sub-kind <c>Pet</c> or <c>Buddy</c>.</param>
    /// <param name="nameUtf8">The game's zero-terminated name bytes.</param>
    public static bool IsNamedNpc(uint ownerId, bool isPetOrBuddy, ReadOnlySpan<byte> nameUtf8) =>
        ownerId == NoOwner && !isPetOrBuddy && nameUtf8.Length > 0 && nameUtf8[0] != 0;

    /// <summary>
    /// Whether NPCs are taken this frame (PROJECT.md, "NPCs": the on/off setting). The game reader checks it to skip
    /// the NPC part of the object table altogether; <see cref="OfferNpc"/> rejects every NPC while it's off.
    /// </summary>
    public bool ShowsNpcs { get; private set; }

    /// <summary>Starts a new frame: empties the set and sets the cube players are tested against.</summary>
    /// <param name="cube">This frame's cube.</param>
    /// <param name="npcs">Whether named NPCs are shown this frame (the user's setting).</param>
    public void Begin(in CubeSpace cube, bool npcs = true)
    {
        this.space = cube;
        this.ShowsNpcs = npcs && this.npcCapacity > 0;
        this.Count = 0;
        this.NpcCount = 0;
        this.playerCount = 0;
        this.Dropped = 0;
        this.Offered = 0;
    }

    /// <summary>
    /// Offers one player. Outside the cube: rejected before anything else is done (no name lookup).
    /// Inside: classified, named from the cache and kept.
    /// </summary>
    /// <param name="slot">Object table index (must be below the name cache's slot count).</param>
    /// <param name="entityId">The game's id for the player.</param>
    /// <param name="position">World position.</param>
    /// <param name="isLocal">Whether this is the local player.</param>
    /// <param name="isParty">Party-member flag.</param>
    /// <param name="isAlliance">Alliance-member flag.</param>
    /// <param name="nameUtf8">The game's name bytes.</param>
    /// <returns>True if the player is inside the cube and was kept.</returns>
    public bool Offer(int slot, uint entityId, Vector3 position, bool isLocal, bool isParty, bool isAlliance, ReadOnlySpan<byte> nameUtf8)
    {
        if (!this.Accept(slot, position, this.playerCount == this.playerCapacity))
        {
            return false;
        }

        this.playerCount++;
        var relation = Classify(isLocal, isParty, isAlliance);
        this.players[this.Count++] = new NearbyPlayer(slot, entityId, position, relation, this.names.Get(slot, nameUtf8));
        return true;
    }

    /// <summary>
    /// Offers one named NPC (the caller has applied <see cref="IsNamedNpc"/>). It goes through the same culling as a
    /// player and is kept with the <see cref="PlayerRelation.Npc"/> relation, up to the NPC cap. With NPCs turned off
    /// (<see cref="ShowsNpcs"/>) it is rejected straight away and not even counted as offered.
    /// </summary>
    /// <param name="slot">Object table index (must be below the name cache's slot count).</param>
    /// <param name="entityId">The game's id for the NPC.</param>
    /// <param name="position">World position.</param>
    /// <param name="nameUtf8">The game's name bytes.</param>
    /// <returns>True if the NPC is inside the cube and was kept.</returns>
    public bool OfferNpc(int slot, uint entityId, Vector3 position, ReadOnlySpan<byte> nameUtf8)
    {
        if (!this.ShowsNpcs || !this.Accept(slot, position, this.NpcCount == this.npcCapacity))
        {
            return false;
        }

        this.NpcCount++;
        this.players[this.Count++] = new NearbyPlayer(slot, entityId, position, PlayerRelation.Npc, this.names.Get(slot, nameUtf8));
        return true;
    }

    /// <summary>
    /// The shared first steps of an offer: counts it, culls it, and counts it as dropped if its kind's cap is full.
    /// Culling comes first, so someone outside the cube costs no name lookup.
    /// </summary>
    /// <param name="slot">Object table index.</param>
    /// <param name="position">World position.</param>
    /// <param name="full">Whether this kind's cap is already reached.</param>
    /// <returns>True if the offer should be kept.</returns>
    private bool Accept(int slot, Vector3 position, bool full)
    {
        this.Offered++;
        if (!this.space.Contains(position) || (uint)slot >= (uint)this.names.Slots)
        {
            return false;
        }

        if (full)
        {
            this.Dropped++;
            return false;
        }

        return true;
    }
}
