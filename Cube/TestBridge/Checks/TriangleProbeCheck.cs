using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Text;
using Dalamud.Bindings.ImGui;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Common.Component.BGCollision;
using FFXIVClientStructs.FFXIV.Common.Component.BGCollision.Math;
using Control = FFXIVClientStructs.FFXIV.Client.Game.Control.Control;
using Framework = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework;

namespace Cube.TestBridge.Checks;

/// <summary>
/// Research probe for #99 (can the real collision triangles be drawn in the cube?): measures, in the running game,
/// reading every triangle in the cube, clipping them to a turned box, deduplicating their edges, projecting and
/// sorting them, and the cost of issuing them to ImGui as wireframe, filled triangles, or both.
/// </summary>
/// <remarks>
/// <para>
/// Everything is measured at the default (40 yalm) and largest (100 yalm) cube, with the box turned 30 degrees (#93:
/// the cube turns with the camera, so clipping is done in box space). Reading and clipping use only plain memory reads
/// like <see cref="CollisionProbeCheck"/>; drawing goes through <see cref="CubeWindow.ProbeDraw"/>, which the window
/// calls inside its own draw so real ImGui calls are timed on the window's draw list. The buffers are large and fixed,
/// allocated when the check starts and dropped in <see cref="Cleanup"/>, so allocation counts for the timed steps are
/// the steps' own. Results are logged in one <c>Probe #99</c> line and written as metrics; the check fails only when
/// there is nothing to read.
/// </para>
/// <para>
/// Primitive colliders are turned into triangles here only to count and time them: a box is 12, a plane 2, a cylinder
/// an 8-sided prism (32) and a sphere an icosahedron (20). Their real shapes are approximate; their counts are small.
/// </para>
/// </remarks>
/// <param name="log">Where the findings are logged, at Information.</param>
internal sealed class TriangleProbeCheck(IPluginLog log) : BridgeCheck
{
    /// <summary>Most world triangles kept when reading; more are counted but dropped.</summary>
    private const int ReadCapacity = 300_000;

    /// <summary>Most clipped triangles kept: clipping can split an edge triangle into several.</summary>
    private const int ClipCapacity = 400_000;

    /// <summary>Slots in the edge table: a power of two above twice the most edges.</summary>
    private const int EdgeSlots = 1 << 21;

    /// <summary>The most primitives the draw step issues in one frame, so a huge scene can't flood the game's draw list.</summary>
    private const int DrawCap = 60_000;

    /// <summary>The cube's turn in the measurements, in radians: a worst-case, not axis-aligned.</summary>
    private const float Yaw = 30f * MathF.PI / 180f;

    /// <summary>The fixed tilt of the viewing angle, in radians, so floors show as areas.</summary>
    private const float Tilt = 35f * MathF.PI / 180f;

    /// <summary>Frames sampled for each draw mode and cube size, per phase (idle and turning).</summary>
    private const int FramesPerCell = 8;

    /// <summary>Quantisation of vertices when matching shared edges: 1/32 yalm.</summary>
    private const float VertexScale = 32f;

    /// <summary>Drawing cell state: which ImGui work the hook does this frame.</summary>
    private enum Mode
    {
        /// <summary>Unique edges as lines.</summary>
        Edges,

        /// <summary>Triangles filled, sorted back to front.</summary>
        Filled,

        /// <summary>Filled triangles then edges.</summary>
        Both,
    }

    /// <summary>World triangles read from the scene (3 vertices each).</summary>
    private Vector3[]? world;

    /// <summary>Clipped triangles in box space (3 vertices each).</summary>
    private Vector3[]? clipped;

    /// <summary>Unique edges in box space (2 vertices each).</summary>
    private Vector3[]? edges;

    /// <summary>Projected triangle corners, 3 per clipped triangle.</summary>
    private Vector2[]? triScreen;

    /// <summary>Projected edge ends, 2 per edge.</summary>
    private Vector2[]? edgeScreen;

    /// <summary>Flat-shade colour per clipped triangle.</summary>
    private uint[]? triColour;

    /// <summary>Depth sort keys per clipped triangle.</summary>
    private float[]? keys;

    /// <summary>Triangle indexes, sorted by key.</summary>
    private int[]? order;

    /// <summary>The edge table's keys (0 = empty).</summary>
    private ulong[]? edgeKeys;

    /// <summary>The edge table's first triangle per key.</summary>
    private int[]? edgeTri;

    /// <summary>Triangles read, wholly or partly in the cube's bounds, and kept.</summary>
    private int worldCount;

    /// <summary>Triangles after clipping.</summary>
    private int clippedCount;

    /// <summary>Unique edges.</summary>
    private int edgeCount;

    /// <summary>The cube's half edge for the cell being measured.</summary>
    private float half;

    /// <summary>The current cell's mode.</summary>
    private Mode mode;

    /// <summary>Whether the current cell re-projects and re-sorts every frame (view turning) or only draws cached data (idle).</summary>
    private bool turning;

    /// <summary>The viewing yaw for the current frame, advanced while turning.</summary>
    private float viewYaw;

