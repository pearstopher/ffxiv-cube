# <a id="Cube_Core_DrawList"></a> Class DrawList

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

The frame's primitives, in window pixels, sorted far to near (PROJECT.md, "Draw order": painter's algorithm,
so near cubes cover far ones). Built by [CubeGeometry](Cube.Core.CubeGeometry.md#Cube_Core_CubeGeometry), drawn by the plugin, and dumped by the
test bridge.

```csharp
public sealed class DrawList
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[DrawList](Cube.Core.DrawList.md)


## Remarks

The buffer is allocated once with a fixed capacity and reused every frame (PROJECT.md, "Efficiency and memory":
no per-frame allocations, bounded memory). If a frame needs more, the extra primitives are dropped and counted
in [Dropped](Cube.Core.DrawList.md#Cube_Core_DrawList_Dropped) rather than growing the buffer.

## Constructors

### <a id="Cube_Core_DrawList__ctor_System_Int32_"></a> DrawList\(int\)

Creates an empty list.

```csharp
public DrawList(int capacity)
```

#### Parameters

`capacity` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The most primitives a frame can hold.

## Fields

### <a id="Cube_Core_DrawList_BackToFront"></a> BackToFront

Sorts far first, then by insertion order. A cached delegate: sorting with an `System.Collections.Generic.IComparer%601`
would wrap it in a new delegate on every call, which allocates each frame.

```csharp
private static readonly Comparison<DrawPrimitive> BackToFront
```

#### Field Value

 [Comparison](https://learn.microsoft.com/dotnet/api/system.comparison\-1)<[DrawPrimitive](Cube.Core.DrawPrimitive.md)\>

### <a id="Cube_Core_DrawList_items"></a> items

The reused buffer.

```csharp
private readonly DrawPrimitive[] items
```

#### Field Value

 [DrawPrimitive](Cube.Core.DrawPrimitive.md)\[\]

## Properties

### <a id="Cube_Core_DrawList_Capacity"></a> Capacity

How many primitives the list can hold.

```csharp
public int Capacity { get; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_DrawList_Count"></a> Count

How many primitives were added this frame (at most [Capacity](Cube.Core.DrawList.md#Cube_Core_DrawList_Capacity)).

```csharp
public int Count { get; private set; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_DrawList_Dropped"></a> Dropped

How many primitives were dropped this frame because the list was full.

```csharp
public int Dropped { get; private set; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_DrawList_Items"></a> Items

This frame's primitives, in drawing order once [SortBackToFront](Cube.Core.DrawList.md#Cube_Core_DrawList_SortBackToFront) has run.

```csharp
public ReadOnlySpan<DrawPrimitive> Items { get; }
```

#### Property Value

 [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[DrawPrimitive](Cube.Core.DrawPrimitive.md)\>

## Methods

### <a id="Cube_Core_DrawList_Add_Cube_Core_DrawPrimitive_"></a> Add\(DrawPrimitive\)

Appends a primitive, or counts it as dropped when full.

```csharp
private void Add(DrawPrimitive primitive)
```

#### Parameters

`primitive` [DrawPrimitive](Cube.Core.DrawPrimitive.md)

The primitive; its sequence number is set here.

### <a id="Cube_Core_DrawList_AddLine_System_Numerics_Vector2_System_Numerics_Vector2_System_Single_System_UInt32_"></a> AddLine\(Vector2, Vector2, float, uint\)

Adds a line.

```csharp
public void AddLine(Vector2 a, Vector2 b, float depth, uint colour)
```

#### Parameters

`a` [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

Start (pixels).

`b` [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

End (pixels).

`depth` [float](https://learn.microsoft.com/dotnet/api/system.single)

Distance from the viewer.

`colour` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

Packed colour.

### <a id="Cube_Core_DrawList_AddQuad_System_Numerics_Vector2_System_Numerics_Vector2_System_Numerics_Vector2_System_Numerics_Vector2_System_Single_System_UInt32_"></a> AddQuad\(Vector2, Vector2, Vector2, Vector2, float, uint\)

Adds a filled quad; points go round the outline in order.

```csharp
public void AddQuad(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float depth, uint colour)
```

#### Parameters

`a` [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

First corner (pixels).

`b` [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

Second corner.

`c` [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

Third corner.

`d` [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

Fourth corner.

`depth` [float](https://learn.microsoft.com/dotnet/api/system.single)

Distance from the viewer.

`colour` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

Packed colour.

### <a id="Cube_Core_DrawList_Clear"></a> Clear\(\)

Empties the list for a new frame, keeping the buffer.

```csharp
public void Clear()
```

### <a id="Cube_Core_DrawList_CompareBackToFront_Cube_Core_DrawPrimitive_Cube_Core_DrawPrimitive_"></a> CompareBackToFront\(DrawPrimitive, DrawPrimitive\)

Orders by depth descending, then by sequence ascending, so the sort is stable.

```csharp
private static int CompareBackToFront(DrawPrimitive x, DrawPrimitive y)
```

#### Parameters

`x` [DrawPrimitive](Cube.Core.DrawPrimitive.md)

One primitive.

`y` [DrawPrimitive](Cube.Core.DrawPrimitive.md)

The other.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_DrawList_SortBackToFront"></a> SortBackToFront\(\)

Sorts so the farthest primitive comes first; ties keep the order they were added in.

```csharp
public void SortBackToFront()
```
