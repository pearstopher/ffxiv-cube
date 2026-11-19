using System.Numerics;
using Cube.Core;

namespace Cube.Tests;

/// <summary>
/// Checks the Mesh collision style's rules (PROJECT.md, "Collision style", #100): the real triangles are drawn as they
/// are and exactly clipped to the cube; nothing is drawn outside it; primitive colliders become their real shapes;
/// a scene bigger than the buffer is reported, not silently cut; gathering is budgeted and dropped on a zone change;
/// rebuilding happens only when inputs change; the per-frame path allocates nothing; Boxes stays the default and old
/// settings files keep it.
/// </summary>
public class MeshStyleTests
{
    private static readonly Vector3 Edge = new(40f);
    private const uint Zone = 132;
    private const ulong Stamp = 7;

    /// <summary>A cube 40 yalms across at the origin.</summary>
    /// <param name="yaw">How far the cube is turned, in radians.</param>
    private static CubeSpace Cube(float yaw = 0f) => new(Vector3.Zero, Edge, yaw);

    /// <summary>The default view fitted to a 400 pixel square window.</summary>
    /// <param name="space">The cube.</param>
    private static ViewProjection View(in CubeSpace space) =>
        ViewProjection.Fit(Vector2.Zero, new Vector2(400, 400), space.Half, ViewOrientation.Default);

    /// <summary>A cache holding the given triangles (three corners each), as if gathered around the origin.</summary>
    /// <param name="corners">The triangles' corners, three per triangle.</param>
    private static TriangleCache CacheWith(params Vector3[] corners)
    {
        var cache = new TriangleCache(1000, capacity: 1024);
        var source = new FakeSource(corners);
        Assert.True(cache.Step(ref source, Vector3.Zero, Edge, Zone, Stamp));
        return cache;
    }

    /// <summary>A clipped mesh built in one go from a cache.</summary>
    /// <param name="space">The cube to clip to.</param>
    /// <param name="cache">The gathered triangles.</param>
    /// <param name="capacity">Most edges kept.</param>
    private static ClippedMesh Build(in CubeSpace space, TriangleCache cache, int capacity = 4096)
    {
        var mesh = new ClippedMesh(capacity);
        Assert.True(mesh.Update(space, cache, long.MaxValue));
        return mesh;
    }

    [Fact]
    public void TriangleWhollyInside_IsKeptAsItIs()
    {
        var a = new Vector3(1, 2, 3);
        var b = new Vector3(4, 2, 3);
        var c = new Vector3(1, 5, 3);
        var mesh = Build(Cube(), CacheWith(a, b, c));

        Assert.Equal(3, mesh.Count);
        var space = Cube();
        var corners = new HashSet<Vector3>(mesh.Edges.ToArray());
        Assert.Equal(new HashSet<Vector3> { space.ToView(a), space.ToView(b), space.ToView(c) }, corners);
    }

    [Fact]
    public void TriangleCrossingAFace_IsClippedExactlyAtTheFace()
    {
        // The edge from (0,0,0) to (30,0,0) crosses the +X face at x = 20.
        var mesh = Build(Cube(), CacheWith(new Vector3(0, 0, 0), new Vector3(30, 0, 0), new Vector3(0, 10, 0)));

        Assert.Equal(3, mesh.Count);
        var longest = mesh.Edges.ToArray().Max(p => p.X);
        Assert.Equal(20f, longest, 4);
        foreach (var p in mesh.Edges)
        {
            Assert.InRange(p.X, -20f, 20f);
            Assert.InRange(p.Y, -20f, 20f);
            Assert.InRange(p.Z, -20f, 20f);
        }
    }

    [Fact]
    public void TriangleOutsideTheCube_IsDropped()
    {
        var mesh = Build(Cube(), CacheWith(new Vector3(30, 0, 0), new Vector3(35, 0, 0), new Vector3(30, 5, 0)));

        Assert.Equal(0, mesh.Count);
    }

    [Fact]
    public void EdgeSharedByTwoTriangles_IsKeptOnce()
    {
        var a = new Vector3(0, 0, 0);
        var b = new Vector3(5, 0, 0);
        var c = new Vector3(0, 5, 0);
        var d = new Vector3(5, 5, 0);
        var mesh = Build(Cube(), CacheWith(a, b, c, b, d, c));

        // Two triangles have six edges; the diagonal b-c is shared.
        Assert.Equal(5, mesh.Count);
    }

