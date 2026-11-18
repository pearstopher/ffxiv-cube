using System.Numerics;
using Cube.Core;

namespace Cube.Tests;

/// <summary>
/// Checks the collision box model (PROJECT.md, "Collision (phase 2)": colliders inside or crossing the cube are shown
/// as simplified boxes clipped to the cube; stale data is refreshed when the player moves; "Efficiency and memory":
/// bounded cache with a hard cap, collision data dropped on a zone change, no per-frame allocations). The box source
/// (one box per mesh node or primitive collider through its world matrix) is the #28 recommendation in
/// <c>docs/research-notes.md</c>.
/// </summary>
public class CollisionBoxTests
{
    private const float Precision = 1e-4f;
    private static readonly Vector3 Centre = new(100, 10, -50);

    private static CollisionBox Box(float x0, float y0, float z0, float x1, float y1, float z1) =>
        new(new Vector3(x0, y0, z0), new Vector3(x1, y1, z1));

    private static CollisionBox Around(Vector3 centre, Vector3 half) => new(centre - half, centre + half);

    private static void AssertBox(CollisionBox expected, CollisionBox actual)
    {
        Assert.True(Vector3.Distance(expected.Min, actual.Min) < Precision, $"Min {actual.Min} != {expected.Min}");
        Assert.True(Vector3.Distance(expected.Max, actual.Max) < Precision, $"Max {actual.Max} != {expected.Max}");
    }

    [Fact]
    public void Node_bounds_through_a_translation_move_with_it()
    {
        var world = Matrix4x4.CreateTranslation(10, 20, 30);

        var box = CollisionBox.FromTransformed(new Vector3(-1, 0, -2), new Vector3(1, 3, 2), world);

        AssertBox(Box(9, 20, 28, 11, 23, 32), box);
    }

    [Fact]
    public void Unit_primitive_through_a_scale_becomes_a_box_of_that_size()
    {
        // Primitive colliders are unit shapes (-1..1) placed by their World matrix (#28).
        var world = Matrix4x4.CreateScale(2, 0.5f, 4) * Matrix4x4.CreateTranslation(5, 0, 5);

        var box = CollisionBox.FromTransformed(-Vector3.One, Vector3.One, world);

        AssertBox(Box(3, -0.5f, 1, 7, 0.5f, 9), box);
    }

    [Fact]
    public void Rotated_node_gets_the_world_box_around_its_turned_corners()
    {
        // A 2×2×2 box turned 45° about Y reaches √2 sideways on X and Z.
        var world = Matrix4x4.CreateRotationY(MathF.PI / 4);

        var box = CollisionBox.FromTransformed(-Vector3.One, Vector3.One, world);

        var r = MathF.Sqrt(2);
        AssertBox(Box(-r, -1, -r, r, 1, r), box);
    }

    [Fact]
    public void Flat_floor_box_is_valid_but_inside_out_or_nan_boxes_are_not()
    {
        Assert.True(Box(0, 5, 0, 10, 5, 10).IsValid);
        Assert.False(Box(1, 0, 0, 0, 1, 1).IsValid);
        Assert.False(Box(float.NaN, 0, 0, 1, 1, 1).IsValid);
        Assert.False(Box(0, 0, 0, float.PositiveInfinity, 1, 1).IsValid);
    }

    [Fact]
    public void Box_inside_the_region_is_kept()
    {
        var set = new CollisionBoxSet();
        set.Begin(Centre, new Vector3(40), 1);

        Assert.True(set.Offer(Around(Centre, new Vector3(2))));

        Assert.Equal(1, set.Count);
        Assert.Equal(1, set.Offered);
    }

    [Fact]
    public void Box_beyond_the_region_is_not_kept()
    {
        var set = new CollisionBoxSet();
        set.Begin(Centre, new Vector3(40), 1);

        // Region reaches 20·√2 + 10 ≈ 38.3 sideways and 30 vertically.
        Assert.False(set.Offer(Around(Centre + new Vector3(45, 0, 0), new Vector3(1))));
        Assert.False(set.Offer(Around(Centre + new Vector3(0, 35, 0), new Vector3(1))));

        Assert.Equal(0, set.Count);
        Assert.Equal(2, set.Offered);
    }

