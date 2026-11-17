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
    /// Most primitives a frame can hold: the 12 cube edges plus three faces and a label for every character slot in
    /// the object table (200), with room to spare. Fixed, so memory never grows (PROJECT.md: bounded memory).
    /// </summary>
    internal const int DrawListCapacity = 1024;

    /// <summary>Colour of the big cube's edges: light grey.</summary>
    private static readonly Vector3 WireRgb = new(0.85f, 0.85f, 0.85f);

    /// <summary>The edges' own opacity (slightly transparent), before the opacity setting is applied.</summary>
    private const float WireAlpha = 0.8f;

    /// <summary>Thickness of the big cube's edges, in pixels.</summary>
    private const float WireThickness = 1.5f;

    /// <summary>The view's elevation in radians (PROJECT.md: 30°).</summary>
    private static readonly float Elevation = ViewProjection.DefaultElevationDegrees * MathF.PI / 180f;

    /// <summary>The view's current yaw; kept between frames so a vertical camera doesn't spin the view.</summary>
    private float yaw;

    /// <summary>Whether <see cref="LastSpace"/> holds a real frame's cube, so its rotation can be reused.</summary>
    private bool hasSpace;

    /// <summary>The <see cref="CubeSettings.Revision"/> the cached colours were built from; -1 forces a first build.</summary>
    private int settingsRevision = -1;

    /// <summary>Creates the window, closed, with a sensible minimum size.</summary>
    /// <param name="settings">The user's settings, read every frame; changes apply on the next frame.</param>
    public CubeWindow(CubeSettings settings)
        : base("Cube###CubeMain")
    {
        this.Settings = settings;
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(200, 200),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue),
        };
    }

    /// <summary>
    /// Name cache slots: the 200 character slots, plus room for the Debug-only fake players above them (#16).
    /// </summary>
    internal const int NameSlots = 256;

    /// <summary>This frame's primitives, in drawing order. Read by the test bridge.</summary>
    internal DrawList DrawList { get; } = new(DrawListCapacity);

    /// <summary>The players inside the cube this frame (PROJECT.md, "Players"). Read by the test bridge.</summary>
    internal PlayerSet Players { get; } = new(NameSlots, new NameCache(NameSlots));

    /// <summary>Whether the last frame found a local player to centre the cube on.</summary>
    internal bool LastHadLocalPlayer { get; private set; }

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

    /// <summary>Text height of the last frame's labels, in pixels; the snapshot sizes label placeholders with it.</summary>
    internal float LastFontSize { get; private set; }

    /// <summary>Thickness of the big cube's edges, in pixels; the snapshot draws lines with it.</summary>
    internal static float LineThickness => WireThickness;

#if DEBUG
    /// <summary>
    /// Time and allocations of each <see cref="Draw"/> call, read by the in-game test bridge to check
    /// PROJECT.md's budgets (under 0.5 ms average, zero bytes per frame). Debug builds only.
    /// </summary>
    /// <remarks>Holds the last 240 frames (about 4 seconds at 60 fps); fixed size, so measuring allocates nothing.</remarks>
    internal DrawCostSampler DrawCost { get; } = new(240, Stopwatch.Frequency);

    /// <summary>Synthetic players the test bridge can inject (#16). Empty unless a check enables them. Debug builds only.</summary>
    internal TestBridge.FakePlayers Fakes { get; } = new();
#endif

    /// <summary>
    /// Builds this frame's draw list for a given drawing region. Separate from the ImGui calls so the bridge can
    /// check exactly what Core produced.
    /// </summary>
    /// <param name="list">The list to fill (cleared first).</param>
    /// <param name="space">The cube.</param>
    /// <param name="projection">The projection.</param>
    /// <param name="players">The players inside the cube.</param>
    /// <param name="palette">Player colours.</param>
    /// <param name="labels">Whether to add name labels.</param>
    /// <param name="wireColour">The big cube's edge colour.</param>
    internal static void BuildFrame(DrawList list, in CubeSpace space, in ViewProjection projection, ReadOnlySpan<NearbyPlayer> players, in PlayerPalette palette, bool labels, uint wireColour)
    {
        list.Clear();
        CubeGeometry.AddWireCube(list, space, projection, wireColour);
        PlayerCubes.Add(list, space, projection, players, palette, labels);
        list.SortBackToFront();
    }

    /// <summary>The user's settings (PROJECT.md, "Features": settings window). The bridge changes them in memory to test them.</summary>
    internal CubeSettings Settings { get; }

    /// <summary>The last frame's player colours, rebuilt from <see cref="Settings"/> only when they change.</summary>
    internal PlayerPalette Palette { get; private set; }

    /// <summary>The last frame's edge colour, rebuilt with <see cref="Palette"/>.</summary>
    internal uint WireColour { get; private set; }

    /// <summary>The last frame's label setting.</summary>
    internal bool ShowLabels { get; private set; }