    [Fact]
    public void SharedEdgeCutByTheCube_IsStillKeptOnce()
    {
        // Both triangles share the edge from (10,0,0) to (30,0,0), which the +X face cuts at x = 20.
        var a = new Vector3(10, 0, 0);
        var b = new Vector3(30, 0, 0);
        var mesh = Build(Cube(), CacheWith(a, b, new Vector3(10, 5, 0), b, a, new Vector3(10, -5, 0)));

        // Two triangles have six edges; a-b is shared and, cut at x = 20 from both sides, matches exactly.
        Assert.Equal(5, mesh.Count);
    }

    [Theory]
    [InlineData(0f)]
    [InlineData(0.5f)]
    [InlineData(1.2f)]
    [InlineData(2.9f)]
    public void TurnedCube_NothingKeptLiesOutsideIt(float yaw)
    {
        var corners = new List<Vector3>();
        var random = new Random(3);
        for (var i = 0; i < 300; i++)
        {
            corners.Add(new Vector3(random.NextSingle() * 50 - 25, random.NextSingle() * 50 - 25, random.NextSingle() * 50 - 25));
        }

        var space = Cube(yaw);
        var mesh = Build(space, CacheWith(corners.ToArray()));

        Assert.True(mesh.Count > 0);
        var half = space.Half;
        foreach (var p in mesh.Edges)
        {
            Assert.InRange(p.X, -half.X, half.X);
            Assert.InRange(p.Y, -half.Y, half.Y);
            Assert.InRange(p.Z, -half.Z, half.Z);
        }
    }

    [Fact]
    public void ProjectedSegments_AreInsideTheCubesOutline()
    {
        var space = Cube(0.7f);
        var view = View(space);
        var mesh = Build(space, CacheWith(new Vector3(-30, 1, 0), new Vector3(30, 1, 0), new Vector3(0, 1, 30)));

        mesh.Project(space, view);

        Assert.True(mesh.ProjectedCount > 0);
        var reach = space.Half.Length() * view.PixelsPerYalm + 0.01f;
        foreach (var p in mesh.Screen)
        {
            Assert.True(Vector2.Distance(p, view.Origin) <= reach);
        }
    }

    [Fact]
    public void StaleResult_IsClippedToTheNewCubeWhenProjected()
    {
        // Built for a cube at the origin, then the player moves 15 yalms: the old edges must not poke out of the new cube.
        var old = Cube();
        var mesh = Build(old, CacheWith(new Vector3(-19, 0, 0), new Vector3(19, 0, 0), new Vector3(0, 19, 0)));
        var moved = new CubeSpace(new Vector3(15, 0, 0), Edge, 0f);
        var view = View(moved);

        mesh.Project(moved, view);

        // The moved cube spans world x -5..35, so the old edges reaching x = -19 must stop at its left face. With the
        // default view's right axis along X, that is the screen position of the cube's own left and right faces.
        Assert.True(mesh.ProjectedCount > 0);
        var left = view.ToScreen(new Vector3(-moved.Half.X, 0, 0)).X;
        var right = view.ToScreen(new Vector3(moved.Half.X, 0, 0)).X;
        foreach (var p in mesh.Screen)
        {
            Assert.InRange(p.X, MathF.Min(left, right) - 0.01f, MathF.Max(left, right) + 0.01f);
        }
    }

    [Fact]
    public void Project_OnlyRunsAgainWhenItsInputsChange()
    {
        var space = Cube();
        var view = View(space);
        var mesh = Build(space, CacheWith(new Vector3(0, 0, 0), new Vector3(5, 0, 0), new Vector3(0, 5, 0)));

        Assert.True(mesh.Project(space, view));
        Assert.False(mesh.Project(space, view));
        Assert.Equal(1, mesh.Projections);
        Assert.True(mesh.Project(Cube(0.3f), view));
    }

