using System.Numerics;
using Cube.Core;

namespace Cube.Tests;

/// <summary>
/// Checks the cube of space (PROJECT.md, "Exact behaviour rules": Cube, Players, Culling): centred on the player, a box
/// whose width, height and depth are each 1–100 yalms (default 40), inside meaning inside on each axis separately,
/// aligned by its yaw (Camera mode: front face where the camera looks; North up: east right,
/// north forward); containment uses the aligned cube, so things enter and leave at the sides as the camera turns, and
/// is inclusive on faces; players outside are not shown at all. Also the zero-allocation rule ("Efficiency and
/// memory").
/// </summary>
public class CubeSpaceTests
{
    private static readonly Vector3 Player = new(100, 20, -50);

    private static CubeSpace NorthUp(float edge = 40) => new(Player, edge, ViewYaw.NorthUp);

    [Theory]
    [InlineData(40, 40)]
    [InlineData(0.2f, 1)]
    [InlineData(5, 5)]
    [InlineData(10, 10)]
    [InlineData(100, 100)]
    [InlineData(250, 100)]
    [InlineData(float.NaN, 40)]
    public void Edge_is_clamped_to_1_to_100_yalms(float requested, float expected)
    {
        Assert.Equal(new Vector3(expected), NorthUp(requested).Size);
    }

    [Fact]
    public void Each_extent_is_clamped_separately()
    {
        var space = new CubeSpace(Player, new Vector3(0.5f, 60, 250), 0);

        Assert.Equal(new Vector3(1, 60, 100), space.Size);
        Assert.Equal(new Vector3(0.5f, 30, 50), space.Half);
    }

    [Fact]
    public void Default_size_is_40_by_40_by_40()
    {
        Assert.Equal(new Vector3(40f), BoxExtents.DefaultSize);
    }

    [Fact]
    public void Largest_is_the_biggest_extent()
    {
        Assert.Equal(70f, new CubeSpace(Player, new Vector3(20, 70, 30), 0).Largest);
    }

    [Fact]
    public void Inside_on_two_axes_and_outside_on_the_third_is_outside()
    {
        // A box 20 wide, 60 tall, 40 deep, north up: half sizes 10, 30, 20.
        var box = new CubeSpace(Player, new Vector3(20, 60, 40), ViewYaw.NorthUp);

        Assert.True(box.Contains(Player + new Vector3(10, 30, -20)));       // a corner
        Assert.False(box.Contains(Player + new Vector3(10.5f, 0, 0)));      // too far right only
        Assert.False(box.Contains(Player + new Vector3(0, 30.5f, 0)));      // too high only
        Assert.False(box.Contains(Player + new Vector3(0, 0, -20.5f)));     // too far forward only
        Assert.True(box.Contains(Player + new Vector3(0, 25, 0)));          // tall, so 25 up is inside
        Assert.False(NorthUp().Contains(Player + new Vector3(0, 25, 0)));   // but not in the 40 cube
    }

    [Fact]
    public void Width_and_depth_follow_the_cube_axes_not_the_world()
    {
        // Wide and shallow, turned to face east: the width now runs north–south.
        var box = new CubeSpace(Player, new Vector3(80, 40, 10), MathF.PI / 2);

        Assert.True(box.Contains(Player + new Vector3(0, 0, 35)));   // 35 south = 35 to the right
        Assert.False(box.Contains(Player + new Vector3(10, 0, 0)));  // 10 east = 10 forward, past the 5 half-depth
    }

    [Fact]
    public void Player_position_is_the_centre()
    {
        Assert.True(NorthUp().Contains(Player));
        Assert.Equal(Vector3.Zero, NorthUp().ToView(Player));
    }

    [Theory]
    [InlineData(20, 0, 0)]
    [InlineData(-20, 0, 0)]
    [InlineData(0, 20, 0)]
    [InlineData(0, -20, 0)]
    [InlineData(0, 0, 20)]
    [InlineData(0, 0, -20)]
    public void Point_exactly_on_a_face_is_inside(float dx, float dy, float dz)
    {
        Assert.True(NorthUp().Contains(Player + new Vector3(dx, dy, dz)));
    }

    [Theory]
    [InlineData(20, 20, 20)]
    [InlineData(-20, -20, -20)]
    [InlineData(20, -20, 20)]
    public void Corner_is_inside(float dx, float dy, float dz)
    {
        Assert.True(NorthUp().Contains(Player + new Vector3(dx, dy, dz)));
    }

    [Theory]
    [InlineData(20.01f, 0, 0)]
    [InlineData(0, 0, -20.01f)]
    [InlineData(20.01f, 20.01f, 20.01f)]
    public void Point_just_outside_a_face_is_outside(float dx, float dy, float dz)
    {
        Assert.False(NorthUp().Contains(Player + new Vector3(dx, dy, dz)));
    }

    [Theory]
    [InlineData(25)]
    [InlineData(-25)]
    public void Player_far_above_or_below_is_outside_even_when_horizontally_close(float dy)
    {
        Assert.False(NorthUp().Contains(Player + new Vector3(1, dy, 1)));
    }

