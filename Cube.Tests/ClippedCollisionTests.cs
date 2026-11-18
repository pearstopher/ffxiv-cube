using System.Numerics;
using Cube.Core;

namespace Cube.Tests;

/// <summary>
/// Checks the gathered collision is clipped to the cube for drawing, and only when something changed (PROJECT.md,
/// "Collision (phase 2)": boxes clipped to the cube; "Efficiency and memory": recompute only when the inputs change,
/// no per-frame allocations, collision data dropped on a zone change).
/// </summary>
public class ClippedCollisionTests
{
    private static readonly Vector3 Edge = new(40f);
    private const uint Zone = 132;

    private static readonly CollisionBox Inside = new(new Vector3(-2, -1, -2), new Vector3(2, 0, 2));
    private static readonly CollisionBox Crossing = new(new Vector3(15, -1, -2), new Vector3(30, 0, 2));
    private static readonly CollisionBox Outside = new(new Vector3(-2, 25, -2), new Vector3(2, 26, 2));

    private static CollisionCache Gathered(Vector3 centre, params CollisionBox[] boxes)
    {
        var cache = new CollisionCache(budgetTicks: 1_000_000, capacity: 16);
        var source = new ListSource(boxes);
        Assert.True(cache.Step(ref source, centre, Edge, Zone, sceneStamp: 1));
        return cache;
    }

    [Fact]
    public void Boxes_are_clipped_to_the_cube_and_those_outside_are_left_out()
    {
        var cache = Gathered(Vector3.Zero, Inside, Crossing, Outside);
        var clipped = new ClippedCollision(16);

        clipped.Update(new CubeSpace(Vector3.Zero, Edge, 0f), cache);

        Assert.Equal(2, clipped.Count);
        Assert.Equal(Inside.Max, clipped.Items[0].Max);
        Assert.Equal(20f, clipped.Items[1].Max.X, 4);
    }

    [Fact]
    public void Same_cube_and_same_gather_is_not_clipped_again()
    {
        var cache = Gathered(Vector3.Zero, Inside, Crossing);
        var clipped = new ClippedCollision(16);
        var space = new CubeSpace(Vector3.Zero, Edge, 0.3f);

        Assert.True(clipped.Update(space, cache));
        Assert.False(clipped.Update(space, cache));
        Assert.Equal(1, clipped.Clips);
    }

    [Fact]
    public void Moving_or_turning_the_cube_clips_again()
    {
        var cache = Gathered(Vector3.Zero, Inside, Crossing);
        var clipped = new ClippedCollision(16);
        clipped.Update(new CubeSpace(Vector3.Zero, Edge, 0f), cache);

        Assert.True(clipped.Update(new CubeSpace(new Vector3(1, 0, 0), Edge, 0f), cache));
        Assert.True(clipped.Update(new CubeSpace(new Vector3(1, 0, 0), Edge, 0.5f), cache));
        Assert.True(clipped.Update(new CubeSpace(new Vector3(1, 0, 0), 60, 0.5f), cache));
        Assert.Equal(4, clipped.Clips);
    }

    [Fact]
    public void A_new_gather_is_clipped_again()
    {
        var cache = Gathered(Vector3.Zero, Inside);
        var clipped = new ClippedCollision(16);
        var space = new CubeSpace(Vector3.Zero, Edge, 0f);
        clipped.Update(space, cache);

        cache.Invalidate();
        var source = new ListSource([Inside, Crossing]);
        cache.Step(ref source, Vector3.Zero, Edge, Zone, sceneStamp: 1);

        Assert.True(clipped.Update(space, cache));
        Assert.Equal(2, clipped.Count);
    }

    [Fact]
    public void Collision_dropped_on_a_zone_change_is_no_longer_drawn()
    {
        var cache = Gathered(Vector3.Zero, Inside);
        var clipped = new ClippedCollision(16);
        var space = new CubeSpace(Vector3.Zero, Edge, 0f);
        clipped.Update(space, cache);

        cache.Clear();

        Assert.True(clipped.Update(space, cache));
        Assert.Equal(0, clipped.Count);
    }

    [Fact]
    public void Clipping_every_frame_does_not_allocate()
    {
        var cache = Gathered(Vector3.Zero, Inside, Crossing, Outside);
        var clipped = new ClippedCollision(16);
        var step = 0;

        Allocations.AssertNone(() => clipped.Update(new CubeSpace(new Vector3(step++ * 0.1f, 0, 0), Edge, 0.2f), cache));
    }

    /// <summary>A fake collision scene: one box per collider, gathered in one frame.</summary>
    /// <param name="boxes">The scene's boxes, one per collider.</param>
    private struct ListSource(CollisionBox[] boxes) : ICollisionSource
    {
        private int next;

        public readonly long Now => 0;

        public bool Seek(int index)
        {
            this.next = index;
            return index <= boxes.Length;
        }

        public bool OfferNext(CollisionBoxSet set)
        {
            if (this.next >= boxes.Length)
            {
                return false;
            }

            set.Offer(boxes[this.next++]);
            return true;
        }
    }
}
