using System.Collections.Generic;
using System.Globalization;

namespace Cube.TestBridge.Checks;

/// <summary>
/// Checks the draw path stays within PROJECT.md's phase 1 budgets ("Efficiency and memory"):
/// under 0.5 ms per frame on average and zero bytes allocated per frame in steady state.
/// A budget failure is a failed test, not a note (CLAUDE.md).
/// </summary>
/// <remarks>
/// Opens the window, lets it warm up (first-use caches, JIT) and then measures
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

    /// <inheritdoc/>
    public override string Name => "draw-within-budget";

    /// <inheritdoc/>
    public override IEnumerable<CheckStep> Run(BridgeContext context)
    {
        var sampler = context.Window.DrawCost;
        this.wasOpen = context.Window.IsOpen;
        context.Window.IsOpen = true;

        // Measure with players in the cube even when nobody is around: the standard fakes (#16).
        context.Window.Fakes.UseStandardSet();

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
        context.Metric("frames", sampler.Count);
        context.Metric("averageMs", average);
        context.Metric("maxMs", sampler.MaxMilliseconds);
        context.Metric("allocatedBytes", bytes);
        context.Metric("framesWithAllocations", allocatingFrames);

        var summary = string.Create(
            CultureInfo.InvariantCulture,
            $"{sampler.Count} frames: average {average:0.0000} ms (budget {BudgetMilliseconds} ms), {bytes} bytes allocated in {allocatingFrames} frame(s) (budget 0).");

        yield return average < BudgetMilliseconds && bytes == 0
            ? CheckStep.Pass(summary)
            : CheckStep.Fail(summary);
    }

    /// <inheritdoc/>
    public override void Cleanup(BridgeContext context)
    {
        context.Window.Fakes.Clear();
        context.Window.IsOpen = this.wasOpen;
    }
}
