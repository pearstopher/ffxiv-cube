using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Cube.TestBridge.Checks;

/// <summary>
/// Soak: draws the worst case for ten minutes and checks nothing grows (PROJECT.md, "Efficiency and memory": zero
/// bytes allocated per frame in steady state, bounded memory, nothing grows with time spent in game). Runs only when
/// asked for with <c>ingame_test.py --soak</c>.
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
/// The process's managed heap (<see cref="GC.GetTotalMemory(bool)"/>) and gen-0 collections are recorded as metrics
/// but not judged: the heap is shared with Dalamud and every other plugin, so it moves for reasons that aren't ours.
/// Since the draw path allocates nothing and every buffer is allocated once at load, the plugin's share can't grow.
/// </para>
/// </remarks>
internal sealed class SoakCheck : BridgeCheck
{
    /// <summary>How long to draw for.</summary>
    private static readonly TimeSpan Duration = TimeSpan.FromMinutes(10);

    /// <summary>Frames drawn before measuring starts.</summary>
    private const int WarmUpFrames = 60;

    /// <summary>The window's open state before the check.</summary>
    private bool wasOpen;

    /// <summary>The user's NPC setting before the check.</summary>
    private bool showedNpcs;

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

        // Players and NPCs at their caps (#74), whatever the user's NPC setting.
        this.showedNpcs = window.Settings.ShowNpcs;
        window.Settings.ShowNpcs = true;
        window.Fakes.UseCrowd();

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
        var started = Stopwatch.GetTimestamp();

        while (Stopwatch.GetElapsedTime(started) < Duration)
        {
            yield return CheckStep.NextFrame;
        }

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
        context.Metric("heapStartBytes", startHeap);
        context.Metric("heapEndBytes", GC.GetTotalMemory(false));
        context.Metric("gen0Collections", GC.CollectionCount(0) - startGen0);

        var summary = string.Create(
            CultureInfo.InvariantCulture,
            $"{frames} frames over {Duration.TotalMinutes:0} min with {window.Players.Count - window.Players.NpcCount} players and {window.Players.NpcCount} NPCs: {bytes} bytes allocated in steady state ({nameFrames} frames built a new name), average {average:0.0000} ms.");
        yield return frames > 0 && bytes == 0 && average < DrawBudgetCheck.BudgetMilliseconds && DrawBudgetCheck.AtCaps(window.Players)
            ? CheckStep.Pass(summary)
            : CheckStep.Fail(summary);
    }

    /// <inheritdoc/>
    public override void Cleanup(BridgeContext context)
    {
        context.Window.Fakes.Clear();
        context.Window.Settings.ShowNpcs = this.showedNpcs;
        context.Window.IsOpen = this.wasOpen;
    }
}
