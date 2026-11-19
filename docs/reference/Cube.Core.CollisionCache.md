# <a id="Cube_Core_CollisionCache"></a> Class CollisionCache

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

Gathers the collision boxes around the cube a few colliders per frame, within a time budget, and keeps the last
complete set for drawing (PROJECT.md, "Collision (phase 2)" and "Efficiency and memory").

```csharp
public sealed class CollisionCache
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[CollisionCache](Cube.Core.CollisionCache.md)


## Remarks

<p>
<b>Two sets, swapped.</b> A gather fills [building](Cube.Core.CollisionCache.md#Cube_Core_CollisionCache_building) while [Ready](Cube.Core.CollisionCache.md#Cube_Core_CollisionCache_Ready) keeps the last complete
one, so the cube never shows a half-gathered area as if it were empty; when the walk reaches the end of the scene's
list the two swap. Both are allocated once at the same capacity, so memory is fixed (2 × 64 KB by default) and
nothing grows with time in game.
</p>
<p>
<b>Spread over frames.</b> Each [Step%60%601](Cube.Core.CollisionCache.md#Cube_Core_CollisionCache_Step_60_601) offers colliders until the frame's budget is used
(always at least one, so a gather always finishes), then stops and remembers only the index of the next collider.
The #28 sample walked a whole house interior in 0.13 ms, so a gather usually completes in one frame; the budget is
there for busier places (PROJECT.md: "any expensive gathering is spread over frames with a time budget").
</p>
<p>
<b>When to gather.</b> Only when [NeedsRefresh](Cube.Core.CollisionBoxSet.md#Cube_Core_CollisionBoxSet_NeedsRefresh) says the ready set is stale (player
moved a quarter of the box's extent, size or scene changed). A gather in progress restarts if the scene changes under it or
the player has already walked out of its region, so a finished set always matches one scene and one place.
</p>
<p>
<b>Zone change.</b> Both sets are dropped as soon as the zone differs (PROJECT.md, "Bounded memory": collision data
is dropped when you change zone), so the new zone shows nothing rather than the old zone's walls until its first
gather completes ("areas not yet processed are simply empty").
</p>

## Constructors

### <a id="Cube_Core_CollisionCache__ctor_System_Int64_System_Int32_"></a> CollisionCache\(long, int\)

Creates an empty cache.

```csharp
public CollisionCache(long budgetTicks, int capacity = 2000)
```

#### Parameters

`budgetTicks` [long](https://learn.microsoft.com/dotnet/api/system.int64)

The longest a frame may spend gathering, in the source's clock ticks.

`capacity` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Most boxes each set keeps ([DefaultCapacity](Cube.Core.CollisionBoxSet.md#Cube_Core_CollisionBoxSet_DefaultCapacity)).

## Fields

### <a id="Cube_Core_CollisionCache_DefaultBudgetMilliseconds"></a> DefaultBudgetMilliseconds

Default time a frame may spend gathering, in milliseconds. A whole house interior took 0.13 ms in #28, so most
gathers finish in one frame; outdoors and in cities they spread over a few. Kept well under the 0.5 ms draw
budget so a gathering frame stays smooth.

```csharp
public const double DefaultBudgetMilliseconds = 0.2
```

#### Field Value

 [double](https://learn.microsoft.com/dotnet/api/system.double)

### <a id="Cube_Core_CollisionCache_budgetTicks"></a> budgetTicks

The longest a frame's gather may run, in the source's clock ticks.

```csharp
private readonly long budgetTicks
```

#### Field Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Cube_Core_CollisionCache_building"></a> building

The set being filled by the current gather; swapped with [Ready](Cube.Core.CollisionCache.md#Cube_Core_CollisionCache_Ready) when it completes.

```csharp
private CollisionBoxSet building
```

#### Field Value

 [CollisionBoxSet](Cube.Core.CollisionBoxSet.md)

### <a id="Cube_Core_CollisionCache_hasZone"></a> hasZone

Whether [zone](Cube.Core.CollisionCache.md#Cube_Core_CollisionCache_zone) has been set by a first [Step%60%601](Cube.Core.CollisionCache.md#Cube_Core_CollisionCache_Step_60_601).

```csharp
private bool hasZone
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_CollisionCache_nextCollider"></a> nextCollider

The index in the scene's collider list the next frame's gather starts at.

