using System.Numerics;
using Cube.Core;

namespace Cube.Tests;

/// <summary>
/// Checks the cube of space (PROJECT.md, "Exact behaviour rules": Cube, View, Players): axis-aligned, centred on
/// the player, edge 10–100 yalms (default 40); players outside are not shown at all; containment is inclusive on
/// faces; the view turns by yaw (north up: east right, north forward). Also the zero-allocation rule
/// ("Efficiency and memory").
/// </summary>
public class CubeSpaceTests
{
    private static readonly Vector3 Player = new(100, 20, -50);

    private static CubeSpace NorthUp(float edge = 40) => new(Player, edge, ViewYaw.NorthUp);

    [Theory]
    [InlineData(40, 40)]
    [InlineData(5, 10)]
    [InlineData(10, 10)]
    [InlineData(100, 100)]
    [InlineData(250, 100)]
    [InlineData(float.NaN, 40)]
    public void Edge_is_clamped_to_10_to_100_yalms(float requested, float expected)
    {
        Assert.Equal(expected, NorthUp(requested).Edge);
    }

    [Fact]
    public void Default_edge_is_40_yalms()
    {
        Assert.Equal(40f, CubeSpace.DefaultEdge);
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
    public void Cube_is_axis_aligned_so_yaw_does_not_change_containment()
    {
        // (19, 0, 19) is inside an axis-aligned 40-yalm cube but would fall outside a cube rotated by 45 degrees.
        var point = Player + new Vector3(19, 0, 19);

        Assert.True(new CubeSpace(Player, 40, MathF.PI / 4).Contains(point));
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
