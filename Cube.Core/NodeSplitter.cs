using System.Numerics;

namespace Cube.Core;

/// <summary>An axis-aligned box in a mesh's own (local) space: a triangle's bounds, or one piece of a split node.</summary>
/// <param name="Min">The corner with the smallest X, Y and Z.</param>
/// <param name="Max">The corner with the largest X, Y and Z.</param>
public readonly record struct LocalBox(Vector3 Min, Vector3 Max);

/// <summary>
/// Splits one collision mesh node's box into pieces wherever its triangles leave a gap the player could walk through
/// (PROJECT.md, "Collision (phase 2)": the boxes are what the player would bump into; #127, from the #125 research in
/// <c>docs/research-notes.md</c>).
/// </summary>
/// <remarks>
/// <para>
/// <b>Why.</b> A mesh leaf node holds only about 9 to 16 triangles, but its box is the tight box around all of them, so
/// when they lie far apart (a counter's two blocks, a room's floor edge and far wall) the box covers the gap between
/// them: a big empty square, or a counter with no opening. Splitting each node's triangles at gaps cut the covered
/// volume in a house to 29% for 14% more boxes.
/// </para>
/// <para>
/// <b>How.</b> Sort the triangles' own bounds along an axis and cut wherever the next one starts at least
/// <see cref="Gap"/> beyond the furthest end so far; every group is again tested along all three axes, and a group that
/// can't be cut becomes one box. <see cref="Gap"/> is 0.5 yalm, about the player's width: a narrower gap can't be walked
/// through, so it needn't be shown. The work is on small spans, with an insertion sort and no allocation, so it can run
/// in the frame (CLAUDE.md, "Efficiency and memory").
/// </para>
/// <para>
/// <b>Never holes.</b> At most <c>maxPieces</c> boxes come out; when the room runs out, the rest of that group is
/// covered by one box (a coarser look, never a missing piece). Every triangle's bounds always lie inside one of the
/// pieces, so the pieces together cover everything the node's own box covered that has a triangle in it.
/// </para>
/// </remarks>
public static class NodeSplitter
{
    /// <summary>The narrowest gap, in yalms, that splits a node: about the player's width.</summary>
    public const float Gap = 0.5f;

    /// <summary>
    /// The thinnest a piece is drawn, in yalms, along each axis. A wall or floor of axis-aligned triangles has no
    /// thickness at all, and a sheet of no thickness would vanish; a few centimetres is enough.
    /// </summary>
    public const float MinThickness = 0.05f;

    /// <summary>Most pieces one node is split into, so a scattered node can't flood the box cap.</summary>
    public const int MaxPiecesPerNode = 8;

    /// <summary>
    /// Splits the triangles' bounds into pieces. Reorders <paramref name="triangles"/> in place (it is the scratch space).
    /// </summary>
    /// <param name="triangles">The bounds of each triangle of one node, in the mesh's local space.</param>
    /// <param name="pieces">Receives the pieces; also limits how many there can be (up to <see cref="MaxPiecesPerNode"/>).</param>
    /// <returns>How many pieces were written; zero only if there were no triangles.</returns>
    public static int Split(Span<LocalBox> triangles, Span<LocalBox> pieces)
    {
        if (triangles.IsEmpty || pieces.IsEmpty)
        {
            return 0;
        }

        var room = Math.Min(pieces.Length, MaxPiecesPerNode);
        var count = SplitGroup(triangles, pieces, 0, room);
        for (var i = 0; i < count; i++)
        {
            pieces[i] = Thicken(pieces[i]);
        }

        return count;
    }

    /// <summary>Splits one group, writing its pieces from <paramref name="start"/>.</summary>
    /// <param name="group">The group's triangle bounds.</param>
    /// <param name="pieces">The output.</param>
    /// <param name="start">Where this group's first piece goes.</param>
    /// <param name="room">How many pieces this group may use.</param>
    /// <returns>How many pieces the group used.</returns>
    private static int SplitGroup(Span<LocalBox> group, Span<LocalBox> pieces, int start, int room)
    {
        if (group.Length > 1 && room > 1 && TryCut(group, out var cut))
        {
            // The first half leaves one piece for the second, which has to be covered somehow.
            var first = SplitGroup(group[..cut], pieces, start, room - 1);
            var second = SplitGroup(group[cut..], pieces, start + first, room - first);
            return first + second;
        }

        pieces[start] = Union(group);
        return 1;
    }

    /// <summary>
    /// Finds an axis along which the group falls into two parts with a gap between them, and leaves the group sorted
    /// along that axis so the parts are <c>group[..cut]</c> and <c>group[cut..]</c>.
    /// </summary>
    /// <param name="group">The group; sorted in place along the axis it cuts on (or the last axis tried).</param>
    /// <param name="cut">The index the second part starts at.</param>
    /// <returns>False if no axis has a gap of at least <see cref="Gap"/>.</returns>
    private static bool TryCut(Span<LocalBox> group, out int cut)
    {
        for (var axis = 0; axis < 3; axis++)
        {
            SortByMin(group, axis);
            var reach = group[0].Max[axis];
            for (var i = 1; i < group.Length; i++)
            {
                if (group[i].Min[axis] - reach >= Gap)
                {
                    cut = i;
                    return true;
                }

                reach = MathF.Max(reach, group[i].Max[axis]);
            }
        }

        cut = 0;
        return false;
    }

    /// <summary>Insertion sort by the low end along an axis; groups are a handful of triangles, so it is the cheapest sort.</summary>
    /// <param name="group">The boxes to sort in place.</param>
    /// <param name="axis">0 for X, 1 for Y, 2 for Z.</param>
    private static void SortByMin(Span<LocalBox> group, int axis)
    {
        for (var i = 1; i < group.Length; i++)
        {
            var item = group[i];
            var key = item.Min[axis];
            var j = i - 1;
            while (j >= 0 && group[j].Min[axis] > key)
            {
                group[j + 1] = group[j];
                j--;
            }

            group[j + 1] = item;
        }
    }

    /// <summary>The smallest box holding every box of a non-empty group.</summary>
    /// <param name="group">The boxes.</param>
    private static LocalBox Union(ReadOnlySpan<LocalBox> group)
    {
        var min = group[0].Min;
        var max = group[0].Max;
        for (var i = 1; i < group.Length; i++)
        {
            min = Vector3.Min(min, group[i].Min);
            max = Vector3.Max(max, group[i].Max);
        }

        return new LocalBox(min, max);
    }

    /// <summary>Grows a box evenly about its centre along any axis thinner than <see cref="MinThickness"/>.</summary>
    /// <param name="box">The piece.</param>
    private static LocalBox Thicken(LocalBox box)
    {
        var size = box.Max - box.Min;
        var grow = Vector3.Max(Vector3.Zero, (new Vector3(MinThickness) - size) * 0.5f);
        return new LocalBox(box.Min - grow, box.Max + grow);
    }
}
