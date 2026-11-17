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
/// <c>primitives</c>, each with <c>kind</c> (<c>line</c>, <c>quad</c> or <c>label</c>), <c>points</c>
/// (<c>[[x, y], ...]</c> in screen pixels; one anchor point for a label), <c>colour</c> (<c>0xAABBGGRR</c> as a
/// number), for lines <c>thickness</c>, and for labels <c>chars</c> and <c>fontSize</c>.
/// </remarks>
public static class SnapshotRenderer
{
    /// <summary>Background colour: close to ImGui's default dark window background.</summary>
    public const uint Background = 0xFF1E1E1E;

    /// <summary>Line thickness used when a dumped line doesn't say.</summary>
    public const float DefaultThickness = 1.5f;

    /// <summary>Label length assumed when a dumped label doesn't say (and in the demo, which has no names).</summary>
    public const int DefaultLabelChars = 8;

    /// <summary>Text height assumed when a dumped label doesn't say: ImGui's default font size.</summary>
    public const float DefaultFontSize = 13f;

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
                case "label" when count >= 1:
                    var chars = primitive.TryGetProperty("chars", out var c) ? c.GetInt32() : DefaultLabelChars;
                    var fontSize = primitive.TryGetProperty("fontSize", out var f) ? f.GetSingle() : DefaultFontSize;
                    FillLabelBar(canvas, points[0], chars, fontSize, colour);
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
            switch (p.Kind)
            {
                case PrimitiveKind.Line:
                    canvas.DrawLine(p.A - regionMin, p.B - regionMin, p.Colour, DefaultThickness);
                    break;
                case PrimitiveKind.Quad:
                    canvas.FillConvex([p.A - regionMin, p.B - regionMin, p.C - regionMin, p.D - regionMin], p.Colour);
                    break;
                default:
                    FillLabelBar(canvas, p.A - regionMin, DefaultLabelChars, DefaultFontSize, p.Colour);
                    break;
            }
        }

        return canvas;
    }

    /// <summary>
    /// Draws a label as a bar where the text would be: the snapshot has no font, and the dump never contains names
    /// (PROJECT.md, "Privacy"), so the bar shows where each label sits and roughly how long it is.
    /// </summary>
    /// <param name="canvas">Target.</param>
    /// <param name="anchor">The label's left end, vertically centred (region pixels).</param>
    /// <param name="chars">Name length in characters.</param>
    /// <param name="fontSize">Text height in pixels.</param>
    /// <param name="colour">Text colour.</param>
    private static void FillLabelBar(Canvas canvas, Vector2 anchor, int chars, float fontSize, uint colour)
    {
        var halfHeight = fontSize * 0.3f;
        var width = chars * fontSize * 0.5f;
        canvas.FillConvex(
            [
                new Vector2(anchor.X, anchor.Y - halfHeight),
                new Vector2(anchor.X + width, anchor.Y - halfHeight),
                new Vector2(anchor.X + width, anchor.Y + halfHeight),
                new Vector2(anchor.X, anchor.Y + halfHeight),
            ],
            colour);
    }

    /// <summary>A background-filled canvas of the region's size (at least 1×1).</summary>
    /// <param name="regionSize">Region size in pixels.</param>
    private static Canvas NewCanvas(Vector2 regionSize) =>
        new(Math.Max(1, (int)MathF.Round(regionSize.X)), Math.Max(1, (int)MathF.Round(regionSize.Y)), Background);

    /// <summary>Reads <c>[x, y]</c>.</summary>
    /// <param name="element">A two-number JSON array.</param>
    private static Vector2 ReadPoint(JsonElement element) => new(element[0].GetSingle(), element[1].GetSingle());
}
