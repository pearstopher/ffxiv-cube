using System;
using System.Numerics;
using Cube.Core;
using Cube.Game;
using Dalamud.Bindings.ImGui;
using Dalamud.Bindings.ImGuizmo;
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
/// Each frame: build the <see cref="CubeSpace"/> and fitted <see cref="ViewProjection"/>, read the players, let the
/// collision reader do its budgeted share of gathering and clip the cached boxes to the cube (only when either
/// changed), let Core fill the reused <see cref="DrawList"/>, sort it far to near, issue one ImGui call per primitive,
/// then draw the view control on top (<see cref="DrawViewControl"/>). Nothing here allocates
/// (PROJECT.md, "Efficiency and memory"); Dalamud skips <see cref="Draw"/> entirely while the window is closed.
/// </remarks>
internal sealed class CubeWindow : Window
{
    /// <summary>
    /// Most primitives a frame can hold: the 12 cube edges plus three faces and a label for every player the set can
    /// hold (200) and every NPC (50), with room to spare (1,280), plus three faces for every collision box the cache
    /// can hold. Fixed, so memory never grows (PROJECT.md: bounded memory); about 380 KB.
    /// </summary>
    internal const int DrawListCapacity = 1280 + (3 * CollisionBoxSet.DefaultCapacity);

    /// <summary>Colour of the big cube's edges: light grey.</summary>
    private static readonly Vector3 WireRgb = new(0.85f, 0.85f, 0.85f);

    /// <summary>The edges' own opacity (slightly transparent), before the opacity setting is applied.</summary>
    private const float WireAlpha = 0.8f;

    /// <summary>Thickness of the big cube's edges, in pixels.</summary>
    private const float WireThickness = 1.5f;

    /// <summary>
    /// How much bigger than the corner square the rectangle handed to ImGuizmo is. ImGuizmo sizes the rotate gizmo
    /// from its rectangle (the outer screen ring's radius is 6% of the rectangle's height, the axis rings about the
    /// same), so a rectangle 7 times the square, centred on it, makes the rings fill the square with a small margin.
    /// ImGuizmo only uses the rectangle for its projection and doesn't clip to it.
    /// </summary>
    private const float GizmoRectScale = 7f;

    /// <summary>
    /// The fixed camera ImGuizmo sees the cube through: on ImGuizmo's +Z axis looking at the centre, up along +Y, so
    /// its screen axes are the window's. Its distance only matters to ImGuizmo's own maths.
    /// </summary>
    private static readonly Matrix4x4 GizmoCamera = Matrix4x4.CreateLookAt(new Vector3(0, 0, 8f), Vector3.Zero, Vector3.UnitY);

    /// <summary>
    /// A right-handed perspective projection for ImGuizmo, matching <see cref="GizmoCamera"/>. The narrow field of view
    /// keeps the rings nearly undistorted, like the orthographic cube. The cube itself is not drawn with it.
    /// </summary>
    private static readonly Matrix4x4 GizmoProjection = Matrix4x4.CreatePerspectiveFieldOfView(MathF.PI / 6f, 1f, 0.1f, 100f);

    /// <summary>Writes the settings to disk; called when the user lets go of the view control.</summary>
    private readonly Action save;

    /// <summary>The view orientation from the settings, rebuilt with the colours only when the settings change.</summary>
    private ViewOrientation view = ViewOrientation.Default;

    /// <summary>
    /// Whether the user is turning the view or resizing the cube with the control: set when ImGuizmo changes either,
    /// cleared (and the settings saved) once the mouse is released and the change has settled.
    /// </summary>
    private bool dragging;

    /// <summary>
    /// What the corner gizmo offers: the rotation rings (#79) and the scale handles (#90), whose axis handles resize
    /// one extent each and whose centre handle zooms.
    /// </summary>
    private const ImGuizmoOperation GizmoOperations = ImGuizmoOperation.Rotate | ImGuizmoOperation.Scale;

    /// <summary>The cube's current alignment yaw; kept between frames so a vertical camera doesn't spin the contents.</summary>
    private float yaw;

    /// <summary>Whether <see cref="LastSpace"/> holds a real frame's cube, so its rotation can be reused.</summary>
    private bool hasSpace;

    /// <summary>The <see cref="CubeSettings.Revision"/> the cached colours were built from; -1 forces a first build.</summary>
    private int settingsRevision = -1;

    /// <summary>Reads the states that hide the cube.</summary>
    private readonly GameStateReader gameState;