    [Fact]
    public void Update_OnlyRebuildsWhenItsInputsChange()
    {
        var cache = CacheWith(new Vector3(0, 0, 0), new Vector3(5, 0, 0), new Vector3(0, 5, 0));
        var mesh = Build(Cube(), cache);

        Assert.False(mesh.Update(Cube(), cache, long.MaxValue));
        Assert.Equal(1, mesh.Builds);
        Assert.True(mesh.Update(Cube(0.4f), cache, long.MaxValue));
        Assert.Equal(2, mesh.Builds);
    }

    [Fact]
    public void Rebuild_IsSpreadOverFramesAndStartedBuildsAlwaysFinish()
    {
        var corners = new List<Vector3>();
        for (var i = 0; i < 2000; i++)
        {
            var x = (i % 30) - 15f;
            corners.Add(new Vector3(x, i / 100f, 0));
            corners.Add(new Vector3(x + 0.5f, i / 100f, 0));
            corners.Add(new Vector3(x, i / 100f + 0.5f, 0));
        }

        var cache = new TriangleCache(1_000_000, capacity: 4096);
        var source = new FakeSource(corners.ToArray());
        Assert.True(cache.Step(ref source, Vector3.Zero, Edge, Zone, Stamp));
        var mesh = new ClippedMesh(8192);

        // A budget of nothing still does one slice (256 triangles) per call, so the build takes several frames, and a
        // changing yaw every frame (a turning camera) doesn't stop it ever finishing.
        var frames = 0;
        var yaw = 0f;
        while (!mesh.Update(Cube(yaw += 0.01f), cache, 0))
        {
            frames++;
            Assert.True(mesh.IsBuilding);
            Assert.True(frames < 50, "build never finished");
        }

        Assert.True(frames >= 2);
        Assert.True(mesh.Count > 0);
    }

    [Fact]
    public void KeptEdgesBeyondTheBuffer_AreCountedNotSilentlyDropped()
    {
        var corners = new List<Vector3>();
        for (var i = 0; i < 40; i++)
        {
            corners.Add(new Vector3(i * 0.3f - 6, 0, 0));
            corners.Add(new Vector3(i * 0.3f - 6, 1, 0));
            corners.Add(new Vector3(i * 0.3f - 6, 0, 1));
        }

        var mesh = Build(Cube(), CacheWith(corners.ToArray()), capacity: 10);

        Assert.Equal(10, mesh.Count);
        Assert.True(mesh.Dropped > 0);
        Assert.Equal(mesh.Dropped, mesh.LargestDropped);
    }

    [Fact]
    public void TrianglesBeyondTheBuffer_AreCountedNotSilentlyDropped()
    {
        var set = new TriangleSet(2);
        set.Begin(Vector3.Zero, Edge, Stamp);

        for (var i = 0; i < 5; i++)
        {
            set.Offer(new Vector3(i, 0, 0), new Vector3(i, 1, 0), new Vector3(i, 0, 1));
        }

        Assert.Equal(2, set.Count);
        Assert.Equal(3, set.Dropped);
        Assert.Equal(5, set.Offered);
    }

    [Fact]
    public void TriangleSet_RejectsTrianglesOutsideTheRegionAndNonFiniteOnes()
    {
        var set = new TriangleSet(8);
        set.Begin(Vector3.Zero, Edge, Stamp);

        Assert.True(set.Offer(Vector3.Zero, Vector3.UnitX, Vector3.UnitY));
        Assert.False(set.Offer(new Vector3(500, 0, 0), new Vector3(501, 0, 0), new Vector3(500, 1, 0)));
        Assert.False(set.Offer(Vector3.Zero, new Vector3(float.NaN, 0, 0), Vector3.UnitY));
        Assert.Equal(1, set.Count);
        Assert.Equal(1, set.Invalid);
    }

    [Fact]
    public void TriangleSet_KeepsATriangleThatOnlyCrossesIntoTheRegion()
    {
        var set = new TriangleSet(8);
        set.Begin(Vector3.Zero, Edge, Stamp);

        Assert.True(set.Offer(new Vector3(-500, 0, 0), new Vector3(500, 0, 0), new Vector3(0, 1, 0)));
    }

