using System.Numerics;
using Cube.Core;

namespace Cube.Tests;

/// <summary>
/// Checks collision stays inside the cube at every cube yaw (#93; PROJECT.md, "Collision (phase 2)": each collider is
/// shown as a simplified box, clipped to the cube; "Cube": in "Rotate contents with camera" the contents turn with
/// the camera). A box cut by a side of a turned cube is kept as the prism the cut leaves, never as a world-aligned
/// box that pokes out of the cube; "Efficiency and memory": clipping and drawing allocate nothing.
/// </summary>
public class ClippedPrismTests
{
    private const float Precision = 1e-3f;

    private static readonly uint Grey = Colour.Pack(0.55f, 0.62f, 0.7f, 0.5f);

    private static readonly CollisionBox HugeFloor = new(new Vector3(-60, -21, -60), new Vector3(60, -2, 60));

    private static readonly CollisionBox[] Scenery =
    [
        HugeFloor,
        new(new Vector3(-3, -40, -60), new Vector3(3, 1, 60)),
        new(new Vector3(10, -5, 10), new Vector3(35, 4, 35)),
        new(new Vector3(-24, 0, -24), new Vector3(24, 1, 24)),
        new(new Vector3(-2, -1, -2), new Vector3(2, 0, 2)),
        new(new Vector3(15, -1, -2), new Vector3(30, 0, 2)),
    ];

    private static float Radians(float degrees) => degrees * MathF.PI / 180f;

    private static bool InsideCube(in CubeSpace cube, Vector3 world)
    {
        var view = cube.ToView(world);
        return MathF.Abs(view.X) <= cube.Half.X + Precision && MathF.Abs(view.Y) <= cube.Half.Y + Precision && MathF.Abs(view.Z) <= cube.Half.Z + Precision;
    }

    private static float Area(in ClippedBox box)
    {
        var sum = 0f;
        for (var i = 0; i < box.CornerCount; i++)
        {
            var p = box.Corner(i);
            var q = box.Corner((i + 1) % box.CornerCount);
            sum += (p.X * q.Y) - (q.X * p.Y);
        }

        return sum / 2f;
    }

    private static DrawList Draw(in CubeSpace space, in ViewProjection projection, ReadOnlySpan<CollisionBox> boxes)
    {
        var clipped = new ClippedBox[boxes.Length];
        var count = CollisionClipper.Clip(space, boxes, clipped);
        var list = new DrawList(CollisionCubes.MaxFacesPerBox * boxes.Length, ClippedBox.MaxCorners * boxes.Length);
        CollisionCubes.Add(list, new BoxProjector(space, projection), clipped.AsSpan(0, count), Grey);
        list.SortBackToFront();
        return list;
    }

    private static IEnumerable<Vector2> Vertices(DrawList list, DrawPrimitive p) =>
        p.Kind == PrimitiveKind.Polygon ? list.Points(p).ToArray() : [p.A, p.B, p.C, p.D];

    public static TheoryData<float, float, float> YawsAndViews()
    {
        var data = new TheoryData<float, float, float>();
        foreach (var yaw in new[] { 0f, 30f, 45f, 90f, 137f })
        {
            // The default view, and a view turned with the view control (North up mode keeps yaw 0 but turns the view).
            data.Add(yaw, 0f, 35f);
            data.Add(yaw, 70f, 20f);
        }

        return data;
    }

    [Theory]
    [MemberData(nameof(YawsAndViews))]
    public void Every_drawn_collision_corner_is_inside_the_cube_outline(float yawDegrees, float viewYaw, float viewPitch)
    {
        var space = new CubeSpace(new Vector3(100, 5, -30), 40, Radians(yawDegrees));
        var view = viewYaw == 0 ? ViewOrientation.Default : ViewOrientation.FromDegrees(viewYaw, viewPitch);
        var projection = ViewProjection.Fit(Vector2.Zero, new Vector2(400, 400), space.Half, view);
        var outline = new CubeOutline(space, projection);
        var boxes = Scenery.Select(b => new CollisionBox(b.Min + space.Centre, b.Max + space.Centre)).ToArray();

        var list = Draw(space, projection, boxes);

        Assert.NotEqual(0, list.Count);
        foreach (var p in list.Items.ToArray())
        {
            foreach (var vertex in Vertices(list, p))
            {
                Assert.True(outline.Contains(vertex, 0.01f), $"{p.Kind} of box {CollisionCubes.IndexOf(p.Tag)} has a corner {vertex} outside the cube at yaw {yawDegrees}°.");
            }
        }
    }

