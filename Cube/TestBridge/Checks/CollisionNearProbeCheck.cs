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
/// Research probe for #129: what collision really sits around the player, and what does the game's own movement
/// raycast say about it? Used with the player standing in a gap (a counter's opening) to see why the cube draws it closed.
/// </summary>
/// <remarks>
/// <para>
/// Logs, as <c>Near #129</c> lines, every collider within <see cref="Reach"/> yalms (type, layer, material, layout id,
/// whether the reader keeps it, its world box), each mesh node holding triangles there with its triangles' bounds, and
/// a ring of horizontal raycasts at three heights (what the game itself treats as blocking, with the collider hit).
/// Read-only (PROJECT.md, "Read-only game access"); it allocates freely because it is a Debug research probe.
/// </para>
/// </remarks>
/// <param name="log">Where the findings are logged, at Information so they show without Debug+.</param>
internal sealed class CollisionNearProbeCheck(IPluginLog log) : BridgeCheck
{
    /// <summary>How far from the player, in yalms, colliders and triangles are listed.</summary>
    private const float Reach = 4f;

    /// <summary>Most triangles listed per node, so one big node can't flood the log.</summary>
    private const int MaxTrianglesPerNode = 300;

    /// <inheritdoc/>
    public override string Name => "probe-collision-near";

    /// <inheritdoc/>
    public override TimeSpan Timeout => TimeSpan.FromSeconds(20);

    /// <inheritdoc/>
    public override IEnumerable<CheckStep> Run(BridgeContext context)
    {
        var lines = new List<string>();
        var error = this.Examine(lines);
        if (error is not null)
        {
            yield return CheckStep.Fail(error);
            yield break;
        }

        foreach (var line in lines)
        {
            log.Information(line);
        }

        yield return CheckStep.Pass($"{lines.Count} lines logged as Near #129.");
    }

    /// <summary>Collects the findings into <paramref name="lines"/>.</summary>
    /// <param name="lines">The log lines.</param>
    /// <returns>An error message, or <see langword="null"/>.</returns>
    private unsafe string? Examine(List<string> lines)
    {
        var control = Control.Instance();
        if (control == null || control->LocalPlayer == null)
        {
            return "Not logged in: no local player.";
        }

        var framework = Framework.Instance();
        var module = framework == null ? null : framework->BGCollisionModule;
        if (module == null || module->SceneManager == null || module->SceneManager->FirstScene == null
            || module->SceneManager->FirstScene->Scene == null)
        {
            return "No collision scene loaded.";
        }

        var p = (Vector3)control->LocalPlayer->Position;
        var lo = p - new Vector3(Reach);
        var hi = p + new Vector3(Reach);
        lines.Add(string.Create(CultureInfo.InvariantCulture, $"Near #129: player at {p}, facing {control->LocalPlayer->Rotation:0.00} rad; listing colliders within {Reach} yalms"));

        foreach (var collider in module->SceneManager->FirstScene->Scene->Colliders)
        {
            var type = collider->GetColliderType();
            Vector3 min;
            Vector3 max;
            switch (type)
            {
                case ColliderType.Mesh:
                    var m = (ColliderMesh*)collider;
                    min = m->WorldBoundingBox.Min;
                    max = m->WorldBoundingBox.Max;
                    break;
                case ColliderType.Box:
                    CollisionProbeCheck.UnitShapeBounds(((ColliderBox*)collider)->World, out min, out max);
                    break;
                case ColliderType.Cylinder:
                    CollisionProbeCheck.UnitShapeBounds(((ColliderCylinder*)collider)->World, out min, out max);
                    break;
                case ColliderType.Sphere:
                    CollisionProbeCheck.UnitShapeBounds(((ColliderSphere*)collider)->World, out min, out max);
                    break;
                case ColliderType.Plane:
                case ColliderType.PlaneTwoSided:
                    CollisionProbeCheck.UnitShapeBounds(((ColliderPlane*)collider)->World, out min, out max);
                    break;
                default:
                    continue;
            }

            if (!CollisionProbeCheck.Overlaps(min, max, lo, hi))
            {
                continue;
            }

            var kept = Core.CollisionFilter.Blocks(new Core.ColliderFields(collider->VisibilityFlags, collider->LayerMask, collider->ObjectMaterialValue, type != ColliderType.Mesh));
            lines.Add(string.Create(
                CultureInfo.InvariantCulture,
                $"Near #129: collider {(nint)collider:X} {type}, reader {(kept ? "keeps" : "DROPS")} it, vis {collider->VisibilityFlags:X}, layer {collider->LayerMask:X}, material {collider->ObjectMaterialValue:X}/{collider->ObjectMaterialMask:X}, layout id {collider->LayoutObjectId:X}, world box {min} to {max}"));
            if (type == ColliderType.Mesh)
            {
                var mesh = (ColliderMesh*)collider;
                if (!mesh->MeshIsSimple && mesh->Mesh != null)
                {
                    var listed = 0;
                    Walk(((MeshPCB*)mesh->Mesh)->RootNode, mesh->World, lo, hi, 0, lines, ref listed);
                }
            }
        }

        // Ground truth: where does the game's own movement raycast stop, in a ring around the player?
        foreach (var height in new[] { 0.3f, 1.0f, 1.8f })
        {
            for (var i = 0; i < 16; i++)
            {
                var angle = i * MathF.PI / 8f;
                var dir = new Vector3(MathF.Sin(angle), 0, MathF.Cos(angle));
                var origin = p + new Vector3(0, height, 0);
                if (BGCollisionModule.RaycastMaterialFilter(origin, dir, out var hit, Reach))
                {
                    var c = hit.Object;
                    lines.Add(string.Create(
                        CultureInfo.InvariantCulture,
                        $"Near #129: ray height {height:0.0} bearing {i * 22.5f:0} deg (dir {dir.X:0.00},{dir.Z:0.00}): hit at {Vector3.Distance(origin, hit.Point):0.00} yalms, point {hit.Point}, collider {(nint)c:X} {(c == null ? "?" : c->GetColliderType().ToString())} layout id {(c == null ? 0 : c->LayoutObjectId):X}"));
                }
                else
                {
                    lines.Add(string.Create(
                        CultureInfo.InvariantCulture,
                        $"Near #129: ray height {height:0.0} bearing {i * 22.5f:0} deg (dir {dir.X:0.00},{dir.Z:0.00}): clear for {Reach} yalms"));
                }
            }
        }

        // What the player would stand on: rays straight down from head height over a 0.25-yalm grid, as a text map
        // (north, -z, at the top; west, -x, on the left). Character = ground relative to the player's feet.
        var letters = new Dictionary<nint, char>();
        var legend = new List<string>();
        lines.Add("Near #129: floor map, one row per 0.25 yalm of z (top = -z = north), one column per 0.25 yalm of x (left = -x = west). ' ' no ground within 12 yalms, '#' over 0.9 above the feet, '+' above, '=' level (+-0.1), '-' below, '_' over 0.9 below. Second map: which collider.");
        var heightRows = new List<string>();
        var colliderRows = new List<string>();
        for (var zi = -12; zi <= 12; zi++)
        {
            var heightRow = new System.Text.StringBuilder();
            var colliderRow = new System.Text.StringBuilder();
            for (var xi = -12; xi <= 12; xi++)
            {
                var origin = new Vector3(p.X + (xi * 0.25f), p.Y + 1.8f, p.Z + (zi * 0.25f));
                if (BGCollisionModule.RaycastMaterialFilter(origin, -Vector3.UnitY, out var hit, 14f))
                {
                    var d = hit.Point.Y - p.Y;
                    heightRow.Append(d > 0.9f ? '#' : d > 0.1f ? '+' : d >= -0.1f ? '=' : d >= -0.9f ? '-' : '_');
                    var c = (nint)hit.Object;
                    if (!letters.TryGetValue(c, out var letter))
                    {
                        letter = (char)('A' + (letters.Count % 26));
                        letters[c] = letter;
                        legend.Add(string.Create(
                            CultureInfo.InvariantCulture,
                            $"Near #129: map letter {letter} = collider {c:X} layout id {(hit.Object == null ? 0 : hit.Object->LayoutObjectId):X}"));
                    }

                    colliderRow.Append(letter);
                }
                else
                {
                    heightRow.Append(' ');
                    colliderRow.Append(' ');
                }
            }

            heightRows.Add(heightRow.ToString());
            colliderRows.Add(colliderRow.ToString());
        }

        for (var i = 0; i < heightRows.Count; i++)
        {
            lines.Add($"Near #129: map z{(i - 12) * 0.25f,6:0.00} |{heightRows[i]}|{colliderRows[i]}|");
        }

        lines.AddRange(legend);
        return null;
    }

