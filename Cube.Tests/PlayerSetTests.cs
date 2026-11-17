using System.Numerics;
using System.Text;
using Cube.Core;

namespace Cube.Tests;

/// <summary>
/// Checks which players are kept and how they're classified and named (PROJECT.md, "Players": only players inside
/// the cube are shown; "Colours": self, party/alliance, everyone else; "Efficiency and memory": name labels cached
/// and only rebuilt when a name changes, no per-frame allocations, bounded memory; "Privacy": names stay in memory).
/// </summary>
public class PlayerSetTests
{
    private static readonly Vector3 Centre = new(10, 0, 10);
    private static readonly byte[] Alice = Encoding.UTF8.GetBytes("Alice Example\0\0\0");
    private static readonly byte[] Bob = Encoding.UTF8.GetBytes("Bob Example");

    private static PlayerSet NewSet(int capacity = 16) => new(capacity, new NameCache(256));

    private static PlayerSet Begin(PlayerSet set)
    {
        set.Begin(new CubeSpace(Centre, 40, 0));
        return set;
    }

    [Fact]
    public void Player_inside_the_cube_is_kept_with_its_position_and_name()
    {
        var set = Begin(NewSet());

        Assert.True(set.Offer(2, 42, Centre + new Vector3(5, 0, 5), false, false, false, Alice));

        var player = Assert.Single(set.Items.ToArray());
        Assert.Equal(42u, player.EntityId);
        Assert.Equal(Centre + new Vector3(5, 0, 5), player.Position);
        Assert.Equal("Alice Example", player.Name);
    }

    [Fact]
    public void Player_outside_the_cube_is_not_kept()
    {
        var set = Begin(NewSet());

        Assert.False(set.Offer(2, 42, Centre + new Vector3(21, 0, 0), false, false, false, Alice));

        Assert.Equal(0, set.Count);
        Assert.Equal(1, set.Offered);
    }

    [Fact]
    public void Player_outside_the_cube_is_rejected_before_its_name_is_read()
    {
        var set = Begin(NewSet());

        set.Offer(2, 42, Centre + new Vector3(0, 30, 0), false, false, false, Alice);

        Assert.Equal(0, set.Names.Builds);
    }

    [Theory]
    [InlineData(true, false, false, PlayerRelation.Self)]
    [InlineData(true, true, true, PlayerRelation.Self)]
    [InlineData(false, true, false, PlayerRelation.Party)]
    [InlineData(false, false, true, PlayerRelation.Party)]
    [InlineData(false, false, false, PlayerRelation.Other)]
    public void Relation_is_self_then_party_or_alliance_then_other(bool isLocal, bool isParty, bool isAlliance, PlayerRelation expected)
    {
        Assert.Equal(expected, PlayerSet.Classify(isLocal, isParty, isAlliance));
    }

    [Fact]
    public void A_new_frame_starts_empty()
    {
        var set = Begin(NewSet());
        set.Offer(2, 42, Centre, true, false, false, Alice);

        Begin(set);

        Assert.Equal(0, set.Count);
        Assert.Equal(0, set.Offered);
    }

    [Fact]
    public void A_full_set_drops_extra_players_instead_of_growing()
    {
        var set = Begin(NewSet(capacity: 2));

        set.Offer(0, 1, Centre, true, false, false, Alice);
        set.Offer(2, 2, Centre, false, false, false, Bob);
        set.Offer(4, 3, Centre, false, false, false, Bob);

        Assert.Equal(2, set.Count);
        Assert.Equal(1, set.Dropped);
    }

    [Fact]
    public void Same_name_every_frame_is_built_once_and_reused()
    {
        var set = NewSet();

        Begin(set).Offer(2, 42, Centre, false, false, false, Alice);
        var first = set.Items[0].Name;
        Begin(set).Offer(2, 42, Centre, false, false, false, Alice);

        Assert.Same(first, set.Items[0].Name);
        Assert.Equal(1, set.Names.Builds);
    }

    [Fact]
    public void Slot_reused_by_another_player_gets_the_new_name()
    {
        var set = NewSet();

        Begin(set).Offer(2, 42, Centre, false, false, false, Alice);
        Begin(set).Offer(2, 43, Centre, false, false, false, Bob);

        Assert.Equal("Bob Example", set.Items[0].Name);
        Assert.Equal(2, set.Names.Builds);
    }

    [Fact]
    public void Name_stops_at_the_first_zero_byte()
    {
        var cache = new NameCache(4);

        Assert.Equal("Al", cache.Get(0, "Al\0ice"u8));
    }

    [Fact]
    public void Slot_beyond_the_name_cache_is_ignored()
    {
        var set = new PlayerSet(4, new NameCache(2));
        set.Begin(new CubeSpace(Centre, 40, 0));

        Assert.False(set.Offer(5, 1, Centre, false, false, false, Alice));
    }

    [Fact]
    public void Reading_the_same_players_every_frame_does_not_allocate()
    {
        var set = NewSet(capacity: 64);
        var space = new CubeSpace(Centre, 40, 0.3f);

        Allocations.AssertNone(() =>
        {
            set.Begin(space);
            for (var i = 0; i < 40; i++)
            {
                set.Offer(i * 2, (uint)i, Centre + new Vector3(i - 20, 0, 20 - i), i == 0, i % 5 == 0, false, i % 2 == 0 ? Alice : Bob);
            }
        });
    }
}
