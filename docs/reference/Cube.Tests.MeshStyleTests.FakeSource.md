# <a id="Cube_Tests_MeshStyleTests_FakeSource"></a> Struct MeshStyleTests.FakeSource

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

A fake scene: a list of triangles (three corners each); one "collider" per triangle, costing a set number of ticks.

```csharp
private struct MeshStyleTests.FakeSource : ITriangleSource
```

#### Implements

[ITriangleSource](Cube.Core.ITriangleSource.md)


## Constructors

### <a id="Cube_Tests_MeshStyleTests_FakeSource__ctor_System_Numerics_Vector3___System_Int64_"></a> FakeSource\(Vector3\[\], long\)

A fake scene: a list of triangles (three corners each); one "collider" per triangle, costing a set number of ticks.

```csharp
public FakeSource(Vector3[] corners, long ticksPerCollider = 1)
```

#### Parameters

`corners` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)\[\]

The triangles' corners, three per triangle.

`ticksPerCollider` [long](https://learn.microsoft.com/dotnet/api/system.int64)

What offering one triangle costs on the fake clock.

## Fields

### <a id="Cube_Tests_MeshStyleTests_FakeSource_clock"></a> clock

```csharp
private long clock
```

#### Field Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Cube_Tests_MeshStyleTests_FakeSource_index"></a> index

```csharp
private int index
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Properties

### <a id="Cube_Tests_MeshStyleTests_FakeSource_Now"></a> Now

The source's clock, in the ticks the cache's budget is given in.

```csharp
public readonly long Now { get; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

## Methods

### <a id="Cube_Tests_MeshStyleTests_FakeSource_OfferNext_Cube_Core_TriangleSet_"></a> OfferNext\(TriangleSet\)

Offers every triangle of the current collider that reaches the set's region, then moves to the next collider.

```csharp
public bool OfferNext(TriangleSet set)
```

#### Parameters

`set` [TriangleSet](Cube.Core.TriangleSet.md)

The set being gathered.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

False if there was no current collider: the walk is complete.

### <a id="Cube_Tests_MeshStyleTests_FakeSource_Seek_System_Int32_"></a> Seek\(int\)

Moves to the collider at <code class="paramref">index</code> in the scene's list.

```csharp
public bool Seek(int at)
```

#### Parameters

`at` [int](https://learn.microsoft.com/dotnet/api/system.int32)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

False if the list has fewer colliders than that.
