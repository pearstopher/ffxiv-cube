using System.Numerics;

namespace Cube.Core;

/// <summary>
/// Projects view coordinates (from <see cref="CubeSpace"/>) onto the window, as seen from the user's chosen
/// <see cref="ViewOrientation"/> (PROJECT.md, "View orientation control"; default from above and behind at 30°).
/// </summary>
/// <remarks>
/// <para>
/// The projection is orthographic: things don't shrink with distance, so a player cube is the same size
/// anywhere in the cube (PROJECT.md, "Players": cube size fixed relative to the big cube) and the painter's
/// sort only needs one depth per primitive.
/// </para>
/// <para>
/// The viewer looks along <see cref="ViewOrientation.Forward"/>. On screen, a point's position is its component along
/// the orientation's right and up axes; <see cref="Depth"/> is its component along the forward axis, so it grows with
/// distance from the viewer. The axes are copied from the orientation, so each point costs two or three dot products.
/// </para>
/// </remarks>
public readonly struct ViewProjection
{
    /// <summary>Fraction of the window the cube may fill, leaving a margin for labels.</summary>
    public const float FillFraction = 0.9f;

    /// <summary>Creates a projection.</summary>
    /// <param name="orientation">How the cube is turned on screen.</param>
    /// <param name="origin">Window position (pixels) of the cube's centre.</param>
    /// <param name="pixelsPerYalm">Scale from yalms to pixels.</param>
    public ViewProjection(in ViewOrientation orientation, Vector2 origin, float pixelsPerYalm)
    {
        this.Orientation = orientation;
        this.Origin = origin;
        this.PixelsPerYalm = pixelsPerYalm;
    }

    /// <summary>How the cube is turned on screen.</summary>
    public ViewOrientation Orientation { get; }

    /// <summary>Screen position (pixels) of the cube's centre.</summary>
    public Vector2 Origin { get; }

    /// <summary>Scale from yalms to pixels.</summary>
    public float PixelsPerYalm { get; }

    /// <summary>
    /// Direction from the scene towards the viewer, in view coordinates. A face is visible when its outward
    /// normal points this way; when the view is tilted from below, that makes bottom faces visible and top faces not.
    /// </summary>
    public Vector3 TowardsViewer => -this.Orientation.Forward;

    /// <summary>
    /// The projection that fits a box of the given size into a window region, centred, at one scale for all axes so
    /// the box keeps its real proportions (PROJECT.md, "On screen"). The scale fits the box's bounding sphere (radius
    /// = the length of the half-size), so the box never changes size on screen while the user turns it with the view
    /// control, and its largest dimension fills the widget as the cube always has.
    /// </summary>
    /// <remarks>
    /// For a cube the radius is √3 × half the edge, the fit used before #90, so a 40 × 40 × 40 cube looks exactly as
    /// it did.
    /// </remarks>
    /// <param name="regionMin">Top-left of the drawing region, in pixels.</param>
    /// <param name="regionSize">Size of the drawing region, in pixels.</param>
    /// <param name="half">Half the box's width, height and depth, in yalms (<see cref="CubeSpace.Half"/>).</param>
    /// <param name="orientation">How the cube is turned on screen.</param>
    public static ViewProjection Fit(Vector2 regionMin, Vector2 regionSize, Vector3 half, in ViewOrientation orientation)
    {
        var reach = half.Length();
        var scale = reach > 0f ? MathF.Max(0f, MathF.Min(regionSize.X, regionSize.Y) / (2f * reach) * FillFraction) : 0f;
        return new ViewProjection(orientation, regionMin + (regionSize / 2f), scale);
    }

    /// <summary>The window position (pixels) of a point in view coordinates.</summary>
    /// <param name="view">A point in view coordinates (right, up, forward), in yalms.</param>
    public Vector2 ToScreen(Vector3 view)
    {
        var right = Vector3.Dot(view, this.Orientation.Right);
        var up = Vector3.Dot(view, this.Orientation.Up);
        return new Vector2(this.Origin.X + (right * this.PixelsPerYalm), this.Origin.Y - (up * this.PixelsPerYalm));
    }

    /// <summary>Distance of a point from the viewer along the view direction; larger is further away.</summary>
    /// <param name="view">A point in view coordinates.</param>
    public float Depth(Vector3 view) => Vector3.Dot(view, this.Orientation.Forward);
}
