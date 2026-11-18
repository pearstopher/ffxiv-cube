using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Cube.Core;

namespace Cube.TestBridge.Checks;

/// <summary>
/// Checks the view orientation control (PROJECT.md, "View orientation control"): a turned view changes the projected
/// wireframe exactly as Core says, turning the camera never changes the view, the control is drawn in the chosen
/// corner, and resetting brings back the default view. Writes a draw-list snapshot from several angles.
/// </summary>
/// <remarks>
/// <para>
/// The orientation is set through the settings, which is exactly where the control writes the user's drag
/// (<see cref="CubeSettings.SetView"/>), so this tests everything after the mouse: settings → projection → frame.
/// Dragging ImGuizmo's widget itself needs a real mouse and is a manual step (<c>docs/manual-tests.md</c>).
/// </para>
/// <para>
/// Settings are changed in memory only, never saved, and the user's values are restored in <see cref="Cleanup"/>.
/// The snapshots (<c>drawlist-view-*.json</c>) contain geometry only, like <see cref="FrameSnapshotCheck"/>'s.
/// </para>
/// </remarks>
/// <param name="folder">The bridge folder the snapshots are written to.</param>
internal sealed class ViewControlCheck(string folder) : BridgeCheck
{
    /// <summary>How far a wireframe end may be from Core's own projection, in pixels: float noise only.</summary>
    private const float LineTolerance = 0.01f;

    /// <summary>The angles snapshotted (yaw°, pitch°, name): default, turned and tilted, from below, straight down.</summary>
    private static readonly (float Yaw, float Pitch, string Name)[] Angles =
    [
        (0f, ViewOrientation.DefaultPitchDegrees, "default"),
        (45f, 60f, "turned"),
        (-30f, -35f, "below"),
        (0f, 90f, "top"),
    ];

    /// <summary>The user's settings before the check.</summary>
    private readonly CubeSettings saved = new();

    /// <summary>The window's open state before the check.</summary>
    private bool wasOpen;

    /// <inheritdoc/>
    public override string Name => "view-control-turns-the-cube";

    /// <inheritdoc/>
    public override TimeSpan Timeout => TimeSpan.FromSeconds(20);

    /// <inheritdoc/>
    public override IEnumerable<CheckStep> Run(BridgeContext context)
    {
        var window = context.Window;
        var settings = window.Settings;
        this.saved.CopyFrom(settings);
        this.wasOpen = window.IsOpen;
        window.IsOpen = true;
        window.Fakes.UseStandardSet();
        settings.CopyFrom(new CubeSettings());
        foreach (var step in WaitForDraws(window))
        {
            yield return step;
        }

        if (window.LastProjection.Orientation != ViewOrientation.Default)
        {
            yield return CheckStep.Fail($"Default settings drew view {window.LastProjection.Orientation.YawDegrees:0.0}°/{window.LastProjection.Orientation.PitchDegrees:0.0}°.");
            yield break;
        }

        var defaultLines = Lines(window);
        string? problem = null;
        foreach (var (yaw, pitch, name) in Angles)
        {
            settings.SetView(ViewOrientation.FromDegrees(yaw, pitch));
            foreach (var step in WaitForDraws(window))
            {
                yield return step;
            }

            problem ??= MatchesCore(window, settings.View);
            AtomicFile.WriteAllText(Path.Combine(folder, $"drawlist-view-{name}.json"), FrameSnapshotCheck.Dump(window));
        }

        if (problem is not null)
        {
            yield return CheckStep.Fail(problem);
            yield break;
        }

        // A turned view must move the wireframe.
        settings.SetView(ViewOrientation.FromDegrees(45f, 60f));
        foreach (var step in WaitForDraws(window))
        {
            yield return step;
        }

        var turnedLines = Lines(window);
        var moved = MaxMove(defaultLines, turnedLines);
        context.Metric("wireframeMovedPixels", moved);
        if (moved < 5f)
        {
            yield return CheckStep.Fail($"Turning the view moved the wireframe only {moved:0.0} px.");
            yield break;
        }

        // The camera never turns the view: turn the simulated camera, the wireframe stays put.
        window.SimulatedCameraForward = new Vector3(1, -0.3f, -1);
        foreach (var step in WaitForDraws(window))
        {
            yield return step;
        }

        var afterCamera = Lines(window);
        if (window.LastProjection.Orientation != settings.View || MaxMove(turnedLines, afterCamera) > LineTolerance)
        {
            yield return CheckStep.Fail("Turning the camera changed the view orientation or the wireframe.");
            yield break;
        }

        window.SimulatedCameraForward = null;

        // The control is drawn in the chosen corner.
        foreach (var corner in new[] { WidgetCorner.BottomRight, WidgetCorner.TopLeft })
        {
            settings.ViewWidgetCorner = corner;
            settings.Changed();
            foreach (var step in WaitForDraws(window))
            {
                yield return step;
            }

            var size = ViewWidget.Size(window.LastRegionSize);
            var expected = ViewWidget.Position(window.LastRegionMin, window.LastRegionSize, corner, size);
            if (window.WidgetSize != size || Vector2.Distance(window.WidgetMin, expected) > 0.5f)
            {
                yield return CheckStep.Fail($"View control in {corner} is at {window.WidgetMin} size {window.WidgetSize}; expected {expected} size {size}.");
                yield break;
            }
        }

        context.Metric("widgetSize", window.WidgetSize);

        // Reset view brings back the default.
        settings.ResetView();
        foreach (var step in WaitForDraws(window))
        {
            yield return step;
        }

        var resetMove = MaxMove(defaultLines, Lines(window));
        yield return window.LastProjection.Orientation == ViewOrientation.Default && resetMove <= LineTolerance
            ? CheckStep.Pass($"{Angles.Length} view angles matched Core and were snapshotted; turning moved the wireframe {moved:0} px; the camera didn't turn the view; the control followed the corner setting; reset restored the default.")
            : CheckStep.Fail($"Reset view left orientation {window.LastProjection.Orientation.YawDegrees:0.0}°/{window.LastProjection.Orientation.PitchDegrees:0.0}° (wireframe {resetMove:0.00} px off).");
    }

