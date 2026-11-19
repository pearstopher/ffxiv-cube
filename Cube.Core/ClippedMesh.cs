using System.Diagnostics;
using System.Numerics;

namespace Cube.Core;

/// <summary>
/// The Mesh style's drawing data (#100; PROJECT.md, "Collision style"): the cached triangles' edges, clipped to the
/// cube, with exact duplicates removed, and projected to the window. Everything is rebuilt only when its inputs change,
/// and a rebuild is spread over frames while the previous result keeps being drawn.
/// </summary>
/// <remarks>
/// <para>
/// <b>What is shown.</b> Every edge of every cached triangle, clipped exactly to the cube (<see cref="SegmentClip"/>).
/// An edge shared by two triangles is stored once (an exact match of its clipped corners, so nothing that is drawn
/// changes); edges wholly outside the cube are skipped before anything else is computed for them.
/// </para>
/// <para>
/// <b>Rebuilt only when needed.</b> The inputs are the gathered triangle set (which set, and the cache's completed
/// count) and the cube's centre, size and yaw. The cube turns with the camera, so a turning camera is a new input every
/// frame; a rebuild reads tens of thousands of triangles, so it is done in slices of <c>budgetTicks</c> per frame. A
/// started rebuild is always finished (never restarted for a newer input, or a turning camera would never finish) and the
/// next one starts after it. Meanwhile the previous result is drawn: its edges are moved through the world into the current
/// cube and clipped to it again at projection time (<see cref="Project"/>), so a stale result lags the camera by a few
/// frames but never shows anything outside the cube.
/// </para>
/// <para>
/// <b>Bounded.</b> Two edge buffers (built and ready, swapped) and a hash table are allocated on first use and dropped by
/// <see cref="Release"/>. An edge that doesn't fit is counted in <see cref="Dropped"/> for the plugin to report; nothing
/// is cut silently. Nothing here allocates once the buffers exist.
/// </para>
/// </remarks>
public sealed class ClippedMesh
{
    /// <summary>
    /// Default most edges kept: a triangle has three edges and neighbours share them, so 131,072 triangles (the
    /// <see cref="TriangleSet"/> default) give at most about 200,000 unique edges. 24 bytes each, 4.7 MB per buffer.
    /// </summary>
    public const int DefaultEdgeCapacity = 196_608;

    /// <summary>How many triangles are processed between looks at the clock.</summary>
    private const int ClockInterval = 256;

    /// <summary>The edges' ends in cube-local coordinates of <see cref="readyCube"/>, two per edge.</summary>
    private Vector3[]? readyEdges;

    /// <summary>The edges being built, for <see cref="buildCube"/>.</summary>
    private Vector3[]? buildEdges;

    /// <summary>The projected ends of the ready edges, two per kept edge.</summary>
    private Vector2[]? screen;

    /// <summary>The hash table of edge numbers plus one (0 is empty) used while building.</summary>
    private int[]? table;

    /// <summary>How many edges the ready buffer holds.</summary>
    private int readyCount;

    /// <summary>The cube the ready edges were clipped for.</summary>
    private CubeSpace readyCube;

    /// <summary>The set the ready edges were built from.</summary>
    private TriangleSet? readySet;

    /// <summary>The cache's completed count when the ready edges were built.</summary>
    private long readyCompleted = -1;

    /// <summary>Whether the ready buffer holds a finished build.</summary>
    private bool hasReady;

    /// <summary>Whether a build is in progress.</summary>
    private bool building;

    /// <summary>The cube the build in progress clips to.</summary>
    private CubeSpace buildCube;

    /// <summary>The set being read by the build.</summary>
    private TriangleSet? buildSet;

    /// <summary>The cache's completed count the build started from.</summary>
    private long buildCompleted;

    /// <summary>The next triangle the build processes.</summary>
    private int nextTriangle;

    /// <summary>Edges the build has kept so far.</summary>
    private int buildCount;

    /// <summary>Edges the build has had to drop for lack of room.</summary>
    private int buildDropped;

    /// <summary>The edge capacity.</summary>
    private readonly int edgeCapacity;

    /// <summary>What the screen positions were last projected for: the cube, the view and the ready build's number.</summary>
    private CubeSpace projectedCube;

