using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using Cube.Core;

namespace Cube.TestBridge.Checks;

/// <summary>
/// Checks collision never reaches outside the cube on screen, at any camera angle, in either orientation mode (#93;
/// PROJECT.md, "Collision (phase 2)": clipped to the cube; "Cube": in "Rotate contents with camera" the contents turn
/// with the camera).
/// </summary>
/// <remarks>
/// <para>
/// In "Rotate contents with camera" the Debug-only simulated camera turns the cube to 0°, 30°, 45° and 90°; in "North
/// up" the cube stays at 0° and the view is turned instead. For each, every corner of every collision face in the
/// window's real frame must be inside the cube's projected outline (<see cref="CubeOutline"/>), within a small
/// tolerance for rounding.
/// </para>
/// <para>
/// The real scenery may not hold a box bigger than the cube, the case that showed the bug, so each angle also clips a
/// synthetic floor three times the cube's size, centred under the player, with the same cube and projection, and
/// checks its faces the same way. Before the fix that floor's corners reached up to √2 past the cube's sides at 30°
/// and 45°. Settings and the simulated camera are changed in memory only and restored in <see cref="Cleanup"/>.
/// </para>
/// </remarks>
internal sealed class CollisionInsideCubeCheck : BridgeCheck
{
    /// <summary>How far outside the outline a corner may be, in pixels: float rounding only.</summary>
    private const float Tolerance = 0.05f;

    /// <summary>Largest allowed difference between the cube's yaw and the simulated camera's, in radians.</summary>
    private const float YawTolerance = 0.02f;

    /// <summary>The cube yaws tried in Camera mode, in degrees.</summary>
    private static readonly float[] CameraYaws = [0f, 30f, 45f, 90f];

    /// <summary>The user's settings before the check, restored by <see cref="Cleanup"/>.</summary>
    private readonly CubeSettings saved = new();

    /// <summary>The synthetic floor's faces, rebuilt for each angle; allocated once per check run.</summary>
    private readonly DrawList synthetic = new(16);

    /// <summary>The window's open state before the check, restored by <see cref="Cleanup"/>.</summary>
    private bool wasOpen;

    /// <inheritdoc/>
    public override string Name => "collision-inside-cube";

    /// <inheritdoc/>
    /// <remarks>Room for a first gather at background frame rates (about 10 fps), then five angles.</remarks>
    public override TimeSpan Timeout => TimeSpan.FromSeconds(45);

    /// <inheritdoc/>
    public override IEnumerable<CheckStep> Run(BridgeContext context)
    {
        var window = context.Window;
        var settings = window.Settings;
        var cache = window.Collision.Cache;
        this.saved.CopyFrom(settings);
        this.wasOpen = window.IsOpen;
        settings.CopyFrom(new CubeSettings());
        settings.Orientation = OrientationMode.Camera;
        settings.Changed();
        window.IsOpen = true;

        while (!cache.Ready.HasData || cache.IsGathering)
        {
            yield return CheckStep.NextFrame;
        }

        var liveCorners = 0;
        var prisms = 0;
        foreach (var degrees in CameraYaws)
        {
            var radians = degrees * MathF.PI / 180f;
            window.SimulatedCameraForward = new Vector3(MathF.Sin(radians), -0.3f, -MathF.Cos(radians));
            foreach (var step in WaitForDraws(window))
            {
                yield return step;
            }

            var off = MathF.Abs(MathF.IEEERemainder(window.LastSpace.Yaw - radians, 2 * MathF.PI));
            if (off > YawTolerance)
            {
                yield return CheckStep.Fail(string.Create(CultureInfo.InvariantCulture, $"Simulated camera at {degrees}° turned the cube to {window.LastSpace.Yaw:0.000} rad."));
                yield break;
            }

            prisms = Math.Max(prisms, CountPrisms(window.ClippedCollision.Items));
            var problem = this.CheckFrame(window, $"camera mode at {degrees}°", ref liveCorners);
            if (problem is not null)
            {
                yield return CheckStep.Fail(problem);
                yield break;
            }
        }

        // North up: the cube stays at 0° whatever the camera does, and the view control turns the view instead.
        settings.Orientation = OrientationMode.NorthUp;
        settings.ViewRotation = ViewOrientation.FromDegrees(70f, 20f).Rotation;
        settings.Changed();
        foreach (var step in WaitForDraws(window))
        {
            yield return step;
        }

        var northProblem = this.CheckFrame(window, "north up with a turned view", ref liveCorners);
        context.Metric("clippedBoxes", window.ClippedCollision.Count);
        context.Metric("mostPrisms", prisms);
        context.Metric("liveCornersChecked", liveCorners);
        if (northProblem is not null)
        {
            yield return CheckStep.Fail(northProblem);
            yield break;
        }

        yield return CheckStep.Pass(string.Create(
            CultureInfo.InvariantCulture,
            $"At 0/30/45/90° (camera mode) and north up with a turned view, all {liveCorners} collision corners drawn were inside the cube (up to {prisms} prisms cut by a turned side), and so was a synthetic floor 3x the cube's size."));
    }

