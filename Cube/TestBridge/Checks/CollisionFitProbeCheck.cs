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
/// Research probe for #125: how well do the mesh node boxes match the real collision, and what would match it better?
/// PROJECT.md, "Phase 2", wants the cube to show what the player bumps into, so a box over a walkable gap (a counter
/// with an opening) or over nothing (a big square nobody can touch) misleads.
/// </summary>
/// <remarks>
/// <para>
/// For every blocking mesh node that holds triangles near the player, the probe compares three boxes: the node's own
/// box (what the reader draws today), the tight box around only the node's <b>own</b> triangles, and the boxes around
/// the connected groups of filled 0.5-yalm cells those triangles touch (the "split at gaps" idea). It logs the
/// covered volumes, how many nodes split, the cost of the analysis, and the worst-fitting nodes with a ray test of
/// each (<see cref="CollisionPhantomsCheck.HasHit"/>), in <c>Fit #125</c> lines.
/// </para>
/// <para>
/// The question behind it: an inner tree node that holds triangles of its own gets a box that spans its whole
/// subtree, so two blocks with a gap between them come out as one long box. The probe counts such nodes and measures
/// how much space their boxes waste.
/// </para>
/// <para>
/// Read-only (PROJECT.md, "Read-only game access"): memory reads, getters and the game's raycast. It allocates freely
/// because it is a Debug research probe, not the per-frame path.
/// </para>
/// </remarks>
/// <param name="log">Where the findings are logged, at Information so they show without Debug+.</param>
internal sealed class CollisionFitProbeCheck(IPluginLog log) : BridgeCheck
{
    /// <summary>Half the edge of the region around the player that is examined, in yalms.</summary>
    private const float HalfEdge = 40f;

    /// <summary>Edge of the cells the occupancy grid uses, in yalms.</summary>
    private const float Cell = 0.5f;

    /// <summary>A triangle whose bounds cover more cells than this is counted but not filled in (keeps the probe short).</summary>
    private const int MaxCellsPerTriangle = 20000;

    /// <summary>
    /// The smallest gap, in yalms, the triangle sweep splits at. The player's collision is about 0.5 yalm wide, so a
    /// narrower gap can't be walked through and need not be shown.
    /// </summary>
    private const float MinGap = 0.5f;

    /// <summary>A node whose longest side is at least this (yalms) counts as "big" in the cost-of-refining numbers.</summary>
    private const float BigSide = 3f;

    /// <summary>How many of the worst-fitting nodes are listed.</summary>
    private const int Listed = 15;

    /// <summary>The scan in progress; set by <see cref="Run"/> so the helpers need not pass it along.</summary>
    private Scan scan = new();

    /// <inheritdoc/>
    public override string Name => "probe-collision-fit";

    /// <inheritdoc/>
    public override TimeSpan Timeout => TimeSpan.FromSeconds(60);

    /// <inheritdoc/>
    public override IEnumerable<CheckStep> Run(BridgeContext context)
    {
        var error = this.Examine();
        if (error is not null)
        {
            yield return CheckStep.Fail(error);
            yield break;
        }

        var s = this.scan;
        context.Metric("fitNodes", s.Nodes);
        context.Metric("fitInnerNodesWithTriangles", s.Inner);
        context.Metric("fitNodesThatSplit", s.Split);
        context.Metric("fitNodeVolume", s.NodeVolume);
        context.Metric("fitOwnTightVolume", s.TightVolume);
        context.Metric("fitComponentVolume", s.ComponentVolume);
        context.Metric("fitComponents", s.Components);
        context.Metric("fitAnalysisMs", s.Milliseconds);
        context.Metric("fitSweepBoxes", s.SweepBoxes);
        context.Metric("fitSweepVolume", s.SweepVolume);
        context.Metric("fitSweepMs", s.SweepMilliseconds);
        foreach (var line in s.Lines)
        {
            log.Information(line);
        }

        yield return CheckStep.Pass(string.Create(
            CultureInfo.InvariantCulture,
            $"{s.Nodes} nodes ({s.Inner} inner with own triangles); volume drawn today {s.NodeVolume:0}, tight box of own triangles {s.TightVolume:0}, split at gaps {s.ComponentVolume:0} ({s.Components} boxes from {s.Nodes}); {s.Milliseconds:0.0} ms."));
    }

