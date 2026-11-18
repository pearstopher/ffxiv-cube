using System.Numerics;

namespace Cube.Core;

/// <summary>
/// Clips collision boxes to the cube (PROJECT.md, "Collision (phase 2)": each collider inside or crossing the cube is
/// shown as a simplified box, clipped to the cube). Boxes wholly outside are left out; the rest are cut back to the
/// part inside, as a <see cref="ClippedBox"/>.
/// </summary>
/// <remarks>
/// <para>
/// The boxes are world-axis-aligned (<see cref="CollisionBox"/>) but the cube is turned by its yaw, so the overlap of
/// a box and the cube is an upright prism: the height is cut exactly, and sideways the cube's square footprint is cut
/// to the box's footprint (Sutherland–Hodgman against four axis lines), leaving a convex polygon of at most eight
/// corners. That polygon is kept as the prism's footprint, so nothing drawn reaches outside the cube at any yaw (#93).
/// With the cube north up, at any multiple of 90°, or for a box wholly inside the cube's footprint, the polygon is the
/// box's own rectangle and the result is marked <see cref="ClippedBox.IsRectangle"/>, which draws as a plain box.
/// </para>
/// <para>
/// Runs every frame the cube moves, over the cached boxes, so it allocates nothing: the footprint is computed once
/// per call and the polygon buffers are on the stack (PROJECT.md, "Efficiency and memory").
/// </para>
/// </remarks>
public static class CollisionClipper
{
    /// <summary>
    /// Scratch polygon size: room for the duplicate corners a cut makes when a corner lies exactly on a line, before
    /// they are removed.
    /// </summary>
    private const int ScratchCorners = 16;

    /// <summary>
    /// Corners closer than this (in yalms, squared) are one corner, and an edge shorter than it doesn't count as
    /// turned: float noise from the rotation, far below anything visible.
    /// </summary>
    private const float SameCorner = 1e-8f;

    /// <summary>How far off an axis an edge may be and still count as world-aligned, in yalms (rotation rounding).</summary>
    private const float AxisTolerance = 1e-4f;

    /// <summary>
    /// Clips every box to the cube and writes the ones that reach into it, in order, to
    /// <paramref name="destination"/>.
    /// </summary>
    /// <param name="cube">This frame's cube.</param>
    /// <param name="source">The cached world boxes (<see cref="CollisionBoxSet.Items"/>).</param>
    /// <param name="destination">Where the clipped boxes go; boxes beyond its length are not written.</param>
    /// <returns>How many boxes were written.</returns>
    public static int Clip(in CubeSpace cube, ReadOnlySpan<CollisionBox> source, Span<ClippedBox> destination)
    {
        var footprint = new Footprint(cube);
        Span<Vector2> a = stackalloc Vector2[ScratchCorners];
        Span<Vector2> b = stackalloc Vector2[ScratchCorners];
        var count = 0;
        for (var i = 0; i < source.Length && count < destination.Length; i++)
        {
            if (TryClip(cube, footprint, source[i], a, b, out destination[count]))
            {
                count++;
            }
        }

        return count;
    }

    /// <summary>Clips one box to the cube.</summary>
    /// <param name="cube">The cube.</param>
    /// <param name="box">A world box.</param>
    /// <param name="clipped">The part of the box inside the cube, if any.</param>
    /// <returns>False if the box doesn't reach into the cube (touching a face counts as reaching it).</returns>
    public static bool TryClip(in CubeSpace cube, in CollisionBox box, out ClippedBox clipped)
    {
        Span<Vector2> a = stackalloc Vector2[ScratchCorners];
        Span<Vector2> b = stackalloc Vector2[ScratchCorners];
        return TryClip(cube, new Footprint(cube), box, a, b, out clipped);
    }

