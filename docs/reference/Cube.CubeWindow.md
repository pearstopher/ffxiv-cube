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

Each frame: build the [CubeSpace](Cube.Core.CubeSpace.md#Cube_Core_CubeSpace) and fitted [ViewProjection](Cube.Core.ViewProjection.md#Cube_Core_ViewProjection), let Core fill the reused
[DrawList](Cube.CubeWindow.md#Cube_CubeWindow_DrawList), sort it far to near, issue one ImGui call per primitive, then draw the view control on
top ([DrawViewControl](Cube.CubeWindow.md#Cube_CubeWindow_DrawViewControl)). Nothing here allocates
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

Reads login, loading, cutscene and GPose state.

`save` [Action](https://learn.microsoft.com/dotnet/api/system.action)

Writes the settings to disk, after the user turns the view with the view control.

## Fields

### <a id="Cube_CubeWindow_DrawListCapacity"></a> DrawListCapacity

Most primitives a frame can hold: the 12 cube edges plus three faces and a label for every player the set can
hold (200) and every NPC (50), with room to spare. Fixed, so memory never grows (PROJECT.md: bounded memory).

```csharp
internal const int DrawListCapacity = 1280
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

### <a id="Cube_CubeWindow_NameSlots"></a> NameSlots

Name cache slots: one per object table index (players and NPCs are both keyed by slot), plus room above them
for a full crowd of Debug-only fake players and fake NPCs (#16, #23, #74), so checks can measure at the caps.

```csharp
internal const int NameSlots = 969
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

### <a id="Cube_CubeWindow_gameState"></a> gameState

Reads the states that hide the cube.

```csharp
private readonly GameStateReader gameState
```

#### Field Value

 [GameStateReader](Cube.Game.GameStateReader.md)

### <a id="Cube_CubeWindow_hasSpace"></a> hasSpace

Whether [LastSpace](Cube.CubeWindow.md#Cube_CubeWindow_LastSpace) holds a real frame's cube, so its rotation can be reused.

```csharp
private bool hasSpace
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

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

### <a id="Cube_CubeWindow_turning"></a> turning

Whether the user is turning the view with the control: set when ImGuizmo changes the view, cleared (and the
settings saved) once the mouse is released and the view has settled.

```csharp
private bool turning
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

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

### <a id="Cube_CubeWindow_SimulatedCameraForward"></a> SimulatedCameraForward

A camera direction the bridge sets to turn the "camera" without touching the game (#62); null reads the real
camera. Debug builds only.

```csharp
internal Vector3? SimulatedCameraForward { get; set; }
```

#### Property Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)?

### <a id="Cube_CubeWindow_SimulatedState"></a> SimulatedState

A game state the bridge sets to test hiding without a real cutscene or loading screen; null uses the real
state. Debug builds only.

```csharp
internal GameState? SimulatedState { get; set; }
```

#### Property Value

 [GameState](Cube.Core.GameState.md)?

### <a id="Cube_CubeWindow_WidgetMin"></a> WidgetMin

The view control's top-left last frame, in screen pixels. Used for the hover test in [PreDraw](Cube.CubeWindow.md#Cube_CubeWindow_PreDraw) and
read by the test bridge.

```csharp
internal Vector2 WidgetMin { get; private set; }
```

#### Property Value

 [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

### <a id="Cube_CubeWindow_WidgetSize"></a> WidgetSize

The view control's side length last frame, in pixels; 0 when it wasn't drawn (cube hidden).

```csharp
internal float WidgetSize { get; private set; }
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

### <a id="Cube_CubeWindow_BuildFrame_Cube_Core_DrawList_Cube_Core_CubeSpace__Cube_Core_ViewProjection__System_ReadOnlySpan_Cube_Core_NearbyPlayer__Cube_Core_PlayerPalette__System_Boolean_System_UInt32_"></a> BuildFrame\(DrawList, in CubeSpace, in ViewProjection, ReadOnlySpan<NearbyPlayer\>, in PlayerPalette, bool, uint\)

Builds this frame's draw list for a given drawing region. Separate from the ImGui calls so the bridge can
check exactly what Core produced.

```csharp
internal static void BuildFrame(DrawList list, in CubeSpace space, in ViewProjection projection, ReadOnlySpan<NearbyPlayer> players, in PlayerPalette palette, bool labels, uint wireColour)
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

### <a id="Cube_CubeWindow_DrawViewControl_System_Numerics_Vector2_System_Numerics_Vector2_"></a> DrawViewControl\(Vector2, Vector2\)

Draws ImGuizmo's rotate gizmo in the chosen corner (PROJECT.md, "View orientation control": a ring per axis
plus an outer screen ring) and applies any turn the user makes with it.

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
ImGuizmo is handed the current orientation as a model matrix each frame ([ToGizmoModel](Cube.Core.ViewOrientation.md#Cube_Core_ViewOrientation_ToGizmoModel))
and turns it while a ring is dragged. Local mode puts the x, y and z rings around the cube's own axes, so the
ring colours match them. A changed matrix is read back into the settings; the projection then picks it up
through the settings revision, so it is only rebuilt when the view really changes. The camera and the player
never feed into this (PROJECT.md: only the control turns the view).
</p>
<p>
The file is saved once the turn is over (mouse released and no change this frame), not on every frame of a
drag. ImGuizmo's global settings (gizmo size, orthographic mode) are left alone because other plugins share
them; the size comes from the rectangle instead ([GizmoRectScale](Cube.CubeWindow.md#Cube_CubeWindow_GizmoRectScale)). Nothing here allocates: the
matrices are structs passed by reference.
</p>

### <a id="Cube_CubeWindow_PreDraw"></a> PreDraw\(\)

Stops ImGui moving the window while the mouse is on the view control or the user is turning the view, so a
drag on the control turns the cube instead of dragging the window around.

```csharp
public override void PreDraw()
```

#### Remarks

ImGuizmo reads the mouse directly rather than claiming it as an ImGui item, so without this ImGui would treat
the drag as a drag on empty window space. An invisible button over the control can't be used instead: ImGuizmo
ignores clicks while another item is active. Uses last frame's position, which is where the mouse went down.

### <a id="Cube_CubeWindow_SafeDrawContents"></a> SafeDrawContents\(\)

Draws, catching and logging any exception so it never reaches the game (CLAUDE.md, "Dalamud rules"). A failed
frame is simply empty. The try costs nothing when nothing is thrown.

```csharp
private void SafeDrawContents()
```

### <a id="Cube_CubeWindow_Submit_Dalamud_Bindings_ImGui_ImDrawListPtr_System_ReadOnlySpan_Cube_Core_DrawPrimitive__System_ReadOnlySpan_Cube_Core_NearbyPlayer__"></a> Submit\(ImDrawListPtr, ReadOnlySpan<DrawPrimitive\>, ReadOnlySpan<NearbyPlayer\>\)

Issues one ImGui call per primitive, in order.

```csharp
private int Submit(ImDrawListPtr target, ReadOnlySpan<DrawPrimitive> items, ReadOnlySpan<NearbyPlayer> players)
```

#### Parameters

`target` [ImDrawListPtr](https://github.com/goatcorp/Dalamud/blob/master/imgui/Dalamud.Bindings.ImGui/Custom/Generated/Generated/Structs.gen.cs)

The window's ImGui draw list.

`items` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[DrawPrimitive](Cube.Core.DrawPrimitive.md)\>

The sorted primitives.

`players` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[NearbyPlayer](Cube.Core.NearbyPlayer.md)\>

This frame's players, for label text.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

How many calls were made.

#### Remarks

Labels are clipped to the window by the window draw list's own clip rectangle, so names near the edge are cut
off rather than spilling over the game. The text is the cached name string, so no string is built here.