```csharp
private int nextCollider
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_CollisionCache_zone"></a> zone

The zone the sets belong to; a different one drops them.

```csharp
private uint zone
```

#### Field Value

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

## Properties

### <a id="Cube_Core_CollisionCache_Completed"></a> Completed

How many gathers have completed since the cache was created; checks compare it before and after.

```csharp
public long Completed { get; private set; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Cube_Core_CollisionCache_GatherFrames"></a> GatherFrames

Frames the current or last gather has taken so far.

```csharp
public int GatherFrames { get; private set; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_CollisionCache_IsGathering"></a> IsGathering

Whether a gather is in progress (started and not yet at the end of the scene's list).

```csharp
public bool IsGathering { get; private set; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_CollisionCache_LastStepColliders"></a> LastStepColliders

Colliders the last frame offered.

```csharp
public int LastStepColliders { get; private set; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_CollisionCache_Ready"></a> Ready

The last complete gather, to be clipped and drawn. Empty ([HasData](Cube.Core.CollisionBoxSet.md#Cube_Core_CollisionBoxSet_HasData) false) until
the first gather in a zone completes.

```csharp
public CollisionBoxSet Ready { get; private set; }
```

#### Property Value

 [CollisionBoxSet](Cube.Core.CollisionBoxSet.md)

### <a id="Cube_Core_CollisionCache_Restarts"></a> Restarts

How many gathers were abandoned and started again because the scene or the player moved on.

```csharp
public long Restarts { get; private set; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Cube_Core_CollisionCache_ZoneChanges"></a> ZoneChanges

How many times the zone changed after the first [Step%60%601](Cube.Core.CollisionCache.md#Cube_Core_CollisionCache_Step_60_601), each dropping both sets. The test
bridge reports it to show memory stays bounded across zone changes (#33).

```csharp
public long ZoneChanges { get; private set; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

## Methods

### <a id="Cube_Core_CollisionCache_Clear"></a> Clear\(\)

Drops both sets and any gather in progress (PROJECT.md: collision data is dropped on a zone change). The buffers
are kept for reuse.

```csharp
public void Clear()
```

### <a id="Cube_Core_CollisionCache_Finish"></a> Finish\(\)

Completes the gather: the filled set becomes [Ready](Cube.Core.CollisionCache.md#Cube_Core_CollisionCache_Ready) and the old one is reused next time.

```csharp
private bool Finish()
```

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Always true (a gather completed), for [Step%60%601](Cube.Core.CollisionCache.md#Cube_Core_CollisionCache_Step_60_601) to return.

### <a id="Cube_Core_CollisionCache_Invalidate"></a> Invalidate\(\)

Marks the ready set as stale without dropping it, so the next [Step%60%601](Cube.Core.CollisionCache.md#Cube_Core_CollisionCache_Step_60_601) gathers again while
the old boxes stay drawn. The test bridge uses it to measure gathers on demand.

```csharp
public void Invalidate()
```

### <a id="Cube_Core_CollisionCache_Start_System_Numerics_Vector3_System_Numerics_Vector3_System_UInt64_"></a> Start\(Vector3, Vector3, ulong\)

Begins a new gather into [building](Cube.Core.CollisionCache.md#Cube_Core_CollisionCache_building) at the start of the scene's list.

```csharp
private void Start(Vector3 position, Vector3 size, ulong sceneStamp)
```

#### Parameters

`position` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The cube's centre.

`size` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The cube's width, height and depth.

`sceneStamp` [ulong](https://learn.microsoft.com/dotnet/api/system.uint64)

The scene stamp.

### <a id="Cube_Core_CollisionCache_Step__1___0__System_Numerics_Vector3_System_Numerics_Vector3_System_UInt32_System_UInt64_"></a> Step<TSource\>\(ref TSource, Vector3, Vector3, uint, ulong\)

One frame's work: drops everything on a zone change, starts a gather if the ready set is stale, and continues the
gather in progress until the budget is spent or the scene's list ends.

```csharp
public bool Step<TSource>(ref TSource source, Vector3 position, Vector3 size, uint zone, ulong sceneStamp) where TSource : struct, ICollisionSource
```

#### Parameters

`source` TSource

The source, positioned anywhere; [Seek](Cube.Core.ICollisionSource.md#Cube_Core_ICollisionSource_Seek) is called first.

`position` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The cube's centre: the player, shifted vertically by the player height setting (#91).

`size` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The cube's width, height and depth in yalms.

`zone` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

The current zone (territory) id.

`sceneStamp` [ulong](https://learn.microsoft.com/dotnet/api/system.uint64)

The source's scene stamp (see [NeedsRefresh](Cube.Core.CollisionBoxSet.md#Cube_Core_CollisionBoxSet_NeedsRefresh)).

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

True if a gather completed this frame, so [Ready](Cube.Core.CollisionCache.md#Cube_Core_CollisionCache_Ready) changed.

#### Type Parameters

`TSource` 

The collision source; a struct, so the calls aren't boxed.
