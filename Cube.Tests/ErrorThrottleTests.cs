using Cube.Core;

namespace Cube.Tests;

/// <summary>
/// Checks repeated errors are logged sparingly (CLAUDE.md, "Dalamud rules": every game-facing handler catches and
/// logs; a handler failing every frame must not flood the log).
/// </summary>
public class ErrorThrottleTests
{
    [Fact]
    public void First_error_is_always_reported()
    {
        var throttle = new ErrorThrottle(interval: 100);

        Assert.True(throttle.ShouldReport(now: 5, out var skipped));
        Assert.Equal(0, skipped);
    }

    [Fact]
    public void Errors_within_the_interval_are_counted_but_not_reported()
    {
        var throttle = new ErrorThrottle(interval: 100);
        throttle.ShouldReport(0, out _);

        Assert.False(throttle.ShouldReport(50, out _));
        Assert.False(throttle.ShouldReport(99, out _));
        Assert.Equal(3, throttle.Total);
    }

    [Fact]
    public void Next_report_after_the_interval_says_how_many_were_skipped()
    {
        var throttle = new ErrorThrottle(interval: 100);
        throttle.ShouldReport(0, out _);
        throttle.ShouldReport(10, out _);
        throttle.ShouldReport(20, out _);

        Assert.True(throttle.ShouldReport(100, out var skipped));
        Assert.Equal(2, skipped);
    }

    [Fact]
    public void Counting_errors_does_not_allocate()
    {
        var throttle = new ErrorThrottle(interval: 100);

        Allocations.AssertNone(() =>
        {
            for (var i = 0; i < 1000; i++)
            {
                throttle.ShouldReport(i, out _);
            }
        });
    }
}
