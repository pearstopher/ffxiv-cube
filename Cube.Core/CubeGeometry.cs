using System.Numerics;

namespace Cube.Core;

/// <summary>
/// Turns cubes into draw-list primitives: the big cube as a wireframe, and solid cubes (players, later
/// collision boxes) as their visible faces only (PROJECT.md: "Everything it draws is a cube"; "Cheap drawing":
/// only the visible faces of a cube).
/// </summary>
/// <remarks>
/// All cubes are axis-aligned to the world, like the big cube, so they turn with the view. Corners and face
/// normals are rotated into view coordinates with <see cref="CubeSpace.RotateOffset"/> and projected with
/// <see cref="ViewProjection"/>. Everything is computed into locals: no allocation.
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

    /// <summary>
    /// A face counts as visible when its normal points towards the viewer by more than this; faces seen exactly
    /// edge-on are skipped, since they would draw as a line.
    /// </summary>
    private const float VisibleThreshold = 1e-4f;

    /// <summary>The 8 corners of a unit cube, as signs: bit 0 → X, bit 1 → Y, bit 2 → Z.</summary>
    /// <param name="index">Corner index 0–7.</param>
    private static Vector3 CornerSign(int index) =>
        new((index & 1) != 0 ? 1 : -1, (index & 2) != 0 ? 1 : -1, (index & 4) != 0 ? 1 : -1);

    /// <summary>Adds the big cube's 12 edges as lines, each at the depth of its midpoint.</summary>
    /// <param name="list">The frame's draw list.</param>
    /// <param name="space">The cube.</param>
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

                var a = space.RotateOffset(CornerSign(corner) * space.HalfEdge);
                var b = space.RotateOffset(CornerSign(corner | bit) * space.HalfEdge);
                list.AddLine(projection.ToScreen(a), projection.ToScreen(b), projection.Depth((a + b) / 2f), colour);
            }
        }
    }

    /// <summary>
    /// Adds a solid, world-axis-aligned cube as its visible faces (at most three). All faces share the cube
    /// centre's depth: a convex cube's visible faces never overlap each other, and one depth per cube keeps
    /// its faces together in the painter's sort.
    /// </summary>
    /// <param name="list">The frame's draw list.</param>
    /// <param name="space">The big cube, for the view rotation.</param>
    /// <param name="projection">The view's projection.</param>
    /// <param name="viewCentre">The small cube's centre in view coordinates.</param>
    /// <param name="halfSize">Half the small cube's edge, in yalms.</param>
    /// <param name="colour">Base colour; side faces are shaded darker.</param>
    public static void AddSolidCube(DrawList list, in CubeSpace space, in ViewProjection projection, Vector3 viewCentre, float halfSize, uint colour)
    {
        var depth = projection.Depth(viewCentre);
        var towardsViewer = projection.TowardsViewer;

        // The world axes in view coordinates, scaled to half the cube's size.
        var x = space.RotateOffset(Vector3.UnitX) * halfSize;
        var y = space.RotateOffset(Vector3.UnitY) * halfSize;
        var z = space.RotateOffset(Vector3.UnitZ) * halfSize;

        AddFace(list, projection, viewCentre, y, x, z, towardsViewer, depth, Colour.Shade(colour, TopShade));
        AddFace(list, projection, viewCentre, -y, x, z, towardsViewer, depth, Colour.Shade(colour, BottomShade));
        AddFace(list, projection, viewCentre, x, y, z, towardsViewer, depth, Colour.Shade(colour, EastWestShade));
        AddFace(list, projection, viewCentre, -x, y, z, towardsViewer, depth, Colour.Shade(colour, EastWestShade));
        AddFace(list, projection, viewCentre, z, x, y, towardsViewer, depth, Colour.Shade(colour, NorthSouthShade));
        AddFace(list, projection, viewCentre, -z, x, y, towardsViewer, depth, Colour.Shade(colour, NorthSouthShade));
    }

    /// <summary>Adds one face if it faces the viewer.</summary>
    /// <param name="list">The draw list.</param>
    /// <param name="projection">The projection.</param>
    /// <param name="centre">The cube's centre (view coordinates).</param>
    /// <param name="normal">Offset from the cube centre to the face centre; also the face's outward normal.</param>
    /// <param name="u">One in-face half-axis.</param>
    /// <param name="v">The other in-face half-axis.</param>
    /// <param name="towardsViewer">Direction to the viewer.</param>
    /// <param name="depth">Depth to sort the face by.</param>
    /// <param name="colour">Face colour.</param>
    private static void AddFace(DrawList list, in ViewProjection projection, Vector3 centre, Vector3 normal, Vector3 u, Vector3 v, Vector3 towardsViewer, float depth, uint colour)
    {
        if (Vector3.Dot(normal, towardsViewer) <= VisibleThreshold * normal.Length())
        {
            return;
        }

        var faceCentre = centre + normal;
        list.AddQuad(
            projection.ToScreen(faceCentre - u - v),
            projection.ToScreen(faceCentre + u - v),
            projection.ToScreen(faceCentre + u + v),
            projection.ToScreen(faceCentre - u + v),
            depth,
            colour);
    }
}
