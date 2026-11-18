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

### <a id="Cube_Core_DrawList__ctor_System_Int32_System_Int32_"></a> DrawList\(int, int\)

Creates an empty list.

```csharp
public DrawList(int capacity, int pointCapacity = 0)
```

#### Parameters

`capacity` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The most primitives a frame can hold.

`pointCapacity` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The most polygon corners a frame can hold, up to [MaxPointCapacity](Cube.Core.DrawList.md#Cube_Core_DrawList_MaxPointCapacity); 0 (the default) gives room for
one [MaxCorners](Cube.Core.ClippedBox.md#Cube_Core_ClippedBox_MaxCorners)-corner polygon per eight primitives.

## Fields

### <a id="Cube_Core_DrawList_MaxPointCapacity"></a> MaxPointCapacity

The largest point buffer: a polygon's start is stored in 16 bits ([FirstPoint](Cube.Core.DrawPrimitive.md#Cube_Core_DrawPrimitive_FirstPoint)) to
keep primitives small.

```csharp
public const int MaxPointCapacity = 65536
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_DrawList_items"></a> items

The reused buffer.

```csharp
private readonly DrawPrimitive[] items
```

#### Field Value

 [DrawPrimitive](Cube.Core.DrawPrimitive.md)\[\]

### <a id="Cube_Core_DrawList_keys"></a> keys

The reused sort keys, one per primitive ([SortKey](Cube.Core.DrawList.md#Cube_Core_DrawList_SortKey)).

```csharp
private readonly long[] keys
```

#### Field Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)\[\]

### <a id="Cube_Core_DrawList_pointCount"></a> pointCount

How many of [points](Cube.Core.DrawList.md#Cube_Core_DrawList_points) are used this frame.

```csharp
private int pointCount
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_DrawList_points"></a> points

The reused corner buffer for polygons ([Polygon](Cube.Core.PrimitiveKind.md#Cube_Core_PrimitiveKind_Polygon)). Corners stay where they were added
while the primitives are sorted, since each polygon refers to its own range.

```csharp
private readonly Vector2[] points
```

#### Field Value

 [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)\[\]

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

### <a id="Cube_Core_DrawList_PointCapacity"></a> PointCapacity

How many polygon corners the list can hold.

```csharp
public int PointCapacity { get; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Methods

### <a id="Cube_Core_DrawList_Add_Cube_Core_DrawPrimitive_"></a> Add\(DrawPrimitive\)

Appends a primitive, or counts it as dropped when full.

```csharp
private void Add(DrawPrimitive primitive)
```

#### Parameters

`primitive` [DrawPrimitive](Cube.Core.DrawPrimitive.md)

The primitive; its sequence number is set here.

### <a id="Cube_Core_DrawList_AddLabel_System_Numerics_Vector2_System_Single_System_UInt32_System_Int32_System_Boolean_"></a> AddLabel\(Vector2, float, uint, int, bool\)

Adds a player's name label ([Label](Cube.Core.PrimitiveKind.md#Cube_Core_PrimitiveKind_Label)).

```csharp
public void AddLabel(Vector2 anchor, float depth, uint colour, int tag, bool centred = false)
```

#### Parameters

`anchor` [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

Left end of the label, vertically centred on it (pixels).

`depth` [float](https://learn.microsoft.com/dotnet/api/system.single)

Distance from the viewer: the player cube's, so the label sorts with its cube.

`colour` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

Text colour.

`tag` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The player's index + 1; the text is that player's name.

`centred` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

True for a label centred above its cube (#112): <code class="paramref">anchor</code> is then the middle of the text's bottom
edge. Stored as [B](Cube.Core.DrawPrimitive.md#Cube_Core_DrawPrimitive_B).X = 1 (unused by labels otherwise), so the primitive stays the same size.

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

### <a id="Cube_Core_DrawList_AddPolygon_System_ReadOnlySpan_System_Numerics_Vector2__System_Single_System_UInt32_System_Int32_"></a> AddPolygon\(ReadOnlySpan<Vector2\>, float, uint, int\)

Adds a filled convex polygon ([Polygon](Cube.Core.PrimitiveKind.md#Cube_Core_PrimitiveKind_Polygon)); its corners are copied into the list's
point buffer. Dropped like any other primitive when either buffer is full.

```csharp
public void AddPolygon(ReadOnlySpan<Vector2> corners, float depth, uint colour, int tag = 0)
```

#### Parameters

`corners` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)\>

3 to [MaxCorners](Cube.Core.ClippedBox.md#Cube_Core_ClippedBox_MaxCorners) corners in order around the outline (pixels).

`depth` [float](https://learn.microsoft.com/dotnet/api/system.single)

Distance from the viewer.

`colour` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

Packed colour.

`tag` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Owner ([Tag](Cube.Core.DrawPrimitive.md#Cube_Core_DrawPrimitive_Tag)).

### <a id="Cube_Core_DrawList_AddQuad_System_Numerics_Vector2_System_Numerics_Vector2_System_Numerics_Vector2_System_Numerics_Vector2_System_Single_System_UInt32_System_Int32_"></a> AddQuad\(Vector2, Vector2, Vector2, Vector2, float, uint, int\)

Adds a filled quad; points go round the outline in order.

```csharp
public void AddQuad(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float depth, uint colour, int tag = 0)
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

`tag` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Owner: 0 for the scene, player index + 1 for a player's cube, negative for a collision box ([Tag](Cube.Core.DrawPrimitive.md#Cube_Core_DrawPrimitive_Tag)).

### <a id="Cube_Core_DrawList_Clear"></a> Clear\(\)

Empties the list for a new frame, keeping the buffer.

```csharp
public void Clear()
```

### <a id="Cube_Core_DrawList_CompareBackToFront_Cube_Core_DrawPrimitive__Cube_Core_DrawPrimitive__"></a> CompareBackToFront\(in DrawPrimitive, in DrawPrimitive\)

The drawing order spelled out: depth descending, then sequence ascending, so the sort is stable.
[SortKey](Cube.Core.DrawList.md#Cube_Core_DrawList_SortKey) encodes the same order as one number; the unit tests check the two agree.

```csharp
public static int CompareBackToFront(in DrawPrimitive x, in DrawPrimitive y)
```

#### Parameters

`x` [DrawPrimitive](Cube.Core.DrawPrimitive.md)

One primitive.

`y` [DrawPrimitive](Cube.Core.DrawPrimitive.md)

The other.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_DrawList_Points_Cube_Core_DrawPrimitive__"></a> Points\(in DrawPrimitive\)

A polygon's corners, in order around it (pixels); empty for other kinds.

```csharp
public ReadOnlySpan<Vector2> Points(in DrawPrimitive primitive)
```

#### Parameters

`primitive` [DrawPrimitive](Cube.Core.DrawPrimitive.md)

A primitive from [Items](Cube.Core.DrawList.md#Cube_Core_DrawList_Items).

#### Returns

 [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)\>

### <a id="Cube_Core_DrawList_SortBackToFront"></a> SortBackToFront\(\)

Sorts so the farthest primitive comes first; ties keep the order they were added in.

```csharp
public void SortBackToFront()
```

#### Remarks

Sorts on one 64-bit key per primitive ([SortKey](Cube.Core.DrawList.md#Cube_Core_DrawList_SortKey)), carrying the primitives along, rather than
comparing primitives through a delegate: with collision boxes a frame holds a few thousand primitives, and
comparing plain numbers is several times cheaper. The order is exactly [CompareBackToFront](Cube.Core.DrawList.md#Cube_Core_DrawList_CompareBackToFront)'s.

### <a id="Cube_Core_DrawList_SortKey_Cube_Core_DrawPrimitive__"></a> SortKey\(in DrawPrimitive\)

A number that orders primitives far first, then by [Sequence](Cube.Core.DrawPrimitive.md#Cube_Core_DrawPrimitive_Sequence): the depth's bits made
to sort like the float and inverted (so larger depths come first) in the high half, the sequence in the low.

```csharp
public static long SortKey(in DrawPrimitive primitive)
```

#### Parameters

`primitive` [DrawPrimitive](Cube.Core.DrawPrimitive.md)

The primitive.

#### Returns

 [long](https://learn.microsoft.com/dotnet/api/system.int64)
