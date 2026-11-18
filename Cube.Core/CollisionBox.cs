using System.Numerics;

namespace Cube.Core;

/// <summary>
/// One piece of collision as a simplified, world-axis-aligned box (PROJECT.md, "Collision (phase 2)": each collider
/// inside or crossing the cube is shown as a simplified box, not the game's model).
/// </summary>
/// <remarks>
/// <para>
/// The research in #28 (<c>docs/research-notes.md</c>, "Where to get collision data") picked one box per collision
/// mesh node: the node's local bounds taken through its collider's world matrix (<see cref="FromTransformed"/>), and
/// one box per primitive collider (a unit shape placed by its matrix). Those follow walls and floors closely enough to
/// navigate by, cost one memory read each, and need no triangles.
/// </para>
/// <para>
/// World-axis-aligned, like the player cubes (both drawn by <see cref="BoxProjector"/>), so a box turns with the rest
/// of the cube's contents when the camera turns and the cached list never has to be rebuilt for a yaw change. Two
/// <see cref="Vector3"/>s, 24 bytes, so the bounded cache in <see cref="CollisionBoxSet"/> stays small.
/// </para>
/// </remarks>
public readonly struct CollisionBox
{
    /// <summary>Creates a box from its lowest and highest corners.</summary>
    /// <param name="min">The corner with the smallest X, Y and Z.</param>
    /// <param name="max">The corner with the largest X, Y and Z.</param>
    public CollisionBox(Vector3 min, Vector3 max)
    {
        this.Min = min;
        this.Max = max;
    }

    /// <summary>The corner with the smallest world X, Y and Z.</summary>
    public Vector3 Min { get; }

    /// <summary>The corner with the largest world X, Y and Z.</summary>
    public Vector3 Max { get; }

    /// <summary>The box's centre in world coordinates.</summary>
    public Vector3 Centre => (this.Min + this.Max) * 0.5f;

    /// <summary>Half the box's size along each world axis.</summary>
    public Vector3 HalfSize => (this.Max - this.Min) * 0.5f;

    /// <summary>
    /// Whether the box is usable: every coordinate a finite number and <see cref="Min"/> not above <see cref="Max"/>
    /// on any axis. A zero thickness is allowed: a flat floor or a thin wall is still something you bump into.
    /// </summary>
    /// <remarks>
    /// Memory read from the game can be mid-update or garbage; an invalid box is dropped rather than drawn as a
    /// screen-filling shape (CLAUDE.md: never let bad game data become an exception or a broken frame).
    /// </remarks>
    public bool IsValid =>
        float.IsFinite(this.Min.X) && float.IsFinite(this.Min.Y) && float.IsFinite(this.Min.Z) &&
        float.IsFinite(this.Max.X) && float.IsFinite(this.Max.Y) && float.IsFinite(this.Max.Z) &&
        this.Min.X <= this.Max.X && this.Min.Y <= this.Max.Y && this.Min.Z <= this.Max.Z;

    /// <summary>
    /// The world box around a local box placed by a world matrix: what a collision mesh node's local bounds or a
    /// primitive collider's unit shape becomes in the world (<c>docs/research-notes.md</c>, #28 recommendation).
    /// </summary>
    /// <remarks>
    /// Uses the <see cref="Vector3.Transform(Vector3, Matrix4x4)"/> convention (row vectors, translation in the
    /// fourth row), which is how the game's matrices are laid out. The local centre is transformed and each world
    /// half-extent is the sum of the local half-extents weighted by the absolute matrix entries (Arvo's method):
    /// exact for the corners, no loop over the eight of them.
    /// </remarks>
    /// <param name="localMin">The local box's lowest corner.</param>
    /// <param name="localMax">The local box's highest corner.</param>
    /// <param name="world">The local-to-world matrix.</param>
    public static CollisionBox FromTransformed(Vector3 localMin, Vector3 localMax, in Matrix4x4 world)
    {
        var centre = Vector3.Transform((localMin + localMax) * 0.5f, world);
        var half = (localMax - localMin) * 0.5f;
        var extent = new Vector3(
            (MathF.Abs(world.M11) * half.X) + (MathF.Abs(world.M21) * half.Y) + (MathF.Abs(world.M31) * half.Z),
            (MathF.Abs(world.M12) * half.X) + (MathF.Abs(world.M22) * half.Y) + (MathF.Abs(world.M32) * half.Z),
            (MathF.Abs(world.M13) * half.X) + (MathF.Abs(world.M23) * half.Y) + (MathF.Abs(world.M33) * half.Z));
        return new CollisionBox(centre - extent, centre + extent);
    }

    /// <summary>Whether two boxes overlap or touch (inclusive, like <see cref="CubeSpace.Contains"/>).</summary>
    /// <param name="other">The other box.</param>
    public bool Intersects(in CollisionBox other) =>
        this.Min.X <= other.Max.X && this.Max.X >= other.Min.X &&
        this.Min.Y <= other.Max.Y && this.Max.Y >= other.Min.Y &&
        this.Min.Z <= other.Max.Z && this.Max.Z >= other.Min.Z;

    /// <summary>Whether this box wholly contains another (inclusive).</summary>
    /// <param name="other">The other box.</param>
    public bool Contains(in CollisionBox other) =>
        this.Min.X <= other.Min.X && this.Min.Y <= other.Min.Y && this.Min.Z <= other.Min.Z &&
        this.Max.X >= other.Max.X && this.Max.Y >= other.Max.Y && this.Max.Z >= other.Max.Z;

    /// <summary>The smallest box holding both boxes.</summary>
    /// <param name="other">The other box.</param>
    public CollisionBox Union(in CollisionBox other) =>
        new(Vector3.Min(this.Min, other.Min), Vector3.Max(this.Max, other.Max));
}