    [Fact]
    public void Region_covers_the_cube_at_every_yaw_with_a_margin()
    {
        var region = CollisionBoxSet.RegionFor(Centre, new Vector3(40));

        for (var yaw = 0f; yaw < MathF.Tau; yaw += 0.1f)
        {
            var cube = new CubeSpace(Centre, 40, yaw);
            for (var corner = 0; corner < 8; corner++)
            {
                var view = new Vector3((corner & 1) != 0 ? 20 : -20, (corner & 2) != 0 ? 20 : -20, (corner & 4) != 0 ? 20 : -20);
                var world = cube.FromView(view);
                Assert.True(region.Contains(new CollisionBox(world, world)), $"corner {corner} at yaw {yaw} outside region");
            }
        }

        Assert.Equal(Centre.Y + 20 + (40 * CollisionBoxSet.RefreshFraction), region.Max.Y, Precision);
    }

    [Fact]
    public void Region_covers_a_non_cube_box_at_every_yaw()
    {
        var size = new Vector3(30, 20, 80);
        var region = CollisionBoxSet.RegionFor(Centre, size);

        for (var yaw = 0f; yaw < MathF.Tau; yaw += 0.1f)
        {
            var box = new CubeSpace(Centre, size, yaw);
            for (var corner = 0; corner < 8; corner++)
            {
                var view = new Vector3((corner & 1) != 0 ? 15 : -15, (corner & 2) != 0 ? 10 : -10, (corner & 4) != 0 ? 40 : -40);
                var world = box.FromView(view);
                Assert.True(region.Contains(new CollisionBox(world, world)), $"corner {corner} at yaw {yaw} outside region");
            }
        }

        // Vertically only the height and its own margin: a flat box doesn't gather far above and below.
        Assert.Equal(Centre.Y + 10 + (20 * CollisionBoxSet.RefreshFraction), region.Max.Y, Precision);
    }

    [Fact]
    public void Changing_one_extent_asks_for_a_new_gather()
    {
        var set = new CollisionBoxSet();
        set.Begin(Centre, new Vector3(40), 1);

        Assert.True(set.NeedsRefresh(Centre, new Vector3(40, 60, 40), 1));
    }

    [Fact]
    public void Invalid_box_is_rejected_and_counted()
    {
        var set = new CollisionBoxSet();
        set.Begin(Centre, new Vector3(40), 1);

        Assert.False(set.Offer(new CollisionBox(new Vector3(float.NaN), Centre)));

        Assert.Equal(0, set.Count);
        Assert.Equal(1, set.Invalid);
    }

    [Fact]
    public void Full_set_drops_extra_boxes_instead_of_growing()
    {
        var set = new CollisionBoxSet(capacity: 3);
        set.Begin(Centre, new Vector3(40), 1);

        // Spaced apart so none of them merge.
        for (var i = 0; i < 5; i++)
        {
            set.Offer(Around(Centre + new Vector3(i * 4, 0, 0), new Vector3(1)));
        }

        Assert.Equal(3, set.Count);
        Assert.Equal(2, set.Dropped);
    }

    [Fact]
    public void Box_inside_the_last_box_is_merged_into_it()
    {
        var set = new CollisionBoxSet();
        set.Begin(Centre, new Vector3(40), 1);

        set.Offer(Around(Centre, new Vector3(5)));
        set.Offer(Around(Centre, new Vector3(1)));

        Assert.Equal(1, set.Count);
        Assert.Equal(1, set.Merged);
        AssertBox(Around(Centre, new Vector3(5)), set.Items[0]);
    }

    [Fact]
    public void Box_holding_the_last_box_replaces_it()
    {
        var set = new CollisionBoxSet();
        set.Begin(Centre, new Vector3(40), 1);

        set.Offer(Around(Centre, new Vector3(1)));
        set.Offer(Around(Centre, new Vector3(5)));

        Assert.Equal(1, set.Count);
        AssertBox(Around(Centre, new Vector3(5)), set.Items[0]);
    }

    [Fact]
    public void Wall_segments_that_line_up_end_to_end_merge_into_one_box()
    {
        Assert.True(CollisionBoxSet.TryMerge(Box(0, 0, 0, 4, 3, 0.2f), Box(4, 0, 0, 9, 3, 0.2f), out var merged));

        AssertBox(Box(0, 0, 0, 9, 3, 0.2f), merged);
    }

