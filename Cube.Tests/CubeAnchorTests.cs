using System.Numerics;
using System.Text;
using Cube.Core;

namespace Cube.Tests;

/// <summary>
/// Checks "Jump" (PROJECT.md, "Exact behaviour rules", Cube; #105): while the player jumps the cube's height is held at
/// the take-off height and x and z still follow; landing snaps back to the player straight away; anything without the
/// jump flag (walking off a ledge, falling) follows as before; the player's own marker is always drawn, kept at the face
/// it would cross; and the zero-allocation rule ("Efficiency and memory").
/// </summary>
public class CubeAnchorTests
{
    private static readonly Vector3 Ground = new(10, 5, -20);

    [Fact]
    public void Without_a_jump_the_anchor_is_exactly_the_player()
    {
        var anchor = new CubeAnchor();

        Assert.Equal(Ground, anchor.Update(Ground, false));
        Assert.Equal(Ground + new Vector3(1, 2, 3), anchor.Update(Ground + new Vector3(1, 2, 3), false));
        Assert.False(anchor.IsHeld);
    }

    [Fact]
    public void During_a_jump_the_height_is_held_at_take_off_and_x_and_z_follow()
    {
        var anchor = new CubeAnchor();
        anchor.Update(Ground, false);

        var up = anchor.Update(Ground + new Vector3(1, 2, 3), true);
        var top = anchor.Update(Ground + new Vector3(2, 6, 4), true);

        Assert.Equal(new Vector3(11, 5, -17), up);
        Assert.Equal(new Vector3(12, 5, -16), top);
        Assert.True(anchor.IsHeld);
    }

    [Fact]
    public void Take_off_height_is_the_previous_frames_height_if_the_flag_arrives_a_frame_late()
    {
        var anchor = new CubeAnchor();
        anchor.Update(Ground, false);

        var first = anchor.Update(Ground + new Vector3(0, 0.4f, 0), true);

        Assert.Equal(Ground.Y, first.Y);
    }

    [Theory]
    [InlineData(5f)]
    [InlineData(9f)]
    [InlineData(-3f)]
    public void Landing_snaps_to_the_players_height_in_the_frame_the_flag_clears(float landingY)
    {
        var anchor = new CubeAnchor();
        anchor.Update(Ground, false);
        anchor.Update(Ground + new Vector3(0, 3, 0), true);

        var landed = anchor.Update(new Vector3(10, landingY, -20), false);

        Assert.Equal(landingY, landed.Y);
        Assert.False(anchor.IsHeld);
    }

    [Fact]
    public void Walking_off_a_ledge_without_the_flag_follows_the_fall()
    {
        var anchor = new CubeAnchor();
        anchor.Update(Ground, false);

        for (var i = 1; i <= 5; i++)
        {
            var player = Ground - new Vector3(0, i, 0);
            Assert.Equal(player, anchor.Update(player, false));
        }
    }

    [Fact]
    public void A_second_jump_holds_its_own_take_off_height()
    {
        var anchor = new CubeAnchor();
        anchor.Update(Ground, false);
        anchor.Update(Ground + new Vector3(0, 2, 0), true);
        anchor.Update(Ground + new Vector3(0, 8, 0), false);

        anchor.Update(Ground + new Vector3(0, 8, 0), false);
        var second = anchor.Update(Ground + new Vector3(0, 10, 0), true);

        Assert.Equal(Ground.Y + 8, second.Y);
    }

    [Fact]
    public void Reset_drops_the_hold_and_the_remembered_height()
    {
        var anchor = new CubeAnchor();
        anchor.Update(Ground, false);
        anchor.Update(Ground, true);
        anchor.Reset();

        Assert.False(anchor.IsHeld);
        Assert.Equal(60f, anchor.Update(new Vector3(0, 60, 0), true).Y);
    }

    [Fact]
    public void The_held_height_gets_the_player_height_offset_like_any_player_position()
    {
        var anchor = new CubeAnchor();
        anchor.Update(Ground, false);
        var held = anchor.Update(Ground + new Vector3(0, 4, 0), true);

        var box = new CubeSpace(held, new Vector3(40), ViewYaw.NorthUp, 0f);

        Assert.Equal(Ground.Y - 0f + 20f, box.Centre.Y);
        Assert.Equal(Ground.Y, box.Centre.Y - 20f);
    }

    [Fact]
    public void While_held_the_world_stays_put_in_the_cube_and_the_player_moves_up()
    {
        var anchor = new CubeAnchor();
        var thing = new Vector3(14, 9, -20);
        anchor.Update(Ground, false);
        var before = new CubeSpace(anchor.Update(Ground, false), new Vector3(40), ViewYaw.NorthUp).ToView(thing);

        var during = new CubeSpace(anchor.Update(Ground + new Vector3(0, 3, 0), true), new Vector3(40), ViewYaw.NorthUp);

        Assert.Equal(before, during.ToView(thing));
        Assert.Equal(3f, during.ToView(Ground + new Vector3(0, 3, 0)).Y);
    }

    [Fact]
    public void Update_and_Reset_allocate_nothing()
    {
        var anchor = new CubeAnchor();
        var flip = false;
        var jump = () =>
        {
            flip = !flip;
            anchor.Update(Ground, flip);
        };

        Allocations.AssertNone(jump, 100);
        Allocations.AssertNone(anchor.Reset, 100);
    }

    private static PlayerSet Begin(CubeSpace space)
    {
        var set = new PlayerSet(8, new NameCache(64), 0);
        set.Begin(space);
        return set;
    }

    private static readonly byte[] Name = Encoding.UTF8.GetBytes("Me");

    [Theory]
    [InlineData(0f)]
    [InlineData(0.5f)]
    [InlineData(1f)]
    public void The_local_marker_is_kept_at_the_face_it_would_cross(float playerHeight)
    {
        var anchor = new CubeAnchor();
        anchor.Update(Ground, false);
        var held = anchor.Update(Ground + new Vector3(0, 3, 0), true);
        var space = new CubeSpace(held, new Vector3(40), ViewYaw.NorthUp, playerHeight);

        foreach (var jumpTo in new[] { 500f, -500f })
        {
            var set = Begin(space);
            var real = Ground + new Vector3(0, jumpTo, 0);

            Assert.True(set.Offer(0, 1, real, true, false, false, Name));

            var view = space.ToView(set.Items[0].Position);
            Assert.InRange(MathF.Abs(view.Y), space.Half.Y * 0.999f, space.Half.Y);
            Assert.Equal(jumpTo > 0 ? 1f : -1f, MathF.Sign(view.Y));
        }
    }

    [Fact]
    public void The_local_marker_inside_the_box_is_left_where_it_is()
    {
        var space = new CubeSpace(Ground, new Vector3(40), ViewYaw.NorthUp);
        var set = Begin(space);
        var real = Ground + new Vector3(2, 7, 1);

        set.Offer(0, 1, real, true, false, false, Name);

        Assert.Equal(real, set.Items[0].Position);
    }

    [Fact]
    public void Other_players_outside_the_box_are_still_culled_while_held()
    {
        var space = new CubeSpace(Ground, new Vector3(40), ViewYaw.NorthUp);
        var set = Begin(space);

        Assert.False(set.Offer(2, 2, Ground + new Vector3(0, 500, 0), false, false, false, Name));
        Assert.Equal(0, set.Count);
    }
}
