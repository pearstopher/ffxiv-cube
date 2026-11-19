using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;
using Cube.Core;
using FFXIVClientStructs.FFXIV.Common.Component.BGCollision;

namespace Cube.TestBridge.Checks;

/// <summary>
/// Checks the Mesh collision style in the running game (#100; PROJECT.md, "Collision style"): the gathered triangles
/// contain the game's own raycast hits, nothing is clipped or projected outside the cube at any yaw, the style's cost is
/// measured and reported, it allocates nothing per frame, and a snapshot of the scene is written.
/// </summary>
/// <remarks>
/// <para>
/// <b>Raycasts.</b> Straight down from above the player on a grid; every hit inside the gathered region must lie within
/// <see cref="HitTolerance"/> of a gathered triangle (as #28 did for boxes). The tolerance covers the flat sides a
/// 16-sided cylinder or an 80-triangle sphere has where the real shape is round.
/// </para>
/// <para>
/// <b>Inside the cube.</b> For four yaws (the cube turns with the camera, so the clip does too) the check waits for the
/// clipped mesh to be rebuilt and verifies that every kept edge end is inside the cube and every projected end is inside
/// the cube's outline on screen, and verifies the outline every frame while the rebuild is still going (the stale picture
/// is clipped to the new cube at projection time).
/// </para>
/// <para>
/// <b>Cost.</b> The frame time of idle Mesh drawing is <em>reported</em> with no limit (PROJECT.md: Mesh mode is an
/// opt-in exception to the 0.5 ms budget); the check fails only on allocations, on a drawing outside the cube, on a
/// scene bigger than the buffers (dropped triangles or edges are reported), or on nothing being gathered. Settings change
/// in memory only and are restored in <see cref="Cleanup"/>.
/// </para>
/// </remarks>
/// <param name="folder">The bridge folder the snapshot dump is written to.</param>
internal sealed class MeshStyleCheck(string folder) : BridgeCheck
{
    /// <summary>Name of the snapshot dump, read by <c>Cube.Snapshot</c> like <see cref="FrameSnapshotCheck.FileName"/>.</summary>
    public const string SnapshotFileName = "drawlist-mesh.json";

    /// <summary>How far a raycast hit may be from a gathered triangle, in yalms.</summary>
    private const float HitTolerance = 0.25f;

    /// <summary>How far above the player the rays start, in yalms.</summary>
    private const float RayStartHeight = 2f;

    /// <summary>How far the rays reach, in yalms.</summary>
    private const float RayLength = 30f;

    /// <summary>Frames the idle cost is measured over.</summary>
    private const int FramesToMeasure = 120;

    /// <summary>Slack on the screen outline test, in pixels.</summary>
    private const float OutlineSlack = 0.75f;

    /// <summary>The user's settings before the check, restored by <see cref="Cleanup"/>.</summary>
    private readonly CubeSettings saved = new();

    /// <summary>The window's open state before the check, restored by <see cref="Cleanup"/>.</summary>
    private bool wasOpen;

    /// <inheritdoc/>
    public override string Name => "mesh-style";

    /// <inheritdoc/>
    public override TimeSpan Timeout => TimeSpan.FromSeconds(120);

