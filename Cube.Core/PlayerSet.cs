using System.Numerics;

namespace Cube.Core;

/// <summary>
/// The players inside the cube this frame (PROJECT.md, "Players": every player character in the object table whose
/// position is inside the cube; players outside are not shown at all). The game reader offers every player it
/// finds; this class culls, classifies and names them.
/// </summary>
/// <remarks>
/// Keeping the rules here, not in the game reader, means they are unit tested without the game, and the Debug
/// fake players (#16) go through exactly the same path. The buffer is fixed-size and reused (PROJECT.md,
/// "Efficiency and memory"); a full set drops extra players and counts them in <see cref="Dropped"/>.
/// </remarks>
public sealed class PlayerSet
{
    /// <summary>The reused buffer.</summary>
    private readonly NearbyPlayer[] players;

    /// <summary>Names, cached per slot.</summary>
    private readonly NameCache names;

    /// <summary>The cube for this frame, set by <see cref="Begin"/>.</summary>
    private CubeSpace space;

    /// <summary>Creates an empty set.</summary>
    /// <param name="capacity">Most players held per frame.</param>
    /// <param name="names">The name cache; its slot count bounds the slots that can be offered.</param>
    public PlayerSet(int capacity, NameCache names)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(capacity, 1);
        this.players = new NearbyPlayer[capacity];
        this.names = names;
    }

    /// <summary>How many players are inside the cube this frame.</summary>
    public int Count { get; private set; }

    /// <summary>How many players inside the cube didn't fit this frame.</summary>
    public int Dropped { get; private set; }

    /// <summary>How many players were offered this frame, inside the cube or not.</summary>
    public int Offered { get; private set; }

    /// <summary>The players inside the cube this frame.</summary>
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

    /// <summary>Starts a new frame: empties the set and sets the cube players are tested against.</summary>
    /// <param name="cube">This frame's cube.</param>
    public void Begin(in CubeSpace cube)
    {
        this.space = cube;
        this.Count = 0;
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
        this.Offered++;
        if (!this.space.Contains(position) || (uint)slot >= (uint)this.names.Slots)
        {
            return false;
        }

        if (this.Count == this.players.Length)
        {
            this.Dropped++;
            return false;
        }

        var relation = Classify(isLocal, isParty, isAlliance);
        this.players[this.Count++] = new NearbyPlayer(slot, entityId, position, relation, this.names.Get(slot, nameUtf8));
        return true;
    }
}
