using System.Numerics;
using Cube.Core;

namespace Cube.Tests;

/// <summary>
/// Checks the collision reader's gathering rules (PROJECT.md, "Collision (phase 2)": expensive gathering is spread
/// over frames with a time budget and cached, areas not yet processed are simply empty, stale data is refreshed when
/// the player moves; "Efficiency and memory": bounded memory, collision data dropped on a zone change, no per-frame
/// allocations). The game side is replaced by a fake scene whose colliders each cost a set number of clock ticks.
/// </summary>
public class CollisionCacheTests
{
    private const float Edge = 40f;
    private const uint Zone = 132;
    private const ulong Stamp = 7;

    // A scene of `count` colliders, one small box each, laid out near the origin.
    private static CollisionBox[] Scene(int count)
    {
        var boxes = new CollisionBox[count];
        for (var i = 0; i < count; i++)
        {
            // Spaced 2 yalms apart on X with gaps, so neighbours never merge.
            var min = new Vector3(-15 + (i * 2f), 0, 0);
            boxes[i] = new CollisionBox(min, min + new Vector3(1, 1, 1));
        }

        return boxes;
    }

    private static CollisionCache Cache(long budget = 1000) => new(budget, capacity: 64);

    [Fact]
    public void Small_scene_is_gathered_in_one_frame_when_within_budget()
    {
        var cache = Cache();
        var source = new FakeSource(Scene(5), ticksPerCollider: 1);

        var completed = cache.Step(ref source, Vector3.Zero, Edge, Zone, Stamp);

        Assert.True(completed);
        Assert.Equal(5, cache.Ready.Count);
        Assert.False(cache.IsGathering);
        Assert.Equal(1, cache.GatherFrames);
    }

    [Fact]
    public void Gather_is_spread_over_frames_when_the_budget_runs_out()
    {
        // Budget 10 ticks, 4 per collider: three colliders fit in a frame (the third crosses the budget).
        var cache = Cache(budget: 10);
        var source = new FakeSource(Scene(10), ticksPerCollider: 4);

        var frames = 0;
        while (!cache.Step(ref source, Vector3.Zero, Edge, Zone, Stamp))
        {
            frames++;
            Assert.True(cache.LastStepColliders <= 3);
            Assert.True(frames < 20, "gather never finished");
        }

        Assert.Equal(10, cache.Ready.Count);
        Assert.Equal(4, cache.GatherFrames);
    }

    [Fact]
    public void Each_frame_resumes_at_the_next_collider_by_index()
    {
        var cache = Cache(budget: 10);
        var source = new FakeSource(Scene(10), ticksPerCollider: 4);

        cache.Step(ref source, Vector3.Zero, Edge, Zone, Stamp);
        cache.Step(ref source, Vector3.Zero, Edge, Zone, Stamp);

        Assert.Equal(3, source.LastSeek);
    }

    [Fact]
    public void Slow_collider_still_makes_progress_one_per_frame()
    {
        var cache = Cache(budget: 10);
        var source = new FakeSource(Scene(3), ticksPerCollider: 100);

        var frames = 1;
        while (!cache.Step(ref source, Vector3.Zero, Edge, Zone, Stamp))
        {
            Assert.Equal(1, cache.LastStepColliders);
            frames++;
        }

        // Three frames offer one collider each; the fourth finds the end of the list.
        Assert.Equal(4, frames);
        Assert.Equal(3, cache.Ready.Count);
    }

    [Fact]
    public void Area_not_yet_gathered_is_empty()
    {
        var cache = Cache(budget: 10);
        var source = new FakeSource(Scene(10), ticksPerCollider: 4);

        cache.Step(ref source, Vector3.Zero, Edge, Zone, Stamp);

        Assert.True(cache.IsGathering);
        Assert.False(cache.Ready.HasData);
        Assert.Equal(0, cache.Ready.Count);
    }

    [Fact]
    public void Nothing_is_read_while_the_cached_boxes_are_current()
    {
        var cache = Cache();
        var source = new FakeSource(Scene(5), ticksPerCollider: 1);
        cache.Step(ref source, Vector3.Zero, Edge, Zone, Stamp);
        var seeks = source.Seeks;

        var completed = cache.Step(ref source, new Vector3(1, 0, 1), Edge, Zone, Stamp);

        Assert.False(completed);
        Assert.Equal(seeks, source.Seeks);
        Assert.Equal(0, cache.LastStepColliders);
    }

    [Fact]
    public void Old_boxes_stay_drawn_while_a_refresh_is_gathered()
    {
        var cache = Cache(budget: 10);
        var source = new FakeSource(Scene(10), ticksPerCollider: 4);
        while (!cache.Step(ref source, Vector3.Zero, Edge, Zone, Stamp))
        {
        }

        // Walk past the refresh threshold (a quarter of the edge).
        var moved = new Vector3(Edge * 0.3f, 0, 0);
        cache.Step(ref source, moved, Edge, Zone, Stamp);

        Assert.True(cache.IsGathering);
        Assert.Equal(10, cache.Ready.Count);
        Assert.Equal(Vector3.Zero, cache.Ready.Region.Centre);
    }

