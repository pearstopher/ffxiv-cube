using System.Numerics;

namespace Cube.Core;

/// <summary>
/// Clips a line segment to the cube in cube-local coordinates (#100; PROJECT.md, "Collision style": everything is
/// clipped exactly to the cube). The Mesh style draws triangle <em>edges</em>, so clipping segments (Liang–Barsky)
/// is all it needs: unlike clipping whole triangles it never invents an edge along the cube's face.
/// </summary>
/// <remarks>
/// Cube-local coordinates have the cube centred on the origin (<see cref="CubeSpace.ToView"/>), so its faces are the
/// planes ±<c>half</c> on each axis. A segment wholly inside is returned untouched, so uncut edges keep their exact
/// corners; a cut end is moved onto the face it crosses, then clamped into the box so rounding can never leave it a
/// hair outside.
/// </remarks>
public static class SegmentClip
{
    /// <summary>Which of the six faces' outsides a point is beyond, as bits (X low, X high, Y low, Y high, Z low, Z high).</summary>
    /// <param name="p">A cube-local point.</param>
    /// <param name="half">The cube's half extents.</param>
    public static int Outcode(Vector3 p, Vector3 half) =>
        (p.X < -half.X ? 1 : 0) | (p.X > half.X ? 2 : 0) |
        (p.Y < -half.Y ? 4 : 0) | (p.Y > half.Y ? 8 : 0) |
        (p.Z < -half.Z ? 16 : 0) | (p.Z > half.Z ? 32 : 0);

    /// <summary>Clips a segment to the box, moving its ends in.</summary>
    /// <param name="a">One end; replaced by the clipped end.</param>
    /// <param name="b">The other end; replaced by the clipped end.</param>
    /// <param name="half">The cube's half extents.</param>
    /// <returns>False if no part of the segment is inside the box (the ends are then unchanged).</returns>
    public static bool Clip(ref Vector3 a, ref Vector3 b, Vector3 half) =>
        Clip(ref a, ref b, half, Outcode(a, half), Outcode(b, half));

    /// <summary>As <see cref="Clip(ref Vector3, ref Vector3, Vector3)"/> when the ends' outcodes are already known.</summary>
    /// <param name="a">One end; replaced by the clipped end.</param>
    /// <param name="b">The other end; replaced by the clipped end.</param>
    /// <param name="half">The cube's half extents.</param>
    /// <param name="codeA">The outcode of <paramref name="a"/>.</param>
    /// <param name="codeB">The outcode of <paramref name="b"/>.</param>
    /// <returns>False if no part of the segment is inside the box.</returns>
    public static bool Clip(ref Vector3 a, ref Vector3 b, Vector3 half, int codeA, int codeB)
    {
        if ((codeA & codeB) != 0)
        {
            return false;
        }

        if ((codeA | codeB) == 0)
        {
            return true;
        }

        var d = b - a;
        float t0 = 0f, t1 = 1f;
        if (!Slab(a.X, d.X, half.X, ref t0, ref t1) ||
            !Slab(a.Y, d.Y, half.Y, ref t0, ref t1) ||
            !Slab(a.Z, d.Z, half.Z, ref t0, ref t1))
        {
            return false;
        }

        var newA = a + (d * t0);
        var newB = a + (d * t1);
        a = Vector3.Clamp(newA, -half, half);
        b = Vector3.Clamp(newB, -half, half);
        return true;
    }

    /// <summary>Narrows the parameter range to the part of the segment inside one axis's slab.</summary>
    /// <param name="start">The segment's start on this axis.</param>
    /// <param name="delta">The segment's change along this axis.</param>
    /// <param name="half">The slab's half extent.</param>
    /// <param name="t0">The range's start, raised as needed.</param>
    /// <param name="t1">The range's end, lowered as needed.</param>
    /// <returns>False if the range became empty.</returns>
    private static bool Slab(float start, float delta, float half, ref float t0, ref float t1)
    {
        if (delta == 0f)
        {
            return start >= -half && start <= half;
        }

        var inv = 1f / delta;
        var near = (-half - start) * inv;
        var far = (half - start) * inv;
        if (near > far)
        {
            (near, far) = (far, near);
        }

        t0 = MathF.Max(t0, near);
        t1 = MathF.Min(t1, far);
        return t0 <= t1;
    }
}
