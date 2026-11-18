using System.Numerics;

namespace Cube.Core;

/// <summary>
/// One thing to draw, already projected to window pixels: the plugin turns each into one ImGui call
/// (PROJECT.md, "Cheap drawing": the fewest primitives that do the job).
/// </summary>
/// <remarks>A plain struct kept in <see cref="DrawList"/>'s reused array, so building a frame allocates nothing.</remarks>
public struct DrawPrimitive
{
    /// <summary>Line, quad, label or polygon.</summary>
    public PrimitiveKind Kind;

    /// <summary>
    /// A polygon's corner count (<see cref="PrimitiveKind.Polygon"/>); 0 for other kinds. A byte, so with
    /// <see cref="FirstPoint"/> it fits in the padding after <see cref="Kind"/>: the struct stays the size it was and
    /// sorting moves no extra bytes.
    /// </summary>
    public byte Corners;

    /// <summary>
    /// Where a polygon's corners start in the draw list's point buffer (<see cref="DrawList.Points"/>); 16 bits, so the
    /// buffer holds at most <see cref="DrawList.MaxPointCapacity"/> corners.
    /// </summary>
    public ushort FirstPoint;

    /// <summary>First point (pixels); a polygon's first corner, for messages.</summary>
    public Vector2 A;

    /// <summary>Second point (pixels).</summary>
    public Vector2 B;

    /// <summary>Third point of a quad (pixels).</summary>
    public Vector2 C;

    /// <summary>Fourth point of a quad (pixels).</summary>
    public Vector2 D;

    /// <summary>Colour, packed as ImGui expects (<see cref="Colour"/>).</summary>
    public uint Colour;

    /// <summary>Distance from the viewer, for the painter's sort; larger is drawn first.</summary>
    public float Depth;

    /// <summary>Order added this frame; breaks depth ties so equal-depth primitives never swap and flicker.</summary>
    public int Sequence;

    /// <summary>
    /// Which thing the primitive belongs to: 0 for the scene (the big cube), a positive number for the index of the
    /// player in this frame's <see cref="PlayerSet"/> plus one, and a negative number for a collision box
    /// (<see cref="CollisionCubes.TagFor"/>). Lets the bridge check each player's cube and each box, and labels find
    /// their player.
    /// </summary>
    public int Tag;
}
