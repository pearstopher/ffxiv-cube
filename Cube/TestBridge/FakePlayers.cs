using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Cube.Core;

namespace Cube.TestBridge;

/// <summary>
/// Synthetic players and NPCs for in-game checks (PROJECT.md, "Automated testing": the bridge can inject synthetic
/// players so rendering checks work even when nobody is nearby). Debug builds only: this file lives in
/// <c>TestBridge/</c>, which Release builds exclude.
/// </summary>
/// <remarks>
/// Fakes are offered to the same <see cref="PlayerSet"/> as real players and NPCs, after them, so they go through
/// exactly the same culling, classification and naming; fake NPCs use <see cref="PlayerSet.OfferNpc"/>, so they are
/// left out when the NPC setting is off, like real ones. They use object slots from <see cref="FirstSlot"/> up (above the whole
/// real object table) and entity ids from <see cref="FirstEntityId"/> up, so checks can tell them apart. Positions are
/// fractions of the cube's half-size on each axis in cube-local coordinates (see <see cref="CubeSpace.FromView"/>), so the same set
/// works at any cube size and stays put in the cube however the camera turns. A fake added with
/// <see cref="AddAtWorld"/> is instead fixed in the world, like a real player, so it rotates with the contents.
/// </remarks>
internal sealed class FakePlayers
{
    /// <summary>First object slot used by fakes, just above the real object table.</summary>
    public const int FirstSlot = Game.PlayerReader.ObjectSlots;

    /// <summary>Entity ids for fakes start here; the game never uses this range for players.</summary>
    public const uint FirstEntityId = 0xFA000000;

    /// <summary>The fakes currently enabled; empty means none are injected.</summary>
    private readonly List<Fake> fakes = [];

    /// <summary>How many fakes are enabled.</summary>
    public int Count => this.fakes.Count;

    /// <summary>
    /// The standard set covering the cases PROJECT.md cares about: near the centre, on a face (inside), on a corner
    /// (inside), two overlapping, above and below, one just outside (never shown), and an NPC inside and outside
    /// (PROJECT.md, "NPCs": shown like players, in the NPC colour).
    /// </summary>
    public static IReadOnlyList<(string Name, Vector3 Offset, PlayerRelation Relation)> Standard { get; } =
    [
        ("Fake Near", new Vector3(0.25f, 0, 0.3f), PlayerRelation.Other),
        // 0.999 rather than 1: centre + offset − centre can round a hair past the face in game coordinates.
        // Exact on-face inclusivity is unit tested in Core (CubeSpaceTests).
        ("Fake Face", new Vector3(0.999f, 0, 0), PlayerRelation.Party),
        ("Fake Corner", new Vector3(-0.999f, 0.999f, -0.999f), PlayerRelation.Other),
        ("Fake Overlap A", new Vector3(-0.4f, 0, 0.5f), PlayerRelation.Other),
        ("Fake Overlap B", new Vector3(-0.38f, 0, 0.5f), PlayerRelation.Party),
        ("Fake Above", new Vector3(0, 0.6f, -0.5f), PlayerRelation.Other),
        ("Fake Below", new Vector3(0.5f, -0.6f, -0.2f), PlayerRelation.Other),
        ("Fake Outside", new Vector3(1.2f, 0, 0), PlayerRelation.Other),
        ("Fake NPC", new Vector3(-0.6f, 0.2f, -0.3f), PlayerRelation.Npc),
        ("Fake NPC Outside", new Vector3(0, 0, -1.3f), PlayerRelation.Npc),
    ];

    /// <summary>How many of <see cref="Standard"/> are inside the cube (all but the two named "... Outside").</summary>
    public static int StandardInside => Standard.Count - 2;

    /// <summary>How many of <see cref="Standard"/> are NPCs inside the cube.</summary>
    public static int StandardNpcsInside => 1;

    /// <summary>
    /// How many of <see cref="Standard"/> reach the cube with the NPC setting on or off: the fake NPC is skipped when
    /// it's off, like a real one.
    /// </summary>
    /// <param name="npcs">The NPC setting.</param>
    public static int StandardArriving(bool npcs) => StandardInside - (npcs ? 0 : StandardNpcsInside);

    /// <summary>Whether a standard fake's name marks it as outside the cube.</summary>
    /// <param name="name">The fake's name.</param>
    public static bool IsOutside(string name) => name.EndsWith(" Outside", System.StringComparison.Ordinal);

    /// <summary>Enables the <see cref="Standard"/> set, replacing any fakes.</summary>
    public void UseStandardSet()
    {
        this.Clear();
        foreach (var (name, offset, relation) in Standard)
        {
            this.Add(name, offset, relation);
        }
    }

