using Cube.Core;

namespace Cube.Tests;

/// <summary>
/// Checks the draw-cost measurements the in-game bridge uses for PROJECT.md's budgets
/// ("Efficiency and memory": under 0.5 ms per frame on average, zero bytes allocated per frame),
/// and that measuring is itself allocation-free with bounded memory.
/// </summary>
public class DrawCostSamplerTests
{
    // 1000 ticks per second makes one tick one millisecond.
    private const long Millisecond = 1;

    [Fact]
    public void Empty_sampler_reports_zero()
    {
        var sampler = new DrawCostSampler(4, 1000);

        Assert.Equal(0, sampler.Count);
        Assert.Equal(0, sampler.AverageMilliseconds);
        Assert.Equal(0, sampler.MaxMilliseconds);
        Assert.Equal(0, sampler.TotalAllocatedBytes);
        Assert.Equal(0, sampler.FramesWithAllocations);
    }

    [Fact]
    public void Average_and_max_time_are_in_milliseconds()
    {
        var sampler = new DrawCostSampler(4, 1000);

        sampler.Record(1 * Millisecond, 0);
        sampler.Record(3 * Millisecond, 0);

        Assert.Equal(2.0, sampler.AverageMilliseconds, 6);
        Assert.Equal(3.0, sampler.MaxMilliseconds, 6);
    }

    [Fact]
    public void Allocations_are_totalled_and_frames_that_allocate_are_counted()
    {
        var sampler = new DrawCostSampler(4, 1000);

        sampler.Record(1, 0);
        sampler.Record(1, 24);
        sampler.Record(1, 40);

        Assert.Equal(64, sampler.TotalAllocatedBytes);
        Assert.Equal(2, sampler.FramesWithAllocations);
    }

    [Fact]
    public void Oldest_frames_are_dropped_when_full()
    {
        var sampler = new DrawCostSampler(2, 1000);

        sampler.Record(100, 8);
        sampler.Record(2, 0);
        sampler.Record(4, 0);

        Assert.Equal(2, sampler.Count);
        Assert.Equal(3.0, sampler.AverageMilliseconds, 6);
        Assert.Equal(0, sampler.TotalAllocatedBytes);
        Assert.Equal(3, sampler.TotalRecorded);
    }

    [Fact]
    public void Reset_forgets_samples_but_keeps_the_running_total()
    {
        var sampler = new DrawCostSampler(4, 1000);
        sampler.Record(5, 16);

        sampler.Reset();

        Assert.Equal(0, sampler.Count);
        Assert.Equal(0, sampler.TotalAllocatedBytes);
        Assert.Equal(1, sampler.TotalRecorded);
    }

    [Fact]
    public void Recording_and_reading_do_not_allocate()
    {
        var sampler = new DrawCostSampler(120, 1000);
        double sink = 0;

        Allocations.AssertNone(() =>
        {
            sampler.Record(2, 0);
            sink += sampler.AverageMilliseconds + sampler.MaxMilliseconds + sampler.TotalAllocatedBytes + sampler.FramesWithAllocations;
        });

        Assert.True(sink > 0);
    }

    [Theory]
    [InlineData(0, 1000)]
    [InlineData(4, 0)]
    public void Invalid_capacity_or_clock_is_rejected(int capacity, long ticksPerSecond)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new DrawCostSampler(capacity, ticksPerSecond));
    }
}
