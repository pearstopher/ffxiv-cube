using System.Numerics;
using Cube.Core;

namespace Cube.Tests;

/// <summary>
/// Checks the corner gizmo's shared model matrix (PROJECT.md, "View orientation control"; #90 "Control"): the axis
/// handles change width, height or depth in the cube's own axes whatever the orientation, the centre handle zooms
/// keeping the ratios, a turn never changes the size and a resize never changes the view. ImGuizmo's handles are
/// simulated by scaling the matrix's rows (its local axes) the way its scale operation does.
/// </summary>
public class GizmoModelTests
{
    private static readonly ViewOrientation Turned = ViewOrientation.FromDegrees(70, -20);

    [Fact]
    public void Default_cube_has_scale_one_so_the_model_is_the_plain_view()
    {
        Assert.Equal(ViewOrientation.Default.ToGizmoModel(), GizmoModel.Build(ViewOrientation.Default, new Vector3(40)));
    }

    [Fact]
    public void Untouched_model_changes_nothing()
    {
        var size = new Vector3(20, 70, 35);
        var model = GizmoModel.Build(Turned, size);

        var change = GizmoModel.Read(model, Turned, size);

        Assert.False(change.ViewChanged);
        Assert.False(change.SizeChanged);
        Assert.Equal(size, change.Size);
        Assert.Equal(Turned, change.View);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void Axis_handle_changes_only_its_own_extent_and_not_the_view(int axis)
    {
        var size = new Vector3(20, 40, 60);
        var stretch = new Vector3(axis == 0 ? 1.5f : 1, axis == 1 ? 1.5f : 1, axis == 2 ? 1.5f : 1);
        var model = Matrix4x4.CreateScale(stretch) * GizmoModel.Build(Turned, size);

        var change = GizmoModel.Read(model, Turned, size);

        Assert.True(change.SizeChanged);
        Assert.False(change.ViewChanged);
        Assert.Equal(Turned, change.View);
        AssertClose(size * stretch, change.Size);
        for (var i = 0; i < 3; i++)
        {
            if (i != axis)
            {
                Assert.Equal(Component(size, i), Component(change.Size, i));
            }
        }
    }

    [Fact]
    public void Axis_handle_is_clamped_to_1_to_100()
    {
        var size = new Vector3(40);
        var model = Matrix4x4.CreateScale(new Vector3(5, 0.001f, 1)) * GizmoModel.Build(ViewOrientation.Default, size);

        var change = GizmoModel.Read(model, ViewOrientation.Default, size);

        Assert.Equal(new Vector3(100, 1, 40), change.Size);
    }

    [Fact]
    public void Centre_handle_zooms_keeping_ratios_even_at_the_limit()
    {
        var size = new Vector3(20, 80, 40);
        var model = Matrix4x4.CreateScale(2f) * GizmoModel.Build(Turned, size);

        var change = GizmoModel.Read(model, Turned, size);

        Assert.True(change.SizeChanged);
        Assert.False(change.ViewChanged);
        AssertClose(new Vector3(25, 100, 50), change.Size);
    }

    [Fact]
    public void Turning_a_box_keeps_its_size_exactly()
    {
        var size = new Vector3(15, 90, 45);
        var after = ViewOrientation.FromDegrees(95, -5);

        // ImGuizmo's rotate keeps each row's length and turns the rows.
        var model = GizmoModel.Build(after, size);
        var change = GizmoModel.Read(model, Turned, size);

        Assert.True(change.ViewChanged);
        Assert.False(change.SizeChanged);
        Assert.Equal(size, change.Size);
        AssertClose(after.Forward, change.View.Forward);
        AssertClose(after.Up, change.View.Up);
    }

    [Fact]
    public void Reading_does_not_allocate()
    {
        var size = new Vector3(20, 40, 60);
        var model = Matrix4x4.CreateScale(1.1f, 1, 1) * GizmoModel.Build(Turned, size);
        var sink = Vector3.Zero;

        Allocations.AssertNone(() =>
        {
            sink += GizmoModel.Read(model, Turned, size).Size;
        });

        Assert.NotEqual(Vector3.Zero, sink);
    }

    private static float Component(Vector3 v, int i) => i == 0 ? v.X : i == 1 ? v.Y : v.Z;

    private static void AssertClose(Vector3 expected, Vector3 actual) =>
        Assert.True(Vector3.Distance(expected, actual) < 1e-3f, $"Expected {expected}, got {actual}");
}
