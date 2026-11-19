# <a id="Cube_CubeWindow"></a> Class CubeWindow

Namespace: [Cube](Cube.md)  
Assembly: Cube.dll  

The window the cube is drawn in (PROJECT.md, "Features": a Dalamud window toggled by <code>/cube</code>, showing a
wireframe cube with the player at its centre).

```csharp
internal sealed class CubeWindow : Window, IWindow
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[Window](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs) ← 
[CubeWindow](Cube.CubeWindow.md)

#### Implements

[IWindow](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/IWindow.cs)


## Remarks

Each frame: build the [CubeSpace](Cube.Core.CubeSpace.md#Cube_Core_CubeSpace) and fitted [ViewProjection](Cube.Core.ViewProjection.md#Cube_Core_ViewProjection), read the players, let the
collision reader do its budgeted share of gathering and clip the cached boxes to the cube (only when either
changed), let Core fill the reused [DrawList](Cube.CubeWindow.md#Cube_CubeWindow_DrawList), sort it far to near, issue one ImGui call per primitive,
then draw the view control on top ([DrawViewControl](Cube.CubeWindow.md#Cube_CubeWindow_DrawViewControl)). Nothing here allocates
(PROJECT.md, "Efficiency and memory"); Dalamud skips [Draw](Cube.CubeWindow.md#Cube_CubeWindow_Draw) entirely while the window is closed.

## Constructors

### <a id="Cube_CubeWindow__ctor_Cube_Core_CubeSettings_Cube_Game_GameStateReader_System_Action_"></a> CubeWindow\(CubeSettings, GameStateReader, Action\)

Creates the window, closed, with a sensible minimum size.

```csharp
public CubeWindow(CubeSettings settings, GameStateReader gameState, Action save)
```

#### Parameters

`settings` [CubeSettings](Cube.Core.CubeSettings.md)

The user's settings, read every frame; changes apply on the next frame.

`gameState` [GameStateReader](Cube.Game.GameStateReader.md)

Reads login, loading, cutscene, GPose and PvP state.

`save` [Action](https://learn.microsoft.com/dotnet/api/system.action)

Writes the settings to disk, after the user turns the view with the view control.

## Fields

### <a id="Cube_CubeWindow_DrawListCapacity"></a> DrawListCapacity

Most primitives a frame can hold: the 12 cube edges plus three faces and a label for every player the set can
hold (200) and every NPC (50), with room to spare (1,280), plus the most faces every collision box the cache
can hold could add ([MaxFacesPerBox](Cube.Core.CollisionCubes.md#Cube_Core_CollisionCubes_MaxFacesPerBox); #93). Fixed, so memory never grows and no face
is ever dropped (PROJECT.md: bounded memory); about 720 KB with the sort keys.

```csharp
internal const int DrawListCapacity = 11280
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_CubeWindow_DrawListPointCapacity"></a> DrawListPointCapacity

