using System;
using System.Diagnostics;
using System.Numerics;
using Cube.Core;
using FFXIVClientStructs.FFXIV.Common.Component.BGCollision;
using FFXIVClientStructs.FFXIV.Common.Component.BGCollision.Math;
using Framework = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework;
using GameMain = FFXIVClientStructs.FFXIV.Client.Game.GameMain;

namespace Cube.Game;

/// <summary>
/// Reads the collision triangles the game client has loaded around the player into a <see cref="TriangleCache"/> for
/// the Mesh collision style (#100; PROJECT.md, "Collision style"): mesh colliders' triangles through their world matrix,
/// and primitive colliders as their real shapes in triangles (<see cref="PrimitiveMeshes"/>).
/// </summary>
/// <remarks>
/// <para>
/// The same walk as <see cref="CollisionReader"/> (<c>docs/research-notes.md</c>, #28 and #99): the same colliders are
/// kept (<see cref="CollisionFilter.Blocks"/>), whole colliders and tree nodes outside the gathered region are skipped
/// before their triangles are read, and the walk is spread over frames by <see cref="TriangleCache"/>. Read-only: plain
/// memory reads plus the read-only <see cref="Collider.GetColliderType"/>; no game pointer is kept between frames.
/// Nothing is simplified: every triangle of every node that reaches the region is kept, and the cull only rejects
/// triangles that can't touch the cube.
/// </para>
/// </remarks>
internal sealed class MeshReader
{
    /// <summary>The cache the walk fills; read by the clipping and the test bridge.</summary>
    public TriangleCache Cache { get; } = new(
        (long)(TriangleCache.DefaultBudgetMilliseconds * Stopwatch.Frequency / 1000.0));

    /// <summary>The most triangles dropped for lack of room already logged, so a larger scene is logged again but a repeat isn't.</summary>
    private int reportedDropped;

#if DEBUG
    /// <summary>A zone id the bridge puts in place of the real one (see <see cref="CollisionReader.SimulatedZone"/>). Debug builds only.</summary>
    internal uint? SimulatedZone { get; set; }
#endif

    /// <summary>
    /// One frame's mesh work: drops the cache on a zone change, and gathers (or continues gathering) when it is stale.
    /// Called from the cube window's draw path only while Mesh is the chosen style and the cube is shown.
    /// </summary>
    /// <param name="centre">The cube's centre.</param>
    /// <param name="size">The cube's width, height and depth in yalms.</param>
    /// <returns>True if a gather completed this frame.</returns>
    public unsafe bool Update(Vector3 centre, Vector3 size)
    {
        var main = GameMain.Instance();
        var zone = main == null ? 0u : main->CurrentTerritoryTypeId;
#if DEBUG
        zone = this.SimulatedZone ?? zone;
#endif
        var scene = FindScene();
        if (scene == null)
        {
            this.Cache.Clear();
            return false;
        }

        var source = new SceneSource(scene);
        var completed = this.Cache.Step(ref source, centre, size, zone, Stamp(scene));
        if (completed && this.Cache.Ready is { Dropped: > 0 } ready && ready.Dropped > this.reportedDropped)
        {
            // PROJECT.md, "Collision style": a scene bigger than the buffer is said so, never cut off silently.
            this.reportedDropped = ready.Dropped;
            Plugin.Log.Warning($"Mesh collision: {ready.Dropped} triangles near the cube didn't fit the {ready.Capacity}-triangle buffer and are not drawn.");
        }

        return completed;
    }

    /// <summary>Drops the triangles and their buffers, when the style goes back to Boxes or collision is turned off.</summary>
    public void Release()
    {
        this.Cache.Release();
        this.reportedDropped = 0;
    }

    /// <summary>The collision scene, or null if the module or scene isn't there.</summary>
    private static unsafe Scene* FindScene()
    {
        var framework = Framework.Instance();
        var module = framework == null ? null : framework->BGCollisionModule;
        if (module == null || module->SceneManager == null || module->SceneManager->FirstScene == null)
        {
            return null;
        }

        return module->SceneManager->FirstScene->Scene;
    }

    /// <summary>What identifies the loaded collision: the first collider's address and the collider count (as in <see cref="CollisionReader"/>).</summary>
    /// <param name="scene">The collision scene.</param>
    private static unsafe ulong Stamp(Scene* scene) =>
        ((ulong)scene->FirstCollider * 31UL) + (uint)scene->NumColliders;

    /// <summary>The game's collision scene as an <see cref="ITriangleSource"/>; made fresh each frame so no collider pointer outlives it.</summary>
    /// <param name="scene">The collision scene.</param>
    private unsafe struct SceneSource(Scene* scene) : ITriangleSource
    {
        /// <summary>Deepest mesh node tree walked; only stops a bad pointer recursing forever.</summary>
        private const int MaxDepth = 64;

        /// <summary>The collider <see cref="OfferNext"/> reads next, or null at the end of the list.</summary>
        private Collider* current;

