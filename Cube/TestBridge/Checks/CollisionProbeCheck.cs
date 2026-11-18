using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Common.Component.BGCollision;
using FFXIVClientStructs.FFXIV.Common.Component.BGCollision.Math;
using Control = FFXIVClientStructs.FFXIV.Client.Game.Control.Control;
using Framework = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework;

namespace Cube.TestBridge.Checks;

/// <summary>
/// Research probe for #28 (where to get collision data): measures, in the running game, the three candidate
/// ways of finding what the player would bump into inside the cube, so <c>docs/research-notes.md</c> can pick
/// one on numbers rather than guesses.
/// </summary>
/// <remarks>
/// <list type="number">
/// <item><b>Collider bounds</b>: walk the collision scene's collider list (as vnavmesh's debug view does) and
/// take each collider's world box. Cheapest, but one box per collider, and terrain colliders are large.</item>
/// <item><b>Mesh node bounds</b>: for mesh colliders crossing the cube, walk the mesh's node tree (each node
/// has a local box and up to 256 vertices) and take the boxes of nodes holding triangles. Finer boxes, still
/// a plain memory read.</item>
/// <item><b>Raycasts</b>: a grid of downward rays through the cube with the game's own
/// <see cref="BGCollisionModule.RaycastMaterialFilter(Vector3, Vector3, out RaycastHit, float)"/>, which gives
/// the floor height under each grid point.</item>
/// <item><b>Coarse voxels</b> (PROJECT.md allows them if boxes are too coarse): every triangle in the crossing
/// nodes marks the 2-yalm cells its bounds touch. Compared with the cells the node boxes would fill, this shows
/// whether node boxes are fine enough to draw.</item>
/// </list>
/// Each is timed per frame and its allocations counted, at the default and the largest cube; the raycast hits
/// are cross-checked against the node boxes so the two methods confirm each other. Counts and times are logged
/// in one <c>Probe #28</c> line. The check fails only when there is nothing to read (not logged in, or no
/// collision scene), so it never blocks a run.
/// Read-only (PROJECT.md, "Read-only game access"): it reads memory and calls only getters and the raycast,
/// which takes the collision module's shared lock itself; nothing is written or hooked.
/// </remarks>
/// <param name="log">Where the findings are logged, at Information so they show without Debug+.</param>
internal sealed class CollisionProbeCheck(IPluginLog log) : BridgeCheck
{
    /// <summary>How long the probe samples, in seconds: enough frames for a stable average at background frame rates.</summary>
    private const double SampleSeconds = 3;

    /// <summary>Raycast grid points per side: 20 × 20 = 400 rays over the largest cube, one every 5 yalms.</summary>
    private const int RayGrid = 20;

    /// <summary>
    /// The layer mask the raycast wrapper passes: layer 0 (the layer movement uses, per vnavmesh's notes); also
    /// used to count the colliders a "walk into" query would see.
    /// </summary>
    private const ulong MovementLayer = 1;

    /// <summary>How far a raycast hit may be outside a node box and still count as inside it (box rounding, compressed vertices).</summary>
    private const float HitTolerance = 0.25f;

    /// <summary>Half the largest cube edge: the worst case for counts and cost.</summary>
    private const float MaxHalfEdge = Core.BoxExtents.Max / 2f;

    /// <summary>Half the default cube edge: what most players will see.</summary>
    private const float DefaultHalfEdge = Core.BoxExtents.Default / 2f;

    /// <summary>Voxel cells per side of the largest cube: 50, so each cell is 2 yalms, about a character's height.</summary>
    private const int VoxelsPerSide = 50;

    /// <summary>Node boxes from the last walk that collected them, for the raycast cross-check; filled once.</summary>
    private readonly List<(Vector3 Min, Vector3 Max)> leafBoxes = [];

    /// <summary>Cells covered by the leaf node boxes: how much of the cube the node boxes would fill if drawn.</summary>
    private readonly VoxelGrid boxCells = new(VoxelsPerSide, Core.BoxExtents.Max);

