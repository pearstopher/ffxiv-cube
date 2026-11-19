using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Common.Component.BGCollision;
using FFXIVClientStructs.FFXIV.Common.Component.BGCollision.Math;
using Control = FFXIVClientStructs.FFXIV.Client.Game.Control.Control;
using Framework = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework;

namespace Cube.TestBridge.Checks;

/// <summary>
/// Research and regression check for #107: some collision boxes are drawn where the player can walk straight through.
/// PROJECT.md, "Phase 2", says collision means "what the player would bump into, walk on, or be stopped by", so such
/// boxes ("phantoms") must not be drawn.
/// </summary>
/// <remarks>
/// <para>
/// For every collider box near the player (primitive colliders, and mesh nodes that hold triangles), the check casts
/// rays through it with the game's own movement raycast (<see cref="BGCollisionModule.RaycastMaterialFilter(Vector3, Vector3, out RaycastHit, float)"/>):
/// a 3 × 3 grid straight down from its top, and two heights from each side. A box with no hit point inside it is a
/// phantom. The boxes are grouped by the fields that could separate them (type, <see cref="Collider.VisibilityFlags"/>,
/// <see cref="Collider.LayerMask"/>, <see cref="Collider.ObjectMaterialValue"/>, <see cref="Collider.ObjectMaterialMask"/>),
/// and each group's phantom share is logged in one <c>Phantoms #107</c> line together with the nearest phantoms, so
/// they can be matched to what is on screen.
/// </para>
/// <para>
/// Read-only (PROJECT.md, "Read-only game access"): memory reads, getters and the raycast only.
/// </para>
/// </remarks>
/// <param name="log">Where the findings are logged, at Information so they show without Debug+.</param>
internal sealed class CollisionPhantomsCheck(IPluginLog log) : BridgeCheck
{
    /// <summary>Half the edge of the region around the player that is examined, in yalms.</summary>
    private const float HalfEdge = 150f;

    /// <summary>Most boxes tested, so the check's one frame of raycasts stays short.</summary>
    private const int MaxBoxes = 4000;

    /// <summary>How far a hit point may be outside a box and still count as inside it (box rounding, compressed vertices).</summary>
    private const float HitTolerance = 0.25f;

    /// <summary>How many of the nearest phantoms are listed in the log.</summary>
    private const int ListedPhantoms = 25;

    /// <summary>The scan in progress; set by <see cref="Examine"/> so the helpers need not pass it along.</summary>
    private Result scan = new();

    /// <inheritdoc/>
    public override string Name => "collision-phantoms";

    /// <inheritdoc/>
    public override TimeSpan Timeout => TimeSpan.FromSeconds(20);

    /// <inheritdoc/>
    public override IEnumerable<CheckStep> Run(BridgeContext context)
    {
        var result = this.Examine();
        if (result.Error is not null)
        {
            yield return CheckStep.Fail(result.Error);
            yield break;
        }

        context.Metric("phantomBoxesTested", result.Tested);
        context.Metric("phantomBoxesWithoutHit", result.Phantoms);
        context.Metric("phantomsPassingFilter", result.PhantomsKept);
        context.Metric("phantomsDroppedByFilter", result.PhantomsDropped);
        foreach (var line in result.Lines)
        {
            log.Information(line);
        }

        context.Metric("primitivePhantomsKept", result.PrimitivePhantomsKept);
        var summary = string.Create(
            CultureInfo.InvariantCulture,
            $"{result.Tested} boxes within {HalfEdge} yalms: {result.Phantoms} without a ray hit: {result.PhantomsDropped} on colliders the reader skips, {result.PrimitivePhantomsKept} on primitive colliders it keeps (must be 0), {result.PhantomsKept - result.PrimitivePhantomsKept} mesh node boxes it keeps (coarse boxes over sparse triangles, left to #100).");
        yield return result.PrimitivePhantomsKept == 0 ? CheckStep.Pass(summary) : CheckStep.Fail(summary);
    }

