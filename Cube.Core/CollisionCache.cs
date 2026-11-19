using System.Numerics;

namespace Cube.Core;

/// <summary>
/// Gathers the collision boxes around the cube a few colliders per frame, within a time budget, and keeps the last
/// complete set for drawing (PROJECT.md, "Collision (phase 2)" and "Efficiency and memory").
/// </summary>
/// <remarks>
/// <para>
/// <b>Two sets, swapped.</b> A gather fills <see cref="building"/> while <see cref="Ready"/> keeps the last complete
/// one, so the cube never shows a half-gathered area as if it were empty; when the walk reaches the end of the scene's
/// list the two swap. Both are allocated once at the same capacity, so memory is fixed (2 × 64 KB by default) and
/// nothing grows with time in game.
/// </para>
/// <para>
/// <b>Spread over frames.</b> Each <see cref="Step{TSource}"/> offers colliders until the frame's budget is used
/// (always at least one, so a gather always finishes), then stops and remembers only the index of the next collider.
/// The #28 sample walked a whole house interior in 0.13 ms, so a gather usually completes in one frame; the budget is
/// there for busier places (PROJECT.md: "any expensive gathering is spread over frames with a time budget").
/// </para>
/// <para>
/// <b>When to gather.</b> Only when <see cref="CollisionBoxSet.NeedsRefresh"/> says the ready set is stale (player
/// moved a quarter of the box's extent, size or scene changed). A gather in progress restarts if the scene changes under it or
/// the player has already walked out of its region, so a finished set always matches one scene and one place.
/// </para>
/// <para>
/// <b>Zone change.</b> Both sets are dropped as soon as the zone differs (PROJECT.md, "Bounded memory": collision data
/// is dropped when you change zone), so the new zone shows nothing rather than the old zone's walls until its first
/// gather completes ("areas not yet processed are simply empty").
/// </para>
/// </remarks>
public sealed class CollisionCache
{
    /// <summary>
    /// Default time a frame may spend gathering, in milliseconds. A whole house interior took 0.13 ms in #28, so most
    /// gathers finish in one frame; outdoors and in cities they spread over a few. Kept well under the 0.5 ms draw
    /// budget so a gathering frame stays smooth.
    /// </summary>
    public const double DefaultBudgetMilliseconds = 0.2;

    /// <summary>The set being filled by the current gather; swapped with <see cref="Ready"/> when it completes.</summary>
    private CollisionBoxSet building;

    /// <summary>The longest a frame's gather may run, in the source's clock ticks.</summary>
    private readonly long budgetTicks;

    /// <summary>The zone the sets belong to; a different one drops them.</summary>
    private uint zone;

    /// <summary>Whether <see cref="zone"/> has been set by a first <see cref="Step{TSource}"/>.</summary>
    private bool hasZone;

    /// <summary>The index in the scene's collider list the next frame's gather starts at.</summary>
    private int nextCollider;

