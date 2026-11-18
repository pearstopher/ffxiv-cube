using System.Numerics;
using Cube.Core;

namespace Cube.Tests;

/// <summary>
/// Checks the box size rules (PROJECT.md, "Cube" and "Zoom", #90): each extent 1–100, default 40; zoom scales all
/// three together keeping their ratios, clamped so no extent leaves 1–100; zooming in means less space.
/// </summary>
public class BoxExtentsTests
{
    [Fact]
    public void Default_size_is_40_on_every_axis()
    {
        Assert.Equal(new Vector3(40), BoxExtents.DefaultSize);
    }

    [Fact]
    public void Corrupt_extent_becomes_the_default()
    {
        Assert.Equal(new Vector3(40, 20, 40), BoxExtents.Clamp(new Vector3(float.NaN, 20, float.NaN)));
    }

    [Fact]
    public void Zoom_out_keeps_the_ratios()
    {
        Assert.Equal(new Vector3(40, 80, 60), BoxExtents.Zoom(new Vector3(20, 40, 30), 2f));
    }

    [Fact]
    public void Zoom_in_means_less_space()
    {
        var zoomed = BoxExtents.Zoom(new Vector3(40), 0.8f);

        Assert.True(zoomed.X < 40 && zoomed.Y < 40 && zoomed.Z < 40);
        Assert.Equal(zoomed.X, zoomed.Y);
        Assert.Equal(zoomed.Y, zoomed.Z);
    }

    [Fact]
    public void Zoom_out_stops_when_the_largest_extent_reaches_100_and_keeps_the_ratios()
    {
        var zoomed = BoxExtents.Zoom(new Vector3(20, 80, 40), 2f);

        AssertClose(new Vector3(25, 100, 50), zoomed);
    }

    [Fact]
    public void Zoom_in_stops_when_the_smallest_extent_reaches_1_and_keeps_the_ratios()
    {
        var zoomed = BoxExtents.Zoom(new Vector3(20, 80, 40), 0.01f);

        AssertClose(new Vector3(1, 4, 2), zoomed);
    }

    [Theory]
    [InlineData(0f)]
    [InlineData(-2f)]
    [InlineData(float.NaN)]
    [InlineData(float.PositiveInfinity)]
    public void Unusable_zoom_factor_leaves_the_size_alone(float factor)
    {
        Assert.Equal(new Vector3(20, 80, 40), BoxExtents.Zoom(new Vector3(20, 80, 40), factor));
    }

    [Fact]
    public void Largest_picks_the_biggest_axis()
    {
        Assert.Equal(90f, BoxExtents.Largest(new Vector3(10, 30, 90)));
    }

    [Fact]
    public void Zoom_at_the_one_to_hundred_limit_does_not_throw()
    {
        var size = new Vector3(1, 100, 37.3f);

        Assert.Equal(size, BoxExtents.Zoom(size, 1.3f));
        Assert.Equal(size, BoxExtents.Zoom(size, 0.7f));
    }

    [Fact]
    public void Clamping_and_zooming_do_not_allocate()
    {
        var size = new Vector3(20, 40, 30);
        Allocations.AssertNone(() =>
        {
            size = BoxExtents.Zoom(BoxExtents.Clamp(size), 1.01f);
        });

        Assert.True(size.X > 20);
    }

    private static void AssertClose(Vector3 expected, Vector3 actual) =>
        Assert.True(Vector3.Distance(expected, actual) < 1e-3f, $"Expected {expected}, got {actual}");
}