    /// <summary>Tests every box near the player and collects the findings.</summary>
    private unsafe Result Examine()
    {
        var result = new Result();
        this.scan = result;
        var control = Control.Instance();
        if (control == null || control->LocalPlayer == null)
        {
            result.Error = "Not logged in: no local player. Log in and leave the game running.";
            return result;
        }

        var framework = Framework.Instance();
        var module = framework == null ? null : framework->BGCollisionModule;
        if (module == null || module->SceneManager == null || module->SceneManager->FirstScene == null
            || module->SceneManager->FirstScene->Scene == null)
        {
            result.Error = "No collision scene loaded.";
            return result;
        }

        var centre = (Vector3)control->LocalPlayer->Position;
        result.Centre = centre;
        var region = new Vector3(HalfEdge);
        result.Area = new Core.CollisionBox(centre - region, centre + region);
        var groups = result.Groups;
        var phantoms = result.PhantomList;
        foreach (var collider in module->SceneManager->FirstScene->Scene->Colliders)
        {
            if (result.Tested >= MaxBoxes)
            {
                break;
            }

            var type = collider->GetColliderType();
            var fields = new Core.ColliderFields(collider->VisibilityFlags, collider->LayerMask, collider->ObjectMaterialValue, type != ColliderType.Mesh);
            var kept = Core.CollisionFilter.Blocks(fields);
            switch (type)
            {
                case ColliderType.Mesh:
                    var mesh = (ColliderMesh*)collider;
                    var bounds = new Core.CollisionBox(mesh->WorldBoundingBox.Min, mesh->WorldBoundingBox.Max);
                    if (!bounds.Intersects(result.Area)
                        || mesh->MeshIsSimple || mesh->Mesh == null)
                    {
                        break;
                    }

                    var world = mesh->World.FullMatrix();
                    var before = result.Tested;
                    result.ColliderHit = false;
                    this.WalkNode(((MeshPCB*)mesh->Mesh)->RootNode, world, collider, type, kept, 0);
                    if (result.Tested > before)
                    {
                        result.CountCollider(collider, kept, "mesh collider (any node hit)");
                    }

                    break;
                case ColliderType.Box:
                    this.TestPrimitive(((ColliderBox*)collider)->World, -Vector3.One, Vector3.One, collider, type, kept);
                    break;
                case ColliderType.Cylinder:
                    this.TestPrimitive(((ColliderCylinder*)collider)->World, -Vector3.One, Vector3.One, collider, type, kept);
                    break;
                case ColliderType.Sphere:
                    this.TestPrimitive(((ColliderSphere*)collider)->World, -Vector3.One, Vector3.One, collider, type, kept);
                    break;
                case ColliderType.Plane:
                case ColliderType.PlaneTwoSided:
                    this.TestPrimitive(((ColliderPlane*)collider)->World, new Vector3(-1, -1, 0), new Vector3(1, 1, 0), collider, type, kept);
                    break;
            }
        }

        var summary = new List<string>();
        foreach (var pair in groups)
        {
            summary.Add(string.Create(CultureInfo.InvariantCulture, $"[{pair.Key}] {pair.Value.Phantoms}/{pair.Value.Boxes}"));
        }

        summary.Sort(StringComparer.Ordinal);
        var colliderSummary = new List<string>();
        foreach (var pair in result.ColliderGroups)
        {
            colliderSummary.Add(string.Create(CultureInfo.InvariantCulture, $"[{pair.Key}] {pair.Value.Missed}/{pair.Value.Colliders}"));
        }

        colliderSummary.Sort(StringComparer.Ordinal);
        var hitSummary = new List<string>();
        foreach (var pair in HitObjects)
        {
            hitSummary.Add(string.Create(CultureInfo.InvariantCulture, $"[{pair.Key}] {pair.Value}"));
        }

        hitSummary.Sort(StringComparer.Ordinal);
        HitObjects.Clear();
        result.Lines.Add($"Phantom ray hits #107: kinds of collider the rays hit: {string.Join("; ", hitSummary)}");
        result.Lines.Add($"Phantom colliders #107: missed/colliders per group: {string.Join("; ", colliderSummary)}");
        result.Lines.Add($"Phantoms #107: player at {centre}; {result.Tested} boxes, {result.Phantoms} with no ray hit inside. Phantom/boxes per group (kept by reader? | type | visibility | layer | material value/mask | source): {string.Join("; ", summary)}");
        phantoms.Sort((a, b) => a.Distance.CompareTo(b.Distance));
        for (var i = 0; i < Math.Min(ListedPhantoms, phantoms.Count); i++)
        {
            result.Lines.Add($"Phantom #107 nearest {i + 1}: {phantoms[i].Text}");
        }

        return result;
    }

    /// <summary>Tests one primitive collider's box.</summary>
    /// <param name="world">The collider's world matrix.</param>
    /// <param name="localMin">Its shape's low corner in its own space.</param>
    /// <param name="localMax">Its shape's high corner in its own space.</param>
    /// <param name="collider">The collider.</param>
    /// <param name="type">Its type.</param>
    /// <param name="kept">Whether the reader's filter keeps it.</param>
    private unsafe void TestPrimitive(in Matrix4x3 world, Vector3 localMin, Vector3 localMax, Collider* collider, ColliderType type, bool kept)
    {
        var box = Core.CollisionBox.FromTransformed(localMin, localMax, world.FullMatrix());
        this.scan.ColliderHit = false;
        var before = this.scan.Tested;
        this.TestBox(box, collider, type, kept, "primitive");
        if (this.scan.Tested > before)
        {
            this.scan.CountCollider(collider, kept, "primitive collider");
        }
    }

