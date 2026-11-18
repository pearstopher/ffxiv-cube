using System.Numerics;

namespace Cube.Core;

/// <summary>
/// Draws the collision around the player as solid boxes in the cube, sorted together with the players (PROJECT.md,
/// "Collision (phase 2)": each collider inside or crossing the cube is shown as a simplified box, clipped to the cube;
/// "Draw order": far things first, so near ones cover far ones).
/// </summary>
/// <remarks>
/// <para>
/// <b>Input.</b> The boxes come already clipped to the cube (<see cref="ClippedCollision"/>), in world coordinates,
/// and are drawn like player cubes (<see cref="BoxProjector"/>): in world axes, so they turn with the rest of the
/// contents, shaded the same way, as their visible faces only. A box cut by a side of a turned cube is drawn as the
/// prism the cut leaves (<see cref="BoxProjector.AddPrism"/>), so it stays inside the cube at any yaw (#93); every
/// other box is a plain box (<see cref="BoxProjector.AddFaces"/>). Each box's faces carry the tag
/// <see cref="TagFor"/> gives it (negative, so they never clash with the scene's 0 or a player's index + 1).
/// </para>
/// <para>
/// <b>Sorting depth.</b> A player cube is small, so its centre's depth sorts it well. A collision box can be as big as
/// the cube (a floor, a wall), and sorting it by its centre would often paint it over a player standing on it or next
/// to it. Each box is sorted by its <em>farthest</em> corner instead (<see cref="BoxProjector.FarDepth"/>): a floor or
/// wall then goes down before the small things around it, and a box resting on a bigger one is painted after it. The
/// cost is that a player behind a wall is painted over the wall rather than under it; the boxes are drawn see-through
/// (<see cref="Alpha"/>), so that reads as "behind" anyway, and PROJECT.md says exact per-pixel depth isn't required.
/// </para>
/// </remarks>
public static class CollisionCubes
{
    /// <summary>
    /// The collision colour: a muted grey-blue, unlike every player and NPC colour, so the scenery reads as scenery.
    /// </summary>
    public static readonly Vector3 Rgb = new(0.55f, 0.62f, 0.7f);

    /// <summary>
    /// The boxes' own opacity before the opacity setting is applied: half see-through, so players and boxes behind a
    /// wall still show through it.
    /// </summary>
    public const float Alpha = 0.5f;

    /// <summary>The <see cref="DrawPrimitive.Tag"/> of the faces of the clipped box at an index: −(index + 1).</summary>
    /// <param name="index">The box's index in the clipped list.</param>
    public static int TagFor(int index) => -(index + 1);

    /// <summary>Whether a primitive belongs to a collision box (its tag is negative).</summary>
    /// <param name="tag">A <see cref="DrawPrimitive.Tag"/>.</param>
    public static bool IsCollisionTag(int tag) => tag < 0;

    /// <summary>The clipped-list index of the box a collision tag belongs to; the inverse of <see cref="TagFor"/>.</summary>
    /// <param name="tag">A collision tag.</param>
    public static int IndexOf(int tag) => -tag - 1;

    /// <summary>
    /// Most faces one clipped box can add: its top and four sides. Each edge of a footprint cut from a turned square
    /// faces one of eight ways (either way along the two world axes and the cube's two), each at most once, and at
    /// most two of the four world-axis ways and two of the cube's face the viewer. A plain box adds at most three.
    /// </summary>
    public const int MaxFacesPerBox = 5;

    /// <summary>Adds every clipped box's visible faces, in the collision colour, each sorted by its farthest corner.</summary>
    /// <param name="list">The frame's draw list.</param>
    /// <param name="projector">This frame's box projector.</param>
    /// <param name="boxes">This frame's boxes, already clipped to the cube (world coordinates).</param>
    /// <param name="colour">The packed collision colour, opacity applied.</param>
    public static void Add(DrawList list, in BoxProjector projector, ReadOnlySpan<ClippedBox> boxes, uint colour)
    {
        if (boxes.IsEmpty)
        {
            return;
        }

        var shades = projector.Shades(colour);
        for (var i = 0; i < boxes.Length; i++)
        {
            ref readonly var box = ref boxes[i];
            var centre = box.Bounds.Centre;
            var half = box.Bounds.HalfSize;
            var depth = projector.FarDepth(centre, half);
            if (box.IsRectangle)
            {
                projector.AddFaces(list, projector.Screen(centre), half, depth, shades, TagFor(i));
            }
            else
            {
                projector.AddPrism(list, box, depth, shades, TagFor(i));
            }
        }
    }
}
