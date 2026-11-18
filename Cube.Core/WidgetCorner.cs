namespace Cube.Core;

/// <summary>
/// Which corner of the cube window the view control sits in (PROJECT.md, "View orientation control": bottom right by
/// default). Placement is worked out by <see cref="ViewWidget.Position"/>.
/// </summary>
public enum WidgetCorner
{
    /// <summary>Bottom right (the default).</summary>
    BottomRight,

    /// <summary>Bottom left.</summary>
    BottomLeft,

    /// <summary>Top right.</summary>
    TopRight,

    /// <summary>Top left.</summary>
    TopLeft,
}
