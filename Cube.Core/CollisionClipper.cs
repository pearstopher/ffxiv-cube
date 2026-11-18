using System.Numerics;

namespace Cube.Core;

/// <summary>
/// Clips collision boxes to the cube (PROJECT.md, "Collision (phase 2)": each collider inside or crossing the cube is
/// shown as a simplified box, clipped to the cube). Boxes wholly outside are left out; the rest are cut back to the
/// part inside.
/// </summary>
/// <remarks>
/// <para>
/// The boxes are world-axis-aligned (<see cref="CollisionBox"/>) but the cube is turned by its yaw, so the exact
/// overlap of a box and the cube is usually not a box. The clipper keeps the spec's "simplified box" and makes it as
/// tight as a world-aligned box can be: the height is cut exactly, and sideways the cube's square footprint is cut
/// to the box's footprint (Sutherland–Hodgman against four axis lines, at most eight corners), and the clipped box
/// is the bounds of what remains. With the cube north up, or at any multiple of 90°, that is the exact overlap; at
/// other yaws a box can reach past a side of the cube by at most the corner triangle the box's own corner cuts off.
/// </para>
/// <para>
/// Runs every frame over the cached boxes, so it allocates nothing: the footprint is computed once per call and the
/// polygon buffers are on the stack (PROJECT.md, "Efficiency and memory").
/// </para>
/// </remarks>
public static class CollisionClipper
{
    /// <summary>Most corners a clipped footprint can have: a square cut by four lines.</summary>
    private const int MaxCorners = 8;

    /// <summary>
    /// Clips every box to the cube and writes the ones that reach into it, in order, to
    /// <paramref name="destination"/>.
    /// </summary>
    /// <param name="cube">This frame's cube.</param>
    /// <param name="source">The cached world boxes (<see cref="CollisionBoxSet.Items"/>).</param>
    /// <param name="destination">Where the clipped boxes go; boxes beyond its length are not written.</param>
    /// <returns>How many boxes were written.</returns>
    public static int Clip(in CubeSpace cube, ReadOnlySpan<CollisionBox> source, Span<CollisionBox> destination)
    {
        var footprint = new Footprint(cube);
        Span<Vector2> a = stackalloc Vector2[MaxCorners];
        Span<Vector2> b = stackalloc Vector2[MaxCorners];
        var count = 0;
        for (var i = 0; i < source.Length && count < destination.Length; i++)
        {
            if (TryClip(cube, footprint, source[i], a, b, out var clipped))
            {
                destination[count++] = clipped;
            }
        }

        return count;
    }

    /// <summary>Clips one box to the cube.</summary>
    /// <param name="cube">The cube.</param>
    /// <param name="box">A world box.</param>
    /// <param name="clipped">The part of the box inside the cube, as a world box, if any.</param>
    /// <returns>False if the box doesn't reach into the cube (touching a face counts as reaching it).</returns>
    public static bool TryClip(in CubeSpace cube, in CollisionBox box, out CollisionBox clipped)
    {
        Span<Vector2> a = stackalloc Vector2[MaxCorners];
        Span<Vector2> b = stackalloc Vector2[MaxCorners];
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
    private static bool TryClip(in CubeSpace cube, in Footprint footprint, in CollisionBox box, Span<Vector2> a, Span<Vector2> b, out CollisionBox clipped)
    {
        clipped = default;
        var minY = MathF.Max(box.Min.Y, cube.Centre.Y - cube.HalfEdge);
        var maxY = MathF.Min(box.Max.Y, cube.Centre.Y + cube.HalfEdge);
        if (minY > maxY)
        {
            return false;
        }

        // Cheap reject on the footprint's bounds before cutting polygons.
        if (box.Max.X < footprint.Min.X || box.Min.X > footprint.Max.X || box.Max.Z < footprint.Min.Y || box.Min.Z > footprint.Max.Y)
        {
            return false;
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
        lo = Vector2.Max(lo, new Vector2(box.Min.X, box.Min.Z));
        hi = Vector2.Min(hi, new Vector2(box.Max.X, box.Max.Z));
        clipped = new CollisionBox(new Vector3(lo.X, minY, lo.Y), new Vector3(hi.X, maxY, hi.Y));
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
    /// The cube's square footprint in world X/Z (stored as <see cref="Vector2"/> X and Y), with its bounds for the
    /// cheap reject. Computed once per clip pass.
    /// </summary>
    private readonly struct Footprint
    {
        /// <summary>Works out the four corners from the cube's alignment.</summary>
        /// <param name="cube">The cube.</param>
        public Footprint(in CubeSpace cube)
        {
            var h = cube.HalfEdge;
            this.C0 = Flat(cube.FromView(new Vector3(-h, 0, -h)));
            this.C1 = Flat(cube.FromView(new Vector3(h, 0, -h)));
            this.C2 = Flat(cube.FromView(new Vector3(h, 0, h)));
            this.C3 = Flat(cube.FromView(new Vector3(-h, 0, h)));
            this.Min = Vector2.Min(Vector2.Min(this.C0, this.C1), Vector2.Min(this.C2, this.C3));
            this.Max = Vector2.Max(Vector2.Max(this.C0, this.C1), Vector2.Max(this.C2, this.C3));
        }

        /// <summary>First corner, in order around the square.</summary>
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