    /// <summary>Whether an idle cell still has to project its first frame.</summary>
    private bool fresh;

    /// <summary>Whether the draw hook should do its work this frame.</summary>
    private bool armed;

    /// <summary>Hook time this frame in milliseconds, set by <see cref="DrawHook"/>.</summary>
    private double hookMs;

    /// <summary>Hook allocations this frame.</summary>
    private long hookBytes;

    /// <summary>Hook primitives issued this frame.</summary>
    private int hookPrims;

    /// <summary>Whether the last project step found more primitives than <see cref="DrawCap"/>.</summary>
    private bool capped;

    /// <summary>The window, for its drawing region.</summary>
    private CubeWindow? window;

    /// <inheritdoc/>
    public override string Name => "probe-triangles";

    /// <inheritdoc/>
    public override TimeSpan Timeout => TimeSpan.FromSeconds(90);

    /// <inheritdoc/>
    public override IEnumerable<CheckStep> Run(BridgeContext context)
    {
        this.window = context.Window;
        var wasOpen = context.Window.IsOpen;
        context.Window.IsOpen = true;
        this.Allocate();
        context.Window.ProbeDraw = this.DrawHook;
        yield return CheckStep.NextFrame;

        var report = new StringBuilder();
        var failure = default(string);
        foreach (var size in new[] { Core.BoxExtents.Default, Core.BoxExtents.Max })
        {
            this.half = size / 2f;
            var read = this.MeasureRead(out var centre, out failure);
            if (failure is not null)
            {
                break;
            }

            var prefix = string.Create(CultureInfo.InvariantCulture, $"{size:0}");
            var clip = this.MeasureClip(centre);
            var dedup = this.MeasureDedup();
            var project = this.MeasureProject();
            var small = this.CountSmall(size);
            context.Metric($"cube{prefix}_trianglesInBounds", read.InBounds);
            context.Metric($"cube{prefix}_trianglesKept", this.worldCount);
            context.Metric($"cube{prefix}_primitiveTriangles", read.Primitive);
            context.Metric($"cube{prefix}_readMs", read.Ms);
            context.Metric($"cube{prefix}_readBytes", read.Bytes);
            context.Metric($"cube{prefix}_wholeInside", clip.Inside);
            context.Metric($"cube{prefix}_wholeOutside", clip.Outside);
            context.Metric($"cube{prefix}_cut", clip.Cut);
            context.Metric($"cube{prefix}_clippedTriangles", this.clippedCount);
            context.Metric($"cube{prefix}_clipMs", clip.Ms);
            context.Metric($"cube{prefix}_clipBytes", clip.Bytes);
            context.Metric($"cube{prefix}_uniqueEdges", this.edgeCount);
            context.Metric($"cube{prefix}_sharedEdges", dedup.Shared);
            context.Metric($"cube{prefix}_coplanarSharedEdges", dedup.Coplanar);
            context.Metric($"cube{prefix}_dedupMs", dedup.Ms);
            context.Metric($"cube{prefix}_dedupBytes", dedup.Bytes);
            context.Metric($"cube{prefix}_projectSortMs", project.Ms);
            context.Metric($"cube{prefix}_projectSortBytes", project.Bytes);
            context.Metric($"cube{prefix}_subPixelTriangles", small);
            report.Append(string.Create(
                CultureInfo.InvariantCulture,
                $"[{size:0} yalm cube at {centre}: {read.InBounds} triangles in node bounds ({read.Primitive} from primitives), {this.worldCount} kept near the cube in {read.Ms:0.###} ms {read.Bytes} B; clip: {clip.Inside} inside, {clip.Outside} outside, {clip.Cut} cut -> {this.clippedCount} triangles in {clip.Ms:0.###} ms {clip.Bytes} B; edges: {this.edgeCount} unique, {dedup.Shared} shared, {dedup.Coplanar} between coplanar triangles, {dedup.Ms:0.###} ms {dedup.Bytes} B; project+sort {project.Ms:0.###} ms {project.Bytes} B; {small} triangles under 1 px at 300 px; "));

            foreach (var drawMode in new[] { Mode.Edges, Mode.Filled, Mode.Both })
            {
                foreach (var isTurning in new[] { false, true })
                {
                    this.mode = drawMode;
                    this.turning = isTurning;
                    this.fresh = true;
                    double ms = 0, maxMs = 0;
                    long bytes = 0;
                    var prims = 0;
                    var frames = 0;
                    for (var i = 0; i < FramesPerCell + 1; i++)
                    {
                        this.hookMs = -1;
                        this.armed = true;
                        yield return CheckStep.NextFrame;
                        this.armed = false;
                        if (i == 0 || this.hookMs < 0)
                        {
                            continue;
                        }

                        frames++;
                        ms += this.hookMs;
                        maxMs = Math.Max(maxMs, this.hookMs);
                        bytes += this.hookBytes;
                        prims = this.hookPrims;
                    }

                    frames = Math.Max(frames, 1);
                    var cell = $"cube{prefix}_{drawMode}_{(isTurning ? "turning" : "idle")}";
                    context.Metric($"{cell}_ms", ms / frames);
                    context.Metric($"{cell}_maxMs", maxMs);
                    context.Metric($"{cell}_bytes", bytes / frames);
                    context.Metric($"{cell}_primitives", prims);
                    report.Append(string.Create(
                        CultureInfo.InvariantCulture,
                        $"{drawMode} {(isTurning ? "turning" : "idle")} {ms / frames:0.###} ms (max {maxMs:0.###}) {bytes / frames} B {prims} prims{(this.capped ? " CAPPED" : string.Empty)}; "));
                }
            }

            report.Append("] ");
        }

        context.Window.ProbeDraw = null;
        context.Window.IsOpen = wasOpen;
        this.Free();
        if (failure is not null)
        {
            yield return CheckStep.Fail(failure);
            yield break;
        }

        log.Information("Probe #99: " + report);
        yield return CheckStep.Pass("Triangle numbers logged as Probe #99 and recorded as metrics.");
    }