    /// <summary>
    /// Clips one box: height first (no rotation needed, the cheapest reject), then the footprint.
    /// </summary>
    /// <param name="cube">The cube.</param>
    /// <param name="footprint">The cube's footprint corners in world X/Z.</param>
    /// <param name="box">The box.</param>
    /// <param name="a">Scratch polygon buffer.</param>
    /// <param name="b">Second scratch polygon buffer.</param>
    /// <param name="clipped">The clipped box.</param>
    private static bool TryClip(in CubeSpace cube, in Footprint footprint, in CollisionBox box, Span<Vector2> a, Span<Vector2> b, out ClippedBox clipped)
    {
        clipped = default;
        var minY = MathF.Max(box.Min.Y, cube.Centre.Y - cube.Half.Y);
        var maxY = MathF.Min(box.Max.Y, cube.Centre.Y + cube.Half.Y);
        if (minY > maxY)
        {
            return false;
        }

        // Cheap reject on the footprint's bounds before cutting polygons.
        if (box.Max.X < footprint.Min.X || box.Min.X > footprint.Max.X || box.Max.Z < footprint.Min.Y || box.Min.Z > footprint.Max.Y)
        {
            return false;
        }

        if (footprint.IsAxisAligned)
        {
            // North up or any multiple of 90°: the footprint is its own bounds, so the overlap is a plain box and no
            // polygon is needed (the common case in North up mode).
            clipped = new ClippedBox(new CollisionBox(
                new Vector3(MathF.Max(box.Min.X, footprint.Min.X), minY, MathF.Max(box.Min.Z, footprint.Min.Y)),
                new Vector3(MathF.Min(box.Max.X, footprint.Max.X), maxY, MathF.Min(box.Max.Z, footprint.Max.Y))));
            return true;
        }

        a[0] = footprint.C0;
        a[1] = footprint.C1;
        a[2] = footprint.C2;
        a[3] = footprint.C3;
        var n = 4;
        n = Cut(a, n, b, axis: 0, box.Min.X, keepAbove: true);
        n = Cut(b, n, a, axis: 0, box.Max.X, keepAbove: false);
        n = Cut(a, n, b, axis: 1, box.Min.Z, keepAbove: true);
        n = Cut(b, n, a, axis: 1, box.Max.Z, keepAbove: false);
        if (n == 0)
        {
            return false;
        }

        var lo = a[0];
        var hi = a[0];
        for (var i = 1; i < n; i++)
        {
            lo = Vector2.Min(lo, a[i]);
            hi = Vector2.Max(hi, a[i]);
        }

        // The polygon lies inside the box's footprint up to rounding; clamp so the result never pokes past the box.
        var boxLo = new Vector2(box.Min.X, box.Min.Z);
        var boxHi = new Vector2(box.Max.X, box.Max.Z);
        lo = Vector2.Max(lo, boxLo);
        hi = Vector2.Min(hi, boxHi);
        var bounds = new CollisionBox(new Vector3(lo.X, minY, lo.Y), new Vector3(hi.X, maxY, hi.Y));

        var polygon = a[..n];
        for (var i = 0; i < n; i++)
        {
            polygon[i] = Vector2.Clamp(polygon[i], boxLo, boxHi);
        }

        n = RemoveDuplicates(polygon);
        polygon = polygon[..n];
        var area = SignedArea(polygon);
        if (n < 3 || MathF.Abs(area) <= SameCorner || IsAxisAlignedRectangle(polygon))
        {
            // No turned edge (or no area: a wall of zero thickness, or a box touching the cube along a line): the
            // bounds are exact.
            clipped = new ClippedBox(bounds);
            return true;
        }

        if (area < 0)
        {
            polygon.Reverse();
        }

        clipped = new ClippedBox(polygon[..Math.Min(n, ClippedBox.MaxCorners)], bounds, isRectangle: false);
        return true;
    }

    /// <summary>
    /// One Sutherland–Hodgman step: keeps the part of a convex polygon on one side of an axis line (inclusive).
    /// </summary>
    /// <param name="input">The polygon's corners.</param>
    /// <param name="count">How many corners it has.</param>
    /// <param name="output">Where the cut polygon goes.</param>
    /// <param name="axis">0 cuts along X (a line of constant X), 1 along Z.</param>
    /// <param name="line">The line's X or Z.</param>
    /// <param name="keepAbove">True keeps coordinates ≥ the line, false keeps ≤.</param>
    /// <returns>How many corners the cut polygon has.</returns>
    private static int Cut(ReadOnlySpan<Vector2> input, int count, Span<Vector2> output, int axis, float line, bool keepAbove)
    {
        var written = 0;
        for (var i = 0; i < count; i++)
        {
            var current = input[i];
            var next = input[(i + 1) % count];
            var c = (axis == 0 ? current.X : current.Y) - line;
            var d = (axis == 0 ? next.X : next.Y) - line;
            var currentIn = keepAbove ? c >= 0 : c <= 0;
            var nextIn = keepAbove ? d >= 0 : d <= 0;
            if (currentIn && written < output.Length)
            {
                output[written++] = current;
            }

            if (currentIn != nextIn && written < output.Length)
            {
                var t = c / (c - d);
                var p = current + ((next - current) * t);

                // Put the crossing exactly on the line, so a box of zero thickness keeps an exact face.
                output[written++] = axis == 0 ? new Vector2(line, p.Y) : new Vector2(p.X, line);
            }
        }

        return written;
    }

