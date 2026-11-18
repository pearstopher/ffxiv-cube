using System.Numerics;
using Cube.Core;

namespace Cube.Tests;

/// <summary>
/// Checks the settings window's zoom slider (#94; PROJECT.md, "Zoom" and "Control"): its value is the box's largest
/// extent; moving it scales all three extents together keeping their ratios; its range stops where the smallest
/// extent reaches 1 and the largest 100; it stores nothing, so changing an extent moves it; dragging back and forth
/// never drifts.
/// </summary>
public class ZoomSliderTests
{
    private static readonly Vector3 TallBox = new(20, 80, 40);

    [Fact]
    public void Slider_value_is_the_largest_extent()
    {
        var slider = default(ZoomSlider);

        slider.Update(TallBox);

        Assert.Equal(80f, slider.Value);
    }

    [Fact]
    public void Default_cube_reads_40()
    {
        var slider = default(ZoomSlider);

        slider.Update(BoxExtents.DefaultSize);

        Assert.Equal(40f, slider.Value);
    }

    [Fact]
    public void Dragging_keeps_the_ratios()
    {
        var slider = default(ZoomSlider);

        var size = slider.Drag(TallBox, 40f);

        AssertClose(new Vector3(10, 40, 20), size);
    }

    [Fact]
    public void Range_stops_where_the_smallest_reaches_1_and_the_largest_100()
    {
        var slider = default(ZoomSlider);

        slider.Update(TallBox);

        Assert.Equal(4f, slider.Low, 3);
        Assert.Equal(100f, slider.High);
    }

    [Fact]
    public void A_cube_can_zoom_over_the_whole_1_to_100_range()
    {
        var slider = default(ZoomSlider);

        slider.Update(new Vector3(55));

        Assert.Equal(1f, slider.Low, 3);
        Assert.Equal(100f, slider.High);
    }

    [Fact]
    public void A_box_at_the_one_to_hundred_limit_cannot_zoom()
    {
        var slider = default(ZoomSlider);

        slider.Update(new Vector3(1, 100, 50));

        Assert.Equal(100f, slider.Low, 3);
        Assert.Equal(100f, slider.High);
    }

    [Theory]
    [InlineData(5f, 1.25f, 5f, 2.5f)]
    [InlineData(0.5f, 1f, 4f, 2f)]
    [InlineData(500f, 25f, 100f, 50f)]
    public void Values_past_the_limits_clamp_and_keep_the_ratios(float value, float x, float y, float z)
    {
        var slider = default(ZoomSlider);

        var size = slider.Drag(TallBox, value);

        AssertClose(new Vector3(x, y, z), size);
    }

    [Fact]
    public void Slider_to_extents_to_slider_round_trips()
    {
        var slider = default(ZoomSlider);

        for (var value = 40f; value <= 100f; value += 0.5f)
        {
            var size = slider.Drag(TallBox, value);
            slider.Update(size);

            Assert.Equal(value, slider.Value, 3);
        }
    }

    [Fact]
    public void Dragging_back_and_forth_returns_exactly_to_the_start()
    {
        var slider = default(ZoomSlider);
        var size = new Vector3(23.7f, 61.3f, 47.9f);
        var start = size;
        slider.Update(size);
        var startValue = slider.Value;

        for (var i = 0; i < 1000; i++)
        {
            size = slider.Drag(size, i % 2 == 0 ? 97.1f : 31.3f);
        }

        size = slider.Drag(size, startValue);
        slider.EndDrag();

        Assert.Equal(start, size);
    }

    [Fact]
    public void A_new_drag_starts_from_the_size_at_that_point()
    {
        var slider = default(ZoomSlider);
        slider.Drag(TallBox, 50f);
        slider.EndDrag();

        var size = slider.Drag(new Vector3(40), 20f);

        Assert.Equal(new Vector3(20), size);
    }

    [Fact]
    public void Changing_one_extent_moves_the_slider()
    {
        var slider = default(ZoomSlider);
        slider.Update(TallBox);

        slider.Update(TallBox with { Y = 30 });

        Assert.Equal(40f, slider.Value);
        Assert.Equal(2f, slider.Low, 3);
    }

    [Fact]
    public void Corrupt_size_reads_as_the_clamped_size()
    {
        var slider = default(ZoomSlider);

        slider.Update(new Vector3(float.NaN, 500, 5));

        Assert.Equal(100f, slider.Value);
    }

    [Fact]
    public void Updating_and_dragging_do_not_allocate()
    {
        var slider = default(ZoomSlider);
        var size = TallBox;
        Allocations.AssertNone(() =>
        {
            slider.Update(size);
            size = slider.Drag(size, slider.Value * 1.01f);
            slider.EndDrag();
        });

        Assert.True(size.Y > 80);
    }

    private static void AssertClose(Vector3 expected, Vector3 actual) =>
        Assert.True(Vector3.Distance(expected, actual) < 1e-3f, $"Expected {expected}, got {actual}");
}