    /// <inheritdoc/>
    public override void Cleanup(BridgeContext context)
    {
        context.Window.ProbeDraw = null;
        this.armed = false;
        this.Free();
    }

    /// <summary>Allocates the probe's large fixed buffers once.</summary>
    private void Allocate()
    {
        this.world ??= new Vector3[ReadCapacity * 3];
        this.clipped ??= new Vector3[ClipCapacity * 3];
        this.edges ??= new Vector3[EdgeSlots];
        this.triScreen ??= new Vector2[ClipCapacity * 3];
        this.edgeScreen ??= new Vector2[EdgeSlots];
        this.triColour ??= new uint[ClipCapacity];
        this.keys ??= new float[ClipCapacity];
        this.order ??= new int[ClipCapacity];
        this.edgeKeys ??= new ulong[EdgeSlots];
        this.edgeTri ??= new int[EdgeSlots];
    }

    /// <summary>Drops the buffers so the probe leaves nothing behind.</summary>
    private void Free()
    {
        this.world = null;
        this.clipped = null;
        this.edges = null;
        this.triScreen = null;
        this.edgeScreen = null;
        this.triColour = null;
        this.keys = null;
        this.order = null;
        this.edgeKeys = null;
        this.edgeTri = null;
    }

    /// <summary>Counts from reading.</summary>
    /// <param name="InBounds">Triangles in nodes crossing the cube (the #28 number).</param>
    /// <param name="Primitive">Triangles made from primitive colliders.</param>
    /// <param name="Ms">Time in milliseconds.</param>
    /// <param name="Bytes">Bytes allocated.</param>
    private readonly record struct ReadResult(int InBounds, int Primitive, double Ms, long Bytes);

    /// <summary>Counts from clipping.</summary>
    /// <param name="Inside">Triangles wholly inside.</param>
    /// <param name="Outside">Triangles wholly outside one plane.</param>
    /// <param name="Cut">Triangles cut by the box.</param>
    /// <param name="Ms">Time in milliseconds.</param>
    /// <param name="Bytes">Bytes allocated.</param>
    private readonly record struct ClipResult(int Inside, int Outside, int Cut, double Ms, long Bytes);

    /// <summary>Counts from edge deduplication.</summary>
    /// <param name="Shared">Edges used by two triangles.</param>
    /// <param name="Coplanar">Shared edges whose two triangles are coplanar.</param>
    /// <param name="Ms">Time in milliseconds.</param>
    /// <param name="Bytes">Bytes allocated.</param>
    private readonly record struct DedupResult(int Shared, int Coplanar, double Ms, long Bytes);

    /// <summary>Time of a step.</summary>
    /// <param name="Ms">Time in milliseconds.</param>
    /// <param name="Bytes">Bytes allocated.</param>
    private readonly record struct Cost(double Ms, long Bytes);

    /// <summary>Reads every triangle near the cube into the world buffer, timed.</summary>
    /// <param name="centre">The player position (the cube's centre).</param>
    /// <param name="failure">Why nothing could be read, or null.</param>
    private unsafe ReadResult MeasureRead(out Vector3 centre, out string? failure)
    {
        centre = default;
        failure = null;
        var control = Control.Instance();
        if (control == null || control->LocalPlayer == null)
        {
            failure = "Not logged in: no local player to read.";
            return default;
        }

        var framework = Framework.Instance();
        var module = framework == null ? null : framework->BGCollisionModule;
        if (module == null || module->SceneManager == null || module->SceneManager->FirstScene == null)
        {
            failure = "No collision scene loaded.";
            return default;
        }

        centre = (Vector3)control->LocalPlayer->Position;

        // The turned cube's world-aligned bounds: what triangles are culled against before the exact clip.
        var reach = this.half * (MathF.Abs(MathF.Cos(Yaw)) + MathF.Abs(MathF.Sin(Yaw)));
        var cullMin = centre - new Vector3(reach, this.half, reach);
        var cullMax = centre + new Vector3(reach, this.half, reach);

        // Warm up once so the JIT isn't in the timing, then measure.
        this.worldCount = 0;
        this.ReadScene(module, cullMin, cullMax, out _, out _);
        this.worldCount = 0;
        var bytes = GC.GetAllocatedBytesForCurrentThread();
        var ticks = Stopwatch.GetTimestamp();
        this.ReadScene(module, cullMin, cullMax, out var inBounds, out var primitive);
        var ms = Stopwatch.GetElapsedTime(ticks).TotalMilliseconds;
        return new ReadResult(inBounds, primitive, ms, GC.GetAllocatedBytesForCurrentThread() - bytes);
    }

