using System.Numerics;

namespace Cube.Core;

/// <summary>
/// Where the view controls go and how big they are (PROJECT.md, "View orientation control": a rotate gizmo in a corner
/// of the window, with a resize gizmo of the same size beside it). Kept in Core so the placement rules are unit tested.
/// </summary>
/// <remarks>
/// The cube is fitted to a circle filling <see cref="ViewProjection.FillFraction"/> of the region's smaller side, so a
/// corner square up to about 0.18 of that side stays outside the circle and out of the cube's way. The control is kept
/// between <see cref="MinSize"/> (still usable) and <see cref="MaxSize"/> (no need for more in a big window).
/// </remarks>
public static class ViewWidget
{
    /// <summary>Fraction of the region's smaller side the control takes.</summary>
    public const float SizeFraction = 0.18f;

    /// <summary>Smallest control, in pixels, so it stays usable in a small window.</summary>
    public const float MinSize = 48f;

    /// <summary>Largest control, in pixels.</summary>
    public const float MaxSize = 120f;

    /// <summary>The control's side length for a drawing region.</summary>
    /// <param name="regionSize">The drawing region's size, in pixels.</param>
    /// <remarks>
    /// Two squares sit side by side (#97), so the size is also capped at half the region's width: in a very narrow
    /// window both shrink rather than overlap, even below <see cref="MinSize"/>.
    /// </remarks>
    public static float Size(Vector2 regionSize) =>
        MathF.Max(0f, MathF.Min(
            Math.Clamp(MathF.Min(regionSize.X, regionSize.Y) * SizeFraction, MinSize, MaxSize),
            regionSize.X / 2f));

    /// <summary>
    /// Top-left of the resize gizmo's square: the same size as the rotate square, next to it along the window's top or
    /// bottom edge and inward from the chosen corner (#97), so the two never overlap.
    /// </summary>
    /// <param name="regionMin">Top-left of the drawing region, in pixels.</param>
    /// <param name="regionSize">Size of the drawing region, in pixels.</param>
    /// <param name="corner">The chosen corner, which the rotate gizmo occupies.</param>
    /// <param name="size">The control's side length (from <see cref="Size"/>).</param>
    public static Vector2 ResizePosition(Vector2 regionMin, Vector2 regionSize, WidgetCorner corner, float size)
    {
        var rotate = Position(regionMin, regionSize, corner, size);
        var right = corner is WidgetCorner.BottomRight or WidgetCorner.TopRight;
        return new Vector2(right ? rotate.X - size : rotate.X + size, rotate.Y);
    }

    /// <summary>Top-left of the control, in the chosen corner of the drawing region.</summary>
    /// <param name="regionMin">Top-left of the drawing region, in pixels.</param>
    /// <param name="regionSize">Size of the drawing region, in pixels.</param>
    /// <param name="corner">The chosen corner.</param>
    /// <param name="size">The control's side length (from <see cref="Size"/>).</param>
    public static Vector2 Position(Vector2 regionMin, Vector2 regionSize, WidgetCorner corner, float size)
    {
        var right = corner is WidgetCorner.BottomRight or WidgetCorner.TopRight;
        var bottom = corner is WidgetCorner.BottomRight or WidgetCorner.BottomLeft;
        return new Vector2(
            right ? regionMin.X + regionSize.X - size : regionMin.X,
            bottom ? regionMin.Y + regionSize.Y - size : regionMin.Y);
    }
}