#if DEBUG
    /// <summary>
    /// The text of each label the last frame drew, in drawing order, so the bridge can check it against the player
    /// set. Holds references to the cached name strings, so recording allocates nothing. Debug builds only.
    /// </summary>
    internal string[] LastLabelText { get; } = new string[NameSlots];

    /// <summary>How many entries of <see cref="LastLabelText"/> the last frame filled.</summary>
    internal int LastLabelCount { get; private set; }
#endif

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
        // North up never reads the camera. With no camera (or one looking straight down) the forward vector is zero
        // and FromForward keeps the previous yaw.
        var settings = this.Settings;
        if (settings.Revision != this.settingsRevision)
        {
            this.Palette = settings.Palette();
            this.WireColour = settings.WithOpacity(WireRgb, WireAlpha);
            this.settingsRevision = settings.Revision;
        }

        this.ShowLabels = settings.ShowLabels;

        var forward = Vector3.Zero;
        if (settings.Orientation == OrientationMode.Camera)
        {
            CameraReader.TryGetForward(out forward);
        }

        var yaw = ViewYaw.Settle(this.yaw, ViewYaw.For(settings.Orientation, forward, this.yaw));

        // The cube fills the content region and keeps its shape on resize: Fit uses one scale for both axes.
        var regionMin = ImGui.GetCursorScreenPos();
        var regionSize = ImGui.GetContentRegionAvail();
        // The cube is centred on the local player and follows them every frame (PROJECT.md, "Cube"). The rotation
        // is only recomputed when the yaw has actually changed.
        this.LastHadLocalPlayer = PlayerReader.TryGetLocalPosition(out var centre);
        var space = this.hasSpace && yaw == this.yaw
            ? this.LastSpace.Recentred(centre, settings.CubeEdge)
            : new CubeSpace(centre, settings.CubeEdge, yaw);
        this.yaw = yaw;
        this.hasSpace = true;
        var projection = ViewProjection.Fit(regionMin, regionSize, space.HalfEdge, Elevation);

        this.Players.Begin(space);
        if (this.LastHadLocalPlayer)
        {
            PlayerReader.Read(this.Players);
        }

#if DEBUG
        this.Fakes.Offer(this.Players, space);
#endif

        BuildFrame(this.DrawList, space, projection, this.Players.Items, this.Palette, this.ShowLabels, this.WireColour);
        this.LastSpace = space;
        this.LastProjection = projection;
        this.LastRegionMin = regionMin;
        this.LastRegionSize = regionSize;
        this.LastSubmitted = this.Submit(ImGui.GetWindowDrawList(), this.DrawList.Items, this.Players.Items);
    }

    /// <summary>Issues one ImGui call per primitive, in order.</summary>
    /// <remarks>
    /// Labels are clipped to the window by the window draw list's own clip rectangle, so names near the edge are cut
    /// off rather than spilling over the game. The text is the cached name string, so no string is built here.
    /// </remarks>
    /// <param name="target">The window's ImGui draw list.</param>
    /// <param name="items">The sorted primitives.</param>
    /// <param name="players">This frame's players, for label text.</param>
    /// <returns>How many calls were made.</returns>
    private int Submit(ImDrawListPtr target, ReadOnlySpan<DrawPrimitive> items, ReadOnlySpan<NearbyPlayer> players)
    {
        var submitted = 0;
        this.LastFontSize = ImGui.GetFontSize();
        var halfLine = this.LastFontSize / 2f;
#if DEBUG
        var labels = 0;
#endif
        foreach (ref readonly var p in items)
        {
            switch (p.Kind)
            {
                case PrimitiveKind.Line:
                    target.AddLine(p.A, p.B, p.Colour, WireThickness);
                    break;
                case PrimitiveKind.Quad:
                    target.AddQuadFilled(p.A, p.B, p.C, p.D, p.Colour);
                    break;
                default:
                    var name = players[p.Tag - 1].Name;
                    target.AddText(new Vector2(p.A.X, p.A.Y - halfLine), p.Colour, name);
#if DEBUG
                    this.LastLabelText[labels++] = name;
#endif
                    break;
            }

            submitted++;
        }

#if DEBUG
        this.LastLabelCount = labels;
#endif
        return submitted;
    }
}
