using System.Collections.Generic;
using System.Globalization;

namespace Cube.TestBridge.Checks;

/// <summary>
/// Checks the draw path stays within PROJECT.md's phase 1 budgets ("Efficiency and memory"):
/// under 0.5 ms per frame on average and zero bytes allocated per frame in steady state.
/// A budget failure is a failed test, not a note (CLAUDE.md).
/// </summary>
/// <remarks>
/// Opens the window with <see cref="FakePlayers.UseCrowd"/> (the most players the game can have around plus the NPC
/// cap, each with a cube and a full-length label), lets it warm up (first-use caches, JIT, name strings) and then measures
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

    /// <summary>The user's NPC setting before the check, restored by <see cref="Cleanup"/>.</summary>
    private bool showedNpcs;

    /// <inheritdoc/>
    public override string Name => "draw-within-budget";

    /// <summary>
    /// Room for the 150 frames even when the game runs in the background at about 10 fps (its own frame limiter); the
    /// budget is per frame, so a slow frame rate mustn't fail the check (#79).
    /// </summary>
    public override System.TimeSpan Timeout => System.TimeSpan.FromSeconds(30);

    /// <inheritdoc/>
    public override IEnumerable<CheckStep> Run(BridgeContext context)
    {
        var sampler = context.Window.DrawCost;
        this.wasOpen = context.Window.IsOpen;
        context.Window.IsOpen = true;

        // Measure the worst case even when nobody is around: a full crowd of fake players and NPCs, all labelled
        // (#23, #74). NPCs are turned on for the check so the NPC cap is measured whatever the user chose.
        this.showedNpcs = context.Window.Settings.ShowNpcs;
        context.Window.Settings.ShowNpcs = true;
        context.Window.Fakes.UseCrowd();

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

        // Fewer than the caps would mean the worst case wasn't measured.
        yield return average < BudgetMilliseconds && bytes == 0 && AtCaps(set)
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

    /// <summary>Whether the set holds at least the player cap and exactly the NPC cap: the worst case (#23, #74).</summary>
    /// <param name="set">The window's player set.</param>
    internal static bool AtCaps(Core.PlayerSet set) =>
        set.Count - set.NpcCount >= Game.PlayerReader.MaxPlayers && set.NpcCount >= Game.PlayerReader.MaxNpcs;
}
