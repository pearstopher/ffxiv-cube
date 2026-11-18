using System.Numerics;
using Cube.Core;

namespace Cube.Tests;

/// <summary>
/// Checks the saved view orientation and the view control's placement (PROJECT.md, "View orientation control": a
/// small control in a corner of the window, bottom right by default; the chosen orientation is saved; there's a way
/// to reset to the default).
/// </summary>
public class ViewSettingsTests
{
    [Fact]
    public void Default_view_is_from_behind_at_30_degrees_with_the_control_bottom_right()
    {
        var settings = new CubeSettings();

        Assert.Equal(ViewOrientation.Default, settings.View);
        Assert.Equal(WidgetCorner.BottomRight, settings.ViewWidgetCorner);
    }

    [Fact]
    public void Turning_the_view_is_stored_in_the_settings_and_bumps_the_revision()
    {
        var settings = new CubeSettings();
        var revision = settings.Revision;

        settings.SetView(ViewOrientation.FromDegrees(45, 60));

        Assert.Equal(45f, settings.ViewYawDegrees, 3);
        Assert.Equal(60f, settings.ViewPitchDegrees, 3);
        Assert.True(settings.Revision > revision);
    }

    [Fact]
    public void Reset_view_restores_the_default()
    {
        var settings = new CubeSettings();
        settings.SetView(ViewOrientation.FromDegrees(-100, -20));

        settings.ResetView();

        Assert.Equal(ViewOrientation.Default, settings.View);
    }

    [Fact]
    public void Reset_to_defaults_also_resets_the_view_and_corner()
    {
        var settings = new CubeSettings { ViewWidgetCorner = WidgetCorner.TopLeft };
        settings.SetView(ViewOrientation.FromDegrees(80, 10));

        settings.CopyFrom(new CubeSettings());

        Assert.Equal(ViewOrientation.Default, settings.View);
        Assert.Equal(WidgetCorner.BottomRight, settings.ViewWidgetCorner);
    }

    [Theory]
    [InlineData(float.NaN, 10f, 0f, 30f)]
    [InlineData(20f, float.PositiveInfinity, 0f, 30f)]
    [InlineData(400f, 120f, 40f, 90f)]
    [InlineData(-30f, -95f, -30f, -90f)]
    public void Bad_saved_view_values_are_repaired(float yaw, float pitch, float expectedYaw, float expectedPitch)
    {
        var settings = new CubeSettings { ViewYawDegrees = yaw, ViewPitchDegrees = pitch };

        settings.Changed();

        Assert.Equal(expectedYaw, settings.ViewYawDegrees, 3);
        Assert.Equal(expectedPitch, settings.ViewPitchDegrees, 3);
    }

    [Fact]
    public void Unknown_corner_falls_back_to_bottom_right()
    {
        var settings = new CubeSettings { ViewWidgetCorner = (WidgetCorner)42 };

        settings.Changed();

        Assert.Equal(WidgetCorner.BottomRight, settings.ViewWidgetCorner);
    }

    [Theory]
    [InlineData(WidgetCorner.BottomRight, 310f, 220f)]
    [InlineData(WidgetCorner.BottomLeft, 10f, 220f)]
    [InlineData(WidgetCorner.TopRight, 310f, 20f)]
    [InlineData(WidgetCorner.TopLeft, 10f, 20f)]
    public void Control_sits_in_the_chosen_corner(WidgetCorner corner, float x, float y)
    {
        var position = ViewWidget.Position(new Vector2(10, 20), new Vector2(350, 250), corner, 50);

        Assert.Equal(new Vector2(x, y), position);
    }

    [Fact]
    public void Control_stays_outside_the_cube_in_a_square_window()
    {
        // The fitted cube lies within a circle of FillFraction × half the side; the control's inner corner must not
        // reach into it.
        var side = 500f;
        var size = ViewWidget.Size(new Vector2(side, side));
        var radius = ViewProjection.FillFraction * side / 2f;

        var innerCornerDistance = MathF.Sqrt(2f) * ((side / 2f) - size);

        Assert.True(innerCornerDistance >= radius, $"{innerCornerDistance} < {radius}");
    }

    [Theory]
    [InlineData(100f, ViewWidget.MinSize)]
    [InlineData(5000f, ViewWidget.MaxSize)]
    [InlineData(400f, 72f)]
    public void Control_size_follows_the_window_within_limits(float side, float expected)
    {
        Assert.Equal(expected, ViewWidget.Size(new Vector2(side, side * 1.5f)), 3);
    }
}