    /// <summary>Walks every blocking mesh near the player and collects the findings.</summary>
    /// <returns>An error message, or <see langword="null"/>.</returns>
    private unsafe string? Examine()
    {
        this.scan = new Scan();
        var control = Control.Instance();
        if (control == null || control->LocalPlayer == null)
        {
            return "Not logged in: no local player. Log in and leave the game running.";
        }

        var framework = Framework.Instance();
        var module = framework == null ? null : framework->BGCollisionModule;
        if (module == null || module->SceneManager == null || module->SceneManager->FirstScene == null
            || module->SceneManager->FirstScene->Scene == null)
        {
            return "No collision scene loaded.";
        }

        var s = this.scan;
        var centre = (Vector3)control->LocalPlayer->Position;
        s.Centre = centre;
        s.AreaMin = centre - new Vector3(HalfEdge);
        s.AreaMax = centre + new Vector3(HalfEdge);
        var watch = Stopwatch.StartNew();
        foreach (var collider in module->SceneManager->FirstScene->Scene->Colliders)
        {
            if (collider->GetColliderType() != ColliderType.Mesh
                || !Core.CollisionFilter.Blocks(new Core.ColliderFields(collider->VisibilityFlags, collider->LayerMask, collider->ObjectMaterialValue, false)))
            {
                continue;
            }

            var mesh = (ColliderMesh*)collider;
            if (mesh->MeshIsSimple || mesh->Mesh == null
                || !CollisionProbeCheck.Overlaps(mesh->WorldBoundingBox.Min, mesh->WorldBoundingBox.Max, s.AreaMin, s.AreaMax))
            {
                continue;
            }

            s.Meshes++;
            this.WalkNode(((MeshPCB*)mesh->Mesh)->RootNode, mesh->World, collider, 0);
        }

        watch.Stop();
        s.Milliseconds = watch.Elapsed.TotalMilliseconds;
        s.Lines.Add(string.Create(
            CultureInfo.InvariantCulture,
            $"Fit #125: player at {centre}; {s.Meshes} blocking meshes within {HalfEdge} yalms, {s.Nodes} nodes with triangles ({s.Leaf} leaves, {s.Inner} inner nodes with own triangles), {s.Triangles} own triangles ({s.SkippedTriangles} too large to fill). Analysis took {s.Milliseconds:0.0} ms."));
        s.Lines.Add(string.Create(
            CultureInfo.InvariantCulture,
            $"Fit #125: volume (yalms^3) drawn today {s.NodeVolume:0}; tight box of the node's own triangles {s.TightVolume:0} ({100 * s.TightVolume / Math.Max(s.NodeVolume, 1):0}%); boxes split at gaps {s.ComponentVolume:0} ({100 * s.ComponentVolume / Math.Max(s.NodeVolume, 1):0}%), {s.Components} boxes for {s.Nodes} nodes, {s.Split} nodes split."));
        s.Lines.Add(string.Create(
            CultureInfo.InvariantCulture,
            $"Fit #125: inner nodes only: drawn {s.InnerNodeVolume:0}, tight {s.InnerTightVolume:0} ({100 * s.InnerTightVolume / Math.Max(s.InnerNodeVolume, 1):0}%), split at gaps {s.InnerComponentVolume:0} ({100 * s.InnerComponentVolume / Math.Max(s.InnerNodeVolume, 1):0}%); {s.InnerEqualsChildren} of {s.Inner} have a box no bigger than their children's boxes together."));
        s.Lines.Add(string.Create(
            CultureInfo.InvariantCulture,
            $"Fit #125: triangle sweep (split at gaps of {MinGap} yalms): {s.SweepBoxes} boxes for {s.Nodes} nodes ({s.SweepSplit} nodes split), volume {s.SweepVolume:0} ({100 * s.SweepVolume / Math.Max(s.NodeVolume, 1):0.00}% of drawn); sweep time {s.SweepMilliseconds:0.0} ms for {s.Triangles} triangles. Nodes with a side >= {BigSide}: {s.BigNodes} nodes, {s.BigTriangles} triangles, volume {s.BigNodeVolume:0} -> {s.BigSweepVolume:0}, {s.BigSweepBoxes} boxes, {s.BigSweepMilliseconds:0.0} ms. Ray hits inside sweep boxes of the wasteful nodes: {s.SweepRayHits} of {s.SweepRayTested}."));
        s.Lines.Add($"Fit #125: share of nodes by (split-at-gap volume / drawn volume): {string.Join(", ", HistogramText(s.Histogram))}");
        s.Lines.Add($"Fit #125: share of nodes with a ray hit inside their box (3x3 down, 8 in from the sides): {s.RayHits} of {s.RayTested} tested (the rest are possible phantoms).");
        s.Worst.Sort((a, b) => b.Waste.CompareTo(a.Waste));
        for (var i = 0; i < Math.Min(Listed, s.Worst.Count); i++)
        {
            s.Lines.Add($"Fit #125 worst {i + 1}: {s.Worst[i].Text}");
        }

        return null;
    }

