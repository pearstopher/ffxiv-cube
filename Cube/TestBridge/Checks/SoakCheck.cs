using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Cube.TestBridge.Checks;

/// <summary>
/// Soak: draws the worst case for ten minutes, changing zone every half minute, and checks nothing grows (PROJECT.md,
/// "Efficiency and memory": zero bytes allocated per frame in steady state, bounded memory, nothing grows with time
/// spent in game, collision data dropped on a zone change). Runs only when asked for with
/// <c>ingame_test.py --soak</c>.
/// </summary>
/// <remarks>
/// <para>
/// Pass criteria, all about the plugin's own work: zero bytes allocated by the draw path over every steady-state frame
/// of the soak (<see cref="Core.DrawCostSampler.LifetimeAllocatedBytes"/>), and an average under the frame-time
/// budget. Frames where a real player walked into view and their name string was built once are excluded from the
/// byte count (<see cref="CubeWindow.NameBuildFrames"/>) and reported; the fakes' names never change, so in an empty
/// area that count is 0.
/// </para>
/// <para>
/// <b>Collision (#33).</b> The soak runs in the <see cref="WorstCase"/>, collision on. Every
/// <see cref="ZoneChangeInterval"/> it switches the reader to another simulated zone
/// (<see cref="Game.CollisionReader.SimulatedZone"/>), so the cache drops everything and gathers again, as after a
/// teleport. Gathering frames are kept out of the draw cost (PROJECT.md: budgeted background gathering is exempt) and
/// judged on their own: zero bytes over the whole soak, and every zone change followed by a completed gather.
/// </para>
/// <para>
/// The process's managed heap (<see cref="GC.GetTotalMemory(bool)"/>) and gen-0 collections are recorded as metrics
/// but not judged: the heap is shared with Dalamud and every other plugin, so it moves for reasons that aren't ours.
/// Since the draw path allocates nothing and every buffer is allocated once at load, the plugin's share can't grow.
/// </para>
/// </remarks>
internal sealed class SoakCheck : BridgeCheck
{
    /// <summary>How long to draw for.</summary>
    private static readonly TimeSpan Duration = TimeSpan.FromMinutes(10);

    /// <summary>How often the soak changes the simulated zone.</summary>
    private static readonly TimeSpan ZoneChangeInterval = TimeSpan.FromSeconds(30);

    /// <summary>Frames drawn before measuring starts.</summary>
    private const int WarmUpFrames = 60;

    /// <summary>First simulated zone id; far above the game's territory ids so it can't match a real one.</summary>
    private const uint FirstSimulatedZone = 900_000;

    /// <summary>The window's open state before the check.</summary>
    private bool wasOpen;

    /// <summary>The user's settings before the check, restored by <see cref="Cleanup"/>.</summary>
    private readonly Core.CubeSettings saved = new();

    /// <inheritdoc/>
    public override string Name => "soak-ten-minutes-at-the-cap";

    /// <inheritdoc/>
    public override bool SoakOnly => true;

    /// <inheritdoc/>
    public override TimeSpan Timeout => Duration + TimeSpan.FromMinutes(1);