    [Fact]
    public void Point_near_a_corner_leaves_the_cube_as_the_camera_turns()
    {
        // (19, 0, 19) is inside the world-aligned 40-yalm cube, but 26.9 yalms along the right axis of a cube turned 45°.
        var point = Player + new Vector3(19, 0, 19);

        Assert.True(NorthUp().Contains(point));
        Assert.False(new CubeSpace(Player, 40, MathF.PI / 4).Contains(point));
    }

    [Fact]
    public void Point_outside_a_corner_enters_the_cube_as_the_camera_turns()
    {
        // 25 yalms due east is outside the world-aligned cube but inside one turned 45° (17.7 yalms along each axis).
        var point = Player + new Vector3(25, 0, 0);

        Assert.False(NorthUp().Contains(point));
        Assert.True(new CubeSpace(Player, 40, MathF.PI / 4).Contains(point));
    }

    [Theory]
    [InlineData(0f)]
    [InlineData(0.7f)]
    [InlineData(2.5f)]
    [InlineData(-1.9f)]
    public void Point_ahead_of_the_camera_is_in_front_for_any_yaw(float yaw)
    {
        var space = new CubeSpace(Player, 40, yaw);
        var ahead = Player + (new Vector3(MathF.Sin(yaw), 0, -MathF.Cos(yaw)) * 10);

        Assert.True(space.TryToView(ahead, out var view));
        AssertClose(new Vector3(0, 0, 10), view);
    }

    [Theory]
    [InlineData(0f)]
    [InlineData(0.7f)]
    [InlineData(-2.2f)]
    public void FromView_puts_a_cube_local_point_back_in_the_world(float yaw)
    {
        var space = new CubeSpace(Player, 40, yaw);
        var local = new Vector3(3, -2, 7);

        AssertClose(local, space.ToView(space.FromView(local)));
    }

    [Fact]
    public void Point_outside_is_rejected_before_it_is_transformed()
    {
        Assert.False(NorthUp().TryToView(Player + new Vector3(0, 0, 30), out var view));
        Assert.Equal(default, view);
    }

    [Fact]
    public void North_up_puts_east_on_the_right_and_north_forward()
    {
        var space = NorthUp();

        Assert.Equal(new Vector3(5, 0, 0), space.ToView(Player + new Vector3(5, 0, 0)));   // east → right
        Assert.Equal(new Vector3(0, 0, 5), space.ToView(Player + new Vector3(0, 0, -5)));  // north → forward
        Assert.Equal(new Vector3(0, 3, 0), space.ToView(Player + new Vector3(0, 3, 0)));   // up stays up
    }

    [Fact]
    public void Looking_east_puts_east_forward_and_south_on_the_right()
    {
        var space = new CubeSpace(Player, 40, MathF.PI / 2);

        AssertClose(new Vector3(0, 0, 5), space.ToView(Player + new Vector3(5, 0, 0)));  // east → forward
        AssertClose(new Vector3(5, 0, 0), space.ToView(Player + new Vector3(0, 0, 5)));  // south → right
    }

    [Fact]
    public void Recentred_cube_keeps_the_view_rotation_and_moves_with_the_player()
    {
        var moved = Player + new Vector3(10, 2, -7);
        var fresh = new CubeSpace(moved, new Vector3(60, 30, 50), 0.9f);

        var recentred = new CubeSpace(Player, 40, 0.9f).Recentred(moved, new Vector3(60, 30, 50));

        Assert.Equal(fresh.Centre, recentred.Centre);
        Assert.Equal(fresh.Size, recentred.Size);
        Assert.Equal(fresh.Largest, recentred.Largest);
        Assert.Equal(fresh.Yaw, recentred.Yaw);
        Assert.Equal(fresh.ToView(moved + new Vector3(3, 1, 4)), recentred.ToView(moved + new Vector3(3, 1, 4)));
    }

    [Fact]
    public void Looking_south_mirrors_north_up()
    {
        var space = new CubeSpace(Player, 40, MathF.PI);

        AssertClose(new Vector3(-5, 0, 0), space.ToView(Player + new Vector3(5, 0, 0)));  // east → left
        AssertClose(new Vector3(0, 0, 5), space.ToView(Player + new Vector3(0, 0, 5)));   // south → forward
    }

    [Fact]
    public void Containment_and_transform_do_not_allocate()
    {
        var space = new CubeSpace(Player, 40, 0.7f);
        var point = Player + new Vector3(3, 1, -4);
        var sink = Vector3.Zero;

        Allocations.AssertNone(() =>
        {
            space = new CubeSpace(Player, 40, 0.7f);
            if (space.TryToView(point, out var view))
            {
                sink += view;
            }
        });

        Assert.NotEqual(Vector3.Zero, sink);
    }

    private static void AssertClose(Vector3 expected, Vector3 actual)
    {
        Assert.True(Vector3.Distance(expected, actual) < 1e-4f, $"Expected {expected}, got {actual}");
    }
}