    /// <summary>Walks a mesh node tree, testing the boxes of nodes that hold triangles and cross the region.</summary>
    /// <param name="node">The node, or null.</param>
    /// <param name="world">The mesh's world matrix.</param>
    /// <param name="collider">The mesh collider.</param>
    /// <param name="type">Its type.</param>
    /// <param name="kept">Whether the reader's filter keeps it.</param>
    /// <param name="depth">Recursion depth, capped against a bad pointer.</param>
    private unsafe void WalkNode(MeshPCB.FileNode* node, in Matrix4x4 world, Collider* collider, ColliderType type, bool kept, int depth)
    {
        if (node == null || depth > 64 || this.scan.Tested >= MaxBoxes)
        {
            return;
        }

        var box = Core.CollisionBox.FromTransformed(node->LocalBounds.Min, node->LocalBounds.Max, world);
        if (!box.Intersects(this.scan.Area))
        {
            return;
        }

        if (node->NumPrims > 0)
        {
            this.TestBox(box, collider, type, kept, "node");
        }

        this.WalkNode(node->Child1, world, collider, type, kept, depth + 1);
        this.WalkNode(node->Child2, world, collider, type, kept, depth + 1);
    }

    /// <summary>Casts the rays through one box and records whether any hit lands inside it.</summary>
    /// <param name="box">The box.</param>
    /// <param name="collider">The collider it came from.</param>
    /// <param name="type">Its type.</param>
    /// <param name="kept">Whether the reader's filter keeps it.</param>
    /// <param name="source">"node" or "primitive".</param>
    private unsafe void TestBox(Core.CollisionBox box, Collider* collider, ColliderType type, bool kept, string source)
    {
        if (!box.Intersects(this.scan.Area))
        {
            return;
        }

        var result = this.scan;
        result.Tested++;
        var hit = HasHit(box, collider);
        result.ColliderHit |= hit;
        var key = string.Create(
            CultureInfo.InvariantCulture,
            $"{(kept ? "kept" : "dropped")} | {type} | vis bit0 {collider->VisibilityFlags & 1} | layer {collider->LayerMask:X} | mat {collider->ObjectMaterialValue:X}/{collider->ObjectMaterialMask:X} | {source}");
        result.Groups.TryGetValue(key, out var group);
        group.Boxes++;
        if (!hit)
        {
            group.Phantoms++;
            result.Phantoms++;
            if (kept)
            {
                result.PhantomsKept++;
                if (source == "primitive")
                {
                    result.PrimitivePhantomsKept++;
                }
            }
            else
            {
                result.PhantomsDropped++;
            }

            result.PhantomList.Add((Vector3.Distance(box.Centre, result.Centre), string.Create(
                CultureInfo.InvariantCulture,
                $"{key}; box {box.Min} to {box.Max} ({Vector3.Distance(box.Centre, result.Centre):0.0} yalms away); layout id {collider->LayoutObjectId:X}")));
        }

        result.Groups[key] = group;
    }

