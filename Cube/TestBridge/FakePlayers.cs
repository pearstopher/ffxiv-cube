using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Cube.Core;

namespace Cube.TestBridge;

/// <summary>
/// Synthetic players for in-game checks (PROJECT.md, "Automated testing": the bridge can inject synthetic players so
/// rendering checks work even when nobody is nearby). Debug builds only: this file lives in <c>TestBridge/</c>,
/// which Release builds exclude.
/// </summary>
/// <remarks>
/// Fakes are offered to the same <see cref="PlayerSet"/> as real players, after them, so they go through exactly the
/// same culling, classification and naming. They use object slots from <see cref="FirstSlot"/> up (above the 200 real
/// character slots) and entity ids from <see cref="FirstEntityId"/> up, so checks can tell them apart. Positions are
/// fractions of the cube's half-edge in cube-local coordinates (see <see cref="CubeSpace.FromView"/>), so the same set
/// works at any cube size and stays put in the cube however the camera turns. A fake added with
/// <see cref="AddAtWorld"/> is instead fixed in the world, like a real player, so it rotates with the contents.
/// </remarks>
internal sealed class FakePlayers
{
    /// <summary>First object slot used by fakes, just above the real character slots.</summary>
    public const int FirstSlot = Game.PlayerReader.CharacterSlots;

    /// <summary>Entity ids for fakes start here; the game never uses this range for players.</summary>
    public const uint FirstEntityId = 0xFA000000;

    /// <summary>The fakes currently enabled; empty means none are injected.</summary>
    private readonly List<Fake> fakes = [];

    /// <summary>How many fakes are enabled.</summary>
    public int Count => this.fakes.Count;

    /// <summary>
    /// The standard set covering the cases PROJECT.md cares about: near the centre, on a face (inside), on a corner
    /// (inside), two overlapping, above and below, and one just outside (never shown).
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
    ];

    /// <summary>How many of <see cref="Standard"/> are inside the cube (all but "Fake Outside").</summary>
    public static int StandardInside => Standard.Count - 1;

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
    /// Enables a crowd of fakes inside the cube, replacing any: by default <see cref="Game.PlayerReader.MaxPlayers"/>,
    /// the most players the game can have around, so the efficiency checks measure the worst case (#23).
    /// </summary>
    /// <remarks>
    /// Spread on a 5×4×5 grid through the cube, alternating party and other, each with a 21-character name (about
    /// the longest the game allows), so every one gets a cube and a full-length label.
    /// </remarks>
    /// <param name="count">How many fakes, at most <see cref="Game.PlayerReader.MaxPlayers"/>.</param>
    public void UseCrowd(int count = Game.PlayerReader.MaxPlayers)
    {
        this.Clear();
        for (var i = 0; i < count; i++)
        {
            var offset = new Vector3(((i % 5) * 0.45f) - 0.9f, (((i / 5) % 4) * 0.5f) - 0.75f, ((i / 20 % 5) * 0.45f) - 0.9f);
            this.Add($"Fake Crowd Member {i:000}", offset, i % 2 == 0 ? PlayerRelation.Other : PlayerRelation.Party);
        }
    }

    /// <summary>Adds one fake.</summary>
    /// <param name="name">Its label.</param>
    /// <param name="offset">Position as fractions of the half-edge from the cube centre (1 = on a face).</param>
    /// <param name="relation">Party or other (fakes are never Self).</param>
    public void Add(string name, Vector3 offset, PlayerRelation relation) =>
        this.fakes.Add(new Fake(Encoding.UTF8.GetBytes(name), offset, relation == PlayerRelation.Party, false));

    /// <summary>
    /// Adds one fake at a fixed world position, like a real player standing still, so it rotates with the cube's
    /// contents when the camera turns (#62).
    /// </summary>
    /// <param name="name">Its label.</param>
    /// <param name="world">Its world position.</param>
    /// <param name="relation">Party or other (fakes are never Self).</param>
    public void AddAtWorld(string name, Vector3 world, PlayerRelation relation) =>
        this.fakes.Add(new Fake(Encoding.UTF8.GetBytes(name), world, relation == PlayerRelation.Party, true));

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
            var position = fake.IsWorld ? fake.Offset : space.FromView(fake.Offset * space.HalfEdge);
            set.Offer(FirstSlot + i, FirstEntityId + (uint)i, position, false, fake.IsParty, false, fake.Name);
        }
    }

    /// <summary>One fake player.</summary>
    /// <param name="Name">UTF-8 name bytes, encoded once.</param>
    /// <param name="Offset">Position as fractions of the half-edge in cube-local coordinates, or a world position if <paramref name="IsWorld"/>.</param>
    /// <param name="IsParty">Whether it is flagged as a party member.</param>
    /// <param name="IsWorld">Whether <paramref name="Offset"/> is a fixed world position.</param>
    private readonly record struct Fake(byte[] Name, Vector3 Offset, bool IsParty, bool IsWorld);
}
