using System.Numerics;
using Cube.Core;

namespace Cube.Tests;

/// <summary>
/// Checks collision boxes are drawn as solid boxes sorted with the players (PROJECT.md, "Collision (phase 2)": each
/// collider inside or crossing the cube is shown as a simplified box; "Draw order": far things are drawn first, so
/// near ones cover far ones; "Cheap drawing": only the visible faces; "Efficiency and memory": no per-frame
/// allocations).
/// </summary>
public class CollisionCubesTests
{
    private static readonly uint Grey = Colour.Pack(0.55f, 0.62f, 0.7f, 0.5f);

    private static CubeSpace Space(float yaw = 0f) => new(Vector3.Zero, 40, yaw);

    private static ViewProjection Projection(in CubeSpace space) =>
        ViewProjection.Fit(Vector2.Zero, new Vector2(400, 400), space.Half, ViewOrientation.Default);

    private static NearbyPlayer Self() => new(0, 1, Vector3.Zero, PlayerRelation.Self, "Me");

    private static ClippedBox[] Plain(CollisionBox[] boxes) => boxes.Select(b => new ClippedBox(b)).ToArray();

    private static DrawPrimitive[] Frame(in CubeSpace space, CollisionBox[] boxes, NearbyPlayer[] players)
    {
        var projection = Projection(space);
        var list = new DrawList(256);
        CollisionCubes.Add(list, new BoxProjector(space, projection), Plain(boxes), Grey);
        PlayerCubes.Add(list, space, projection, players, PlayerPalette.Default, labels: false);
        list.SortBackToFront();
        return list.Items.ToArray();
    }

    [Fact]
    public void Box_in_the_cube_gets_its_visible_faces_in_the_collision_colour_with_its_own_tag()
    {
        var boxes = new[] { new CollisionBox(new Vector3(-8, -2, 3), new Vector3(-5, 4, 6)), new CollisionBox(new Vector3(5, -3, -4), new Vector3(9, -1, 0)) };

        var items = Frame(Space(0.4f), boxes, []);

        Assert.All(items, p => Assert.Equal(PrimitiveKind.Quad, p.Kind));
        Assert.InRange(items.Count(p => p.Tag == CollisionCubes.TagFor(0)), 1, 3);
        Assert.InRange(items.Count(p => p.Tag == CollisionCubes.TagFor(1)), 1, 3);
        Assert.Contains(items, p => p.Colour == Grey);
        Assert.All(items, p => Assert.Equal(Grey >> 24, p.Colour >> 24));
    }

    [Theory]
    [InlineData(0f)]
    [InlineData(0.4f)]
    [InlineData(2.5f)]
    [InlineData(-1.9f)]
    public void Cube_shaped_box_is_drawn_exactly_like_a_player_cube(float yaw)
    {
        var space = new CubeSpace(new Vector3(100, 5, -30), 40, yaw);
        var projection = Projection(space);
        var box = new CollisionBox(new Vector3(103, 3, -36), new Vector3(107, 7, -32));
        var collision = new DrawList(8);
        var player = new DrawList(8);

        CollisionCubes.Add(collision, new BoxProjector(space, projection), [new ClippedBox(box)], Grey);
        CubeGeometry.AddSolidCube(player, space, projection, space.ToView(box.Centre), 2f, Grey);

        static string Key(DrawPrimitive p) =>
            string.Join(";", new[] { p.A, p.B, p.C, p.D }.Select(v => $"{MathF.Round(v.X, 2)},{MathF.Round(v.Y, 2)}").Order()) + $"#{p.Colour:X8}";
        Assert.Equal(player.Items.ToArray().Select(Key).Order(), collision.Items.ToArray().Select(Key).Order());
    }

    [Fact]
    public void Collision_tags_are_negative_and_lead_back_to_their_box()
    {
        Assert.Equal(-1, CollisionCubes.TagFor(0));
        Assert.True(CollisionCubes.IsCollisionTag(CollisionCubes.TagFor(1999)));
        Assert.False(CollisionCubes.IsCollisionTag(0));
        Assert.False(CollisionCubes.IsCollisionTag(1));
        Assert.Equal(1999, CollisionCubes.IndexOf(CollisionCubes.TagFor(1999)));
    }

    [Fact]
    public void Flat_floor_seen_from_above_shows_its_top_face_only()
    {
        // A floor of zero thickness: its sides would be lines, its top is what you walk on.
        var floor = new CollisionBox(new Vector3(-10, -1, -10), new Vector3(10, -1, 10));

        var items = Frame(Space(), [floor], []);

        var face = Assert.Single(items);
        Assert.Equal(Colour.Shade(Grey, CubeGeometry.TopShade), face.Colour);
    }

