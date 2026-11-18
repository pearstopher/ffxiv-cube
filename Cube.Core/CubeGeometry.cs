using System.Numerics;

namespace Cube.Core;

/// <summary>
/// Turns cubes into draw-list primitives: the big cube as a wireframe, and a single solid cube as its visible faces
/// (PROJECT.md: "Everything it draws is a cube"; "Cheap drawing": only the visible faces of a cube). Also holds the
/// face shades every solid box uses.
/// </summary>
/// <remarks>
/// The big cube's wireframe is drawn from fixed cube-local corners, so it never turns on screen whatever the camera
/// does (PROJECT.md: "Cube on screen never turns by itself"). Solid cubes and boxes are world-axis-aligned contents
/// drawn by <see cref="BoxProjector"/>, so they turn with the rest of the contents. Everything is projected with
/// <see cref="ViewProjection"/> and computed into locals: no allocation.
/// </remarks>
public static class CubeGeometry
{
    /// <summary>Brightness of the top face (lit from above).</summary>
    public const float TopShade = 1f;

    /// <summary>Brightness of the side faces running east–west (normals ±Z).</summary>
    public const float NorthSouthShade = 0.75f;

    /// <summary>Brightness of the side faces running north–south (normals ±X).</summary>
    public const float EastWestShade = 0.6f;

    /// <summary>Brightness of the bottom face, seen only if the view were from below.</summary>
    public const float BottomShade = 0.45f;

    /// <summary>The 8 corners of a unit cube, as signs: bit 0 → X, bit 1 → Y, bit 2 → Z.</summary>
    /// <param name="index">Corner index 0–7.</param>
    private static Vector3 CornerSign(int index) =>
        new((index & 1) != 0 ? 1 : -1, (index & 2) != 0 ? 1 : -1, (index & 4) != 0 ? 1 : -1);

    /// <summary>
    /// Adds the big cube's 12 edges as lines, each at the depth of its midpoint. The corners are fixed cube-local
    /// points (± half the size on each axis), independent of <see cref="CubeSpace.Yaw"/>, so turning the camera leaves
    /// these primitives unchanged. A box that isn't a cube is drawn in its real proportions (PROJECT.md, "On screen":
    /// a tall box is drawn tall).
    /// </summary>
    /// <param name="list">The frame's draw list.</param>
    /// <param name="space">The cube, for its size.</param>
    /// <param name="projection">The view's projection.</param>
    /// <param name="colour">Edge colour.</param>
    public static void AddWireCube(DrawList list, in CubeSpace space, in ViewProjection projection, uint colour)
    {
        // Each edge joins two corners that differ in exactly one bit.
        for (var corner = 0; corner < 8; corner++)
        {
            for (var bit = 1; bit <= 4; bit <<= 1)
            {
                if ((corner & bit) != 0)
                {
                    continue;
                }

                var a = CornerSign(corner) * space.Half;
                var b = CornerSign(corner | bit) * space.Half;
                list.AddLine(projection.ToScreen(a), projection.ToScreen(b), projection.Depth((a + b) / 2f), colour);
            }
        }
    }

    /// <summary>
    /// Adds a solid, world-axis-aligned cube as its visible faces (at most three). It is part of the contents, so it
    /// turns with them when the cube's alignment changes. All faces share the cube
    /// centre's depth: a convex cube's visible faces never overlap each other, and one depth per cube keeps
    /// its faces together in the painter's sort.
    /// </summary>
    /// <param name="list">The frame's draw list.</param>
    /// <param name="space">The big cube, for its alignment.</param>
    /// <param name="projection">The view's projection.</param>
    /// <param name="viewCentre">The small cube's centre in view coordinates.</param>
    /// <param name="halfSize">Half the small cube's edge, in yalms.</param>
    /// <param name="colour">Base colour; side faces are shaded darker.</param>
    /// <param name="tag">Owner of the faces (<see cref="DrawPrimitive.Tag"/>).</param>
    /// <remarks>
    /// A one-off convenience over <see cref="BoxProjector"/>; per-frame code that draws many boxes builds one
    /// projector for the frame instead (<see cref="PlayerCubes"/>, <see cref="CollisionCubes"/>).
    /// </remarks>
    public static void AddSolidCube(DrawList list, in CubeSpace space, in ViewProjection projection, Vector3 viewCentre, float halfSize, uint colour, int tag = 0)
    {
        var projector = new BoxProjector(space, projection);
        projector.AddFaces(list, projection.ToScreen(viewCentre), new Vector3(halfSize), projection.Depth(viewCentre), projector.Shades(colour), tag);
    }
}
