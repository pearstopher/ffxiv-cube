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
[DrawList](Cube.CubeWindow.md#Cube_CubeWindow_DrawList), sort it far to near, then issue one ImGui call per primitive. Nothing here allocates
(PROJECT.md, "Efficiency and memory"); Dalamud skips [Draw](Cube.CubeWindow.md#Cube_CubeWindow_Draw) entirely while the window is closed.

## Constructors

### <a id="Cube_CubeWindow__ctor"></a> CubeWindow\(\)

Creates the window, closed, with a sensible minimum size.

```csharp
public CubeWindow()
```

## Fields

### <a id="Cube_CubeWindow_DrawListCapacity"></a> DrawListCapacity

Most primitives a frame can hold: the 12 cube edges plus three faces for every character slot in the
object table (200), with room to spare. Fixed, so memory never grows (PROJECT.md: bounded memory).

```csharp
internal const int DrawListCapacity = 1024
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_CubeWindow_Elevation"></a> Elevation

The view's elevation in radians (PROJECT.md: 30°).

```csharp
private static readonly float Elevation
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_CubeWindow_NameSlots"></a> NameSlots

Name cache slots: the 200 character slots, plus room for the Debug-only fake players above them (#16).

```csharp
internal const int NameSlots = 256
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_CubeWindow_WireColour"></a> WireColour

Colour of the big cube's edges: light grey, slightly transparent.

```csharp
private static readonly uint WireColour
```

#### Field Value

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

### <a id="Cube_CubeWindow_WireThickness"></a> WireThickness

Thickness of the big cube's edges, in pixels.

```csharp
private const float WireThickness = 1.5
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_CubeWindow_yaw"></a> yaw

The view's current yaw; kept between frames so a vertical camera doesn't spin the view.

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

### <a id="Cube_CubeWindow_Fakes"></a> Fakes

Synthetic players the test bridge can inject (#16). Empty unless a check enables them. Debug builds only.

```csharp
internal FakePlayers Fakes { get; }
```

#### Property Value

 [FakePlayers](Cube.TestBridge.FakePlayers.md)

### <a id="Cube_CubeWindow_LastHadLocalPlayer"></a> LastHadLocalPlayer

Whether the last frame found a local player to centre the cube on.

```csharp
internal bool LastHadLocalPlayer { get; private set; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

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

### <a id="Cube_CubeWindow_LineThickness"></a> LineThickness

Thickness of the big cube's edges, in pixels; the snapshot draws lines with it.

```csharp
internal static float LineThickness { get; }
```

#### Property Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_CubeWindow_Players"></a> Players

The players inside the cube this frame (PROJECT.md, "Players"). Read by the test bridge.

```csharp
internal PlayerSet Players { get; }
```

#### Property Value

 [PlayerSet](Cube.Core.PlayerSet.md)

## Methods

### <a id="Cube_CubeWindow_BuildFrame_Cube_Core_DrawList_Cube_Core_CubeSpace__Cube_Core_ViewProjection__"></a> BuildFrame\(DrawList, in CubeSpace, in ViewProjection\)

Builds this frame's draw list for a given drawing region. Separate from the ImGui calls so the bridge can
check exactly what Core produced.

```csharp
internal static void BuildFrame(DrawList list, in CubeSpace space, in ViewProjection projection)
```

#### Parameters

`list` [DrawList](Cube.Core.DrawList.md)

The list to fill (cleared first).

`space` [CubeSpace](Cube.Core.CubeSpace.md)

The cube.

`projection` [ViewProjection](Cube.Core.ViewProjection.md)

The projection.

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

### <a id="Cube_CubeWindow_Submit_Dalamud_Bindings_ImGui_ImDrawListPtr_System_ReadOnlySpan_Cube_Core_DrawPrimitive__"></a> Submit\(ImDrawListPtr, ReadOnlySpan<DrawPrimitive\>\)

Issues one ImGui call per primitive, in order.

```csharp
private static int Submit(ImDrawListPtr target, ReadOnlySpan<DrawPrimitive> items)
```

#### Parameters

`target` [ImDrawListPtr](https://github.com/goatcorp/Dalamud/blob/master/imgui/Dalamud.Bindings.ImGui/Custom/Generated/Generated/Structs.gen.cs)

The window's ImGui draw list.

`items` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[DrawPrimitive](Cube.Core.DrawPrimitive.md)\>

The sorted primitives.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

How many calls were made.
