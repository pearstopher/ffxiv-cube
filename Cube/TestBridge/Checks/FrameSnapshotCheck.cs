using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using Cube.Core;

namespace Cube.TestBridge.Checks;

/// <summary>
/// Dumps one frame's draw list to <c>drawlist.json</c> in the bridge folder (PROJECT.md, "Automated testing": the
/// bridge dumps the frame's draw list; the snapshot tool turns it into a PNG so Claude can see what was drawn).
/// </summary>
/// <remarks>
/// The format is read by <c>Cube.Snapshot.SnapshotRenderer</c>. It contains only geometry and colours; player names
/// are never written (PROJECT.md, "Privacy"). Written atomically, like the results.
/// </remarks>
/// <param name="folder">The bridge folder to write into.</param>
internal sealed class FrameSnapshotCheck(string folder) : BridgeCheck
{
    /// <summary>Name of the dump file.</summary>
    public const string FileName = "drawlist.json";

    /// <summary>Frames to wait for the window to draw after opening it.</summary>
    private const int FramesToWait = 30;

    /// <summary>The window's open state before the check, restored by <see cref="Cleanup"/>.</summary>
    private bool wasOpen;

    /// <inheritdoc/>
    public override string Name => "frame-snapshot";

    /// <inheritdoc/>
    public override IEnumerable<CheckStep> Run(BridgeContext context)
    {
        var window = context.Window;
        this.wasOpen = window.IsOpen;
        window.IsOpen = true;

        // The standard fakes make sure the snapshot shows player cubes even with nobody around (#16).
        window.Fakes.UseStandardSet();

        var start = window.DrawCost.TotalRecorded;
        var waited = 0;
        while (window.DrawCost.TotalRecorded - start < 2)
        {
            if (++waited > FramesToWait)
            {
                yield return CheckStep.Fail($"Window didn't draw within {FramesToWait} frames (game UI hidden?).");
                yield break;
            }

            yield return CheckStep.NextFrame;
        }

        var path = Path.Combine(folder, FileName);
        AtomicFile.WriteAllText(path, Dump(window));
        context.Metric("primitives", window.DrawList.Count);
        context.Metric("width", window.LastRegionSize.X);
        context.Metric("height", window.LastRegionSize.Y);
        yield return CheckStep.Pass($"Wrote {window.DrawList.Count} primitives to {path}.");
    }

    /// <inheritdoc/>
    public override void Cleanup(BridgeContext context)
    {
        context.Window.Fakes.Clear();
        context.Window.IsOpen = this.wasOpen;
    }

    /// <summary>Serialises the window's last frame.</summary>
    /// <param name="window">The cube window.</param>
    private static string Dump(CubeWindow window)
    {
        using var stream = new MemoryStream();
        using (var json = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true }))
        {
            json.WriteStartObject();
            WritePoint(json, "regionMin", window.LastRegionMin);
            WritePoint(json, "regionSize", window.LastRegionSize);
            json.WriteStartArray("primitives");
            foreach (var p in window.DrawList.Items)
            {
                json.WriteStartObject();
                json.WriteString("kind", p.Kind == PrimitiveKind.Line ? "line" : "quad");
                json.WriteStartArray("points");
                WritePoint(json, null, p.A);
                WritePoint(json, null, p.B);
                if (p.Kind == PrimitiveKind.Quad)
                {
                    WritePoint(json, null, p.C);
                    WritePoint(json, null, p.D);
                }

                json.WriteEndArray();
                json.WriteNumber("colour", p.Colour);
                json.WriteNumber("depth", p.Depth);
                if (p.Kind == PrimitiveKind.Line)
                {
                    json.WriteNumber("thickness", CubeWindow.LineThickness);
                }

                json.WriteEndObject();
            }

            json.WriteEndArray();
            json.WriteEndObject();
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }

    /// <summary>Writes <c>[x, y]</c>, as a named property or an array element.</summary>
    /// <param name="json">The writer.</param>
    /// <param name="name">Property name, or null inside an array.</param>
    /// <param name="point">The point.</param>
    private static void WritePoint(Utf8JsonWriter json, string? name, System.Numerics.Vector2 point)
    {
        if (name is null)
        {
            json.WriteStartArray();
        }
        else
        {
            json.WriteStartArray(name);
        }

        json.WriteNumberValue(point.X);
        json.WriteNumberValue(point.Y);
        json.WriteEndArray();
    }
}
