# <a id="Cube_Tests_ClippedCollisionTests_ListSource"></a> Struct ClippedCollisionTests.ListSource

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

A fake collision scene: one box per collider, gathered in one frame.

```csharp
private struct ClippedCollisionTests.ListSource : ICollisionSource
```

#### Implements

[ICollisionSource](Cube.Core.ICollisionSource.md)


## Constructors

### <a id="Cube_Tests_ClippedCollisionTests_ListSource__ctor_Cube_Core_CollisionBox___"></a> ListSource\(CollisionBox\[\]\)

A fake collision scene: one box per collider, gathered in one frame.

```csharp
public ListSource(CollisionBox[] boxes)
```

#### Parameters

`boxes` [CollisionBox](Cube.Core.CollisionBox.md)\[\]

The scene's boxes, one per collider.

## Fields

### <a id="Cube_Tests_ClippedCollisionTests_ListSource_next"></a> next

```csharp
private int next
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Properties

### <a id="Cube_Tests_ClippedCollisionTests_ListSource_Now"></a> Now

The source's clock, in the ticks the cache's budget is given in (Stopwatch ticks in the plugin).

```csharp
public readonly long Now { get; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

## Methods

### <a id="Cube_Tests_ClippedCollisionTests_ListSource_OfferNext_Cube_Core_CollisionBoxSet_"></a> OfferNext\(CollisionBoxSet\)

Offers every box of the current collider that reaches the set's region ([Overlaps](Cube.Core.CollisionBoxSet.md#Cube_Core_CollisionBoxSet_Overlaps)
lets it skip whole colliders and subtrees first), then moves to the next collider.

```csharp
public bool OfferNext(CollisionBoxSet set)
```

#### Parameters

`set` [CollisionBoxSet](Cube.Core.CollisionBoxSet.md)

The set being gathered.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

False if there was no current collider: the walk is complete.

### <a id="Cube_Tests_ClippedCollisionTests_ListSource_Seek_System_Int32_"></a> Seek\(int\)

Moves to the collider at <code class="paramref">index</code> in the scene's list, ready for [OfferNext](Cube.Core.ICollisionSource.md#Cube_Core_ICollisionSource_OfferNext).

```csharp
public bool Seek(int index)
```

#### Parameters

`index` [int](https://learn.microsoft.com/dotnet/api/system.int32)

How many colliders to skip from the start of the list.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

False if the list has fewer colliders than that.