    /// <summary>Cells touched by an actual triangle's bounds: what a coarse voxel grid would draw.</summary>
    private readonly VoxelGrid triangleCells = new(VoxelsPerSide, Core.BoxExtents.Max);

    /// <inheritdoc/>
    public override string Name => "probe-collision";

    /// <inheritdoc/>
    public override TimeSpan Timeout => TimeSpan.FromSeconds(20);

    /// <inheritdoc/>
    public override IEnumerable<CheckStep> Run(BridgeContext context)
    {
        var first = this.TakeSample(collectLeaves: true);
        if (first.Error is not null)
        {
            yield return CheckStep.Fail(first.Error);
            yield break;
        }

        var hits = this.CrossCheckRaycasts(first.Centre, out var hitsInBoxes, out var hitsInCells, out var hitsByType);

        var frames = 0;
        double collidersMs = 0, nodesMs = 0, raysMs = 0, voxelMs = 0;
        long collidersBytes = 0, nodesBytes = 0, voxelBytes = 0;
        var started = Stopwatch.GetTimestamp();
        while (Stopwatch.GetElapsedTime(started).TotalSeconds < SampleSeconds)
        {
            yield return CheckStep.NextFrame;
            var s = this.TakeSample(collectLeaves: false);
            if (s.Error is not null)
            {
                continue;
            }

            frames++;
            collidersMs += s.CollidersMs;
            nodesMs += s.NodesMs;
            raysMs += s.RaysMs;
            collidersBytes += s.CollidersBytes;
            nodesBytes += s.NodesBytes;
            voxelMs += s.VoxelMs;
            voxelBytes += s.VoxelBytes;
        }

        frames = Math.Max(frames, 1);
        var big = first.Max;
        var small = first.Default;
        context.Metric("collisionFrames", frames);
        context.Metric("collidersTotal", first.Total);
        context.Metric("collidersInMaxCube", big.Colliders);
        context.Metric("collidersInDefaultCube", small.Colliders);
        context.Metric("leafNodesInMaxCube", big.Leaves);
        context.Metric("leafNodesInDefaultCube", small.Leaves);
        context.Metric("trianglesInMaxCube", big.Triangles);
        context.Metric("nodesVisitedMaxCube", big.NodesVisited);
        context.Metric("colliderWalkMs", collidersMs / frames);
        context.Metric("nodeWalkMs", nodesMs / frames);
        context.Metric("raycast400Ms", raysMs / frames);
        context.Metric("colliderWalkBytes", collidersBytes / frames);
        context.Metric("nodeWalkBytes", nodesBytes / frames);
        context.Metric("rayHits", hits);
        context.Metric("rayHitsInNodeBoxes", hitsInBoxes);
        context.Metric("rayHitsInTriangleCells", hitsInCells);
        context.Metric("voxelWalkMs", voxelMs / frames);
        context.Metric("voxelWalkBytes", voxelBytes / frames);
        context.Metric("nodeBoxCells", first.BoxCells);
        context.Metric("triangleCells", first.TriangleCells);
        var cellCount = (double)this.triangleCells.Length;

        log.Information(string.Create(
            CultureInfo.InvariantCulture,
            $"Probe #28: {first.Scenes} scene(s), {first.Total} colliders ({first.TypeCounts}), streaming sphere {first.Streaming}; player at {first.Centre}. " +
            $"Max cube: {big.Colliders} colliders ({big.TypeCounts}), {big.NotRaycastable} not raycast-active, {big.OffLayer0} off layer 0, collider box extent mean {big.MeanExtent:0.#} max {big.MaxExtent:0.#}; " +
            $"{big.Leaves} leaf nodes (extent mean {big.MeanLeafExtent:0.#} max {big.MaxLeafExtent:0.#}), {big.Triangles} triangles, {big.NodesVisited} nodes visited. " +
            $"Default cube: {small.Colliders} colliders, {small.Leaves} leaf nodes, {small.Triangles} triangles. " +
            $"Per frame over {frames} frames: collider walk {collidersMs / frames:0.###} ms {collidersBytes / frames} B, node walk {nodesMs / frames:0.###} ms {nodesBytes / frames} B, {RayGrid * RayGrid} raycasts {raysMs / frames:0.###} ms. " +
            $"Voxels ({VoxelsPerSide}^3, {Core.BoxExtents.Max / VoxelsPerSide:0.#} yalm cells): node boxes fill {first.BoxCells} ({first.BoxCells / cellCount:P1}), triangles touch {first.TriangleCells} ({first.TriangleCells / cellCount:P1}); triangle voxel walk {voxelMs / frames:0.###} ms {voxelBytes / frames} B. " +
            $"Raycasts: {hits}/{RayGrid * RayGrid} hit ({hitsByType}), {hitsInBoxes} inside a leaf node box, {hitsInCells} in a triangle cell."));

        yield return CheckStep.Pass(string.Create(
            CultureInfo.InvariantCulture,
            $"Largest cube: {big.Colliders} colliders, {big.Leaves} leaf nodes, {big.Triangles} triangles; collider walk {collidersMs / frames:0.###} ms, node walk {nodesMs / frames:0.###} ms, voxel walk {voxelMs / frames:0.###} ms, 400 rays {raysMs / frames:0.###} ms; node boxes fill {first.BoxCells / cellCount:P1} of the cube, triangles {first.TriangleCells / cellCount:P1}; ray hits {hitsInBoxes}/{hits} in node boxes, {hitsInCells}/{hits} in triangle cells."));
    }

