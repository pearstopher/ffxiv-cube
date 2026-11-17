using System;
using System.Collections.Generic;
using Cube.Core;
using Cube.Game;

namespace Cube.TestBridge.Checks;

/// <summary>
/// Checks both orientation modes against the live game camera (PROJECT.md, "View"): in Camera mode the view's yaw
/// matches the camera's heading, in North up mode it is 0 whatever the camera does.
/// </summary>
/// <remarks>
/// The camera is read again here, independently of the window, through <see cref="CameraReader"/> and turned into
/// a heading with <see cref="ViewYaw.FromForward"/>. A small tolerance covers the camera moving between the window's
/// draw and this check, plus <see cref="ViewYaw.ChangeThreshold"/>. If the camera happens to face north, North up
/// can't be told apart from Camera mode; the <c>cameraYaw</c> metric shows when that was the case.
/// </remarks>
internal sealed class OrientationCheck : BridgeCheck
{
    /// <summary>Frames to wait for the window to draw in each mode.</summary>
    private const int FramesToWait = 30;

    /// <summary>Largest allowed difference between the view's yaw and the camera heading, in radians (about 1°).</summary>
    private const float Tolerance = 0.02f;

    /// <summary>The window's open state before the check.</summary>
    private bool wasOpen;

    /// <summary>The window's orientation mode before the check.</summary>
    private OrientationMode wasMode;

    /// <inheritdoc/>
    public override string Name => "orientation-modes-follow-camera-or-north";

    /// <inheritdoc/>
    public override IEnumerable<CheckStep> Run(BridgeContext context)
    {
        var window = context.Window;
        this.wasOpen = window.IsOpen;
        this.wasMode = window.Orientation;
        window.IsOpen = true;

        window.Orientation = OrientationMode.Camera;
        foreach (var step in WaitForFrames(window))
        {
            yield return step;
        }

        if (!CameraReader.TryGetForward(out var forward))
        {
            yield return CheckStep.Fail("No game camera to compare with.");
            yield break;
        }

        var cameraYaw = ViewYaw.FromForward(forward, window.LastSpace.Yaw);
        var off = MathF.Abs(MathF.IEEERemainder(window.LastSpace.Yaw - cameraYaw, 2 * MathF.PI));
        context.Metric("cameraYaw", cameraYaw);
        context.Metric("cameraModeYaw", window.LastSpace.Yaw);
        if (off > Tolerance)
        {
            yield return CheckStep.Fail($"Camera mode yaw {window.LastSpace.Yaw:0.000} is {off:0.000} rad from the camera heading {cameraYaw:0.000}.");
            yield break;
        }

        window.Orientation = OrientationMode.NorthUp;
        foreach (var step in WaitForFrames(window))
        {
            yield return step;
        }

        context.Metric("northUpYaw", window.LastSpace.Yaw);
        yield return window.LastSpace.Yaw == ViewYaw.NorthUp
            ? CheckStep.Pass($"Camera mode matched the camera heading ({cameraYaw:0.000} rad, within {off:0.0000}); North up stayed at 0.")
            : CheckStep.Fail($"North up yaw is {window.LastSpace.Yaw:0.000}, not 0.");
    }

    /// <inheritdoc/>
    public override void Cleanup(BridgeContext context)
    {
        context.Window.Orientation = this.wasMode;
        context.Window.IsOpen = this.wasOpen;
    }

    /// <summary>Waits until the window has drawn two frames in the current mode.</summary>
    /// <param name="window">The cube window.</param>
    private static IEnumerable<CheckStep> WaitForFrames(CubeWindow window)
    {
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
    }
}
