using System;
using System.Numerics;
using Cube.Core;
using Cube.Game;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
#if DEBUG
using System.Diagnostics;
#endif

namespace Cube;

/// <summary>
/// The window the cube is drawn in (PROJECT.md, "Features": a Dalamud window toggled by <c>/cube</c>, showing a
/// wireframe cube with the player at its centre).
/// </summary>
/// <remarks>
/// Each frame: build the <see cref="CubeSpace"/> and fitted <see cref="ViewProjection"/>, let Core fill the reused
/// <see cref="DrawList"/>, sort it far to near, then issue one ImGui call per primitive. Nothing here allocates
/// (PROJECT.md, "Efficiency and memory"); Dalamud skips <see cref="Draw"/> entirely while the window is closed.
/// </remarks>
internal sealed class CubeWindow : Window
{
    /// <summary>
    /// Most primitives a frame can hold: the 12 cube edges plus three faces for every character slot in the
    /// object table (200), with room to spare. Fixed, so memory never grows (PROJECT.md: bounded memory).
    /// </summary>
    internal const int DrawListCapacity = 1024;

    /// <summary>Colour of the big cube's edges: light grey, slightly transparent.</summary>
    private static readonly uint WireColour = Colour.Pack(0.85f, 0.85f, 0.85f, 0.8f);

    /// <summary>Thickness of the big cube's edges, in pixels.</summary>
    private const float WireThickness = 1.5f;

    /// <summary>The view's elevation in radians (PROJECT.md: 30°).</summary>
    private static readonly float Elevation = ViewProjection.DefaultElevationDegrees * MathF.PI / 180f;

    /// <summary>The view's current yaw; kept between frames so a vertical camera doesn't spin the view.</summary>
    private float yaw;

    /// <summary>Creates the window, closed, with a sensible minimum size.</summary>
    public CubeWindow()
        : base("Cube###CubeMain")
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(200, 200),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue),
        };
    }

    /// <summary>This frame's primitives, in drawing order. Read by the test bridge.</summary>
    internal DrawList DrawList { get; } = new(DrawListCapacity);

    /// <summary>The cube space used for the last frame drawn, so the bridge can rebuild the frame independently.</summary>
    internal CubeSpace LastSpace { get; private set; }

    /// <summary>The projection used for the last frame drawn.</summary>
    internal ViewProjection LastProjection { get; private set; }

    /// <summary>How many ImGui draw calls the last frame issued; the bridge checks it equals the draw list's count.</summary>
    internal int LastSubmitted { get; private set; }

    /// <summary>Top-left of the last frame's drawing region, in screen pixels; the snapshot crops to this region.</summary>
    internal Vector2 LastRegionMin { get; private set; }

    /// <summary>Size of the last frame's drawing region, in pixels.</summary>
    internal Vector2 LastRegionSize { get; private set; }

    /// <summary>Thickness of the big cube's edges, in pixels; the snapshot draws lines with it.</summary>
    internal static float LineThickness => WireThickness;

#if DEBUG
    /// <summary>
    /// Time and allocations of each <see cref="Draw"/> call, read by the in-game test bridge to check
    /// PROJECT.md's budgets (under 0.5 ms average, zero bytes per frame). Debug builds only.
    /// </summary>
    /// <remarks>Holds the last 240 frames (about 4 seconds at 60 fps); fixed size, so measuring allocates nothing.</remarks>
    internal DrawCostSampler DrawCost { get; } = new(240, Stopwatch.Frequency);
#endif

    /// <summary>
    /// Builds this frame's draw list for a given drawing region. Separate from the ImGui calls so the bridge can
    /// check exactly what Core produced.
    /// </summary>
    /// <param name="list">The list to fill (cleared first).</param>
    /// <param name="space">The cube.</param>
    /// <param name="projection">The projection.</param>
    internal static void BuildFrame(DrawList list, in CubeSpace space, in ViewProjection projection)
    {
        list.Clear();
        CubeGeometry.AddWireCube(list, space, projection, WireColour);
        list.SortBackToFront();
    }

    /// <summary>Draws the window's contents, measuring the cost in Debug builds.</summary>
    public override void Draw()
    {
#if DEBUG
        var startBytes = GC.GetAllocatedBytesForCurrentThread();
        var startTicks = Stopwatch.GetTimestamp();
        this.DrawContents();
        var elapsed = Stopwatch.GetTimestamp() - startTicks;
        this.DrawCost.Record(elapsed, GC.GetAllocatedBytesForCurrentThread() - startBytes);
#else
        this.DrawContents();
#endif
    }

    /// <summary>The plugin's per-frame draw path: builds the frame with Core and draws it.</summary>
    private void DrawContents()
    {
        if (CameraReader.TryGetForward(out var forward))
        {
            this.yaw = ViewYaw.For(OrientationMode.Camera, forward, this.yaw);
        }

        // The cube fills the content region and keeps its shape on resize: Fit uses one scale for both axes.
        var regionMin = ImGui.GetCursorScreenPos();
        var regionSize = ImGui.GetContentRegionAvail();
        var space = new CubeSpace(Vector3.Zero, CubeSpace.DefaultEdge, this.yaw);
        var projection = ViewProjection.Fit(regionMin, regionSize, space.HalfEdge, Elevation);

        BuildFrame(this.DrawList, space, projection);
        this.LastSpace = space;
        this.LastProjection = projection;
        this.LastRegionMin = regionMin;
        this.LastRegionSize = regionSize;
        this.LastSubmitted = Submit(ImGui.GetWindowDrawList(), this.DrawList.Items);
    }

    /// <summary>Issues one ImGui call per primitive, in order.</summary>
    /// <param name="target">The window's ImGui draw list.</param>
    /// <param name="items">The sorted primitives.</param>
    /// <returns>How many calls were made.</returns>
    private static int Submit(ImDrawListPtr target, ReadOnlySpan<DrawPrimitive> items)
    {
        var submitted = 0;
        foreach (ref readonly var p in items)
        {
            if (p.Kind == PrimitiveKind.Line)
            {
                target.AddLine(p.A, p.B, p.Colour, WireThickness);
            }
            else
            {
                target.AddQuadFilled(p.A, p.B, p.C, p.D, p.Colour);
            }

            submitted++;
        }

        return submitted;
    }
}
