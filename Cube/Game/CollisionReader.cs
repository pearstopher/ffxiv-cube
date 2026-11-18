using System.Diagnostics;
using System.Numerics;
using Cube.Core;
using FFXIVClientStructs.FFXIV.Common.Component.BGCollision;
using FFXIVClientStructs.FFXIV.Common.Component.BGCollision.Math;
using Framework = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework;
using GameMain = FFXIVClientStructs.FFXIV.Client.Game.GameMain;

namespace Cube.Game;

/// <summary>
/// Reads the collision the game client already has loaded around the player into a <see cref="CollisionCache"/>
/// (PROJECT.md, "Collision (phase 2)"), the way #28 recommends (<c>docs/research-notes.md</c>, "Where to get
/// collision data"): walk the collision scene's colliders, take mesh colliders through their node trees and primitive
/// colliders through their world matrix, culling against the cube's region first.
/// </summary>
/// <remarks>
/// <para>
/// <b>Read-only</b> (PROJECT.md, "Read-only game access"): plain memory reads plus the read-only
/// <see cref="Collider.GetColliderType"/> getter, on the framework thread like the #28 probe. Nothing is written,
/// hooked or sent, and nothing depends on vnavmesh.
/// </para>
/// <para>
/// <b>Spread over frames</b> by <see cref="CollisionCache"/>, which stops between colliders when the frame's budget
/// is spent. No game pointer is kept between frames: each frame finds its place again by index from the start of the
/// list, so a collider unloaded in between is never touched.
/// </para>
/// </remarks>
internal sealed class CollisionReader
{
    /// <summary>The cache the walk fills; read by the drawing code (#31) and the test bridge.</summary>
    public CollisionCache Cache { get; } = new(
        (long)(CollisionCache.DefaultBudgetMilliseconds * Stopwatch.Frequency / 1000.0));

#if DEBUG
    /// <summary>
    /// A zone id the test bridge puts in place of the real one, so zone changes can be checked without a human
    /// teleporting (#33). Null uses the game's zone. The scene itself is still the real one. Debug builds only.
    /// </summary>
    internal uint? SimulatedZone { get; set; }
#endif

    /// <summary>
    /// One frame's collision work: drops the cache on a zone change, and gathers (or continues gathering) when it is
    /// stale. Called from the cube window's draw path only while the cube is shown, so nothing is read while hidden
    /// (PROJECT.md, "When hidden").
    /// </summary>
    /// <param name="centre">The cube's centre (the player).</param>
    /// <param name="size">The cube's width, height and depth in yalms.</param>
    /// <param name="detail">
    /// The detail setting (#32). Part of the stamp the cache compares, so a new level gathers again at once.
    /// </param>
    /// <returns>True if a gather completed this frame.</returns>
    public unsafe bool Update(Vector3 centre, Vector3 size, CollisionDetail detail)
    {
        var main = GameMain.Instance();
        var zone = main == null ? 0u : main->CurrentTerritoryTypeId;
#if DEBUG
        zone = this.SimulatedZone ?? zone;
#endif
        var scene = FindScene();
        if (scene == null)
        {
            // No collision loaded (between zones): nothing to show, and what was kept belongs to the old area.
            this.Cache.Clear();
            return false;
        }

        var source = new SceneSource(scene, CollisionDetails.MaxNodeDepth(detail));
        return this.Cache.Step(ref source, centre, size, zone, CollisionDetails.Stamp(Stamp(scene), detail));
    }

    /// <summary>The collision scene, or null if the module or scene isn't there.</summary>
    /// <remarks>The game always has exactly one scene (FFXIVClientStructs' <c>SceneManager.FirstScene</c> notes).</remarks>
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

    /// <summary>
    /// What identifies the loaded collision: the first collider's address and the collider count, as #28 suggests.
    /// Terrain streaming in or out, or objects being added or removed, changes one of them and so starts a new gather.
    /// </summary>
    /// <param name="scene">The collision scene.</param>
    private static unsafe ulong Stamp(Scene* scene) =>
        ((ulong)scene->FirstCollider * 31UL) + (uint)scene->NumColliders;