    /// <inheritdoc/>
    public override void Cleanup(BridgeContext context) => this.leafBoxes.Clear();

    /// <summary>World box of a box, cylinder, sphere or plane collider: the unit shape (-1..1) through its world matrix.</summary>
    /// <param name="world">The collider's world matrix.</param>
    /// <param name="min">The box's low corner.</param>
    /// <param name="max">The box's high corner.</param>
    internal static void UnitShapeBounds(in Matrix4x3 world, out Vector3 min, out Vector3 max) =>
        TransformBox(world, -Vector3.One, Vector3.One, out min, out max);

    /// <summary>
    /// Smallest world-aligned box around a local box after a world transform: the centre goes through the
    /// matrix, and each half-extent axis adds its absolute row to the world half-extent.
    /// </summary>
    /// <param name="world">The transform.</param>
    /// <param name="localMin">The local box's low corner.</param>
    /// <param name="localMax">The local box's high corner.</param>
    /// <param name="min">The world box's low corner.</param>
    /// <param name="max">The world box's high corner.</param>
    internal static void TransformBox(in Matrix4x3 world, Vector3 localMin, Vector3 localMax, out Vector3 min, out Vector3 max)
    {
        var centre = world.TransformCoordinate((localMin + localMax) * 0.5f);
        var half = (localMax - localMin) * 0.5f;
        var extent = (Vector3.Abs(world.Row0) * half.X) + (Vector3.Abs(world.Row1) * half.Y) + (Vector3.Abs(world.Row2) * half.Z);
        min = centre - extent;
        max = centre + extent;
    }

    /// <summary>Whether two boxes overlap (touching counts).</summary>
    /// <param name="aMin">First box's low corner.</param>
    /// <param name="aMax">First box's high corner.</param>
    /// <param name="bMin">Second box's low corner.</param>
    /// <param name="bMax">Second box's high corner.</param>
    internal static bool Overlaps(Vector3 aMin, Vector3 aMax, Vector3 bMin, Vector3 bMax) =>
        aMin.X <= bMax.X && aMax.X >= bMin.X && aMin.Y <= bMax.Y && aMax.Y >= bMin.Y && aMin.Z <= bMax.Z && aMax.Z >= bMin.Z;

    /// <summary>The largest side of a box.</summary>
    /// <param name="min">Low corner.</param>
    /// <param name="max">High corner.</param>
    private static float Extent(Vector3 min, Vector3 max)
    {
        var size = max - min;
        return MathF.Max(size.X, MathF.Max(size.Y, size.Z));
    }