    /// <summary>Logs the nodes with triangles that reach the region, with their triangles' bounds.</summary>
    /// <param name="node">The node, or null.</param>
    /// <param name="world">The mesh's world matrix.</param>
    /// <param name="lo">Region low corner.</param>
    /// <param name="hi">Region high corner.</param>
    /// <param name="depth">Recursion depth.</param>
    /// <param name="lines">The log lines.</param>
    /// <param name="listed">Nodes listed so far for this mesh.</param>
    private static unsafe void Walk(MeshPCB.FileNode* node, in Matrix4x3 world, Vector3 lo, Vector3 hi, int depth, List<string> lines, ref int listed)
    {
        if (node == null || depth > 64)
        {
            return;
        }

        CollisionProbeCheck.TransformBox(world, node->LocalBounds.Min, node->LocalBounds.Max, out var min, out var max);
        if (!CollisionProbeCheck.Overlaps(min, max, lo, hi))
        {
            return;
        }

        if (node->NumPrims > 0)
        {
            listed++;
            lines.Add(string.Create(
                CultureInfo.InvariantCulture,
                $"Near #129:   node depth {depth}, {node->NumPrims} triangles, box {min} to {max}"));
            var shown = 0;
            foreach (ref var prim in node->Primitives)
            {
                var a = world.TransformCoordinate(node->Vertex(prim.V1));
                var b = world.TransformCoordinate(node->Vertex(prim.V2));
                var c = world.TransformCoordinate(node->Vertex(prim.V3));
                if (shown >= MaxTrianglesPerNode)
                {
                    break;
                }

                shown++;
                lines.Add(string.Create(CultureInfo.InvariantCulture, $"Near #129:     triangle {a} {b} {c}"));
            }
        }

        Walk(node->Child1, world, lo, hi, depth + 1, lines, ref listed);
        Walk(node->Child2, world, lo, hi, depth + 1, lines, ref listed);
    }
}
