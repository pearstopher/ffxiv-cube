using System.Numerics;

namespace Cube.Core;

/// <summary>
/// Where the view control goes and how big it is (PROJECT.md, "View orientation control": a small control in a corner
/// of the window). Kept in Core so the placement rules are unit tested.
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
    public static float Size(Vector2 regionSize) =>
        Math.Clamp(MathF.Min(regionSize.X, regionSize.Y) * SizeFraction, MinSize, MaxSize);

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
