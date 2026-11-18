using System.Numerics;

namespace Cube.Core;

/// <summary>
/// The model matrix the corner gizmo works on, which carries both the view orientation (#79) and the cube's size
/// (#90), and the rules for reading a drag back from it (PROJECT.md, "View orientation control" and "Control" under
/// the box size: the axis handles change width, height or depth, the centre handle is zoom).
/// </summary>
/// <remarks>
/// <para>
/// ImGuizmo's rotate and scale operations share one model matrix. Its rotation is the view orientation
/// (<see cref="ViewOrientation.ToGizmoModel"/>); its rows are scaled by the cube's extents over
/// <see cref="BoxExtents.Default"/>, so the default cube has scale 1. Rows are the model's local axes, and local axis i
/// is the cube's axis i, so the x, y and z scale handles change the width, height and depth whatever the current
/// orientation is.
/// </para>
/// <para>
/// Reading back, a drag is sorted into what it changed so one never disturbs the other (#90: "the rotation from #79
/// still works and isn't disturbed by scaling"): a turn only when the rotation moved by more than float noise, a
/// resize only on axes whose scale moved. All three scales moving by the same ratio is the centre handle, which zooms
/// with <see cref="BoxExtents.Zoom"/> so the ratios survive clamping. Pure maths on structs: nothing is allocated, so
/// it is safe in the per-frame path.
/// </para>
/// </remarks>
public static class GizmoModel
{
    /// <summary>
    /// Smallest change in any of the view's axes that counts as a turn. ImGuizmo's re-orthonormalising leaves noise
    /// around 1e-6; one pixel of drag turns the view by hundredths of a radian.
    /// </summary>
    public const float RotationTolerance = 1e-4f;

    /// <summary>Smallest change in an axis's scale ratio that counts as a resize on that axis.</summary>
    public const float ScaleTolerance = 1e-4f;

    /// <summary>How close the three ratios must be (relative to the largest) for a drag to count as the centre handle's zoom.</summary>
    public const float UniformTolerance = 1e-3f;

    /// <summary>The model matrix for the gizmo: the view's rotation with each local axis scaled by the cube's extent.</summary>
    /// <param name="view">The current view orientation.</param>
    /// <param name="size">The cube's width, height and depth; clamped first.</param>
    /// <remarks>
    /// Scaling rows before the Z mirror in <see cref="ViewOrientation.ToGizmoModel"/> is the same as after it: the
    /// mirror only flips signs, so it commutes with a diagonal scale.
    /// </remarks>
    public static Matrix4x4 Build(in ViewOrientation view, Vector3 size) =>
        Matrix4x4.CreateScale(BoxExtents.Clamp(size) / BoxExtents.Default) * view.ToGizmoModel();

    /// <summary>Reads what a gizmo drag changed: the view, the size, both or neither.</summary>
    /// <param name="model">The model matrix after <c>ImGuizmo.Manipulate</c>.</param>
    /// <param name="view">The view the matrix was built from.</param>
    /// <param name="size">The size the matrix was built from.</param>
    /// <returns>The new view and size, and which of them changed. Unchanged parts are returned exactly as given.</returns>
    public static GizmoChange Read(in Matrix4x4 model, in ViewOrientation view, Vector3 size)
    {
        size = BoxExtents.Clamp(size);
        var newView = view;
        var viewChanged = ViewOrientation.TryFromGizmoModel(model, out var turned) && Differs(turned, view);
        if (viewChanged)
        {
            newView = turned;
        }

        var newSize = ReadSize(model, size);
        return new GizmoChange(newView, newSize, viewChanged, newSize != size);
    }

    /// <summary>
    /// The size a matrix holds: per-axis changes on the axes that moved, or a zoom when all three moved together.
    /// </summary>
    /// <param name="model">The model matrix.</param>
    /// <param name="size">The clamped size it was built from.</param>
    private static Vector3 ReadSize(in Matrix4x4 model, Vector3 size)
    {
        var scaled = new Vector3(
            RowLength(model.M11, model.M12, model.M13),
            RowLength(model.M21, model.M22, model.M23),
            RowLength(model.M31, model.M32, model.M33)) * BoxExtents.Default;
        if (!float.IsFinite(scaled.X) || !float.IsFinite(scaled.Y) || !float.IsFinite(scaled.Z))
        {
            return size;
        }

        var ratio = scaled / size;
        var movedX = MathF.Abs(ratio.X - 1f) > ScaleTolerance;
        var movedY = MathF.Abs(ratio.Y - 1f) > ScaleTolerance;
        var movedZ = MathF.Abs(ratio.Z - 1f) > ScaleTolerance;
        if (!movedX && !movedY && !movedZ)
        {
            return size;
        }

        var high = MathF.Max(ratio.X, MathF.Max(ratio.Y, ratio.Z));
        var low = MathF.Min(ratio.X, MathF.Min(ratio.Y, ratio.Z));
        if (movedX && movedY && movedZ && high - low <= UniformTolerance * high)
        {
            // The centre handle: zoom, keeping the ratios even when a limit is reached (PROJECT.md, "Zoom").
            return BoxExtents.Zoom(size, (ratio.X + ratio.Y + ratio.Z) / 3f);
        }

        return BoxExtents.Clamp(new Vector3(
            movedX ? scaled.X : size.X,
            movedY ? scaled.Y : size.Y,
            movedZ ? scaled.Z : size.Z));
    }

    /// <summary>Whether two orientations differ by more than <see cref="RotationTolerance"/> in any axis component.</summary>
    /// <param name="a">One orientation.</param>
    /// <param name="b">The other.</param>
    private static bool Differs(in ViewOrientation a, in ViewOrientation b) =>
        MaxAbs(a.Right - b.Right) > RotationTolerance
        || MaxAbs(a.Up - b.Up) > RotationTolerance
        || MaxAbs(a.Forward - b.Forward) > RotationTolerance;

    /// <summary>The largest absolute component of a vector.</summary>
    /// <param name="v">The vector.</param>
    private static float MaxAbs(Vector3 v) => MathF.Max(MathF.Abs(v.X), MathF.Max(MathF.Abs(v.Y), MathF.Abs(v.Z)));

    /// <summary>The length of one matrix row: that local axis's scale.</summary>
    /// <param name="x">First entry.</param>
    /// <param name="y">Second entry.</param>
    /// <param name="z">Third entry.</param>
    private static float RowLength(float x, float y, float z) => MathF.Sqrt((x * x) + (y * y) + (z * z));
}

/// <summary>What a gizmo drag changed (<see cref="GizmoModel.Read"/>).</summary>
/// <param name="View">The view after the drag; the old one if it didn't turn.</param>
/// <param name="Size">The cube size after the drag, clamped; the old one if it wasn't resized.</param>
/// <param name="ViewChanged">Whether the drag turned the view.</param>
/// <param name="SizeChanged">Whether the drag resized or zoomed the cube.</param>
public readonly record struct GizmoChange(ViewOrientation View, Vector3 Size, bool ViewChanged, bool SizeChanged);