    [Fact]
    public void Gather_IsSpreadOverFramesWithinTheBudget_AndFinishes()
    {
        var corners = new List<Vector3>();
        for (var i = 0; i < 10; i++)
        {
            corners.Add(new Vector3(i, 0, 0));
            corners.Add(new Vector3(i, 1, 0));
            corners.Add(new Vector3(i, 0, 1));
        }

        var cache = new TriangleCache(10, capacity: 64);
        var source = new FakeSource(corners.ToArray(), ticksPerCollider: 4);

        var frames = 1;
        while (!cache.Step(ref source, Vector3.Zero, Edge, Zone, Stamp))
        {
            frames++;
            Assert.True(frames < 20);
        }

        Assert.Equal(10, cache.Ready!.Count);
        Assert.True(frames > 1);
    }

    [Fact]
    public void ZoneChange_DropsTheTriangles()
    {
        var cache = CacheWith(new Vector3(0, 0, 0), new Vector3(5, 0, 0), new Vector3(0, 5, 0));
        Assert.Equal(1, cache.Ready!.Count);
        var empty = new FakeSource([]);

        cache.Step(ref empty, Vector3.Zero, Edge, Zone + 1, Stamp);

        Assert.Equal(1, cache.ZoneChanges);
        Assert.Equal(0, cache.Ready!.Count);
    }

    [Fact]
    public void Release_FreesTheBuffers_AndBoxesModeHoldsNothing()
    {
        var cache = CacheWith(new Vector3(0, 0, 0), new Vector3(5, 0, 0), new Vector3(0, 5, 0));
        var mesh = Build(Cube(), cache);
        Assert.True(cache.IsAllocated);
        Assert.True(mesh.IsAllocated);

        cache.Release();
        mesh.Release();

        Assert.False(cache.IsAllocated);
        Assert.False(mesh.IsAllocated);
        Assert.Equal(0, mesh.Count);
    }

    [Fact]
    public void SteadyState_UpdateAndProject_AllocateNothing()
    {
        var space = Cube();
        var view = View(space);
        var cache = CacheWith(new Vector3(0, 0, 0), new Vector3(5, 0, 0), new Vector3(0, 5, 0));
        var mesh = Build(space, cache);
        var yaw = 0f;

        // Rebuilding (a turning camera) and projecting, both after the buffers exist.
        Allocations.AssertNone(() =>
        {
            var turned = new CubeSpace(Vector3.Zero, Edge, yaw += 0.1f);
            mesh.Update(turned, cache, long.MaxValue);
            mesh.Project(turned, view);
        });
    }

    [Fact]
    public void Box_IsTwelveTrianglesOnItsEightCorners()
    {
        var set = new TriangleSet(64);
        set.Begin(Vector3.Zero, Edge, Stamp);

        PrimitiveMeshes.AddBox(set, Matrix4x4.CreateScale(2f, 3f, 4f));

        Assert.Equal(PrimitiveMeshes.BoxTriangles, set.Count);
        Assert.Equal(8, set.Corners.ToArray().Distinct().Count());
        Assert.All(set.Corners.ToArray(), p => Assert.Equal(1f, MathF.Abs(p.X) / 2f));
    }

    [Fact]
    public void Plane_IsTwoTrianglesInItsOwnXAndYPlane()
    {
        var set = new TriangleSet(8);
        set.Begin(Vector3.Zero, Edge, Stamp);

        PrimitiveMeshes.AddPlane(set, Matrix4x4.Identity);

        Assert.Equal(PrimitiveMeshes.PlaneTriangles, set.Count);
        Assert.All(set.Corners.ToArray(), p => Assert.Equal(0f, p.Z));
    }

    [Fact]
    public void Cylinder_HasItsRimOnTheUnitCircleAndCapsAtItsEnds()
    {
        var set = new TriangleSet(256);
        set.Begin(Vector3.Zero, Edge, Stamp);

        PrimitiveMeshes.AddCylinder(set, Matrix4x4.Identity);

        Assert.Equal(PrimitiveMeshes.CylinderTriangles, set.Count);
        foreach (var p in set.Corners)
        {
            Assert.InRange(p.Y, -1f, 1f);
            var radius = MathF.Sqrt((p.X * p.X) + (p.Z * p.Z));
            Assert.True(radius < 1e-5f || MathF.Abs(radius - 1f) < 1e-5f, $"corner {p} is neither on the rim nor an axis point");
        }
    }

