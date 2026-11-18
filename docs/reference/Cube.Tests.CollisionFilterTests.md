# <a id="Cube_Tests_CollisionFilterTests"></a> Class CollisionFilterTests

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

Checks which colliders are drawn: PROJECT.md, "Phase 2", says collision is "what the player would bump into, walk
on, or be stopped by", so colliders the player walks through (#107) must be dropped.

```csharp
public class CollisionFilterTests
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[CollisionFilterTests](Cube.Tests.CollisionFilterTests.md)


## Fields

### <a id="Cube_Tests_CollisionFilterTests_Active"></a> Active

Visible to raycasts, on the movement layer.

```csharp
private const byte Active = 1
```

#### Field Value

 [byte](https://learn.microsoft.com/dotnet/api/system.byte)

### <a id="Cube_Tests_CollisionFilterTests_Layer0"></a> Layer0

The movement layer bit.

```csharp
private const ulong Layer0 = 1
```

#### Field Value

 [ulong](https://learn.microsoft.com/dotnet/api/system.uint64)

## Methods

### <a id="Cube_Tests_CollisionFilterTests_ColliderOffTheMovementLayer_IsNotDrawn"></a> ColliderOffTheMovementLayer\_IsNotDrawn\(\)

```csharp
[Fact]
public void ColliderOffTheMovementLayer_IsNotDrawn()
```

### <a id="Cube_Tests_CollisionFilterTests_ColliderRaycastsIgnore_IsNotDrawn"></a> ColliderRaycastsIgnore\_IsNotDrawn\(\)

```csharp
[Fact]
public void ColliderRaycastsIgnore_IsNotDrawn()
```

### <a id="Cube_Tests_CollisionFilterTests_MeshOffTheMovementLayer_IsNotDrawn"></a> MeshOffTheMovementLayer\_IsNotDrawn\(\)

```csharp
[Fact]
public void MeshOffTheMovementLayer_IsNotDrawn()
```

### <a id="Cube_Tests_CollisionFilterTests_MeshOnMovementLayer_IsDrawn_WhateverItsMaterial_System_UInt64_"></a> MeshOnMovementLayer\_IsDrawn\_WhateverItsMaterial\(ulong\)

```csharp
[Theory]
[InlineData(new object[] { 0 })]
[InlineData(new object[] { 28672 })]
public void MeshOnMovementLayer_IsDrawn_WhateverItsMaterial(ulong material)
```

#### Parameters

`material` [ulong](https://learn.microsoft.com/dotnet/api/system.uint64)

### <a id="Cube_Tests_CollisionFilterTests_PrimitiveWithBlockingMaterial_IsDrawn_System_UInt64_"></a> PrimitiveWithBlockingMaterial\_IsDrawn\(ulong\)

```csharp
[Theory]
[InlineData(new object[] { 16384 })]
[InlineData(new object[] { 16389 })]
[InlineData(new object[] { 24580 })]
[InlineData(new object[] { 25606 })]
public void PrimitiveWithBlockingMaterial_IsDrawn(ulong material)
```

#### Parameters

`material` [ulong](https://learn.microsoft.com/dotnet/api/system.uint64)

### <a id="Cube_Tests_CollisionFilterTests_PrimitiveWithoutBlockingMaterial_IsNotDrawn_System_UInt64_"></a> PrimitiveWithoutBlockingMaterial\_IsNotDrawn\(ulong\)

```csharp
[Theory]
[InlineData(new object[] { 8196 })]
[InlineData(new object[] { 8197 })]
[InlineData(new object[] { 8198 })]
[InlineData(new object[] { 8202 })]
[InlineData(new object[] { 12293 })]
[InlineData(new object[] { 8192 })]
[InlineData(new object[] { 0 })]
public void PrimitiveWithoutBlockingMaterial_IsNotDrawn(ulong material)
```

#### Parameters

`material` [ulong](https://learn.microsoft.com/dotnet/api/system.uint64)
