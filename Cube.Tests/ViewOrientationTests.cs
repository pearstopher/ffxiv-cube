using System.Numerics;
using Cube.Core;

namespace Cube.Tests;

/// <summary>
/// Checks the user's view orientation and the projection it drives (PROJECT.md, "View orientation control": the view
/// control is a rotate gizmo that turns how the cube is shown, any orientation including roll; default from above and
/// behind at 30°, front face away; orientations saved by the old yaw-and-tilt control carry over; "Efficiency and
/// memory": no per-frame allocations).
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

        Assert.Equal(new Vector3(0, sin, -cos), projection.TowardsViewer, new VectorComparer(5));
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
        Assert.Equal(0f, view.Right.Y, 5); // a heading and tilt alone never roll the view
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
        Assert.Equal(Origin, projection.ToScreen(new Vector3(0, 8, 0)), new VectorComparer(3));
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
    public void Old_yaw_and_tilt_give_the_same_projection_as_the_old_control(float yaw, float pitch)
    {
        // The old control's axes (#63), written out, so saved yaw/tilt values carry over exactly (PROJECT.md).
        var (sinYaw, cosYaw) = MathF.SinCos(yaw * Deg);
        var (sinPitch, cosPitch) = MathF.SinCos(pitch * Deg);
        var right = new Vector3(cosYaw, 0, -sinYaw);
        var up = new Vector3(sinYaw * sinPitch, cosPitch, cosYaw * sinPitch);
        var projection = Projection(ViewOrientation.FromDegrees(yaw, pitch));

        foreach (var p in SamplePoints())
        {
            var screen = projection.ToScreen(p);
            Assert.Equal(Origin.X + (Vector3.Dot(p, right) * 10), screen.X, 2);
            Assert.Equal(Origin.Y - (Vector3.Dot(p, up) * 10), screen.Y, 2);
        }
    }

    [Theory]
    [InlineData(0f, 30f, 0f)]
    [InlineData(45f, 30f, 0f)]
    [InlineData(-120f, 10f, 40f)]
    [InlineData(170f, -40f, -75f)]
    [InlineData(25f, 90f, 120f)]
    [InlineData(-60f, -90f, 180f)]
    public void Rotation_round_trips_through_the_gizmo_model_matrix(float yaw, float pitch, float roll)
    {
        var view = Rolled(ViewOrientation.FromDegrees(yaw, pitch), roll);

        Assert.True(ViewOrientation.TryFromGizmoModel(view.ToGizmoModel(), out var back));

        AssertSameAxes(view, back);
    }

    [Fact]
    public void Gizmo_model_of_the_default_view_is_seen_from_above_and_behind()
    {
        // The gizmo's camera sits on ImGuizmo's +Z axis. View coordinates mirror Z, so the cube's front (+Z) must point
        // away from that camera and down the screen's depth, and its up (+Y) tilts towards the camera.
        var model = ViewOrientation.Default.ToGizmoModel();
        var (sin, cos) = MathF.SinCos(30 * Deg);

        var front = Vector3.TransformNormal(-Vector3.UnitZ, model); // the cube's +Z, mirrored into ImGuizmo's world
        var up = Vector3.TransformNormal(Vector3.UnitY, model);

        Assert.Equal(new Vector3(0, sin, -cos), front, new VectorComparer(4));
        Assert.Equal(new Vector3(0, cos, sin), up, new VectorComparer(4));
    }

    [Fact]
    public void Dragging_the_x_ring_turns_the_cube_around_its_own_x_axis()
    {
        // As ImGuizmo's rotate gizmo in local mode does: a turn around the model's own x axis, applied first.
        var view = ViewOrientation.FromDegrees(35, 20);
        var dragged = Matrix4x4.CreateRotationX(25 * Deg) * view.ToGizmoModel();

        Assert.True(ViewOrientation.TryFromGizmoModel(dragged, out var turned));

        var before = Projection(view);
        var after = Projection(turned);
        Assert.Equal(before.ToScreen(Vector3.UnitX * 10), after.ToScreen(Vector3.UnitX * 10), new VectorComparer(3));
        Assert.NotEqual(before.ToScreen(Vector3.UnitY * 10), after.ToScreen(Vector3.UnitY * 10));
    }

    [Fact]
    public void Gizmo_drift_and_scale_are_removed()
    {
        var view = ViewOrientation.FromDegrees(-50, 40);
        var scaled = Matrix4x4.CreateScale(1.03f) * view.ToGizmoModel();

        Assert.True(ViewOrientation.TryFromGizmoModel(scaled, out var back));

        AssertSameAxes(view, back);
    }

    [Fact]
    public void Unusable_gizmo_matrix_is_rejected()
    {
        Assert.False(ViewOrientation.TryFromGizmoModel(default, out _));
    }

    [Fact]
    public void Rolling_the_view_changes_the_projection()
    {
        var view = ViewOrientation.FromDegrees(20, 30);
        var rolled = Rolled(view, 30);

        Assert.NotEqual(0f, rolled.Right.Y, 3); // the screen's right is no longer level
        Assert.NotEqual(Projection(view).ToScreen(new Vector3(5, 0, 0)), Projection(rolled).ToScreen(new Vector3(5, 0, 0)));
    }

    [Fact]
    public void Default_view_is_unchanged_from_the_old_control_default()
    {
        AssertSameAxes(ViewOrientation.FromDegrees(0, ViewOrientation.DefaultPitchDegrees), ViewOrientation.Default);
    }

    [Fact]
    public void Unusable_rotation_falls_back_to_the_default()
    {
        Assert.Equal(ViewOrientation.Default, new ViewOrientation(default));
        Assert.Equal(ViewOrientation.Default, new ViewOrientation(new Quaternion(float.NaN, 0, 0, 1)));
    }

    [Fact]
    public void Old_tilt_is_clamped_and_heading_wrapped()
    {
        AssertSameAxes(ViewOrientation.FromDegrees(10, 90), ViewOrientation.FromDegrees(370, 120));
        AssertSameAxes(ViewOrientation.FromDegrees(0, -90), ViewOrientation.FromDegrees(0, -200));
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
            var matrix = view.ToGizmoModel();
            ViewOrientation.TryFromGizmoModel(matrix, out var back);
            sink += projection.ToScreen(point).X + projection.Depth(point) + back.Rotation.W;
        });

        Assert.True(float.IsFinite(sink));
    }

    private static ViewOrientation Rolled(ViewOrientation view, float degrees) =>
        new(Quaternion.Concatenate(view.Rotation, Quaternion.CreateFromAxisAngle(Vector3.UnitZ, degrees * Deg)));

    private static void AssertSameAxes(ViewOrientation expected, ViewOrientation actual)
    {
        var comparer = new VectorComparer(4);
        Assert.Equal(expected.Right, actual.Right, comparer);
        Assert.Equal(expected.Up, actual.Up, comparer);
        Assert.Equal(expected.Forward, actual.Forward, comparer);
    }

    private sealed class VectorComparer(int digits) : IEqualityComparer<Vector3>, IEqualityComparer<Vector2>
    {
        private readonly float tolerance = MathF.Pow(10, -digits);

        public bool Equals(Vector3 x, Vector3 y) => Vector3.Distance(x, y) <= this.tolerance;

        public bool Equals(Vector2 x, Vector2 y) => Vector2.Distance(x, y) <= this.tolerance;

        public int GetHashCode(Vector3 obj) => 0;

        public int GetHashCode(Vector2 obj) => 0;
    }
}
