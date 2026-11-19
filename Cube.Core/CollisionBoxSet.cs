using System.Numerics;

namespace Cube.Core;

/// <summary>
/// The cached collision boxes around the cube (PROJECT.md, "Collision (phase 2)" and "Efficiency and memory"): the
/// game reader walks the client's collision scene and offers every box it finds; this class culls them to the area
/// around the cube, merges the cheap cases, holds at most a fixed number, and says when the list is stale.
/// </summary>
/// <remarks>
/// <para>
/// <b>Gathered once, used for many frames.</b> The walk is cheap (0.13 ms in the #28 sample) but still not something
/// to repeat every frame, so the boxes are gathered for a <see cref="Region"/> a little larger than the cube: wide
/// enough for any camera yaw (the box's horizontal diagonal) plus a margin of <see cref="RefreshFraction"/> of its
/// extents (sideways the larger of width and depth, vertically the height). Turning the camera never invalidates it; the player can walk that margin before <see cref="NeedsRefresh"/>
/// asks for a new walk (PROJECT.md: "When the player moves, stale data is refreshed"; "recompute only when inputs
/// change beyond a small threshold"). Each frame <see cref="CollisionClipper"/> clips the cached boxes to the cube.
/// </para>
/// <para>
/// <b>Bounded.</b> The buffer is allocated once at <see cref="Capacity"/> boxes (2,000 × 32 bytes = 64 KB by
/// default, the #28 recommendation) and reused; a full set drops extras and counts them in <see cref="Dropped"/>,
/// like the player cap. <see cref="Clear"/> drops the data on a zone change (PROJECT.md: collision data is dropped
/// when you leave the area or change zone).
/// </para>
/// <para>
/// <b>Merging where cheap.</b> The reader offers the nodes of one mesh tree one after another, and neighbouring nodes
/// often line up. Each new box is compared with the last one kept only (constant time): if one holds the other, or
/// they share their extent on two axes and meet on the third, the two become one box. Anything smarter would cost a
/// search per box and isn't needed to stay under the cap.
/// </para>
/// </remarks>
public sealed class CollisionBoxSet
{
    /// <summary>Default most boxes kept (<c>docs/research-notes.md</c>, #28: 431 in a house interior; room for busier places).</summary>
    public const int DefaultCapacity = 2000;

    /// <summary>
    /// How far the player may move, as a fraction of the cube's extent along that direction (sideways the larger of
    /// width and depth, vertically the height), before the boxes are gathered again. Also the margin the gathered
    /// region has around the cube, so nothing at the cube's edge is missing until then.
    /// </summary>
    public const float RefreshFraction = 0.25f;

    /// <summary>How close two box faces must be, in yalms, to count as lined up when merging.</summary>
    public const float MergeTolerance = 0.01f;

    /// <summary>
    /// Stretches the horizontal half-diagonal by a hair, so rounding never leaves a corner of the cube just outside
    /// the gathered region.
    /// </summary>
    private const float RoundUp = 1.00001f;

    /// <summary>The reused buffer of kept boxes.</summary>
    private readonly CollisionBox[] boxes;

    /// <summary>The scene stamp the boxes were gathered for (see <see cref="Begin"/>).</summary>
    private ulong stamp;

    /// <summary>The cube centre the boxes were gathered around.</summary>
    private Vector3 centre;

    /// <summary>The cube size the boxes were gathered for, already clamped.</summary>
    private Vector3 size;

    /// <summary>Set by <see cref="MarkStale"/>: the boxes are kept but <see cref="NeedsRefresh"/> asks for a new gather.</summary>
    private bool stale;