    /// <summary>Walks the colliders, culling first, writing triangles to the world buffer.</summary>
    /// <param name="module">The collision module.</param>
    /// <param name="cullMin">Low corner of the cull box.</param>
    /// <param name="cullMax">High corner of the cull box.</param>
    /// <param name="inBounds">Triangles in nodes crossing the cull box.</param>
    /// <param name="primitive">Triangles from primitive colliders.</param>
    private unsafe void ReadScene(BGCollisionModule* module, Vector3 cullMin, Vector3 cullMax, out int inBounds, out int primitive)
    {
        inBounds = 0;
        primitive = 0;
        foreach (var wrapper in module->SceneManager->Scenes)
        {
            var scene = wrapper->Scene;
            if (scene == null)
            {
                continue;
            }

            foreach (var collider in scene->Colliders)
            {
                if ((collider->VisibilityFlags & 1) == 0)
                {
                    continue;
                }

                Vector3 min, max;
                var type = collider->GetColliderType();
                switch (type)
                {
                    case ColliderType.Mesh:
                        var mesh = (ColliderMesh*)collider;
                        min = mesh->WorldBoundingBox.Min;
                        max = mesh->WorldBoundingBox.Max;
                        if (CollisionProbeCheck.Overlaps(min, max, cullMin, cullMax) && !mesh->MeshIsSimple && mesh->Mesh != null)
                        {
                            this.ReadNode(((MeshPCB*)mesh->Mesh)->RootNode, mesh->World, cullMin, cullMax, ref inBounds, 0);
                        }

                        continue;
                    case ColliderType.Box:
                        CollisionProbeCheck.UnitShapeBounds(((ColliderBox*)collider)->World, out min, out max);
                        if (CollisionProbeCheck.Overlaps(min, max, cullMin, cullMax))
                        {
                            primitive += this.AddBox(((ColliderBox*)collider)->World);
                        }

                        break;
                    case ColliderType.Cylinder:
                        CollisionProbeCheck.UnitShapeBounds(((ColliderCylinder*)collider)->World, out min, out max);
                        if (CollisionProbeCheck.Overlaps(min, max, cullMin, cullMax))
                        {
                            primitive += this.AddPrism(((ColliderCylinder*)collider)->World, 8);
                        }

                        break;
                    case ColliderType.Sphere:
                        CollisionProbeCheck.UnitShapeBounds(((ColliderSphere*)collider)->World, out min, out max);
                        if (CollisionProbeCheck.Overlaps(min, max, cullMin, cullMax))
                        {
                            primitive += this.AddIcosahedron(((ColliderSphere*)collider)->World);
                        }

                        break;
                    case ColliderType.Plane:
                    case ColliderType.PlaneTwoSided:
                        CollisionProbeCheck.UnitShapeBounds(((ColliderPlane*)collider)->World, out min, out max);
                        if (CollisionProbeCheck.Overlaps(min, max, cullMin, cullMax))
                        {
                            primitive += this.AddPlane(((ColliderPlane*)collider)->World);
                        }

                        break;
                }
            }
        }
    }

    /// <summary>Reads one mesh node and its children whose boxes cross the cull box.</summary>
    /// <param name="node">The node, or null.</param>
    /// <param name="matrix">The mesh's world matrix.</param>
    /// <param name="cullMin">Low corner of the cull box.</param>
    /// <param name="cullMax">High corner of the cull box.</param>
    /// <param name="inBounds">Running count of triangles in crossing nodes.</param>
    /// <param name="depth">Recursion depth, capped.</param>
    private unsafe void ReadNode(MeshPCB.FileNode* node, in Matrix4x3 matrix, Vector3 cullMin, Vector3 cullMax, ref int inBounds, int depth)
    {
        if (node == null || depth > 64)
        {
            return;
        }

        CollisionProbeCheck.TransformBox(matrix, node->LocalBounds.Min, node->LocalBounds.Max, out var min, out var max);
        if (!CollisionProbeCheck.Overlaps(min, max, cullMin, cullMax))
        {
            return;
        }

        if (node->NumPrims > 0)
        {
            inBounds += node->NumPrims;
            foreach (ref var prim in node->Primitives)
            {
                var a = matrix.TransformCoordinate(node->Vertex(prim.V1));
                var b = matrix.TransformCoordinate(node->Vertex(prim.V2));
                var c = matrix.TransformCoordinate(node->Vertex(prim.V3));
                if (CollisionProbeCheck.Overlaps(Vector3.Min(a, Vector3.Min(b, c)), Vector3.Max(a, Vector3.Max(b, c)), cullMin, cullMax))
                {
                    this.AddTriangle(a, b, c);
                }
            }
        }

        this.ReadNode(node->Child1, matrix, cullMin, cullMax, ref inBounds, depth + 1);
        this.ReadNode(node->Child2, matrix, cullMin, cullMax, ref inBounds, depth + 1);
    }

