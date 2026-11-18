# <a id="Cube_TestBridge_Checks_CollisionProbeCheck_VoxelGrid"></a> Class CollisionProbeCheck.VoxelGrid

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

A fixed cube of on/off cells centred on the player, allocated once: the shape a "coarse voxel grid"
(PROJECT.md, "Collision") would take, used here to measure how much of the cube each method fills.

```csharp
private sealed class CollisionProbeCheck.VoxelGrid
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[CollisionProbeCheck.VoxelGrid](Cube.TestBridge.Checks.CollisionProbeCheck.VoxelGrid.md)


## Constructors

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_VoxelGrid__ctor_System_Int32_System_Single_"></a> VoxelGrid\(int, float\)

A fixed cube of on/off cells centred on the player, allocated once: the shape a "coarse voxel grid"
(PROJECT.md, "Collision") would take, used here to measure how much of the cube each method fills.

```csharp
public VoxelGrid(int perSide, float edge)
```

#### Parameters

`perSide` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Cells per side.

`edge` [float](https://learn.microsoft.com/dotnet/api/system.single)

The grid's edge in yalms.

## Fields

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_VoxelGrid_cells"></a> cells

One flag per cell, x fastest, then z, then y.

```csharp
private readonly bool[] cells
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)\[\]

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_VoxelGrid_origin"></a> origin

The grid's low corner in world space.

```csharp
private Vector3 origin
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

## Properties

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_VoxelGrid_Length"></a> Length

Gets the number of cells.

```csharp
public int Length { get; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Methods

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_VoxelGrid_Clamp_System_Single_"></a> Clamp\(float\)

A cell coordinate clamped into the grid.

```csharp
private int Clamp(float value)
```

#### Parameters

`value` [float](https://learn.microsoft.com/dotnet/api/system.single)

The coordinate in cells.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_VoxelGrid_Count"></a> Count\(\)

Counts the set cells.

```csharp
public int Count()
```

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_VoxelGrid_IsSet_System_Numerics_Vector3_"></a> IsSet\(Vector3\)

Whether the cell holding a point is set.

```csharp
public bool IsSet(Vector3 point)
```

#### Parameters

`point` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

A world position.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_VoxelGrid_MarkBox_System_Numerics_Vector3_System_Numerics_Vector3_"></a> MarkBox\(Vector3, Vector3\)

Sets every cell a box touches (clipped to the grid).

```csharp
public void MarkBox(Vector3 min, Vector3 max)
```

#### Parameters

`min` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The box's low corner.

`max` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The box's high corner.

### <a id="Cube_TestBridge_Checks_CollisionProbeCheck_VoxelGrid_Reset_System_Numerics_Vector3_"></a> Reset\(Vector3\)

Clears every cell and centres the grid on a new point.

```csharp
public void Reset(Vector3 centre)
```

#### Parameters

`centre` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The grid's centre.
