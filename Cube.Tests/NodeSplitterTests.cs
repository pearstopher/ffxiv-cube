using System.Numerics;
using Cube.Core;
using Xunit;

namespace Cube.Tests;

/// <summary>
/// Checks the node splitting of #127 against PROJECT.md, "Collision (phase 2)": the drawn boxes are what the player
/// would bump into, so a gap the player could walk through (about 0.5 yalm) must not be covered by a box, and no
/// triangle may ever be left uncovered. Also the bounded cache that keeps the pieces (PROJECT.md, "Bounded memory").
/// </summary>
public class NodeSplitterTests
{
    private static LocalBox Box(float x0, float y0, float z0, float x1, float y1, float z1) =>
        new(new Vector3(x0, y0, z0), new Vector3(x1, y1, z1));

    [Fact]
    public void TwoBlocksWithAGap_AreTwoPieces()
    {
        var triangles = new[] { Box(0, 0, 0, 1, 1, 1), Box(3, 0, 0, 4, 1, 1) };
        var pieces = new LocalBox[8];

        var count = NodeSplitter.Split(triangles, pieces);

        Assert.Equal(2, count);
        Assert.Equal(1f, pieces[0].Max.X);
        Assert.Equal(3f, pieces[1].Min.X);
    }

    [Fact]
    public void CounterWithAnOpening_SeventyEightTriangles_LeavesTheOpeningUncovered()
    {
        // The #129 counter, as measured in game: a node of 78 triangles whose bounds fall in two runs along z,
        // -0.89 to 0.30 and 1.38 to 3.03, with the 1.08-yalm opening between them (the player falls into it).
        var triangles = new LocalBox[78];
        for (var i = 0; i < 38; i++)
        {
            var z = -0.89f + (i * 0.03f);
            triangles[i] = Box(-2.1f, -0.21f, z, -1.1f, 0.78f, Math.Min(z + 0.3f, 0.30f));
        }

        for (var i = 38; i < 78; i++)
        {
            var z = 1.38f + ((i - 38) * 0.04f);
            triangles[i] = Box(-2.1f, -0.21f, z, -1.1f, 0.78f, Math.Min(z + 0.3f, 3.03f));
        }

        var pieces = new LocalBox[NodeSplitter.MaxPiecesPerNode];

        var count = NodeSplitter.Split(triangles, pieces);

        Assert.Equal(2, count);
        Assert.Equal(0.30f, pieces[0].Max.Z, 3);
        Assert.Equal(1.38f, pieces[1].Min.Z, 3);
    }

    [Fact]
    public void TouchingOrOverlappingTriangles_StayTogether()
    {
        var triangles = new[] { Box(0, 0, 0, 1, 1, 1), Box(1, 0, 0, 2, 1, 1), Box(1.5f, 0, 0, 3, 1, 1) };
        var pieces = new LocalBox[8];

        var count = NodeSplitter.Split(triangles, pieces);

        Assert.Equal(1, count);
        Assert.Equal(3f, pieces[0].Max.X);
    }

    [Fact]
    public void GapNarrowerThanAPlayer_IsNotSplit()
    {
        var triangles = new[] { Box(0, 0, 0, 1, 1, 1), Box(1.4f, 0, 0, 2, 1, 1) };

        Assert.Equal(1, NodeSplitter.Split(triangles, new LocalBox[8]));
    }

    [Fact]
    public void GapOnTheVerticalAxis_Splits()
    {
        var triangles = new[] { Box(0, 0, 0, 1, 1, 1), Box(0, 4, 0, 1, 5, 1) };

        Assert.Equal(2, NodeSplitter.Split(triangles, new LocalBox[8]));
    }

    [Fact]
    public void ABigTriangleSpanningTheGap_KeepsEverythingTogether()
    {
        var triangles = new[] { Box(0, 0, 0, 1, 1, 1), Box(3, 0, 0, 4, 1, 1), Box(0, 0, 0, 4, 0.1f, 1) };

        Assert.Equal(1, NodeSplitter.Split(triangles, new LocalBox[8]));
    }

    [Fact]
    public void SingleFlatTriangle_GetsAMinimumThickness()
    {
        var triangles = new[] { Box(0, 2, 0, 4, 2, 4) };
        var pieces = new LocalBox[8];

        var count = NodeSplitter.Split(triangles, pieces);

        Assert.Equal(1, count);
        Assert.Equal(NodeSplitter.MinThickness, pieces[0].Max.Y - pieces[0].Min.Y, 5);
        Assert.Equal(2f, (pieces[0].Min.Y + pieces[0].Max.Y) / 2f, 5);
        Assert.Equal(4f, pieces[0].Max.X - pieces[0].Min.X);
    }

