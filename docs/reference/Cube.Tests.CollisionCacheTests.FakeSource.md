# <a id="Cube_Tests_CollisionCacheTests_FakeSource"></a> Struct CollisionCacheTests.FakeSource

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

A scene of fixed boxes, one per collider, whose clock advances a set amount per collider offered.

```csharp
private struct CollisionCacheTests.FakeSource : ICollisionSource
```

#### Implements

[ICollisionSource](Cube.Core.ICollisionSource.md)


## Constructors

### <a id="Cube_Tests_CollisionCacheTests_FakeSource__ctor_Cube_Core_CollisionBox___System_Int64_"></a> FakeSource\(CollisionBox\[\], long\)

```csharp
public FakeSource(CollisionBox[] colliders, long ticksPerCollider)
```

#### Parameters

`colliders` [CollisionBox](Cube.Core.CollisionBox.md)\[\]

`ticksPerCollider` [long](https://learn.microsoft.com/dotnet/api/system.int64)

## Fields

### <a id="Cube_Tests_CollisionCacheTests_FakeSource_colliders"></a> colliders

```csharp
private readonly CollisionBox[] colliders
```

#### Field Value

 [CollisionBox](Cube.Core.CollisionBox.md)\[\]

### <a id="Cube_Tests_CollisionCacheTests_FakeSource_current"></a> current

```csharp
private int current
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Tests_CollisionCacheTests_FakeSource_length"></a> length

```csharp
private int length
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Tests_CollisionCacheTests_FakeSource_ticksPerCollider"></a> ticksPerCollider

```csharp
private readonly long ticksPerCollider
```

#### Field Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

## Properties

### <a id="Cube_Tests_CollisionCacheTests_FakeSource_LastSeek"></a> LastSeek

```csharp
public int LastSeek { readonly get; private set; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Tests_CollisionCacheTests_FakeSource_Now"></a> Now

The source's clock, in the ticks the cache's budget is given in (Stopwatch ticks in the plugin).

```csharp
public long Now { readonly get; private set; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Cube_Tests_CollisionCacheTests_FakeSource_Seeks"></a> Seeks

```csharp
public int Seeks { readonly get; private set; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Methods

### <a id="Cube_Tests_CollisionCacheTests_FakeSource_OfferNext_Cube_Core_CollisionBoxSet_"></a> OfferNext\(CollisionBoxSet\)

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

### <a id="Cube_Tests_CollisionCacheTests_FakeSource_Seek_System_Int32_"></a> Seek\(int\)

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

### <a id="Cube_Tests_CollisionCacheTests_FakeSource_Truncate_System_Int32_"></a> Truncate\(int\)

```csharp
public void Truncate(int length)
```

#### Parameters

`length` [int](https://learn.microsoft.com/dotnet/api/system.int32)