    /// <summary>
    /// One frame of all three methods: the collider walk and node walk at the largest cube (timed, with
    /// allocations), their counts again at the default cube, and the timed raycast grid.
    /// </summary>
    /// <param name="collectLeaves">Whether to keep the largest cube's leaf node boxes for the cross-check (allocates; first frame only).</param>
    private unsafe Sample TakeSample(bool collectLeaves)
    {
        var sample = new Sample();
        var control = Control.Instance();
        if (control == null || control->LocalPlayer == null)
        {
            sample.Error = "Not logged in: no local player to read. Log in and leave the game running.";
            return sample;
        }

        var framework = Framework.Instance();
        var module = framework == null ? null : framework->BGCollisionModule;
        if (module == null || module->SceneManager == null || module->SceneManager->FirstScene == null)
        {
            sample.Error = "No collision scene loaded.";
            return sample;
        }

        var centre = (Vector3)control->LocalPlayer->Position;
        sample.Centre = centre;
        sample.Streaming = module->SceneManager->StreamingSphere;

        var bytes = GC.GetAllocatedBytesForCurrentThread();
        var ticks = Stopwatch.GetTimestamp();
        sample.Max = Walk(module, centre, MaxHalfEdge, nodes: false, ref sample, null);
        sample.CollidersMs = Stopwatch.GetElapsedTime(ticks).TotalMilliseconds;
        sample.CollidersBytes = GC.GetAllocatedBytesForCurrentThread() - bytes;

        bytes = GC.GetAllocatedBytesForCurrentThread();
        ticks = Stopwatch.GetTimestamp();
        var withNodes = Walk(module, centre, MaxHalfEdge, nodes: true, ref sample, null);
        sample.NodesMs = Stopwatch.GetElapsedTime(ticks).TotalMilliseconds;
        sample.NodesBytes = GC.GetAllocatedBytesForCurrentThread() - bytes;
        sample.Max = withNodes;

        sample.Default = Walk(module, centre, DefaultHalfEdge, nodes: true, ref sample, null);

        if (collectLeaves)
        {
            this.leafBoxes.Clear();
            Walk(module, centre, MaxHalfEdge, nodes: true, ref sample, this.leafBoxes);
        }

        bytes = GC.GetAllocatedBytesForCurrentThread();
        ticks = Stopwatch.GetTimestamp();
        this.boxCells.Reset(centre);
        this.triangleCells.Reset(centre);
        Walk(module, centre, MaxHalfEdge, nodes: true, ref sample, null, this.boxCells, this.triangleCells);
        sample.VoxelMs = Stopwatch.GetElapsedTime(ticks).TotalMilliseconds;
        sample.VoxelBytes = GC.GetAllocatedBytesForCurrentThread() - bytes;
        sample.BoxCells = this.boxCells.Count();
        sample.TriangleCells = this.triangleCells.Count();

        ticks = Stopwatch.GetTimestamp();
        for (var i = 0; i < RayGrid * RayGrid; i++)
        {
            BGCollisionModule.RaycastMaterialFilter(RayOrigin(centre, i), -Vector3.UnitY, out _, Core.BoxExtents.Max);
        }

        sample.RaysMs = Stopwatch.GetElapsedTime(ticks).TotalMilliseconds;
        return sample;
    }

    /// <summary>Top of the largest cube above grid point <paramref name="index"/>, spaced evenly over its floor plan.</summary>
    /// <param name="centre">The cube's centre (the player).</param>
    /// <param name="index">Grid point, row-major.</param>
    private static Vector3 RayOrigin(Vector3 centre, int index)
    {
        var step = Core.BoxExtents.Max / RayGrid;
        var x = centre.X - MaxHalfEdge + (step * ((index % RayGrid) + 0.5f));
        var z = centre.Z - MaxHalfEdge + (step * ((index / RayGrid) + 0.5f));
        return new Vector3(x, centre.Y + MaxHalfEdge, z);
    }