    [Fact]
    public void Boxes_that_differ_on_two_axes_are_not_merged()
    {
        // Merging these would fill the empty corner between them: a box where there's nothing to bump into.
        Assert.False(CollisionBoxSet.TryMerge(Box(0, 0, 0, 4, 3, 1), Box(4, 0, 1, 8, 3, 3), out _));
    }

    [Fact]
    public void Lined_up_boxes_with_a_gap_between_them_are_not_merged()
    {
        Assert.False(CollisionBoxSet.TryMerge(Box(0, 0, 0, 4, 3, 1), Box(5, 0, 0, 9, 3, 1), out _));
    }

    [Fact]
    public void Fresh_set_needs_a_gather()
    {
        Assert.True(new CollisionBoxSet().NeedsRefresh(Centre, new Vector3(40), 1));
    }

    [Fact]
    public void Small_moves_and_turning_the_camera_do_not_need_a_new_gather()
    {
        var set = new CollisionBoxSet();
        set.Begin(Centre, new Vector3(40), 1);

        Assert.False(set.NeedsRefresh(Centre + new Vector3(9, -9, 9), new Vector3(40), 1));
    }

    [Fact]
    public void Moving_a_quarter_of_the_cube_needs_a_new_gather()
    {
        var set = new CollisionBoxSet();
        set.Begin(Centre, new Vector3(40), 1);

        Assert.True(set.NeedsRefresh(Centre + new Vector3(0, 0, 10.5f), new Vector3(40), 1));
        Assert.True(set.NeedsRefresh(Centre + new Vector3(0, -10.5f, 0), new Vector3(40), 1));
    }

    [Fact]
    public void Scene_change_or_new_cube_size_needs_a_new_gather()
    {
        var set = new CollisionBoxSet();
        set.Begin(Centre, new Vector3(40), 1);

        Assert.True(set.NeedsRefresh(Centre, new Vector3(40), 2));
        Assert.True(set.NeedsRefresh(Centre, new Vector3(60), 1));
    }

    [Fact]
    public void Zone_change_drops_every_box()
    {
        var set = new CollisionBoxSet();
        set.Begin(Centre, new Vector3(40), 1);
        set.Offer(Around(Centre, new Vector3(1)));

        set.Clear();

        Assert.Equal(0, set.Count);
        Assert.False(set.HasData);
        Assert.True(set.NeedsRefresh(Centre, new Vector3(40), 1));
    }

    [Fact]
    public void Box_inside_the_cube_is_unchanged_by_clipping()
    {
        var cube = new CubeSpace(Centre, 40, 0.7f);
        var box = Around(Centre, new Vector3(3, 1, 2));

        Assert.True(CollisionClipper.TryClip(cube, box, out var clipped));

        AssertBox(box, clipped);
    }

    [Fact]
    public void Box_crossing_the_cube_north_up_is_cut_exactly_at_its_faces()
    {
        var cube = new CubeSpace(Centre, 40, 0);
        var box = Box(Centre.X - 5, Centre.Y - 50, Centre.Z + 10, Centre.X + 50, Centre.Y + 5, Centre.Z + 30);

        Assert.True(CollisionClipper.TryClip(cube, box, out var clipped));

        AssertBox(Box(Centre.X - 5, Centre.Y - 20, Centre.Z + 10, Centre.X + 20, Centre.Y + 5, Centre.Z + 20), clipped);
    }

    [Fact]
    public void Box_beside_the_cube_is_left_out()
    {
        var cube = new CubeSpace(Centre, 40, 0);

        Assert.False(CollisionClipper.TryClip(cube, Around(Centre + new Vector3(25, 0, 0), new Vector3(2)), out _));
        Assert.False(CollisionClipper.TryClip(cube, Around(Centre + new Vector3(0, -25, 0), new Vector3(2)), out _));
    }

    [Fact]
    public void Box_in_the_corner_gap_of_a_turned_cube_is_left_out()
    {
        // Turned 45°, the cube's footprint is a diamond; a box near the old corner (inside the north-up square but
        // outside the diamond) is not in the cube.
        var cube = new CubeSpace(Centre, 40, MathF.PI / 4);

        Assert.False(CollisionClipper.TryClip(cube, Around(Centre + new Vector3(18, 0, 18), new Vector3(1, 1, 1)), out _));
    }

