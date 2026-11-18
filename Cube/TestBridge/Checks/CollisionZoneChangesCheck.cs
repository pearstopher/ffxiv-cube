using System;
using System.Collections.Generic;
using System.Globalization;
using Cube.Core;

namespace Cube.TestBridge.Checks;

/// <summary>
/// Checks memory stays bounded across zone changes (PROJECT.md, "Bounded memory": caches have fixed caps, nothing
/// grows with time spent in game, and collision data is dropped when you change zone; #33).
/// </summary>
/// <remarks>
/// <para>
/// No human is in game to teleport, so the check changes zone by putting simulated zone ids in place of the real one
/// (<see cref="Game.CollisionReader.SimulatedZone"/>) and finally switching back to the real zone. The collision cache
/// sees exactly what it sees after a teleport: a different zone id, so it drops both sets and gathers afresh. Only the
/// scene stays the same, which lets the check compare each new gather with the first.
/// </para>
/// <para>
/// Pass criteria: every change is counted (<see cref="CollisionCache.ZoneChanges"/>) and followed by a completed
/// gather; the ready set is always one of the same two buffers (nothing is allocated for a new zone) at the same
/// capacity; and no gathering or drawing frame allocates. The process heap is recorded but not judged, as in
/// <see cref="SoakCheck"/>.
/// </para>
/// </remarks>
internal sealed class CollisionZoneChangesCheck : BridgeCheck
{
    /// <summary>How many simulated zone changes are made (plus one back to the real zone).</summary>
    private const int Changes = 10;

    /// <summary>First simulated zone id; far above the game's territory ids so it can't match a real one.</summary>
    private const uint FirstSimulatedZone = 800_000;

    /// <summary>The user's settings before the check, restored by <see cref="Cleanup"/>.</summary>
    private readonly CubeSettings saved = new();

    /// <summary>The window's open state before the check, restored by <see cref="Cleanup"/>.</summary>
    private bool wasOpen;

    /// <inheritdoc/>
    public override string Name => "collision-zone-changes";

    /// <inheritdoc/>
    /// <remarks>Twelve gathers in the largest cube, each possibly a few frames at background frame rates (about 10 fps).</remarks>
    public override TimeSpan Timeout => TimeSpan.FromSeconds(60);

    /// <inheritdoc/>
    public override IEnumerable<CheckStep> Run(BridgeContext context)
    {
        var window = context.Window;
        var settings = window.Settings;
        var cache = window.Collision.Cache;
        this.saved.CopyFrom(settings);
        this.wasOpen = window.IsOpen;
        window.IsOpen = true;
        settings.ShowCollision = true;
        settings.CollisionDetail = CollisionDetail.Full;
        settings.CubeEdge = CubeSpace.MaxEdge;
        settings.Changed();

        // Two gathers, so both of the cache's sets have been the ready one.
        foreach (var step in WorstCase.WaitForCollision(cache))
        {
            yield return step;
        }

        var first = cache.Ready;
        foreach (var step in WorstCase.WaitForCollision(cache))
        {
            yield return step;
        }

        var second = cache.Ready;
        var capacity = first.Capacity;
        var boxesBefore = cache.Ready.Count;
        var startZoneChanges = cache.ZoneChanges;
        var startGatherBytes = window.GatherCost.LifetimeAllocatedBytes;
        var startDrawBytes = window.DrawCost.LifetimeAllocatedBytes;
        var startHeap = GC.GetTotalMemory(false);
        var fewest = int.MaxValue;
        var most = 0;
        string? problem = null;

        for (var i = 0; i <= Changes && problem is null; i++)
        {
            var before = cache.Completed;
            window.Collision.SimulatedZone = i < Changes ? FirstSimulatedZone + (uint)i : null;
            while (cache.Completed == before || cache.IsGathering)
            {
                yield return CheckStep.NextFrame;
            }

            var ready = cache.Ready;
            fewest = Math.Min(fewest, ready.Count);
            most = Math.Max(most, ready.Count);
            if (!ReferenceEquals(ready, first) && !ReferenceEquals(ready, second))
            {
                problem = $"Zone change {i + 1} gathered into a new set instead of reusing the cache's two.";
            }
            else if (ready.Capacity != capacity || ready.Count > capacity)
            {
                problem = $"Zone change {i + 1}: {ready.Count} boxes at capacity {ready.Capacity}, expected capacity {capacity}.";
            }
        }

        var zoneChanges = cache.ZoneChanges - startZoneChanges;
        var gatherBytes = window.GatherCost.LifetimeAllocatedBytes - startGatherBytes;
        var drawBytes = window.DrawCost.LifetimeAllocatedBytes - startDrawBytes;
        context.Metric("zoneChanges", zoneChanges);
        context.Metric("boxesBefore", boxesBefore);
        context.Metric("fewestBoxes", fewest);
        context.Metric("mostBoxes", most);
        context.Metric("capacity", capacity);
        context.Metric("gatherBytes", gatherBytes);
        context.Metric("drawBytes", drawBytes);
        context.Metric("heapStartBytes", startHeap);
        context.Metric("heapEndBytes", GC.GetTotalMemory(false));

        if (problem is not null)
        {
            yield return CheckStep.Fail(problem);
            yield break;
        }

        var summary = string.Create(
            CultureInfo.InvariantCulture,
            $"{zoneChanges} zone changes, each dropped and gathered again into the same two sets (capacity {capacity}): {boxesBefore} boxes before, {fewest}-{most} after; {gatherBytes} bytes allocated gathering, {drawBytes} drawing (budget 0).");
        yield return zoneChanges == Changes + 1 && gatherBytes == 0 && drawBytes == 0
            ? CheckStep.Pass(summary)
            : CheckStep.Fail(summary);
    }

    /// <inheritdoc/>
    public override void Cleanup(BridgeContext context)
    {
        context.Window.Collision.SimulatedZone = null;
        context.Window.Settings.CopyFrom(this.saved);
        context.Window.IsOpen = this.wasOpen;
    }
}
