using System.Collections.Generic;
using System.Globalization;

namespace Cube.TestBridge.Checks;

/// <summary>
/// Checks the draw path stays within PROJECT.md's budgets ("Efficiency and memory"): under 0.5 ms per frame on average
/// and zero bytes allocated per frame in steady state, for phase 1 and, with collision on, phase 2 (#33).
/// A budget failure is a failed test, not a note (CLAUDE.md).
/// </summary>
/// <remarks>
/// Opens the window in the <see cref="WorstCase"/>: the most players the game can have around plus the NPC cap, each
/// with a cube and a full-length label, and collision on at full detail in the largest cube. It waits for a collision
/// gather to finish, lets the window warm up (first-use caches, JIT, name strings) and then measures
/// <see cref="FramesToMeasure"/> frames with the window's <see cref="Core.DrawCostSampler"/>.
/// </remarks>
internal sealed class DrawBudgetCheck : BridgeCheck
{
    /// <summary>The phase 1 average frame-time budget, in milliseconds.</summary>
    public const double BudgetMilliseconds = 0.5;

    /// <summary>Frames drawn before measuring starts.</summary>
    private const int WarmUpFrames = 30;

    /// <summary>Frames measured.</summary>
    private const int FramesToMeasure = 120;

    /// <summary>The window's open state before the check, restored by <see cref="Cleanup"/>.</summary>
    private bool wasOpen;

    /// <summary>The user's settings before the check, restored by <see cref="Cleanup"/>.</summary>
    private readonly Core.CubeSettings saved = new();

    /// <inheritdoc/>
    public override string Name => "draw-within-budget";

    /// <summary>
    /// Room for a collision gather and the 150 frames even when the game runs in the background at about 10 fps (its own frame limiter); the
    /// budget is per frame, so a slow frame rate mustn't fail the check (#79).
    /// </summary>
    public override System.TimeSpan Timeout => System.TimeSpan.FromSeconds(30);

    /// <inheritdoc/>
    public override IEnumerable<CheckStep> Run(BridgeContext context)
    {
        var sampler = context.Window.DrawCost;
        this.wasOpen = context.Window.IsOpen;
        context.Window.IsOpen = true;

        // Measure the worst case even when nobody is around, whatever the user chose (#23, #74, #33).
        this.saved.CopyFrom(context.Window.Settings);
        WorstCase.Apply(context.Window);
        foreach (var step in WorstCase.WaitForCollision(context.Window.Collision.Cache))
        {
            yield return step;
        }

        var start = sampler.TotalRecorded;
        while (sampler.TotalRecorded - start < WarmUpFrames)
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
        var allocatingFrames = sampler.FramesWithAllocations;
        var set = context.Window.Players;
        var players = set.Count - set.NpcCount;
        context.Metric("players", players);
        context.Metric("npcs", set.NpcCount);
        context.Metric("primitives", context.Window.DrawList.Count);
        context.Metric("collisionBoxes", context.Window.ClippedCollision.Count);
        context.Metric("frames", sampler.Count);
        context.Metric("averageMs", average);
        context.Metric("maxMs", sampler.MaxMilliseconds);
        context.Metric("allocatedBytes", bytes);
        context.Metric("framesWithAllocations", allocatingFrames);

        var summary = string.Create(
            CultureInfo.InvariantCulture,
            $"{players} players, {set.NpcCount} NPCs and {context.Window.ClippedCollision.Count} collision boxes, {sampler.Count} frames: average {average:0.0000} ms (budget {BudgetMilliseconds} ms), {bytes} bytes allocated in {allocatingFrames} frame(s) (budget 0).");

        // Fewer than the caps, or collision not gathered, would mean the worst case wasn't measured.
        yield return average < BudgetMilliseconds && bytes == 0 && AtCaps(set) && context.Window.Collision.Cache.Ready.HasData
            ? CheckStep.Pass(summary)
            : CheckStep.Fail(summary);
    }

    /// <inheritdoc/>
    public override void Cleanup(BridgeContext context)
    {
        context.Window.Fakes.Clear();
        context.Window.Settings.CopyFrom(this.saved);
        context.Window.IsOpen = this.wasOpen;
    }

    /// <summary>Whether the set holds at least the player cap and exactly the NPC cap: the worst case (#23, #74).</summary>
    /// <param name="set">The window's player set.</param>
    internal static bool AtCaps(Core.PlayerSet set) =>
        set.Count - set.NpcCount >= Game.PlayerReader.MaxPlayers && set.NpcCount >= Game.PlayerReader.MaxNpcs;
}
