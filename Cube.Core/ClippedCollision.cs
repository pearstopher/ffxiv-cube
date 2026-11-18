using System.Numerics;

namespace Cube.Core;

/// <summary>
/// The cached collision boxes clipped to this frame's cube, ready for <see cref="CollisionCubes"/> (PROJECT.md,
/// "Collision (phase 2)": boxes clipped to the cube). Clips again only when the cube or the gathered boxes change.
/// </summary>
/// <remarks>
/// PROJECT.md, "Efficiency and memory": recompute only when the inputs change. The inputs are the cube (centre, size,
/// alignment yaw) and the cache's ready set (which set it is, whether it holds data, and how many gathers have
/// completed); standing still with the camera still clips nothing. The buffer is allocated once at the cache's
/// capacity, so clipping can never drop a box and memory never grows.
/// </remarks>
public sealed class ClippedCollision
{
    /// <summary>The reused buffer of clipped boxes.</summary>
    private readonly CollisionBox[] boxes;

    /// <summary>The ready set the boxes were clipped from.</summary>
    private CollisionBoxSet? set;

    /// <summary>Whether that set held data when clipped.</summary>
    private bool hadData;

    /// <summary>The cache's completed-gather count when clipped.</summary>
    private long completed = -1;

    /// <summary>The cube's centre when clipped.</summary>
    private Vector3 centre;

    /// <summary>The cube's size when clipped.</summary>
    private Vector3 size;

    /// <summary>The cube's yaw when clipped.</summary>
    private float yaw;

    /// <summary>Creates an empty list.</summary>
    /// <param name="capacity">Most boxes; the cache's set capacity, so nothing is ever cut off.</param>
    public ClippedCollision(int capacity = CollisionBoxSet.DefaultCapacity)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(capacity, 1);
        this.boxes = new CollisionBox[capacity];
    }

    /// <summary>How many boxes reach into the cube.</summary>
    public int Count { get; private set; }

    /// <summary>How many times the boxes were clipped again; the bridge and tests check that unchanged inputs don't.</summary>
    public long Clips { get; private set; }

    /// <summary>The boxes inside the cube, clipped to it, in world coordinates.</summary>
    public ReadOnlySpan<CollisionBox> Items => this.boxes.AsSpan(0, this.Count);

    /// <summary>Clips the cache's ready boxes to the cube if either changed since the last call.</summary>
    /// <param name="cube">This frame's cube.</param>
    /// <param name="cache">The collision cache.</param>
    /// <returns>True if the boxes were clipped again.</returns>
    public bool Update(in CubeSpace cube, CollisionCache cache)
    {
        var ready = cache.Ready;
        if (ReferenceEquals(ready, this.set) && ready.HasData == this.hadData && cache.Completed == this.completed &&
            cube.Centre == this.centre && cube.Size == this.size && cube.Yaw == this.yaw)
        {
            return false;
        }

        this.set = ready;
        this.hadData = ready.HasData;
        this.completed = cache.Completed;
        this.centre = cube.Centre;
        this.size = cube.Size;
        this.yaw = cube.Yaw;
        this.Count = ready.HasData ? CollisionClipper.Clip(cube, ready.Items, this.boxes) : 0;
        this.Clips++;
        return true;
    }
}
