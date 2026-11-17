# <a id="Cube_Snapshot_SnapshotRenderer"></a> Class SnapshotRenderer

Namespace: [Cube.Snapshot](Cube.Snapshot.md)  
Assembly: Cube.Snapshot.dll  

Renders a draw-list dump (<code>drawlist.json</code>, written by the bridge's <code>frame-snapshot</code> check) onto a
[Canvas](Cube.Snapshot.Canvas.md#Cube_Snapshot_Canvas), in the dump's order: the list is already sorted far to near, so painting in order
reproduces what ImGui drew.

```csharp
public static class SnapshotRenderer
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[SnapshotRenderer](Cube.Snapshot.SnapshotRenderer.md)


## Remarks

Dump format: <code>regionMin</code> and <code>regionSize</code> (the window's drawing region in screen pixels) and
<code>primitives</code>, each with <code>kind</code> (<code>line</code>, <code>quad</code> or <code>label</code>), <code>points</code>
(<code>[[x, y], ...]</code> in screen pixels; one anchor point for a label), <code>colour</code> (<code>0xAABBGGRR</code> as a
number), for lines <code>thickness</code>, and for labels <code>chars</code> and <code>fontSize</code>.

## Fields

### <a id="Cube_Snapshot_SnapshotRenderer_Background"></a> Background

Background colour: close to ImGui's default dark window background.

```csharp
public const uint Background = 4280163870
```

#### Field Value

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

### <a id="Cube_Snapshot_SnapshotRenderer_DefaultFontSize"></a> DefaultFontSize

Text height assumed when a dumped label doesn't say: ImGui's default font size.

```csharp
public const float DefaultFontSize = 13
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Snapshot_SnapshotRenderer_DefaultLabelChars"></a> DefaultLabelChars

Label length assumed when a dumped label doesn't say (and in the demo, which has no names).

```csharp
public const int DefaultLabelChars = 8
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Snapshot_SnapshotRenderer_DefaultThickness"></a> DefaultThickness

Line thickness used when a dumped line doesn't say.

```csharp
public const float DefaultThickness = 1.5
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

## Methods

### <a id="Cube_Snapshot_SnapshotRenderer_FillLabelBar_Cube_Snapshot_Canvas_System_Numerics_Vector2_System_Int32_System_Single_System_UInt32_"></a> FillLabelBar\(Canvas, Vector2, int, float, uint\)

Draws a label as a bar where the text would be: the snapshot has no font, and the dump never contains names
(PROJECT.md, "Privacy"), so the bar shows where each label sits and roughly how long it is.

```csharp
private static void FillLabelBar(Canvas canvas, Vector2 anchor, int chars, float fontSize, uint colour)
```

#### Parameters

`canvas` [Canvas](Cube.Snapshot.Canvas.md)

Target.

`anchor` [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

The label's left end, vertically centred (region pixels).

`chars` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Name length in characters.

`fontSize` [float](https://learn.microsoft.com/dotnet/api/system.single)

Text height in pixels.

`colour` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

Text colour.

### <a id="Cube_Snapshot_SnapshotRenderer_NewCanvas_System_Numerics_Vector2_"></a> NewCanvas\(Vector2\)

A background-filled canvas of the region's size (at least 1×1).

```csharp
private static Canvas NewCanvas(Vector2 regionSize)
```

#### Parameters

`regionSize` [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

Region size in pixels.

#### Returns

 [Canvas](Cube.Snapshot.Canvas.md)

### <a id="Cube_Snapshot_SnapshotRenderer_Paint_System_Numerics_Vector2_System_Numerics_Vector2_System_ReadOnlySpan_Cube_Core_DrawPrimitive__"></a> Paint\(Vector2, Vector2, ReadOnlySpan<DrawPrimitive\>\)

Paints Core's primitives directly, in order. Used by <code>--demo</code> to check the look of Core's output
without the game.

```csharp
public static Canvas Paint(Vector2 regionMin, Vector2 regionSize, ReadOnlySpan<DrawPrimitive> items)
```

#### Parameters

`regionMin` [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

Screen position of the region's top-left, subtracted from every point.

`regionSize` [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

Region size: the canvas size.

`items` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[DrawPrimitive](Cube.Core.DrawPrimitive.md)\>

The sorted primitives.

#### Returns

 [Canvas](Cube.Snapshot.Canvas.md)

### <a id="Cube_Snapshot_SnapshotRenderer_ReadPoint_System_Text_Json_JsonElement_"></a> ReadPoint\(JsonElement\)

Reads <code>[x, y]</code>.

```csharp
private static Vector2 ReadPoint(JsonElement element)
```

#### Parameters

`element` [JsonElement](https://learn.microsoft.com/dotnet/api/system.text.json.jsonelement)

A two-number JSON array.

#### Returns

 [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

### <a id="Cube_Snapshot_SnapshotRenderer_Render_System_String_"></a> Render\(string\)

Renders a dump to a canvas the size of the dumped region.

```csharp
public static Canvas Render(string json)
```

#### Parameters

`json` [string](https://learn.microsoft.com/dotnet/api/system.string)

The dump's JSON text.

#### Returns

 [Canvas](Cube.Snapshot.Canvas.md)
