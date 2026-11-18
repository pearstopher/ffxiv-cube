using System.Numerics;
using System.Text;
using Cube.Core;

namespace Cube.Tests;

/// <summary>
/// Checks which players and NPCs are kept and how they're classified and named (PROJECT.md, "Players": only players
/// inside the cube are shown; "NPCs": named, unowned NPCs inside the cube are shown, with an on/off setting; "Colours":
/// self, party/alliance, everyone else, NPCs; "Efficiency and memory": name labels cached
/// and only rebuilt when a name changes, no per-frame allocations, bounded memory; "Privacy": names stay in memory).
/// </summary>
public class PlayerSetTests
{
    private static readonly Vector3 Centre = new(10, 0, 10);
    private static readonly byte[] Alice = Encoding.UTF8.GetBytes("Alice Example\0\0\0");
    private static readonly byte[] Bob = Encoding.UTF8.GetBytes("Bob Example");

    private static PlayerSet NewSet(int capacity = 16, int npcCapacity = 0) => new(capacity, new NameCache(256), npcCapacity);

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
    public void Npc_inside_the_cube_is_kept_as_an_npc_with_its_name()
    {
        var set = Begin(NewSet(npcCapacity: 4));

        Assert.True(set.OfferNpc(200, 7, Centre + new Vector3(3, 0, -3), Alice));

        var npc = Assert.Single(set.Items.ToArray());
        Assert.Equal(PlayerRelation.Npc, npc.Relation);
        Assert.Equal("Alice Example", npc.Name);
        Assert.Equal(1, set.NpcCount);
    }

    [Fact]
    public void Npc_outside_the_cube_is_not_kept()
    {
        var set = Begin(NewSet(npcCapacity: 4));

        Assert.False(set.OfferNpc(500, 7, Centre + new Vector3(0, 0, 25), Alice));

        Assert.Equal(0, set.Count);
        Assert.Equal(0, set.Names.Builds);
    }

    [Fact]
    public void With_npcs_off_no_npc_is_kept_but_players_still_are()
    {
        var set = NewSet(npcCapacity: 4);
        set.Begin(new CubeSpace(Centre, 40, 0), npcs: false);

        Assert.False(set.ShowsNpcs);
        Assert.False(set.OfferNpc(500, 7, Centre, Alice));
        Assert.True(set.Offer(0, 1, Centre, true, false, false, Bob));
        Assert.Equal(1, set.Count);
        Assert.Equal(0, set.NpcCount);
    }

    [Fact]
    public void Npc_cap_drops_extra_npcs_without_pushing_out_players()
    {
        var set = Begin(NewSet(capacity: 2, npcCapacity: 2));

        for (var i = 0; i < 5; i++)
        {
            set.OfferNpc(200 + i, (uint)i, Centre, Bob);
        }

        Assert.True(set.Offer(0, 1, Centre, true, false, false, Alice));
        Assert.True(set.Offer(2, 2, Centre, false, false, false, Alice));
        Assert.Equal(2, set.NpcCount);
        Assert.Equal(4, set.Count);
        Assert.Equal(3, set.Dropped);
    }

    [Fact]
    public void Unowned_npc_with_a_name_is_a_named_npc()
    {
        Assert.True(PlayerSet.IsNamedNpc(PlayerSet.NoOwner, false, "Guard\0"u8));
    }

    [Fact]
    public void Player_owned_pet_is_not_treated_as_an_npc()
    {
        // Owned by a player (OwnerId set), or a pet/chocobo by sub-kind: either is enough to leave it out.
        Assert.False(PlayerSet.IsNamedNpc(0x10000001, false, "Carbuncle"u8));
        Assert.False(PlayerSet.IsNamedNpc(PlayerSet.NoOwner, true, "Carbuncle"u8));
    }

    [Fact]
    public void Nameless_npc_is_not_a_named_npc()
    {
        Assert.False(PlayerSet.IsNamedNpc(PlayerSet.NoOwner, false, "\0\0\0"u8));
        Assert.False(PlayerSet.IsNamedNpc(PlayerSet.NoOwner, false, ReadOnlySpan<byte>.Empty));
    }

    [Fact]
    public void Reading_the_same_npcs_every_frame_does_not_allocate()
    {
        var set = NewSet(capacity: 4, npcCapacity: 50);
        var space = new CubeSpace(Centre, 40, 0.3f);

        Allocations.AssertNone(() =>
        {
            set.Begin(space);
            for (var i = 0; i < 60; i++)
            {
                set.OfferNpc(100 + i, (uint)i, Centre + new Vector3(i % 30 - 15, 0, 0), i % 2 == 0 ? Alice : Bob);
            }
        });
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