    /// <summary>
    /// Walks every collider in every scene, counts those whose world box crosses the cube, and optionally walks
    /// the node tree of each crossing mesh collider.
    /// </summary>
    /// <param name="module">The collision module.</param>
    /// <param name="centre">The cube's centre.</param>
    /// <param name="half">Half the cube's edge.</param>
    /// <param name="nodes">Whether to walk mesh node trees too.</param>
    /// <param name="sample">Scene-wide totals (filled on every walk; cheap).</param>
    /// <param name="leaves">Where to put leaf node boxes, or <see langword="null"/> to keep none.</param>
    /// <param name="boxes">Grid to mark leaf node boxes in, or <see langword="null"/>.</param>
    /// <param name="triangles">Grid to mark each triangle's bounds in, or <see langword="null"/>.</param>
    private static unsafe CubeCounts Walk(BGCollisionModule* module, Vector3 centre, float half, bool nodes, ref Sample sample, List<(Vector3 Min, Vector3 Max)>? leaves, VoxelGrid? boxes = null, VoxelGrid? triangles = null)
    {
        var counts = default(CubeCounts);
        var cubeMin = centre - new Vector3(half);
        var cubeMax = centre + new Vector3(half);
        sample.Scenes = 0;
        sample.Total = 0;
        sample.TypeCounts = default;

        foreach (var wrapper in module->SceneManager->Scenes)
        {
            sample.Scenes++;
            var scene = wrapper->Scene;
            if (scene == null)
            {
                continue;
            }

            foreach (var collider in scene->Colliders)
            {
                sample.Total++;
                var type = collider->GetColliderType();
                sample.TypeCounts.Add(type);

                Vector3 min, max;
                switch (type)
                {
                    case ColliderType.Mesh:
                        var mesh = (ColliderMesh*)collider;
                        min = mesh->WorldBoundingBox.Min;
                        max = mesh->WorldBoundingBox.Max;
                        break;
                    case ColliderType.Box:
                        UnitShapeBounds(((ColliderBox*)collider)->World, out min, out max);
                        break;
                    case ColliderType.Cylinder:
                        UnitShapeBounds(((ColliderCylinder*)collider)->World, out min, out max);
                        break;
                    case ColliderType.Sphere:
                        UnitShapeBounds(((ColliderSphere*)collider)->World, out min, out max);
                        break;
                    case ColliderType.Plane:
                    case ColliderType.PlaneTwoSided:
                        UnitShapeBounds(((ColliderPlane*)collider)->World, out min, out max);
                        break;
                    default:
                        // Streamed colliders only add and remove the terrain mesh colliders, which are in this list themselves.
                        continue;
                }

                if (!Overlaps(min, max, cubeMin, cubeMax))
                {
                    continue;
                }

                counts.Colliders++;
                counts.TypeCounts.Add(type);
                var extent = Extent(min, max);
                counts.ExtentSum += extent;
                counts.MaxExtent = MathF.Max(counts.MaxExtent, extent);
                if ((collider->VisibilityFlags & 1) == 0)
                {
                    counts.NotRaycastable++;
                }

                if ((collider->LayerMask & MovementLayer) == 0)
                {
                    counts.OffLayer0++;
                }

                if (nodes && type == ColliderType.Mesh)
                {
                    var mesh = (ColliderMesh*)collider;
                    if (!mesh->MeshIsSimple && mesh->Mesh != null)
                    {
                        WalkNode(((MeshPCB*)mesh->Mesh)->RootNode, mesh->World, cubeMin, cubeMax, ref counts, leaves, boxes, triangles, 0);
                    }
                }
            }
        }

        return counts;
    }

