namespace Cube.Core;

/// <summary>
/// Where <see cref="CollisionCache"/> gets its boxes from: in the plugin, the game's collision scene
/// (<c>Cube.Game.CollisionReader</c>); in the tests, a list of fake colliders.
/// </summary>
/// <remarks>
/// <para>
/// The cache works through the scene one collider at a time and may stop between any two of them when its frame's time
/// budget runs out (PROJECT.md, "Collision (phase 2)": expensive gathering is spread over frames with a time budget).
/// It only remembers the <em>index</em> of the next collider, never a game pointer, so nothing it holds between frames
/// can point at a collider the game has since unloaded: each frame starts again with <see cref="Seek"/>.
/// </para>
/// <para>
/// Used as a generic constraint on a struct (<see cref="CollisionCache.Step{TSource}"/>), so the calls are direct, not
/// interface calls on a boxed copy, and the per-frame path allocates nothing.
/// </para>
/// </remarks>
public interface ICollisionSource
{
    /// <summary>The source's clock, in the ticks the cache's budget is given in (Stopwatch ticks in the plugin).</summary>
    long Now { get; }

    /// <summary>Moves to the collider at <paramref name="index"/> in the scene's list, ready for <see cref="OfferNext"/>.</summary>
    /// <param name="index">How many colliders to skip from the start of the list.</param>
    /// <returns>False if the list has fewer colliders than that.</returns>
    bool Seek(int index);

    /// <summary>
    /// Offers every box of the current collider that reaches the set's region (<see cref="CollisionBoxSet.Overlaps"/>
    /// lets it skip whole colliders and subtrees first), then moves to the next collider.
    /// </summary>
    /// <param name="set">The set being gathered.</param>
    /// <returns>False if there was no current collider: the walk is complete.</returns>
    bool OfferNext(CollisionBoxSet set);
}
