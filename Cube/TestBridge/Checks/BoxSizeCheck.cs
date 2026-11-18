using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using Cube.Core;

namespace Cube.TestBridge.Checks;

/// <summary>
/// Checks the box size and zoom (#90; PROJECT.md, "Cube", "Zoom", "On screen", "Culling"): set extents change the
/// projected wireframe's proportions, zooming changes which points are inside, resetting restores 40 × 40 × 40, and
/// an idle frame changes no settings.
/// </summary>
/// <remarks>
/// <para>
/// The view is set straight on and level (heading 0, tilt 0) and the cube north up, so the wireframe's height and width
/// on screen are exactly the box's height and width at one scale: a 20-wide, 80-tall box must measure four times as
/// tall as wide. A fake fixed 18 yalms east of the player is inside the default cube (half-width 20), outside after one
/// "Zoom in" step (half-width 16), and inside again after "Reset size".
/// </para>
/// <para>
/// Dragging the gizmo's handles needs a real mouse, so it is a manual step in <c>docs/manual-tests.md</c>; the maths
/// that turns a drag into a resize or zoom is unit tested in <c>GizmoModelTests</c>. The settings, fakes and window
/// state are changed in memory only and restored in <see cref="Cleanup"/>.
/// </para>
/// </remarks>
internal sealed class BoxSizeCheck : BridgeCheck
{
    /// <summary>How far the measured height-to-width ratio may be from the box's, for rounding.</summary>
    private const float RatioTolerance = 0.01f;

    /// <summary>Frames watched with nothing touched, to check the gizmo leaves the settings alone when idle.</summary>
    private const int IdleFrames = 10;

    /// <summary>The box drawn: tall and narrow, so its proportions can't be mistaken for a cube's.</summary>
    private static readonly Vector3 TallBox = new(20, 80, 40);

    /// <summary>The user's settings before the check, restored by <see cref="Cleanup"/>.</summary>
    private readonly CubeSettings saved = new();

    /// <summary>The window's open state before the check, restored by <see cref="Cleanup"/>.</summary>
    private bool wasOpen;

    /// <inheritdoc/>
    public override string Name => "box-size-and-zoom";

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

        settings.CopyFrom(new CubeSettings());
        settings.Orientation = OrientationMode.NorthUp;
        settings.ViewRotation = ViewOrientation.FromDegrees(0, 0).Rotation;
        settings.Changed();
        foreach (var step in WaitForDraws(window))
        {
            yield return step;
        }

        var cubeRatio = WireRatio(window);

        // Extents change the drawn proportions.
        settings.SetSize(TallBox);
        foreach (var step in WaitForDraws(window))
        {
            yield return step;
        }

        var boxRatio = WireRatio(window);
        context.Metric("cubeRatio", cubeRatio);
        context.Metric("boxRatio", boxRatio);
        var wantRatio = TallBox.Y / TallBox.X;
        if (window.LastSpace.Size != TallBox || MathF.Abs(boxRatio - wantRatio) > RatioTolerance || MathF.Abs(cubeRatio - 1f) > RatioTolerance)
        {
            yield return CheckStep.Fail(string.Create(
                CultureInfo.InvariantCulture,
                $"Box {window.LastSpace.Size} drew height/width {boxRatio:0.000} (want {wantRatio}); the 40 cube drew {cubeRatio:0.000} (want 1)."));
            yield break;
        }

        // Nothing touched for a while: the gizmo must not write the settings (nothing recomputed while idle).
        var revision = settings.Revision;
        var start = window.DrawCost.TotalRecorded;
        while (window.DrawCost.TotalRecorded - start < IdleFrames)
        {
            yield return CheckStep.NextFrame;
        }

        if (settings.Revision != revision)
        {
            yield return CheckStep.Fail($"The settings changed {settings.Revision - revision} times while nothing was touched.");
            yield break;
        }

        // Zoom changes which points are inside: a fake 18 yalms east is inside the 40 cube and outside after zooming in.
        settings.ResetSize();
        window.Fakes.Clear();
        window.Fakes.AddAtWorld("Fake Zoom East", window.LastSpace.Centre + new Vector3(18, 0, 0), PlayerRelation.Other);
        foreach (var step in WaitForDraws(window))
        {
            yield return step;
        }

        var insideBefore = OrientationCheck.HasFake(window, 0);
        settings.Zoom(1f / BoxExtents.ZoomStep);
        foreach (var step in WaitForDraws(window))
        {
            yield return step;
        }

        var zoomedSize = window.LastSpace.Size;
        var insideZoomed = OrientationCheck.HasFake(window, 0);

        settings.ResetSize();
        foreach (var step in WaitForDraws(window))
        {
            yield return step;
        }

        var insideReset = OrientationCheck.HasFake(window, 0);
        context.Metric("zoomedWidth", zoomedSize.X);
        if (!insideBefore || insideZoomed || !insideReset)
        {
            yield return CheckStep.Fail($"The fake 18 yalms east should be inside, outside after zooming in to {zoomedSize}, inside after reset; was {insideBefore}/{insideZoomed}/{insideReset}.");
        }
        else if (window.LastSpace.Size != BoxExtents.DefaultSize)
        {
            yield return CheckStep.Fail($"Reset size drew {window.LastSpace.Size}, not 40 x 40 x 40.");
        }
        else
        {
            yield return CheckStep.Pass(string.Create(
                CultureInfo.InvariantCulture,
                $"Box {TallBox} drew {boxRatio:0.000} times as tall as wide (cube {cubeRatio:0.000}); idle frames left the settings alone; zooming in to {zoomedSize} pushed a fake out, reset brought it back."));
        }
    }

    /// <inheritdoc/>
    public override void Cleanup(BridgeContext context)
    {
        context.Window.Fakes.Clear();
        context.Window.Settings.CopyFrom(this.saved);
        context.Window.IsOpen = this.wasOpen;
    }

    /// <summary>The last frame's wireframe height on screen divided by its width.</summary>
    /// <param name="window">The cube window.</param>
    private static float WireRatio(CubeWindow window)
    {
        var min = new Vector2(float.MaxValue);
        var max = new Vector2(float.MinValue);
        foreach (var p in window.DrawList.Items)
        {
            if (p.Kind == PrimitiveKind.Line)
            {
                min = Vector2.Min(min, Vector2.Min(p.A, p.B));
                max = Vector2.Max(max, Vector2.Max(p.A, p.B));
            }
        }

        var size = max - min;
        return size.X > 0 ? size.Y / size.X : 0f;
    }
}