    [Fact]
    public void Floor_bigger_than_the_cube_turned_45_degrees_is_cut_to_the_turned_square()
    {
        var cube = new CubeSpace(Vector3.Zero, 40, MathF.PI / 4);

        Assert.True(CollisionClipper.TryClip(cube, HugeFloor, out var clipped));

        Assert.False(clipped.IsRectangle);
        Assert.Equal(4, clipped.CornerCount);
        Assert.Equal(40f * 40f, Area(clipped), 0.1f);
        for (var i = 0; i < clipped.CornerCount; i++)
        {
            Assert.True(InsideCube(cube, clipped.CornerAt(i, clipped.Bounds.Max.Y)), $"corner {clipped.Corner(i)} is outside the turned cube");
        }
    }

    [Theory]
    [InlineData(0f)]
    [InlineData(17f)]
    [InlineData(30f)]
    [InlineData(45f)]
    [InlineData(90f)]
    [InlineData(200f)]
    [InlineData(-61f)]
    public void Clipped_footprint_never_leaves_the_turned_cube(float yawDegrees)
    {
        var cube = new CubeSpace(Vector3.Zero, new Vector3(40, 30, 24), Radians(yawDegrees));
        foreach (var box in Scenery)
        {
            if (!CollisionClipper.TryClip(cube, box, out var clipped))
            {
                continue;
            }

            Assert.True(box.Contains(clipped.Bounds));
            for (var i = 0; i < clipped.CornerCount; i++)
            {
                Assert.True(InsideCube(cube, clipped.CornerAt(i, clipped.Bounds.Min.Y)), $"corner {clipped.Corner(i)} of {box.Min}-{box.Max} outside at {yawDegrees}°");
                Assert.True(InsideCube(cube, clipped.CornerAt(i, clipped.Bounds.Max.Y)));
            }
        }
    }

    [Theory]
    [InlineData(0f)]
    [InlineData(90f)]
    [InlineData(180f)]
    [InlineData(-90f)]
    public void At_multiples_of_90_degrees_every_clipped_box_is_a_plain_box_cut_at_the_cube_faces(float yawDegrees)
    {
        var cube = new CubeSpace(Vector3.Zero, 40, Radians(yawDegrees));
        foreach (var box in Scenery)
        {
            Assert.True(CollisionClipper.TryClip(cube, box, out var clipped));

            Assert.True(clipped.IsRectangle, $"{box.Min}-{box.Max} at {yawDegrees}°");
            var expectedMin = Vector3.Max(box.Min, new Vector3(-20));
            var expectedMax = Vector3.Min(box.Max, new Vector3(20));
            Assert.Equal(expectedMin.X, clipped.Bounds.Min.X, Precision);
            Assert.Equal(expectedMin.Y, clipped.Bounds.Min.Y, Precision);
            Assert.Equal(expectedMin.Z, clipped.Bounds.Min.Z, Precision);
            Assert.Equal(expectedMax.X, clipped.Bounds.Max.X, Precision);
            Assert.Equal(expectedMax.Y, clipped.Bounds.Max.Y, Precision);
            Assert.Equal(expectedMax.Z, clipped.Bounds.Max.Z, Precision);
        }
    }

    [Fact]
    public void Box_inside_a_turned_cube_stays_a_plain_box()
    {
        var cube = new CubeSpace(Vector3.Zero, 40, Radians(45));
        var box = new CollisionBox(new Vector3(-2, -1, -2), new Vector3(2, 0, 2));

        Assert.True(CollisionClipper.TryClip(cube, box, out var clipped));

        Assert.True(clipped.IsRectangle);
        Assert.Equal(box.Min, clipped.Bounds.Min);
        Assert.Equal(box.Max, clipped.Bounds.Max);
    }

