using System.Numerics;
using Cube.Core;

namespace Cube.Tests;

/// <summary>
/// Checks the user's view orientation and the projection it drives (PROJECT.md, "View orientation control": the view
/// control turns how the cube is shown, yaw and tilt; default from above and behind at 30°, front face away; the
/// orientation is saved and can be reset; "Efficiency and memory": no per-frame allocations).
/// </summary>
public class ViewOrientationTests
{
    private const float Deg = MathF.PI / 180f;
    private static readonly Vector2 Origin = new(200, 150);

    private static ViewProjection Projection(ViewOrientation view) => new(view, Origin, pixelsPerYalm: 10);

    private static CubeSpace Space(float yaw = 0) => new(Vector3.Zero, 40, yaw);

    private static IEnumerable<Vector3> SamplePoints()
    {
        yield return new Vector3(5, 0, 0);
        yield return new Vector3(0, 7, 0);
        yield return new Vector3(0, 0, -9);
        yield return new Vector3(3, -4, 12);
        yield return new Vector3(-20, 20, 20);
    }

    [Fact]
    public void Default_view_reproduces_the_old_fixed_30_degree_projection()
    {
        // The projection before #63: up = y·cos + z·sin, depth = z·cos − y·sin, right unchanged.
        var (sin, cos) = MathF.SinCos(30 * Deg);
        var projection = Projection(ViewOrientation.Default);

        foreach (var p in SamplePoints())
        {
            var screen = projection.ToScreen(p);
            Assert.Equal(Origin.X + (p.X * 10), screen.X, 3);
            Assert.Equal(Origin.Y - (((p.Y * cos) + (p.Z * sin)) * 10), screen.Y, 3);
            Assert.Equal((p.Z * cos) - (p.Y * sin), projection.Depth(p), 4);
        }

        Assert.Equal(new Vector3(0, sin, -cos), projection.TowardsViewer);
    }

    [Theory]
    [InlineData(0f, 30f)]
    [InlineData(45f, 30f)]
    [InlineData(-120f, 10f)]
    [InlineData(170f, -40f)]
    [InlineData(30f, 90f)]
    [InlineData(-75f, -90f)]
    public void View_axes_are_orthonormal_at_any_orientation(float yaw, float pitch)
    {
        var view = ViewOrientation.FromDegrees(yaw, pitch);

        Assert.Equal(1f, view.Right.Length(), 4);
        Assert.Equal(1f, view.Up.Length(), 4);
        Assert.Equal(1f, view.Forward.Length(), 4);
        Assert.Equal(0f, Vector3.Dot(view.Right, view.Up), 4);
        Assert.Equal(0f, Vector3.Dot(view.Right, view.Forward), 4);
        Assert.Equal(0f, Vector3.Dot(view.Up, view.Forward), 4);
        Assert.Equal(0f, view.Right.Y, 6); // the view never rolls
    }

    [Fact]
    public void Turning_the_view_a_quarter_turn_right_shows_the_cube_from_its_left_side()
    {
        // Viewer heading 90°: looking along +X, so a point ahead (+Z) appears on the left of the screen.
        var projection = Projection(ViewOrientation.FromDegrees(90, 0));

        Assert.Equal(Origin.X - 50, projection.ToScreen(new Vector3(0, 0, 5)).X, 3);
        Assert.True(projection.Depth(new Vector3(5, 0, 0)) > projection.Depth(new Vector3(-5, 0, 0)));
    }

    [Fact]
    public void Looking_straight_down_puts_forward_at_the_top_of_the_screen()
    {
        var projection = Projection(ViewOrientation.FromDegrees(0, 90));

        Assert.Equal(Origin.Y - 50, projection.ToScreen(new Vector3(0, 0, 5)).Y, 3);
        Assert.Equal(Origin, projection.ToScreen(new Vector3(0, 8, 0)));
        Assert.True(projection.Depth(new Vector3(0, 5, 0)) < projection.Depth(new Vector3(0, -5, 0)));
    }

    [Fact]
    public void Tilted_from_below_shows_the_bottom_face_and_not_the_top()
    {
        var list = new DrawList(16);
        var white = Colour.Pack(1, 1, 1, 1);

        CubeGeometry.AddSolidCube(list, Space(), Projection(ViewOrientation.FromDegrees(0, -30)), Vector3.Zero, 1, white);

        var shades = list.Items.ToArray().Select(p => p.Colour).ToArray();
        Assert.Equal(2, shades.Length);
        Assert.Contains(Colour.Shade(white, CubeGeometry.BottomShade), shades);
        Assert.DoesNotContain(Colour.Shade(white, CubeGeometry.TopShade), shades);
    }

    [Fact]
    public void Straight_down_shows_only_the_top_face()
    {
        var list = new DrawList(16);
        var white = Colour.Pack(1, 1, 1, 1);

        CubeGeometry.AddSolidCube(list, Space(), Projection(ViewOrientation.FromDegrees(0, 90)), Vector3.Zero, 1, white);

        Assert.Single(list.Items.ToArray());
        Assert.Equal(Colour.Shade(white, CubeGeometry.TopShade), list.Items[0].Colour);
    }

    [Fact]
    public void Turned_view_shows_three_faces_of_a_cube()
    {
        var list = new DrawList(16);

        CubeGeometry.AddSolidCube(list, Space(), Projection(ViewOrientation.FromDegrees(35, 25)), Vector3.Zero, 1, 0xFFFFFFFF);

        Assert.Equal(3, list.Count);
    }

