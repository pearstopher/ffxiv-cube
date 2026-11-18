# <a id="Cube_Core_CollisionBoxSet"></a> Class CollisionBoxSet

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

The cached collision boxes around the cube (PROJECT.md, "Collision (phase 2)" and "Efficiency and memory"): the
game reader walks the client's collision scene and offers every box it finds; this class culls them to the area
around the cube, merges the cheap cases, holds at most a fixed number, and says when the list is stale.

```csharp
public sealed class CollisionBoxSet
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[CollisionBoxSet](Cube.Core.CollisionBoxSet.md)


## Remarks

<p>
<b>Gathered once, used for many frames.</b> The walk is cheap (0.13 ms in the #28 sample) but still not something
to repeat every frame, so the boxes are gathered for a [Region](Cube.Core.CollisionBoxSet.md#Cube_Core_CollisionBoxSet_Region) a little larger than the cube: wide
enough for any camera yaw (the cube's horizontal diagonal) plus a margin of [RefreshFraction](Cube.Core.CollisionBoxSet.md#Cube_Core_CollisionBoxSet_RefreshFraction) of the
edge. Turning the camera never invalidates it; the player can walk that margin before [NeedsRefresh](Cube.Core.CollisionBoxSet.md#Cube_Core_CollisionBoxSet_NeedsRefresh)
asks for a new walk (PROJECT.md: "When the player moves, stale data is refreshed"; "recompute only when inputs
change beyond a small threshold"). Each frame [CollisionClipper](Cube.Core.CollisionClipper.md#Cube_Core_CollisionClipper) clips the cached boxes to the cube.
</p>
<p>
<b>Bounded.</b> The buffer is allocated once at [Capacity](Cube.Core.CollisionBoxSet.md#Cube_Core_CollisionBoxSet_Capacity) boxes (2,000 × 24 bytes = 48 KB by
default, the #28 recommendation) and reused; a full set drops extras and counts them in [Dropped](Cube.Core.CollisionBoxSet.md#Cube_Core_CollisionBoxSet_Dropped),
like the player cap. [Clear](Cube.Core.CollisionBoxSet.md#Cube_Core_CollisionBoxSet_Clear) drops the data on a zone change (PROJECT.md: collision data is dropped
when you leave the area or change zone).
</p>
<p>
<b>Merging where cheap.</b> The reader offers the nodes of one mesh tree one after another, and neighbouring nodes
often line up. Each new box is compared with the last one kept only (constant time): if one holds the other, or
they share their extent on two axes and meet on the third, the two become one box. Anything smarter would cost a
search per box and isn't needed to stay under the cap.
</p>

## Constructors

### <a id="Cube_Core_CollisionBoxSet__ctor_System_Int32_"></a> CollisionBoxSet\(int\)

Creates an empty set.

```csharp
public CollisionBoxSet(int capacity = 2000)
```

#### Parameters

`capacity` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Most boxes kept.

## Fields

### <a id="Cube_Core_CollisionBoxSet_DefaultCapacity"></a> DefaultCapacity

Default most boxes kept (<code>docs/research-notes.md</code>, #28: 431 in a house interior; room for busier places).

```csharp
public const int DefaultCapacity = 2000
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_CollisionBoxSet_Diagonal"></a> Diagonal

The cube's horizontal half-diagonal per unit of half-edge (√2, rounded up so a corner is never missed): the
furthest a corner of the cube reaches sideways from the centre at any yaw.

```csharp
private const float Diagonal = 1.41422
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_CollisionBoxSet_MergeTolerance"></a> MergeTolerance

How close two box faces must be, in yalms, to count as lined up when merging.

```csharp
public const float MergeTolerance = 0.01
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_CollisionBoxSet_RefreshFraction"></a> RefreshFraction

How far the player may move, as a fraction of the cube's edge, before the boxes are gathered again. Also the
margin the gathered region has around the cube, so nothing at the cube's edge is missing until then.

```csharp
public const float RefreshFraction = 0.25
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_CollisionBoxSet_boxes"></a> boxes

The reused buffer of kept boxes.

```csharp
private readonly CollisionBox[] boxes
```

#### Field Value

 [CollisionBox](Cube.Core.CollisionBox.md)\[\]

### <a id="Cube_Core_CollisionBoxSet_centre"></a> centre

The cube centre the boxes were gathered around.

```csharp
private Vector3 centre
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_CollisionBoxSet_edge"></a> edge

The cube edge the boxes were gathered for, already clamped.

```csharp
private float edge
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_CollisionBoxSet_stale"></a> stale

Set by [MarkStale](Cube.Core.CollisionBoxSet.md#Cube_Core_CollisionBoxSet_MarkStale): the boxes are kept but [NeedsRefresh](Cube.Core.CollisionBoxSet.md#Cube_Core_CollisionBoxSet_NeedsRefresh) asks for a new gather.

```csharp
private bool stale
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_CollisionBoxSet_stamp"></a> stamp