    /// <summary>Appends a world triangle if there is room.</summary>
    /// <param name="a">First corner.</param>
    /// <param name="b">Second corner.</param>
    /// <param name="c">Third corner.</param>
    private void AddTriangle(Vector3 a, Vector3 b, Vector3 c)
    {
        if (this.worldCount >= ReadCapacity)
        {
            return;
        }

        var w = this.world!;
        var i = this.worldCount++ * 3;
        w[i] = a;
        w[i + 1] = b;
        w[i + 2] = c;
    }

    /// <summary>Adds a unit box (-1..1) through a matrix as 12 triangles.</summary>
    /// <param name="m">The collider's world matrix.</param>
    private int AddBox(in Matrix4x3 m)
    {
        Span<Vector3> p = stackalloc Vector3[8];
        for (var i = 0; i < 8; i++)
        {
            p[i] = m.TransformCoordinate(new Vector3((i & 1) == 0 ? -1 : 1, (i & 2) == 0 ? -1 : 1, (i & 4) == 0 ? -1 : 1));
        }

        ReadOnlySpan<byte> quads = [0, 1, 3, 2, 4, 6, 7, 5, 0, 4, 5, 1, 2, 3, 7, 6, 0, 2, 6, 4, 1, 5, 7, 3];
        for (var q = 0; q < 6; q++)
        {
            this.AddTriangle(p[quads[q * 4]], p[quads[(q * 4) + 1]], p[quads[(q * 4) + 2]]);
            this.AddTriangle(p[quads[q * 4]], p[quads[(q * 4) + 2]], p[quads[(q * 4) + 3]]);
        }

        return 12;
    }

    /// <summary>Adds a flat unit square (-1..1 in X and Y) as 2 triangles.</summary>
    /// <param name="m">The collider's world matrix.</param>
    private int AddPlane(in Matrix4x3 m)
    {
        var a = m.TransformCoordinate(new Vector3(-1, -1, 0));
        var b = m.TransformCoordinate(new Vector3(1, -1, 0));
        var c = m.TransformCoordinate(new Vector3(1, 1, 0));
        var d = m.TransformCoordinate(new Vector3(-1, 1, 0));
        this.AddTriangle(a, b, c);
        this.AddTriangle(a, c, d);
        return 2;
    }

    /// <summary>Adds a unit cylinder (radius 1, height -1..1 along Y) as a prism with caps.</summary>
    /// <param name="m">The collider's world matrix.</param>
    /// <param name="sides">Sides of the prism.</param>
    private int AddPrism(in Matrix4x3 m, int sides)
    {
        var count = 0;
        var topC = m.TransformCoordinate(Vector3.UnitY);
        var botC = m.TransformCoordinate(-Vector3.UnitY);
        for (var i = 0; i < sides; i++)
        {
            var a0 = i * MathF.Tau / sides;
            var a1 = (i + 1) * MathF.Tau / sides;
            var t0 = m.TransformCoordinate(new Vector3(MathF.Cos(a0), 1, MathF.Sin(a0)));
            var t1 = m.TransformCoordinate(new Vector3(MathF.Cos(a1), 1, MathF.Sin(a1)));
            var b0 = m.TransformCoordinate(new Vector3(MathF.Cos(a0), -1, MathF.Sin(a0)));
            var b1 = m.TransformCoordinate(new Vector3(MathF.Cos(a1), -1, MathF.Sin(a1)));
            this.AddTriangle(t0, b0, b1);
            this.AddTriangle(t0, b1, t1);
            this.AddTriangle(topC, t0, t1);
            this.AddTriangle(botC, b1, b0);
            count += 4;
        }

        return count;
    }

    /// <summary>Adds a unit sphere as a 20-triangle icosahedron.</summary>
    /// <param name="m">The collider's world matrix.</param>
    private int AddIcosahedron(in Matrix4x3 m)
    {
        const float T = 1.6180339887f;
        Span<Vector3> v = stackalloc Vector3[12];
        v[0] = new(-1, T, 0); v[1] = new(1, T, 0); v[2] = new(-1, -T, 0); v[3] = new(1, -T, 0);
        v[4] = new(0, -1, T); v[5] = new(0, 1, T); v[6] = new(0, -1, -T); v[7] = new(0, 1, -T);
        v[8] = new(T, 0, -1); v[9] = new(T, 0, 1); v[10] = new(-T, 0, -1); v[11] = new(-T, 0, 1);
        for (var i = 0; i < 12; i++)
        {
            v[i] = m.TransformCoordinate(Vector3.Normalize(v[i]));
        }

        ReadOnlySpan<byte> f = [0, 11, 5, 0, 5, 1, 0, 1, 7, 0, 7, 10, 0, 10, 11, 1, 5, 9, 5, 11, 4, 11, 10, 2, 10, 7, 6, 7, 1, 8, 3, 9, 4, 3, 4, 2, 3, 2, 6, 3, 6, 8, 3, 8, 9, 4, 9, 5, 2, 4, 11, 6, 2, 10, 8, 6, 7, 9, 8, 1];
        for (var i = 0; i < 20; i++)
        {
            this.AddTriangle(v[f[i * 3]], v[f[(i * 3) + 1]], v[f[(i * 3) + 2]]);
        }

        return 20;
    }