    /// <summary>
    /// Walks one mesh node and its children, skipping any subtree whose box misses the cube, and counts the
    /// nodes holding triangles (leaves) that cross it.
    /// </summary>
    /// <param name="node">The node, or <see langword="null"/>.</param>
    /// <param name="world">The mesh collider's world matrix.</param>
    /// <param name="cubeMin">The cube's low corner.</param>
    /// <param name="cubeMax">The cube's high corner.</param>
    /// <param name="counts">Totals to add to.</param>
    /// <param name="leaves">Where to put leaf boxes, or <see langword="null"/>.</param>
    /// <param name="boxes">Grid to mark leaf boxes in, or <see langword="null"/>.</param>
    /// <param name="triangles">Grid to mark triangle bounds in, or <see langword="null"/>.</param>
    /// <param name="depth">Recursion depth, capped so a bad pointer can't recurse forever.</param>
    private static unsafe void WalkNode(MeshPCB.FileNode* node, in Matrix4x3 world, Vector3 cubeMin, Vector3 cubeMax, ref CubeCounts counts, List<(Vector3 Min, Vector3 Max)>? leaves, VoxelGrid? boxes, VoxelGrid? triangles, int depth)
    {
        if (node == null || depth > 64)
        {
            return;
        }

        counts.NodesVisited++;
        TransformBox(world, node->LocalBounds.Min, node->LocalBounds.Max, out var min, out var max);
        if (!Overlaps(min, max, cubeMin, cubeMax))
        {
            return;
        }

        if (node->NumPrims > 0)
        {
            counts.Leaves++;
            counts.Triangles += node->NumPrims;
            var extent = Extent(min, max);
            counts.LeafExtentSum += extent;
            counts.MaxLeafExtent = MathF.Max(counts.MaxLeafExtent, extent);
            leaves?.Add((min, max));
            boxes?.MarkBox(min, max);
            if (triangles is not null)
            {
                foreach (ref var prim in node->Primitives)
                {
                    var a = world.TransformCoordinate(node->Vertex(prim.V1));
                    var b = world.TransformCoordinate(node->Vertex(prim.V2));
                    var c = world.TransformCoordinate(node->Vertex(prim.V3));
                    triangles.MarkBox(Vector3.Min(a, Vector3.Min(b, c)), Vector3.Max(a, Vector3.Max(b, c)));
                }
            }
        }

        WalkNode(node->Child1, world, cubeMin, cubeMax, ref counts, leaves, boxes, triangles, depth + 1);
        WalkNode(node->Child2, world, cubeMin, cubeMax, ref counts, leaves, boxes, triangles, depth + 1);
    }

    /// <summary>
    /// Casts the ray grid once more, keeping the hits, and counts how many land inside one of the leaf node
    /// boxes collected on the first frame: if the node walk sees the same geometry as the game's raycast,
    /// nearly all should.
    /// </summary>
    /// <param name="centre">The cube's centre from the first frame.</param>
    /// <param name="inBoxes">Hits inside a leaf node box.</param>
    /// <param name="inCells">Hits in a cell marked by a triangle (the voxel grids must hold the first frame).</param>
    /// <param name="byType">Hits per type of collider hit.</param>
    /// <returns>How many rays hit something.</returns>
    private unsafe int CrossCheckRaycasts(Vector3 centre, out int inBoxes, out int inCells, out TypeCounts byType)
    {
        var hits = 0;
        inBoxes = 0;
        inCells = 0;
        byType = default;
        for (var i = 0; i < RayGrid * RayGrid; i++)
        {
            if (!BGCollisionModule.RaycastMaterialFilter(RayOrigin(centre, i), -Vector3.UnitY, out var hit, Core.BoxExtents.Max))
            {
                continue;
            }

            hits++;
            if (this.triangleCells.IsSet(hit.Point))
            {
                inCells++;
            }

            if (hit.Object != null)
            {
                byType.Add(hit.Object->GetColliderType());
            }

            var tolerance = new Vector3(HitTolerance);
            foreach (var (min, max) in this.leafBoxes)
            {
                if (Overlaps(hit.Point - tolerance, hit.Point + tolerance, min, max))
                {
                    inBoxes++;
                    break;
                }
            }
        }

        return hits;
    }

