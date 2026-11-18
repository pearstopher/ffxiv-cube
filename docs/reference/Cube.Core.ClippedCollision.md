# <a id="Cube_Core_ClippedCollision"></a> Class ClippedCollision

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

The cached collision boxes clipped to this frame's cube, ready for [CollisionCubes](Cube.Core.CollisionCubes.md#Cube_Core_CollisionCubes) (PROJECT.md,
"Collision (phase 2)": boxes clipped to the cube). Clips again only when the cube or the gathered boxes change.

```csharp
public sealed class ClippedCollision
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ClippedCollision](Cube.Core.ClippedCollision.md)


## Remarks

PROJECT.md, "Efficiency and memory": recompute only when the inputs change. The inputs are the cube (centre, edge,
alignment yaw) and the cache's ready set (which set it is, whether it holds data, and how many gathers have
completed); standing still with the camera still clips nothing. The buffer is allocated once at the cache's
capacity, so clipping can never drop a box and memory never grows.

## Constructors

### <a id="Cube_Core_ClippedCollision__ctor_System_Int32_"></a> ClippedCollision\(int\)

Creates an empty list.

```csharp
public ClippedCollision(int capacity = 2000)
```

#### Parameters

`capacity` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Most boxes; the cache's set capacity, so nothing is ever cut off.

## Fields

### <a id="Cube_Core_ClippedCollision_boxes"></a> boxes

The reused buffer of clipped boxes.

```csharp
private readonly CollisionBox[] boxes
```

#### Field Value

 [CollisionBox](Cube.Core.CollisionBox.md)\[\]

### <a id="Cube_Core_ClippedCollision_centre"></a> centre

The cube's centre when clipped.

```csharp
private Vector3 centre
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_ClippedCollision_completed"></a> completed

The cache's completed-gather count when clipped.

```csharp
private long completed
```

#### Field Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Cube_Core_ClippedCollision_edge"></a> edge

The cube's edge when clipped.

```csharp
private float edge
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_ClippedCollision_hadData"></a> hadData

Whether that set held data when clipped.

```csharp
private bool hadData
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_ClippedCollision_set"></a> set

The ready set the boxes were clipped from.

```csharp
private CollisionBoxSet? set
```

#### Field Value

 [CollisionBoxSet](Cube.Core.CollisionBoxSet.md)?

### <a id="Cube_Core_ClippedCollision_yaw"></a> yaw

The cube's yaw when clipped.

```csharp
private float yaw
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

## Properties

### <a id="Cube_Core_ClippedCollision_Clips"></a> Clips

How many times the boxes were clipped again; the bridge and tests check that unchanged inputs don't.

```csharp
public long Clips { get; private set; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Cube_Core_ClippedCollision_Count"></a> Count

How many boxes reach into the cube.

```csharp
public int Count { get; private set; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_ClippedCollision_Items"></a> Items

The boxes inside the cube, clipped to it, in world coordinates.

```csharp
public ReadOnlySpan<CollisionBox> Items { get; }
```

#### Property Value

 [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[CollisionBox](Cube.Core.CollisionBox.md)\>

## Methods

### <a id="Cube_Core_ClippedCollision_Clear"></a> Clear\(\)

Empties the list, so the next [Update](Cube.Core.ClippedCollision.md#Cube_Core_ClippedCollision_Update) clips again.

```csharp
public void Clear()
```

### <a id="Cube_Core_ClippedCollision_Update_Cube_Core_CubeSpace__Cube_Core_CollisionCache_"></a> Update\(in CubeSpace, CollisionCache\)

Clips the cache's ready boxes to the cube if either changed since the last call.

```csharp
public bool Update(in CubeSpace cube, CollisionCache cache)
```

#### Parameters

`cube` [CubeSpace](Cube.Core.CubeSpace.md)

This frame's cube.

`cache` [CollisionCache](Cube.Core.CollisionCache.md)

The collision cache.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

True if the boxes were clipped again.