    /// <summary>
    /// The game's collision scene as an <see cref="ICollisionSource"/>. Lives for one frame only (made fresh in
    /// <see cref="Update"/>), so the collider pointer it holds is never carried into a later frame.
    /// </summary>
    /// <param name="scene">The collision scene.</param>
    /// <param name="detailDepth">
    /// The deepest node-tree level offered (<see cref="CollisionDetails.MaxNodeDepth"/>, from the detail setting).
    /// </param>
    private unsafe struct SceneSource(Scene* scene, int detailDepth) : ICollisionSource
    {
        /// <summary>
        /// Deepest mesh node tree walked. The trees are balanced binary trees of a few hundred nodes; the cap only
        /// stops a bad pointer recursing forever.
        /// </summary>
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
        /// <remarks>
        /// Colliders the player can't bump into are skipped (<see cref="CollisionFilter.Blocks"/>: raycast-inactive,
        /// off the movement layer, or a primitive without the blocking material bit; #28, #107). Streamed colliders
        /// only load and unload terrain meshes, which are in the list themselves, so they add no box.
        /// </remarks>
        public bool OfferNext(CollisionBoxSet set)
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
                    OfferMesh((ColliderMesh*)collider, set, detailDepth);
                    break;
                case ColliderType.Box:
                    OfferUnit(((ColliderBox*)collider)->World, UnitMin, Vector3.One, set);
                    break;
                case ColliderType.Cylinder:
                    OfferUnit(((ColliderCylinder*)collider)->World, UnitMin, Vector3.One, set);
                    break;
                case ColliderType.Sphere:
                    OfferUnit(((ColliderSphere*)collider)->World, UnitMin, Vector3.One, set);
                    break;
                case ColliderType.Plane:
                case ColliderType.PlaneTwoSided:
                    OfferUnit(((ColliderPlane*)collider)->World, PlaneMin, PlaneMax, set);
                    break;
            }

            return true;
        }

        /// <summary>The low corner of a box, cylinder or sphere collider's unit shape: (-1,-1,-1) to (1,1,1).</summary>
        private static Vector3 UnitMin => -Vector3.One;

        /// <summary>A plane collider's low corner: it is flat in its own Z, (-1,-1,0) to (1,1,0).</summary>
        private static Vector3 PlaneMin => new(-1, -1, 0);

        /// <summary>A plane collider's high corner.</summary>
        private static Vector3 PlaneMax => new(1, 1, 0);

        /// <summary>Offers a primitive collider: its unit shape through its world matrix, if it reaches the region.</summary>
        /// <param name="world">The collider's world matrix.</param>
        /// <param name="localMin">The shape's low corner in its own space.</param>
        /// <param name="localMax">The shape's high corner in its own space.</param>
        /// <param name="set">The set being gathered.</param>
        private static void OfferUnit(in Matrix4x3 world, Vector3 localMin, Vector3 localMax, CollisionBoxSet set)
        {
            var box = CollisionBox.FromTransformed(localMin, localMax, world.FullMatrix());
            if (set.Overlaps(box))
            {
                set.Offer(box);
            }
        }

        /// <summary>
        /// Offers a mesh collider: skipped whole if its world box misses the region (the cheap first cull), otherwise
        /// one box per node holding triangles, down to the detail setting's depth, through the collider's world matrix.
        /// </summary>
        /// <param name="mesh">The mesh collider.</param>
        /// <param name="set">The set being gathered.</param>
        /// <param name="detailDepth">The deepest tree level offered.</param>
        private static void OfferMesh(ColliderMesh* mesh, CollisionBoxSet set, int detailDepth)
        {
            var bounds = new CollisionBox(mesh->WorldBoundingBox.Min, mesh->WorldBoundingBox.Max);
            if (!set.Overlaps(bounds) || mesh->MeshIsSimple || mesh->Mesh == null)
            {
                // Not loaded yet, or the unused simple mesh form (FFXIVClientStructs notes it's never used in game).
                return;
            }

            var world = mesh->World.FullMatrix();
            OfferNode(((MeshPCB*)mesh->Mesh)->RootNode, world, set, 0, detailDepth);
        }

        /// <summary>
        /// Offers one mesh node's box if it holds triangles, then its children, skipping any subtree whose box misses
        /// the region (#28: that is what keeps the walk to a few hundred nodes). At the detail setting's depth the
        /// node's box stands for its whole subtree (it holds its children's boxes), so the walk stops there.
        /// </summary>
        /// <param name="node">The node, or null.</param>
        /// <param name="world">The mesh's world matrix.</param>
        /// <param name="set">The set being gathered.</param>
        /// <param name="depth">How deep in the tree; capped at <see cref="MaxDepth"/>.</param>
        /// <param name="detailDepth">The deepest tree level offered (#32).</param>
        private static void OfferNode(MeshPCB.FileNode* node, in Matrix4x4 world, CollisionBoxSet set, int depth, int detailDepth)
        {
            if (node == null || depth > MaxDepth)
            {
                return;
            }

            var box = CollisionBox.FromTransformed(node->LocalBounds.Min, node->LocalBounds.Max, world);
            if (!set.Overlaps(box))
            {
                return;
            }

            if (depth >= detailDepth)
            {
                set.Offer(box);
                return;
            }

            if (node->NumPrims > 0)
            {
                set.Offer(box);
            }

            OfferNode(node->Child1, world, set, depth + 1, detailDepth);
            OfferNode(node->Child2, world, set, depth + 1, detailDepth);
        }
    }
}