    [Fact]
    public void Square_cut_at_all_four_corners_by_a_turned_cube_has_eight_corners_counter_clockwise()
    {
        // The 45° cube's footprint is the diamond |x| + |z| ≤ 20·√2 ≈ 28.3; a ±24 square pokes past it at all four corners.
        var cube = new CubeSpace(Vector3.Zero, 40, Radians(45));

        Assert.True(CollisionClipper.TryClip(cube, new CollisionBox(new Vector3(-24, 0, -24), new Vector3(24, 1, 24)), out var clipped));

        Assert.False(clipped.IsRectangle);
        Assert.Equal(ClippedBox.MaxCorners, clipped.CornerCount);
        Assert.True(Area(clipped) > 0, "corners go counter-clockwise");
    }

    [Fact]
    public void Turned_floor_seen_from_above_draws_one_top_polygon_and_no_sides()
    {
        var space = new CubeSpace(Vector3.Zero, 40, Radians(45));
        var projection = ViewProjection.Fit(Vector2.Zero, new Vector2(400, 400), space.Half, ViewOrientation.Default);
        var floor = new CollisionBox(new Vector3(-24, -1, -24), new Vector3(24, -1, 24));

        var list = Draw(space, projection, [floor]);

        var top = Assert.Single(list.Items.ToArray());
        Assert.Equal(PrimitiveKind.Polygon, top.Kind);
        Assert.Equal(ClippedBox.MaxCorners, list.Points(top).Length);
        Assert.Equal(Colour.Shade(Grey, CubeGeometry.TopShade), top.Colour);
    }

    [Fact]
    public void Prism_shows_its_top_and_the_sides_facing_the_viewer_only()
    {
        var space = new CubeSpace(Vector3.Zero, 40, Radians(30));
        var projection = ViewProjection.Fit(Vector2.Zero, new Vector2(400, 400), space.Half, ViewOrientation.Default);
        var block = new CollisionBox(new Vector3(-24, -5, -24), new Vector3(24, 5, 24));

        var list = Draw(space, projection, [block]);

        var items = list.Items.ToArray();
        Assert.InRange(items.Length, 2, CollisionCubes.MaxFacesPerBox);
        Assert.Single(items, p => p.Colour == Colour.Shade(Grey, CubeGeometry.TopShade));
        Assert.All(items, p => Assert.Equal(CollisionCubes.TagFor(0), p.Tag));
    }

    [Fact]
    public void Clipping_and_drawing_prisms_for_a_full_set_does_not_allocate()
    {
        var space = new CubeSpace(Vector3.Zero, 40, Radians(45));
        var projection = ViewProjection.Fit(Vector2.Zero, new Vector2(400, 400), space.Half, ViewOrientation.Default);
        var boxes = new CollisionBox[CollisionBoxSet.DefaultCapacity];
        for (var i = 0; i < boxes.Length; i++)
        {
            // Long boards across the whole cube, so every one is cut by the turned sides.
            var y = (i % 40) - 20f;
            boxes[i] = i % 2 == 0
                ? new CollisionBox(new Vector3(-50, y, (i % 37) - 18), new Vector3(50, y + 0.5f, (i % 37) - 17))
                : new CollisionBox(new Vector3((i % 37) - 18, y, -50), new Vector3((i % 37) - 17, y + 0.5f, 50));
        }

        var clipped = new ClippedBox[boxes.Length];
        var list = new DrawList(CollisionCubes.MaxFacesPerBox * boxes.Length, ClippedBox.MaxCorners * boxes.Length);

        Allocations.AssertNone(() =>
        {
            var count = CollisionClipper.Clip(space, boxes, clipped);
            list.Clear();
            CollisionCubes.Add(list, new BoxProjector(space, projection), clipped.AsSpan(0, count), Grey);
            list.SortBackToFront();
        });
        Assert.Equal(0, list.Dropped);
        Assert.Contains(clipped, b => !b.IsRectangle);
    }
}