    /// <summary>Creates the window, closed, with a sensible minimum size.</summary>
    /// <param name="settings">The user's settings, read every frame; changes apply on the next frame.</param>
    /// <param name="gameState">Reads login, loading, cutscene, GPose and PvP state.</param>
    /// <param name="save">Writes the settings to disk, after the user turns the view with the view control.</param>
    public CubeWindow(CubeSettings settings, GameStateReader gameState, Action save)
        : base("Cube###CubeMain")
    {
        this.Settings = settings;
        this.gameState = gameState;
        this.save = save;
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(200, 200),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue),
        };
    }

#if DEBUG
    /// <summary>
    /// Name cache slots: one per object table index (players and NPCs are both keyed by slot), plus room above them
    /// for a full crowd of Debug-only fake players and fake NPCs (#16, #23, #74), so checks can measure at the caps.
    /// </summary>
    internal const int NameSlots = PlayerReader.ObjectSlots + PlayerReader.MaxPlayers + PlayerReader.MaxNpcs;
#else
    /// <summary>
    /// Name cache slots: one per object table index, since NPCs live above the character slots (#73). Fixed, so memory
    /// never grows (PROJECT.md: bounded memory); about 52 KB of name bytes.
    /// </summary>
    internal const int NameSlots = PlayerReader.ObjectSlots;
#endif

    /// <summary>
    /// Most players kept per frame: every character slot, which covers the 100 real players the game can hold plus,
    /// in Debug builds, a full crowd of fakes on top.
    /// </summary>
    internal const int PlayerCapacity = PlayerReader.CharacterSlots;

    /// <summary>This frame's primitives, in drawing order. Read by the test bridge.</summary>
    internal DrawList DrawList { get; } = new(DrawListCapacity);

    /// <summary>
    /// The players and named NPCs inside the cube this frame (PROJECT.md, "Players" and "NPCs"), NPCs capped at
    /// <see cref="PlayerReader.MaxNpcs"/>. Read by the test bridge.
    /// </summary>
    internal PlayerSet Players { get; } = new(PlayerCapacity, new NameCache(NameSlots), PlayerReader.MaxNpcs);

    /// <summary>
    /// The collision around the cube (PROJECT.md, "Collision (phase 2)"), gathered over frames while the cube is shown.
    /// Read by the test bridge.
    /// </summary>
    internal CollisionReader Collision { get; } = new();

    /// <summary>
    /// The gathered collision boxes clipped to the last frame's cube, as drawn (#31). Read by the test bridge.
    /// </summary>
    internal ClippedCollision ClippedCollision { get; } = new();

    /// <summary>Whether the last frame found a local player to centre the cube on.</summary>
    internal bool LastHadLocalPlayer { get; private set; }

    /// <summary>Whether the last frame drew the cube, or why not (PROJECT.md, "When hidden").</summary>
    internal Visibility LastVisibility { get; private set; }

#if DEBUG
    /// <summary>
    /// A game state the bridge sets to test hiding without a real cutscene or loading screen; null uses the real
    /// state. Debug builds only.
    /// </summary>
    internal GameState? SimulatedState { get; set; }

    /// <summary>
    /// A camera direction the bridge sets to turn the "camera" without touching the game (#62); null reads the real
    /// camera. Debug builds only.
    /// </summary>
    internal Vector3? SimulatedCameraForward { get; set; }
#endif

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

    /// <summary>
    /// The view control's top-left last frame, in screen pixels. Used for the hover test in <see cref="PreDraw"/> and
    /// read by the test bridge.
    /// </summary>
    internal Vector2 WidgetMin { get; private set; }

    /// <summary>The view control's side length last frame, in pixels; 0 when it wasn't drawn (cube hidden).</summary>
    internal float WidgetSize { get; private set; }

    /// <summary>Thickness of the big cube's edges, in pixels; the snapshot draws lines with it.</summary>
    internal static float LineThickness => WireThickness;

