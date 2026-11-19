# <a id="Cube_Core_TriangleCache"></a> Class TriangleCache

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

Gathers the collision triangles around the cube a few colliders per frame within a time budget, and keeps the last
complete set for clipping and drawing (#100; PROJECT.md, "Collision style" and "Efficiency and memory"): the
Mesh-style counterpart of [CollisionCache](Cube.Core.CollisionCache.md#Cube_Core_CollisionCache).

```csharp
public sealed class TriangleCache
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[TriangleCache](Cube.Core.TriangleCache.md)


## Remarks

<p>
Two sets are swapped like the boxes': a gather fills one while [Ready](Cube.Core.TriangleCache.md#Cube_Core_TriangleCache_Ready) keeps the last complete one, so a
half-gathered area never shows as empty. The buffers (4.7 MB per set) are allocated on first use and dropped by
[Release](Cube.Core.TriangleCache.md#Cube_Core_TriangleCache_Release) when the user goes back to Boxes, so memory is only spent while Mesh is in use. A gather in
progress restarts if the scene changes under it or the player walks out of its region; the zone change drops both
sets (PROJECT.md, "Bounded memory").
</p>
<p>
A full #99 rebuild read 96k triangles in about 8.5 ms, so the walk stops between colliders when the frame's budget is
spent (always after at least one collider, so a gather always finishes).
</p>

## Constructors

### <a id="Cube_Core_TriangleCache__ctor_System_Int64_System_Int32_"></a> TriangleCache\(long, int\)

Creates an empty cache; no buffer is allocated until the first [Step%60%601](Cube.Core.TriangleCache.md#Cube_Core_TriangleCache_Step_60_601).

```csharp
public TriangleCache(long budgetTicks, int capacity = 131072)
```

#### Parameters

`budgetTicks` [long](https://learn.microsoft.com/dotnet/api/system.int64)

The longest a frame may spend gathering, in the source's clock ticks.

`capacity` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Most triangles each set keeps.

## Fields

### <a id="Cube_Core_TriangleCache_DefaultBudgetMilliseconds"></a> DefaultBudgetMilliseconds

Default time a frame may spend gathering, in milliseconds: a bit more than the boxes' because reading triangles
is the bulk of Mesh mode's work and it is an opt-in, slower mode. The rest of the gather waits for the next frame.

```csharp
public const double DefaultBudgetMilliseconds = 1
```

#### Field Value

 [double](https://learn.microsoft.com/dotnet/api/system.double)

### <a id="Cube_Core_TriangleCache_budgetTicks"></a> budgetTicks

The longest a frame's gather may run, in the source's clock ticks.

```csharp
private readonly long budgetTicks
```

#### Field Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Cube_Core_TriangleCache_building"></a> building

The set being filled; null until first use and after [Release](Cube.Core.TriangleCache.md#Cube_Core_TriangleCache_Release).

```csharp
private TriangleSet? building
```

#### Field Value

 [TriangleSet](Cube.Core.TriangleSet.md)?

### <a id="Cube_Core_TriangleCache_capacity"></a> capacity

Most triangles each set keeps.

```csharp
private readonly int capacity
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_TriangleCache_hasZone"></a> hasZone

Whether [zone](Cube.Core.TriangleCache.md#Cube_Core_TriangleCache_zone) has been set.

```csharp
private bool hasZone
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_TriangleCache_nextCollider"></a> nextCollider

The index in the scene's collider list the next frame's gather starts at.

```csharp
private int nextCollider
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_TriangleCache_zone"></a> zone

The zone the sets belong to.

```csharp
private uint zone
```

#### Field Value

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

## Properties

### <a id="Cube_Core_TriangleCache_Completed"></a> Completed

How many gathers have completed.

```csharp
public long Completed { get; private set; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Cube_Core_TriangleCache_IsAllocated"></a> IsAllocated

Whether the buffers are allocated.

```csharp
public bool IsAllocated { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_TriangleCache_IsGathering"></a> IsGathering

Whether a gather is in progress.

```csharp
public bool IsGathering { get; private set; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_TriangleCache_LargestDropped"></a> LargestDropped

The most triangles any completed gather found that didn't fit, so the plugin can say a scene was cut off.

```csharp
public int LargestDropped { get; private set; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_TriangleCache_Ready"></a> Ready

The last complete gather; null until the first [Step%60%601](Cube.Core.TriangleCache.md#Cube_Core_TriangleCache_Step_60_601) allocates it.

```csharp
public TriangleSet? Ready { get; private set; }
```

#### Property Value

 [TriangleSet](Cube.Core.TriangleSet.md)?

### <a id="Cube_Core_TriangleCache_Restarts"></a> Restarts

How many gathers were abandoned and started again.

```csharp
public long Restarts { get; private set; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Cube_Core_TriangleCache_ZoneChanges"></a> ZoneChanges

How many times the zone changed after the first step.

```csharp
public long ZoneChanges { get; private set; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

## Methods

### <a id="Cube_Core_TriangleCache_Clear"></a> Clear\(\)

Drops both sets' data and any gather in progress; the buffers are kept.

```csharp
public void Clear()
```

### <a id="Cube_Core_TriangleCache_Finish"></a> Finish\(\)

Completes the gather: the filled set becomes [Ready](Cube.Core.TriangleCache.md#Cube_Core_TriangleCache_Ready) and the old one is reused.

```csharp
private bool Finish()
```

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Always true.

### <a id="Cube_Core_TriangleCache_Invalidate"></a> Invalidate\(\)

Marks the ready set stale so the next step gathers again while the old triangles stay drawn.

```csharp
public void Invalidate()
```

### <a id="Cube_Core_TriangleCache_Release"></a> Release\(\)

Drops the buffers too, so Boxes mode holds no Mesh memory.

```csharp
public void Release()
```

### <a id="Cube_Core_TriangleCache_Start_System_Numerics_Vector3_System_Numerics_Vector3_System_UInt64_"></a> Start\(Vector3, Vector3, ulong\)

Begins a new gather at the start of the scene's list.

```csharp
private void Start(Vector3 centre, Vector3 size, ulong sceneStamp)
```

#### Parameters

`centre` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The cube's centre.

`size` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The cube's size.

`sceneStamp` [ulong](https://learn.microsoft.com/dotnet/api/system.uint64)

The scene stamp.

### <a id="Cube_Core_TriangleCache_Step__1___0__System_Numerics_Vector3_System_Numerics_Vector3_System_UInt32_System_UInt64_"></a> Step<TSource\>\(ref TSource, Vector3, Vector3, uint, ulong\)

One frame's work: drops everything on a zone change, starts a gather if the ready set is stale, and continues
the gather in progress until the budget is spent or the scene's list ends.

```csharp
public bool Step<TSource>(ref TSource source, Vector3 centre, Vector3 size, uint zone, ulong sceneStamp) where TSource : struct, ITriangleSource
```

#### Parameters

`source` TSource

The source; [Seek](Cube.Core.ITriangleSource.md#Cube_Core_ITriangleSource_Seek) is called first.

`centre` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The cube's centre.

`size` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The cube's width, height and depth in yalms.

`zone` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

The current zone id.

`sceneStamp` [ulong](https://learn.microsoft.com/dotnet/api/system.uint64)

The source's scene stamp.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

True if a gather completed this frame, so [Ready](Cube.Core.TriangleCache.md#Cube_Core_TriangleCache_Ready) changed.

#### Type Parameters

`TSource` 

The triangle source; a struct, so the calls aren't boxed.
