using System.Numerics;

namespace Cube.Core;

/// <summary>
/// The big cube's outline on screen: the convex hull of its eight projected corners. Used to check that nothing drawn
/// for the contents reaches outside the cube (PROJECT.md, "Collision (phase 2)": clipped to the cube; #93).
/// </summary>
/// <remarks>
/// The projection is orthographic, so a point inside the cube projects inside this outline; the reverse test is what
/// the unit tests and the in-game bridge use on every collision corner. Built once per check, then each test is a
/// few cross products. A struct with its corners stored inline: nothing is allocated.
/// </remarks>
public readonly struct CubeOutline
{
    /// <summary>The hull's corners in screen pixels, in order around it.</summary>
    private readonly FootprintCorners hull;

    /// <summary>How many hull corners there are (4 to 6 for a box seen from any angle, fewer if degenerate).</summary>
    private readonly int count;

    /// <summary>Projects the cube's corners and keeps their convex hull.</summary>
    /// <param name="space">The cube.</param>
    /// <param name="projection">The view's projection.</param>
    public CubeOutline(in CubeSpace space, in ViewProjection projection)
    {
        Span<Vector2> corners = stackalloc Vector2[8];
        var half = space.Half;
        for (var i = 0; i < 8; i++)
        {
            var view = new Vector3((i & 1) != 0 ? half.X : -half.X, (i & 2) != 0 ? half.Y : -half.Y, (i & 4) != 0 ? half.Z : -half.Z);
            corners[i] = projection.ToScreen(view);
        }

        // Andrew's monotone chain: sort, then build the lower and upper chains.
        corners.Sort(static (a, b) => a.X != b.X ? a.X.CompareTo(b.X) : a.Y.CompareTo(b.Y));
        Span<Vector2> chain = stackalloc Vector2[16];
        var k = 0;
        for (var i = 0; i < 8; i++)
        {
            while (k >= 2 && Cross(chain[k - 2], chain[k - 1], corners[i]) <= 0)
            {
                k--;
            }

            chain[k++] = corners[i];
        }

        for (int i = 6, lower = k + 1; i >= 0; i--)
        {
            while (k >= lower && Cross(chain[k - 2], chain[k - 1], corners[i]) <= 0)
            {
                k--;
            }

            chain[k++] = corners[i];
        }

        this.count = Math.Min(k - 1, ClippedBox.MaxCorners);
        for (var i = 0; i < this.count; i++)
        {
            this.hull[i] = chain[i];
        }
    }

    /// <summary>Whether a screen point is inside the outline or within a tolerance of it.</summary>
    /// <param name="point">A point in screen pixels.</param>
    /// <param name="tolerance">How far outside still counts, in pixels (float rounding).</param>
    public bool Contains(Vector2 point, float tolerance)
    {
        if (this.count < 3)
        {
            return false;
        }

        for (var i = 0; i < this.count; i++)
        {
            var a = this.hull[i];
            var b = this.hull[i + 1 == this.count ? 0 : i + 1];
            var length = Vector2.Distance(a, b);

            // The hull goes counter-clockwise in (x, y); a point on the right of an edge is outside it.
            if (length > 0 && Cross(a, b, point) / length < -tolerance)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>The z of (b − a) × (c − a): positive when a, b, c turn counter-clockwise.</summary>
    /// <param name="a">First point.</param>
    /// <param name="b">Second point.</param>
    /// <param name="c">Third point.</param>
    private static float Cross(Vector2 a, Vector2 b, Vector2 c) => ((b.X - a.X) * (c.Y - a.Y)) - ((b.Y - a.Y) * (c.X - a.X));
}
