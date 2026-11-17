using System.Numerics;
using System.Text.Json;
using Cube.Core;

namespace Cube.Snapshot;

/// <summary>
/// Renders a draw-list dump (<c>drawlist.json</c>, written by the bridge's <c>frame-snapshot</c> check) onto a
/// <see cref="Canvas"/>, in the dump's order: the list is already sorted far to near, so painting in order
/// reproduces what ImGui drew.
/// </summary>
/// <remarks>
/// Dump format: <c>regionMin</c> and <c>regionSize</c> (the window's drawing region in screen pixels) and
/// <c>primitives</c>, each with <c>kind</c> (<c>line</c> or <c>quad</c>), <c>points</c> (<c>[[x, y], ...]</c> in
/// screen pixels), <c>colour</c> (<c>0xAABBGGRR</c> as a number) and, for lines, <c>thickness</c>.
/// </remarks>
public static class SnapshotRenderer
{
    /// <summary>Background colour: close to ImGui's default dark window background.</summary>
    public const uint Background = 0xFF1E1E1E;

    /// <summary>Line thickness used when a dumped line doesn't say.</summary>
    public const float DefaultThickness = 1.5f;

    /// <summary>Renders a dump to a canvas the size of the dumped region.</summary>
    /// <param name="json">The dump's JSON text.</param>
    public static Canvas Render(string json)
    {
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;
        var regionMin = ReadPoint(root.GetProperty("regionMin"));
        var regionSize = ReadPoint(root.GetProperty("regionSize"));

        var canvas = NewCanvas(regionSize);
        Span<Vector2> points = stackalloc Vector2[4];
        foreach (var primitive in root.GetProperty("primitives").EnumerateArray())
        {
            var count = 0;
            foreach (var point in primitive.GetProperty("points").EnumerateArray())
            {
                if (count < points.Length)
                {
                    points[count++] = ReadPoint(point) - regionMin;
                }
            }

            var colour = primitive.GetProperty("colour").GetUInt32();
            switch (primitive.GetProperty("kind").GetString())
            {
                case "line" when count >= 2:
                    var thickness = primitive.TryGetProperty("thickness", out var t) ? t.GetSingle() : DefaultThickness;
                    canvas.DrawLine(points[0], points[1], colour, thickness);
                    break;
                case "quad" when count >= 3:
                    canvas.FillConvex(points[..count], colour);
                    break;
            }
        }

        return canvas;
    }

    /// <summary>
    /// Paints Core's primitives directly, in order. Used by <c>--demo</c> to check the look of Core's output
    /// without the game.
    /// </summary>
    /// <param name="regionMin">Screen position of the region's top-left, subtracted from every point.</param>
    /// <param name="regionSize">Region size: the canvas size.</param>
    /// <param name="items">The sorted primitives.</param>
    public static Canvas Paint(Vector2 regionMin, Vector2 regionSize, ReadOnlySpan<DrawPrimitive> items)
    {
        var canvas = NewCanvas(regionSize);
        foreach (ref readonly var p in items)
        {
            if (p.Kind == PrimitiveKind.Line)
            {
                canvas.DrawLine(p.A - regionMin, p.B - regionMin, p.Colour, DefaultThickness);
            }
            else
            {
                canvas.FillConvex([p.A - regionMin, p.B - regionMin, p.C - regionMin, p.D - regionMin], p.Colour);
            }
        }

        return canvas;
    }

    /// <summary>A background-filled canvas of the region's size (at least 1×1).</summary>
    /// <param name="regionSize">Region size in pixels.</param>
    private static Canvas NewCanvas(Vector2 regionSize) =>
        new(Math.Max(1, (int)MathF.Round(regionSize.X)), Math.Max(1, (int)MathF.Round(regionSize.Y)), Background);

    /// <summary>Reads <c>[x, y]</c>.</summary>
    /// <param name="element">A two-number JSON array.</param>
    private static Vector2 ReadPoint(JsonElement element) => new(element[0].GetSingle(), element[1].GetSingle());
}
