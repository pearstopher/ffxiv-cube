using Xunit.Sdk;

namespace Cube.Tests;

/// <summary>
/// Test helper for the "zero bytes allocated per frame" budget (PROJECT.md, "Efficiency and memory"):
/// hot-path Core code is run under <see cref="AssertNone"/> so an accidental allocation
/// (LINQ, a capturing lambda, boxing, string building) fails a unit test long before it shows up in game.
/// </summary>
/// <remarks>
/// Allocations are measured with <see cref="GC.GetAllocatedBytesForCurrentThread"/>, the same counter
/// the in-game bridge uses, so the two agree. The action runs once first, unmeasured, so one-time costs
/// (JIT, static initialisers, a cache filled on first use) don't count: the budget is about steady state.
/// Create the delegate before calling; the helper itself allocates nothing while measuring.
/// </remarks>
internal static class Allocations
{
    /// <summary>
    /// Fails the test if <paramref name="action"/> allocates any managed memory after its warm-up run.
    /// </summary>
    /// <param name="action">The code to measure, e.g. one frame's worth of Core work.</param>
    /// <param name="iterations">How many times to run it while measuring; more runs catch amortised growth such as a list resizing every few calls.</param>
    public static void AssertNone(Action action, int iterations = 10)
    {
        var bytes = Measure(action, iterations);
        if (bytes != 0)
        {
            throw new XunitException($"Expected no allocations, but {bytes} bytes were allocated over {iterations} runs.");
        }
    }

    /// <summary>
    /// Returns the bytes <paramref name="action"/> allocates over <paramref name="iterations"/> runs, after one unmeasured warm-up run.
    /// </summary>
    /// <param name="action">The code to measure.</param>
    /// <param name="iterations">How many measured runs.</param>
    public static long Measure(Action action, int iterations = 10)
    {
        ArgumentNullException.ThrowIfNull(action);
        ArgumentOutOfRangeException.ThrowIfLessThan(iterations, 1);

        action();

        var before = GC.GetAllocatedBytesForCurrentThread();
        for (var i = 0; i < iterations; i++)
        {
            action();
        }

        return GC.GetAllocatedBytesForCurrentThread() - before;
    }
}
