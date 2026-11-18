# <a id="Cube_Core_ColliderFields"></a> Struct ColliderFields

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

The fields of a game collider that decide whether the player can bump into it, copied out of the game's struct so
the decision ([Blocks](Cube.Core.CollisionFilter.md#Cube_Core_CollisionFilter_Blocks)) is a pure function that can be unit tested.

```csharp
public readonly record struct ColliderFields : IEquatable<ColliderFields>
```

#### Implements

[IEquatable<ColliderFields\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)


## Constructors

### <a id="Cube_Core_ColliderFields__ctor_System_Byte_System_UInt64_System_UInt64_System_Boolean_"></a> ColliderFields\(byte, ulong, ulong, bool\)

The fields of a game collider that decide whether the player can bump into it, copied out of the game's struct so
the decision ([Blocks](Cube.Core.CollisionFilter.md#Cube_Core_CollisionFilter_Blocks)) is a pure function that can be unit tested.

```csharp
public ColliderFields(byte VisibilityFlags, ulong LayerMask, ulong ObjectMaterialValue, bool IsPrimitive)
```

#### Parameters

`VisibilityFlags` [byte](https://learn.microsoft.com/dotnet/api/system.byte)

The collider's <code>VisibilityFlags</code>; bit 1 is set when the game's raycasts use it (#28).

`LayerMask` [ulong](https://learn.microsoft.com/dotnet/api/system.uint64)

The collider's <code>LayerMask</code>; bit 0 is the layer movement queries use (#28, vnavmesh notes).

`ObjectMaterialValue` [ulong](https://learn.microsoft.com/dotnet/api/system.uint64)

The collider's <code>ObjectMaterialValue</code>; bit 0x4000 marks primitives that block (#107).

`IsPrimitive` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

True for box, cylinder, sphere and plane colliders; false for mesh colliders.

## Properties

### <a id="Cube_Core_ColliderFields_IsPrimitive"></a> IsPrimitive

True for box, cylinder, sphere and plane colliders; false for mesh colliders.

```csharp
public bool IsPrimitive { get; init; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_ColliderFields_LayerMask"></a> LayerMask

The collider's <code>LayerMask</code>; bit 0 is the layer movement queries use (#28, vnavmesh notes).

```csharp
public ulong LayerMask { get; init; }
```

#### Property Value

 [ulong](https://learn.microsoft.com/dotnet/api/system.uint64)

### <a id="Cube_Core_ColliderFields_ObjectMaterialValue"></a> ObjectMaterialValue

The collider's <code>ObjectMaterialValue</code>; bit 0x4000 marks primitives that block (#107).

```csharp
public ulong ObjectMaterialValue { get; init; }
```

#### Property Value

 [ulong](https://learn.microsoft.com/dotnet/api/system.uint64)

### <a id="Cube_Core_ColliderFields_VisibilityFlags"></a> VisibilityFlags

The collider's <code>VisibilityFlags</code>; bit 1 is set when the game's raycasts use it (#28).

```csharp
public byte VisibilityFlags { get; init; }
```

#### Property Value

 [byte](https://learn.microsoft.com/dotnet/api/system.byte)
