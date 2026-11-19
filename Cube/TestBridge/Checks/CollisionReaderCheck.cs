using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using Cube.Core;
using FFXIVClientStructs.FFXIV.Common.Component.BGCollision;

namespace Cube.TestBridge.Checks;

/// <summary>
/// Checks the collision reader (#30; PROJECT.md, "Collision (phase 2)" and "Efficiency and memory") in the running
/// game: once the first gather completes there is a box under the player's feet, and repeated gathers stay within
/// their per-frame time budget, allocate nothing and stay under the box cap.
/// </summary>
/// <remarks>
/// <para>
/// <b>Under the feet.</b> The game's own downward raycast from just above the player finds the floor they stand on;
/// the check passes if that point lies in one of the gathered boxes (within <see cref="HitTolerance"/>). This is the
/// same cross-check #28's probe used, so the reader and the game's collision agree. The raycast is a read-only game
/// function that takes the module's shared lock itself.
/// </para>
/// <para>
/// <b>Budgets.</b> The check forces <see cref="Gathers"/> fresh gathers (<see cref="CollisionCache.Invalidate"/>) and
/// reads the window's <see cref="CubeWindow.GatherCost"/>: no gathering frame may allocate, and none may run longer
/// than <see cref="MaxFrameMilliseconds"/>, the budget plus room for the one collider that crosses it (the cache only
/// checks the clock between colliders). The draw budget itself is still checked by <see cref="DrawBudgetCheck"/>,
/// which no longer counts gathering.
/// </para>
/// </remarks>
internal sealed class CollisionReaderCheck : BridgeCheck
{
    /// <summary>How many gathers are forced and measured.</summary>
    private const int Gathers = 10;

    /// <summary>How far above the player's feet the test ray starts, in yalms.</summary>
    private const float RayStartHeight = 1f;

    /// <summary>How far the test ray reaches down, in yalms.</summary>
    private const float RayLength = 5f;

    /// <summary>How far the floor hit may be outside a box and still count as inside (box rounding, compressed vertices; as in #28).</summary>
    private const float HitTolerance = 0.25f;

    /// <summary>
    /// The longest a gathering frame may take: the 0.2 ms budget plus the collider that crosses it (one mesh tree walk,
    /// a few hundred nodes at most) and the timer's own overhead.
    /// </summary>
    internal const double MaxFrameMilliseconds = 1.0;

    /// <summary>The window's open state before the check, restored by <see cref="Cleanup"/>.</summary>
    private bool wasOpen;

    /// <inheritdoc/>
    public override string Name => "collision-reader";

    /// <inheritdoc/>
    /// <remarks>Room for eleven gathers at background frame rates (about 10 fps), each a few frames.</remarks>
    public override TimeSpan Timeout => TimeSpan.FromSeconds(30);

