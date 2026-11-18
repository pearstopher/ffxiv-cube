using System.Numerics;

namespace Cube.Core;

/// <summary>
/// The cube of space around the local player (PROJECT.md, "Exact behaviour rules", Cube): centred on the player
/// horizontally, and vertically too unless "Player height in cube" (#91, <see cref="PlayerHeight"/>) moves them up or down,
/// a box with a width, height and depth of 10–100 yalms each (default 40, see <see cref="BoxExtents"/>), turned about
/// the vertical axis by its alignment <see cref="Yaw"/>. Answers "is this point inside?" and turns world positions into
/// cube-local view coordinates.
/// </summary>
/// <remarks>
/// <para>
/// Since #90 the "cube" may be any box; the name stays because PROJECT.md still calls it the cube. Its extents are in
/// its own axes, so the width is always across the cube and the depth front to back, whatever the yaw. A point is
/// inside when it is inside on each axis separately (PROJECT.md, "Culling").
/// </para>
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
    /// <summary>Cosine of <see cref="Yaw"/>, cached so each point costs a few multiplies.</summary>
    private readonly float cos;

    /// <summary>Sine of <see cref="Yaw"/>.</summary>
    private readonly float sin;

    /// <summary>Creates a cube with every extent the same: the shape before #90, still handy in tests and checks.</summary>
    /// <param name="centre">The local player's world position.</param>
    /// <param name="edge">Edge length in yalms; clamped to <see cref="BoxExtents.Min"/>–<see cref="BoxExtents.Max"/>.</param>
    /// <param name="yaw">The cube's alignment yaw in radians (see <see cref="ViewYaw"/>).</param>
    public CubeSpace(Vector3 centre, float edge, float yaw)
        : this(centre, new Vector3(edge), yaw)
    {
    }

    /// <summary>Creates the box around a centre.</summary>
    /// <param name="centre">The local player's world position.</param>
    /// <param name="size">Width, height and depth in yalms; each clamped (<see cref="BoxExtents.Clamp(Vector3)"/>).</param>
    /// <param name="yaw">The cube's alignment yaw in radians (see <see cref="ViewYaw"/>): 0 faces north, increasing clockwise seen from above.</param>
    public CubeSpace(Vector3 centre, Vector3 size, float yaw)
    {
        this.Centre = centre;
        this.Player = centre;
        this.Size = BoxExtents.Clamp(size);
        this.Half = this.Size / 2f;
        this.Largest = BoxExtents.Largest(this.Size);
        this.Yaw = yaw;
        this.cos = MathF.Cos(yaw);
        this.sin = MathF.Sin(yaw);
    }

    /// <summary>
    /// Creates the box around a player who sits at a chosen height in it (PROJECT.md, "Player height in cube", #91):
    /// the box's centre is shifted vertically by <see cref="PlayerHeight.BoxCentre"/>, so culling, projection and the
    /// collision region all use the shifted box.
    /// </summary>
    /// <param name="player">The local player's world position.</param>
    /// <param name="size">Width, height and depth in yalms; each clamped (<see cref="BoxExtents.Clamp(Vector3)"/>).</param>
    /// <param name="yaw">The cube's alignment yaw in radians (see <see cref="ViewYaw"/>).</param>
    /// <param name="playerHeight">Where the player sits, as a fraction of the height from the bottom face (0–1).</param>
    public CubeSpace(Vector3 player, Vector3 size, float yaw, float playerHeight)
        : this(PlayerHeight.BoxCentre(player, BoxExtents.Clamp(size.Y), playerHeight), size, yaw)
    {
        this.Player = player;
    }

    /// <summary>Copies a cube with a new player position, size and player height, keeping its yaw and rotation.</summary>
    /// <param name="source">The cube whose yaw (and cached sine and cosine) to keep.</param>
    /// <param name="player">The new player position.</param>
    /// <param name="size">The new size; clamped like the main constructor.</param>
    /// <param name="playerHeight">The player height fraction (see <see cref="PlayerHeight"/>).</param>
    private CubeSpace(in CubeSpace source, Vector3 player, Vector3 size, float playerHeight)
    {
        this.Size = BoxExtents.Clamp(size);
        this.Player = player;
        this.Centre = PlayerHeight.BoxCentre(player, this.Size.Y, playerHeight);
        this.Half = this.Size / 2f;
        this.Largest = BoxExtents.Largest(this.Size);
        this.Yaw = source.Yaw;
        this.cos = source.cos;
        this.sin = source.sin;
    }

    /// <summary>
    /// The same alignment around a new player position, size and player height, without recomputing the sine and
    /// cosine. The window uses it every frame the yaw hasn't changed (PROJECT.md, "Efficiency and memory": recompute
    /// only when inputs change).
    /// </summary>
    /// <param name="player">The local player's new world position.</param>
    /// <param name="size">The new width, height and depth in yalms.</param>
    /// <param name="playerHeight">Where the player sits in the box (#91); <see cref="PlayerHeight.Default"/> centres them.</param>
    public CubeSpace Recentred(Vector3 player, Vector3 size, float playerHeight = PlayerHeight.Default) =>
        new(this, player, size, playerHeight);

    /// <summary>
    /// The box's centre in world coordinates: the local player's position, shifted vertically when the player height
    /// setting isn't 50% (#91). Containment and view coordinates are measured from here, so the box is drawn centred in
    /// the widget and turns around this point.
    /// </summary>
    public Vector3 Centre { get; }

    /// <summary>
    /// The local player's world position the box was built around. Equal to <see cref="Centre"/> at the default player
    /// height; the test bridge places fake players relative to it.
    /// </summary>
    public Vector3 Player { get; }

    /// <summary>Width (X), height (Y) and depth (Z) in yalms, in the cube's own axes, already clamped.</summary>
    public Vector3 Size { get; }

    /// <summary>Half the size: the distance from the centre to the faces on each axis.</summary>
    public Vector3 Half { get; }

    /// <summary>
    /// The largest of the three extents. Player and NPC markers are about 1/40 of it (PROJECT.md, "Players"), and the
    /// cache and clipper use it where one number has to cover the whole box.
    /// </summary>
    public float Largest { get; }

    /// <summary>
    /// The cube's alignment yaw in radians: the world heading its front face points to. 0 faces north, increasing
    /// clockwise seen from above.
    /// </summary>
    public float Yaw { get; }

    /// <summary>
    /// Whether a world position is inside the cube as currently aligned: inside on each of its three axes separately
    /// (PROJECT.md, "Culling"). Inclusive: a point exactly on a face, edge or corner counts as inside.
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
        if (MathF.Abs(offset.Y) > this.Half.Y)
        {
            view = default;
            return false;
        }

        var local = this.RotateOffset(offset);
        if (MathF.Abs(local.X) > this.Half.X || MathF.Abs(local.Z) > this.Half.Z)
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