    /// <summary>
    /// Clips the world triangles to the turned box in box space (the box's centre at the origin), writing the result to
    /// the clipped buffer; timed. Wholly inside triangles are copied, wholly outside ones dropped, and the rest cut
    /// with Sutherland-Hodgman against the six planes and fanned into triangles.
    /// </summary>
    /// <param name="centre">The cube's centre in the world.</param>
    private ClipResult MeasureClip(Vector3 centre)
    {
        this.RunClip(centre, out _, out _, out _);
        var bytes = GC.GetAllocatedBytesForCurrentThread();
        var ticks = Stopwatch.GetTimestamp();
        this.RunClip(centre, out var inside, out var outside, out var cut);
        var ms = Stopwatch.GetElapsedTime(ticks).TotalMilliseconds;
        return new ClipResult(inside, outside, cut, ms, GC.GetAllocatedBytesForCurrentThread() - bytes);
    }

    /// <summary>One clip pass.</summary>
    /// <param name="centre">The cube's centre in the world.</param>
    /// <param name="inside">Triangles wholly inside.</param>
    /// <param name="outside">Triangles wholly outside.</param>
    /// <param name="cut">Triangles that needed cutting.</param>
    private void RunClip(Vector3 centre, out int inside, out int outside, out int cut)
    {
        inside = 0;
        outside = 0;
        cut = 0;
        this.clippedCount = 0;
        var w = this.world!;
        var o = this.clipped!;
        var cos = MathF.Cos(-Yaw);
        var sin = MathF.Sin(-Yaw);
        var h = this.half;
        Span<Vector3> a = stackalloc Vector3[16];
        Span<Vector3> b = stackalloc Vector3[16];
        for (var t = 0; t < this.worldCount; t++)
        {
            var i = t * 3;
            var p0 = ToBox(w[i] - centre, cos, sin);
            var p1 = ToBox(w[i + 1] - centre, cos, sin);
            var p2 = ToBox(w[i + 2] - centre, cos, sin);
            var f0 = Outcode(p0, h);
            var f1 = Outcode(p1, h);
            var f2 = Outcode(p2, h);
            if ((f0 & f1 & f2) != 0)
            {
                outside++;
                continue;
            }

            if ((f0 | f1 | f2) == 0)
            {
                inside++;
                if (this.clippedCount < ClipCapacity)
                {
                    var k = this.clippedCount++ * 3;
                    o[k] = p0;
                    o[k + 1] = p1;
                    o[k + 2] = p2;
                }

                continue;
            }

            cut++;
            a[0] = p0;
            a[1] = p1;
            a[2] = p2;
            var n = 3;
            for (var plane = 0; plane < 6 && n > 0; plane++)
            {
                n = ClipPlane(a, n, b, plane, h);
                var swap = a;
                a = b;
                b = swap;
            }

            for (var v = 1; v + 1 < n && this.clippedCount < ClipCapacity; v++)
            {
                var k = this.clippedCount++ * 3;
                o[k] = a[0];
                o[k + 1] = a[v];
                o[k + 2] = a[v + 1];
            }
        }
    }

    /// <summary>Rotates a world offset into box space.</summary>
    /// <param name="d">Offset from the box's centre.</param>
    /// <param name="cos">Cosine of the inverse turn.</param>
    /// <param name="sin">Sine of the inverse turn.</param>
    private static Vector3 ToBox(Vector3 d, float cos, float sin) => new((d.X * cos) + (d.Z * sin), d.Y, (-d.X * sin) + (d.Z * cos));

    /// <summary>Which of the six planes a point is outside, as bits.</summary>
    /// <param name="p">A point in box space.</param>
    /// <param name="h">The box's half edge.</param>
    private static int Outcode(Vector3 p, float h) =>
        (p.X < -h ? 1 : 0) | (p.X > h ? 2 : 0) | (p.Y < -h ? 4 : 0) | (p.Y > h ? 8 : 0) | (p.Z < -h ? 16 : 0) | (p.Z > h ? 32 : 0);

    /// <summary>The signed distance of a point inside one of the six planes (positive is inside).</summary>
    /// <param name="p">A point.</param>
    /// <param name="plane">0..5: -X, +X, -Y, +Y, -Z, +Z.</param>
    /// <param name="h">The box's half edge.</param>
    private static float Inside(Vector3 p, int plane, float h) => plane switch
    {
        0 => p.X + h,
        1 => h - p.X,
        2 => p.Y + h,
        3 => h - p.Y,
        4 => p.Z + h,
        _ => h - p.Z,
    };

