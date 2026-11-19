# <a id="Cube_Core_ITriangleSource"></a> Interface ITriangleSource

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

Where [TriangleCache](Cube.Core.TriangleCache.md#Cube_Core_TriangleCache) gets its triangles from: in the plugin, the game's collision scene
(<code>Cube.Game.MeshReader</code>); in the tests, a list of fake colliders. The Mesh-style twin of
[ICollisionSource](Cube.Core.ICollisionSource.md#Cube_Core_ICollisionSource), with the same rules: no game pointer is kept between frames, so each frame starts
with [Seek](Cube.Core.ITriangleSource.md#Cube_Core_ITriangleSource_Seek); and it is a struct generic argument so nothing is boxed.

```csharp
public interface ITriangleSource
```

## Properties

### <a id="Cube_Core_ITriangleSource_Now"></a> Now

The source's clock, in the ticks the cache's budget is given in.

```csharp
long Now { get; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

## Methods

### <a id="Cube_Core_ITriangleSource_OfferNext_Cube_Core_TriangleSet_"></a> OfferNext\(TriangleSet\)

Offers every triangle of the current collider that reaches the set's region, then moves to the next collider.

```csharp
bool OfferNext(TriangleSet set)
```

#### Parameters

`set` [TriangleSet](Cube.Core.TriangleSet.md)

The set being gathered.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

False if there was no current collider: the walk is complete.

### <a id="Cube_Core_ITriangleSource_Seek_System_Int32_"></a> Seek\(int\)

Moves to the collider at <code class="paramref">index</code> in the scene's list.

```csharp
bool Seek(int index)
```

#### Parameters

`index` [int](https://learn.microsoft.com/dotnet/api/system.int32)

How many colliders to skip from the start of the list.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

False if the list has fewer colliders than that.