    /// <summary>Whether any of the box's rays hits its own collider inside the box (something else hit there doesn't count: it would be that other thing the player bumps into).</summary>
    /// <param name="box">The box.</param>
    /// <param name="collider">The collider the box came from.</param>
    private static unsafe bool HasHit(Core.CollisionBox box, Collider* collider)
    {
        var min = box.Min;
        var max = box.Max;
        var size = max - min;
        var tolerance = new Vector3(HitTolerance);
        var reach = Math.Max(size.Length(), 1f) + 1f;
        const float Outside = 0.1f;

        // Down from just above the top, over a 3 x 3 grid.
        for (var ix = 0; ix < 3; ix++)
        {
            for (var iz = 0; iz < 3; iz++)
            {
                var origin = new Vector3(
                    min.X + (size.X * (ix + 0.5f) / 3f),
                    max.Y + Outside,
                    min.Z + (size.Z * (iz + 0.5f) / 3f));
                if (Hits(origin, -Vector3.UnitY, reach, min, max, tolerance, collider))
                {
                    return true;
                }
            }
        }

        // In from each side at a quarter and three quarters of the height.
        for (var level = 0; level < 2; level++)
        {
            var y = min.Y + (size.Y * (level == 0 ? 0.25f : 0.75f));
            var midX = (min.X + max.X) * 0.5f;
            var midZ = (min.Z + max.Z) * 0.5f;
            if (Hits(new Vector3(min.X - Outside, y, midZ), Vector3.UnitX, reach, min, max, tolerance, collider)
                || Hits(new Vector3(max.X + Outside, y, midZ), -Vector3.UnitX, reach, min, max, tolerance, collider)
                || Hits(new Vector3(midX, y, min.Z - Outside), Vector3.UnitZ, reach, min, max, tolerance, collider)
                || Hits(new Vector3(midX, y, max.Z + Outside), -Vector3.UnitZ, reach, min, max, tolerance, collider))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>The same ray test for a box given by its corners, for the #125 fit probe.</summary>
    /// <param name="min">Low corner.</param>
    /// <param name="max">High corner.</param>
    /// <param name="collider">The collider the box came from.</param>
    internal static unsafe bool HasHitIn(Vector3 min, Vector3 max, Collider* collider) =>
        HasHit(new Core.CollisionBox(min, max), collider);

    /// <summary>Which kinds of collider (type, layer, material) any ray hit, and how often: the kinds the game's movement raycast can ever hit.</summary>
    private static readonly Dictionary<string, int> HitObjects = [];

    /// <summary>Counts one hit object into <see cref="HitObjects"/>.</summary>
    /// <param name="collider">The collider hit, or null.</param>
    private static unsafe void Tally(Collider* collider)
    {
        if (collider == null)
        {
            return;
        }

        var key = string.Create(
            CultureInfo.InvariantCulture,
            $"{collider->GetColliderType()} | layer {collider->LayerMask:X} | vis bit0 {collider->VisibilityFlags & 1} | mat {collider->ObjectMaterialValue:X}/{collider->ObjectMaterialMask:X}");
        HitObjects.TryGetValue(key, out var count);
        HitObjects[key] = count + 1;
    }

    /// <summary>One raycast; true if it hits at a point inside the (slightly grown) box.</summary>
    /// <param name="origin">Where the ray starts.</param>
    /// <param name="direction">The ray's direction.</param>
    /// <param name="reach">How far it reaches.</param>
    /// <param name="min">The box's low corner.</param>
    /// <param name="max">The box's high corner.</param>
    /// <param name="tolerance">How far outside the box a hit still counts.</param>
    /// <param name="collider">The collider the hit must belong to.</param>
    private static unsafe bool Hits(Vector3 origin, Vector3 direction, float reach, Vector3 min, Vector3 max, Vector3 tolerance, Collider* collider)
    {
        if (!BGCollisionModule.RaycastMaterialFilter(origin, direction, out var hit, reach))
        {
            return false;
        }

        Tally(hit.Object);
        if (hit.Object != collider)
        {
            return false;
        }

        var p = hit.Point;
        var lo = min - tolerance;
        var hi = max + tolerance;
        return p.X >= lo.X && p.X <= hi.X && p.Y >= lo.Y && p.Y <= hi.Y && p.Z >= lo.Z && p.Z <= hi.Z;
    }

    /// <summary>What <see cref="Examine"/> found.</summary>
    private sealed class Result
    {
        /// <summary>The player's position.</summary>
        public Vector3 Centre;

        /// <summary>The region examined around the player.</summary>
        public Core.CollisionBox Area;

        /// <summary>Whether any box of the collider being tested was hit.</summary>
        public bool ColliderHit;

        /// <summary>Colliders (not boxes) per group: how many had any box hit.</summary>
        public Dictionary<string, (int Colliders, int Missed)> ColliderGroups = [];

        /// <summary>Counts the collider just tested into its group.</summary>
        /// <param name="collider">The collider.</param>
        /// <param name="kept">Whether the reader's filter keeps it.</param>
        /// <param name="what">The kind of collider.</param>
        public unsafe void CountCollider(Collider* collider, bool kept, string what)
        {
            var key = string.Create(
                CultureInfo.InvariantCulture,
                $"{(kept ? "kept" : "dropped")} | {collider->GetColliderType()} | layer {collider->LayerMask:X} | mat {collider->ObjectMaterialValue:X}/{collider->ObjectMaterialMask:X} | {what}");
            this.ColliderGroups.TryGetValue(key, out var group);
            group.Colliders++;
            if (!this.ColliderHit)
            {
                group.Missed++;
            }

            this.ColliderGroups[key] = group;
        }

        /// <summary>Boxes and phantoms per group of collider fields.</summary>
        public Dictionary<string, (int Boxes, int Phantoms)> Groups = [];

        /// <summary>Every phantom with its distance from the player, for the nearest-first listing.</summary>
        public List<(float Distance, string Text)> PhantomList = [];

        /// <summary>Why nothing could be tested, or <see langword="null"/>.</summary>
        public string? Error;

        /// <summary>Boxes tested.</summary>
        public int Tested;

        /// <summary>Boxes none of whose rays hit inside them.</summary>
        public int Phantoms;

        /// <summary>Phantoms on colliders the reader keeps.</summary>
        public int PhantomsKept;

        /// <summary>Phantoms among primitive colliders the reader keeps: what the filter must leave at zero.</summary>
        public int PrimitivePhantomsKept;

        /// <summary>Phantoms on colliders the reader's filter skips.</summary>
        public int PhantomsDropped;

        /// <summary>Log lines to write.</summary>
        public List<string> Lines = [];
    }
}