    /// <summary>One Sutherland-Hodgman step: clips a polygon against one plane.</summary>
    /// <param name="input">The polygon.</param>
    /// <param name="count">Its corner count.</param>
    /// <param name="output">Where the clipped polygon goes.</param>
    /// <param name="plane">The plane.</param>
    /// <param name="h">The box's half edge.</param>
    private static int ClipPlane(ReadOnlySpan<Vector3> input, int count, Span<Vector3> output, int plane, float h)
    {
        var n = 0;
        var prev = input[count - 1];
        var dPrev = Inside(prev, plane, h);
        for (var i = 0; i < count; i++)
        {
            var cur = input[i];
            var d = Inside(cur, plane, h);
            if ((d >= 0) != (dPrev >= 0))
            {
                output[n++] = Vector3.Lerp(prev, cur, dPrev / (dPrev - d));
            }

            if (d >= 0)
            {
                output[n++] = cur;
            }

            prev = cur;
            dPrev = d;
        }

        return n;
    }

    /// <summary>Deduplicates shared edges across the clipped triangles; timed.</summary>
    private DedupResult MeasureDedup()
    {
        this.RunDedup(out _, out _);
        var bytes = GC.GetAllocatedBytesForCurrentThread();
        var ticks = Stopwatch.GetTimestamp();
        this.RunDedup(out var shared, out var coplanar);
        var ms = Stopwatch.GetElapsedTime(ticks).TotalMilliseconds;
        return new DedupResult(shared, coplanar, ms, GC.GetAllocatedBytesForCurrentThread() - bytes);
    }

    /// <summary>One edge pass: hashes quantised vertex pairs into the open-addressing table.</summary>
    /// <param name="shared">Edges met a second time.</param>
    /// <param name="coplanar">Of those, edges whose two triangles are coplanar.</param>
    private void RunDedup(out int shared, out int coplanar)
    {
        shared = 0;
        coplanar = 0;
        this.edgeCount = 0;
        var keys = this.edgeKeys!;
        var tris = this.edgeTri!;
        var list = this.edges!;
        Array.Clear(keys);
        var c = this.clipped!;
        for (var t = 0; t < this.clippedCount; t++)
        {
            var i = t * 3;
            for (var e = 0; e < 3; e++)
            {
                var va = c[i + e];
                var vb = c[i + ((e + 1) % 3)];
                var ka = Quantise(va);
                var kb = Quantise(vb);
                if (ka == kb)
                {
                    continue;
                }

                var key = (((Math.Min(ka, kb) * 0x9E3779B97F4A7C15UL) ^ (Math.Max(ka, kb) * 0xC2B2AE3D27D4EB4FUL)) | 1UL);
                var slot = (int)((key >> 20) & (EdgeSlots - 1));
                while (true)
                {
                    if (keys[slot] == 0)
                    {
                        keys[slot] = key;
                        tris[slot] = t;
                        if (this.edgeCount * 2 + 1 < list.Length)
                        {
                            list[this.edgeCount * 2] = va;
                            list[(this.edgeCount * 2) + 1] = vb;
                            this.edgeCount++;
                        }

                        break;
                    }

                    if (keys[slot] == key)
                    {
                        shared++;
                        if (Coplanar(c, tris[slot] * 3, i))
                        {
                            coplanar++;
                        }

                        break;
                    }

                    slot = (slot + 1) & (EdgeSlots - 1);
                }
            }
        }
    }

    /// <summary>Packs a box-space point into 36 bits at 1/32 yalm.</summary>
    /// <param name="v">The point.</param>
    private static ulong Quantise(Vector3 v) =>
        ((ulong)(uint)(int)MathF.Round((v.X * VertexScale) + 4096) & 0xFFF) |
        (((ulong)(uint)(int)MathF.Round((v.Y * VertexScale) + 4096) & 0xFFF) << 12) |
        (((ulong)(uint)(int)MathF.Round((v.Z * VertexScale) + 4096) & 0xFFF) << 24);

    /// <summary>Whether two triangles (by first vertex index) have nearly the same normal.</summary>
    /// <param name="c">The clipped vertex buffer.</param>
    /// <param name="a">First triangle's first vertex index.</param>
    /// <param name="b">Second triangle's first vertex index.</param>
    private static bool Coplanar(Vector3[] c, int a, int b)
    {
        var na = Vector3.Cross(c[a + 1] - c[a], c[a + 2] - c[a]);
        var nb = Vector3.Cross(c[b + 1] - c[b], c[b + 2] - c[b]);
        var l = na.Length() * nb.Length();
        return l > 0 && MathF.Abs(Vector3.Dot(na, nb)) / l > 0.9999f;
    }

    /// <summary>Projects and sorts the clipped triangles once, timed; what a view change would cost.</summary>
    private Cost MeasureProject()
    {
        this.ProjectAndSort(0.2f, true);
        var bytes = GC.GetAllocatedBytesForCurrentThread();
        var ticks = Stopwatch.GetTimestamp();
        this.ProjectAndSort(0.3f, true);
        var ms = Stopwatch.GetElapsedTime(ticks).TotalMilliseconds;
        return new Cost(ms, GC.GetAllocatedBytesForCurrentThread() - bytes);
    }