    /// <summary>Creates an empty set.</summary>
    /// <param name="capacity">Most boxes kept.</param>
    public CollisionBoxSet(int capacity = DefaultCapacity)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(capacity, 1);
        this.boxes = new CollisionBox[capacity];
    }

    /// <summary>Most boxes kept.</summary>
    public int Capacity => this.boxes.Length;

    /// <summary>Whether boxes have been gathered since the set was created or last <see cref="Clear">cleared</see>.</summary>
    public bool HasData { get; private set; }

    /// <summary>How many boxes are kept.</summary>
    public int Count { get; private set; }

    /// <summary>How many boxes were offered in the last gather, inside the region or not.</summary>
    public int Offered { get; private set; }

    /// <summary>How many offered boxes were folded into the box before them instead of being kept on their own.</summary>
    public int Merged { get; private set; }

    /// <summary>How many boxes inside the region didn't fit under <see cref="Capacity"/>.</summary>
    public int Dropped { get; private set; }

    /// <summary>How many offered boxes were rejected as invalid (non-finite or inside out).</summary>
    public int Invalid { get; private set; }

    /// <summary>
    /// The world region boxes are gathered for: the cube at any yaw plus the refresh margin. The reader tests collider
    /// and node boxes against it (<see cref="Overlaps"/>) to skip whole subtrees, the cull-first rule from #28.
    /// </summary>
    public CollisionBox Region { get; private set; }

    /// <summary>The kept boxes, unclipped, in world coordinates.</summary>
    public ReadOnlySpan<CollisionBox> Items => this.boxes.AsSpan(0, this.Count);

    /// <summary>
    /// The world region gathered for a cube, before any yaw: centre ± (horizontal half-diagonal + margin) sideways and
    /// ± (half the height + margin) vertically. The box can turn to any yaw, so sideways both world axes get the same
    /// reach: the furthest a corner of its footprint gets from the centre.
    /// </summary>
    /// <param name="centre">The cube's centre: the player's position, shifted vertically by the player height setting (#91).</param>
    /// <param name="size">The cube's width, height and depth in yalms; clamped like <see cref="CubeSpace"/>.</param>
    public static CollisionBox RegionFor(Vector3 centre, Vector3 size)
    {
        size = BoxExtents.Clamp(size);
        var margin = Margin(size);
        var diagonal = MathF.Sqrt((size.X * size.X) + (size.Z * size.Z)) / 2f * RoundUp;
        var reach = new Vector3(diagonal + margin.X, (size.Y / 2f) + margin.Y, diagonal + margin.X);
        return new CollisionBox(centre - reach, centre + reach);
    }

    /// <summary>
    /// The refresh margin for a size: <see cref="RefreshFraction"/> of the larger of width and depth sideways (in
    /// <see cref="Vector2.X"/>), and of the height vertically (in <see cref="Vector2.Y"/>). For a cube both are a
    /// quarter of the edge, as before #90.
    /// </summary>
    /// <param name="size">A clamped size.</param>
    private static Vector2 Margin(Vector3 size) =>
        new(MathF.Max(size.X, size.Z) * RefreshFraction, size.Y * RefreshFraction);

    /// <summary>
    /// Whether the boxes must be gathered again: there are none yet, the scene changed (a different
    /// <paramref name="sceneStamp"/>), the cube's size changed, or the player has moved further than the refresh
    /// margin since the last gather (sideways or vertically, see <see cref="RefreshFraction"/>).
    /// </summary>
    /// <param name="playerPosition">The cube's centre now.</param>
    /// <param name="size">The cube's width, height and depth now.</param>
    /// <param name="sceneStamp">
    /// Whatever identifies the loaded collision to the reader (in the plugin, the first collider's address and the
    /// collider count, as #28 suggests, with the detail level folded in by <see cref="CollisionDetails.Stamp"/>). Core
    /// only compares it; zone changes are handled separately by <see cref="CollisionCache"/>.
    /// </param>
    public bool NeedsRefresh(Vector3 playerPosition, Vector3 size, ulong sceneStamp)
    {
        size = BoxExtents.Clamp(size);
        if (!this.HasData || this.stale || sceneStamp != this.stamp || size != this.size)
        {
            return true;
        }

        var moved = Vector3.Abs(playerPosition - this.centre);
        var limit = Margin(size);
        return moved.X > limit.X || moved.Y > limit.Y || moved.Z > limit.X;
    }

    /// <summary>Starts a new gather: empties the set and sets the region boxes are culled against.</summary>
    /// <param name="playerPosition">The cube's centre.</param>
    /// <param name="size">The cube's width, height and depth in yalms.</param>
    /// <param name="sceneStamp">The reader's scene stamp, remembered for <see cref="NeedsRefresh"/>.</param>
    public void Begin(Vector3 playerPosition, Vector3 size, ulong sceneStamp)
    {
        this.centre = playerPosition;
        this.size = BoxExtents.Clamp(size);
        this.stamp = sceneStamp;
        this.Region = RegionFor(playerPosition, this.size);
        this.HasData = true;
        this.stale = false;
        this.Count = 0;
        this.Offered = 0;
        this.Merged = 0;
        this.Dropped = 0;
        this.Invalid = 0;
    }

    /// <summary>
    /// Drops every box (PROJECT.md, "Bounded memory": collision data is dropped when you leave the area or change
    /// zone). The buffer is kept for reuse, so the next gather allocates nothing.
    /// </summary>
    public void Clear()
    {
        this.HasData = false;
        this.Count = 0;
        this.Offered = 0;
        this.Merged = 0;
        this.Dropped = 0;
        this.Invalid = 0;
    }

    /// <summary>
    /// Keeps the boxes but makes <see cref="NeedsRefresh"/> true, so they stay drawn until a new gather replaces them
    /// (<see cref="CollisionCache.Invalidate"/>).
    /// </summary>
    public void MarkStale() => this.stale = true;

    /// <summary>
    /// Whether a box reaches into the gathered region. The reader calls it on collider and node boxes to skip
    /// everything outside before reading further (PROJECT.md, "Efficiency": cull first).
    /// </summary>
    /// <param name="box">A world box.</param>
    public bool Overlaps(in CollisionBox box) => this.Region.Intersects(box);

    /// <summary>
    /// Offers one box. Invalid boxes and boxes outside the region are rejected; a box that lines up with the last one
    /// kept is merged into it; otherwise it is kept, or counted as dropped if the set is full.
    /// </summary>
    /// <param name="box">A world box from the collision scene.</param>
    /// <returns>True if the box is now covered by the set (kept or merged).</returns>
    public bool Offer(in CollisionBox box)
    {
        this.Offered++;
        if (!box.IsValid)
        {
            this.Invalid++;
            return false;
        }

        if (!this.Region.Intersects(box))
        {
            return false;
        }

        if (this.Count > 0 && TryMerge(this.boxes[this.Count - 1], box, out var merged))
        {
            this.boxes[this.Count - 1] = merged;
            this.Merged++;
            return true;
        }

        if (this.Count == this.boxes.Length)
        {
            this.Dropped++;
            return false;
        }

        this.boxes[this.Count++] = box;
        return true;
    }

    /// <summary>
    /// The cheap merge: two boxes become one when one holds the other, or when they have the same extent on two axes
    /// (within <see cref="MergeTolerance"/>) and overlap or touch on the third, so their union covers no extra space.
    /// </summary>
    /// <param name="a">The box kept last.</param>
    /// <param name="b">The new box.</param>
    /// <param name="merged">The single box covering both, if they merge.</param>
    /// <returns>True if they merge.</returns>
    public static bool TryMerge(in CollisionBox a, in CollisionBox b, out CollisionBox merged)
    {
        if (a.IsTurned || b.IsTurned)
        {
            // The merge tests compare world-axis boxes, which is only the box itself when it isn't turned. A turned
            // box is its own object (a fence post, a diagonal wall) and merging it would grow its footprint.
            merged = default;
            return false;
        }

        if (a.Contains(b))
        {
            merged = a;
            return true;
        }

        if (b.Contains(a))
        {
            merged = b;
            return true;
        }

        var sameX = Same(a.Min.X, b.Min.X) && Same(a.Max.X, b.Max.X);
        var sameY = Same(a.Min.Y, b.Min.Y) && Same(a.Max.Y, b.Max.Y);
        var sameZ = Same(a.Min.Z, b.Min.Z) && Same(a.Max.Z, b.Max.Z);
        var lined =
            (sameY && sameZ && Meets(a.Min.X, a.Max.X, b.Min.X, b.Max.X)) ||
            (sameX && sameZ && Meets(a.Min.Y, a.Max.Y, b.Min.Y, b.Max.Y)) ||
            (sameX && sameY && Meets(a.Min.Z, a.Max.Z, b.Min.Z, b.Max.Z));
        merged = lined ? a.Union(b) : default;
        return lined;
    }

    /// <summary>Whether two face positions are the same within <see cref="MergeTolerance"/>.</summary>
    /// <param name="a">One position.</param>
    /// <param name="b">The other.</param>
    private static bool Same(float a, float b) => MathF.Abs(a - b) <= MergeTolerance;

    /// <summary>Whether two intervals overlap or touch, within <see cref="MergeTolerance"/>.</summary>
    /// <param name="aMin">First interval's start.</param>
    /// <param name="aMax">First interval's end.</param>
    /// <param name="bMin">Second interval's start.</param>
    /// <param name="bMax">Second interval's end.</param>
    private static bool Meets(float aMin, float aMax, float bMin, float bMax) =>
        aMin <= bMax + MergeTolerance && bMin <= aMax + MergeTolerance;
}