    [Fact]
    public void Moving_past_the_threshold_refreshes_around_the_new_position()
    {
        var cache = Cache();
        var source = new FakeSource(Scene(5), ticksPerCollider: 1);
        cache.Step(ref source, Vector3.Zero, Edge, Zone, Stamp);

        var moved = new Vector3(0, 0, Edge * 0.3f);
        var completed = cache.Step(ref source, moved, Edge, Zone, Stamp);

        Assert.True(completed);
        Assert.Equal(2, cache.Completed);
        Assert.Equal(moved, cache.Ready.Region.Centre);
    }

    [Fact]
    public void Zone_change_drops_the_boxes_at_once()
    {
        var cache = Cache(budget: 10);
        var source = new FakeSource(Scene(10), ticksPerCollider: 4);
        while (!cache.Step(ref source, Vector3.Zero, Edge, Zone, Stamp))
        {
        }

        cache.Step(ref source, Vector3.Zero, Edge, Zone + 1, Stamp);

        Assert.False(cache.Ready.HasData);
        Assert.Equal(0, cache.Ready.Count);
        Assert.True(cache.IsGathering);
    }

    [Fact]
    public void Scene_change_during_a_gather_starts_it_again()
    {
        var cache = Cache(budget: 10);
        var source = new FakeSource(Scene(10), ticksPerCollider: 4);
        cache.Step(ref source, Vector3.Zero, Edge, Zone, Stamp);

        cache.Step(ref source, Vector3.Zero, Edge, Zone, Stamp + 1);

        Assert.Equal(1, cache.Restarts);
        Assert.Equal(1, cache.GatherFrames);
        Assert.Equal(0, source.LastSeek);
    }

    [Fact]
    public void Invalidate_keeps_the_boxes_and_gathers_again()
    {
        var cache = Cache();
        var source = new FakeSource(Scene(5), ticksPerCollider: 1);
        cache.Step(ref source, Vector3.Zero, Edge, Zone, Stamp);

        cache.Invalidate();

        Assert.Equal(5, cache.Ready.Count);
        Assert.True(cache.Step(ref source, Vector3.Zero, Edge, Zone, Stamp));
        Assert.Equal(2, cache.Completed);
    }

    [Fact]
    public void Scene_shorter_than_the_resume_point_finishes_the_gather()
    {
        var cache = Cache(budget: 10);
        var source = new FakeSource(Scene(10), ticksPerCollider: 4);
        cache.Step(ref source, Vector3.Zero, Edge, Zone, Stamp);

        source.Truncate(2);

        Assert.True(cache.Step(ref source, Vector3.Zero, Edge, Zone, Stamp));
        Assert.False(cache.IsGathering);
    }

    [Fact]
    public void Memory_is_bounded_by_the_cap()
    {
        var cache = new CollisionCache(budgetTicks: 1_000_000, capacity: 4);
        var source = new FakeSource(Scene(10), ticksPerCollider: 1);

        cache.Step(ref source, Vector3.Zero, Edge, Zone, Stamp);

        Assert.Equal(4, cache.Ready.Count);
        Assert.Equal(6, cache.Ready.Dropped);
    }

    [Fact]
    public void Gathering_and_idle_frames_allocate_nothing()
    {
        var cache = Cache(budget: 10);
        var source = new FakeSource(Scene(30), ticksPerCollider: 4);
        var far = new Vector3(Edge, 0, 0);

        // Warm up: JIT and a full gather in both directions.
        for (var i = 0; i < 40; i++)
        {
            cache.Step(ref source, (i & 8) == 0 ? Vector3.Zero : far, Edge, Zone, Stamp);
        }

        var before = GC.GetAllocatedBytesForCurrentThread();
        for (var i = 0; i < 200; i++)
        {
            cache.Step(ref source, (i & 16) == 0 ? Vector3.Zero : far, Edge, Zone + (uint)(i / 100), Stamp);
        }

        Assert.Equal(0, GC.GetAllocatedBytesForCurrentThread() - before);
    }

    /// <summary>A scene of fixed boxes, one per collider, whose clock advances a set amount per collider offered.</summary>
    private struct FakeSource : ICollisionSource
    {
        private readonly CollisionBox[] colliders;
        private readonly long ticksPerCollider;
        private int length;
        private int current;

        public FakeSource(CollisionBox[] colliders, long ticksPerCollider)
        {
            this.colliders = colliders;
            this.ticksPerCollider = ticksPerCollider;
            this.length = colliders.Length;
        }

        public long Now { get; private set; }

        public int Seeks { get; private set; }

        public int LastSeek { get; private set; }

        public void Truncate(int length) => this.length = length;

        public bool Seek(int index)
        {
            this.Seeks++;
            this.LastSeek = index;
            this.current = index;
            return index <= this.length;
        }

        public bool OfferNext(CollisionBoxSet set)
        {
            if (this.current >= this.length)
            {
                return false;
            }

            var box = this.colliders[this.current++];
            if (set.Overlaps(box))
            {
                set.Offer(box);
            }

            this.Now += this.ticksPerCollider;
            return true;
        }
    }
}