    /// <inheritdoc/>
    public override IEnumerable<CheckStep> Run(BridgeContext context)
    {
        var window = context.Window;
        var settings = window.Settings;
        this.saved.CopyFrom(settings);
        this.wasOpen = window.IsOpen;
        settings.CopyFrom(new CubeSettings());
        settings.CollisionStyle = CollisionStyle.Mesh;
        settings.Orientation = OrientationMode.Camera;
        settings.Changed();
        window.IsOpen = true;
#if DEBUG
        window.SimulatedCameraForward = Forward(0f);
#endif

        // Gathered and built for the first time.
        foreach (var step in WaitForBuild(window, 0f, null))
        {
            yield return step;
        }

        var cache = window.Mesh.Cache;
        var set = cache.Ready;
        if (set is null || set.Count == 0)
        {
            yield return CheckStep.Fail("No collision triangles gathered. Stand somewhere with walls or floor and run again.");
            yield break;
        }

        context.Metric("triangles", set.Count);
        context.Metric("trianglesOffered", set.Offered);
        context.Metric("trianglesDropped", set.Dropped);
        context.Metric("edges", window.ClippedMesh.Count);
        context.Metric("edgesDropped", window.ClippedMesh.Dropped);

        // Raycast hits against the gathered triangles.
        var rays = CheckRays(window.LastSpace, set, out var hits, out var misses, out var firstMiss);
        context.Metric("rayHits", hits);
        context.Metric("rayMisses", misses);
        if (rays is not null)
        {
            yield return CheckStep.Fail(rays);
            yield break;
        }

        // Nothing outside the cube at four yaws; the outline is also checked every frame while rebuilding.
        string? outside = null;
        foreach (var yaw in new[] { 0f, 0.6f, 1.3f, 2.5f })
        {
#if DEBUG
            window.SimulatedCameraForward = Forward(yaw);
#endif
            foreach (var step in WaitForBuild(window, yaw, problem => outside ??= problem))
            {
                yield return step;
            }

            outside ??= CheckInside(window);
            if (outside is not null)
            {
                yield return CheckStep.Fail($"At yaw {yaw}: {outside}");
                yield break;
            }
        }

        // Idle cost, reported only: Mesh mode is an exception to the 0.5 ms budget (PROJECT.md, "Collision style").
        var sampler = window.DrawCost;
        var start = sampler.TotalRecorded;
        while (sampler.TotalRecorded - start < 10)
        {
            yield return CheckStep.NextFrame;
        }

        sampler.Reset();
        while (sampler.Count < FramesToMeasure)
        {
            yield return CheckStep.NextFrame;
        }

        var average = sampler.AverageMilliseconds;
        var bytes = sampler.TotalAllocatedBytes;
        context.Metric("segmentsDrawn", window.ClippedMesh.ProjectedCount);
        context.Metric("idleAverageMs", average);
        context.Metric("idleMaxMs", sampler.MaxMilliseconds);
        context.Metric("allocatedBytes", bytes);
        context.Metric("meshBuilds", window.ClippedMesh.Builds);

        // A snapshot of the scene, with the cube's own edges drawn over it.
        AtomicFile.WriteAllText(Path.Combine(folder, SnapshotFileName), FrameSnapshotCheck.Dump(window));

        var summary = string.Create(
            CultureInfo.InvariantCulture,
            $"{set.Count} triangles ({set.Dropped} dropped), {window.ClippedMesh.Count} edges ({window.ClippedMesh.Dropped} dropped), {window.ClippedMesh.ProjectedCount} drawn; {hits} raycast hits all on a triangle; nothing outside the cube at four yaws; idle draw {average:0.000} ms (reported, no limit), {bytes} bytes allocated (budget 0); snapshot {SnapshotFileName}.");
        yield return bytes == 0 && set.Dropped == 0 && window.ClippedMesh.Dropped == 0
            ? CheckStep.Pass(summary)
            : CheckStep.Fail(summary);
    }

    /// <inheritdoc/>
    public override void Cleanup(BridgeContext context)
    {
#if DEBUG
        context.Window.SimulatedCameraForward = null;
#endif
        context.Window.Settings.CopyFrom(this.saved);
        context.Window.IsOpen = this.wasOpen;
    }

    /// <summary>The camera's forward direction for a cube yaw (see <see cref="CubeSpace"/>: forward is (sin yaw, -cos yaw) in X and Z).</summary>
    /// <param name="yaw">The cube's yaw in radians.</param>
    private static Vector3 Forward(float yaw) => new(MathF.Sin(yaw), 0f, -MathF.Cos(yaw));

    /// <summary>
    /// Waits until the mesh has been gathered and a build has finished for the cube at a yaw, checking the screen
    /// outline each frame on the way.
    /// </summary>
    /// <param name="window">The cube window.</param>
    /// <param name="yaw">The yaw the cube is to reach.</param>
    /// <param name="onProblem">Called with a description when a frame's projected segments lie outside the cube's outline.</param>
    private static IEnumerable<CheckStep> WaitForBuild(CubeWindow window, float yaw, Action<string>? onProblem)
    {
        var frames = 0;
        while (true)
        {
            var mesh = window.ClippedMesh;
            var cache = window.Mesh.Cache;
            var built = mesh.Builds > 0 && !mesh.IsBuilding && !cache.IsGathering &&
                MathF.Abs(window.LastSpace.Yaw - yaw) < 1e-3f && mesh.BuiltFor.Yaw == window.LastSpace.Yaw &&
                mesh.BuiltFor.Centre == window.LastSpace.Centre;
            if (built && frames > 2)
            {
                yield break;
            }

            var problem = OutlineProblem(window);
            if (problem is not null)
            {
                onProblem?.Invoke(problem);
            }

            frames++;
            yield return CheckStep.NextFrame;
        }
    }

    /// <summary>Whether every projected segment end of the last frame lies inside the cube's outline on screen.</summary>
    /// <param name="window">The cube window.</param>
    private static string? OutlineProblem(CubeWindow window)
    {
        var projection = window.LastProjection;
        var reach = (window.LastSpace.Half.Length() * projection.PixelsPerYalm) + OutlineSlack;
        var screen = window.ClippedMesh.Screen;
        for (var i = 0; i < screen.Length; i++)
        {
            var distance = Vector2.Distance(screen[i], projection.Origin);
            if (distance > reach)
            {
                return string.Create(CultureInfo.InvariantCulture, $"a projected end {distance:0.0} px from the centre, outside the cube's {reach:0.0} px outline");
            }
        }

        return null;
    }

