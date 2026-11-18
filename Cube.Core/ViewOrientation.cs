using System.Numerics;

namespace Cube.Core;

/// <summary>
/// How the user has turned the cube on screen with the view control (PROJECT.md, "View orientation control"): a
/// heading (yaw) and a tilt (pitch) of the viewer around the cube. Default: from above and behind, 30° elevation,
/// front face away from the viewer.
/// </summary>
/// <remarks>
/// <para>
/// This only changes how the cube is shown. It is separate from <see cref="CubeSpace.Yaw"/>, which turns the
/// contents with the camera: the camera and player movement never change the view orientation, and the view control
/// never changes what is inside the cube.
/// </para>
/// <para>
/// Two angles rather than a full rotation, because the view control never rolls the view (ImGuizmo keeps the world's
/// up direction, even when looking straight down) and two numbers are easy to save, check and reset. The viewer's
/// axes are worked out once in the constructor, so <see cref="ViewProjection"/> only does dot products per point.
/// </para>
/// <para>
/// The view control is ImGuizmo's view manipulator, which works on a right-handed camera view matrix.
/// <see cref="ToGizmoView"/> and <see cref="FromGizmoView"/> convert, so the plugin can hand ImGuizmo a matrix and
/// read the user's change back. Pure maths, so it is unit tested without the game.
/// </para>
/// </remarks>
public readonly struct ViewOrientation : IEquatable<ViewOrientation>
{
    /// <summary>Default tilt of the view above the horizontal, in degrees (PROJECT.md: 30°).</summary>
    public const float DefaultPitchDegrees = 30f;

    /// <summary>Highest tilt: looking straight down. Lower limit is straight up (the negative).</summary>
    public const float MaxPitchDegrees = 90f;

    /// <summary>Converts degrees to radians.</summary>
    private const float Radians = MathF.PI / 180f;

    /// <summary>Creates an orientation, wrapping the yaw into −π…π and clamping the pitch to ±90°.</summary>
    /// <param name="yaw">
    /// The viewer's heading, in radians: 0 looks forward (+Z in view coordinates, the front face away from the viewer),
    /// positive turns the viewer to the right (looking from above, clockwise).
    /// </param>
    /// <param name="pitch">Tilt of the view above the horizontal, in radians; negative looks up from below.</param>
    public ViewOrientation(float yaw, float pitch)
    {
        this.Yaw = MathF.IEEERemainder(yaw, 2f * MathF.PI);
        this.Pitch = Math.Clamp(pitch, -MaxPitchDegrees * Radians, MaxPitchDegrees * Radians);
        var (sinYaw, cosYaw) = MathF.SinCos(this.Yaw);
        var (sinPitch, cosPitch) = MathF.SinCos(this.Pitch);
        this.Right = new Vector3(cosYaw, 0, -sinYaw);
        this.Up = new Vector3(sinYaw * sinPitch, cosPitch, cosYaw * sinPitch);
        this.Forward = new Vector3(sinYaw * cosPitch, -sinPitch, cosYaw * cosPitch);
    }

    /// <summary>The default view (PROJECT.md): from behind, 30° above, front face away.</summary>
    public static ViewOrientation Default => new(0f, DefaultPitchDegrees * Radians);

    /// <summary>The viewer's heading, in radians (−π…π).</summary>
    public float Yaw { get; }

    /// <summary>The view's tilt above the horizontal, in radians (−π/2…π/2).</summary>
    public float Pitch { get; }

    /// <summary>The screen's right direction, in view coordinates. Always horizontal: the view never rolls.</summary>
    public Vector3 Right { get; }

    /// <summary>The screen's up direction, in view coordinates.</summary>
    public Vector3 Up { get; }

    /// <summary>The direction the viewer looks in, in view coordinates; depth grows along it.</summary>
    public Vector3 Forward { get; }

    /// <summary>The yaw in degrees, as saved in the settings.</summary>
    public float YawDegrees => this.Yaw / Radians;

    /// <summary>The pitch in degrees, as saved in the settings.</summary>
    public float PitchDegrees => this.Pitch / Radians;

    /// <summary>Two orientations are equal when their angles are exactly equal.</summary>
    /// <param name="left">One orientation.</param>
    /// <param name="right">The other.</param>
    public static bool operator ==(ViewOrientation left, ViewOrientation right) => left.Equals(right);

    /// <summary>Two orientations differ when either angle differs.</summary>
    /// <param name="left">One orientation.</param>
    /// <param name="right">The other.</param>
    public static bool operator !=(ViewOrientation left, ViewOrientation right) => !left.Equals(right);

    /// <summary>An orientation from angles in degrees, as stored in the settings.</summary>
    /// <param name="yawDegrees">Heading, in degrees.</param>
    /// <param name="pitchDegrees">Tilt, in degrees.</param>
    public static ViewOrientation FromDegrees(float yawDegrees, float pitchDegrees) =>
        new(yawDegrees * Radians, pitchDegrees * Radians);

    /// <summary>
    /// Reads an orientation back from an ImGuizmo view matrix (after the user dragged or clicked the view control).
    /// Any roll is dropped, and a view that somehow went past straight up or down is clamped.
    /// </summary>
    /// <remarks>
    /// The matrix's columns are the camera's right, up and back axes in ImGuizmo's world, which is view coordinates
    /// with Z mirrored (see <see cref="ToGizmoView"/>). The yaw comes from the right axis, which stays horizontal and
    /// well defined even looking straight down; if it ever isn't usable, the forward direction is used instead.
    /// </remarks>
    /// <param name="view">The view matrix ImGuizmo returned.</param>
    public static ViewOrientation FromGizmoView(in Matrix4x4 view)
    {
        var right = new Vector3(view.M11, view.M21, -view.M31);
        var forward = new Vector3(-view.M13, -view.M23, view.M33);
        var yaw = (right.X * right.X) + (right.Z * right.Z) > 1e-6f
            ? MathF.Atan2(-right.Z, right.X)
            : MathF.Atan2(forward.X, forward.Z);
        var (sinYaw, cosYaw) = MathF.SinCos(yaw);
        var horizontal = (forward.X * sinYaw) + (forward.Z * cosYaw);
        return new ViewOrientation(yaw, MathF.Atan2(-forward.Y, horizontal));
    }

    /// <summary>
    /// The camera view matrix ImGuizmo's view manipulator expects for this orientation: right-handed, looking at the
    /// cube's centre from <paramref name="distance"/> away, laid out like <see cref="Matrix4x4.CreateLookAt"/>.
    /// </summary>
    /// <remarks>
    /// View coordinates are left-handed (right, up, forward), ImGuizmo's world is right-handed, so Z is mirrored on
    /// the way in and out. The widget's own cube therefore turns exactly like the drawn cube.
    /// </remarks>
    /// <param name="distance">How far the camera is from the centre; only matters to ImGuizmo's own maths.</param>
    public Matrix4x4 ToGizmoView(float distance)
    {
        var x = Mirror(this.Right);
        var y = Mirror(this.Up);
        var z = -Mirror(this.Forward);
        var eye = z * distance;
        return new Matrix4x4(
            x.X, y.X, z.X, 0,
            x.Y, y.Y, z.Y, 0,
            x.Z, y.Z, z.Z, 0,
            -Vector3.Dot(x, eye), -Vector3.Dot(y, eye), -Vector3.Dot(z, eye), 1);
    }

    /// <inheritdoc/>
    public bool Equals(ViewOrientation other) => this.Yaw == other.Yaw && this.Pitch == other.Pitch;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ViewOrientation other && this.Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(this.Yaw, this.Pitch);

    /// <summary>Converts between view coordinates and ImGuizmo's right-handed world by mirroring Z.</summary>
    /// <param name="v">A direction.</param>
    private static Vector3 Mirror(Vector3 v) => new(v.X, v.Y, -v.Z);
}
