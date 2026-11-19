namespace Cube.Core;

/// <summary>
/// How collision is drawn (#100; PROJECT.md, "Collision style"): as simplified boxes, or as the game's own triangles.
/// </summary>
/// <remarks>
/// Boxes stays the default and the only style that keeps PROJECT.md's 0.5 ms budget. Mesh is an opt-in exception to
/// it: the scene's real triangles are drawn as they are, so its cost depends on the scene and is reported, not limited.
/// Old settings files have no style and load as <see cref="Boxes"/>.
/// </remarks>
public enum CollisionStyle
{
    /// <summary>Simplified boxes, one per collider or mesh node piece (<see cref="CollisionCache"/>). The default.</summary>
    Boxes = 0,

    /// <summary>
    /// The real collision triangles as a wireframe (<see cref="TriangleCache"/>, <see cref="ClippedMesh"/>). Slower:
    /// nothing is simplified, so busy places cost more than the boxes' budget.
    /// </summary>
    Mesh = 1,
}
