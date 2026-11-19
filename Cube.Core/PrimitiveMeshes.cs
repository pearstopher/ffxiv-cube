using System.Numerics;

namespace Cube.Core;

/// <summary>
/// Turns the game's primitive colliders (box, cylinder, sphere, plane) into triangles for the Mesh style (#100;
/// PROJECT.md, "Collision style": primitives are shown as their real shapes, in triangles).
/// </summary>
/// <remarks>
/// <para>
/// Each primitive is a unit shape placed by its collider's world matrix (<c>docs/research-notes.md</c>, #28): a box
/// from (-1,-1,-1) to (1,1,1), a plane from (-1,-1,0) to (1,1,0), a cylinder of radius 1 from y = -1 to 1, and a sphere
/// of radius 1. A box and a plane are exact; a cylinder and a sphere are curved, so they are drawn as a
/// <see cref="CylinderSides"/>-sided prism and a sphere cut into <see cref="SphereTriangles"/> triangles, which is what
/// any triangle wireframe of a curve is. The matrix uses the <see cref="Vector3.Transform(Vector3, Matrix4x4)"/>
/// convention like <see cref="CollisionBox.FromTransformed"/>. Nothing is allocated: corners live on the stack.
/// </para>
/// </remarks>
public static class PrimitiveMeshes
{
    /// <summary>Sides of a cylinder's prism.</summary>
    public const int CylinderSides = 16;

    /// <summary>Triangles in a box: two per face.</summary>
    public const int BoxTriangles = 12;

    /// <summary>Triangles in a plane: one quad.</summary>
    public const int PlaneTriangles = 2;

    /// <summary>Triangles in a cylinder: two per side wall and one per side for each cap.</summary>
    public const int CylinderTriangles = CylinderSides * 4;

    /// <summary>Triangles in a sphere: an icosahedron's 20 faces, each cut into four.</summary>
    public const int SphereTriangles = 80;

    /// <summary>The icosahedron's corner order, three per face.</summary>
    private static ReadOnlySpan<byte> IcosahedronFaces =>
        [0, 11, 5, 0, 5, 1, 0, 1, 7, 0, 7, 10, 0, 10, 11, 1, 5, 9, 5, 11, 4, 11, 10, 2, 10, 7, 6, 7, 1, 8, 3, 9, 4, 3, 4, 2, 3, 2, 6, 3, 6, 8, 3, 8, 9, 4, 9, 5, 2, 4, 11, 6, 2, 10, 8, 6, 7, 9, 8, 1];

    /// <summary>The box's faces as corner quads; corner bits are X, Y, Z (0 low, 1 high).</summary>
    private static ReadOnlySpan<byte> BoxQuads => [0, 1, 3, 2, 4, 6, 7, 5, 0, 4, 5, 1, 2, 3, 7, 6, 0, 2, 6, 4, 1, 5, 7, 3];

    /// <summary>Adds a box collider's 12 triangles.</summary>
    /// <param name="set">The set to add to.</param>
    /// <param name="world">The collider's world matrix.</param>
    public static void AddBox(TriangleSet set, in Matrix4x4 world)
    {
        Span<Vector3> p = stackalloc Vector3[8];
        for (var i = 0; i < 8; i++)
        {
            p[i] = Vector3.Transform(new Vector3((i & 1) == 0 ? -1 : 1, (i & 2) == 0 ? -1 : 1, (i & 4) == 0 ? -1 : 1), world);
        }

        var quads = BoxQuads;
        for (var q = 0; q < 6; q++)
        {
            set.Offer(p[quads[q * 4]], p[quads[(q * 4) + 1]], p[quads[(q * 4) + 2]]);
            set.Offer(p[quads[q * 4]], p[quads[(q * 4) + 2]], p[quads[(q * 4) + 3]]);
        }
    }