    /// <summary>Creates an empty cache.</summary>
    /// <param name="budgetTicks">The longest a frame may spend gathering, in the source's clock ticks.</param>
    /// <param name="capacity">Most boxes each set keeps (<see cref="CollisionBoxSet.DefaultCapacity"/>).</param>
    public CollisionCache(long budgetTicks, int capacity = CollisionBoxSet.DefaultCapacity)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(budgetTicks, 1);
        this.budgetTicks = budgetTicks;
        this.Ready = new CollisionBoxSet(capacity);
        this.building = new CollisionBoxSet(capacity);
    }

    /// <summary>
    /// The last complete gather, to be clipped and drawn. Empty (<see cref="CollisionBoxSet.HasData"/> false) until
    /// the first gather in a zone completes.
    /// </summary>
    public CollisionBoxSet Ready { get; private set; }

    /// <summary>Whether a gather is in progress (started and not yet at the end of the scene's list).</summary>
    public bool IsGathering { get; private set; }

    /// <summary>How many gathers have completed since the cache was created; checks compare it before and after.</summary>
    public long Completed { get; private set; }

    /// <summary>How many gathers were abandoned and started again because the scene or the player moved on.</summary>
    public long Restarts { get; private set; }

    /// <summary>
    /// How many times the zone changed after the first <see cref="Step{TSource}"/>, each dropping both sets. The test
    /// bridge reports it to show memory stays bounded across zone changes (#33).
    /// </summary>
    public long ZoneChanges { get; private set; }

    /// <summary>Frames the current or last gather has taken so far.</summary>
    public int GatherFrames { get; private set; }

    /// <summary>Colliders the last frame offered.</summary>
    public int LastStepColliders { get; private set; }

    /// <summary>
    /// Drops both sets and any gather in progress (PROJECT.md: collision data is dropped on a zone change). The buffers
    /// are kept for reuse.
    /// </summary>
    public void Clear()
    {
        this.Ready.Clear();
        this.building.Clear();
        this.IsGathering = false;
        this.nextCollider = 0;
        this.GatherFrames = 0;
    }

    /// <summary>
    /// Marks the ready set as stale without dropping it, so the next <see cref="Step{TSource}"/> gathers again while
    /// the old boxes stay drawn. The test bridge uses it to measure gathers on demand.
    /// </summary>
    public void Invalidate() => this.Ready.MarkStale();

    /// <summary>
    /// One frame's work: drops everything on a zone change, starts a gather if the ready set is stale, and continues the
    /// gather in progress until the budget is spent or the scene's list ends.
    /// </summary>
    /// <typeparam name="TSource">The collision source; a struct, so the calls aren't boxed.</typeparam>
    /// <param name="source">The source, positioned anywhere; <see cref="ICollisionSource.Seek"/> is called first.</param>
    /// <param name="position">The cube's centre: the player, shifted vertically by the player height setting (#91).</param>
    /// <param name="size">The cube's width, height and depth in yalms.</param>
    /// <param name="zone">The current zone (territory) id.</param>
    /// <param name="sceneStamp">The source's scene stamp (see <see cref="CollisionBoxSet.NeedsRefresh"/>).</param>
    /// <returns>True if a gather completed this frame, so <see cref="Ready"/> changed.</returns>
    public bool Step<TSource>(ref TSource source, Vector3 position, Vector3 size, uint zone, ulong sceneStamp)
        where TSource : struct, ICollisionSource
    {
        this.LastStepColliders = 0;
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
            if (this.building.NeedsRefresh(position, size, sceneStamp))
            {
                // The scene changed under the gather, or the player already left its region: what's gathered so far
                // would be a mix, so start over here.
                this.Restarts++;
                this.Start(position, size, sceneStamp);
            }
        }
        else if (this.Ready.NeedsRefresh(position, size, sceneStamp))
        {
            this.Start(position, size, sceneStamp);
        }
        else
        {
            return false;
        }

        this.GatherFrames++;
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
            this.LastStepColliders++;
        }
        while (source.Now - started < this.budgetTicks);

        return false;
    }

    /// <summary>Begins a new gather into <see cref="building"/> at the start of the scene's list.</summary>
    /// <param name="position">The cube's centre.</param>
    /// <param name="size">The cube's width, height and depth.</param>
    /// <param name="sceneStamp">The scene stamp.</param>
    private void Start(Vector3 position, Vector3 size, ulong sceneStamp)
    {
        this.building.Begin(position, size, sceneStamp);
        this.nextCollider = 0;
        this.GatherFrames = 0;
        this.IsGathering = true;
    }

    /// <summary>Completes the gather: the filled set becomes <see cref="Ready"/> and the old one is reused next time.</summary>
    /// <returns>Always true (a gather completed), for <see cref="Step{TSource}"/> to return.</returns>
    private bool Finish()
    {
        (this.Ready, this.building) = (this.building, this.Ready);
        this.IsGathering = false;
        this.nextCollider = 0;
        this.Completed++;
        return true;
    }
}
