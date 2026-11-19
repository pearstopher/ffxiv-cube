using System.Numerics;

namespace Cube.Core;

/// <summary>
/// One piece of collision as a simplified box (PROJECT.md, "Collision (phase 2)": each collider inside or crossing the
/// cube is shown as a simplified box, not the game's model). Upright, but turned about the vertical axis to fit its
/// object when the game's matrix says so (#123).
/// </summary>
/// <remarks>
/// <para>
/// The research in #28 (<c>docs/research-notes.md</c>, "Where to get collision data") picked one box per collision
/// mesh node: the node's local bounds taken through its collider's world matrix (<see cref="FromTransformed"/>), and
/// one box per primitive collider (a unit shape placed by its matrix). Those follow walls and floors closely enough to
/// navigate by, cost one memory read each, and need no triangles.
/// </para>
/// <para>
/// The box is stored as a centre, a half-size along its own axes and the turn as a (cos, sin) pair (32 bytes, so the
/// bounded cache in <see cref="CollisionBoxSet"/> stays small). Wrapping a turned object in a world-axis box made a
/// thin diagonal fence about 15-20 times too big (#110, <c>docs/research-notes.md</c>), so a box turned by a pure yaw
/// keeps its turn. <see cref="Min"/> and <see cref="Max"/> are still the world-axis box around it: culling, merging and
/// the cheap rejects all use that, so they are no worse than before. A matrix that tilts or shears falls back to that
/// axis box (<see cref="FromTransformed"/>). The turn is independent of the camera, so the cached list never has to be
/// rebuilt for a yaw change.
/// </para>
/// </remarks>
public readonly struct CollisionBox
{
    /// <summary>How far from a quarter turn a yaw may be and still count as one (sine or cosine rounding).</summary>
    private const float SnapTolerance = 1e-4f;

    /// <summary>How far a matrix may be from a pure turn (relative, per axis) before it counts as tilted or sheared.</summary>
    private const float PureTurnTolerance = 1e-3f;

    /// <summary>The box's centre in world coordinates.</summary>
    private readonly Vector3 centre;

    /// <summary>Half the size along the box's own axes.</summary>
    private readonly Vector3 half;

    /// <summary>Creates a world-axis box from its lowest and highest corners.</summary>
    /// <param name="min">The corner with the smallest X, Y and Z.</param>
    /// <param name="max">The corner with the largest X, Y and Z.</param>
    public CollisionBox(Vector3 min, Vector3 max)
    {
        this.centre = (min + max) * 0.5f;
        this.half = (max - min) * 0.5f;
        this.Cos = 1f;
        this.Sin = 0f;
    }

    /// <summary>Creates a box turned about the vertical axis.</summary>
    /// <param name="centre">The box's centre in world coordinates.</param>
    /// <param name="half">Half the size along the box's own X, Y and Z (its Z is the world Z when not turned).</param>
    /// <param name="cos">Cosine of the turn: the world X and Z of the box's own X axis are (cos, sin).</param>
    /// <param name="sin">Sine of the turn.</param>
    public CollisionBox(Vector3 centre, Vector3 half, float cos, float sin)
    {
        this.centre = centre;
        this.half = half;
        this.Cos = cos;
        this.Sin = sin;
    }

    /// <summary>Cosine of the turn about the vertical axis (1 when not turned).</summary>
    public float Cos { get; }

    /// <summary>Sine of the turn (0 when not turned; exactly 0 for a box that lines up with the world axes).</summary>
    public float Sin { get; }

    /// <summary>Whether the box is turned off the world axes, so its footprint is not its world-axis box.</summary>
    public bool IsTurned => this.Sin != 0f;

    /// <summary>The box's centre in world coordinates.</summary>
    public Vector3 Centre => this.centre;

    /// <summary>Half the box's size along its own axes (the world axes when not turned).</summary>
    public Vector3 OwnHalfSize => this.half;

    /// <summary>
    /// Half the size of the world-axis box around this one (the same as <see cref="OwnHalfSize"/> when not turned).
    /// </summary>
    public Vector3 HalfSize => this.IsTurned
        ? new Vector3(
            (MathF.Abs(this.Cos) * this.half.X) + (MathF.Abs(this.Sin) * this.half.Z),
            this.half.Y,
            (MathF.Abs(this.Sin) * this.half.X) + (MathF.Abs(this.Cos) * this.half.Z))
        : this.half;

    /// <summary>The corner of the world-axis box around this one with the smallest X, Y and Z.</summary>
    public Vector3 Min => this.centre - this.HalfSize;

    /// <summary>The corner of the world-axis box around this one with the largest X, Y and Z.</summary>
    public Vector3 Max => this.centre + this.HalfSize;

    /// <summary>
    /// Whether the box is usable: every number finite and no negative size. A zero thickness is allowed: a flat floor
    /// or a thin wall is still something you bump into.
    /// </summary>
    /// <remarks>
    /// Memory read from the game can be mid-update or garbage; an invalid box is dropped rather than drawn as a
    /// screen-filling shape (CLAUDE.md: never let bad game data become an exception or a broken frame).
    /// </remarks>
    public bool IsValid =>
        float.IsFinite(this.centre.X) && float.IsFinite(this.centre.Y) && float.IsFinite(this.centre.Z) &&
        float.IsFinite(this.half.X) && float.IsFinite(this.half.Y) && float.IsFinite(this.half.Z) &&
        float.IsFinite(this.Cos) && float.IsFinite(this.Sin) &&
        this.half.X >= 0 && this.half.Y >= 0 && this.half.Z >= 0;

    /// <summary>
    /// The box a local box becomes when placed by a world matrix: what a collision mesh node's local bounds or a
    /// primitive collider's unit shape becomes in the world (<c>docs/research-notes.md</c>, #28 and #110).
    /// </summary>
    /// <remarks>
    /// Uses the <see cref="Vector3.Transform(Vector3, Matrix4x4)"/> convention (row vectors, translation in the
    /// fourth row), which is how the game's matrices are laid out. A matrix that is a pure turn about the vertical axis
    /// (with any scale, mirrors included, since a box is its own mirror image) gives a turned box with the scale folded
    /// into its half-size. Any other matrix (tilted or sheared) gives the world-axis box around the transformed
    /// corners: the local centre is transformed and each world half-extent is the sum of the local half-extents
    /// weighted by the absolute matrix entries (Arvo's method), exact for the corners without looping over all eight.
    /// </remarks>
    /// <param name="localMin">The local box's lowest corner.</param>
    /// <param name="localMax">The local box's highest corner.</param>
    /// <param name="world">The local-to-world matrix.</param>
    public static CollisionBox FromTransformed(Vector3 localMin, Vector3 localMax, in Matrix4x4 world)
    {
        var centre = Vector3.Transform((localMin + localMax) * 0.5f, world);
        var half = (localMax - localMin) * 0.5f;
        if (TryYaw(world, out var cos, out var sin, out var scale))
        {
            var own = new Vector3(half.X * scale.X, half.Y * scale.Y, half.Z * scale.Z);
            if (MathF.Abs(sin) <= SnapTolerance)
            {
                return new CollisionBox(centre, own, 1f, 0f);
            }

            if (MathF.Abs(cos) <= SnapTolerance)
            {
                // A quarter turn lines up with the world axes again, with the two sides swapped.
                return new CollisionBox(centre, new Vector3(own.Z, own.Y, own.X), 1f, 0f);
            }

            // (cos, sin) and (-cos, -sin) are the same box; keep cos positive so equal boxes compare equal.
            return cos < 0
                ? new CollisionBox(centre, own, -cos, -sin)
                : new CollisionBox(centre, own, cos, sin);
        }

        var extent = new Vector3(
            (MathF.Abs(world.M11) * half.X) + (MathF.Abs(world.M21) * half.Y) + (MathF.Abs(world.M31) * half.Z),
            (MathF.Abs(world.M12) * half.X) + (MathF.Abs(world.M22) * half.Y) + (MathF.Abs(world.M32) * half.Z),
            (MathF.Abs(world.M13) * half.X) + (MathF.Abs(world.M23) * half.Y) + (MathF.Abs(world.M33) * half.Z));
        return new CollisionBox(centre - extent, centre + extent);
    }

    /// <summary>
    /// Reads the turn out of a matrix if it is a pure turn about the vertical axis, possibly scaled and mirrored.
    /// </summary>
    /// <remarks>
    /// The matrix's rows are the images of the local X, Y and Z axes. A pure yaw keeps Y vertical (row 2 has only an
    /// M22), keeps X and Z horizontal (M12 and M32 zero) and keeps them at right angles. Everything is compared
    /// relative to the axis length so a big scale doesn't loosen the test.
    /// </remarks>
    /// <param name="m">The matrix.</param>
    /// <param name="cos">Cosine of the turn of the local X axis.</param>
    /// <param name="sin">Sine of the turn of the local X axis.</param>
    /// <param name="scale">How much each local axis is stretched.</param>
    /// <returns>False for a tilted, sheared or degenerate matrix.</returns>
    private static bool TryYaw(in Matrix4x4 m, out float cos, out float sin, out Vector3 scale)
    {
        cos = 1f;
        sin = 0f;
        var lx = MathF.Sqrt((m.M11 * m.M11) + (m.M12 * m.M12) + (m.M13 * m.M13));
        var ly = MathF.Sqrt((m.M21 * m.M21) + (m.M22 * m.M22) + (m.M23 * m.M23));
        var lz = MathF.Sqrt((m.M31 * m.M31) + (m.M32 * m.M32) + (m.M33 * m.M33));
        scale = new Vector3(lx, ly, lz);
        if (!float.IsFinite(lx + ly + lz) || lx <= 0f || ly <= 0f || lz <= 0f)
        {
            return false;
        }

        var pure =
            MathF.Abs(m.M12) <= PureTurnTolerance * lx &&
            MathF.Abs(m.M32) <= PureTurnTolerance * lz &&
            MathF.Abs(m.M21) <= PureTurnTolerance * ly &&
            MathF.Abs(m.M23) <= PureTurnTolerance * ly &&
            MathF.Abs((m.M11 * m.M31) + (m.M13 * m.M33)) <= PureTurnTolerance * lx * lz;
        if (!pure)
        {
            return false;
        }

        cos = m.M11 / lx;
        sin = m.M13 / lx;
        return true;
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
