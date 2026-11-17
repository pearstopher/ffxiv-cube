using System.Numerics;

namespace Cube.Core;

/// <summary>
/// Projects view coordinates (from <see cref="CubeSpace"/>) onto the window: a fixed-angle view from above
/// and behind (PROJECT.md, "View": default 30° elevation).
/// </summary>
/// <remarks>
/// <para>
/// The projection is orthographic: things don't shrink with distance, so a player cube is the same size
/// anywhere in the cube (PROJECT.md, "Players": cube size fixed relative to the big cube) and the painter's
/// sort only needs one depth per primitive.
/// </para>
/// <para>
/// The viewer looks forward (+Z in view coordinates) and down by <see cref="Elevation"/>. On screen, a point
/// further forward or higher appears higher; <see cref="Depth"/> grows with distance from the viewer.
/// </para>
/// </remarks>
public readonly struct ViewProjection
{
    /// <summary>Default elevation of the view, in degrees (PROJECT.md).</summary>
    public const float DefaultElevationDegrees = 30f;

    /// <summary>Fraction of the window the cube may fill, leaving a margin for labels.</summary>
    public const float FillFraction = 0.9f;

    /// <summary>Cosine of the elevation.</summary>
    private readonly float cos;

    /// <summary>Sine of the elevation.</summary>
    private readonly float sin;

    /// <summary>Creates a projection.</summary>
    /// <param name="elevation">Angle of the view above the horizontal, in radians.</param>
    /// <param name="origin">Window position (pixels) of the cube's centre.</param>
    /// <param name="pixelsPerYalm">Scale from yalms to pixels.</param>
    public ViewProjection(float elevation, Vector2 origin, float pixelsPerYalm)
    {
        this.Elevation = elevation;
        this.Origin = origin;
        this.PixelsPerYalm = pixelsPerYalm;
        this.cos = MathF.Cos(elevation);
        this.sin = MathF.Sin(elevation);
    }

    /// <summary>Angle of the view above the horizontal, in radians.</summary>
    public float Elevation { get; }

    /// <summary>Screen position (pixels) of the cube's centre.</summary>
    public Vector2 Origin { get; }

    /// <summary>Scale from yalms to pixels.</summary>
    public float PixelsPerYalm { get; }

    /// <summary>
    /// Direction from the scene towards the viewer, in view coordinates. A face is visible when its outward
    /// normal points this way.
    /// </summary>
    public Vector3 TowardsViewer => new(0, this.sin, -this.cos);

    /// <summary>
    /// The projection that fits a cube of the given size into a window region, centred. The scale allows for
    /// any yaw (a corner can swing out to √2 × half the edge horizontally), so the cube doesn't change size
    /// as the camera turns.
    /// </summary>
    /// <param name="regionMin">Top-left of the drawing region, in pixels.</param>
    /// <param name="regionSize">Size of the drawing region, in pixels.</param>
    /// <param name="halfEdge">Half the cube's edge, in yalms.</param>
    /// <param name="elevation">Elevation of the view, in radians.</param>
    public static ViewProjection Fit(Vector2 regionMin, Vector2 regionSize, float halfEdge, float elevation)
    {
        var reach = halfEdge * MathF.Sqrt(2f);
        var halfWidth = reach;
        var halfHeight = (reach * MathF.Sin(elevation)) + (halfEdge * MathF.Cos(elevation));
        var scale = MathF.Max(0f, MathF.Min(regionSize.X / (2f * halfWidth), regionSize.Y / (2f * halfHeight)) * FillFraction);
        return new ViewProjection(elevation, regionMin + (regionSize / 2f), scale);
    }

    /// <summary>The window position (pixels) of a point in view coordinates.</summary>
    /// <param name="view">A point in view coordinates (right, up, forward), in yalms.</param>
    public Vector2 ToScreen(Vector3 view)
    {
        var up = (view.Y * this.cos) + (view.Z * this.sin);
        return new Vector2(this.Origin.X + (view.X * this.PixelsPerYalm), this.Origin.Y - (up * this.PixelsPerYalm));
    }

    /// <summary>Distance of a point from the viewer along the view direction; larger is further away.</summary>
    /// <param name="view">A point in view coordinates.</param>
    public float Depth(Vector3 view) => (view.Z * this.cos) - (view.Y * this.sin);
}