#if DEBUG
    /// <summary>
    /// Time and allocations of each <see cref="Draw"/> call, read by the in-game test bridge to check
    /// PROJECT.md's budgets (under 0.5 ms average, zero bytes per frame). Debug builds only.
    /// </summary>
    /// <remarks>Holds the last 240 frames (about 4 seconds at 60 fps); fixed size, so measuring allocates nothing.</remarks>
    internal DrawCostSampler DrawCost { get; } = new(240, Stopwatch.Frequency);

    /// <summary>
    /// Frames in which a player's name string was built (someone new came into view). Their allocations are the
    /// cached label strings, so they're counted here rather than in <see cref="DrawCost"/>. Debug builds only.
    /// </summary>
    internal long NameBuildFrames { get; private set; }

    /// <summary>Bytes allocated in <see cref="NameBuildFrames"/>. Debug builds only.</summary>
    internal long NameBuildBytes { get; private set; }

    /// <summary>
    /// Time and allocations of each frame's collision gathering, on frames where a gather ran. PROJECT.md's budgets
    /// exempt "budgeted background gathering" from the draw budget, so these frames' gathering is taken out of
    /// <see cref="DrawCost"/> and checked here against its own budget instead. Debug builds only.
    /// </summary>
    internal DrawCostSampler GatherCost { get; } = new(240, Stopwatch.Frequency);

    /// <summary>Ticks this frame's collision gather took, subtracted from the draw cost. Debug builds only.</summary>
    private long gatherTicks;

    /// <summary>Bytes this frame's collision gather allocated, subtracted from the draw cost. Debug builds only.</summary>
    private long gatherBytes;

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
    /// <param name="collision">The collision boxes clipped to the cube (PROJECT.md, "Collision (phase 2)").</param>
    /// <param name="collisionColour">The collision boxes' colour.</param>
    internal static void BuildFrame(DrawList list, in CubeSpace space, in ViewProjection projection, ReadOnlySpan<NearbyPlayer> players, in PlayerPalette palette, bool labels, uint wireColour, ReadOnlySpan<CollisionBox> collision, uint collisionColour)
    {
        list.Clear();
        CubeGeometry.AddWireCube(list, space, projection, wireColour);
        CollisionCubes.Add(list, new BoxProjector(space, projection), collision, collisionColour);
        PlayerCubes.Add(list, space, projection, players, palette, labels);
        list.SortBackToFront();
    }

    /// <summary>The user's settings (PROJECT.md, "Features": settings window). The bridge changes them in memory to test them.</summary>
    internal CubeSettings Settings { get; }

    /// <summary>The last frame's player colours, rebuilt from <see cref="Settings"/> only when they change.</summary>
    internal PlayerPalette Palette { get; private set; }

    /// <summary>The last frame's edge colour, rebuilt with <see cref="Palette"/>.</summary>
    internal uint WireColour { get; private set; }

    /// <summary>The last frame's collision colour (<see cref="CollisionCubes.Rgb"/> at the opacity setting), rebuilt with <see cref="Palette"/>.</summary>
    internal uint CollisionColour { get; private set; }

    /// <summary>The last frame's label setting.</summary>
    internal bool ShowLabels { get; private set; }

#if DEBUG
    /// <summary>
    /// The text of each label the last frame drew, in drawing order, so the bridge can check it against the player
    /// set. Holds references to the cached name strings, so recording allocates nothing. Debug builds only.
    /// </summary>
    internal string[] LastLabelText { get; } = new string[PlayerCapacity + PlayerReader.MaxNpcs];

    /// <summary>How many entries of <see cref="LastLabelText"/> the last frame filled.</summary>
    internal int LastLabelCount { get; private set; }
