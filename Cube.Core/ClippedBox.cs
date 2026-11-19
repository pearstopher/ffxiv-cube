using System.Numerics;
using System.Runtime.CompilerServices;

namespace Cube.Core;

/// <summary>
/// One collision box clipped to the cube (PROJECT.md, "Collision (phase 2)": each collider is shown as a simplified
/// box, clipped to the cube): an upright prism whose footprint is the part of the box's footprint inside the cube's
/// turned square, cut to the cube's height.
/// </summary>
/// <remarks>
/// <para>
/// The cube is turned by its yaw (and a collision box may be turned too, #123), so the overlap of the two footprints is a
/// convex polygon of up to eight corners, not a rectangle (#93). Drawing the polygon's world-aligned bounds instead
/// reached up to √2 past the cube's sides for a box bigger than the cube (a floor), so collision stuck out of the
/// cube and looked like a separate, still shape when the contents turned with the camera. The prism is exact.
/// </para>
/// <para>
/// When the overlap is still a world-aligned rectangle (the box lies inside the cube's footprint, or the cube is at a
/// multiple of 90°), <see cref="IsRectangle"/> is true and the box is drawn exactly as before, from
/// <see cref="Bounds"/>: the common case stays the cheap one. A plain struct with its corners stored inline, so the
/// reused buffer in <see cref="ClippedCollision"/> never allocates (PROJECT.md, "Efficiency and memory").
/// </para>
/// </remarks>
public struct ClippedBox
{
    /// <summary>Most corners a clipped footprint can have: a rectangle cut by the four sides of another.</summary>
    public const int MaxCorners = 8;

    /// <summary>The footprint's corners in world X/Z, counter-clockwise; only the first <see cref="CornerCount"/> are used.</summary>
    private FootprintCorners corners;

    /// <summary>Creates a clipped box that is a world-aligned box: no turned edges.</summary>
    /// <param name="bounds">The box.</param>
    public ClippedBox(in CollisionBox bounds)
    {
        this.Bounds = bounds;
        this.IsRectangle = true;
        this.CornerCount = 4;
        this.corners[0] = new Vector2(bounds.Min.X, bounds.Min.Z);
        this.corners[1] = new Vector2(bounds.Max.X, bounds.Min.Z);
        this.corners[2] = new Vector2(bounds.Max.X, bounds.Max.Z);
        this.corners[3] = new Vector2(bounds.Min.X, bounds.Max.Z);
    }

    /// <summary>Creates a prism from its footprint and height.</summary>
    /// <param name="footprint">The footprint's corners in world X/Z, counter-clockwise; at most <see cref="MaxCorners"/>.</param>
    /// <param name="bounds">The prism's world-aligned bounds.</param>
    /// <param name="isRectangle">Whether the footprint is the bounds' own rectangle.</param>
    internal ClippedBox(ReadOnlySpan<Vector2> footprint, in CollisionBox bounds, bool isRectangle)
    {
        this.Bounds = bounds;
        this.IsRectangle = isRectangle;
        this.CornerCount = Math.Min(footprint.Length, MaxCorners);
        for (var i = 0; i < this.CornerCount; i++)
        {
            this.corners[i] = footprint[i];
        }
    }

    /// <summary>
    /// The prism's world-aligned bounds: its exact height, and sideways the box around its footprint. Used for the
    /// sorting depth (<see cref="BoxProjector.FarDepth"/>), for drawing when <see cref="IsRectangle"/>, and by the
    /// bridge to find the floor.
    /// </summary>
    public CollisionBox Bounds { readonly get; private set; }

    /// <summary>
    /// Whether the footprint is a world-aligned rectangle equal to <see cref="Bounds"/>' (or has no area), so the box
    /// is drawn as a plain box.
    /// </summary>
    public bool IsRectangle { readonly get; private set; }

    /// <summary>How many footprint corners there are: 4 for a rectangle, 3 to <see cref="MaxCorners"/> for a prism.</summary>
    public int CornerCount { readonly get; private set; }

    /// <summary>One footprint corner in world X/Z (as <see cref="Vector2"/> X and Y), counter-clockwise seen with +Z up.</summary>
    /// <param name="index">0 to <see cref="CornerCount"/> − 1.</param>
    public readonly Vector2 Corner(int index) => this.corners[index];

    /// <summary>One footprint corner as a world point at a height.</summary>
    /// <param name="index">0 to <see cref="CornerCount"/> − 1.</param>
    /// <param name="y">The world height.</param>
    public readonly Vector3 CornerAt(int index, float y) => new(this.corners[index].X, y, this.corners[index].Y);
}

/// <summary>Inline storage for a <see cref="ClippedBox"/>'s footprint corners, so the struct holds no array.</summary>
[InlineArray(ClippedBox.MaxCorners)]
internal struct FootprintCorners
{
    /// <summary>The first corner; the runtime lays out the rest after it.</summary>
    private Vector2 first;
}