    [Fact]
    public void Sphere_HasAllItsCornersOnTheSurface()
    {
        var set = new TriangleSet(256);
        set.Begin(Vector3.Zero, Edge, Stamp);

        PrimitiveMeshes.AddSphere(set, Matrix4x4.CreateScale(3f) * Matrix4x4.CreateTranslation(1, 2, 3));

        Assert.Equal(PrimitiveMeshes.SphereTriangles, set.Count);
        foreach (var p in set.Corners)
        {
            Assert.Equal(3f, Vector3.Distance(p, new Vector3(1, 2, 3)), 3);
        }
    }

    [Fact]
    public void SegmentClip_LeavesAnInsideSegmentUntouched_AndRejectsOutsideOnes()
    {
        var half = new Vector3(10);
        var a = new Vector3(1, 2, 3);
        var b = new Vector3(-4, 5, 6);
        Assert.True(SegmentClip.Clip(ref a, ref b, half));
        Assert.Equal(new Vector3(1, 2, 3), a);
        Assert.Equal(new Vector3(-4, 5, 6), b);

        var c = new Vector3(11, 0, 0);
        var d = new Vector3(12, 5, 0);
        Assert.False(SegmentClip.Clip(ref c, ref d, half));

        // Passes the corner region without entering the box: not rejected by the outcodes alone.
        var e = new Vector3(-20, 15, 0);
        var f = new Vector3(15, 20, 0);
        Assert.False(SegmentClip.Clip(ref e, ref f, half));
    }

    [Fact]
    public void SegmentClip_CutsAtTheFace()
    {
        var a = new Vector3(0, 0, 0);
        var b = new Vector3(30, 0, 0);

        Assert.True(SegmentClip.Clip(ref a, ref b, new Vector3(10)));

        Assert.Equal(new Vector3(0, 0, 0), a);
        Assert.Equal(10f, b.X, 4);
    }

    [Fact]
    public void BoxesIsTheDefaultStyle_AndOldFilesKeepIt()
    {
        var settings = new CubeSettings();
        Assert.Equal(CollisionStyle.Boxes, settings.CollisionStyle);

        settings.CollisionStyle = CollisionStyle.Mesh;
        settings.Upgrade(CubeSettings.PlayerHeightVersion);
        Assert.Equal(CollisionStyle.Boxes, settings.CollisionStyle);

        settings.CollisionStyle = CollisionStyle.Mesh;
        settings.Upgrade(CubeSettings.CollisionStyleVersion);
        Assert.Equal(CollisionStyle.Mesh, settings.CollisionStyle);
    }

    [Fact]
    public void BadStyleValue_IsRepairedToBoxes()
    {
        var settings = new CubeSettings { CollisionStyle = (CollisionStyle)99 };

        settings.Changed();

        Assert.Equal(CollisionStyle.Boxes, settings.CollisionStyle);
    }

    [Fact]
    public void CopyFrom_CopiesTheStyle()
    {
        var settings = new CubeSettings();

        settings.CopyFrom(new CubeSettings { CollisionStyle = CollisionStyle.Mesh });

        Assert.Equal(CollisionStyle.Mesh, settings.CollisionStyle);
    }

    /// <summary>A fake scene: a list of triangles (three corners each); one "collider" per triangle, costing a set number of ticks.</summary>
    /// <param name="corners">The triangles' corners, three per triangle.</param>
    /// <param name="ticksPerCollider">What offering one triangle costs on the fake clock.</param>
    private struct FakeSource(Vector3[] corners, long ticksPerCollider = 1) : ITriangleSource
    {
        private int index;
        private long clock;

        public readonly long Now => this.clock;

        public bool Seek(int at)
        {
            this.index = at;
            return at <= corners.Length / 3;
        }

        public bool OfferNext(TriangleSet set)
        {
            if (this.index >= corners.Length / 3)
            {
                return false;
            }

            var i = this.index++ * 3;
            set.Offer(corners[i], corners[i + 1], corners[i + 2]);
            this.clock += ticksPerCollider;
            return true;
        }
    }
}
