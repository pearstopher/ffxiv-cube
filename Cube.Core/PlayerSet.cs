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

    /// <summary>Most interactable objects kept per frame (#112).</summary>
    private readonly int interactableCapacity;

    /// <summary>Whether friendly NPCs are taken this frame (<see cref="Begin"/>).</summary>
    private bool showNpcs;

    /// <summary>Whether hostile NPCs are taken this frame.</summary>
    private bool showEnemies;

    /// <summary>Whether other players are taken this frame.</summary>
    private bool showPlayers;

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
    /// <param name="interactableCapacity">Most interactable objects held per frame, on top of both; 0 keeps none (#112).</param>
    public PlayerSet(int capacity, NameCache names, int npcCapacity = 0, int interactableCapacity = 0)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(capacity, 1);
        ArgumentOutOfRangeException.ThrowIfNegative(npcCapacity);
        ArgumentOutOfRangeException.ThrowIfNegative(interactableCapacity);
        this.interactableCapacity = interactableCapacity;
        this.players = new NearbyPlayer[capacity + npcCapacity + interactableCapacity];
        this.playerCapacity = capacity;
        this.npcCapacity = npcCapacity;
        this.names = names;
        this.markers = new MarkerLabels(names.Slots);
    }

    /// <summary>Label text with the target and attacking markers (#111), cached per slot.</summary>
    private readonly MarkerLabels markers;

    /// <summary>The marker cache, for tests and the bridge.</summary>
    public MarkerLabels Markers => this.markers;

    /// <summary>How many players and NPCs are inside the cube this frame.</summary>
    public int Count { get; private set; }

    /// <summary>How many of <see cref="Count"/> are NPCs.</summary>
    public int NpcCount { get; private set; }

    /// <summary>How many of <see cref="Count"/> are interactable objects (#112).</summary>
    public int InteractableCount { get; private set; }

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
    /// The relation for a named NPC (#111): a hostile battle NPC is an enemy, anything else a plain NPC. Event NPCs are
    /// never enemies, whatever their flags say.
    /// </summary>
    /// <param name="isBattleNpc">Whether the object is a battle NPC (not an event NPC).</param>
    /// <param name="isHostile">The game's hostile flag (<c>Character.IsHostile</c>).</param>
    public static PlayerRelation ClassifyNpc(bool isBattleNpc, bool isHostile) =>
        isBattleNpc && isHostile ? PlayerRelation.Enemy : PlayerRelation.Npc;

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

    /// <summary>
    /// Whether interactable objects are taken this frame (#112). The game reader checks it to skip the object part of the
    /// table altogether; <see cref="OfferInteractable"/> rejects every one while it's off.
    /// </summary>
    public bool ShowsInteractables { get; private set; }

    /// <summary>
    /// Whether an object counts as a text-labelled interactable (#112; <c>docs/research-notes.md</c>): an event object
    /// with a name. Nameless event objects are the invisible triggers and icon-only markers (arrows); housing furniture
    /// and gardening plants are a different object kind and never reach here. The caller has checked the kind.
    /// </summary>
    /// <param name="nameUtf8">The game's zero-terminated name bytes.</param>
    public static bool IsTextInteractable(ReadOnlySpan<byte> nameUtf8) => nameUtf8.Length > 0 && nameUtf8[0] != 0;

    /// <summary>Starts a new frame: empties the set and sets the cube players are tested against.</summary>
    /// <param name="cube">This frame's cube.</param>
    /// <param name="npcs">Whether friendly named NPCs are shown this frame (the user's setting).</param>
    /// <param name="players">Whether other players are shown (#112); the local player always is.</param>
    /// <param name="enemies">Whether hostile NPCs are shown (#112).</param>
    /// <param name="interactables">Whether interactable objects are shown (#112).</param>
    public void Begin(in CubeSpace cube, bool npcs = true, bool players = true, bool enemies = true, bool interactables = true)
    {
        this.space = cube;
        this.showNpcs = npcs;
        this.showEnemies = enemies;
        this.showPlayers = players;
        this.ShowsNpcs = (npcs || enemies) && this.npcCapacity > 0;
        this.ShowsInteractables = interactables && this.interactableCapacity > 0;
        this.InteractableCount = 0;
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
    /// <param name="isTarget">Whether this is the local player's current target (#111: gets a <c>[T]</c> marker).</param>
    /// <returns>True if the player is inside the cube and was kept.</returns>
    public bool Offer(int slot, uint entityId, Vector3 position, bool isLocal, bool isParty, bool isAlliance, ReadOnlySpan<byte> nameUtf8, bool isTarget = false)
    {
        if (isLocal)
        {
            // While the player jumps the cube is held at the take-off height (#105), so the player can leave the box
            // vertically. Their own marker is always drawn, kept at the face it would cross (PROJECT.md, "Jump").
            // x and z follow the player, so only the height can be out. It is put a hair inside the face (0.01%), so
            // the inclusive containment test can't reject it through rounding.
            var offset = position.Y - this.space.Centre.Y;
            var half = this.space.Half.Y;
            if (MathF.Abs(offset) > half)
            {
                position.Y = this.space.Centre.Y + (MathF.Sign(offset) * half * 0.9999f);
            }
        }

        if (!isLocal && !this.showPlayers)
        {
            return false;
        }

        if (!this.Accept(slot, position, this.playerCount == this.playerCapacity))
        {
            return false;
        }

        this.playerCount++;
        var relation = Classify(isLocal, isParty, isAlliance);
        var name = this.names.Get(slot, nameUtf8);
        this.players[this.Count++] = new NearbyPlayer(slot, entityId, position, relation, name, this.markers.Get(slot, isTarget, false, name));
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
    /// <param name="isEnemy">Whether it is a hostile battle NPC (#111: kept as <see cref="PlayerRelation.Enemy"/>).</param>
    /// <param name="isTarget">Whether it is the local player's current target (<c>[T]</c> marker).</param>
    /// <param name="isAttacking">Whether it is attacking the local player (<c>[!]</c> marker); only enemies are marked.</param>
    /// <returns>True if the NPC is inside the cube and was kept.</returns>
    public bool OfferNpc(int slot, uint entityId, Vector3 position, ReadOnlySpan<byte> nameUtf8, bool isEnemy = false, bool isTarget = false, bool isAttacking = false)
    {
        if (!this.ShowsNpcs || !(isEnemy ? this.showEnemies : this.showNpcs) || !this.Accept(slot, position, this.NpcCount == this.npcCapacity))
        {
            return false;
        }

        this.NpcCount++;
        var name = this.names.Get(slot, nameUtf8);
        var relation = isEnemy ? PlayerRelation.Enemy : PlayerRelation.Npc;
        this.players[this.Count++] = new NearbyPlayer(slot, entityId, position, relation, name, this.markers.Get(slot, isTarget, isEnemy && isAttacking, name));
        return true;
    }

    /// <summary>
    /// Offers one text-labelled interactable object (the caller has applied <see cref="IsTextInteractable"/>; #112). It is
    /// culled like a player and kept as <see cref="PlayerRelation.Interactable"/>, up to its own cap, so a crowd of
    /// objects never pushes out players or NPCs. Rejected straight away while the toggle is off.
    /// </summary>
    /// <param name="slot">Object table index (must be below the name cache's slot count).</param>
    /// <param name="entityId">The game's id for the object.</param>
    /// <param name="position">World position.</param>
    /// <param name="nameUtf8">The game's name bytes.</param>
    /// <param name="isTarget">Whether it is the local player's current target (<c>[T]</c> marker).</param>
    /// <returns>True if the object is inside the cube and was kept.</returns>
    public bool OfferInteractable(int slot, uint entityId, Vector3 position, ReadOnlySpan<byte> nameUtf8, bool isTarget = false)
    {
        if (!this.ShowsInteractables || !this.Accept(slot, position, this.InteractableCount == this.interactableCapacity))
        {
            return false;
        }

        this.InteractableCount++;
        var name = this.names.Get(slot, nameUtf8);
        this.players[this.Count++] = new NearbyPlayer(slot, entityId, position, PlayerRelation.Interactable, name, this.markers.Get(slot, isTarget, false, name));
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