    /// <inheritdoc/>
    public override IEnumerable<CheckStep> Run(BridgeContext context)
    {
        var window = context.Window;
        var cache = window.Collision.Cache;
        this.wasOpen = window.IsOpen;
        window.IsOpen = true;

        // The first gather (or one already done), then the floor check against it.
        while (!cache.Ready.HasData || cache.IsGathering)
        {
            yield return CheckStep.NextFrame;
        }

        if (!Game.PlayerReader.TryGetLocalPosition(out var feet))
        {
            yield return CheckStep.Fail("Not logged in: no local player to stand on anything. Log in and leave the game running.");
            yield break;
        }

        var floor = FindFloor(feet, cache.Ready.Items, out var hit, out var floorBox);
        context.Metric("boxes", cache.Ready.Count);
        context.Metric("offered", cache.Ready.Offered);
        context.Metric("merged", cache.Ready.Merged);
        context.Metric("dropped", cache.Ready.Dropped);
        context.Metric("invalid", cache.Ready.Invalid);

        // #123 probe: how much space the turned boxes save at this real location (volume in cubic yalms, summed over
        // the kept boxes; "axis" is what the world-axis boxes around them would cover).
        var turned = 0;
        double ownVolume = 0, axisVolume = 0;
        foreach (var box in cache.Ready.Items)
        {
            var own = box.OwnHalfSize;
            var axis = box.HalfSize;
            ownVolume += 8.0 * own.X * own.Y * own.Z;
            axisVolume += 8.0 * axis.X * axis.Y * axis.Z;
            if (box.IsTurned)
            {
                turned++;
            }
        }

        context.Metric("turnedBoxes", turned);
        context.Metric("volumeOrientedYalms3", ownVolume);
        context.Metric("volumeAxisYalms3", axisVolume);
        if (floor == Floor.NoRayHit)
        {
            yield return CheckStep.Fail(string.Create(
                CultureInfo.InvariantCulture,
                $"The game's raycast found nothing within {RayLength} yalms under the player at {feet} (flying or swimming?). Stand on the ground and run again."));
            yield break;
        }

        if (floor == Floor.NotInABox)
        {
            yield return CheckStep.Fail(string.Create(
                CultureInfo.InvariantCulture,
                $"The floor under the player ({hit}) is in none of the {cache.Ready.Count} gathered boxes."));
            yield break;
        }

        // Budgets over fresh gathers in steady state.
        window.GatherCost.Reset();
        var startCompleted = cache.Completed;
        var startRestarts = cache.Restarts;
        for (var i = 0; i < Gathers; i++)
        {
            var before = cache.Completed;
            cache.Invalidate();
            while (cache.Completed == before)
            {
                yield return CheckStep.NextFrame;
            }
        }

        var cost = window.GatherCost;
        var gathered = cache.Completed - startCompleted;
        var frames = cost.Count;
        var maxMs = cost.MaxMilliseconds;
        var averageMs = cost.AverageMilliseconds;
        var bytes = cost.TotalAllocatedBytes;
        var withinCap = cache.Ready.Count <= cache.Ready.Capacity;
        context.Metric("gathers", gathered);
        context.Metric("gatherFrames", frames);
        context.Metric("gatherAverageMs", averageMs);
        context.Metric("gatherMaxMs", maxMs);
        context.Metric("gatherBytes", bytes);
        context.Metric("restarts", cache.Restarts - startRestarts);

        var summary = string.Create(
            CultureInfo.InvariantCulture,
            $"Floor at {hit} is in box {floorBox.Min}-{floorBox.Max}; {cache.Ready.Count} boxes kept of {cache.Ready.Offered} offered ({cache.Ready.Merged} merged, {cache.Ready.Dropped} dropped, cap {cache.Ready.Capacity}). " +
            $"{gathered} gathers over {frames} frames: average {averageMs:0.0000} ms, max {maxMs:0.0000} ms (limit {MaxFrameMilliseconds} ms, budget {CollisionCache.DefaultBudgetMilliseconds} ms), {bytes} bytes allocated (budget 0).");
        yield return bytes == 0 && maxMs <= MaxFrameMilliseconds && withinCap
            ? CheckStep.Pass(summary)
            : CheckStep.Fail(summary);
    }

    /// <inheritdoc/>
    public override void Cleanup(BridgeContext context) => context.Window.IsOpen = this.wasOpen;

    /// <summary>
    /// Casts the game's raycast straight down from just above the player and looks for a gathered box holding the hit.
    /// </summary>
    /// <param name="feet">The player's position (their feet).</param>
    /// <param name="boxes">The gathered boxes.</param>
    /// <param name="hit">Where the ray hit the floor.</param>
    /// <param name="box">The box holding the hit, if any.</param>
    internal static Floor FindFloor(Vector3 feet, ReadOnlySpan<CollisionBox> boxes, out Vector3 hit, out CollisionBox box)
    {
        box = default;
        hit = default;
        var origin = feet + new Vector3(0, RayStartHeight, 0);
        if (!BGCollisionModule.RaycastMaterialFilter(origin, -Vector3.UnitY, out var result, RayLength))
        {
            return Floor.NoRayHit;
        }

        hit = result.Point;
        var tolerance = new Vector3(HitTolerance);
        var around = new CollisionBox(hit - tolerance, hit + tolerance);
        foreach (ref readonly var candidate in boxes)
        {
            if (candidate.Intersects(around))
            {
                box = candidate;
                return Floor.InABox;
            }
        }

        return Floor.NotInABox;
    }

    /// <summary>What <see cref="FindFloor"/> found.</summary>
    internal enum Floor
    {
        /// <summary>The floor hit is inside a gathered box.</summary>
        InABox,

        /// <summary>The ray hit a floor that no gathered box holds.</summary>
        NotInABox,

        /// <summary>The ray hit nothing.</summary>
        NoRayHit,
    }
}
