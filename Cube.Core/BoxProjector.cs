using System.Numerics;

namespace Cube.Core;

/// <summary>
/// Projects world-axis-aligned boxes (player cubes, collision boxes) onto the window for one frame, as their visible
/// faces only (PROJECT.md: "Everything it draws is a cube"; "Cheap drawing": only the visible faces of a cube).
/// </summary>
/// <remarks>
/// <para>
/// The projection is orthographic, so it is linear: one yalm along a world axis is always the same step on screen
/// and the same step in depth, wherever the box is. The constructor works those steps out once from the cube's
/// alignment and the view, together with which side of each axis faces the viewer (the same for every box). Each box
/// then costs one centre projection and a few 2D additions, which matters with thousands of collision boxes
/// (PROJECT.md, "Efficiency and memory").
/// </para>
/// <para>
/// Boxes are world-axis-aligned contents: the steps include the cube's alignment, so boxes turn with the rest of the
/// contents when it changes, while the wireframe (<see cref="CubeGeometry.AddWireCube"/>) stays still. A struct built
/// on the stack: nothing is allocated.
/// </para>
/// </remarks>
public readonly struct BoxProjector
{
    /// <summary>
    /// A face counts as visible when its normal points towards the viewer by more than this; faces seen exactly
    /// edge-on are skipped, since they would draw as a line.
    /// </summary>
    private const float VisibleThreshold = 1e-4f;

    /// <summary>The cube's centre in the world.</summary>
    private readonly Vector3 centre;

    /// <summary>The screen position of the cube's centre.</summary>
    private readonly Vector2 origin;

    /// <summary>The screen step of one yalm along world X.</summary>
    private readonly Vector2 stepX;

    /// <summary>The screen step of one yalm along world Y.</summary>
    private readonly Vector2 stepY;

    /// <summary>The screen step of one yalm along world Z.</summary>
    private readonly Vector2 stepZ;

    /// <summary>The depth step of one yalm along each world axis.</summary>
    private readonly Vector3 depthStep;

    /// <summary>Which side of each world axis faces the viewer: +1, −1, or 0 when those faces are seen edge-on.</summary>
    private readonly Vector3 facing;

    /// <summary>Works out the frame's steps and facing.</summary>
    /// <param name="space">The big cube, for its centre and alignment.</param>
    /// <param name="projection">The view's projection.</param>
    public BoxProjector(in CubeSpace space, in ViewProjection projection)
    {
        this.centre = space.Centre;
        this.origin = projection.Origin;
        var x = space.RotateOffset(Vector3.UnitX);
        var y = space.RotateOffset(Vector3.UnitY);
        var z = space.RotateOffset(Vector3.UnitZ);
        this.stepX = projection.ToScreen(x) - this.origin;
        this.stepY = projection.ToScreen(y) - this.origin;
        this.stepZ = projection.ToScreen(z) - this.origin;
        this.depthStep = new Vector3(projection.Depth(x), projection.Depth(y), projection.Depth(z));

        // A face is visible when its outward normal points towards the viewer, i.e. moving along it reduces depth.
        this.facing = new Vector3(Facing(this.depthStep.X), Facing(this.depthStep.Y), Facing(this.depthStep.Z));
    }

    /// <summary>Whether the visible horizontal face is the top (the view is from above or level), not the bottom.</summary>
    public bool SeesTop => this.facing.Y >= 0;

    /// <summary>The screen position of a world point.</summary>
    /// <param name="world">A world point.</param>
    public Vector2 Screen(Vector3 world)
    {
        var offset = world - this.centre;
        return this.origin + (this.stepX * offset.X) + (this.stepY * offset.Y) + (this.stepZ * offset.Z);
    }

    /// <summary>The depth of a world point (<see cref="ViewProjection.Depth"/> of its view position).</summary>
    /// <param name="world">A world point.</param>
    public float Depth(Vector3 world) => Vector3.Dot(world - this.centre, this.depthStep);

    /// <summary>The depth of a box's farthest corner: its centre's depth plus how far each half-size reaches in depth.</summary>
    /// <param name="world">The box's centre in the world.</param>
    /// <param name="half">Half the box's size along the world axes.</param>
    public float FarDepth(Vector3 world, Vector3 half) => this.Depth(world) + Vector3.Dot(half, Vector3.Abs(this.depthStep));

    /// <summary>The three colours a box's visible faces are drawn in, from its base colour.</summary>
    /// <param name="colour">The base colour.</param>
    public BoxShades Shades(uint colour) => new(
        Colour.Shade(colour, this.SeesTop ? CubeGeometry.TopShade : CubeGeometry.BottomShade),
        Colour.Shade(colour, CubeGeometry.EastWestShade),
        Colour.Shade(colour, CubeGeometry.NorthSouthShade));

    /// <summary>
    /// Adds a box's visible faces (at most three), all at one depth. A face with a zero-length side is skipped: a flat
    /// floor shows its top only, and a line-shaped box nothing.
    /// </summary>
    /// <param name="list">The frame's draw list.</param>
    /// <param name="screen">The box centre's screen position (<see cref="Screen"/>).</param>
    /// <param name="half">Half the box's size along the world axes, in yalms.</param>
    /// <param name="depth">The depth all its faces are sorted by.</param>
    /// <param name="shades">The face colours (<see cref="Shades"/>).</param>
    /// <param name="tag">Owner of the faces (<see cref="DrawPrimitive.Tag"/>).</param>
    public void AddFaces(DrawList list, Vector2 screen, Vector3 half, float depth, in BoxShades shades, int tag)
    {
        var hx = this.stepX * half.X;
        var hy = this.stepY * half.Y;
        var hz = this.stepZ * half.Z;
        if (this.facing.Y != 0 && half.X > 0 && half.Z > 0)
        {
            AddFace(list, screen + (hy * this.facing.Y), hx, hz, depth, shades.Horizontal, tag);
        }

        if (this.facing.X != 0 && half.Y > 0 && half.Z > 0)
        {
            AddFace(list, screen + (hx * this.facing.X), hy, hz, depth, shades.EastWest, tag);
        }

        if (this.facing.Z != 0 && half.X > 0 && half.Y > 0)
        {
            AddFace(list, screen + (hz * this.facing.Z), hx, hy, depth, shades.NorthSouth, tag);
        }
    }

    /// <summary>
    /// Adds the visible faces of a collision box clipped to a turned cube (#93): an upright prism on its footprint
    /// polygon (<see cref="ClippedBox"/>). The top (or bottom, seen from below) is one quad or polygon, and each side
    /// whose outward normal faces the viewer is a quad, so nothing reaches past the footprint and so past the cube.
    /// </summary>
    /// <remarks>
    /// Sides are shaded like a box's: the east/west shade when the side faces more east or west than north or south,
    /// otherwise the north/south shade, so a turned side reads as part of the same solid. All faces share one depth,
    /// as a box's do. A zero-height prism (a flat floor) shows its top only. Nothing is allocated: the projected
    /// corners are on the stack.
    /// </remarks>
    /// <param name="list">The frame's draw list.</param>
    /// <param name="box">The clipped box; its footprint corners go counter-clockwise.</param>
    /// <param name="depth">The depth all its faces are sorted by.</param>
    /// <param name="shades">The face colours (<see cref="Shades"/>).</param>
    /// <param name="tag">Owner of the faces (<see cref="DrawPrimitive.Tag"/>).</param>
    public void AddPrism(DrawList list, in ClippedBox box, float depth, in BoxShades shades, int tag)
    {
        var n = box.CornerCount;
        var minY = box.Bounds.Min.Y;
        var rise = this.stepY * (box.Bounds.Max.Y - minY);
        Span<Vector2> bottom = stackalloc Vector2[ClippedBox.MaxCorners];
        for (var i = 0; i < n; i++)
        {
            bottom[i] = this.Screen(box.CornerAt(i, minY));
        }

        if (this.facing.Y != 0)
        {
            Span<Vector2> face = stackalloc Vector2[ClippedBox.MaxCorners];
            var lift = this.facing.Y > 0 ? rise : Vector2.Zero;
            for (var i = 0; i < n; i++)
            {
                face[i] = bottom[i] + lift;
            }

            if (n == 4)
            {
                list.AddQuad(face[0], face[1], face[2], face[3], depth, shades.Horizontal, tag);
            }
            else
            {
                list.AddPolygon(face[..n], depth, shades.Horizontal, tag);
            }
        }

        if (box.Bounds.Max.Y <= minY)
        {
            return;
        }

        for (var i = 0; i < n; i++)
        {
            var j = i + 1 == n ? 0 : i + 1;
            var edge = box.Corner(j) - box.Corner(i);

            // Counter-clockwise corners: the outside is on the right of each edge.
            var normal = new Vector3(edge.Y, 0, -edge.X);
            if (Vector3.Dot(normal, this.depthStep) >= -VisibleThreshold * edge.Length())
            {
                continue;
            }

            var colour = MathF.Abs(normal.X) >= MathF.Abs(normal.Z) ? shades.EastWest : shades.NorthSouth;
            list.AddQuad(bottom[i], bottom[j], bottom[j] + rise, bottom[i] + rise, depth, colour, tag);
        }
    }

    /// <summary>The sign of the face along an axis that faces the viewer, from the axis's depth step.</summary>
    /// <param name="depthStep">How much depth one yalm along the axis adds.</param>
    private static float Facing(float depthStep) =>
        depthStep < -VisibleThreshold ? 1f : depthStep > VisibleThreshold ? -1f : 0f;

    /// <summary>Adds one face as a quad, its corners in order around it.</summary>
    /// <param name="list">The draw list.</param>
    /// <param name="centre">The face's centre on screen.</param>
    /// <param name="u">One in-face half-axis on screen.</param>
    /// <param name="v">The other in-face half-axis on screen.</param>
    /// <param name="depth">The sorting depth.</param>
    /// <param name="colour">The face's colour.</param>
    /// <param name="tag">The owner.</param>
    private static void AddFace(DrawList list, Vector2 centre, Vector2 u, Vector2 v, float depth, uint colour, int tag) =>
        list.AddQuad(centre - u - v, centre + u - v, centre + u + v, centre - u + v, depth, colour, tag);
}

/// <summary>The colours of a box's three visible faces, worked out once per colour per frame (<see cref="BoxProjector.Shades"/>).</summary>
/// <param name="Horizontal">The top face's shade (or the bottom's, seen from below).</param>
/// <param name="EastWest">The shade of the faces whose normals point east or west.</param>
/// <param name="NorthSouth">The shade of the faces whose normals point north or south.</param>
public readonly record struct BoxShades(uint Horizontal, uint EastWest, uint NorthSouth);
