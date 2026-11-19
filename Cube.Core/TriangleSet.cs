using System.Numerics;

namespace Cube.Core;

/// <summary>
/// The collision triangles around the cube in world coordinates (#100; PROJECT.md, "Collision style"): the Mesh-style
/// counterpart of <see cref="CollisionBoxSet"/>. The game reader offers every triangle it reads; this class keeps the
/// ones that could be inside the cube at any yaw, up to a fixed capacity, and says when the set is stale.
/// </summary>
/// <remarks>
/// <para>
/// <b>Same region, same refresh rule as the boxes.</b> It borrows <see cref="CollisionBoxSet"/>'s region (the cube at
/// any yaw plus a margin) and staleness rule through a one-box set, so turning the camera never invalidates the
/// triangles and the player can walk the margin before a new gather. A triangle wholly outside the region can't touch
/// the cube, so rejecting it changes nothing that is drawn (PROJECT.md: only efficiencies with no effect on the result).
/// </para>
/// <para>
/// <b>Bounded, and honest about it.</b> The buffer is allocated once. A triangle that doesn't fit is counted in
/// <see cref="Dropped"/>; the plugin logs it and the bridge reports it, so a scene bigger than the buffer is never cut
/// off silently.
/// </para>
/// </remarks>
public sealed class TriangleSet
{
    /// <summary>
    /// Default most triangles kept. The #99 samples held 15,038 and 95,786 triangles in the nodes crossing a 40 yalm
    /// cube; this leaves room for a bigger cube. 131,072 triangles × 3 corners × 12 bytes is 4.7 MB per set.
    /// </summary>
    public const int DefaultCapacity = 131_072;

    /// <summary>The corners, three per triangle.</summary>
    private readonly Vector3[] corners;

    /// <summary>Holds the region and the refresh rule, so they are the boxes' own.</summary>
    private readonly CollisionBoxSet frame = new(1);

    /// <summary>Creates an empty set.</summary>
    /// <param name="capacity">Most triangles kept.</param>
    public TriangleSet(int capacity = DefaultCapacity)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(capacity, 1);
        this.corners = new Vector3[capacity * 3];
    }

    /// <summary>Most triangles kept.</summary>
    public int Capacity => this.corners.Length / 3;

    /// <summary>Whether a gather has started since the set was created or last <see cref="Clear">cleared</see>.</summary>
    public bool HasData => this.frame.HasData;

    /// <summary>How many triangles are kept.</summary>
    public int Count { get; private set; }

    /// <summary>How many triangles were offered in the last gather, inside the region or not.</summary>
    public int Offered { get; private set; }

    /// <summary>How many triangles inside the region didn't fit under <see cref="Capacity"/>.</summary>
    public int Dropped { get; private set; }

    /// <summary>How many offered triangles were rejected for a non-finite corner.</summary>
    public int Invalid { get; private set; }

    /// <summary>The world region triangles are gathered for (<see cref="CollisionBoxSet.Region"/>).</summary>
    public CollisionBox Region => this.frame.Region;

    /// <summary>The kept triangles' corners, three per triangle, in world coordinates.</summary>
    public ReadOnlySpan<Vector3> Corners => this.corners.AsSpan(0, this.Count * 3);

    /// <summary>Whether the triangles must be gathered again (see <see cref="CollisionBoxSet.NeedsRefresh"/>).</summary>
    /// <param name="centre">The cube's centre now.</param>
    /// <param name="size">The cube's width, height and depth now.</param>
    /// <param name="sceneStamp">What identifies the loaded collision to the reader.</param>
    public bool NeedsRefresh(Vector3 centre, Vector3 size, ulong sceneStamp) => this.frame.NeedsRefresh(centre, size, sceneStamp);

    /// <summary>Starts a new gather: empties the set and sets the region triangles are culled against.</summary>
    /// <param name="centre">The cube's centre.</param>
    /// <param name="size">The cube's width, height and depth.</param>
    /// <param name="sceneStamp">The reader's scene stamp.</param>
    public void Begin(Vector3 centre, Vector3 size, ulong sceneStamp)
    {
        this.frame.Begin(centre, size, sceneStamp);
        this.Count = 0;
        this.Offered = 0;
        this.Dropped = 0;
        this.Invalid = 0;
    }

    /// <summary>Drops every triangle (PROJECT.md: collision data is dropped on a zone change); the buffer is kept.</summary>
    public void Clear()
    {
        this.frame.Clear();
        this.Count = 0;
        this.Offered = 0;
        this.Dropped = 0;
        this.Invalid = 0;
    }

    /// <summary>Keeps the triangles but makes <see cref="NeedsRefresh"/> true until a new gather replaces them.</summary>
    public void MarkStale() => this.frame.MarkStale();

    /// <summary>
    /// Whether a world box reaches into the gathered region; the reader calls it on colliders and nodes to skip
    /// everything outside before reading their triangles (cull first).
    /// </summary>
    /// <param name="box">A world box.</param>
    public bool Overlaps(in CollisionBox box) => this.frame.Overlaps(box);

    /// <summary>
    /// Offers one triangle. One with a non-finite corner or wholly outside the region is rejected; otherwise it is
    /// kept, or counted as dropped when the set is full.
    /// </summary>
    /// <param name="a">First corner, in world coordinates.</param>
    /// <param name="b">Second corner.</param>
    /// <param name="c">Third corner.</param>
    /// <returns>True if the triangle was kept.</returns>
    public bool Offer(Vector3 a, Vector3 b, Vector3 c)
    {
        this.Offered++;
        var min = Vector3.Min(a, Vector3.Min(b, c));
        var max = Vector3.Max(a, Vector3.Max(b, c));
        if (!float.IsFinite(min.X + min.Y + min.Z + max.X + max.Y + max.Z))
        {
            this.Invalid++;
            return false;
        }

        var region = this.frame.Region;
        if (max.X < region.Min.X || min.X > region.Max.X || max.Y < region.Min.Y || min.Y > region.Max.Y ||
            max.Z < region.Min.Z || min.Z > region.Max.Z)
        {
            return false;
        }

        if (this.Count == this.Capacity)
        {
            this.Dropped++;
            return false;
        }

        var i = this.Count++ * 3;
        this.corners[i] = a;
        this.corners[i + 1] = b;
        this.corners[i + 2] = c;
        return true;
    }
}
