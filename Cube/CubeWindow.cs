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
/// Each frame: build the <see cref="CubeSpace"/> and fitted <see cref="ViewProjection"/>, let Core fill the reused
/// <see cref="DrawList"/>, sort it far to near, issue one ImGui call per primitive, then draw the view control on
/// top (<see cref="DrawViewControl"/>). Nothing here allocates
/// (PROJECT.md, "Efficiency and memory"); Dalamud skips <see cref="Draw"/> entirely while the window is closed.
/// </remarks>
internal sealed class CubeWindow : Window
{
    /// <summary>
    /// Most primitives a frame can hold: the 12 cube edges plus three faces and a label for every player the set can
    /// hold (200) and every NPC (50), with room to spare. Fixed, so memory never grows (PROJECT.md: bounded memory).
    /// </summary>
    internal const int DrawListCapacity = 1280;

    /// <summary>Colour of the big cube's edges: light grey.</summary>
    private static readonly Vector3 WireRgb = new(0.85f, 0.85f, 0.85f);

    /// <summary>The edges' own opacity (slightly transparent), before the opacity setting is applied.</summary>
    private const float WireAlpha = 0.8f;

    /// <summary>Thickness of the big cube's edges, in pixels.</summary>
    private const float WireThickness = 1.5f;

    /// <summary>
    /// How far ImGuizmo's camera sits from the cube's centre. Only ImGuizmo's own maths uses it (it orbits around the
    /// point this far in front of the camera); the drawn cube is orthographic, so any positive value works.
    /// </summary>
    private const float GizmoDistance = 8f;

    /// <summary>
    /// A right-handed perspective projection handed to ImGuizmo, only so it sets up its context (mouse hover,
    /// handedness) the way its view manipulator expects. The cube itself is not drawn with it.
    /// </summary>
    private static readonly Matrix4x4 GizmoProjection = Matrix4x4.CreatePerspectiveFieldOfView(MathF.PI / 4f, 1f, 0.1f, 100f);

    /// <summary>The model matrix ImGuizmo's context needs; unused by the view manipulator.</summary>
    private static readonly Matrix4x4 GizmoModel = Matrix4x4.Identity;

    /// <summary>Writes the settings to disk; called when the user lets go of the view control.</summary>
    private readonly Action save;

    /// <summary>The view orientation from the settings, rebuilt with the colours only when the settings change.</summary>
    private ViewOrientation view = ViewOrientation.Default;

    /// <summary>
    /// Whether the user is turning the view with the control: set when ImGuizmo changes the view, cleared (and the
    /// settings saved) once the mouse is released and the view has settled.
    /// </summary>
    private bool turning;

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
        var startBytes = GC.GetAllocatedBytesForCurrentThread();
        var startTicks = Stopwatch.GetTimestamp();
        this.SafeDrawContents();
        var elapsed = Stopwatch.GetTimestamp() - startTicks;
        var allocated = GC.GetAllocatedBytesForCurrentThread() - startBytes;
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
            ? this.LastSpace.Recentred(centre, settings.CubeEdge)
            : new CubeSpace(centre, settings.CubeEdge, yaw);
        this.yaw = yaw;
        this.hasSpace = true;
        var projection = ViewProjection.Fit(regionMin, regionSize, space.HalfEdge, this.view);

        this.Players.Begin(space, settings.ShowNpcs);
        PlayerReader.Read(this.Players);

#if DEBUG
        this.Fakes.Offer(this.Players, space);
#endif

        BuildFrame(this.DrawList, space, projection, this.Players.Items, this.Palette, this.ShowLabels, this.WireColour);
        this.LastSpace = space;
        this.LastProjection = projection;
        this.LastRegionMin = regionMin;
        this.LastRegionSize = regionSize;
        this.LastSubmitted = this.Submit(ImGui.GetWindowDrawList(), this.DrawList.Items, this.Players.Items);
        this.DrawViewControl(regionMin, regionSize);
    }

    /// <summary>
    /// Stops ImGui moving the window while the mouse is on the view control or the user is turning the view, so a
    /// drag on the control turns the cube instead of dragging the window around.
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
        if (overControl || this.turning)
        {
            this.Flags |= ImGuiWindowFlags.NoMove;
        }
        else
        {
            this.Flags &= ~ImGuiWindowFlags.NoMove;
        }
    }

    /// <summary>
    /// Draws ImGuizmo's view manipulator in the chosen corner (PROJECT.md, "View orientation control") and applies
    /// any turn the user makes with it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// ImGuizmo is handed the current orientation as a view matrix each frame and changes the matrix when the control
    /// is dragged, or animates it after a face is clicked. A changed matrix is read back into the settings; the
    /// projection then picks it up through the settings revision, so it is only rebuilt when the view really changes.
    /// The camera and the player never feed into this (PROJECT.md: only the control turns the view).
    /// </para>
    /// <para>
    /// The file is saved once the turn is over (mouse released and no change this frame), not on every frame of a
    /// drag. The 9-argument overload is used because it sets up ImGuizmo's context (mouse hover over this window,
    /// handedness) first; the short one relies on state from an earlier <c>Manipulate</c> call that this plugin never
    /// makes. Nothing here allocates: the matrices are structs passed by reference.
    /// </para>
    /// </remarks>
    /// <param name="regionMin">Top-left of the drawing region, in pixels.</param>
    /// <param name="regionSize">Size of the drawing region, in pixels.</param>
    private void DrawViewControl(Vector2 regionMin, Vector2 regionSize)
    {
        var size = ViewWidget.Size(regionSize);
        var position = ViewWidget.Position(regionMin, regionSize, this.Settings.ViewWidgetCorner, size);
        this.WidgetMin = position;
        this.WidgetSize = size;

        ImGuizmo.SetDrawlist();
        ImGuizmo.SetRect(position.X, position.Y, size, size);
        var matrix = this.view.ToGizmoView(GizmoDistance);
        var before = matrix;
        var projection = GizmoProjection;
        var model = GizmoModel;
        ImGuizmo.ViewManipulate(ref matrix, ref projection, ImGuizmoOperation.Rotate, ImGuizmoMode.World, ref model, GizmoDistance, position, new Vector2(size, size), 0u);

        if (matrix != before)
        {
            this.Settings.SetView(ViewOrientation.FromGizmoView(matrix));
            this.turning = true;
        }
        else if (this.turning && !ImGui.IsMouseDown(ImGuiMouseButton.Left))
        {
            this.turning = false;
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
