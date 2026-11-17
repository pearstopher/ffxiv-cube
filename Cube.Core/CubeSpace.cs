using System.Numerics;

namespace Cube.Core;

/// <summary>
/// The cube of space around the local player (PROJECT.md, "Exact behaviour rules", Cube): axis-aligned to
/// the world, centred on the player, with an edge of 10–100 yalms (default 40). Answers "is this point
/// inside?" and turns world positions into cube-local view coordinates.
/// </summary>
/// <remarks>
/// <para>
/// World axes follow the game (see <c>docs/research-notes.md</c>, #10): X east, Y up, Z south, so north is −Z.
/// The cube itself never rotates; only the view around it does, by <see cref="Yaw"/> (see <see cref="ViewYaw"/>).
/// </para>
/// <para>
/// View coordinates are in yalms relative to the centre: <c>X</c> to the right on screen, <c>Y</c> up,
/// <c>Z</c> away from the viewer ("forward"). With yaw 0 (north up), right is east and forward is north.
/// </para>
/// <para>
/// A small immutable struct, so it is rebuilt each frame from the current inputs at no allocation cost.
/// Points are culled (<see cref="TryToView"/>) before anything else is computed for them (PROJECT.md,
/// "Efficiency": cull before projecting).
/// </para>
/// </remarks>
public readonly struct CubeSpace
{
    /// <summary>Default cube edge in yalms (PROJECT.md).</summary>
    public const float DefaultEdge = 40f;

    /// <summary>Smallest allowed cube edge in yalms (PROJECT.md: setting 10–100).</summary>
    public const float MinEdge = 10f;

    /// <summary>Largest allowed cube edge in yalms (PROJECT.md: setting 10–100).</summary>
    public const float MaxEdge = 100f;

    /// <summary>Cosine of <see cref="Yaw"/>, cached so each point costs a few multiplies.</summary>
    private readonly float cos;

    /// <summary>Sine of <see cref="Yaw"/>.</summary>
    private readonly float sin;

    /// <summary>Creates the cube around a centre.</summary>
    /// <param name="centre">The local player's world position.</param>
    /// <param name="edge">Edge length in yalms; clamped to <see cref="MinEdge"/>–<see cref="MaxEdge"/>.</param>
    /// <param name="yaw">The view's yaw in radians (see <see cref="ViewYaw"/>): 0 looks north, increasing clockwise seen from above.</param>
    public CubeSpace(Vector3 centre, float edge, float yaw)
    {
        this.Centre = centre;
        this.Edge = ClampEdge(edge);
        this.HalfEdge = this.Edge / 2f;
        this.Yaw = yaw;
        this.cos = MathF.Cos(yaw);
        this.sin = MathF.Sin(yaw);
    }

    /// <summary>Copies a cube with a new centre and edge, keeping its yaw and rotation.</summary>
    /// <param name="source">The cube whose yaw (and cached sine and cosine) to keep.</param>
    /// <param name="centre">The new centre.</param>
    /// <param name="edge">The new edge; clamped like the main constructor.</param>
    private CubeSpace(in CubeSpace source, Vector3 centre, float edge)
    {
        this.Centre = centre;
        this.Edge = ClampEdge(edge);
        this.HalfEdge = this.Edge / 2f;
        this.Yaw = source.Yaw;
        this.cos = source.cos;
        this.sin = source.sin;
    }

    /// <summary>
    /// The same view rotation around a new centre and edge, without recomputing the sine and cosine. The window
    /// uses it every frame the yaw hasn't changed (PROJECT.md, "Efficiency and memory": recompute only when inputs
    /// change).
    /// </summary>
    /// <param name="centre">The new centre.</param>
    /// <param name="edge">The new edge in yalms.</param>
    public CubeSpace Recentred(Vector3 centre, float edge) => new(this, centre, edge);

    /// <summary>The cube's centre in world coordinates: the local player's position.</summary>
    public Vector3 Centre { get; }

    /// <summary>Edge length in yalms, already clamped.</summary>
    public float Edge { get; }

    /// <summary>Half the edge: the distance from the centre to each face.</summary>
    public float HalfEdge { get; }

    /// <summary>The view's yaw in radians: 0 looks north, increasing clockwise seen from above.</summary>
    public float Yaw { get; }

    /// <summary>
    /// Clamps an edge length to the allowed range; a non-number becomes the default, so a corrupt
    /// setting can never produce an empty or infinite cube.
    /// </summary>
    /// <param name="edge">The requested edge in yalms.</param>
    public static float ClampEdge(float edge) => float.IsNaN(edge) ? DefaultEdge : Math.Clamp(edge, MinEdge, MaxEdge);

    /// <summary>
    /// Whether a world position is inside the cube. Inclusive: a point exactly on a face, edge or corner
    /// counts as inside.
    /// </summary>
    /// <param name="world">A world position.</param>
    public bool Contains(Vector3 world)
    {
        var d = world - this.Centre;
        return MathF.Abs(d.X) <= this.HalfEdge && MathF.Abs(d.Y) <= this.HalfEdge && MathF.Abs(d.Z) <= this.HalfEdge;
    }

    /// <summary>
    /// Culls then transforms: returns false for a point outside the cube (players outside aren't shown at all,
    /// PROJECT.md "Players"), otherwise gives its view coordinates.
    /// </summary>
    /// <param name="world">A world position.</param>
    /// <param name="view">The position in view coordinates, if inside.</param>
    public bool TryToView(Vector3 world, out Vector3 view)
    {
        if (!this.Contains(world))
        {
            view = default;
            return false;
        }

        view = this.ToView(world);
        return true;
    }

    /// <summary>
    /// Turns a world position into view coordinates (right, up, forward), without culling. Used for the
    /// cube's own corners and for points already known to be inside.
    /// </summary>
    /// <param name="world">A world position.</param>
    public Vector3 ToView(Vector3 world) => this.RotateOffset(world - this.Centre);

    /// <summary>Rotates an offset from the centre (world axes) into view axes.</summary>
    /// <param name="offset">An offset in world axes.</param>
    /// <remarks>
    /// With forward = (sin yaw, −cos yaw) and right = (cos yaw, sin yaw) in world (X, Z): yaw 0 gives
    /// right = east (+X), forward = north (−Z).
    /// </remarks>
    public Vector3 RotateOffset(Vector3 offset) => new(
        (offset.X * this.cos) + (offset.Z * this.sin),
        offset.Y,
        (offset.X * this.sin) - (offset.Z * this.cos));
}
