using System;
using System.Collections.Generic;
using System.Numerics;
using Cube.Core;
using Cube.Game;

namespace Cube.TestBridge.Checks;

/// <summary>
/// Checks both orientation modes (PROJECT.md, "Cube" and "Cube on screen never turns by itself"): in Camera mode the
/// cube's alignment yaw matches the live camera's heading, in North up mode it is 0 whatever the camera does; and
/// turning the camera leaves the wireframe exactly where it was on screen while the contents rotate, entering and
/// leaving at the sides.
/// </summary>
/// <remarks>
/// <para>
/// The live camera is read again here, independently of the window, through <see cref="CameraReader"/> and turned
/// into a heading with <see cref="ViewYaw.FromForward"/>. A small tolerance covers the camera moving between the
/// window's draw and this check, plus <see cref="ViewYaw.ChangeThreshold"/>. If the camera happens to face north,
/// North up can't be told apart from Camera mode; the <c>cameraYaw</c> metric shows when that was the case.
/// </para>
/// <para>
/// The turning part uses the window's Debug-only simulated camera, so no game input is needed: two fakes are fixed in
/// the world (one east of the player, one near a horizontal corner), the "camera" faces north and then north-east,
/// and the check compares the 12 wireframe lines and where the fakes went. The mode, simulated camera and fakes are
/// changed in memory only and restored afterwards.
/// </para>
/// </remarks>
internal sealed class OrientationCheck : BridgeCheck
{
    /// <summary>Largest allowed difference between the cube's yaw and the camera heading, in radians (about 1°).</summary>
    private const float Tolerance = 0.02f;

    /// <summary>How far a wireframe end may move between the two simulated yaws, in pixels: float noise only.</summary>
    private const float LineTolerance = 0.01f;

    /// <summary>The simulated camera facing north: alignment yaw 0.</summary>
    private static readonly Vector3 FacingNorth = new(0, -0.3f, -1);

    /// <summary>The simulated camera facing north-east: alignment yaw π/4, the turn that moves corners furthest.</summary>
    private static readonly Vector3 FacingNorthEast = new(1, -0.3f, -1);

    /// <summary>The window's open state before the check.</summary>
    private bool wasOpen;

    /// <summary>The orientation setting before the check.</summary>
    private OrientationMode wasMode;

    /// <inheritdoc/>
    public override string Name => "orientation-modes-follow-camera-or-north";

