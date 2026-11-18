# <a id="Cube_TestBridge_Checks_CollisionProbeCheck_TypeCounts"></a> Struct CollisionProbeCheck.TypeCounts

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

Colliders counted by type, without a dictionary so the timed walk allocates nothing.

```csharp
private struct CollisionProbeCheck.TypeCounts
```


## Fields

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_TypeCounts_Box"></a> Box

Box colliders.

```csharp
public int Box
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_TypeCounts_Cylinder"></a> Cylinder

Cylinder colliders.

```csharp
public int Cylinder
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_TypeCounts_Mesh"></a> Mesh

Mesh colliders (terrain tiles and placed objects).

```csharp
public int Mesh
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_TypeCounts_Plane"></a> Plane

One- and two-sided plane colliders.

```csharp
public int Plane
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_TypeCounts_Sphere"></a> Sphere

Sphere colliders.

```csharp
public int Sphere
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_TypeCounts_Streamed"></a> Streamed

Streamed colliders (terrain managers).

```csharp
public int Streamed
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Methods

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_TypeCounts_Add_FFXIVClientStructs_FFXIV_Common_Component_BGCollision_ColliderType_"></a> Add\(ColliderType\)

Counts one collider.

```csharp
public void Add(ColliderType type)
```

#### Parameters

`type` ColliderType

Its type.

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_TypeCounts_ToString"></a> ToString\(\)

Returns the fully qualified type name of this instance.

```csharp
public override readonly string ToString()
```

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

The fully qualified type name.