The scene stamp the boxes were gathered for (see [Begin](Cube.Core.CollisionBoxSet.md#Cube_Core_CollisionBoxSet_Begin)).

```csharp
private ulong stamp
```

#### Field Value

 [ulong](https://learn.microsoft.com/dotnet/api/system.uint64)

## Properties

### <a id="Cube_Core_CollisionBoxSet_Capacity"></a> Capacity

Most boxes kept.

```csharp
public int Capacity { get; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_CollisionBoxSet_Count"></a> Count

How many boxes are kept.

```csharp
public int Count { get; private set; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_CollisionBoxSet_Dropped"></a> Dropped

How many boxes inside the region didn't fit under [Capacity](Cube.Core.CollisionBoxSet.md#Cube_Core_CollisionBoxSet_Capacity).

```csharp
public int Dropped { get; private set; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_CollisionBoxSet_HasData"></a> HasData

Whether boxes have been gathered since the set was created or last [Clear?text=cleared](Cube.Core.CollisionBoxSet.md#Cube_Core_CollisionBoxSet_Clear_text_cleared).

```csharp
public bool HasData { get; private set; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_CollisionBoxSet_Invalid"></a> Invalid

How many offered boxes were rejected as invalid (non-finite or inside out).

```csharp
public int Invalid { get; private set; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_CollisionBoxSet_Items"></a> Items

The kept boxes, unclipped, in world coordinates.

```csharp
public ReadOnlySpan<CollisionBox> Items { get; }
```

#### Property Value

 [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[CollisionBox](Cube.Core.CollisionBox.md)\>

### <a id="Cube_Core_CollisionBoxSet_Merged"></a> Merged

How many offered boxes were folded into the box before them instead of being kept on their own.

```csharp
public int Merged { get; private set; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_CollisionBoxSet_Offered"></a> Offered

How many boxes were offered in the last gather, inside the region or not.

```csharp
public int Offered { get; private set; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_CollisionBoxSet_Region"></a> Region

The world region boxes are gathered for: the cube at any yaw plus the refresh margin. The reader tests collider
and node boxes against it ([Overlaps](Cube.Core.CollisionBoxSet.md#Cube_Core_CollisionBoxSet_Overlaps)) to skip whole subtrees, the cull-first rule from #28.

```csharp
public CollisionBox Region { get; private set; }
```

#### Property Value

 [CollisionBox](Cube.Core.CollisionBox.md)

## Methods

### <a id="Cube_Core_CollisionBoxSet_Begin_System_Numerics_Vector3_System_Single_System_UInt64_"></a> Begin\(Vector3, float, ulong\)

Starts a new gather: empties the set and sets the region boxes are culled against.

```csharp
public void Begin(Vector3 playerPosition, float edge, ulong sceneStamp)
```

#### Parameters

`playerPosition` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The cube's centre.

`edge` [float](https://learn.microsoft.com/dotnet/api/system.single)

The cube's edge in yalms.

`sceneStamp` [ulong](https://learn.microsoft.com/dotnet/api/system.uint64)

The reader's scene stamp, remembered for [NeedsRefresh](Cube.Core.CollisionBoxSet.md#Cube_Core_CollisionBoxSet_NeedsRefresh).

### <a id="Cube_Core_CollisionBoxSet_Clear"></a> Clear\(\)

Drops every box (PROJECT.md, "Bounded memory": collision data is dropped when you leave the area or change
zone). The buffer is kept for reuse, so the next gather allocates nothing.

```csharp
public void Clear()
```

### <a id="Cube_Core_CollisionBoxSet_MarkStale"></a> MarkStale\(\)

Keeps the boxes but makes [NeedsRefresh](Cube.Core.CollisionBoxSet.md#Cube_Core_CollisionBoxSet_NeedsRefresh) true, so they stay drawn until a new gather replaces them
([Invalidate](Cube.Core.CollisionCache.md#Cube_Core_CollisionCache_Invalidate)).

```csharp
public void MarkStale()
```

### <a id="Cube_Core_CollisionBoxSet_Meets_System_Single_System_Single_System_Single_System_Single_"></a> Meets\(float, float, float, float\)

Whether two intervals overlap or touch, within [MergeTolerance](Cube.Core.CollisionBoxSet.md#Cube_Core_CollisionBoxSet_MergeTolerance).

```csharp
private static bool Meets(float aMin, float aMax, float bMin, float bMax)
```

#### Parameters

`aMin` [float](https://learn.microsoft.com/dotnet/api/system.single)

First interval's start.

`aMax` [float](https://learn.microsoft.com/dotnet/api/system.single)

First interval's end.

`bMin` [float](https://learn.microsoft.com/dotnet/api/system.single)

Second interval's start.

`bMax` [float](https://learn.microsoft.com/dotnet/api/system.single)

Second interval's end.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_CollisionBoxSet_NeedsRefresh_System_Numerics_Vector3_System_Single_System_UInt64_"></a> NeedsRefresh\(Vector3, float, ulong\)

Whether the boxes must be gathered again: there are none yet, the scene changed (a different
<code class="paramref">sceneStamp</code>), the cube's edge changed, or the player has moved more than
[RefreshFraction](Cube.Core.CollisionBoxSet.md#Cube_Core_CollisionBoxSet_RefreshFraction) of the edge along any axis since the last gather.

```csharp
public bool NeedsRefresh(Vector3 playerPosition, float edge, ulong sceneStamp)
```

#### Parameters

`playerPosition` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The cube's centre now.

`edge` [float](https://learn.microsoft.com/dotnet/api/system.single)

The cube's edge now.

`sceneStamp` [ulong](https://learn.microsoft.com/dotnet/api/system.uint64)

Whatever identifies the loaded collision to the reader (in the plugin, the first collider's address and the
collider count, as #28 suggests, with the detail level folded in by [Stamp](Cube.Core.CollisionDetails.md#Cube_Core_CollisionDetails_Stamp)). Core
only compares it; zone changes are handled separately by [CollisionCache](Cube.Core.CollisionCache.md#Cube_Core_CollisionCache).

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_CollisionBoxSet_Offer_Cube_Core_CollisionBox__"></a> Offer\(in CollisionBox\)

Offers one box. Invalid boxes and boxes outside the region are rejected; a box that lines up with the last one
kept is merged into it; otherwise it is kept, or counted as dropped if the set is full.

```csharp
public bool Offer(in CollisionBox box)
```

#### Parameters

`box` [CollisionBox](Cube.Core.CollisionBox.md)

A world box from the collision scene.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

True if the box is now covered by the set (kept or merged).

### <a id="Cube_Core_CollisionBoxSet_Overlaps_Cube_Core_CollisionBox__"></a> Overlaps\(in CollisionBox\)

Whether a box reaches into the gathered region. The reader calls it on collider and node boxes to skip
everything outside before reading further (PROJECT.md, "Efficiency": cull first).

```csharp
public bool Overlaps(in CollisionBox box)
```

#### Parameters

`box` [CollisionBox](Cube.Core.CollisionBox.md)

A world box.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_CollisionBoxSet_RegionFor_System_Numerics_Vector3_System_Single_"></a> RegionFor\(Vector3, float\)

The world region gathered for a cube, before any yaw: centre ± (half-diagonal + margin) sideways and
± (half-edge + margin) vertically.

```csharp
public static CollisionBox RegionFor(Vector3 centre, float edge)
```

#### Parameters

`centre` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The cube's centre (the player's position).

`edge` [float](https://learn.microsoft.com/dotnet/api/system.single)

The cube's edge in yalms; clamped like [CubeSpace](Cube.Core.CubeSpace.md#Cube_Core_CubeSpace).

#### Returns

 [CollisionBox](Cube.Core.CollisionBox.md)

### <a id="Cube_Core_CollisionBoxSet_Same_System_Single_System_Single_"></a> Same\(float, float\)

Whether two face positions are the same within [MergeTolerance](Cube.Core.CollisionBoxSet.md#Cube_Core_CollisionBoxSet_MergeTolerance).

```csharp
private static bool Same(float a, float b)
```

#### Parameters

`a` [float](https://learn.microsoft.com/dotnet/api/system.single)

One position.

`b` [float](https://learn.microsoft.com/dotnet/api/system.single)

The other.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_CollisionBoxSet_TryMerge_Cube_Core_CollisionBox__Cube_Core_CollisionBox__Cube_Core_CollisionBox__"></a> TryMerge\(in CollisionBox, in CollisionBox, out CollisionBox\)

The cheap merge: two boxes become one when one holds the other, or when they have the same extent on two axes
(within [MergeTolerance](Cube.Core.CollisionBoxSet.md#Cube_Core_CollisionBoxSet_MergeTolerance)) and overlap or touch on the third, so their union covers no extra space.

```csharp
public static bool TryMerge(in CollisionBox a, in CollisionBox b, out CollisionBox merged)
```

#### Parameters

`a` [CollisionBox](Cube.Core.CollisionBox.md)

The box kept last.

`b` [CollisionBox](Cube.Core.CollisionBox.md)

The new box.

`merged` [CollisionBox](Cube.Core.CollisionBox.md)

The single box covering both, if they merge.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

True if they merge.
