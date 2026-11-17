using System.Numerics;

namespace Cube.Snapshot;

/// <summary>
/// An RGBA image in memory that lines and filled polygons can be drawn onto, with alpha blending like ImGui's.
/// Just enough rasterising to see what the plugin drew; not a general graphics library.
/// </summary>
/// <remarks>
/// Polygons are filled by testing each pixel centre in their bounding box against every edge (they are convex:
/// cube faces). Lines are drawn as thin quads, so both go through the same fill.
/// </remarks>
public sealed class Canvas
{
    /// <summary>Pixels as RGBA bytes, row by row from the top.</summary>
    private readonly byte[] pixels;

    /// <summary>Creates a canvas filled with one colour.</summary>
    /// <param name="width">Width in pixels.</param>
    /// <param name="height">Height in pixels.</param>
    /// <param name="background">Fill colour, packed <c>0xAABBGGRR</c> like the draw list.</param>
    public Canvas(int width, int height, uint background)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(width, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(height, 1);
        this.Width = width;
        this.Height = height;
        this.pixels = new byte[width * height * 4];
        for (var i = 0; i < width * height; i++)
        {
            this.pixels[(i * 4) + 0] = (byte)background;
            this.pixels[(i * 4) + 1] = (byte)(background >> 8);
            this.pixels[(i * 4) + 2] = (byte)(background >> 16);
            this.pixels[(i * 4) + 3] = (byte)(background >> 24);
        }
    }

    /// <summary>Width in pixels.</summary>
    public int Width { get; }

    /// <summary>Height in pixels.</summary>
    public int Height { get; }

    /// <summary>The RGBA bytes, row by row from the top.</summary>
    public ReadOnlySpan<byte> Pixels => this.pixels;

    /// <summary>Reads one pixel as packed <c>0xAABBGGRR</c>.</summary>
    /// <param name="x">Column.</param>
    /// <param name="y">Row.</param>
    public uint GetPixel(int x, int y)
    {
        var i = ((y * this.Width) + x) * 4;
        return this.pixels[i] | ((uint)this.pixels[i + 1] << 8) | ((uint)this.pixels[i + 2] << 16) | ((uint)this.pixels[i + 3] << 24);
    }

    /// <summary>Fills a convex polygon, in either winding order.</summary>
    /// <param name="points">Corners in order round the outline.</param>
    /// <param name="colour">Packed <c>0xAABBGGRR</c>; its alpha blends over what's there.</param>
    public void FillConvex(ReadOnlySpan<Vector2> points, uint colour)
    {
        if (points.Length < 3)
        {
            return;
        }

        var min = points[0];
        var max = points[0];
        foreach (var p in points)
        {
            min = Vector2.Min(min, p);
            max = Vector2.Max(max, p);
        }

        var x0 = Math.Max(0, (int)MathF.Floor(min.X));
        var y0 = Math.Max(0, (int)MathF.Floor(min.Y));
        var x1 = Math.Min(this.Width - 1, (int)MathF.Ceiling(max.X));
        var y1 = Math.Min(this.Height - 1, (int)MathF.Ceiling(max.Y));

        for (var y = y0; y <= y1; y++)
        {
            for (var x = x0; x <= x1; x++)
            {
                if (Inside(points, new Vector2(x + 0.5f, y + 0.5f)))
                {
                    this.Blend(x, y, colour);
                }
            }
        }
    }

    /// <summary>Draws a line as a quad of the given thickness (at least one pixel, so thin lines don't vanish).</summary>
    /// <param name="a">Start.</param>
    /// <param name="b">End.</param>
    /// <param name="colour">Packed colour.</param>
    /// <param name="thickness">Width in pixels.</param>
    public void DrawLine(Vector2 a, Vector2 b, uint colour, float thickness)
    {
        var along = b - a;
        var length = along.Length();
        if (length < 1e-6f)
        {
            return;
        }

        var half = MathF.Max(thickness, 1f) / 2f;
        var normal = new Vector2(-along.Y, along.X) / length * half;
        Span<Vector2> quad = [a + normal, b + normal, b - normal, a - normal];
        this.FillConvex(quad, colour);
    }

    /// <summary>Whether a point is inside a convex polygon: on the same side of every edge.</summary>
    /// <param name="points">The polygon.</param>
    /// <param name="p">The point.</param>
    private static bool Inside(ReadOnlySpan<Vector2> points, Vector2 p)
    {
        var positive = false;
        var negative = false;
        for (var i = 0; i < points.Length; i++)
        {
            var a = points[i];
            var b = points[(i + 1) % points.Length];
            var cross = ((b.X - a.X) * (p.Y - a.Y)) - ((b.Y - a.Y) * (p.X - a.X));
            positive |= cross > 0;
            negative |= cross < 0;
            if (positive && negative)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>Blends a colour over one pixel ("source over", like ImGui).</summary>
    /// <param name="x">Column.</param>
    /// <param name="y">Row.</param>
    /// <param name="colour">Packed colour.</param>
    private void Blend(int x, int y, uint colour)
    {
        var i = ((y * this.Width) + x) * 4;
        var alpha = (colour >> 24) / 255f;
        for (var c = 0; c < 3; c++)
        {
            var source = (colour >> (c * 8)) & 0xFF;
            this.pixels[i + c] = (byte)MathF.Round((source * alpha) + (this.pixels[i + c] * (1 - alpha)));
        }

        this.pixels[i + 3] = (byte)MathF.Round(255 * (alpha + ((this.pixels[i + 3] / 255f) * (1 - alpha))));
    }
}
