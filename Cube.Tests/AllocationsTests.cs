using Xunit.Sdk;

namespace Cube.Tests;

/// <summary>
/// Checks the <see cref="Allocations"/> helper itself, which enforces PROJECT.md's
/// "zero bytes allocated per frame" budget in unit tests: it must pass allocation-free code
/// and catch code that allocates, including small amortised growth.
/// </summary>
public class AllocationsTests
{
    private readonly int[] buffer = new int[64];
    private object? sink;

    [Fact]
    public void Code_that_does_not_allocate_passes()
    {
        Allocations.AssertNone(() =>
        {
            for (var i = 0; i < this.buffer.Length; i++)
            {
                this.buffer[i] = i * i;
            }
        });
    }

    [Fact]
    public void Code_that_allocates_fails()
    {
        var ex = Assert.Throws<XunitException>(() => Allocations.AssertNone(() => this.sink = new int[4]));

        Assert.Contains("bytes were allocated", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Boxing_is_caught()
    {
        var value = 42;

        Assert.Throws<XunitException>(() => Allocations.AssertNone(() => this.sink = value));
    }

    [Fact]
    public void Growth_on_a_later_call_is_caught()
    {
        // A list that only resizes now and then: the warm-up run alone wouldn't notice.
        var list = new List<int>(1);

        Assert.Throws<XunitException>(() => Allocations.AssertNone(() => list.Add(1), iterations: 20));
    }

    [Fact]
    public void One_time_warm_up_allocations_are_not_counted()
    {
        int[]? cache = null;

        Allocations.AssertNone(() => cache ??= new int[16]);
    }
}