    /// <summary>Adds a plane collider's 2 triangles (a flat square in its own X and Y).</summary>
    /// <param name="set">The set to add to.</param>
    /// <param name="world">The collider's world matrix.</param>
    public static void AddPlane(TriangleSet set, in Matrix4x4 world)
    {
        var a = Vector3.Transform(new Vector3(-1, -1, 0), world);
        var b = Vector3.Transform(new Vector3(1, -1, 0), world);
        var c = Vector3.Transform(new Vector3(1, 1, 0), world);
        var d = Vector3.Transform(new Vector3(-1, 1, 0), world);
        set.Offer(a, b, c);
        set.Offer(a, c, d);
    }

    /// <summary>Adds a cylinder collider's triangles: a prism with a fan on each cap.</summary>
    /// <param name="set">The set to add to.</param>
    /// <param name="world">The collider's world matrix.</param>
    public static void AddCylinder(TriangleSet set, in Matrix4x4 world)
    {
        var top = Vector3.Transform(Vector3.UnitY, world);
        var bottom = Vector3.Transform(-Vector3.UnitY, world);
        var rim = RingPoint(0, 1f, world);
        var rimBottom = RingPoint(0, -1f, world);
        var first = rim;
        var firstBottom = rimBottom;
        for (var i = 1; i <= CylinderSides; i++)
        {
            var next = i == CylinderSides ? first : RingPoint(i, 1f, world);
            var nextBottom = i == CylinderSides ? firstBottom : RingPoint(i, -1f, world);
            set.Offer(rim, rimBottom, nextBottom);
            set.Offer(rim, nextBottom, next);
            set.Offer(top, rim, next);
            set.Offer(bottom, nextBottom, rimBottom);
            rim = next;
            rimBottom = nextBottom;
        }
    }

    /// <summary>Adds a sphere collider's <see cref="SphereTriangles"/> triangles.</summary>
    /// <param name="set">The set to add to.</param>
    /// <param name="world">The collider's world matrix.</param>
    public static void AddSphere(TriangleSet set, in Matrix4x4 world)
    {
        const float T = 1.6180339887f;
        Span<Vector3> v = stackalloc Vector3[12];
        v[0] = new(-1, T, 0);
        v[1] = new(1, T, 0);
        v[2] = new(-1, -T, 0);
        v[3] = new(1, -T, 0);
        v[4] = new(0, -1, T);
        v[5] = new(0, 1, T);
        v[6] = new(0, -1, -T);
        v[7] = new(0, 1, -T);
        v[8] = new(T, 0, -1);
        v[9] = new(T, 0, 1);
        v[10] = new(-T, 0, -1);
        v[11] = new(-T, 0, 1);
        for (var i = 0; i < 12; i++)
        {
            v[i] = Vector3.Normalize(v[i]);
        }

        var faces = IcosahedronFaces;
        for (var f = 0; f < 20; f++)
        {
            var a = v[faces[f * 3]];
            var b = v[faces[(f * 3) + 1]];
            var c = v[faces[(f * 3) + 2]];
            var ab = Vector3.Normalize(a + b);
            var bc = Vector3.Normalize(b + c);
            var ca = Vector3.Normalize(c + a);
            var wa = Vector3.Transform(a, world);
            var wb = Vector3.Transform(b, world);
            var wc = Vector3.Transform(c, world);
            var wab = Vector3.Transform(ab, world);
            var wbc = Vector3.Transform(bc, world);
            var wca = Vector3.Transform(ca, world);
            set.Offer(wa, wab, wca);
            set.Offer(wb, wbc, wab);
            set.Offer(wc, wca, wbc);
            set.Offer(wab, wbc, wca);
        }
    }

    /// <summary>A point on a cylinder's rim, in the world.</summary>
    /// <param name="index">Which of <see cref="CylinderSides"/> points round the rim.</param>
    /// <param name="y">1 for the top rim, -1 for the bottom.</param>
    /// <param name="world">The collider's world matrix.</param>
    private static Vector3 RingPoint(int index, float y, in Matrix4x4 world)
    {
        var angle = index * MathF.Tau / CylinderSides;
        return Vector3.Transform(new Vector3(MathF.Cos(angle), y, MathF.Sin(angle)), world);
    }
}