    /// <summary>Whether every kept edge end of the finished build lies inside the cube it was clipped for.</summary>
    /// <param name="window">The cube window.</param>
    private static string? CheckInside(CubeWindow window)
    {
        var half = window.ClippedMesh.BuiltFor.Half + new Vector3(1e-3f);
        foreach (var p in window.ClippedMesh.Edges)
        {
            if (MathF.Abs(p.X) > half.X || MathF.Abs(p.Y) > half.Y || MathF.Abs(p.Z) > half.Z)
            {
                return string.Create(CultureInfo.InvariantCulture, $"an edge end at {p} (cube-local) is outside the cube of half size {half}");
            }
        }

        return OutlineProblem(window);
    }

    /// <summary>Casts rays straight down on a grid around the player and finds a gathered triangle for each hit in the region.</summary>
    /// <param name="space">The cube.</param>
    /// <param name="set">The gathered triangles.</param>
    /// <param name="hits">Rays that hit something inside the gathered region.</param>
    /// <param name="misses">Of those, hits with no triangle within <see cref="HitTolerance"/>.</param>
    /// <param name="firstMiss">Where the first such hit was.</param>
    /// <returns>A failure message, or null.</returns>
    private static string? CheckRays(in CubeSpace space, TriangleSet set, out int hits, out int misses, out Vector3 firstMiss)
    {
        hits = 0;
        misses = 0;
        firstMiss = default;
        var feet = space.Player;
        var corners = set.Corners;
        for (var gx = -5; gx <= 5; gx++)
        {
            for (var gz = -5; gz <= 5; gz++)
            {
                var origin = feet + new Vector3(gx * 2f, RayStartHeight, gz * 2f);
                if (!BGCollisionModule.RaycastMaterialFilter(origin, -Vector3.UnitY, out var result, RayLength))
                {
                    continue;
                }

                var hit = result.Point;
                if (hit.X < set.Region.Min.X || hit.X > set.Region.Max.X || hit.Y < set.Region.Min.Y || hit.Y > set.Region.Max.Y ||
                    hit.Z < set.Region.Min.Z || hit.Z > set.Region.Max.Z)
                {
                    continue;
                }

                hits++;
                var found = false;
                for (var t = 0; t + 2 < corners.Length && !found; t += 3)
                {
                    found = Distance(hit, corners[t], corners[t + 1], corners[t + 2]) <= HitTolerance;
                }

                if (!found && misses++ == 0)
                {
                    firstMiss = hit;
                }
            }
        }

        if (hits == 0)
        {
            return "No raycast hit anything near the player, so the triangles couldn't be compared with the game's own collision.";
        }

        return misses == 0
            ? null
            : string.Create(CultureInfo.InvariantCulture, $"{misses} of {hits} raycast hits (first at {firstMiss}) are not within {HitTolerance} yalm of any gathered triangle.");
    }

    /// <summary>The distance from a point to a triangle (Ericson, Real-Time Collision Detection, closest point on a triangle).</summary>
    /// <param name="p">The point.</param>
    /// <param name="a">First corner.</param>
    /// <param name="b">Second corner.</param>
    /// <param name="c">Third corner.</param>
    internal static float Distance(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
    {
        var ab = b - a;
        var ac = c - a;
        var ap = p - a;
        var d1 = Vector3.Dot(ab, ap);
        var d2 = Vector3.Dot(ac, ap);
        if (d1 <= 0 && d2 <= 0)
        {
            return Vector3.Distance(p, a);
        }

        var bp = p - b;
        var d3 = Vector3.Dot(ab, bp);
        var d4 = Vector3.Dot(ac, bp);
        if (d3 >= 0 && d4 <= d3)
        {
            return Vector3.Distance(p, b);
        }

        var vc = (d1 * d4) - (d3 * d2);
        if (vc <= 0 && d1 >= 0 && d3 <= 0)
        {
            return Vector3.Distance(p, a + (ab * (d1 / (d1 - d3))));
        }

        var cp = p - c;
        var d5 = Vector3.Dot(ab, cp);
        var d6 = Vector3.Dot(ac, cp);
        if (d6 >= 0 && d5 <= d6)
        {
            return Vector3.Distance(p, c);
        }

        var vb = (d5 * d2) - (d1 * d6);
        if (vb <= 0 && d2 >= 0 && d6 <= 0)
        {
            return Vector3.Distance(p, a + (ac * (d2 / (d2 - d6))));
        }

        var va = (d3 * d6) - (d5 * d4);
        if (va <= 0 && (d4 - d3) >= 0 && (d5 - d6) >= 0)
        {
            var w = (d4 - d3) / ((d4 - d3) + (d5 - d6));
            return Vector3.Distance(p, b + ((c - b) * w));
        }

        var denominator = 1f / (va + vb + vc);
        return Vector3.Distance(p, a + (ab * (vb * denominator)) + (ac * (vc * denominator)));
    }
}
