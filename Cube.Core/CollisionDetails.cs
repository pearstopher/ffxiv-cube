namespace Cube.Core;

/// <summary>
/// The rules behind each <see cref="CollisionDetail"/> level (#32): how deep the game reader walks a mesh's node tree,
/// and how a change of level makes the collision cache gather again straight away.
/// </summary>
public static class CollisionDetails
{
    /// <summary>
    /// How many levels below the root <see cref="CollisionDetail.Medium"/> walks: 2³ = at most 8 boxes per mesh, so a
    /// terrain tile or building is split into a few blocks while small objects (shallow trees) keep their full shape.
    /// </summary>
    public const int MediumDepth = 3;

    /// <summary>
    /// The deepest tree level whose nodes are offered at a detail level; a node at this depth is offered as one box
    /// covering its whole subtree, and its children are not visited. <see cref="int.MaxValue"/> means no limit.
    /// </summary>
    /// <param name="detail">The detail level.</param>
    public static int MaxNodeDepth(CollisionDetail detail) => detail switch
    {
        CollisionDetail.Coarse => 0,
        CollisionDetail.Medium => MediumDepth,
        _ => int.MaxValue,
    };

    /// <summary>
    /// The scene stamp the cache compares (<see cref="CollisionBoxSet.NeedsRefresh"/>) with the detail level folded
    /// in. A new level is a different stamp, so the cache gathers again at once (restarting a gather in progress)
    /// while the old boxes stay drawn until it finishes: the setting applies immediately (#32).
    /// </summary>
    /// <param name="sceneStamp">What identifies the loaded collision (collider list address and count).</param>
    /// <param name="detail">The detail level.</param>
    public static ulong Stamp(ulong sceneStamp, CollisionDetail detail) => (sceneStamp * 4UL) + (uint)detail;
}