#endif

    /// <summary>Exceptions caught while drawing; the bridge checks there were none.</summary>
    internal ErrorReporter Errors { get; } = new("Cube window");

    /// <summary>Draws the window's contents, measuring the cost in Debug builds.</summary>
    public override void Draw()
    {
#if DEBUG
        var startBuilds = this.Players.Names.Builds;
        this.gatherTicks = 0;
        this.gatherBytes = 0;
        var startBytes = GC.GetAllocatedBytesForCurrentThread();
        var startTicks = Stopwatch.GetTimestamp();
        this.SafeDrawContents();
        var elapsed = Stopwatch.GetTimestamp() - startTicks - this.gatherTicks;
        var allocated = GC.GetAllocatedBytesForCurrentThread() - startBytes - this.gatherBytes;
        if (this.Players.Names.Builds != startBuilds)
        {
            // A new name came into view and its label string was built: allowed (PROJECT.md: labels are rebuilt
            // when a name changes), and not steady state, so it's counted here instead of against the budget.
            this.NameBuildFrames++;
            this.NameBuildBytes += allocated;
            allocated = 0;
        }

        this.DrawCost.Record(elapsed, allocated);
#else
        this.SafeDrawContents();
#endif
    }

    /// <summary>
    /// Draws, catching and logging any exception so it never reaches the game (CLAUDE.md, "Dalamud rules"). A failed
    /// frame is simply empty. The try costs nothing when nothing is thrown.
    /// </summary>
    private void SafeDrawContents()
    {
        try
        {
            this.DrawContents();
        }
        catch (Exception ex)
        {
            this.DrawList.Clear();
            this.Errors.Report(ex);
        }
    }

    /// <summary>The plugin's per-frame draw path: builds the frame with Core and draws it.</summary>
    private void DrawContents()
    {
        // The cube is centred on the local player and follows them every frame (PROJECT.md, "Cube").
        this.LastHadLocalPlayer = PlayerReader.TryGetLocalPosition(out var centre);
        var state = this.gameState.Read(this.LastHadLocalPlayer);
#if DEBUG
        state = this.SimulatedState ?? state;
#endif
        this.LastVisibility = VisibilityRules.For(state);
        if (this.LastVisibility != Visibility.Shown)
        {
            // Hidden: an empty window, and nothing else is read or computed (PROJECT.md, "When hidden").
            this.DrawList.Clear();
            this.Players.Begin(this.LastSpace);
            this.LastSubmitted = 0;
            this.WidgetSize = 0f;
            return;
        }

        var settings = this.Settings;
        if (settings.Revision != this.settingsRevision)
        {
            this.Palette = settings.Palette();
            this.WireColour = settings.WithOpacity(WireRgb, WireAlpha);
            this.CollisionColour = settings.WithOpacity(CollisionCubes.Rgb, CollisionCubes.Alpha);
            this.view = settings.View;
            this.settingsRevision = settings.Revision;
        }

        this.ShowLabels = settings.ShowLabels;

        // The cube's alignment (PROJECT.md, "Cube"): in Camera mode it follows the camera's horizontal direction, so
        // the contents rotate while the wireframe stays still. North up never reads the camera. With no camera (or
        // one looking straight down) the forward vector is zero and FromForward keeps the previous yaw.
        var forward = Vector3.Zero;
        if (settings.Orientation == OrientationMode.Camera)
        {
#if DEBUG
            if (this.SimulatedCameraForward is { } simulated)
            {
                forward = simulated;
            }
            else
            {
                CameraReader.TryGetForward(out forward);
            }
#else
            CameraReader.TryGetForward(out forward);
#endif
        }

        var yaw = ViewYaw.Settle(this.yaw, ViewYaw.For(settings.Orientation, forward, this.yaw));

        // The cube fills the content region and keeps its shape on resize: Fit uses one scale for both axes.
        var regionMin = ImGui.GetCursorScreenPos();
        var regionSize = ImGui.GetContentRegionAvail();
        // The rotation is only recomputed when the yaw has actually changed.
        var space = this.hasSpace && yaw == this.yaw
            ? this.LastSpace.Recentred(centre, settings.CubeSize)
            : new CubeSpace(centre, settings.CubeSize, yaw);
        this.yaw = yaw;
        this.hasSpace = true;
        var projection = ViewProjection.Fit(regionMin, regionSize, space.Half, this.view);

        this.Players.Begin(space, settings.ShowNpcs);
        PlayerReader.Read(this.Players);

#if DEBUG
        this.Fakes.Offer(this.Players, space);
#endif

        if (settings.ShowCollision)
        {
            this.UpdateCollision(centre, space.Size, settings.CollisionDetail);
        }
        else
        {
            // Collision off (#32): the scene isn't read and the boxes are dropped, so the clip below empties the cube
            // and turning it back on gathers afresh around wherever the player is then.
            this.Collision.Cache.Clear();
        }

        this.ClippedCollision.Update(space, this.Collision.Cache);

        BuildFrame(this.DrawList, space, projection, this.Players.Items, this.Palette, this.ShowLabels, this.WireColour, this.ClippedCollision.Items, this.CollisionColour);
        this.LastSpace = space;
        this.LastProjection = projection;
        this.LastRegionMin = regionMin;
        this.LastRegionSize = regionSize;
        this.LastSubmitted = this.Submit(ImGui.GetWindowDrawList(), this.DrawList.Items, this.Players.Items);
        this.DrawViewControl(regionMin, regionSize);
    }

    /// <summary>
    /// Lets the collision reader do this frame's share of gathering (PROJECT.md, "Collision (phase 2)"). In Debug builds
    /// a frame where a gather ran is timed on its own and recorded in <c>GatherCost</c> (Debug only), and kept out of the
    /// draw budget, which PROJECT.md says doesn't cover budgeted background gathering.
    /// </summary>
    /// <param name="centre">The cube's centre.</param>
    /// <param name="size">The cube's width, height and depth in yalms.</param>
    /// <param name="detail">The collision detail setting (#32).</param>
    private void UpdateCollision(Vector3 centre, Vector3 size, CollisionDetail detail)
    {
#if DEBUG
        var startBytes = GC.GetAllocatedBytesForCurrentThread();
        var startTicks = Stopwatch.GetTimestamp();
        var completed = this.Collision.Update(centre, size, detail);
        if (completed || this.Collision.Cache.IsGathering)
        {
            this.gatherTicks = Stopwatch.GetTimestamp() - startTicks;
            this.gatherBytes = GC.GetAllocatedBytesForCurrentThread() - startBytes;
            this.GatherCost.Record(this.gatherTicks, this.gatherBytes);
        }
#else
        this.Collision.Update(centre, size, detail);
#endif
    }

    /// <summary>
    /// Stops ImGui moving the window while the mouse is on the view control or the user is turning the view, so a
    /// drag on the control turns or resizes the cube instead of dragging the window around (#63, #79, #90).
    /// </summary>
    /// <remarks>
    /// ImGuizmo reads the mouse directly rather than claiming it as an ImGui item, so without this ImGui would treat
    /// the drag as a drag on empty window space. An invisible button over the control can't be used instead: ImGuizmo
    /// ignores clicks while another item is active. Uses last frame's position, which is where the mouse went down.
    /// </remarks>
    public override void PreDraw()
    {
        var overControl = this.WidgetSize > 0f
            && ImGui.IsMouseHoveringRect(this.WidgetMin, this.WidgetMin + new Vector2(this.WidgetSize), false);
        if (overControl || this.dragging)
        {
            this.Flags |= ImGuiWindowFlags.NoMove;
        }
        else
        {
            this.Flags &= ~ImGuiWindowFlags.NoMove;
        }
    }

    /// <summary>
    /// Draws ImGuizmo's rotate and scale gizmo in the chosen corner (PROJECT.md, "View orientation control": a ring per
    /// axis plus an outer screen ring; #90: scale handles that resize the cube per axis, and a centre handle that
    /// zooms) and applies any turn or resize the user makes with it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// ImGuizmo is handed the current orientation and size as one model matrix each frame (<see cref="GizmoModel.Build"/>)
    /// and turns or scales it while a handle is dragged. Local mode puts the rings around, and the scale handles along,
    /// the cube's own axes, so the colours match them and a handle always changes the matching extent. A changed
    /// matrix is read back with <see cref="GizmoModel.Read"/>, which keeps the turn and the resize apart; the
    /// projection picks a new view up through the settings revision, so it is only rebuilt when the view really
    /// changes. The camera and the player never feed into this (PROJECT.md: only the control turns the view).
    /// </para>
    /// <para>
    /// The file is saved once the drag is over (mouse released and no change this frame), not on every frame of a
    /// drag. ImGuizmo's global settings (gizmo size, orthographic mode) are left alone because other plugins share
    /// them; the size comes from the rectangle instead (<see cref="GizmoRectScale"/>). Nothing here allocates: the
    /// matrices are structs passed by reference.
    /// </para>
    /// </remarks>
    /// <param name="regionMin">Top-left of the drawing region, in pixels.</param>
    /// <param name="regionSize">Size of the drawing region, in pixels.</param>
    private void DrawViewControl(Vector2 regionMin, Vector2 regionSize)
    {
        var side = ViewWidget.Size(regionSize);
        var position = ViewWidget.Position(regionMin, regionSize, this.Settings.ViewWidgetCorner, side);
        this.WidgetMin = position;
        this.WidgetSize = side;

        var rect = side * GizmoRectScale;
        var centre = position + new Vector2(side / 2f);
        ImGuizmo.SetDrawlist();
        ImGuizmo.SetRect(centre.X - (rect / 2f), centre.Y - (rect / 2f), rect, rect);
        var size = this.Settings.CubeSize;
        var model = GizmoModel.Build(this.view, size);
        var before = model;
        var camera = GizmoCamera;
        var projection = GizmoProjection;

        // One call with Rotate | Scale, as #90 asks: ImGuizmo handles scale before rotation and keeps the model's
        // scale through a turn, so the two share one matrix. If the combined call ever misbehaves in game, split it
        // into two Manipulate calls on the same matrix (see docs/manual-tests.md, "Resize the cube").
        ImGuizmo.Manipulate(ref camera, ref projection, GizmoOperations, ImGuizmoMode.Local, ref model);

        var change = model != before ? GizmoModel.Read(model, this.view, size) : default;
        if (change.ViewChanged || change.SizeChanged)
        {
            // Only the part that moved is stored, so a resize never nudges the view and a turn never the size.
            if (change.ViewChanged)
            {
                this.Settings.ViewRotation = change.View.Rotation;
            }

            if (change.SizeChanged)
            {
                this.Settings.CubeSize = change.Size;
            }

            this.Settings.Changed();
            this.dragging = true;
        }
        else if (this.dragging && !ImGui.IsMouseDown(ImGuiMouseButton.Left))
        {
            this.dragging = false;
            this.save();
        }
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