    /// <summary>Formats the ratio histogram as "label count" pairs.</summary>
    /// <param name="histogram">Counts per bucket.</param>
    private static IEnumerable<string> HistogramText(int[] histogram)
    {
        string[] labels = ["<5%", "<10%", "<25%", "<50%", "<75%", "<100%", "100%+"];
        for (var i = 0; i < histogram.Length; i++)
        {
            yield return $"{labels[i]}: {histogram[i]}";
        }
    }

    /// <summary>Analyses one node with triangles, then its children (skipping subtrees outside the region).</summary>
    /// <param name="node">The node, or null.</param>
    /// <param name="world">The mesh's world matrix.</param>
    /// <param name="collider">The mesh collider.</param>
    /// <param name="depth">Recursion depth, capped against a bad pointer.</param>
    private unsafe void WalkNode(MeshPCB.FileNode* node, in Matrix4x3 world, Collider* collider, int depth)
    {
        if (node == null || depth > 64)
        {
            return;
        }

        var s = this.scan;
        CollisionProbeCheck.TransformBox(world, node->LocalBounds.Min, node->LocalBounds.Max, out var min, out var max);
        if (!CollisionProbeCheck.Overlaps(min, max, s.AreaMin, s.AreaMax))
        {
            return;
        }

        if (node->NumPrims > 0)
        {
            this.AnalyseNode(node, world, collider, depth, min, max);
        }

        this.WalkNode(node->Child1, world, collider, depth + 1);
        this.WalkNode(node->Child2, world, collider, depth + 1);
    }