    [Fact]
    public void Clipped_box_never_reaches_beyond_the_box_or_the_cube_height()
    {
        var box = Box(Centre.X - 3, Centre.Y - 40, Centre.Z - 60, Centre.X + 3, Centre.Y + 1, Centre.Z + 60);
        for (var yaw = 0f; yaw < MathF.Tau; yaw += 0.13f)
        {
            var cube = new CubeSpace(Centre, 40, yaw);

            Assert.True(CollisionClipper.TryClip(cube, box, out var clipped));

            Assert.True(box.Contains(clipped));
            Assert.Equal(Centre.Y - 20, clipped.Min.Y, Precision);

            // A long thin wall through the centre: what's left spans no more than the cube's diagonal.
            Assert.True(clipped.Max.Z - clipped.Min.Z <= (40 * MathF.Sqrt(2)) + Precision);
        }
    }

    [Fact]
    public void Clipped_box_follows_the_turned_cube_closely()
    {
        // A long wall along Z, off to one side of a cube turned 45°: the diamond's footprint is |x| + |z| ≤ 20·√2.
        // The wall spans x = 5..8, so the part inside reaches z = ±(20·√2 − 5), not the whole diamond's ±28.3
        // that a cut to the cube's world-aligned bounds would give.
        var cube = new CubeSpace(Centre, 40, MathF.PI / 4);
        var box = Box(Centre.X + 5, Centre.Y, Centre.Z - 60, Centre.X + 8, Centre.Y + 1, Centre.Z + 60);

        Assert.True(CollisionClipper.TryClip(cube, box, out var clipped));

        var reach = (20 * MathF.Sqrt(2)) - 5;
        Assert.Equal(Centre.Z + reach, clipped.Max.Z, 1e-3f);
        Assert.Equal(Centre.Z - reach, clipped.Min.Z, 1e-3f);
    }

    [Fact]
    public void Zero_thickness_wall_keeps_its_exact_face_when_clipped()
    {
        var cube = new CubeSpace(Centre, 40, 0.4f);
        var wall = Box(Centre.X + 2, Centre.Y - 1, Centre.Z - 60, Centre.X + 2, Centre.Y + 3, Centre.Z + 60);

        Assert.True(CollisionClipper.TryClip(cube, wall, out var clipped));

        Assert.Equal(Centre.X + 2, clipped.Min.X);
        Assert.Equal(Centre.X + 2, clipped.Max.X);
    }

    [Fact]
    public void Clipping_a_list_keeps_only_boxes_in_the_cube_in_order()
    {
        var cube = new CubeSpace(Centre, 40, 0);
        var source = new[]
        {
            Around(Centre + new Vector3(5, 0, 0), new Vector3(1)),
            Around(Centre + new Vector3(30, 0, 0), new Vector3(1)),
            Around(Centre + new Vector3(-5, 0, 0), new Vector3(1)),
        };
        var destination = new CollisionBox[3];

        var count = CollisionClipper.Clip(cube, source, destination);

        Assert.Equal(2, count);
        AssertBox(source[0], destination[0]);
        AssertBox(source[2], destination[1]);
    }

    [Fact]
    public void Clipping_stops_when_the_destination_is_full()
    {
        var cube = new CubeSpace(Centre, 40, 0);
        var source = new[] { Around(Centre, new Vector3(1)), Around(Centre, new Vector3(2)) };

        Assert.Equal(1, CollisionClipper.Clip(cube, source, new CollisionBox[1]));
    }

    [Fact]
    public void Gathering_and_clipping_every_frame_does_not_allocate()
    {
        var set = new CollisionBoxSet();
        var clipped = new CollisionBox[set.Capacity];
        var cube = new CubeSpace(Centre, 40, 0.3f);
        var world = Matrix4x4.CreateRotationY(0.5f) * Matrix4x4.CreateTranslation(Centre);

        Allocations.AssertNone(() =>
        {
            set.Begin(Centre, new Vector3(40), 7);
            for (var i = 0; i < 500; i++)
            {
                var offset = new Vector3((i % 25) - 12, (i % 7) - 3, (i / 25) - 10) * 2;
                set.Offer(CollisionBox.FromTransformed(offset, offset + new Vector3(1, 0.5f, 1.5f), world));
            }

            CollisionClipper.Clip(cube, set.Items, clipped);
            set.NeedsRefresh(Centre, new Vector3(40), 7);
        });
    }
}
