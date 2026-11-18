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

        var view = ViewOrientation.FromDegrees(45, 60);

        settings.SetView(view);

        Assert.Equal(view.Rotation, settings.ViewRotation);
        Assert.Equal(view, settings.View);
        Assert.True(settings.Revision > revision);
    }

    [Fact]
    public void Rolled_view_is_stored_as_is()
    {
        var settings = new CubeSettings();
        var rolled = new ViewOrientation(Quaternion.Concatenate(
            ViewOrientation.Default.Rotation,
            Quaternion.CreateFromAxisAngle(Vector3.UnitZ, 0.5f)));

        settings.SetView(rolled);

        Assert.Equal(rolled, settings.View);
    }

    [Theory]
    [InlineData(45f, 60f)]
    [InlineData(-120f, -20f)]
    [InlineData(0f, 90f)]
    public void Old_file_with_yaw_and_tilt_loads_as_the_same_orientation(float yaw, float pitch)
    {
        // A version-1 file: the old fields are read, the rotation field is missing (so it has its default).
        var settings = new CubeSettings { ViewYawDegrees = yaw, ViewPitchDegrees = pitch };

        settings.Upgrade(1);

        Assert.Equal(ViewOrientation.FromDegrees(yaw, pitch).Rotation.W, settings.View.Rotation.W, 5);
        Assert.Equal(ViewOrientation.FromDegrees(yaw, pitch).Forward.X, settings.View.Forward.X, 5);
        Assert.Equal(ViewOrientation.FromDegrees(yaw, pitch).Up.Z, settings.View.Up.Z, 5);
    }

    [Fact]
    public void Old_file_from_before_the_view_control_loads_the_default_view()
    {
        var settings = new CubeSettings();

        settings.Upgrade(1);

        Assert.Equal(ViewOrientation.Default.Forward, settings.View.Forward);
    }

    [Fact]
    public void Current_file_keeps_its_rotation_and_ignores_the_old_fields()
    {
        var view = ViewOrientation.FromDegrees(80, -10);
        var settings = new CubeSettings { ViewRotation = view.Rotation, ViewYawDegrees = 10, ViewPitchDegrees = 10 };

        settings.Upgrade(CubeSettings.RotationViewVersion);

        Assert.Equal(view, settings.View);
    }

    [Fact]
    public void Old_fields_are_never_written_to_the_file()
    {
        var settings = new CubeSettings();

        Assert.False(settings.ShouldSerializeViewYawDegrees());
        Assert.False(settings.ShouldSerializeViewPitchDegrees());
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
    [InlineData(float.NaN, 0f, 0f, 1f)]
    [InlineData(0f, float.PositiveInfinity, 0f, 1f)]
    [InlineData(0f, 0f, 0f, 0f)]
    public void Corrupt_saved_rotation_becomes_the_default_view(float x, float y, float z, float w)
    {
        var settings = new CubeSettings { ViewRotation = new Quaternion(x, y, z, w) };

        settings.Changed();

        Assert.Equal(ViewOrientation.Default, settings.View);
    }

    [Fact]
    public void Unnormalised_saved_rotation_is_normalised()
    {
        var settings = new CubeSettings { ViewRotation = ViewOrientation.FromDegrees(30, 20).Rotation * 3f };

        settings.Changed();

        Assert.Equal(1f, settings.ViewRotation.Length(), 5);
    }

    [Fact]
    public void Corrupt_old_yaw_and_tilt_give_the_default_view()
    {
        var settings = new CubeSettings { ViewYawDegrees = float.NaN, ViewPitchDegrees = float.PositiveInfinity };

        settings.Upgrade(1);

        Assert.Equal(ViewOrientation.Default.Forward, settings.View.Forward);
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

    [Theory]
    [InlineData(WidgetCorner.BottomRight, 260f, 220f)]
    [InlineData(WidgetCorner.BottomLeft, 60f, 220f)]
    [InlineData(WidgetCorner.TopRight, 260f, 20f)]
    [InlineData(WidgetCorner.TopLeft, 60f, 20f)]
    public void ResizeGizmo_sits_beside_the_rotate_gizmo_inward_from_the_corner(WidgetCorner corner, float x, float y)
    {
        var position = ViewWidget.ResizePosition(new Vector2(10, 20), new Vector2(350, 250), corner, 50);

        Assert.Equal(new Vector2(x, y), position);
    }

    [Theory]
    [InlineData(WidgetCorner.BottomRight, 350f, 250f)]
    [InlineData(WidgetCorner.TopLeft, 350f, 250f)]
    [InlineData(WidgetCorner.BottomLeft, 60f, 400f)]
    [InlineData(WidgetCorner.TopRight, 30f, 30f)]
    [InlineData(WidgetCorner.BottomRight, 10f, 10f)]
    public void TwoGizmos_never_overlap_and_stay_inside_the_region(WidgetCorner corner, float width, float height)
    {
        var min = new Vector2(10, 20);
        var region = new Vector2(width, height);
        var size = ViewWidget.Size(region);

        var rotate = ViewWidget.Position(min, region, corner, size);
        var resize = ViewWidget.ResizePosition(min, region, corner, size);

        Assert.True(MathF.Abs(rotate.X - resize.X) >= size - 0.001f, "squares overlap");
        Assert.Equal(rotate.Y, resize.Y);
        Assert.True(MathF.Min(rotate.X, resize.X) >= min.X - 0.001f);
        Assert.True(MathF.Max(rotate.X, resize.X) + size <= min.X + width + 0.001f);
    }

    [Fact]
    public void Gizmos_shrink_in_a_narrow_window_instead_of_overlapping()
    {
        Assert.Equal(30f, ViewWidget.Size(new Vector2(60f, 500f)), 3);
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
