# <a id="Cube_Core_ICollisionSource"></a> Interface ICollisionSource

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

Where [CollisionCache](Cube.Core.CollisionCache.md#Cube_Core_CollisionCache) gets its boxes from: in the plugin, the game's collision scene
(<code>Cube.Game.CollisionReader</code>); in the tests, a list of fake colliders.

```csharp
public interface ICollisionSource
```

## Remarks

<p>
The cache works through the scene one collider at a time and may stop between any two of them when its frame's time
budget runs out (PROJECT.md, "Collision (phase 2)": expensive gathering is spread over frames with a time budget).
It only remembers the <em>index</em> of the next collider, never a game pointer, so nothing it holds between frames
can point at a collider the game has since unloaded: each frame starts again with [Seek](Cube.Core.ICollisionSource.md#Cube_Core_ICollisionSource_Seek).
</p>
<p>
Used as a generic constraint on a struct ([Step%60%601](Cube.Core.CollisionCache.md#Cube_Core_CollisionCache_Step_60_601)), so the calls are direct, not
interface calls on a boxed copy, and the per-frame path allocates nothing.
</p>

## Properties

### <a id="Cube_Core_ICollisionSource_Now"></a> Now

The source's clock, in the ticks the cache's budget is given in (Stopwatch ticks in the plugin).

```csharp
long Now { get; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

## Methods

### <a id="Cube_Core_ICollisionSource_OfferNext_Cube_Core_CollisionBoxSet_"></a> OfferNext\(CollisionBoxSet\)

Offers every box of the current collider that reaches the set's region ([Overlaps](Cube.Core.CollisionBoxSet.md#Cube_Core_CollisionBoxSet_Overlaps)
lets it skip whole colliders and subtrees first), then moves to the next collider.

```csharp
bool OfferNext(CollisionBoxSet set)
```

#### Parameters

`set` [CollisionBoxSet](Cube.Core.CollisionBoxSet.md)

The set being gathered.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

False if there was no current collider: the walk is complete.

### <a id="Cube_Core_ICollisionSource_Seek_System_Int32_"></a> Seek\(int\)

Moves to the collider at <code class="paramref">index</code> in the scene's list, ready for [OfferNext](Cube.Core.ICollisionSource.md#Cube_Core_ICollisionSource_OfferNext).

```csharp
bool Seek(int index)
```

#### Parameters

`index` [int](https://learn.microsoft.com/dotnet/api/system.int32)

How many colliders to skip from the start of the list.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

False if the list has fewer colliders than that.