        /// <inheritdoc/>
        public readonly long Now => Stopwatch.GetTimestamp();

        /// <inheritdoc/>
        public bool Seek(int index)
        {
            this.current = scene->FirstCollider;
            for (var i = 0; i < index; i++)
            {
                if (this.current == null)
                {
                    return false;
                }

                this.current = (Collider*)this.current->Next;
            }

            return true;
        }

        /// <inheritdoc/>
        public bool OfferNext(TriangleSet set)
        {
            var collider = this.current;
            if (collider == null)
            {
                return false;
            }

            this.current = (Collider*)collider->Next;
            var type = collider->GetColliderType();
            var isPrimitive = type != ColliderType.Mesh && type != ColliderType.Streamed;
            if (!CollisionFilter.Blocks(new ColliderFields(collider->VisibilityFlags, collider->LayerMask, collider->ObjectMaterialValue, isPrimitive)))
            {
                return true;
            }

            switch (type)
            {
                case ColliderType.Mesh:
                    OfferMesh((ColliderMesh*)collider, set);
                    break;
                case ColliderType.Box:
                    if (UnitReaches(((ColliderBox*)collider)->World, -Vector3.One, Vector3.One, set))
                    {
                        PrimitiveMeshes.AddBox(set, ((ColliderBox*)collider)->World.FullMatrix());
                    }

                    break;
                case ColliderType.Cylinder:
                    if (UnitReaches(((ColliderCylinder*)collider)->World, -Vector3.One, Vector3.One, set))
                    {
                        PrimitiveMeshes.AddCylinder(set, ((ColliderCylinder*)collider)->World.FullMatrix());
                    }

                    break;
                case ColliderType.Sphere:
                    if (UnitReaches(((ColliderSphere*)collider)->World, -Vector3.One, Vector3.One, set))
                    {
                        PrimitiveMeshes.AddSphere(set, ((ColliderSphere*)collider)->World.FullMatrix());
                    }

                    break;
                case ColliderType.Plane:
                case ColliderType.PlaneTwoSided:
                    if (UnitReaches(((ColliderPlane*)collider)->World, new Vector3(-1, -1, 0), new Vector3(1, 1, 0), set))
                    {
                        PrimitiveMeshes.AddPlane(set, ((ColliderPlane*)collider)->World.FullMatrix());
                    }

                    break;
            }

            return true;
        }

        /// <summary>Whether a primitive's unit shape placed by its matrix reaches the region: the cull before tessellating it.</summary>
        /// <param name="world">The collider's world matrix.</param>
        /// <param name="localMin">The shape's low corner in its own space.</param>
        /// <param name="localMax">The shape's high corner in its own space.</param>
        /// <param name="set">The set being gathered.</param>
        private static bool UnitReaches(in Matrix4x3 world, Vector3 localMin, Vector3 localMax, TriangleSet set) =>
            set.Overlaps(CollisionBox.FromTransformed(localMin, localMax, world.FullMatrix()));

        /// <summary>Reads a mesh collider's triangles unless its world box misses the region.</summary>
        /// <param name="mesh">The mesh collider.</param>
        /// <param name="set">The set being gathered.</param>
        private static void OfferMesh(ColliderMesh* mesh, TriangleSet set)
        {
            var bounds = new CollisionBox(mesh->WorldBoundingBox.Min, mesh->WorldBoundingBox.Max);
            if (!set.Overlaps(bounds) || mesh->MeshIsSimple || mesh->Mesh == null)
            {
                return;
            }

            var world = mesh->World.FullMatrix();
            OfferNode(((MeshPCB*)mesh->Mesh)->RootNode, world, set, 0);
        }

        /// <summary>Reads one node's triangles and then its children's, skipping any subtree whose box misses the region.</summary>
        /// <param name="node">The node, or null.</param>
        /// <param name="world">The mesh's world matrix.</param>
        /// <param name="set">The set being gathered.</param>
        /// <param name="depth">How deep in the tree; capped at <see cref="MaxDepth"/>.</param>
        private static void OfferNode(MeshPCB.FileNode* node, in Matrix4x4 world, TriangleSet set, int depth)
        {
            if (node == null || depth > MaxDepth)
            {
                return;
            }

            if (!set.Overlaps(CollisionBox.FromTransformed(node->LocalBounds.Min, node->LocalBounds.Max, world)))
            {
                return;
            }

            if (node->NumPrims > 0)
            {
                foreach (ref var prim in node->Primitives)
                {
                    set.Offer(
                        Vector3.Transform(node->Vertex(prim.V1), world),
                        Vector3.Transform(node->Vertex(prim.V2), world),
                        Vector3.Transform(node->Vertex(prim.V3), world));
                }
            }

            OfferNode(node->Child1, world, set, depth + 1);
            OfferNode(node->Child2, world, set, depth + 1);
        }
    }
}
