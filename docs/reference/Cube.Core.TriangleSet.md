# <a id="Cube_Core_TriangleSet"></a> Class TriangleSet

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

The collision triangles around the cube in world coordinates (#100; PROJECT.md, "Collision style"): the Mesh-style
counterpart of [CollisionBoxSet](Cube.Core.CollisionBoxSet.md#Cube_Core_CollisionBoxSet). The game reader offers every triangle it reads; this class keeps the
ones that could be inside the cube at any yaw, up to a fixed capacity, and says when the set is stale.

```csharp
public sealed class TriangleSet
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[TriangleSet](Cube.Core.TriangleSet.md)


## Remarks

<p>
<b>Same region, same refresh rule as the boxes.</b> It borrows [CollisionBoxSet](Cube.Core.CollisionBoxSet.md#Cube_Core_CollisionBoxSet)'s region (the cube at
any yaw plus a margin) and staleness rule through a one-box set, so turning the camera never invalidates the
triangles and the player can walk the margin before a new gather. A triangle wholly outside the region can't touch
the cube, so rejecting it changes nothing that is drawn (PROJECT.md: only efficiencies with no effect on the result).
</p>
<p>
<b>Bounded, and honest about it.</b> The buffer is allocated once. A triangle that doesn't fit is counted in
[Dropped](Cube.Core.TriangleSet.md#Cube_Core_TriangleSet_Dropped); the plugin logs it and the bridge reports it, so a scene bigger than the buffer is never cut
off silently.
</p>

## Constructors

### <a id="Cube_Core_TriangleSet__ctor_System_Int32_"></a> TriangleSet\(int\)

Creates an empty set.

```csharp
public TriangleSet(int capacity = 131072)
```

#### Parameters

`capacity` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Most triangles kept.

## Fields

### <a id="Cube_Core_TriangleSet_DefaultCapacity"></a> DefaultCapacity

Default most triangles kept. The #99 samples held 15,038 and 95,786 triangles in the nodes crossing a 40 yalm
cube; this leaves room for a bigger cube. 131,072 triangles × 3 corners × 12 bytes is 4.7 MB per set.

```csharp
public const int DefaultCapacity = 131072
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_TriangleSet_corners"></a> corners

The corners, three per triangle.

```csharp
private readonly Vector3[] corners
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)\[\]

### <a id="Cube_Core_TriangleSet_frame"></a> frame

Holds the region and the refresh rule, so they are the boxes' own.

```csharp
private readonly CollisionBoxSet frame
```

#### Field Value

 [CollisionBoxSet](Cube.Core.CollisionBoxSet.md)

## Properties

### <a id="Cube_Core_TriangleSet_Capacity"></a> Capacity

Most triangles kept.

```csharp
public int Capacity { get; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_TriangleSet_Corners"></a> Corners

The kept triangles' corners, three per triangle, in world coordinates.

```csharp
public ReadOnlySpan<Vector3> Corners { get; }
```

#### Property Value

 [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)\>

### <a id="Cube_Core_TriangleSet_Count"></a> Count

How many triangles are kept.

```csharp
public int Count { get; private set; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_TriangleSet_Dropped"></a> Dropped

How many triangles inside the region didn't fit under [Capacity](Cube.Core.TriangleSet.md#Cube_Core_TriangleSet_Capacity).

```csharp
public int Dropped { get; private set; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_TriangleSet_HasData"></a> HasData

Whether a gather has started since the set was created or last [Clear?text=cleared](Cube.Core.TriangleSet.md#Cube_Core_TriangleSet_Clear_text_cleared).

```csharp
public bool HasData { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_TriangleSet_Invalid"></a> Invalid

How many offered triangles were rejected for a non-finite corner.

```csharp
public int Invalid { get; private set; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_TriangleSet_Offered"></a> Offered

How many triangles were offered in the last gather, inside the region or not.

```csharp
public int Offered { get; private set; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_TriangleSet_Region"></a> Region

The world region triangles are gathered for ([Region](Cube.Core.CollisionBoxSet.md#Cube_Core_CollisionBoxSet_Region)).

```csharp
public CollisionBox Region { get; }
```

#### Property Value

 [CollisionBox](Cube.Core.CollisionBox.md)

## Methods

### <a id="Cube_Core_TriangleSet_Begin_System_Numerics_Vector3_System_Numerics_Vector3_System_UInt64_"></a> Begin\(Vector3, Vector3, ulong\)

Starts a new gather: empties the set and sets the region triangles are culled against.

```csharp
public void Begin(Vector3 centre, Vector3 size, ulong sceneStamp)
```

#### Parameters

`centre` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The cube's centre.

`size` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The cube's width, height and depth.

`sceneStamp` [ulong](https://learn.microsoft.com/dotnet/api/system.uint64)

The reader's scene stamp.

### <a id="Cube_Core_TriangleSet_Clear"></a> Clear\(\)

Drops every triangle (PROJECT.md: collision data is dropped on a zone change); the buffer is kept.

```csharp
public void Clear()
```

### <a id="Cube_Core_TriangleSet_MarkStale"></a> MarkStale\(\)

Keeps the triangles but makes [NeedsRefresh](Cube.Core.TriangleSet.md#Cube_Core_TriangleSet_NeedsRefresh) true until a new gather replaces them.

```csharp
public void MarkStale()
```

### <a id="Cube_Core_TriangleSet_NeedsRefresh_System_Numerics_Vector3_System_Numerics_Vector3_System_UInt64_"></a> NeedsRefresh\(Vector3, Vector3, ulong\)

Whether the triangles must be gathered again (see [NeedsRefresh](Cube.Core.CollisionBoxSet.md#Cube_Core_CollisionBoxSet_NeedsRefresh)).

```csharp
public bool NeedsRefresh(Vector3 centre, Vector3 size, ulong sceneStamp)
```

#### Parameters

`centre` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The cube's centre now.

`size` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The cube's width, height and depth now.

`sceneStamp` [ulong](https://learn.microsoft.com/dotnet/api/system.uint64)

What identifies the loaded collision to the reader.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_TriangleSet_Offer_System_Numerics_Vector3_System_Numerics_Vector3_System_Numerics_Vector3_"></a> Offer\(Vector3, Vector3, Vector3\)

Offers one triangle. One with a non-finite corner or wholly outside the region is rejected; otherwise it is
kept, or counted as dropped when the set is full.

```csharp
public bool Offer(Vector3 a, Vector3 b, Vector3 c)
```

#### Parameters

`a` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

First corner, in world coordinates.

`b` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

Second corner.

`c` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

Third corner.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

True if the triangle was kept.

### <a id="Cube_Core_TriangleSet_Overlaps_Cube_Core_CollisionBox__"></a> Overlaps\(in CollisionBox\)

Whether a world box reaches into the gathered region; the reader calls it on colliders and nodes to skip
everything outside before reading their triangles (cull first).

```csharp
public bool Overlaps(in CollisionBox box)
```

#### Parameters

`box` [CollisionBox](Cube.Core.CollisionBox.md)

A world box.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)
