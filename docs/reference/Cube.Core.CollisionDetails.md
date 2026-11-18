# <a id="Cube_Core_CollisionDetails"></a> Class CollisionDetails

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

The rules behind each [CollisionDetail](Cube.Core.CollisionDetail.md#Cube_Core_CollisionDetail) level (#32): how deep the game reader walks a mesh's node tree,
and how a change of level makes the collision cache gather again straight away.

```csharp
public static class CollisionDetails
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[CollisionDetails](Cube.Core.CollisionDetails.md)


## Fields

### <a id="Cube_Core_CollisionDetails_MediumDepth"></a> MediumDepth

How many levels below the root [Medium](Cube.Core.CollisionDetail.md#Cube_Core_CollisionDetail_Medium) walks: 2³ = at most 8 boxes per mesh, so a
terrain tile or building is split into a few blocks while small objects (shallow trees) keep their full shape.

```csharp
public const int MediumDepth = 3
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Methods

### <a id="Cube_Core_CollisionDetails_MaxNodeDepth_Cube_Core_CollisionDetail_"></a> MaxNodeDepth\(CollisionDetail\)

The deepest tree level whose nodes are offered at a detail level; a node at this depth is offered as one box
covering its whole subtree, and its children are not visited. `System.Int32.MaxValue` means no limit.

```csharp
public static int MaxNodeDepth(CollisionDetail detail)
```

#### Parameters

`detail` [CollisionDetail](Cube.Core.CollisionDetail.md)

The detail level.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_CollisionDetails_Stamp_System_UInt64_Cube_Core_CollisionDetail_"></a> Stamp\(ulong, CollisionDetail\)

The scene stamp the cache compares ([NeedsRefresh](Cube.Core.CollisionBoxSet.md#Cube_Core_CollisionBoxSet_NeedsRefresh)) with the detail level folded
in. A new level is a different stamp, so the cache gathers again at once (restarting a gather in progress)
while the old boxes stay drawn until it finishes: the setting applies immediately (#32).

```csharp
public static ulong Stamp(ulong sceneStamp, CollisionDetail detail)
```

#### Parameters

`sceneStamp` [ulong](https://learn.microsoft.com/dotnet/api/system.uint64)

What identifies the loaded collision (collider list address and count).

`detail` [CollisionDetail](Cube.Core.CollisionDetail.md)

The detail level.

#### Returns

 [ulong](https://learn.microsoft.com/dotnet/api/system.uint64)
