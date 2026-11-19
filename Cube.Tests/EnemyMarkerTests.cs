using System.Numerics;
using System.Text;
using Cube.Core;

namespace Cube.Tests;

/// <summary>
/// Checks #111: hostile battle NPCs are enemies with their own colour, the current target's label starts with
/// <c>[T]</c>, an enemy attacking the player's starts with <c>[!]</c>, both read <c>[T] [!] Name</c>, and the prefixed
/// strings are cached rather than rebuilt every frame (PROJECT.md, "Efficiency and memory": no per-frame allocation).
/// </summary>
public class EnemyMarkerTests
{
    private static readonly Vector3 Centre = new(10, 0, 10);
    private static readonly byte[] Wolf = Encoding.UTF8.GetBytes("Dire Wolf");

    private static PlayerSet NewSet()
    {
        var set = new PlayerSet(8, new NameCache(16), 8);
        set.Begin(new CubeSpace(Centre, new Vector3(40), 0));
        return set;
    }

    [Theory]
    [InlineData(true, true, PlayerRelation.Enemy)]
    [InlineData(true, false, PlayerRelation.Npc)]
    [InlineData(false, true, PlayerRelation.Npc)]
    [InlineData(false, false, PlayerRelation.Npc)]
    public void OnlyAHostileBattleNpc_IsAnEnemy(bool battle, bool hostile, PlayerRelation expected) =>
        Assert.Equal(expected, PlayerSet.ClassifyNpc(battle, hostile));

    [Fact]
    public void EnemyOffered_IsKeptAsAnEnemyAndCountsAsAnNpc()
    {
        var set = NewSet();

        Assert.True(set.OfferNpc(1, 5, Centre, Wolf, isEnemy: true));

        Assert.Equal(PlayerRelation.Enemy, set.Items[0].Relation);
        Assert.Equal(1, set.NpcCount);
    }

    [Fact]
    public void EnemyColour_IsItsOwnAndDiffersFromNpc()
    {
        var palette = new CubeSettings().Palette();

        Assert.Equal(palette.Enemy, palette.For(PlayerRelation.Enemy));
        Assert.NotEqual(palette.Npc, palette.Enemy);
    }

    [Fact]
    public void EnemyColour_IsSavedAndRepaired()
    {
        var settings = new CubeSettings { EnemyColour = new Vector3(float.NaN, 0, 0) };
        settings.Changed();
        Assert.Equal(CubeSettings.DefaultEnemyColour, settings.EnemyColour);

        var copy = new CubeSettings();
        copy.CopyFrom(new CubeSettings { EnemyColour = new Vector3(0.1f, 0.2f, 0.3f) });
        Assert.Equal(new Vector3(0.1f, 0.2f, 0.3f), copy.EnemyColour);
    }

    [Theory]
    [InlineData(false, false, "Dire Wolf")]
    [InlineData(true, false, "[T] Dire Wolf")]
    [InlineData(false, true, "[!] Dire Wolf")]
    [InlineData(true, true, "[T] [!] Dire Wolf")]
    public void EnemyLabel_HasTargetFirstThenAttacking(bool target, bool attacking, string expected)
    {
        var set = NewSet();

        set.OfferNpc(1, 5, Centre, Wolf, isEnemy: true, isTarget: target, isAttacking: attacking);

        Assert.Equal(expected, set.Items[0].Label);
        Assert.Equal("Dire Wolf", set.Items[0].Name);
    }

    [Fact]
    public void FriendlyNpc_NeverGetsTheAttackingMarker()
    {
        var set = NewSet();

        set.OfferNpc(1, 5, Centre, Wolf, isEnemy: false, isTarget: true, isAttacking: true);

        Assert.Equal("[T] Dire Wolf", set.Items[0].Label);
    }

    [Fact]
    public void TargetedPlayer_GetsTheTargetMarker()
    {
        var set = NewSet();

        set.Offer(1, 5, Centre, false, false, false, "Alice"u8, isTarget: true);

        Assert.Equal("[T] Alice", set.Items[0].Label);
    }

    [Fact]
    public void UnchangedMarkers_ReuseTheSameStringEveryFrame()
    {
        var set = NewSet();
        set.OfferNpc(1, 5, Centre, Wolf, true, true, true);
        var first = set.Items[0].Label;
        var builds = set.Markers.Builds;

        for (var i = 0; i < 5; i++)
        {
            set.Begin(new CubeSpace(Centre, new Vector3(40), 0));
            set.OfferNpc(1, 5, Centre, Wolf, true, true, true);
        }

        Assert.Same(first, set.Items[0].Label);
        Assert.Equal(builds, set.Markers.Builds);
    }

    [Fact]
    public void ChangedMarkers_RebuildTheLabel()
    {
        var set = NewSet();
        set.OfferNpc(1, 5, Centre, Wolf, true, true, false);
        set.Begin(new CubeSpace(Centre, new Vector3(40), 0));

        set.OfferNpc(1, 5, Centre, Wolf, true, true, true);

        Assert.Equal("[T] [!] Dire Wolf", set.Items[0].Label);
    }

    [Fact]
    public void UnmarkedLabel_IsTheNameItselfAndBuildsNothing()
    {
        var set = NewSet();

        set.OfferNpc(1, 5, Centre, Wolf, true);

        Assert.Same(set.Items[0].Name, set.Items[0].Label);
        Assert.Equal(0, set.Markers.Builds);
    }

    [Fact]
    public void SteadyMarkedLabels_AllocateNothing()
    {
        var set = NewSet();
        set.OfferNpc(1, 5, Centre, Wolf, true, true, true);
        var cube = new CubeSpace(Centre, new Vector3(40), 0);

        Allocations.AssertNone(() =>
        {
            set.Begin(cube);
            set.OfferNpc(1, 5, Centre, Wolf, true, true, true);
        });
    }
}