    /// <summary>Colliders counted by type, without a dictionary so the timed walk allocates nothing.</summary>
    private struct TypeCounts
    {
        /// <summary>Mesh colliders (terrain tiles and placed objects).</summary>
        public int Mesh;

        /// <summary>Box colliders.</summary>
        public int Box;

        /// <summary>Cylinder colliders.</summary>
        public int Cylinder;

        /// <summary>Sphere colliders.</summary>
        public int Sphere;

        /// <summary>One- and two-sided plane colliders.</summary>
        public int Plane;

        /// <summary>Streamed colliders (terrain managers).</summary>
        public int Streamed;

        /// <summary>Counts one collider.</summary>
        /// <param name="type">Its type.</param>
        public void Add(ColliderType type)
        {
            switch (type)
            {
                case ColliderType.Mesh: this.Mesh++; break;
                case ColliderType.Box: this.Box++; break;
                case ColliderType.Cylinder: this.Cylinder++; break;
                case ColliderType.Sphere: this.Sphere++; break;
                case ColliderType.Plane:
                case ColliderType.PlaneTwoSided: this.Plane++; break;
                case ColliderType.Streamed: this.Streamed++; break;
            }
        }

        /// <inheritdoc/>
        public override readonly string ToString() => string.Create(
            CultureInfo.InvariantCulture,
            $"mesh {this.Mesh}, box {this.Box}, cylinder {this.Cylinder}, sphere {this.Sphere}, plane {this.Plane}, streamed {this.Streamed}");
    }

    /// <summary>What one walk found inside one cube size.</summary>
    private struct CubeCounts
    {
        /// <summary>Colliders whose world box crosses the cube.</summary>
        public int Colliders;

        /// <summary>Those colliders by type.</summary>
        public TypeCounts TypeCounts;

        /// <summary>Crossing colliders not active for raycasts (visibility flag 1 clear).</summary>
        public int NotRaycastable;

        /// <summary>Crossing colliders not on layer 0, the layer the raycast uses.</summary>
        public int OffLayer0;

        /// <summary>Sum of the crossing colliders' largest sides, for the mean.</summary>
        public float ExtentSum;

        /// <summary>The largest side of any crossing collider's box.</summary>
        public float MaxExtent;

        /// <summary>Mesh nodes looked at (including those skipped as outside).</summary>
        public int NodesVisited;

        /// <summary>Mesh nodes holding triangles whose box crosses the cube.</summary>
        public int Leaves;

        /// <summary>Triangles in those nodes.</summary>
        public int Triangles;

        /// <summary>Sum of those nodes' largest sides, for the mean.</summary>
        public float LeafExtentSum;

        /// <summary>The largest side of any of those nodes' boxes.</summary>
        public float MaxLeafExtent;

        /// <summary>Gets the mean largest side of the crossing colliders' boxes.</summary>
        public readonly float MeanExtent => this.Colliders == 0 ? 0 : this.ExtentSum / this.Colliders;

        /// <summary>Gets the mean largest side of the crossing leaf nodes' boxes.</summary>
        public readonly float MeanLeafExtent => this.Leaves == 0 ? 0 : this.LeafExtentSum / this.Leaves;
    }

    /// <summary>One frame's measurements.</summary>
    private struct Sample
    {
        /// <summary>Why nothing could be read, or <see langword="null"/>.</summary>
        public string? Error;

        /// <summary>The player's position: the cube's centre.</summary>
        public Vector3 Centre;

        /// <summary>The collision module's streaming sphere (xyz centre, w radius): how far around the camera collision is loaded.</summary>
        public Vector4 Streaming;

        /// <summary>Collision scenes.</summary>
        public int Scenes;

        /// <summary>Colliders in all scenes.</summary>
        public int Total;

        /// <summary>All colliders by type.</summary>
        public TypeCounts TypeCounts;

        /// <summary>Counts at the largest cube.</summary>
        public CubeCounts Max;

