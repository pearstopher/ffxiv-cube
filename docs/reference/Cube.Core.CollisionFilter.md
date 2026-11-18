# <a id="Cube_Core_CollisionFilter"></a> Class CollisionFilter

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

Decides which of the game's colliders are drawn (PROJECT.md, "Phase 2": collision means "what the player would
bump into, walk on, or be stopped by"). Colliders that exist only for something else are dropped, because drawing
them shows walls the player can walk through (#107).

```csharp
public static class CollisionFilter
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[CollisionFilter](Cube.Core.CollisionFilter.md)


## Remarks

<p>
Evidence (<code>docs/research-notes.md</code>, "Phantom boxes (#107)"): the game's own movement raycast was cast through
every collider box within 150 yalms. Every mesh collider kind it was ever asked about was hit (materials 0 and
0x7000), but among primitive colliders only those with material bit 0x4000 (<code>4000</code>, <code>4005</code>, <code>6004</code>,
<code>6406</code>) were ever hit; the roughly 200 primitives with only bit 0x2000 (<code>2004</code>, <code>2005</code>, <code>2006</code>,
<code>200A</code>, <code>3005</code> and so on) were never hit by any ray. Those are the invisible trigger and event volumes
the player walks through.
</p>
<p>
Mesh node boxes that are larger than their triangles are a separate matter (box coarseness, not filtering) and are
left to the real-triangle work in #100.
</p>

## Fields

### <a id="Cube_Core_CollisionFilter_BlockingMaterial"></a> BlockingMaterial

The bit of [ObjectMaterialValue](Cube.Core.ColliderFields.md#Cube_Core_ColliderFields_ObjectMaterialValue) that a primitive collider needs to block.

```csharp
public const ulong BlockingMaterial = 16384
```

#### Field Value

 [ulong](https://learn.microsoft.com/dotnet/api/system.uint64)

### <a id="Cube_Core_CollisionFilter_MovementLayer"></a> MovementLayer

The bit of [LayerMask](Cube.Core.ColliderFields.md#Cube_Core_ColliderFields_LayerMask) for the layer movement queries use.

```csharp
public const ulong MovementLayer = 1
```

#### Field Value

 [ulong](https://learn.microsoft.com/dotnet/api/system.uint64)

### <a id="Cube_Core_CollisionFilter_RaycastActive"></a> RaycastActive

The bit of [VisibilityFlags](Cube.Core.ColliderFields.md#Cube_Core_ColliderFields_VisibilityFlags) marking colliders the game's raycasts use.

```csharp
public const byte RaycastActive = 1
```

#### Field Value

 [byte](https://learn.microsoft.com/dotnet/api/system.byte)

## Methods

### <a id="Cube_Core_CollisionFilter_Blocks_Cube_Core_ColliderFields_"></a> Blocks\(ColliderFields\)

Whether a collider is something the player could bump into: active for raycasts, on the movement layer, and,
if it is a primitive, carrying the blocking material bit.

```csharp
public static bool Blocks(ColliderFields fields)
```

#### Parameters

`fields` [ColliderFields](Cube.Core.ColliderFields.md)

The collider's fields.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)