    /// <inheritdoc/>
    public override void Cleanup(BridgeContext context)
    {
        context.Window.SimulatedCameraForward = null;
        context.Window.Settings.CopyFrom(this.saved);
        context.Window.Fakes.Clear();
        context.Window.IsOpen = this.wasOpen;
    }

    /// <summary>
    /// Checks the last frame used the wanted orientation and its 12 wireframe lines are where Core projects the cube's
    /// corners with that orientation.
    /// </summary>
    /// <param name="window">The cube window.</param>
    /// <param name="want">The orientation set in the settings.</param>
    /// <returns>What was wrong, or null.</returns>
    private static string? MatchesCore(CubeWindow window, ViewOrientation want)
    {
        if (window.LastProjection.Orientation != want)
        {
            return $"Set view {want.YawDegrees:0.0}°/{want.PitchDegrees:0.0}° but the frame used {window.LastProjection.Orientation.YawDegrees:0.0}°/{window.LastProjection.Orientation.PitchDegrees:0.0}°.";
        }

        var expected = new DrawList(16);
        var projection = ViewProjection.Fit(window.LastRegionMin, window.LastRegionSize, window.LastSpace.HalfEdge, want);
        CubeGeometry.AddWireCube(expected, window.LastSpace, projection, window.WireColour);
        var drawn = Lines(window);
        var expectedLines = new List<(Vector2 A, Vector2 B)>();
        foreach (var p in expected.Items)
        {
            expectedLines.Add((p.A, p.B));
        }

        var off = MaxMove(expectedLines, drawn);
        return off <= LineTolerance ? null : $"At {want.YawDegrees:0}°/{want.PitchDegrees:0}° the wireframe is {off:0.00} px from Core's projection.";
    }

    /// <summary>The last frame's wireframe lines, as end points on screen.</summary>
    /// <param name="window">The cube window.</param>
    private static List<(Vector2 A, Vector2 B)> Lines(CubeWindow window)
    {
        var lines = new List<(Vector2 A, Vector2 B)>();
        foreach (var p in window.DrawList.Items)
        {
            if (p.Kind == PrimitiveKind.Line)
            {
                lines.Add((p.A, p.B));
            }
        }

        return lines;
    }

    /// <summary>
    /// The furthest any line of <paramref name="before"/> is from its nearest line in <paramref name="after"/>
    /// (lines may be reordered by the depth sort); infinite if the counts differ.
    /// </summary>
    /// <param name="before">One frame's lines.</param>
    /// <param name="after">Another frame's lines.</param>
    private static float MaxMove(List<(Vector2 A, Vector2 B)> before, List<(Vector2 A, Vector2 B)> after)
    {
        if (before.Count != after.Count || before.Count == 0)
        {
            return float.PositiveInfinity;
        }

        var worst = 0f;
        foreach (var line in before)
        {
            var best = float.PositiveInfinity;
            foreach (var other in after)
            {
                best = MathF.Min(best, MathF.Max(Vector2.Distance(line.A, other.A), Vector2.Distance(line.B, other.B)));
            }

            worst = MathF.Max(worst, best);
        }

        return worst;
    }
}
