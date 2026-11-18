using System.Numerics;

namespace Cube.Core;

/// <summary>
/// How the user has turned the cube on screen with the view control (PROJECT.md, "View orientation control"): a full
/// rotation, roll included. Default: from above and behind, 30° elevation, front face away from the viewer.
/// </summary>
/// <remarks>
/// <para>
/// This only changes how the cube is shown. It is separate from <see cref="CubeSpace.Yaw"/>, which turns the
/// contents with the camera: the camera and player movement never change the view orientation, and the view control
/// never changes what is inside the cube.
/// </para>
/// <para>
/// A quaternion rather than angles, because the control is ImGuizmo's rotate gizmo, whose rings can turn the cube
/// around any axis, so any orientation is allowed (PROJECT.md: "including roll"). The rotation takes a point in view
/// coordinates to the viewer's (right, up, forward) frame. The viewer's axes are worked out once in the constructor,
/// so <see cref="ViewProjection"/> only does dot products per point.
/// </para>
/// <para>
/// Orientations saved by the old yaw-and-tilt control (#63) are converted with <see cref="FromDegrees"/>.
/// <see cref="ToGizmoModel"/> and <see cref="TryFromGizmoModel"/> convert to and from the model matrix the gizmo
/// turns. Pure maths, so it is unit tested without the game.
/// </para>
/// </remarks>
public readonly struct ViewOrientation : IEquatable<ViewOrientation>
{
    /// <summary>Default tilt of the view above the horizontal, in degrees (PROJECT.md: 30°).</summary>
    public const float DefaultPitchDegrees = 30f;

    /// <summary>Highest tilt the old yaw-and-tilt control allowed: looking straight down (and −90° straight up).</summary>
    public const float MaxPitchDegrees = 90f;

    /// <summary>Converts degrees to radians.</summary>
    private const float Radians = MathF.PI / 180f;

    /// <summary>
    /// Creates an orientation from a rotation, normalised. A rotation that isn't usable (zero length or not a number,
    /// e.g. from a corrupt file) gives the default view, so a bad value never produces a broken cube.
    /// </summary>
    /// <param name="rotation">
    /// Takes a point in view coordinates to the viewer's frame: x along the screen's right, y up, z away from the
    /// viewer.
    /// </param>
    public ViewOrientation(Quaternion rotation)
    {
        this.Rotation = Quaternion.Normalize(IsUsable(rotation) ? rotation : DefaultRotation());
        var m = Matrix4x4.CreateFromQuaternion(this.Rotation);
        this.Right = new Vector3(m.M11, m.M21, m.M31);
        this.Up = new Vector3(m.M12, m.M22, m.M32);
        this.Forward = new Vector3(m.M13, m.M23, m.M33);
    }

    /// <summary>The default view (PROJECT.md): from behind, 30° above, front face away.</summary>
    public static ViewOrientation Default => new(DefaultRotation());

    /// <summary>The rotation from view coordinates to the viewer's frame, normalised. This is what's saved.</summary>
    public Quaternion Rotation { get; }

    /// <summary>The screen's right direction, in view coordinates.</summary>
    public Vector3 Right { get; }

    /// <summary>The screen's up direction, in view coordinates.</summary>
    public Vector3 Up { get; }

    /// <summary>The direction the viewer looks in, in view coordinates; depth grows along it.</summary>
    public Vector3 Forward { get; }

    /// <summary>Two orientations are equal when their rotations are exactly equal.</summary>
    /// <param name="left">One orientation.</param>
    /// <param name="right">The other.</param>
    public static bool operator ==(ViewOrientation left, ViewOrientation right) => left.Equals(right);

    /// <summary>Two orientations differ when their rotations differ.</summary>
    /// <param name="left">One orientation.</param>
    /// <param name="right">The other.</param>
    public static bool operator !=(ViewOrientation left, ViewOrientation right) => !left.Equals(right);

    /// <summary>
    /// An orientation from a heading and a tilt in degrees: the old yaw-and-tilt control's saved values (#63), and a
    /// handy way to name a view in tests. The tilt is clamped to ±90°, as the old control did.
    /// </summary>
    /// <param name="yawDegrees">
    /// The viewer's heading: 0 looks forward (+Z in view coordinates, the front face away from the viewer), positive
    /// turns the viewer to the right (looking from above, clockwise).
    /// </param>
    /// <param name="pitchDegrees">Tilt of the view above the horizontal; negative looks up from below.</param>
    public static ViewOrientation FromDegrees(float yawDegrees, float pitchDegrees)
    {
        var (sinYaw, cosYaw) = MathF.SinCos(yawDegrees * Radians);
        var (sinPitch, cosPitch) = MathF.SinCos(Math.Clamp(pitchDegrees, -MaxPitchDegrees, MaxPitchDegrees) * Radians);
        var right = new Vector3(cosYaw, 0, -sinYaw);
        var up = new Vector3(sinYaw * sinPitch, cosPitch, cosYaw * sinPitch);
        var forward = new Vector3(sinYaw * cosPitch, -sinPitch, cosYaw * cosPitch);
        return new ViewOrientation(Quaternion.CreateFromRotationMatrix(FromAxes(right, up, forward)));
    }

    /// <summary>
    /// Reads an orientation back from the model matrix ImGuizmo's rotate gizmo returned after a drag. Any scale or
    /// drift the gizmo's maths added is removed.
    /// </summary>
    /// <param name="model">The model matrix, laid out as by <see cref="ToGizmoModel"/>.</param>
    /// <param name="view">The orientation it holds.</param>
    /// <returns>False if the matrix holds no usable rotation; the caller then keeps the old view.</returns>
    public static bool TryFromGizmoModel(in Matrix4x4 model, out ViewOrientation view)
    {
        // A (nearly) flat matrix has no rotation to read, though Decompose may still "succeed" on it.
        var unmirrored = Mirror(model);
        if (MathF.Abs(unmirrored.GetDeterminant()) > 1e-6f
            && Matrix4x4.Decompose(unmirrored, out _, out var rotation, out _)
            && IsUsable(rotation))
        {
            view = new ViewOrientation(rotation);
            return true;
        }

        view = Default;
        return false;
    }

    /// <summary>
    /// The model matrix for ImGuizmo's rotate gizmo: the cube's rotation as seen by a camera on the +Z axis looking
    /// at the centre (laid out like <see cref="Matrix4x4.CreateLookAt"/>, the way ImGuizmo expects).
    /// </summary>
    /// <remarks>
    /// View coordinates are left-handed (right, up, forward), ImGuizmo's world is right-handed, so Z is mirrored on
    /// both sides of the rotation. The gizmo's x, y and z rings then lie around the cube's own x, y and z axes, so the
    /// ring colours match the cube's axes and dragging the x ring turns the cube around its x axis.
    /// </remarks>
    public Matrix4x4 ToGizmoModel() => Mirror(Matrix4x4.CreateFromQuaternion(this.Rotation));

    /// <inheritdoc/>
    public bool Equals(ViewOrientation other) => this.Rotation == other.Rotation;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ViewOrientation other && this.Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => this.Rotation.GetHashCode();

    /// <summary>The default rotation: heading 0, tilt 30° (computed, not stored, so <see cref="Default"/> can't recurse).</summary>
    private static Quaternion DefaultRotation()
    {
        var (sin, cos) = MathF.SinCos(DefaultPitchDegrees * Radians);
        return Quaternion.CreateFromRotationMatrix(FromAxes(Vector3.UnitX, new Vector3(0, cos, sin), new Vector3(0, -sin, cos)));
    }

    /// <summary>A rotation matrix whose columns are the viewer's axes, so a point times it gives its viewer coordinates.</summary>
    /// <param name="right">The screen's right, in view coordinates.</param>
    /// <param name="up">The screen's up.</param>
    /// <param name="forward">The viewing direction.</param>
    private static Matrix4x4 FromAxes(Vector3 right, Vector3 up, Vector3 forward) => new(
        right.X, up.X, forward.X, 0,
        right.Y, up.Y, forward.Y, 0,
        right.Z, up.Z, forward.Z, 0,
        0, 0, 0, 1);

    /// <summary>
    /// Converts a rotation between view coordinates and ImGuizmo's right-handed world by mirroring Z on both sides
    /// (S·M·S with S = diag(1, 1, −1)), which negates the entries that mix Z with X or Y. Its own inverse.
    /// </summary>
    /// <param name="m">A rotation matrix.</param>
    private static Matrix4x4 Mirror(Matrix4x4 m)
    {
        m.M13 = -m.M13;
        m.M23 = -m.M23;
        m.M31 = -m.M31;
        m.M32 = -m.M32;
        return m;
    }

    /// <summary>Whether a rotation can be normalised: every part a number and not (nearly) zero length.</summary>
    /// <param name="q">The rotation.</param>
    private static bool IsUsable(Quaternion q) =>
        float.IsFinite(q.X) && float.IsFinite(q.Y) && float.IsFinite(q.Z) && float.IsFinite(q.W) && q.LengthSquared() > 1e-6f;
}