        /// <summary>Counts at the default cube.</summary>
        public CubeCounts Default;

        /// <summary>Time for the collider walk alone at the largest cube.</summary>
        public double CollidersMs;

        /// <summary>Bytes allocated by the collider walk.</summary>
        public long CollidersBytes;

        /// <summary>Time for the collider walk plus node walk at the largest cube.</summary>
        public double NodesMs;

        /// <summary>Bytes allocated by the node walk.</summary>
        public long NodesBytes;

        /// <summary>Time for the 400-ray grid.</summary>
        public double RaysMs;

        /// <summary>Time for a full walk that also marks both voxel grids (every triangle in the cube).</summary>
        public double VoxelMs;

        /// <summary>Bytes allocated by the voxel walk.</summary>
        public long VoxelBytes;

        /// <summary>Cells covered by leaf node boxes.</summary>
        public int BoxCells;

        /// <summary>Cells touched by a triangle's bounds.</summary>
        public int TriangleCells;
    }

    /// <summary>
    /// A fixed cube of on/off cells centred on the player, allocated once: the shape a "coarse voxel grid"
    /// (PROJECT.md, "Collision") would take, used here to measure how much of the cube each method fills.
    /// </summary>
    /// <param name="perSide">Cells per side.</param>
    /// <param name="edge">The grid's edge in yalms.</param>
    private sealed class VoxelGrid(int perSide, float edge)
    {
        /// <summary>One flag per cell, x fastest, then z, then y.</summary>
        private readonly bool[] cells = new bool[perSide * perSide * perSide];

        /// <summary>The grid's low corner in world space.</summary>
        private Vector3 origin;

        /// <summary>Gets the number of cells.</summary>
        public int Length => this.cells.Length;

        /// <summary>Clears every cell and centres the grid on a new point.</summary>
        /// <param name="centre">The grid's centre.</param>
        public void Reset(Vector3 centre)
        {
            Array.Clear(this.cells);
            this.origin = centre - new Vector3(edge / 2f);
        }

        /// <summary>Sets every cell a box touches (clipped to the grid).</summary>
        /// <param name="min">The box's low corner.</param>
        /// <param name="max">The box's high corner.</param>
        public void MarkBox(Vector3 min, Vector3 max)
        {
            var scale = perSide / edge;
            var lo = (min - this.origin) * scale;
            var hi = (max - this.origin) * scale;
            int x0 = Clamp(lo.X), y0 = Clamp(lo.Y), z0 = Clamp(lo.Z);
            int x1 = Clamp(hi.X), y1 = Clamp(hi.Y), z1 = Clamp(hi.Z);
            if (hi.X < 0 || hi.Y < 0 || hi.Z < 0 || lo.X >= perSide || lo.Y >= perSide || lo.Z >= perSide)
            {
                return;
            }

            for (var y = y0; y <= y1; y++)
            {
                for (var z = z0; z <= z1; z++)
                {
                    var row = (((y * perSide) + z) * perSide) + x0;
                    Array.Fill(this.cells, true, row, x1 - x0 + 1);
                }
            }
        }

        /// <summary>Whether the cell holding a point is set.</summary>
        /// <param name="point">A world position.</param>
        public bool IsSet(Vector3 point)
        {
            var cell = (point - this.origin) * (perSide / edge);
            if (cell.X < 0 || cell.Y < 0 || cell.Z < 0 || cell.X >= perSide || cell.Y >= perSide || cell.Z >= perSide)
            {
                return false;
            }

            return this.cells[(((((int)cell.Y * perSide) + (int)cell.Z) * perSide) + (int)cell.X)];
        }

        /// <summary>Counts the set cells.</summary>
        public int Count()
        {
            var count = 0;
            foreach (var cell in this.cells)
            {
                if (cell)
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>A cell coordinate clamped into the grid.</summary>
        /// <param name="value">The coordinate in cells.</param>
        private int Clamp(float value) => Math.Clamp((int)MathF.Floor(value), 0, perSide - 1);
    }
}