    /// <summary>
    /// Enables a crowd of fakes inside the cube, replacing any: by default <see cref="Game.PlayerReader.MaxPlayers"/>
    /// players and <see cref="Game.PlayerReader.MaxNpcs"/> NPCs, the most the cube can hold, so the efficiency checks
    /// measure the worst case (#23, #74).
    /// </summary>
    /// <remarks>
    /// Players are spread on a 5×4×5 grid through the cube, alternating party and other; NPCs on a 5×2×5 grid offset
    /// half a step from it. Each has a 21-character name (about the longest the game allows), so every one gets a cube
    /// and a full-length label.
    /// </remarks>
    /// <param name="count">How many fake players, at most <see cref="Game.PlayerReader.MaxPlayers"/>.</param>
    /// <param name="npcs">How many fake NPCs, at most <see cref="Game.PlayerReader.MaxNpcs"/>.</param>
    public void UseCrowd(int count = Game.PlayerReader.MaxPlayers, int npcs = Game.PlayerReader.MaxNpcs)
    {
        this.Clear();
        for (var i = 0; i < count; i++)
        {
            var offset = new Vector3(((i % 5) * 0.45f) - 0.9f, (((i / 5) % 4) * 0.5f) - 0.75f, ((i / 20 % 5) * 0.45f) - 0.9f);
            this.Add($"Fake Crowd Member {i:000}", offset, i % 2 == 0 ? PlayerRelation.Other : PlayerRelation.Party);
        }

        for (var i = 0; i < npcs; i++)
        {
            var offset = new Vector3(((i % 5) * 0.4f) - 0.775f, (((i / 5) % 2) * 1f) - 0.5f, ((i / 10 % 5) * 0.4f) - 0.775f);
            this.Add($"Fake Crowd Npc No {i:000}", offset, PlayerRelation.Npc);
        }
    }

    /// <summary>Adds one fake.</summary>
    /// <param name="name">Its label.</param>
    /// <param name="offset">Position as fractions of the half-size on each axis from the cube centre (1 = on a face).</param>
    /// <param name="relation">Party, other or NPC (fakes are never Self).</param>
    public void Add(string name, Vector3 offset, PlayerRelation relation) =>
        this.fakes.Add(MakeFake(name, offset, relation, false));

    /// <summary>
    /// Adds one fake at a fixed world position, like a real player standing still, so it rotates with the cube's
    /// contents when the camera turns (#62).
    /// </summary>
    /// <param name="name">Its label.</param>
    /// <param name="world">Its world position.</param>
    /// <param name="relation">Party, other or NPC (fakes are never Self).</param>
    public void AddAtWorld(string name, Vector3 world, PlayerRelation relation) =>
        this.fakes.Add(MakeFake(name, world, relation, true));

    /// <summary>Removes all fakes.</summary>
    public void Clear() => this.fakes.Clear();

    /// <summary>Offers every fake to the frame's player set. Allocation-free: names were encoded when added.</summary>
    /// <param name="set">The frame's player set, already begun.</param>
    /// <param name="space">The frame's cube.</param>
    public void Offer(PlayerSet set, in CubeSpace space)
    {
        for (var i = 0; i < this.fakes.Count; i++)
        {
            var fake = this.fakes[i];
            var position = fake.IsWorld ? fake.Offset : space.FromView(fake.Offset * space.Half);
            if (fake.Relation is PlayerRelation.Npc or PlayerRelation.Enemy)
            {
                // Rejected by the set while NPCs are turned off, like a real NPC. Fakes whose names contain "Target" or
                // "Attacker" stand in for the real target and enemy-list reads (#111).
                var isEnemy = fake.Relation == PlayerRelation.Enemy;
                set.OfferNpc(FirstSlot + i, FirstEntityId + (uint)i, position, fake.Name, isEnemy, fake.IsTarget, fake.IsAttacker);
            }
            else
            {
                set.Offer(FirstSlot + i, FirstEntityId + (uint)i, position, false, fake.Relation == PlayerRelation.Party, false, fake.Name, fake.IsTarget);
            }
        }
    }

    /// <summary>One fake player or NPC.</summary>
    /// <param name="Name">UTF-8 name bytes, encoded once.</param>
    /// <param name="Offset">Position as fractions of the half-size on each axis in cube-local coordinates, or a world position if <paramref name="IsWorld"/>.</param>
    /// <param name="Relation">Party, other or NPC.</param>
    /// <param name="IsWorld">Whether <paramref name="Offset"/> is a fixed world position.</param>
    /// <param name="IsTarget">Whether it stands in for the current target (name contains "Target"; #111).</param>
    /// <param name="IsAttacker">Whether it stands in for an attacking enemy (name contains "Attacker"; #111).</param>
    private readonly record struct Fake(byte[] Name, Vector3 Offset, PlayerRelation Relation, bool IsWorld, bool IsTarget, bool IsAttacker);

    /// <summary>Builds a fake; the markers it shows come from its name, worked out once here so offering allocates nothing.</summary>
    /// <param name="name">Its label.</param>
    /// <param name="offset">Position (see <see cref="Fake"/>).</param>
    /// <param name="relation">Its relation.</param>
    /// <param name="isWorld">Whether <paramref name="offset"/> is a world position.</param>
    private static Fake MakeFake(string name, Vector3 offset, PlayerRelation relation, bool isWorld) => new(
        Encoding.UTF8.GetBytes(name),
        offset,
        relation,
        isWorld,
        name.Contains("Target", System.StringComparison.Ordinal),
        name.Contains("Attacker", System.StringComparison.Ordinal));
}