    /// <inheritdoc/>
    public override IEnumerable<CheckStep> Run(BridgeContext context)
    {
        var window = context.Window;
        this.wasOpen = window.IsOpen;
        this.wasMode = window.Settings.Orientation;
        window.IsOpen = true;

        window.Settings.Orientation = OrientationMode.Camera;
        window.Settings.Changed();
        foreach (var step in WaitForDraws(window))
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

        window.Settings.Orientation = OrientationMode.NorthUp;
        window.Settings.Changed();
        foreach (var step in WaitForDraws(window))
        {
            yield return step;
        }

        context.Metric("northUpYaw", window.LastSpace.Yaw);
        if (window.LastSpace.Yaw != ViewYaw.NorthUp)
        {
            yield return CheckStep.Fail($"North up yaw is {window.LastSpace.Yaw:0.000}, not 0.");
            yield break;
        }

        // Turning the camera: the wireframe stays, the contents rotate (#62).
        window.Settings.Orientation = OrientationMode.Camera;
        window.Settings.Changed();
        var half = MathF.Min(window.LastSpace.Half.X, window.LastSpace.Half.Z);
        var anchor = window.LastSpace.Centre;
        var east = anchor + new Vector3(0.5f * half, 0, 0);
        var corner = anchor + new Vector3(0.9f * half, 0, 0.9f * half);
        window.Fakes.Clear();
        window.Fakes.AddAtWorld("Fake World East", east, PlayerRelation.Other);
        window.Fakes.AddAtWorld("Fake World Corner", corner, PlayerRelation.Other);

        window.SimulatedCameraForward = FacingNorth;
        foreach (var step in WaitForDraws(window))
        {
            yield return step;
        }

        var northLines = Lines(window);
        var northEast = window.LastProjection.ToScreen(window.LastSpace.ToView(east));
        var cornerInsideAtNorth = HasFake(window, 1);
        var eastDrawnAtNorth = HasFake(window, 0);

        window.SimulatedCameraForward = FacingNorthEast;
        foreach (var step in WaitForDraws(window))
        {
            yield return step;
        }

        var turnedLines = Lines(window);
        var turnedEast = window.LastProjection.ToScreen(window.LastSpace.ToView(east));
        var cornerInsideTurned = HasFake(window, 1);
        var eastDrawnTurned = HasFake(window, 0);
        var eastMoved = Vector2.Distance(northEast, turnedEast);
        context.Metric("turnedYaw", window.LastSpace.Yaw);
        context.Metric("fakeMovedPixels", eastMoved);

        if (!SameLines(northLines, turnedLines, out var why))
        {
            yield return CheckStep.Fail($"The wireframe turned with the camera: {why}");
        }
        else if (!eastDrawnAtNorth || !eastDrawnTurned || eastMoved < 1f)
        {
            yield return CheckStep.Fail($"The world-fixed fake east of the player didn't move with the camera (drawn {eastDrawnAtNorth}/{eastDrawnTurned}, moved {eastMoved:0.0} px).");
        }
        else if (!cornerInsideAtNorth || cornerInsideTurned)
        {
            yield return CheckStep.Fail($"The fake near a corner should be inside facing north and outside facing north-east; was {cornerInsideAtNorth}/{cornerInsideTurned}.");
        }
        else
        {
            yield return CheckStep.Pass($"Camera mode matched the camera heading ({cameraYaw:0.000} rad, within {off:0.0000}); North up stayed at 0; turning the camera kept all 12 wireframe lines still, moved a fake {eastMoved:0} px and a corner fake left the cube.");
        }
    }

    /// <inheritdoc/>
    public override void Cleanup(BridgeContext context)
    {
        context.Window.SimulatedCameraForward = null;
        context.Window.Fakes.Clear();
        context.Window.Settings.Orientation = this.wasMode;
        context.Window.Settings.Changed();
        context.Window.IsOpen = this.wasOpen;
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

    /// <summary>Whether two frames drew the same 12 lines (in any order, since depth sorting may reorder them).</summary>
    /// <param name="before">The first frame's lines.</param>
    /// <param name="after">The second frame's lines.</param>
    /// <param name="why">What differed, if anything.</param>
    private static bool SameLines(List<(Vector2 A, Vector2 B)> before, List<(Vector2 A, Vector2 B)> after, out string why)
    {
        if (before.Count != 12 || after.Count != 12)
        {
            why = $"expected 12 lines in each frame, got {before.Count} and {after.Count}.";
            return false;
        }

        foreach (var line in before)
        {
            var found = false;
            foreach (var other in after)
            {
                if (Vector2.Distance(line.A, other.A) <= LineTolerance && Vector2.Distance(line.B, other.B) <= LineTolerance)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                why = $"line {line.A}-{line.B} is gone after the turn.";
                return false;
            }
        }

        why = string.Empty;
        return true;
    }

    /// <summary>Whether a world-fixed fake is in the cube and drawn (it has at least one face tagged as its own).</summary>
    /// <param name="window">The cube window.</param>
    /// <param name="fakeIndex">Which fake, in the order added.</param>
    internal static bool HasFake(CubeWindow window, int fakeIndex)
    {
        var players = window.Players.Items;
        for (var i = 0; i < players.Length; i++)
        {
            if (players[i].EntityId != FakePlayers.FirstEntityId + (uint)fakeIndex)
            {
                continue;
            }

            foreach (var p in window.DrawList.Items)
            {
                if (p.Kind == PrimitiveKind.Quad && p.Tag == i + 1)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
