namespace Cube.Core;

/// <summary>
/// How finely collision is drawn (#32, the collision detail setting; PROJECT.md, "Collision (phase 2)": colliders are
/// shown as simplified boxes). Each level is a depth limit on the walk down a mesh collider's node tree; primitive
/// colliders (boxes, cylinders, spheres, planes) are one box at every level.
/// </summary>
/// <remarks>
/// A mesh's node tree is a bounding tree: each node's box holds its children's (#28's sample: every raycast hit was
/// inside a node box). Stopping at a depth and keeping that node's box therefore still covers everything below it,
/// just more loosely, and the bridge's <c>collision-settings</c> check confirms it in game. Fewer, bigger boxes cost
/// less to walk, clip and draw.
/// </remarks>
public enum CollisionDetail
{
    /// <summary>
    /// The default and #28's recommendation: one box per tree node holding triangles, which follows walls and floors
    /// closely.
    /// </summary>
    Full = 0,

    /// <summary>The tree is cut off <see cref="CollisionDetails.MediumDepth"/> levels down: at most 8 boxes per mesh.</summary>
    Medium = 1,

    /// <summary>One box per collider: the mesh's root node box. Cheapest, but a building or terrain tile is one block.</summary>
    Coarse = 2,
}