    /// <summary>Measures one node's box against its own triangles.</summary>
    /// <param name="node">The node (it has triangles).</param>
    /// <param name="world">The mesh's world matrix.</param>
    /// <param name="collider">The mesh collider.</param>
    /// <param name="depth">The node's depth in its tree.</param>
    /// <param name="min">The world box of the node's local bounds, low corner.</param>
    /// <param name="max">The world box of the node's local bounds, high corner.</param>
    private unsafe void AnalyseNode(MeshPCB.FileNode* node, in Matrix4x3 world, Collider* collider, int depth, Vector3 min, Vector3 max)
    {
        var s = this.scan;
        var inner = node->Child1 != null || node->Child2 != null;
        var tightMin = new Vector3(float.MaxValue);
        var tightMax = new Vector3(float.MinValue);
        var cells = new HashSet<long>();
        var triangles = new List<(Vector3 Lo, Vector3 Hi)>(node->NumPrims);
        foreach (ref var prim in node->Primitives)
        {
            var a = world.TransformCoordinate(node->Vertex(prim.V1));
            var b = world.TransformCoordinate(node->Vertex(prim.V2));
            var c = world.TransformCoordinate(node->Vertex(prim.V3));
            var lo = Vector3.Min(a, Vector3.Min(b, c));
            var hi = Vector3.Max(a, Vector3.Max(b, c));
            tightMin = Vector3.Min(tightMin, lo);
            tightMax = Vector3.Max(tightMax, hi);
            triangles.Add((lo, hi));
            s.Triangles++;
            if (!AddCells(cells, lo, hi))
            {
                s.SkippedTriangles++;
            }
        }

        var nodeVolume = Volume(min, max);
        var tightVolume = Volume(tightMin, tightMax);

        // Candidate B: split the node's triangle bounds wherever a gap the player could fit through separates them.
        var sweepWatch = Stopwatch.StartNew();
        var sweepBoxes = new List<(Vector3 Lo, Vector3 Hi)>();
        Sweep(triangles, sweepBoxes, 0);
        sweepWatch.Stop();
        s.SweepMilliseconds += sweepWatch.Elapsed.TotalMilliseconds;
        var sweepVolume = 0f;
        foreach (var box in sweepBoxes)
        {
            sweepVolume += Volume(box.Lo, box.Hi);
        }

        s.SweepBoxes += sweepBoxes.Count;
        s.SweepVolume += sweepVolume;
        if (sweepBoxes.Count > 1)
        {
            s.SweepSplit++;
        }

        var side = Math.Max(max.X - min.X, Math.Max(max.Y - min.Y, max.Z - min.Z));
        if (side >= BigSide)
        {
            s.BigNodes++;
            s.BigTriangles += triangles.Count;
            s.BigNodeVolume += nodeVolume;
            s.BigSweepVolume += sweepVolume;
            s.BigSweepBoxes += sweepBoxes.Count;
            s.BigSweepMilliseconds += sweepWatch.Elapsed.TotalMilliseconds;
        }

        var components = Components(cells, out var componentVolume, out var largest);
        s.Nodes++;
        s.NodeVolume += nodeVolume;
        s.TightVolume += tightVolume;
        s.ComponentVolume += componentVolume;
        s.Components += components;
        if (components > 1)
        {
            s.Split++;
        }

        if (inner)
        {
            s.Inner++;
            s.InnerNodeVolume += nodeVolume;
            s.InnerTightVolume += tightVolume;
            s.InnerComponentVolume += componentVolume;
            var childVolume = ChildVolume(node->Child1, world) + ChildVolume(node->Child2, world);
            if (nodeVolume <= childVolume * 1.05f)
            {
                s.InnerEqualsChildren++;
            }
        }
        else
        {
            s.Leaf++;
        }

        var ratio = componentVolume / Math.Max(nodeVolume, 1e-6f);
        s.Histogram[ratio < 0.05f ? 0 : ratio < 0.1f ? 1 : ratio < 0.25f ? 2 : ratio < 0.5f ? 3 : ratio < 0.75f ? 4 : ratio < 1f ? 5 : 6]++;

        // A ray test of every node would be many thousands of raycasts: test only the large boxes, where phantoms show.
        var waste = nodeVolume - componentVolume;
        if (waste > 2f)
        {
            var hit = CollisionPhantomsCheck.HasHitIn(min, max, collider);
            s.RayTested++;
            if (hit)
            {
                s.RayHits++;
            }

            var sweepHits = 0;
            foreach (var box in sweepBoxes)
            {
                if (CollisionPhantomsCheck.HasHitIn(box.Lo, box.Hi, collider))
                {
                    sweepHits++;
                }
            }

            s.SweepRayTested += sweepBoxes.Count;
            s.SweepRayHits += sweepHits;
            var size = max - min;
            var distance = Vector3.Distance((min + max) * 0.5f, s.Centre);
            s.Worst.Add((waste, string.Create(
                CultureInfo.InvariantCulture,
                $"{distance:0.0} yalms away, box {min} to {max} (size {size.X:0.0} x {size.Y:0.0} x {size.Z:0.0}), depth {depth}, {(inner ? "inner node" : "leaf")}, {node->NumPrims} own triangles, tight box {(tightMax.X - tightMin.X):0.0} x {(tightMax.Y - tightMin.Y):0.0} x {(tightMax.Z - tightMin.Z):0.0}, {components} gap-split cell groups (largest {largest:0} yalms^3), volume {nodeVolume:0} -> {componentVolume:0}; triangle sweep: {sweepBoxes.Count} boxes, volume {sweepVolume:0}, {sweepHits} with a ray hit; whole box ray hit inside: {hit}, layout id {collider->LayoutObjectId:X}")));
        }
    }

