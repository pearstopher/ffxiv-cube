using System.Numerics;

namespace Cube.Core;

/// <summary>
/// Where <see cref="TriangleCache"/> gets its triangles from: in the plugin, the game's collision scene
/// (<c>Cube.Game.MeshReader</c>); in the tests, a list of fake colliders. The Mesh-style twin of
/// <see cref="ICollisionSource"/>, with the same rules: no game pointer is kept between frames, so each frame starts
/// with <see cref="Seek"/>; and it is a struct generic argument so nothing is boxed.
/// </summary>
public interface ITriangleSource
{
    /// <summary>The source's clock, in the ticks the cache's budget is given in.</summary>
    long Now { get; }

    /// <summary>Moves to the collider at <paramref name="index"/> in the scene's list.</summary>
    /// <param name="index">How many colliders to skip from the start of the list.</param>
    /// <returns>False if the list has fewer colliders than that.</returns>
    bool Seek(int index);

    /// <summary>Offers every triangle of the current collider that reaches the set's region, then moves to the next collider.</summary>
    /// <param name="set">The set being gathered.</param>
    /// <returns>False if there was no current collider: the walk is complete.</returns>
    bool OfferNext(TriangleSet set);
}

/// <summary>
/// Gathers the collision triangles around the cube a few colliders per frame within a time budget, and keeps the last
/// complete set for clipping and drawing (#100; PROJECT.md, "Collision style" and "Efficiency and memory"): the
/// Mesh-style counterpart of <see cref="CollisionCache"/>.
/// </summary>
/// <remarks>
/// <para>
/// Two sets are swapped like the boxes': a gather fills one while <see cref="Ready"/> keeps the last complete one, so a
/// half-gathered area never shows as empty. The buffers (4.7 MB per set) are allocated on first use and dropped by
/// <see cref="Release"/> when the user goes back to Boxes, so memory is only spent while Mesh is in use. A gather in
/// progress restarts if the scene changes under it or the player walks out of its region; the zone change drops both
/// sets (PROJECT.md, "Bounded memory").
/// </para>
/// <para>
/// A full #99 rebuild read 96k triangles in about 8.5 ms, so the walk stops between colliders when the frame's budget is
/// spent (always after at least one collider, so a gather always finishes).
/// </para>
/// </remarks>
public sealed class TriangleCache
{
    /// <summary>
    /// Default time a frame may spend gathering, in milliseconds: a bit more than the boxes' because reading triangles
    /// is the bulk of Mesh mode's work and it is an opt-in, slower mode. The rest of the gather waits for the next frame.
    /// </summary>
    public const double DefaultBudgetMilliseconds = 1.0;

    /// <summary>The longest a frame's gather may run, in the source's clock ticks.</summary>
    private readonly long budgetTicks;

    /// <summary>Most triangles each set keeps.</summary>
    private readonly int capacity;

    /// <summary>The set being filled; null until first use and after <see cref="Release"/>.</summary>
    private TriangleSet? building;

    /// <summary>The zone the sets belong to.</summary>
    private uint zone;

    /// <summary>Whether <see cref="zone"/> has been set.</summary>
    private bool hasZone;

    /// <summary>The index in the scene's collider list the next frame's gather starts at.</summary>
    private int nextCollider;

    /// <summary>Creates an empty cache; no buffer is allocated until the first <see cref="Step{TSource}"/>.</summary>
    /// <param name="budgetTicks">The longest a frame may spend gathering, in the source's clock ticks.</param>
    /// <param name="capacity">Most triangles each set keeps.</param>
    public TriangleCache(long budgetTicks, int capacity = TriangleSet.DefaultCapacity)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(budgetTicks, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(capacity, 1);
        this.budgetTicks = budgetTicks;
        this.capacity = capacity;
    }

    /// <summary>The last complete gather; null until the first <see cref="Step{TSource}"/> allocates it.</summary>
    public TriangleSet? Ready { get; private set; }

    /// <summary>Whether a gather is in progress.</summary>
    public bool IsGathering { get; private set; }

    /// <summary>How many gathers have completed.</summary>
    public long Completed { get; private set; }

