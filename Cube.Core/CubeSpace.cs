using System.Numerics;

namespace Cube.Core;

/// <summary>
/// The cube of space around the local player (PROJECT.md, "Exact behaviour rules", Cube): centred on the player,
/// with an edge of 10–100 yalms (default 40), turned about the vertical axis by its alignment <see cref="Yaw"/>.
/// Answers "is this point inside?" and turns world positions into cube-local view coordinates.
/// </summary>
/// <remarks>
/// <para>
/// World axes follow the game (see <c>docs/research-notes.md</c>, #10): X east, Y up, Z south, so north is −Z.
/// In Camera mode the cube is aligned to the camera's horizontal direction (its front face points where the camera
/// looks); in North up mode <see cref="Yaw"/> is 0 and it is aligned to the world (see <see cref="ViewYaw"/>).
/// </para>
/// <para>
/// Cube-local ("view") coordinates are in yalms relative to the centre: <c>X</c> towards the cube's right face,
/// <c>Y</c> up, <c>Z</c> towards its front face ("forward"). The wireframe is drawn from fixed cube-local corners, so
/// it never turns on screen (PROJECT.md: "Cube on screen never turns by itself"); when the camera turns, the yaw
/// changes and the world <i>contents</i> rotate around the player instead. Containment is tested in the same
/// coordinates, so things enter and leave at the cube's sides as the camera turns. With yaw 0 (north up), right is
/// east and forward is north.
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
    /// <param name="yaw">The cube's alignment yaw in radians (see <see cref="ViewYaw"/>): 0 faces north, increasing clockwise seen from above.</param>
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
    /// The same alignment around a new centre and edge, without recomputing the sine and cosine. The window
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

    /// <summary>
    /// The cube's alignment yaw in radians: the world heading its front face points to. 0 faces north, increasing
    /// clockwise seen from above.
    /// </summary>
    public float Yaw { get; }

    /// <summary>
    /// Clamps an edge length to the allowed range; a non-number becomes the default, so a corrupt
    /// setting can never produce an empty or infinite cube.
    /// </summary>
    /// <param name="edge">The requested edge in yalms.</param>
    public static float ClampEdge(float edge) => float.IsNaN(edge) ? DefaultEdge : Math.Clamp(edge, MinEdge, MaxEdge);

    /// <summary>
    /// Whether a world position is inside the cube as currently aligned. Inclusive: a point exactly on a face, edge
    /// or corner counts as inside.
    /// </summary>
    /// <remarks>
    /// The height test comes first and needs no rotation, so players far above or below are rejected for one
    /// subtraction (PROJECT.md, "Efficiency": cull before projecting).
    /// </remarks>
    /// <param name="world">A world position.</param>
    public bool Contains(Vector3 world) => this.TryToView(world, out _);

    /// <summary>
    /// Culls then transforms: returns false for a point outside the cube (players outside aren't shown at all,
    /// PROJECT.md "Players"), otherwise gives its cube-local coordinates.
    /// </summary>
    /// <param name="world">A world position.</param>
    /// <param name="view">The position in cube-local coordinates, if inside.</param>
    public bool TryToView(Vector3 world, out Vector3 view)
    {
        var offset = world - this.Centre;
        if (MathF.Abs(offset.Y) > this.HalfEdge)
        {
            view = default;
            return false;
        }

        var local = this.RotateOffset(offset);
        if (MathF.Abs(local.X) > this.HalfEdge || MathF.Abs(local.Z) > this.HalfEdge)
        {
            view = default;
            return false;
        }

        view = local;
        return true;
    }

    /// <summary>
    /// Turns a world position into cube-local coordinates (right, up, forward), without culling. Used for points
    /// already known to be inside.
    /// </summary>
    /// <param name="world">A world position.</param>
    public Vector3 ToView(Vector3 world) => this.RotateOffset(world - this.Centre);

    /// <summary>
    /// Turns cube-local coordinates back into a world position: the inverse of <see cref="ToView"/>. The test bridge
    /// uses it to place fake players at fixed spots in the cube whatever the camera does.
    /// </summary>
    /// <param name="view">A point in cube-local coordinates.</param>
    /// <remarks>
    /// The rotation in <see cref="RotateOffset"/> is its own inverse (it also flips Z, since forward is −Z at yaw 0),
    /// so the same formula maps back.
    /// </remarks>
    public Vector3 FromView(Vector3 view) => this.Centre + this.RotateOffset(view);

    /// <summary>Rotates an offset from the centre (world axes) into cube-local axes.</summary>
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