    /// <summary>Counts clipped triangles whose projected bounds are under one pixel in both directions.</summary>
    /// <param name="size">The cube's edge.</param>
    private int CountSmall(float size)
    {
        var scale = 300f / size;
        var c = this.clipped!;
        var n = 0;
        for (var t = 0; t < this.clippedCount; t++)
        {
            var i = t * 3;
            var d = Vector3.Max(Vector3.Abs(c[i] - c[i + 1]), Vector3.Max(Vector3.Abs(c[i] - c[i + 2]), Vector3.Abs(c[i + 1] - c[i + 2])));
            if (MathF.Max(d.X, MathF.Max(d.Y, d.Z)) * scale < 1f)
            {
                n++;
            }
        }

        return n;
    }

    /// <summary>
    /// Projects every clipped triangle (orthographic, turned by <paramref name="yaw"/> and tilted) into the window's
    /// region, flat-shades it by its normal, and depth-sorts the triangle indexes; also projects the edges.
    /// </summary>
    /// <param name="yaw">The view's turn in radians.</param>
    /// <param name="all">Whether to project triangles and edges both (otherwise only what the mode draws).</param>
    private void ProjectAndSort(float yaw, bool all)
    {
        var win = this.window!;
        var centre = win.LastRegionMin + (win.LastRegionSize / 2f);
        var scale = MathF.Min(win.LastRegionSize.X, win.LastRegionSize.Y) / (this.half * 2f * 1.8f);
        var rotation = Matrix4x4.CreateRotationY(yaw) * Matrix4x4.CreateRotationX(Tilt);
        var c = this.clipped!;
        var s = this.triScreen!;
        var colours = this.triColour!;
        var keys = this.keys!;
        var order = this.order!;
        var light = Vector3.Normalize(new Vector3(0.4f, 1f, 0.3f));
        var drawTris = all || this.mode != Mode.Edges;
        if (drawTris)
        {
            for (var t = 0; t < this.clippedCount; t++)
            {
                var i = t * 3;
                var a = Vector3.Transform(c[i], rotation);
                var b = Vector3.Transform(c[i + 1], rotation);
                var d = Vector3.Transform(c[i + 2], rotation);
                s[i] = centre + (new Vector2(a.X, -a.Y) * scale);
                s[i + 1] = centre + (new Vector2(b.X, -b.Y) * scale);
                s[i + 2] = centre + (new Vector2(d.X, -d.Y) * scale);
                keys[t] = (a.Z + b.Z + d.Z) * (1f / 3f);
                order[t] = t;
                var n = Vector3.Cross(c[i + 1] - c[i], c[i + 2] - c[i]);
                var len = n.Length();
                var shade = len > 0 ? 0.35f + (0.65f * MathF.Abs(Vector3.Dot(n / len, light))) : 0.35f;
                var g = (uint)(shade * 255f);
                colours[t] = 0x80000000u | (g << 16) | (g << 8) | g;
            }

            Array.Sort(keys, order, 0, this.clippedCount);
        }

        if (all || this.mode != Mode.Filled)
        {
            var e = this.edges!;
            var es = this.edgeScreen!;
            for (var k = 0; k < this.edgeCount * 2; k++)
            {
                var p = Vector3.Transform(e[k], rotation);
                es[k] = centre + (new Vector2(p.X, -p.Y) * scale);
            }
        }
    }

    /// <summary>
    /// Runs inside the window's draw: when armed, re-projects (if turning) and issues the current mode's ImGui calls on
    /// the window's draw list, timing the lot and counting allocations.
    /// </summary>
    /// <param name="target">The window's draw list.</param>
    private void DrawHook(ImDrawListPtr target)
    {
        if (!this.armed || this.clipped is null)
        {
            return;
        }

        var bytes = GC.GetAllocatedBytesForCurrentThread();
        var ticks = Stopwatch.GetTimestamp();
        if (this.turning)
        {
            this.viewYaw += 0.05f;
            this.ProjectAndSort(this.viewYaw, false);
        }
        else if (this.fresh)
        {
            // The first frame of an idle cell projects once; later frames reuse it.
            this.fresh = false;
            this.ProjectAndSort(0.3f, false);
        }

        var prims = 0;
        this.capped = false;
        if (this.mode != Mode.Edges)
        {
            var s = this.triScreen!;
            var order = this.order!;
            var colours = this.triColour!;
            for (var n = 0; n < this.clippedCount; n++)
            {
                if (prims >= DrawCap)
                {
                    this.capped = true;
                    break;
                }

                var t = order[n];
                var i = t * 3;
                target.AddTriangleFilled(s[i], s[i + 1], s[i + 2], colours[t]);
                prims++;
            }
        }

        if (this.mode != Mode.Filled)
        {
            var es = this.edgeScreen!;
            for (var k = 0; k < this.edgeCount; k++)
            {
                if (prims >= DrawCap)
                {
                    this.capped = true;
                    break;
                }

                target.AddLine(es[k * 2], es[(k * 2) + 1], 0xC0FFFFFFu, 1f);
                prims++;
            }
        }

        this.hookMs = Stopwatch.GetElapsedTime(ticks).TotalMilliseconds;
        this.hookBytes = GC.GetAllocatedBytesForCurrentThread() - bytes;
        this.hookPrims = prims;
    }
}