    [Fact]
    public void NoTriangles_GivesNoPieces() =>
        Assert.Equal(0, NodeSplitter.Split(Span<LocalBox>.Empty, new LocalBox[8]));

    [Fact]
    public void ManyScatteredTriangles_RespectTheCapAndStillCoverEveryTriangle()
    {
        var triangles = new LocalBox[20];
        for (var i = 0; i < triangles.Length; i++)
        {
            triangles[i] = Box(i * 3, 0, 0, (i * 3) + 1, 1, 1);
        }

        var original = (LocalBox[])triangles.Clone();
        var pieces = new LocalBox[NodeSplitter.MaxPiecesPerNode];

        var count = NodeSplitter.Split(triangles, pieces);

        Assert.Equal(NodeSplitter.MaxPiecesPerNode, count);
        foreach (var triangle in original)
        {
            Assert.Contains(pieces.Take(count), p => Covers(p, triangle));
        }
    }

    [Fact]
    public void SmallerOutputSpan_LimitsThePieces()
    {
        var triangles = new[] { Box(0, 0, 0, 1, 1, 1), Box(3, 0, 0, 4, 1, 1), Box(6, 0, 0, 7, 1, 1) };
        var pieces = new LocalBox[2];

        var count = NodeSplitter.Split(triangles, pieces);

        Assert.Equal(2, count);
        Assert.Equal(0f, pieces[0].Min.X);
        Assert.True(pieces[1].Max.X >= 7f);
    }

    [Fact]
    public void Splitting_AllocatesNothing()
    {
        var triangles = new LocalBox[12];
        var pieces = new LocalBox[8];
        void Fill()
        {
            for (var i = 0; i < triangles.Length; i++)
            {
                triangles[i] = Box((i % 4) * 2, 0, (i / 4) * 2, ((i % 4) * 2) + 1, 1, ((i / 4) * 2) + 1);
            }
        }

        Fill();
        NodeSplitter.Split(triangles, pieces);
        Fill();

        var bytes = Allocations.Measure(() => NodeSplitter.Split(triangles, pieces));

        Assert.Equal(0, bytes);
    }

    [Fact]
    public void Cache_ReturnsWhatWasStored_AndMissesOnADifferentCheck()
    {
        var cache = new NodePieceCache(10);
        var pieces = new[] { Box(0, 0, 0, 1, 1, 1), Box(3, 0, 0, 4, 1, 1) };

        Assert.True(cache.Add(100, 7, pieces));

        Assert.True(cache.TryGet(100, 7, out var found));
        Assert.Equal(2, found.Length);
        Assert.False(cache.TryGet(100, 8, out _));
        Assert.False(cache.TryGet(101, 7, out _));
    }

    [Fact]
    public void Cache_StoresAnEmptyNode_AsRefinedWithNoPieces()
    {
        var cache = new NodePieceCache(10);

        Assert.True(cache.Add(5, 1, default));

        Assert.True(cache.TryGet(5, 1, out var found));
        Assert.True(found.IsEmpty);
    }

    [Fact]
    public void Cache_IsBounded_AndClearFreesTheRoom()
    {
        var cache = new NodePieceCache(3);
        var two = new[] { Box(0, 0, 0, 1, 1, 1), Box(3, 0, 0, 4, 1, 1) };

        Assert.True(cache.Add(1, 1, two));
        Assert.False(cache.Add(2, 1, two));
        Assert.Equal(2, cache.Pieces);

        cache.Clear();

        Assert.Equal(0, cache.Nodes);
        Assert.True(cache.Add(2, 1, two));
    }

    [Fact]
    public void Cache_LookupAndAdd_AllocateNothing()
    {
        var cache = new NodePieceCache(100);
        var piece = new[] { Box(0, 0, 0, 1, 1, 1) };
        cache.Add(1, 1, piece);
        cache.Clear();
        cache.Add(1, 1, piece);

        var bytes = Allocations.Measure(() =>
        {
            cache.TryGet(1, 1, out _);
            cache.Add(2, 1, piece);
        });

        Assert.Equal(0, bytes);
    }

    private static bool Covers(LocalBox piece, LocalBox triangle) =>
        piece.Min.X <= triangle.Min.X && piece.Min.Y <= triangle.Min.Y && piece.Min.Z <= triangle.Min.Z &&
        piece.Max.X >= triangle.Max.X && piece.Max.Y >= triangle.Max.Y && piece.Max.Z >= triangle.Max.Z;
}