    /// <summary>The projection the screen positions were last made with.</summary>
    private ViewProjection projectedView;

    /// <summary>The number of the build the screen positions came from; -1 when none.</summary>
    private long projectedBuild = -1;

    /// <summary>Creates an empty mesh; no buffer is allocated until the first build.</summary>
    /// <param name="edgeCapacity">Most unique edges kept per buffer.</param>
    public ClippedMesh(int edgeCapacity = DefaultEdgeCapacity)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(edgeCapacity, 1);
        this.edgeCapacity = edgeCapacity;
    }

    /// <summary>How many unique edges the ready build holds.</summary>
    public int Count => this.readyCount;

    /// <summary>How many builds have finished.</summary>
    public long Builds { get; private set; }

    /// <summary>How many segments the last <see cref="Project"/> made (edges still inside the current cube); two corners each.</summary>
    public int ProjectedCount { get; private set; }

    /// <summary>How many times <see cref="Project"/> actually projected (not just reused its result).</summary>
    public long Projections { get; private set; }

    /// <summary>How many edges the ready build couldn't keep for lack of room; plugin and bridge report it.</summary>
    public int Dropped { get; private set; }

    /// <summary>The largest <see cref="Dropped"/> of any finished build.</summary>
    public int LargestDropped { get; private set; }

    /// <summary>Whether a build is in progress, so the picture lags the inputs.</summary>
    public bool IsBuilding => this.building;

    /// <summary>Whether the buffers are allocated.</summary>
    public bool IsAllocated => this.readyEdges is not null;

    /// <summary>The cube the ready edges were clipped for.</summary>
    public CubeSpace BuiltFor => this.readyCube;

    /// <summary>The ready edges' ends in cube-local coordinates of <see cref="BuiltFor"/>, two per edge.</summary>
    public ReadOnlySpan<Vector3> Edges => this.readyEdges is null ? default : this.readyEdges.AsSpan(0, this.readyCount * 2);

    /// <summary>The projected segments' ends, two per segment, as of the last <see cref="Project"/>.</summary>
    public ReadOnlySpan<Vector2> Screen => this.screen is null ? default : this.screen.AsSpan(0, this.ProjectedCount * 2);

    /// <summary>Drops the result and any build in progress; buffers are kept.</summary>
    public void Clear()
    {
        this.hasReady = false;
        this.building = false;
        this.readyCount = 0;
        this.readySet = null;
        this.readyCompleted = -1;
        this.projectedBuild = -1;
        this.ProjectedCount = 0;
        this.Dropped = 0;
    }

    /// <summary>Drops the buffers as well, so Boxes mode holds no Mesh memory.</summary>
    public void Release()
    {
        this.Clear();
        this.readyEdges = null;
        this.buildEdges = null;
        this.screen = null;
        this.table = null;
    }

    /// <summary>
    /// Brings the clipped edges up to date with the cube and the gathered triangles, spending at most
    /// <paramref name="budgetTicks"/> of <see cref="Stopwatch"/> time on a rebuild this frame.
    /// </summary>
    /// <param name="cube">This frame's cube.</param>
    /// <param name="cache">The triangle cache.</param>
    /// <param name="budgetTicks">The most clock ticks this call may spend building.</param>
    /// <returns>True if a build finished, so the edges changed.</returns>
    public bool Update(in CubeSpace cube, TriangleCache cache, long budgetTicks)
    {
        var set = cache.Ready;
        if (set is null || !set.HasData)
        {
            // Nothing gathered (zone change, collision off): nothing to show, and nothing kept from the old place.
            var had = this.hasReady || this.building;
            this.Clear();
            return had;
        }

        if (this.building && (!ReferenceEquals(set, this.buildSet) || cache.Completed != this.buildCompleted))
        {
            // The triangles changed under the build; start over from the new set.
            this.building = false;
        }

        if (!this.building)
        {
            if (this.hasReady && ReferenceEquals(set, this.readySet) && cache.Completed == this.readyCompleted &&
                cube.Centre == this.readyCube.Centre && cube.Size == this.readyCube.Size && cube.Yaw == this.readyCube.Yaw)
            {
                return false;
            }

            this.Begin(cube, set, cache.Completed);
        }

        return this.Build(budgetTicks);
    }

    /// <summary>Allocates the buffers if needed and starts a build for a cube.</summary>
    /// <param name="cube">The cube to clip to.</param>
    /// <param name="set">The triangles.</param>
    /// <param name="completed">The cache's completed count.</param>
    private void Begin(in CubeSpace cube, TriangleSet set, long completed)
    {
        if (this.readyEdges is null || this.buildEdges is null || this.table is null || this.screen is null)
        {
            this.readyEdges = new Vector3[this.edgeCapacity * 2];
            this.buildEdges = new Vector3[this.edgeCapacity * 2];
            this.screen = new Vector2[this.edgeCapacity * 2];
            this.table = new int[(int)BitOperations.RoundUpToPowerOf2((uint)this.edgeCapacity * 2)];
        }

        this.buildCube = cube;
        this.buildSet = set;
        this.buildCompleted = completed;
        this.nextTriangle = 0;
        this.buildCount = 0;
        this.buildDropped = 0;
        Array.Clear(this.table);
        this.building = true;
    }

    /// <summary>Processes triangles until the budget is spent or they run out.</summary>
    /// <param name="budgetTicks">The most ticks to spend.</param>
    /// <returns>True if the build finished.</returns>
    private bool Build(long budgetTicks)
    {
        var started = Stopwatch.GetTimestamp();
        var corners = this.buildSet!.Corners;
        var triangles = corners.Length / 3;
        var cube = this.buildCube;
        var half = cube.Half;
        var next = this.nextTriangle;
        while (next < triangles)
        {
            var end = Math.Min(triangles, next + ClockInterval);
            for (; next < end; next++)
            {
                this.AddTriangle(cube, half, corners[next * 3], corners[(next * 3) + 1], corners[(next * 3) + 2]);
            }

            if (next < triangles && Stopwatch.GetTimestamp() - started >= budgetTicks)
            {
                this.nextTriangle = next;
                return false;
            }
        }

        (this.readyEdges, this.buildEdges) = (this.buildEdges, this.readyEdges);
        this.readyCount = this.buildCount;
        this.readyCube = cube;
        this.readySet = this.buildSet;
        this.readyCompleted = this.buildCompleted;
        this.hasReady = true;
        this.building = false;
        this.Dropped = this.buildDropped;
        this.LargestDropped = Math.Max(this.LargestDropped, this.buildDropped);
        this.Builds++;
        return true;
    }

    /// <summary>Clips a triangle's three edges to the cube and keeps those that remain and are new.</summary>
    /// <param name="cube">The cube.</param>
    /// <param name="half">The cube's half extents.</param>
    /// <param name="a">First corner, in world coordinates.</param>
    /// <param name="b">Second corner.</param>
    /// <param name="c">Third corner.</param>
    private void AddTriangle(in CubeSpace cube, Vector3 half, Vector3 a, Vector3 b, Vector3 c)
    {
        var va = cube.ToView(a);
        var vb = cube.ToView(b);
        var vc = cube.ToView(c);
        var fa = SegmentClip.Outcode(va, half);
        var fb = SegmentClip.Outcode(vb, half);
        var fc = SegmentClip.Outcode(vc, half);
        if ((fa & fb & fc) != 0)
        {
            return;
        }

        this.AddEdge(va, vb, fa, fb, half);
        this.AddEdge(vb, vc, fb, fc, half);
        this.AddEdge(vc, va, fc, fa, half);
    }

    /// <summary>
    /// Clips one edge and keeps it unless an identical one is already kept. The ends are put in a fixed order first, so
    /// the same edge met from either neighbouring triangle clips to the same numbers and matches exactly.
    /// </summary>
    /// <param name="p">One end, in cube-local coordinates.</param>
    /// <param name="q">The other end.</param>
    /// <param name="codeP">The outcode of <paramref name="p"/>.</param>
    /// <param name="codeQ">The outcode of <paramref name="q"/>.</param>
    /// <param name="half">The cube's half extents.</param>
    private void AddEdge(Vector3 p, Vector3 q, int codeP, int codeQ, Vector3 half)
    {
        if ((codeP & codeQ) != 0)
        {
            return;
        }

        if (Before(q, p))
        {
            (p, q) = (q, p);
            (codeP, codeQ) = (codeQ, codeP);
        }

        if (!SegmentClip.Clip(ref p, ref q, half, codeP, codeQ))
        {
            return;
        }

        var table = this.table!;
        var edges = this.buildEdges!;
        var mask = table.Length - 1;
        var slot = (int)(Hash(p, q) & (uint)mask);
        while (true)
        {
            var entry = table[slot];
            if (entry == 0)
            {
                break;
            }

            var at = (entry - 1) * 2;
            if (edges[at] == p && edges[at + 1] == q)
            {
                return;
            }

            slot = (slot + 1) & mask;
        }

        if (this.buildCount >= this.edgeCapacity)
        {
            this.buildDropped++;
            return;
        }

        var i = this.buildCount * 2;
        edges[i] = p;
        edges[i + 1] = q;
        table[slot] = ++this.buildCount;
    }

    /// <summary>Whether <paramref name="a"/> sorts before <paramref name="b"/> by X, then Y, then Z.</summary>
    /// <param name="a">One point.</param>
    /// <param name="b">The other.</param>
    private static bool Before(Vector3 a, Vector3 b) =>
        a.X != b.X ? a.X < b.X : a.Y != b.Y ? a.Y < b.Y : a.Z < b.Z;

    /// <summary>A hash of both ends' float bits.</summary>
    /// <param name="p">First end.</param>
    /// <param name="q">Second end.</param>
    private static uint Hash(Vector3 p, Vector3 q)
    {
        unchecked
        {
            var h = 2166136261u;
            h = (h ^ BitConverter.SingleToUInt32Bits(p.X)) * 16777619u;
            h = (h ^ BitConverter.SingleToUInt32Bits(p.Y)) * 16777619u;
            h = (h ^ BitConverter.SingleToUInt32Bits(p.Z)) * 16777619u;
            h = (h ^ BitConverter.SingleToUInt32Bits(q.X)) * 16777619u;
            h = (h ^ BitConverter.SingleToUInt32Bits(q.Y)) * 16777619u;
            h = (h ^ BitConverter.SingleToUInt32Bits(q.Z)) * 16777619u;
            return h ^ (h >> 15);
        }
    }

    /// <summary>
    /// Projects the ready edges to the window, only when the cube, the view or the edges changed since the last call.
    /// When the edges were clipped for an older cube (a rebuild is still going), each is carried into the current cube
    /// through the world and clipped to it, so nothing is ever drawn outside the cube (PROJECT.md, "Collision style").
    /// </summary>
    /// <param name="cube">This frame's cube.</param>
    /// <param name="projection">This frame's projection.</param>
    /// <returns>True if the screen segments were made again.</returns>
    public bool Project(in CubeSpace cube, in ViewProjection projection)
    {
        if (!this.hasReady || this.readyEdges is null || this.screen is null)
        {
            this.ProjectedCount = 0;
            return false;
        }

        if (this.projectedBuild == this.Builds && cube.Centre == this.projectedCube.Centre && cube.Size == this.projectedCube.Size &&
            cube.Yaw == this.projectedCube.Yaw && projection.Origin == this.projectedView.Origin &&
            projection.PixelsPerYalm == this.projectedView.PixelsPerYalm &&
            projection.Orientation.Rotation == this.projectedView.Orientation.Rotation)
        {
            return false;
        }

        var edges = this.readyEdges;
        var output = this.screen;
        var fresh = cube.Centre == this.readyCube.Centre && cube.Size == this.readyCube.Size && cube.Yaw == this.readyCube.Yaw;
        var half = cube.Half;
        var kept = 0;
        for (var i = 0; i < this.readyCount; i++)
        {
            var p = edges[i * 2];
            var q = edges[(i * 2) + 1];
            if (!fresh)
            {
                p = cube.ToView(this.readyCube.FromView(p));
                q = cube.ToView(this.readyCube.FromView(q));
                if (!SegmentClip.Clip(ref p, ref q, half))
                {
                    continue;
                }
            }

            output[kept * 2] = projection.ToScreen(p);
            output[(kept * 2) + 1] = projection.ToScreen(q);
            kept++;
        }

        this.ProjectedCount = kept;
        this.projectedCube = cube;
        this.projectedView = projection;
        this.projectedBuild = this.Builds;
        this.Projections++;
        return true;
    }
}
