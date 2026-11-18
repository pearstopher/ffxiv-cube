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
    /// <summary>The reused buffer.</summary>
    private readonly DrawPrimitive[] items;

    /// <summary>The reused sort keys, one per primitive (<see cref="SortKey"/>).</summary>
    private readonly long[] keys;

    /// <summary>
    /// The reused corner buffer for polygons (<see cref="PrimitiveKind.Polygon"/>). Corners stay where they were added
    /// while the primitives are sorted, since each polygon refers to its own range.
    /// </summary>
    private readonly Vector2[] points;

    /// <summary>How many of <see cref="points"/> are used this frame.</summary>
    private int pointCount;

    /// <summary>
    /// The largest point buffer: a polygon's start is stored in 16 bits (<see cref="DrawPrimitive.FirstPoint"/>) to
    /// keep primitives small.
    /// </summary>
    public const int MaxPointCapacity = ushort.MaxValue + 1;

    /// <summary>Creates an empty list.</summary>
    /// <param name="capacity">The most primitives a frame can hold.</param>
    /// <param name="pointCapacity">
    /// The most polygon corners a frame can hold, up to <see cref="MaxPointCapacity"/>; 0 (the default) gives room for
    /// one <see cref="ClippedBox.MaxCorners"/>-corner polygon per eight primitives.
    /// </param>
    public DrawList(int capacity, int pointCapacity = 0)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(capacity, 1);
        ArgumentOutOfRangeException.ThrowIfNegative(pointCapacity);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(pointCapacity, MaxPointCapacity);
        this.items = new DrawPrimitive[capacity];
        this.keys = new long[capacity];
        this.points = new Vector2[pointCapacity > 0 ? pointCapacity : Math.Clamp(capacity, ClippedBox.MaxCorners, MaxPointCapacity)];
    }

    /// <summary>How many primitives the list can hold.</summary>
    public int Capacity => this.items.Length;

    /// <summary>How many polygon corners the list can hold.</summary>
    public int PointCapacity => this.points.Length;

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
        this.pointCount = 0;
    }

    /// <summary>A polygon's corners, in order around it (pixels); empty for other kinds.</summary>
    /// <param name="primitive">A primitive from <see cref="Items"/>.</param>
    public ReadOnlySpan<Vector2> Points(in DrawPrimitive primitive) =>
        primitive.Kind == PrimitiveKind.Polygon ? this.points.AsSpan(primitive.FirstPoint, primitive.Corners) : default;

    /// <summary>
    /// Adds a filled convex polygon (<see cref="PrimitiveKind.Polygon"/>); its corners are copied into the list's
    /// point buffer. Dropped like any other primitive when either buffer is full.
    /// </summary>
    /// <param name="corners">3 to <see cref="ClippedBox.MaxCorners"/> corners in order around the outline (pixels).</param>
    /// <param name="depth">Distance from the viewer.</param>
    /// <param name="colour">Packed colour.</param>
    /// <param name="tag">Owner (<see cref="DrawPrimitive.Tag"/>).</param>
    public void AddPolygon(ReadOnlySpan<Vector2> corners, float depth, uint colour, int tag = 0)
    {
        if (corners.Length < 3 || corners.Length > byte.MaxValue || this.pointCount + corners.Length > this.points.Length || this.Count == this.items.Length)
        {
            this.Dropped++;
            return;
        }

        var first = this.pointCount;
        corners.CopyTo(this.points.AsSpan(first));
        this.pointCount += corners.Length;
        this.Add(new DrawPrimitive { Kind = PrimitiveKind.Polygon, Corners = (byte)corners.Length, FirstPoint = (ushort)first, A = corners[0], Depth = depth, Colour = colour, Tag = tag });
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
    /// <param name="tag">Owner: 0 for the scene, player index + 1 for a player's cube, negative for a collision box (<see cref="DrawPrimitive.Tag"/>).</param>
    public void AddQuad(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float depth, uint colour, int tag = 0) =>
        this.Add(new DrawPrimitive { Kind = PrimitiveKind.Quad, A = a, B = b, C = c, D = d, Depth = depth, Colour = colour, Tag = tag });

    /// <summary>Adds a player's name label (<see cref="PrimitiveKind.Label"/>).</summary>
    /// <param name="anchor">Left end of the label, vertically centred on it (pixels).</param>
    /// <param name="depth">Distance from the viewer: the player cube's, so the label sorts with its cube.</param>
    /// <param name="colour">Text colour.</param>
    /// <param name="tag">The player's index + 1; the text is that player's name.</param>
    /// <param name="centred">
    /// True for a label centred above its cube (#112): <paramref name="anchor"/> is then the middle of the text's bottom
    /// edge. Stored as <see cref="DrawPrimitive.B"/>.X = 1 (unused by labels otherwise), so the primitive stays the same size.
    /// </param>
    public void AddLabel(Vector2 anchor, float depth, uint colour, int tag, bool centred = false) =>
        this.Add(new DrawPrimitive { Kind = PrimitiveKind.Label, A = anchor, B = centred ? Vector2.UnitX : default, Depth = depth, Colour = colour, Tag = tag });

    /// <summary>Sorts so the farthest primitive comes first; ties keep the order they were added in.</summary>
    /// <remarks>
    /// Sorts on one 64-bit key per primitive (<see cref="SortKey"/>), carrying the primitives along, rather than
    /// comparing primitives through a delegate: with collision boxes a frame holds a few thousand primitives, and
    /// comparing plain numbers is several times cheaper. The order is exactly <see cref="CompareBackToFront"/>'s.
    /// </remarks>
    public void SortBackToFront()
    {
        var items = this.items.AsSpan(0, this.Count);
        var keys = this.keys.AsSpan(0, this.Count);
        for (var i = 0; i < items.Length; i++)
        {
            keys[i] = SortKey(items[i]);
        }

        keys.Sort(items);
    }

    /// <summary>
    /// A number that orders primitives far first, then by <see cref="DrawPrimitive.Sequence"/>: the depth's bits made
    /// to sort like the float and inverted (so larger depths come first) in the high half, the sequence in the low.
    /// </summary>
    /// <param name="primitive">The primitive.</param>
    public static long SortKey(in DrawPrimitive primitive)
    {
        // +0 and -0 compare equal as floats; adding 0 turns -0 into +0 so they get the same key too.
        var bits = BitConverter.SingleToInt32Bits(primitive.Depth + 0f);
        var ordered = bits >= 0 ? bits : bits ^ int.MaxValue;
        return ((long)~ordered << 32) | (uint)primitive.Sequence;
    }

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

    /// <summary>
    /// The drawing order spelled out: depth descending, then sequence ascending, so the sort is stable.
    /// <see cref="SortKey"/> encodes the same order as one number; the unit tests check the two agree.
    /// </summary>
    /// <param name="x">One primitive.</param>
    /// <param name="y">The other.</param>
    public static int CompareBackToFront(in DrawPrimitive x, in DrawPrimitive y)
    {
        var byDepth = y.Depth.CompareTo(x.Depth);
        return byDepth != 0 ? byDepth : x.Sequence.CompareTo(y.Sequence);
    }
}