    /// <inheritdoc/>
    public override IEnumerable<CheckStep> Run(BridgeContext context)
    {
        var window = context.Window;
        var sampler = window.DrawCost;
        this.wasOpen = window.IsOpen;
        window.IsOpen = true;

        // Players and NPCs at their caps, collision on at full detail in the largest cube (#74, #33).
        this.saved.CopyFrom(window.Settings);
        WorstCase.Apply(window);
        var cache = window.Collision.Cache;
        foreach (var step in WorstCase.WaitForCollision(cache))
        {
            yield return step;
        }

        var warm = sampler.TotalRecorded;
        while (sampler.TotalRecorded - warm < WarmUpFrames)
        {
            yield return CheckStep.NextFrame;
        }

        var startFrames = sampler.TotalRecorded;
        var startBytes = sampler.LifetimeAllocatedBytes;
        var startTicks = sampler.LifetimeTicks;
        var startNameFrames = window.NameBuildFrames;
        var startNameBytes = window.NameBuildBytes;
        var startHeap = GC.GetTotalMemory(false);
        var startGen0 = GC.CollectionCount(0);
        var gather = window.GatherCost;
        var startGatherFrames = gather.TotalRecorded;
        var startGatherBytes = gather.LifetimeAllocatedBytes;
        var startGatherTicks = gather.LifetimeTicks;
        var startZoneChanges = cache.ZoneChanges;
        var startCompleted = cache.Completed;
        var zonesChanged = 0;
        var zonesGathered = 0;
        var started = Stopwatch.GetTimestamp();
        var lastChange = started;
        var completedAtChange = cache.Completed;

        while (Stopwatch.GetElapsedTime(started) < Duration)
        {
            if (Stopwatch.GetElapsedTime(lastChange) >= ZoneChangeInterval)
            {
                // A gather completed since the last change means that zone's collision came back.
                zonesGathered += zonesChanged > 0 && cache.Completed > completedAtChange ? 1 : 0;
                zonesChanged++;
                window.Collision.SimulatedZone = FirstSimulatedZone + (uint)zonesChanged;
                completedAtChange = cache.Completed;
                lastChange = Stopwatch.GetTimestamp();
            }

            yield return CheckStep.NextFrame;
        }

        zonesGathered += zonesChanged > 0 && cache.Completed > completedAtChange ? 1 : 0;

        var frames = sampler.TotalRecorded - startFrames;
        var bytes = sampler.LifetimeAllocatedBytes - startBytes;
        var average = frames == 0 ? 0 : sampler.ToMilliseconds(sampler.LifetimeTicks - startTicks) / frames;
        var nameFrames = window.NameBuildFrames - startNameFrames;
        context.Metric("frames", frames);
        context.Metric("players", window.Players.Count - window.Players.NpcCount);
        context.Metric("npcs", window.Players.NpcCount);
        context.Metric("allocatedBytes", bytes);
        context.Metric("averageMs", average);
        context.Metric("nameBuildFrames", nameFrames);
        context.Metric("nameBuildBytes", window.NameBuildBytes - startNameBytes);
        var gatherFrames = gather.TotalRecorded - startGatherFrames;
        var gatherBytes = gather.LifetimeAllocatedBytes - startGatherBytes;
        var gatherAverage = gatherFrames == 0 ? 0 : gather.ToMilliseconds(gather.LifetimeTicks - startGatherTicks) / gatherFrames;
        var zoneChanges = cache.ZoneChanges - startZoneChanges;
        context.Metric("collisionBoxes", cache.Ready.Count);
        context.Metric("zoneChanges", zoneChanges);
        context.Metric("zonesGathered", zonesGathered);
        context.Metric("gathers", cache.Completed - startCompleted);
        context.Metric("gatherFrames", gatherFrames);
        context.Metric("gatherBytes", gatherBytes);
        context.Metric("gatherAverageMs", gatherAverage);
        context.Metric("heapStartBytes", startHeap);
        context.Metric("heapEndBytes", GC.GetTotalMemory(false));
        context.Metric("gen0Collections", GC.CollectionCount(0) - startGen0);

        var summary = string.Create(
            CultureInfo.InvariantCulture,
            $"{frames} frames over {Duration.TotalMinutes:0} min with {window.Players.Count - window.Players.NpcCount} players, {window.Players.NpcCount} NPCs and {cache.Ready.Count} collision boxes: {bytes} bytes allocated in steady state ({nameFrames} frames built a new name), average {average:0.0000} ms. " +
            $"{zoneChanges} zone changes, {zonesGathered} gathered again; gathering {gatherFrames} frames, {gatherBytes} bytes, average {gatherAverage:0.0000} ms.");
        yield return frames > 0 && bytes == 0 && average < DrawBudgetCheck.BudgetMilliseconds && DrawBudgetCheck.AtCaps(window.Players)
            && zoneChanges == zonesChanged && zonesGathered == zonesChanged && gatherBytes == 0
            ? CheckStep.Pass(summary)
            : CheckStep.Fail(summary);
    }

    /// <inheritdoc/>
    public override void Cleanup(BridgeContext context)
    {
        context.Window.Fakes.Clear();
        context.Window.Collision.SimulatedZone = null;
        context.Window.Settings.CopyFrom(this.saved);
        context.Window.IsOpen = this.wasOpen;
    }
}