    /// <summary>
    /// Drops corners that repeat the one before them (a cut through an existing corner adds it twice), including the
    /// last against the first.
    /// </summary>
    /// <param name="polygon">The corners, compacted in place.</param>
    /// <returns>How many corners are left.</returns>
    private static int RemoveDuplicates(Span<Vector2> polygon)
    {
        var n = 0;
        for (var i = 0; i < polygon.Length; i++)
        {
            if (n == 0 || Vector2.DistanceSquared(polygon[i], polygon[n - 1]) > SameCorner)
            {
                polygon[n++] = polygon[i];
            }
        }

        while (n > 1 && Vector2.DistanceSquared(polygon[n - 1], polygon[0]) <= SameCorner)
        {
            n--;
        }

        return n;
    }

    /// <summary>The signed area of a polygon (shoelace); positive when the corners go counter-clockwise.</summary>
    /// <param name="polygon">The corners.</param>
    /// <remarks>
    /// Measured from the first corner: world coordinates are in the hundreds, and the products of such numbers lose
    /// the small difference in float, so a sliver cut from the cube's corner read as zero area and was drawn as its
    /// whole bounding rectangle, which pokes out of the cube (#115).
    /// </remarks>
    private static float SignedArea(ReadOnlySpan<Vector2> polygon)
    {
        var sum = 0f;
        var origin = polygon.IsEmpty ? Vector2.Zero : polygon[0];
        for (var i = 0; i < polygon.Length; i++)
        {
            var p = polygon[i] - origin;
            var q = polygon[(i + 1) % polygon.Length] - origin;
            sum += (p.X * q.Y) - (q.X * p.Y);
        }

        return sum / 2f;
    }

    /// <summary>Whether a polygon is four corners joined by world-axis-aligned edges: a rectangle equal to its bounds.</summary>
    /// <param name="polygon">The corners.</param>
    private static bool IsAxisAlignedRectangle(ReadOnlySpan<Vector2> polygon)
    {
        if (polygon.Length != 4)
        {
            return false;
        }

        for (var i = 0; i < 4; i++)
        {
            var edge = polygon[(i + 1) % 4] - polygon[i];
            if (MathF.Abs(edge.X) > AxisTolerance && MathF.Abs(edge.Y) > AxisTolerance)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// The cube's rectangular footprint in world X/Z (stored as <see cref="Vector2"/> X and Y), with its bounds for the
    /// cheap reject. Computed once per clip pass.
    /// </summary>
    private readonly struct Footprint
    {
        /// <summary>Works out the four corners from the cube's alignment.</summary>
        /// <param name="cube">The cube.</param>
        public Footprint(in CubeSpace cube)
        {
            var x = cube.Half.X;
            var z = cube.Half.Z;
            this.C0 = Flat(cube.FromView(new Vector3(-x, 0, -z)));
            this.C1 = Flat(cube.FromView(new Vector3(x, 0, -z)));
            this.C2 = Flat(cube.FromView(new Vector3(x, 0, z)));
            this.C3 = Flat(cube.FromView(new Vector3(-x, 0, z)));
            this.Min = Vector2.Min(Vector2.Min(this.C0, this.C1), Vector2.Min(this.C2, this.C3));
            this.Max = Vector2.Max(Vector2.Max(this.C0, this.C1), Vector2.Max(this.C2, this.C3));
            var side = this.C1 - this.C0;
            this.IsAxisAligned = MathF.Abs(side.X) <= AxisTolerance || MathF.Abs(side.Y) <= AxisTolerance;
        }

        /// <summary>Whether the footprint's sides run along the world axes (the yaw is a multiple of 90°).</summary>
        public bool IsAxisAligned { get; }

        /// <summary>First corner, in order around the rectangle.</summary>
        public Vector2 C0 { get; }

        /// <summary>Second corner.</summary>
        public Vector2 C1 { get; }

        /// <summary>Third corner.</summary>
        public Vector2 C2 { get; }

        /// <summary>Fourth corner.</summary>
        public Vector2 C3 { get; }

        /// <summary>Lowest world X and Z of the footprint.</summary>
        public Vector2 Min { get; }

        /// <summary>Highest world X and Z of the footprint.</summary>
        public Vector2 Max { get; }

        /// <summary>A world point's X and Z.</summary>
        /// <param name="world">The point.</param>
        private static Vector2 Flat(Vector3 world) => new(world.X, world.Z);
    }
}