    /// <inheritdoc/>
    public override void Cleanup(BridgeContext context)
    {
        context.Window.SimulatedCameraForward = null;
        context.Window.Settings.CopyFrom(this.saved);
        context.Window.Settings.Changed();
        context.Window.IsOpen = this.wasOpen;
    }

    /// <summary>How many clipped boxes are prisms rather than plain boxes.</summary>
    /// <param name="boxes">The clipped boxes.</param>
    private static int CountPrisms(ReadOnlySpan<ClippedBox> boxes)
    {
        var count = 0;
        foreach (ref readonly var box in boxes)
        {
            count += box.IsRectangle ? 0 : 1;
        }

        return count;
    }

    /// <summary>Finds a corner of a face in a list that lies outside the cube's outline.</summary>
    /// <param name="list">The faces.</param>
    /// <param name="outline">The cube's outline.</param>
    /// <param name="checkedCorners">Incremented for every corner checked.</param>
    /// <param name="where">The offending corner and its face's tag, if any.</param>
    private static bool FindOutside(DrawList list, in CubeOutline outline, ref int checkedCorners, out string where)
    {
        foreach (var p in list.Items)
        {
            if (!CollisionCubes.IsCollisionTag(p.Tag))
            {
                continue;
            }

            ReadOnlySpan<Vector2> corners = p.Kind == PrimitiveKind.Polygon ? list.Points(p) : [p.A, p.B, p.C, p.D];
            foreach (var corner in corners)
            {
                checkedCorners++;
                if (!outline.Contains(corner, Tolerance))
                {
                    where = string.Create(CultureInfo.InvariantCulture, $"a {p.Kind} of box {CollisionCubes.IndexOf(p.Tag)} has a corner at {corner}");
                    return true;
                }
            }
        }

        where = string.Empty;
        return false;
    }

    /// <summary>Checks the window's last frame and the synthetic floor against the cube's outline.</summary>
    /// <param name="window">The cube window.</param>
    /// <param name="angle">Which angle this is, for the message.</param>
    /// <param name="liveCorners">Incremented for every real collision corner checked.</param>
    /// <returns>What was wrong, or null.</returns>
    private string? CheckFrame(CubeWindow window, string angle, ref int liveCorners)
    {
        var space = window.LastSpace;
        var projection = window.LastProjection;
        var outline = new CubeOutline(space, projection);
        if (FindOutside(window.DrawList, outline, ref liveCorners, out var where))
        {
            return $"In {angle}, {where}, outside the cube's outline.";
        }

        var reach = 3f * space.Largest;
        var floor = new CollisionBox(
            new Vector3(space.Centre.X - reach, space.Centre.Y - space.Half.Y - 1f, space.Centre.Z - reach),
            new Vector3(space.Centre.X + reach, space.Centre.Y - 1f, space.Centre.Z + reach));
        if (!CollisionClipper.TryClip(space, floor, out var clipped))
        {
            return $"In {angle}, the synthetic floor wasn't clipped into the cube.";
        }

        this.synthetic.Clear();
        CollisionCubes.Add(this.synthetic, new BoxProjector(space, projection), [clipped], window.CollisionColour);
        var syntheticCorners = 0;
        return FindOutside(this.synthetic, outline, ref syntheticCorners, out where)
            ? $"In {angle}, the synthetic floor bigger than the cube: {where}, outside the cube's outline."
            : null;
    }
}