    [Fact]
    public void Line_shaped_box_draws_nothing()
    {
        var line = new CollisionBox(new Vector3(-10, 0, 2), new Vector3(10, 0, 2));

        Assert.Empty(Frame(Space(), [line], []));
    }

    [Fact]
    public void Floor_under_the_player_is_painted_before_the_player()
    {
        var floor = new CollisionBox(new Vector3(-20, -20, -20), new Vector3(20, 0, 20));

        var items = Frame(Space(0.4f), [floor], [Self()]);

        Assert.True(Array.FindLastIndex(items, p => p.Tag == CollisionCubes.TagFor(0)) < Array.FindIndex(items, p => p.Tag == 1));
    }

    [Fact]
    public void Floor_reaching_towards_the_viewer_is_still_painted_under_a_player_standing_on_it()
    {
        // The floor runs from the player's feet towards the viewer (south): its centre is nearer than the player, so
        // sorting it by its centre would paint it over the player. Its farthest corner is behind the player.
        var floor = new CollisionBox(new Vector3(-20, -1, 0), new Vector3(20, 0, 20));
        var space = Space();
        var projection = Projection(space);
        Assert.True(projection.Depth(space.ToView(floor.Centre)) < projection.Depth(Vector3.Zero), "Test set-up: the floor's centre must be nearer than the player.");

        var items = Frame(space, [floor], [Self()]);

        Assert.True(Array.FindLastIndex(items, p => p.Tag == CollisionCubes.TagFor(0)) < Array.FindIndex(items, p => p.Tag == 1));
    }

    [Fact]
    public void Player_in_front_of_a_wall_is_painted_over_it()
    {
        // A wall north of the player (north is −Z, away from the viewer at yaw 0).
        var wall = new CollisionBox(new Vector3(-20, -20, -5), new Vector3(20, 20, -4));

        var items = Frame(Space(), [wall], [Self()]);

        Assert.True(Array.FindLastIndex(items, p => p.Tag == CollisionCubes.TagFor(0)) < Array.FindIndex(items, p => p.Tag == 1));
    }

    [Fact]
    public void Small_box_on_a_big_one_is_painted_after_it()
    {
        var ground = new CollisionBox(new Vector3(-20, -20, -20), new Vector3(20, -2, 20));
        var crate = new CollisionBox(new Vector3(3, -2, 3), new Vector3(5, 0, 5));

        var items = Frame(Space(0.4f), [crate, ground], []);

        Assert.True(Array.FindLastIndex(items, p => p.Tag == CollisionCubes.TagFor(1)) < Array.FindIndex(items, p => p.Tag == CollisionCubes.TagFor(0)));
    }

    [Theory]
    [InlineData(0f)]
    [InlineData(0.4f)]
    [InlineData(2.5f)]
    public void Far_depth_is_the_depth_of_the_farthest_corner(float yaw)
    {
        var space = new CubeSpace(new Vector3(100, 5, -30), 40, yaw);
        var projection = Projection(space);
        var box = new CollisionBox(new Vector3(95, 0, -40), new Vector3(110, 8, -33));

        var far = new BoxProjector(space, projection).FarDepth(box.Centre, box.HalfSize);

        var expected = float.MinValue;
        for (var i = 0; i < 8; i++)
        {
            var corner = new Vector3((i & 1) != 0 ? box.Max.X : box.Min.X, (i & 2) != 0 ? box.Max.Y : box.Min.Y, (i & 4) != 0 ? box.Max.Z : box.Min.Z);
            expected = MathF.Max(expected, projection.Depth(space.ToView(corner)));
        }

        Assert.Equal(expected, far, 3);
    }

    [Fact]
    public void Drawing_a_full_set_of_boxes_does_not_allocate()
    {
        var space = Space(0.4f);
        var projection = Projection(space);
        var list = new DrawList(3 * CollisionBoxSet.DefaultCapacity);
        var boxes = new ClippedBox[CollisionBoxSet.DefaultCapacity];
        for (var i = 0; i < boxes.Length; i++)
        {
            var min = new Vector3((i % 40) - 20, (i / 40 % 10) - 5, (i / 400) - 3);
            boxes[i] = new ClippedBox(new CollisionBox(min, min + new Vector3(0.8f, 0.5f, 0.9f)));
        }

        Allocations.AssertNone(() =>
        {
            list.Clear();
            CollisionCubes.Add(list, new BoxProjector(space, projection), boxes, Grey);
            list.SortBackToFront();
        });
        Assert.Equal(0, list.Dropped);
    }
}