    [Theory]
    [InlineData(0f, 30f)]
    [InlineData(60f, 75f)]
    [InlineData(-150f, -60f)]
    [InlineData(10f, 90f)]
    public void Fitted_cube_stays_inside_the_region_and_keeps_its_size_at_any_orientation(float yaw, float pitch)
    {
        var regionMin = new Vector2(10, 20);
        var regionSize = new Vector2(300, 200);
        var view = ViewOrientation.FromDegrees(yaw, pitch);
        var projection = ViewProjection.Fit(regionMin, regionSize, 20, view);

        Assert.Equal(ViewProjection.Fit(regionMin, regionSize, 20, ViewOrientation.Default).PixelsPerYalm, projection.PixelsPerYalm);
        for (var i = 0; i < 8; i++)
        {
            var corner = new Vector3((i & 1) != 0 ? 20 : -20, (i & 2) != 0 ? 20 : -20, (i & 4) != 0 ? 20 : -20);
            var screen = projection.ToScreen(corner);
            Assert.InRange(screen.X, regionMin.X, regionMin.X + regionSize.X);
            Assert.InRange(screen.Y, regionMin.Y, regionMin.Y + regionSize.Y);
        }
    }

    [Fact]
    public void Wireframe_changes_when_the_view_is_turned()
    {
        var before = new DrawList(16);
        var after = new DrawList(16);

        CubeGeometry.AddWireCube(before, Space(), Projection(ViewOrientation.Default), 0xFFFFFFFF);
        CubeGeometry.AddWireCube(after, Space(), Projection(ViewOrientation.FromDegrees(40, 50)), 0xFFFFFFFF);

        Assert.NotEqual(before.Items.ToArray(), after.Items.ToArray());
    }

    [Theory]
    [InlineData(0f, 30f)]
    [InlineData(45f, 30f)]
    [InlineData(-120f, 10f)]
    [InlineData(170f, -40f)]
    [InlineData(-179f, 89f)]
    [InlineData(25f, 90f)]
    [InlineData(-60f, -90f)]
    public void Gizmo_view_matrix_round_trips(float yaw, float pitch)
    {
        var view = ViewOrientation.FromDegrees(yaw, pitch);

        var back = ViewOrientation.FromGizmoView(view.ToGizmoView(8f));

        Assert.Equal(view.YawDegrees, back.YawDegrees, 2);
        Assert.Equal(view.PitchDegrees, back.PitchDegrees, 2);
    }

    [Fact]
    public void Gizmo_view_matrix_matches_a_right_handed_look_at_from_above_and_behind()
    {
        // ImGuizmo's LookAt has the same layout as System.Numerics' right-handed CreateLookAt. View coordinates mirror
        // Z, so "behind" (−Z in view coordinates) is +Z in ImGuizmo's world.
        var (sin, cos) = MathF.SinCos(30 * Deg);
        var eye = new Vector3(0, sin, cos) * 8f;
        var expected = Matrix4x4.CreateLookAt(eye, Vector3.Zero, Vector3.UnitY);

        var actual = ViewOrientation.Default.ToGizmoView(8f);

        for (var row = 0; row < 4; row++)
        {
            for (var column = 0; column < 4; column++)
            {
                Assert.Equal(expected[row, column], actual[row, column], 4);
            }
        }
    }

    [Fact]
    public void Gizmo_drag_to_the_side_reads_back_as_a_turned_view()
    {
        // As ImGuizmo would after a drag: the camera moved round to the right of the cube (+X), level.
        var dragged = Matrix4x4.CreateLookAt(new Vector3(8, 0, 0), Vector3.Zero, Vector3.UnitY);

        var view = ViewOrientation.FromGizmoView(dragged);

        Assert.Equal(-90f, view.YawDegrees, 2);
        Assert.Equal(0f, view.PitchDegrees, 2);
    }

    [Fact]
    public void Gizmo_view_from_straight_above_keeps_its_heading()
    {
        // ImGuizmo's "top" face click: looking straight down with the screen's right still along +X.
        var top = Matrix4x4.CreateLookAt(new Vector3(0, 8, 0), Vector3.Zero, -Vector3.UnitZ);

        var view = ViewOrientation.FromGizmoView(top);

        Assert.Equal(0f, view.YawDegrees, 2);
        Assert.Equal(90f, view.PitchDegrees, 2);
    }

    [Fact]
    public void Pitch_is_clamped_and_yaw_wrapped()
    {
        var view = ViewOrientation.FromDegrees(370, 120);

        Assert.Equal(10f, view.YawDegrees, 3);
        Assert.Equal(90f, view.PitchDegrees, 3);
        Assert.Equal(-90f, ViewOrientation.FromDegrees(0, -200).PitchDegrees, 3);
    }

    [Fact]
    public void Projecting_and_converting_do_not_allocate()
    {
        var view = ViewOrientation.FromDegrees(30, 40);
        var projection = ViewProjection.Fit(Vector2.Zero, new Vector2(400, 300), 20, view);
        var point = new Vector3(1, 2, 3);
        var sink = 0f;

        Allocations.AssertNone(() =>
        {
            var matrix = view.ToGizmoView(8f);
            var back = ViewOrientation.FromGizmoView(matrix);
            sink += projection.ToScreen(point).X + projection.Depth(point) + back.Yaw;
        });

        Assert.True(float.IsFinite(sink));
    }
}