Most polygon corners a frame can hold: a full [MaxCorners](Cube.Core.ClippedBox.md#Cube_Core_ClippedBox_MaxCorners)-corner top for every
collision box (#93), so no polygon is ever dropped; about 125 KB.

```csharp
internal const int DrawListPointCapacity = 16000
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_CubeWindow_GizmoCamera"></a> GizmoCamera

The fixed camera ImGuizmo sees the cube through: on ImGuizmo's +Z axis looking at the centre, up along +Y, so
its screen axes are the window's. Its distance only matters to ImGuizmo's own maths.

```csharp
private static readonly Matrix4x4 GizmoCamera
```

#### Field Value

 [Matrix4x4](https://learn.microsoft.com/dotnet/api/system.numerics.matrix4x4)

### <a id="Cube_CubeWindow_GizmoProjection"></a> GizmoProjection

A right-handed perspective projection for ImGuizmo, matching [GizmoCamera](Cube.CubeWindow.md#Cube_CubeWindow_GizmoCamera). The narrow field of view
keeps the rings nearly undistorted, like the orthographic cube. The cube itself is not drawn with it.

```csharp
private static readonly Matrix4x4 GizmoProjection
```

#### Field Value

 [Matrix4x4](https://learn.microsoft.com/dotnet/api/system.numerics.matrix4x4)

### <a id="Cube_CubeWindow_GizmoRectScale"></a> GizmoRectScale

How much bigger than the corner square the rectangle handed to ImGuizmo is. ImGuizmo sizes the rotate gizmo
from its rectangle (the outer screen ring's radius is 6% of the rectangle's height, the axis rings about the
same), so a rectangle 7 times the square, centred on it, makes the rings fill the square with a small margin.
ImGuizmo only uses the rectangle for its projection and doesn't clip to it.

```csharp
private const float GizmoRectScale = 7
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_CubeWindow_MeshAlpha"></a> MeshAlpha

The Mesh style's line opacity: the collision colour, fainter than the boxes' so a dense mesh stays readable.

```csharp
private const float MeshAlpha = 0.45
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_CubeWindow_MeshBuildTicks"></a> MeshBuildTicks

How long a frame may spend rebuilding the clipped mesh, in clock ticks. The rest of a rebuild waits for the next
frame while the previous result keeps being drawn (PROJECT.md, "Collision style").

```csharp
private static readonly long MeshBuildTicks
```

#### Field Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Cube_CubeWindow_NameSlots"></a> NameSlots

Name cache slots: one per object table index (players and NPCs are both keyed by slot), plus room above them
for a full crowd of Debug-only fake players and fake NPCs (#16, #23, #74), so checks can measure at the caps.

```csharp
internal const int NameSlots = 999
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_CubeWindow_PlayerCapacity"></a> PlayerCapacity

Most players kept per frame: every character slot, which covers the 100 real players the game can hold plus,
in Debug builds, a full crowd of fakes on top.

```csharp
internal const int PlayerCapacity = 200
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_CubeWindow_RotateOperation"></a> RotateOperation

What the rotate gizmo offers: the x/y/z rings and the outer screen ring (#79, #97).

```csharp
private const ImGuizmoOperation RotateOperation = Rotate
```

#### Field Value

 [ImGuizmoOperation](https://github.com/goatcorp/Dalamud/blob/master/imgui/Dalamud.Bindings.ImGuizmo/Generated/Enums/ImGuizmoOperation.cs)

### <a id="Cube_CubeWindow_ScaleOperation"></a> ScaleOperation

What the resize gizmo offers: scale handles whose axis handles resize one extent each and whose centre handle
zooms (#90, #97).

```csharp
private const ImGuizmoOperation ScaleOperation = Scale
```

#### Field Value

 [ImGuizmoOperation](https://github.com/goatcorp/Dalamud/blob/master/imgui/Dalamud.Bindings.ImGuizmo/Generated/Enums/ImGuizmoOperation.cs)

### <a id="Cube_CubeWindow_WireAlpha"></a> WireAlpha

The edges' own opacity (slightly transparent), before the opacity setting is applied.

```csharp
private const float WireAlpha = 0.8
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_CubeWindow_WireRgb"></a> WireRgb

Colour of the big cube's edges: light grey.

```csharp
private static readonly Vector3 WireRgb
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_CubeWindow_WireThickness"></a> WireThickness

Thickness of the big cube's edges, in pixels.

```csharp
private const float WireThickness = 1.5
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_CubeWindow_anchor"></a> anchor

Holds the cube's height at the take-off height while the player jumps (PROJECT.md, "Jump"; #105).

```csharp
private readonly CubeAnchor anchor
```

#### Field Value

 [CubeAnchor](Cube.Core.CubeAnchor.md)

### <a id="Cube_CubeWindow_dragging"></a> dragging

Whether the user is turning the view or resizing the cube with the control: set when ImGuizmo changes either,
cleared (and the settings saved) once the mouse is released and the change has settled.

```csharp
private bool dragging
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_CubeWindow_gameState"></a> gameState

Reads the states that hide the cube.

```csharp
private readonly GameStateReader gameState
```

#### Field Value

 [GameStateReader](Cube.Game.GameStateReader.md)

### <a id="Cube_CubeWindow_gatherBytes"></a> gatherBytes

Bytes this frame's collision gather allocated, subtracted from the draw cost. Debug builds only.

```csharp
private long gatherBytes
```

#### Field Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Cube_CubeWindow_gatherTicks"></a> gatherTicks

Ticks this frame's collision gather took, subtracted from the draw cost. Debug builds only.

```csharp
private long gatherTicks
```

#### Field Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Cube_CubeWindow_hasSpace"></a> hasSpace

Whether [LastSpace](Cube.CubeWindow.md#Cube_CubeWindow_LastSpace) holds a real frame's cube, so its rotation can be reused.

```csharp
private bool hasSpace
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_CubeWindow_reportedEdgesDropped"></a> reportedEdgesDropped

The most dropped edges already logged, so a larger overflow is logged again but a repeat isn't.

```csharp
private int reportedEdgesDropped
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_CubeWindow_save"></a> save

Writes the settings to disk; called when the user lets go of the view control.

```csharp
private readonly Action save
```

#### Field Value

 [Action](https://learn.microsoft.com/dotnet/api/system.action)

### <a id="Cube_CubeWindow_settingsRevision"></a> settingsRevision

The [Revision](Cube.Core.CubeSettings.md#Cube_Core_CubeSettings_Revision) the cached colours were built from; -1 forces a first build.

```csharp
private int settingsRevision
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_CubeWindow_view"></a> view

The view orientation from the settings, rebuilt with the colours only when the settings change.

```csharp
private ViewOrientation view
```

#### Field Value

 [ViewOrientation](Cube.Core.ViewOrientation.md)

### <a id="Cube_CubeWindow_yaw"></a> yaw

The cube's current alignment yaw; kept between frames so a vertical camera doesn't spin the contents.

```csharp
private float yaw
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

## Properties

### <a id="Cube_CubeWindow_Anchor"></a> Anchor

The cube's current anchor and whether it is held at the take-off height, for the test bridge (#105).

```csharp
internal CubeAnchor Anchor { get; }
```

#### Property Value

 [CubeAnchor](Cube.Core.CubeAnchor.md)

### <a id="Cube_CubeWindow_BuildTicks"></a> BuildTicks

Total ticks spent building frames with Core ([BuildFrame](Cube.CubeWindow.md#Cube_CubeWindow_BuildFrame)), so the budget check can tell Core's share
of the frame time from ImGui's. Debug builds only.

```csharp
internal long BuildTicks { get; private set; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Cube_CubeWindow_ClippedCollision"></a> ClippedCollision

The gathered collision boxes clipped to the last frame's cube, as drawn (#31). Read by the test bridge.

```csharp
internal ClippedCollision ClippedCollision { get; }
```

#### Property Value

 [ClippedCollision](Cube.Core.ClippedCollision.md)

### <a id="Cube_CubeWindow_ClippedMesh"></a> ClippedMesh

The triangles' edges clipped to the cube and projected, as drawn in the Mesh style (#100). Read by the test bridge.

```csharp
internal ClippedMesh ClippedMesh { get; }
```

#### Property Value

 [ClippedMesh](Cube.Core.ClippedMesh.md)

### <a id="Cube_CubeWindow_Collision"></a> Collision

The collision around the cube (PROJECT.md, "Collision (phase 2)"), gathered over frames while the cube is shown.
Read by the test bridge.

```csharp
internal CollisionReader Collision { get; }
```

#### Property Value

 [CollisionReader](Cube.Game.CollisionReader.md)

### <a id="Cube_CubeWindow_CollisionColour"></a> CollisionColour

The last frame's collision colour ([Rgb](Cube.Core.CollisionCubes.md#Cube_Core_CollisionCubes_Rgb) at the opacity setting), rebuilt with [Palette](Cube.CubeWindow.md#Cube_CubeWindow_Palette).

```csharp
internal uint CollisionColour { get; private set; }
```

#### Property Value

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

### <a id="Cube_CubeWindow_DrawCost"></a> DrawCost

Time and allocations of each [Draw](Cube.CubeWindow.md#Cube_CubeWindow_Draw) call, read by the in-game test bridge to check
PROJECT.md's budgets (under 0.5 ms average, zero bytes per frame). Debug builds only.

```csharp
internal DrawCostSampler DrawCost { get; }
```

#### Property Value

 [DrawCostSampler](Cube.Core.DrawCostSampler.md)

#### Remarks

Holds the last 240 frames (about 4 seconds at 60 fps); fixed size, so measuring allocates nothing.

### <a id="Cube_CubeWindow_DrawList"></a> DrawList

This frame's primitives, in drawing order. Read by the test bridge.

```csharp
internal DrawList DrawList { get; }
```

#### Property Value

 [DrawList](Cube.Core.DrawList.md)

### <a id="Cube_CubeWindow_Errors"></a> Errors

Exceptions caught while drawing; the bridge checks there were none.

```csharp
internal ErrorReporter Errors { get; }
```

#### Property Value

 [ErrorReporter](Cube.ErrorReporter.md)

### <a id="Cube_CubeWindow_Fakes"></a> Fakes

Synthetic players the test bridge can inject (#16). Empty unless a check enables them. Debug builds only.

```csharp
internal FakePlayers Fakes { get; }
```

#### Property Value

 [FakePlayers](Cube.TestBridge.FakePlayers.md)

### <a id="Cube_CubeWindow_GatherCost"></a> GatherCost

Time and allocations of each frame's collision gathering, on frames where a gather ran. PROJECT.md's budgets
exempt "budgeted background gathering" from the draw budget, so these frames' gathering is taken out of
[DrawCost](Cube.CubeWindow.md#Cube_CubeWindow_DrawCost) and checked here against its own budget instead. Debug builds only.

```csharp
internal DrawCostSampler GatherCost { get; }
```

#### Property Value

 [DrawCostSampler](Cube.Core.DrawCostSampler.md)

### <a id="Cube_CubeWindow_LastDrewMesh"></a> LastDrewMesh

Whether the last frame drew the Mesh style (collision on and Mesh chosen).

```csharp
internal bool LastDrewMesh { get; private set; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_CubeWindow_LastFontSize"></a> LastFontSize

Text height of the last frame's labels, in pixels; the snapshot sizes label placeholders with it.

```csharp
internal float LastFontSize { get; private set; }
```

#### Property Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_CubeWindow_LastHadLocalPlayer"></a> LastHadLocalPlayer

Whether the last frame found a local player to centre the cube on.

```csharp
internal bool LastHadLocalPlayer { get; private set; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_CubeWindow_LastLabelCount"></a> LastLabelCount

How many entries of [LastLabelText](Cube.CubeWindow.md#Cube_CubeWindow_LastLabelText) the last frame filled.

```csharp
internal int LastLabelCount { get; private set; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_CubeWindow_LastLabelText"></a> LastLabelText

The text of each label the last frame drew, in drawing order, so the bridge can check it against the player
set. Holds references to the cached name strings, so recording allocates nothing. Debug builds only.

```csharp
internal string[] LastLabelText { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)\[\]

### <a id="Cube_CubeWindow_LastProjection"></a> LastProjection

The projection used for the last frame drawn.

```csharp
internal ViewProjection LastProjection { get; private set; }
```

#### Property Value

 [ViewProjection](Cube.Core.ViewProjection.md)

### <a id="Cube_CubeWindow_LastRegionMin"></a> LastRegionMin

Top-left of the last frame's drawing region, in screen pixels; the snapshot crops to this region.

```csharp
internal Vector2 LastRegionMin { get; private set; }
```

#### Property Value

 [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

### <a id="Cube_CubeWindow_LastRegionSize"></a> LastRegionSize

Size of the last frame's drawing region, in pixels.

```csharp
internal Vector2 LastRegionSize { get; private set; }
```

#### Property Value

 [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

### <a id="Cube_CubeWindow_LastSpace"></a> LastSpace

The cube space used for the last frame drawn, so the bridge can rebuild the frame independently.

```csharp
internal CubeSpace LastSpace { get; private set; }
```

#### Property Value

 [CubeSpace](Cube.Core.CubeSpace.md)

### <a id="Cube_CubeWindow_LastSubmitted"></a> LastSubmitted

How many ImGui draw calls the last frame issued; the bridge checks it equals the draw list's count.

```csharp
internal int LastSubmitted { get; private set; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_CubeWindow_LastVisibility"></a> LastVisibility

Whether the last frame drew the cube, or why not (PROJECT.md, "When hidden").

```csharp
internal Visibility LastVisibility { get; private set; }
```

#### Property Value

 [Visibility](Cube.Core.Visibility.md)

### <a id="Cube_CubeWindow_LineThickness"></a> LineThickness

Thickness of the big cube's edges, in pixels; the snapshot draws lines with it.

```csharp
internal static float LineThickness { get; }
```

#### Property Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_CubeWindow_Mesh"></a> Mesh

The collision triangles around the cube for the Mesh style (#100); only allocated and walked while that style is
chosen. Read by the test bridge.

```csharp
internal MeshReader Mesh { get; }
```

#### Property Value

 [MeshReader](Cube.Game.MeshReader.md)

### <a id="Cube_CubeWindow_MeshColour"></a> MeshColour

The Mesh style's line colour, rebuilt with [Palette](Cube.CubeWindow.md#Cube_CubeWindow_Palette).

```csharp
internal uint MeshColour { get; private set; }
```

#### Property Value

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

### <a id="Cube_CubeWindow_NameBuildBytes"></a> NameBuildBytes

Bytes allocated in [NameBuildFrames](Cube.CubeWindow.md#Cube_CubeWindow_NameBuildFrames). Debug builds only.

```csharp
internal long NameBuildBytes { get; private set; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Cube_CubeWindow_NameBuildFrames"></a> NameBuildFrames

Frames in which a player's name string was built (someone new came into view). Their allocations are the
cached label strings, so they're counted here rather than in [DrawCost](Cube.CubeWindow.md#Cube_CubeWindow_DrawCost). Debug builds only.

```csharp
internal long NameBuildFrames { get; private set; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Cube_CubeWindow_NamePosition"></a> NamePosition

The last frame's name position setting (#112).

```csharp
internal NamePosition NamePosition { get; private set; }
```

#### Property Value

 [NamePosition](Cube.Core.NamePosition.md)

### <a id="Cube_CubeWindow_Palette"></a> Palette

The last frame's player colours, rebuilt from [Settings](Cube.CubeWindow.md#Cube_CubeWindow_Settings) only when they change.

```csharp
internal PlayerPalette Palette { get; private set; }
```

#### Property Value

 [PlayerPalette](Cube.Core.PlayerPalette.md)

### <a id="Cube_CubeWindow_Players"></a> Players

The players and named NPCs inside the cube this frame (PROJECT.md, "Players" and "NPCs"), NPCs capped at
[MaxNpcs](Cube.Game.PlayerReader.md#Cube_Game_PlayerReader_MaxNpcs). Read by the test bridge.

```csharp
internal PlayerSet Players { get; }
```

#### Property Value

 [PlayerSet](Cube.Core.PlayerSet.md)

### <a id="Cube_CubeWindow_ProbeDraw"></a> ProbeDraw

Extra drawing a research probe asks the window to do on its own draw list, inside the frame, so real ImGui
costs can be measured (#99). Null outside such a probe. Runs after the frame's cost is recorded, so it never
counts against the draw budget. Debug builds only.

```csharp
internal Action<ImDrawListPtr>? ProbeDraw { get; set; }
```

#### Property Value

 [Action](https://learn.microsoft.com/dotnet/api/system.action\-1)<[ImDrawListPtr](https://github.com/goatcorp/Dalamud/blob/master/imgui/Dalamud.Bindings.ImGui/Custom/Generated/Generated/Structs.gen.cs)\>?

### <a id="Cube_CubeWindow_Settings"></a> Settings

The user's settings (PROJECT.md, "Features": settings window). The bridge changes them in memory to test them.

```csharp
internal CubeSettings Settings { get; }
```

#### Property Value

 [CubeSettings](Cube.Core.CubeSettings.md)

### <a id="Cube_CubeWindow_ShowLabels"></a> ShowLabels

The last frame's label setting.

```csharp
internal bool ShowLabels { get; private set; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_CubeWindow_ShowOwnName"></a> ShowOwnName

The last frame's own-name setting (#112).

```csharp
internal bool ShowOwnName { get; private set; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_CubeWindow_SimulatedCameraForward"></a> SimulatedCameraForward

A camera direction the bridge sets to turn the "camera" without touching the game (#62); null reads the real
camera. Debug builds only.

```csharp
internal Vector3? SimulatedCameraForward { get; set; }
```

#### Property Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)?

### <a id="Cube_CubeWindow_SimulatedJump"></a> SimulatedJump

A fake jump flag the bridge sets so a jump can be tested without any game input (#105); null reads the game's
flag. Debug builds only.

```csharp
internal bool? SimulatedJump { get; set; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)?

### <a id="Cube_CubeWindow_SimulatedPlayerY"></a> SimulatedPlayerY

A scripted height for the local player (their marker and the cube's anchor), set by the bridge to simulate a
jump's path (#105); null reads the real height. Debug builds only.

```csharp
internal float? SimulatedPlayerY { get; set; }
```

#### Property Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)?

### <a id="Cube_CubeWindow_SimulatedState"></a> SimulatedState

A game state the bridge sets to test hiding without a real cutscene or loading screen; null uses the real
state. Debug builds only.

```csharp
internal GameState? SimulatedState { get; set; }
```

#### Property Value

 [GameState](Cube.Core.GameState.md)?

### <a id="Cube_CubeWindow_SubmitTicks"></a> SubmitTicks

Total ticks spent issuing ImGui calls ([Submit](Cube.CubeWindow.md#Cube_CubeWindow_Submit)). Debug builds only.

```csharp
internal long SubmitTicks { get; private set; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Cube_CubeWindow_WidgetMin"></a> WidgetMin

The top-left of the two gizmos' combined area last frame, in screen pixels. Used for the hover test in
[PreDraw](Cube.CubeWindow.md#Cube_CubeWindow_PreDraw) and read by the test bridge.

```csharp
internal Vector2 WidgetMin { get; private set; }
```

#### Property Value

 [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

### <a id="Cube_CubeWindow_WidgetSize"></a> WidgetSize

One gizmo square's side length last frame, in pixels; 0 when they weren't drawn (cube hidden).

```csharp
internal float WidgetSize { get; private set; }
```

#### Property Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_CubeWindow_WidgetWidth"></a> WidgetWidth

The two gizmo squares' combined width last frame (twice [WidgetSize](Cube.CubeWindow.md#Cube_CubeWindow_WidgetSize)), in pixels.

```csharp
internal float WidgetWidth { get; private set; }
```

#### Property Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_CubeWindow_WireColour"></a> WireColour

The last frame's edge colour, rebuilt with [Palette](Cube.CubeWindow.md#Cube_CubeWindow_Palette).

```csharp
internal uint WireColour { get; private set; }
```

#### Property Value

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

## Methods

### <a id="Cube_CubeWindow_BuildFrame_Cube_Core_DrawList_Cube_Core_CubeSpace__Cube_Core_ViewProjection__System_ReadOnlySpan_Cube_Core_NearbyPlayer__Cube_Core_PlayerPalette__System_Boolean_System_UInt32_System_ReadOnlySpan_Cube_Core_ClippedBox__System_UInt32_System_Boolean_Cube_Core_NamePosition_"></a> BuildFrame\(DrawList, in CubeSpace, in ViewProjection, ReadOnlySpan<NearbyPlayer\>, in PlayerPalette, bool, uint, ReadOnlySpan<ClippedBox\>, uint, bool, NamePosition\)

Builds this frame's draw list for a given drawing region. Separate from the ImGui calls so the bridge can
check exactly what Core produced.

```csharp
internal static void BuildFrame(DrawList list, in CubeSpace space, in ViewProjection projection, ReadOnlySpan<NearbyPlayer> players, in PlayerPalette palette, bool labels, uint wireColour, ReadOnlySpan<ClippedBox> collision, uint collisionColour, bool selfLabel = false, NamePosition namePosition = NamePosition.Side)
```

#### Parameters

`list` [DrawList](Cube.Core.DrawList.md)

The list to fill (cleared first).

`space` [CubeSpace](Cube.Core.CubeSpace.md)

The cube.

`projection` [ViewProjection](Cube.Core.ViewProjection.md)

The projection.

`players` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[NearbyPlayer](Cube.Core.NearbyPlayer.md)\>

The players inside the cube.

`palette` [PlayerPalette](Cube.Core.PlayerPalette.md)

Player colours.

`labels` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether to add name labels.

`wireColour` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

The big cube's edge colour.

`collision` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[ClippedBox](Cube.Core.ClippedBox.md)\>

The collision boxes clipped to the cube (PROJECT.md, "Collision (phase 2)").

`collisionColour` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

The collision boxes' colour.

`selfLabel` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether the local player's own name is labelled (#112).

`namePosition` [NamePosition](Cube.Core.NamePosition.md)

Whether names sit beside or above their cubes (#112).

### <a id="Cube_CubeWindow_Draw"></a> Draw\(\)

Draws the window's contents, measuring the cost in Debug builds.

```csharp
public override void Draw()
```

### <a id="Cube_CubeWindow_DrawContents"></a> DrawContents\(\)

The plugin's per-frame draw path: builds the frame with Core and draws it.

```csharp
private void DrawContents()
```

### <a id="Cube_CubeWindow_DrawGizmo_System_Int32_System_Numerics_Vector2_System_Single_Dalamud_Bindings_ImGuizmo_ImGuizmoOperation_System_Numerics_Vector3_"></a> DrawGizmo\(int, Vector2, float, ImGuizmoOperation, Vector3\)

Draws one of the two gizmos in its own square and reports what a drag in it changed (#97).

```csharp
private GizmoChange DrawGizmo(int id, Vector2 min, float side, ImGuizmoOperation operation, Vector3 size)
```

#### Parameters

`id` [int](https://learn.microsoft.com/dotnet/api/system.int32)

ImGuizmo ID for this gizmo; differs between the two.

`min` [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

Top-left of the gizmo's square, in pixels.

`side` [float](https://learn.microsoft.com/dotnet/api/system.single)

The square's side length.

`operation` [ImGuizmoOperation](https://github.com/goatcorp/Dalamud/blob/master/imgui/Dalamud.Bindings.ImGuizmo/Generated/Enums/ImGuizmoOperation.cs)

Which operation the gizmo offers.

`size` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The cube's current size.

#### Returns

 [GizmoChange](Cube.Core.GizmoChange.md)

#### Remarks

Each gizmo gets its own rectangle and ImGuizmo ID so hovering or dragging one never activates the other. The
rotate gizmo only offers rotation and the resize gizmo only scaling, so a drag can only change its own part;
the caller ignores the other flag anyway. Nothing is allocated: the matrices are structs passed by reference.

### <a id="Cube_CubeWindow_DrawViewControl_System_Numerics_Vector2_System_Numerics_Vector2_"></a> DrawViewControl\(Vector2, Vector2\)

Draws two ImGuizmo gizmos side by side in the chosen corner (PROJECT.md, "View orientation control"): a rotate
gizmo (a ring per axis plus an outer screen ring) in the corner and, inward from it, a resize gizmo (#90, #97:
scale handles that resize the cube per axis, and a centre handle that zooms). Applies any turn or resize.

```csharp
private void DrawViewControl(Vector2 regionMin, Vector2 regionSize)
```

#### Parameters

`regionMin` [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

Top-left of the drawing region, in pixels.

`regionSize` [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

Size of the drawing region, in pixels.

#### Remarks

<p>
They are separate (#97) because the scale handles used to end on the rotation rings, so grabbing a handle's end
often started a rotation. Each is handed the current orientation and size as a model matrix each frame
([Build](Cube.Core.GizmoModel.md#Cube_Core_GizmoModel_Build)) by [DrawGizmo](Cube.CubeWindow.md#Cube_CubeWindow_DrawGizmo). Local mode puts the rings around, and the scale
handles along, the cube's own axes, so the colours match them and a handle always changes the matching extent. A changed
matrix is read back with [Read](Cube.Core.GizmoModel.md#Cube_Core_GizmoModel_Read), which keeps the turn and the resize apart; the
projection picks a new view up through the settings revision, so it is only rebuilt when the view really
changes. The camera and the player never feed into this (PROJECT.md: only the control turns the view).
</p>
<p>
The file is saved once the drag is over (mouse released and no change this frame), not on every frame of a
drag. ImGuizmo's global settings (gizmo size, orthographic mode) are left alone because other plugins share
them; the size comes from the rectangle instead ([GizmoRectScale](Cube.CubeWindow.md#Cube_CubeWindow_GizmoRectScale)). Nothing here allocates: the
matrices are structs passed by reference.
</p>

### <a id="Cube_CubeWindow_PreDraw"></a> PreDraw\(\)

Stops ImGui moving the window while the mouse is on the view control or the user is turning the view, so a
drag on the control turns or resizes the cube instead of dragging the window around (#63, #79, #90).

```csharp
public override void PreDraw()
```

#### Remarks

ImGuizmo reads the mouse directly rather than claiming it as an ImGui item, so without this ImGui would treat
the drag as a drag on empty window space. An invisible button over the control can't be used instead: ImGuizmo
ignores clicks while another item is active. Uses last frame's position, which is where the mouse went down.

### <a id="Cube_CubeWindow_ReadJumping_System_Single_"></a> ReadJumping\(float\)

Reads the game's jump flag, treating a failed read as "not jumping" so the cube simply follows the player as it
did before #105. In Debug builds also logs the flags and height around each jump (research for #105).

```csharp
private bool ReadJumping(float playerY)
```

#### Parameters

`playerY` [float](https://learn.microsoft.com/dotnet/api/system.single)

The local player's real height this frame.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_CubeWindow_SafeDrawContents"></a> SafeDrawContents\(\)

Draws, catching and logging any exception so it never reaches the game (CLAUDE.md, "Dalamud rules"). A failed
frame is simply empty. The try costs nothing when nothing is thrown.

```csharp
private void SafeDrawContents()
```

### <a id="Cube_CubeWindow_Submit_Dalamud_Bindings_ImGui_ImDrawListPtr_Cube_Core_DrawList_System_ReadOnlySpan_Cube_Core_NearbyPlayer__"></a> Submit\(ImDrawListPtr, DrawList, ReadOnlySpan<NearbyPlayer\>\)

Issues one ImGui call per primitive, in order.

```csharp
private int Submit(ImDrawListPtr target, DrawList list, ReadOnlySpan<NearbyPlayer> players)
```

#### Parameters

`target` [ImDrawListPtr](https://github.com/goatcorp/Dalamud/blob/master/imgui/Dalamud.Bindings.ImGui/Custom/Generated/Generated/Structs.gen.cs)

The window's ImGui draw list.

`list` [DrawList](Cube.Core.DrawList.md)

The sorted primitives, and the corners of any polygons.

`players` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[NearbyPlayer](Cube.Core.NearbyPlayer.md)\>

This frame's players, for label text.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

How many calls were made.

#### Remarks

Labels are clipped to the window by the window draw list's own clip rectangle, so names near the edge are cut
off rather than spilling over the game. The text is the cached name string, so no string is built here.

### <a id="Cube_CubeWindow_SubmitMesh_Dalamud_Bindings_ImGui_ImDrawListPtr_"></a> SubmitMesh\(ImDrawListPtr\)

Issues one ImGui line per projected mesh edge (#100).

```csharp
private void SubmitMesh(ImDrawListPtr target)
```

#### Parameters

`target` [ImDrawListPtr](https://github.com/goatcorp/Dalamud/blob/master/imgui/Dalamud.Bindings.ImGui/Custom/Generated/Generated/Structs.gen.cs)

The window's ImGui draw list.

### <a id="Cube_CubeWindow_UpdateCollision_System_Numerics_Vector3_System_Numerics_Vector3_Cube_Core_CollisionDetail_"></a> UpdateCollision\(Vector3, Vector3, CollisionDetail\)

Lets the collision reader do this frame's share of gathering (PROJECT.md, "Collision (phase 2)"). In Debug builds
a frame where a gather ran is timed on its own and recorded in <code>GatherCost</code> (Debug only), and kept out of the
draw budget, which PROJECT.md says doesn't cover budgeted background gathering.

```csharp
private void UpdateCollision(Vector3 centre, Vector3 size, CollisionDetail detail)
```

#### Parameters

`centre` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The cube's centre.

`size` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The cube's width, height and depth in yalms.

`detail` [CollisionDetail](Cube.Core.CollisionDetail.md)

The collision detail setting (#32).

### <a id="Cube_CubeWindow_UpdateMesh_Cube_Core_CubeSpace__"></a> UpdateMesh\(in CubeSpace\)

The Mesh style's frame work (#100): lets the reader gather its share of triangles, then lets the clipped mesh
rebuild its share, both within time budgets. In Debug builds a frame where either did work is timed on its own
like [UpdateCollision](Cube.CubeWindow.md#Cube_CubeWindow_UpdateCollision), and kept out of the draw cost, since this is background gathering.

```csharp
private void UpdateMesh(in CubeSpace space)
```

#### Parameters

`space` [CubeSpace](Cube.Core.CubeSpace.md)

This frame's cube.