    /// <summary>
    /// Splits triangle bounds into groups wherever a gap of at least <see cref="MinGap"/> separates them along an axis,
    /// recursively, and adds the group boxes to <paramref name="output"/>: one box per group of triangles that the
    /// player could not squeeze between. A group that doesn't split on any axis is one box around its triangles.
    /// </summary>
    /// <param name="triangles">Triangle bounds (sorted in place).</param>
    /// <param name="output">The boxes found.</param>
    /// <param name="depth">Recursion depth, capped.</param>
    private static void Sweep(List<(Vector3 Lo, Vector3 Hi)> triangles, List<(Vector3 Lo, Vector3 Hi)> output, int depth)
    {
        if (triangles.Count > 1 && depth < 8)
        {
            for (var axis = 0; axis < 3; axis++)
            {
                var a = axis;
                triangles.Sort((p, q) => Component(p.Lo, a).CompareTo(Component(q.Lo, a)));
                var start = 0;
                var reach = Component(triangles[0].Hi, a);
                var groups = new List<List<(Vector3 Lo, Vector3 Hi)>>();
                for (var i = 1; i < triangles.Count; i++)
                {
                    if (Component(triangles[i].Lo, a) > reach + MinGap)
                    {
                        groups.Add(triangles.GetRange(start, i - start));
                        start = i;
                        reach = float.MinValue;
                    }

                    reach = Math.Max(reach, Component(triangles[i].Hi, a));
                }

                if (groups.Count > 0)
                {
                    groups.Add(triangles.GetRange(start, triangles.Count - start));
                    foreach (var group in groups)
                    {
                        Sweep(group, output, depth + 1);
                    }

                    return;
                }
            }
        }

        var lo = new Vector3(float.MaxValue);
        var hi = new Vector3(float.MinValue);
        foreach (var t in triangles)
        {
            lo = Vector3.Min(lo, t.Lo);
            hi = Vector3.Max(hi, t.Hi);
        }

        output.Add((lo, hi));
    }

    /// <summary>One component of a vector by axis number.</summary>
    /// <param name="v">The vector.</param>
    /// <param name="axis">0 for X, 1 for Y, 2 for Z.</param>
    private static float Component(Vector3 v, int axis) => axis == 0 ? v.X : axis == 1 ? v.Y : v.Z;

    /// <summary>The volume of a child node's world box, or 0 for no child.</summary>
    /// <param name="child">The child node, or null.</param>
    /// <param name="world">The mesh's world matrix.</param>
    private static unsafe float ChildVolume(MeshPCB.FileNode* child, in Matrix4x3 world)
    {
        if (child == null)
        {
            return 0;
        }

        CollisionProbeCheck.TransformBox(world, child->LocalBounds.Min, child->LocalBounds.Max, out var min, out var max);
        return Volume(min, max);
    }

    /// <summary>Volume of a box, or 0 if it is empty or inverted.</summary>
    /// <param name="min">Low corner.</param>
    /// <param name="max">High corner.</param>
    private static float Volume(Vector3 min, Vector3 max)
    {
        var d = max - min;
        return d.X <= 0 || d.Y <= 0 || d.Z <= 0 ? 0 : d.X * d.Y * d.Z;
    }

