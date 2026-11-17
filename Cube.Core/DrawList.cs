using System.Numerics;

namespace Cube.Core;

/// <summary>
/// The frame's primitives, in window pixels, sorted far to near (PROJECT.md, "Draw order": painter's algorithm,
/// so near cubes cover far ones). Built by <see cref="CubeGeometry"/>, drawn by the plugin, and dumped by the
/// test bridge.
/// </summary>
/// <remarks>
/// The buffer is allocated once with a fixed capacity and reused every frame (PROJECT.md, "Efficiency and memory":
/// no per-frame allocations, bounded memory). If a frame needs more, the extra primitives are dropped and counted
/// in <see cref="Dropped"/> rather than growing the buffer.
/// </remarks>
public sealed class DrawList
{
    /// <summary>
    /// Sorts far first, then by insertion order. A cached delegate: sorting with an <see cref="IComparer{T}"/>
    /// would wrap it in a new delegate on every call, which allocates each frame.
    /// </summary>
    private static readonly Comparison<DrawPrimitive> BackToFront = CompareBackToFront;

    /// <summary>The reused buffer.</summary>
    private readonly DrawPrimitive[] items;

    /// <summary>Creates an empty list.</summary>
    /// <param name="capacity">The most primitives a frame can hold.</param>
    public DrawList(int capacity)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(capacity, 1);
        this.items = new DrawPrimitive[capacity];
    }

    /// <summary>How many primitives the list can hold.</summary>
    public int Capacity => this.items.Length;

    /// <summary>How many primitives were added this frame (at most <see cref="Capacity"/>).</summary>
    public int Count { get; private set; }

    /// <summary>How many primitives were dropped this frame because the list was full.</summary>
    public int Dropped { get; private set; }

    /// <summary>This frame's primitives, in drawing order once <see cref="SortBackToFront"/> has run.</summary>
    public ReadOnlySpan<DrawPrimitive> Items => this.items.AsSpan(0, this.Count);

    /// <summary>Empties the list for a new frame, keeping the buffer.</summary>
    public void Clear()
    {
        this.Count = 0;
        this.Dropped = 0;
    }

    /// <summary>Adds a line.</summary>
    /// <param name="a">Start (pixels).</param>
    /// <param name="b">End (pixels).</param>
    /// <param name="depth">Distance from the viewer.</param>
    /// <param name="colour">Packed colour.</param>
    public void AddLine(Vector2 a, Vector2 b, float depth, uint colour) =>
        this.Add(new DrawPrimitive { Kind = PrimitiveKind.Line, A = a, B = b, Depth = depth, Colour = colour });

    /// <summary>Adds a filled quad; points go round the outline in order.</summary>
    /// <param name="a">First corner (pixels).</param>
    /// <param name="b">Second corner.</param>
    /// <param name="c">Third corner.</param>
    /// <param name="d">Fourth corner.</param>
    /// <param name="depth">Distance from the viewer.</param>
    /// <param name="colour">Packed colour.</param>
    /// <param name="tag">Owner: 0 for the scene, player index + 1 for a player's cube (<see cref="DrawPrimitive.Tag"/>).</param>
    public void AddQuad(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float depth, uint colour, int tag = 0) =>
        this.Add(new DrawPrimitive { Kind = PrimitiveKind.Quad, A = a, B = b, C = c, D = d, Depth = depth, Colour = colour, Tag = tag });

    /// <summary>Adds a player's name label (<see cref="PrimitiveKind.Label"/>).</summary>
    /// <param name="anchor">Left end of the label, vertically centred on it (pixels).</param>
    /// <param name="depth">Distance from the viewer: the player cube's, so the label sorts with its cube.</param>
    /// <param name="colour">Text colour.</param>
    /// <param name="tag">The player's index + 1; the text is that player's name.</param>
    public void AddLabel(Vector2 anchor, float depth, uint colour, int tag) =>
        this.Add(new DrawPrimitive { Kind = PrimitiveKind.Label, A = anchor, Depth = depth, Colour = colour, Tag = tag });

    /// <summary>Sorts so the farthest primitive comes first; ties keep the order they were added in.</summary>
    public void SortBackToFront() => this.items.AsSpan(0, this.Count).Sort(BackToFront);

    /// <summary>Appends a primitive, or counts it as dropped when full.</summary>
    /// <param name="primitive">The primitive; its sequence number is set here.</param>
    private void Add(DrawPrimitive primitive)
    {
        if (this.Count == this.items.Length)
        {
            this.Dropped++;
            return;
        }

        primitive.Sequence = this.Count;
        this.items[this.Count++] = primitive;
    }

    /// <summary>Orders by depth descending, then by sequence ascending, so the sort is stable.</summary>
    /// <param name="x">One primitive.</param>
    /// <param name="y">The other.</param>
    private static int CompareBackToFront(DrawPrimitive x, DrawPrimitive y)
    {
        var byDepth = y.Depth.CompareTo(x.Depth);
        return byDepth != 0 ? byDepth : x.Sequence.CompareTo(y.Sequence);
    }
}