    /// <summary>How many gathers were abandoned and started again.</summary>
    public long Restarts { get; private set; }

    /// <summary>How many times the zone changed after the first step.</summary>
    public long ZoneChanges { get; private set; }

    /// <summary>The most triangles any completed gather found that didn't fit, so the plugin can say a scene was cut off.</summary>
    public int LargestDropped { get; private set; }

    /// <summary>Whether the buffers are allocated.</summary>
    public bool IsAllocated => this.Ready is not null;

    /// <summary>Drops both sets' data and any gather in progress; the buffers are kept.</summary>
    public void Clear()
    {
        this.Ready?.Clear();
        this.building?.Clear();
        this.IsGathering = false;
        this.nextCollider = 0;
    }

    /// <summary>Drops the buffers too, so Boxes mode holds no Mesh memory.</summary>
    public void Release()
    {
        this.Ready = null;
        this.building = null;
        this.IsGathering = false;
        this.nextCollider = 0;
    }

    /// <summary>Marks the ready set stale so the next step gathers again while the old triangles stay drawn.</summary>
    public void Invalidate() => this.Ready?.MarkStale();

    /// <summary>
    /// One frame's work: drops everything on a zone change, starts a gather if the ready set is stale, and continues
    /// the gather in progress until the budget is spent or the scene's list ends.
    /// </summary>
    /// <typeparam name="TSource">The triangle source; a struct, so the calls aren't boxed.</typeparam>
    /// <param name="source">The source; <see cref="ITriangleSource.Seek"/> is called first.</param>
    /// <param name="centre">The cube's centre.</param>
    /// <param name="size">The cube's width, height and depth in yalms.</param>
    /// <param name="zone">The current zone id.</param>
    /// <param name="sceneStamp">The source's scene stamp.</param>
    /// <returns>True if a gather completed this frame, so <see cref="Ready"/> changed.</returns>
    public bool Step<TSource>(ref TSource source, Vector3 centre, Vector3 size, uint zone, ulong sceneStamp)
        where TSource : struct, ITriangleSource
    {
        if (this.Ready is null || this.building is null)
        {
            this.Ready = new TriangleSet(this.capacity);
            this.building = new TriangleSet(this.capacity);
        }

        if (!this.hasZone || zone != this.zone)
        {
            if (this.hasZone)
            {
                this.ZoneChanges++;
            }

            this.Clear();
            this.zone = zone;
            this.hasZone = true;
        }

        if (this.IsGathering)
        {
            if (this.building.NeedsRefresh(centre, size, sceneStamp))
            {
                this.Restarts++;
                this.Start(centre, size, sceneStamp);
            }
        }
        else if (this.Ready.NeedsRefresh(centre, size, sceneStamp))
        {
            this.Start(centre, size, sceneStamp);
        }
        else
        {
            return false;
        }

        var started = source.Now;
        if (!source.Seek(this.nextCollider))
        {
            return this.Finish();
        }

        do
        {
            if (!source.OfferNext(this.building))
            {
                return this.Finish();
            }

            this.nextCollider++;
        }
        while (source.Now - started < this.budgetTicks);

        return false;
    }

    /// <summary>Begins a new gather at the start of the scene's list.</summary>
    /// <param name="centre">The cube's centre.</param>
    /// <param name="size">The cube's size.</param>
    /// <param name="sceneStamp">The scene stamp.</param>
    private void Start(Vector3 centre, Vector3 size, ulong sceneStamp)
    {
        this.building!.Begin(centre, size, sceneStamp);
        this.nextCollider = 0;
        this.IsGathering = true;
    }

    /// <summary>Completes the gather: the filled set becomes <see cref="Ready"/> and the old one is reused.</summary>
    /// <returns>Always true.</returns>
    private bool Finish()
    {
        (this.Ready, this.building) = (this.building, this.Ready);
        this.IsGathering = false;
        this.nextCollider = 0;
        this.Completed++;
        this.LargestDropped = Math.Max(this.LargestDropped, this.Ready!.Dropped);
        return true;
    }
}