    /// <summary>Packs a cell's integer coordinates into one key.</summary>
    /// <param name="x">Cell X.</param>
    /// <param name="y">Cell Y.</param>
    /// <param name="z">Cell Z.</param>
    private static long Key(int x, int y, int z) => ((long)(x + 0x100000) << 42) | ((long)(y + 0x100000) << 21) | (long)(z + 0x100000);

    /// <summary>Marks every cell a triangle's bounds touch.</summary>
    /// <param name="cells">The occupied cells.</param>
    /// <param name="lo">Low corner of the triangle's bounds.</param>
    /// <param name="hi">High corner of the triangle's bounds.</param>
    /// <returns>False if the triangle was too large to fill in.</returns>
    private static bool AddCells(HashSet<long> cells, Vector3 lo, Vector3 hi)
    {
        var x0 = (int)MathF.Floor(lo.X / Cell);
        var y0 = (int)MathF.Floor(lo.Y / Cell);
        var z0 = (int)MathF.Floor(lo.Z / Cell);
        var x1 = (int)MathF.Floor(hi.X / Cell);
        var y1 = (int)MathF.Floor(hi.Y / Cell);
        var z1 = (int)MathF.Floor(hi.Z / Cell);
        if ((long)(x1 - x0 + 1) * (y1 - y0 + 1) * (z1 - z0 + 1) > MaxCellsPerTriangle)
        {
            return false;
        }

        for (var x = x0; x <= x1; x++)
        {
            for (var y = y0; y <= y1; y++)
            {
                for (var z = z0; z <= z1; z++)
                {
                    cells.Add(Key(x, y, z));
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Splits the filled cells into groups that touch face to face and adds up the volume of each group's bounding box.
    /// </summary>
    /// <param name="cells">The occupied cells (consumed).</param>
    /// <param name="volume">Out: total volume of the groups' boxes.</param>
    /// <param name="largest">Out: volume of the largest group's box.</param>
    /// <returns>The number of groups.</returns>
    private static int Components(HashSet<long> cells, out float volume, out float largest)
    {
        volume = 0;
        largest = 0;
        var count = 0;
        var stack = new Stack<long>();
        while (cells.Count > 0)
        {
            long start = 0;
            foreach (var k in cells)
            {
                start = k;
                break;
            }

            cells.Remove(start);
            stack.Push(start);
            var lo = new int[] { int.MaxValue, int.MaxValue, int.MaxValue };
            var hi = new int[] { int.MinValue, int.MinValue, int.MinValue };
            while (stack.Count > 0)
            {
                var k = stack.Pop();
                var x = (int)((k >> 42) & 0x1FFFFF) - 0x100000;
                var y = (int)((k >> 21) & 0x1FFFFF) - 0x100000;
                var z = (int)(k & 0x1FFFFF) - 0x100000;
                lo[0] = Math.Min(lo[0], x);
                lo[1] = Math.Min(lo[1], y);
                lo[2] = Math.Min(lo[2], z);
                hi[0] = Math.Max(hi[0], x);
                hi[1] = Math.Max(hi[1], y);
                hi[2] = Math.Max(hi[2], z);
                Visit(cells, stack, x + 1, y, z);
                Visit(cells, stack, x - 1, y, z);
                Visit(cells, stack, x, y + 1, z);
                Visit(cells, stack, x, y - 1, z);
                Visit(cells, stack, x, y, z + 1);
                Visit(cells, stack, x, y, z - 1);
            }

            var v = (hi[0] - lo[0] + 1) * (hi[1] - lo[1] + 1) * (hi[2] - lo[2] + 1) * Cell * Cell * Cell;
            volume += v;
            largest = Math.Max(largest, v);
            count++;
        }

        return count;
    }

    /// <summary>Moves a neighbouring cell from the unvisited set to the stack, if it is filled.</summary>
    /// <param name="cells">The unvisited filled cells.</param>
    /// <param name="stack">The cells still to expand.</param>
    /// <param name="x">Cell X.</param>
    /// <param name="y">Cell Y.</param>
    /// <param name="z">Cell Z.</param>
    private static void Visit(HashSet<long> cells, Stack<long> stack, int x, int y, int z)
    {
        var key = Key(x, y, z);
        if (cells.Remove(key))
        {
            stack.Push(key);
        }
    }

    /// <summary>What <see cref="Examine"/> found.</summary>
    private sealed class Scan
    {
        /// <summary>The player's position.</summary>
        public Vector3 Centre;

        /// <summary>Low corner of the examined region.</summary>
        public Vector3 AreaMin;

        /// <summary>High corner of the examined region.</summary>
        public Vector3 AreaMax;

        /// <summary>Blocking meshes in the region.</summary>
        public int Meshes;

        /// <summary>Nodes with triangles analysed.</summary>
        public int Nodes;

        /// <summary>Of those, nodes with no children.</summary>
        public int Leaf;

        /// <summary>Of those, nodes that also have children.</summary>
        public int Inner;

        /// <summary>Inner nodes whose box is no bigger than their children's boxes together.</summary>
        public int InnerEqualsChildren;

        /// <summary>Nodes whose triangles fall into more than one group of cells.</summary>
        public int Split;

        /// <summary>Boxes if every node were split at gaps.</summary>
        public int Components;

        /// <summary>Own triangles read.</summary>
        public int Triangles;

        /// <summary>Triangles too large to fill in.</summary>
        public int SkippedTriangles;

        /// <summary>Total volume of the node boxes drawn today.</summary>
        public float NodeVolume;

        /// <summary>Total volume of the tight boxes of own triangles.</summary>
        public float TightVolume;

        /// <summary>Total volume of the gap-split boxes.</summary>
        public float ComponentVolume;

        /// <summary>Node-box volume of inner nodes only.</summary>
        public float InnerNodeVolume;

        /// <summary>Tight-box volume of inner nodes only.</summary>
        public float InnerTightVolume;

        /// <summary>Gap-split volume of inner nodes only.</summary>
        public float InnerComponentVolume;

        /// <summary>Nodes per band of (gap-split volume / node volume).</summary>
        public int[] Histogram = new int[7];

        /// <summary>Wasteful boxes ray-tested.</summary>
        public int RayTested;

        /// <summary>Of those, boxes with a ray hit inside.</summary>
        public int RayHits;

        /// <summary>Boxes from the triangle sweep over all nodes.</summary>
        public int SweepBoxes;

        /// <summary>Nodes the sweep split into more than one box.</summary>
        public int SweepSplit;

        /// <summary>Total volume of the sweep's boxes.</summary>
        public float SweepVolume;

        /// <summary>Time spent in the sweep itself (Debug build, allocating: an upper bound).</summary>
        public double SweepMilliseconds;

        /// <summary>Nodes with a longest side of at least 3 yalms.</summary>
        public int BigNodes;

        /// <summary>Triangles in those nodes.</summary>
        public int BigTriangles;

        /// <summary>Their drawn volume.</summary>
        public float BigNodeVolume;

        /// <summary>Their volume after the sweep.</summary>
        public float BigSweepVolume;

        /// <summary>Their box count after the sweep.</summary>
        public int BigSweepBoxes;

        /// <summary>Sweep time spent on those nodes.</summary>
        public double BigSweepMilliseconds;

        /// <summary>Sweep boxes of the wasteful nodes that were ray-tested.</summary>
        public int SweepRayTested;

        /// <summary>Of those, boxes with a ray hit inside.</summary>
        public int SweepRayHits;

        /// <summary>Wall-clock time of the whole analysis.</summary>
        public double Milliseconds;

        /// <summary>Worst-fitting nodes with the volume they waste, for the sorted listing.</summary>
        public List<(float Waste, string Text)> Worst = [];

        /// <summary>Log lines to write.</summary>
        public List<string> Lines = [];
    }
}
