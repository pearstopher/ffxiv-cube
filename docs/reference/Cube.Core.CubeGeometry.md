# <a id="Cube_Core_CubeGeometry"></a> Class CubeGeometry

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

Turns cubes into draw-list primitives: the big cube as a wireframe, and solid cubes (players, later
collision boxes) as their visible faces only (PROJECT.md: "Everything it draws is a cube"; "Cheap drawing":
only the visible faces of a cube).

```csharp
public static class CubeGeometry
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[CubeGeometry](Cube.Core.CubeGeometry.md)


## Remarks

The big cube's wireframe is drawn from fixed cube-local corners, so it never turns on screen whatever the camera
does (PROJECT.md: "Cube on screen never turns by itself"). Solid cubes are world-axis-aligned contents, so their
face normals are rotated into cube-local coordinates with [RotateOffset](Cube.Core.CubeSpace.md#Cube_Core_CubeSpace_RotateOffset) and they turn with
the rest of the contents. Everything is projected with [ViewProjection](Cube.Core.ViewProjection.md#Cube_Core_ViewProjection) and computed into locals: no
allocation.

## Fields

### <a id="Cube_Core_CubeGeometry_BottomShade"></a> BottomShade

Brightness of the bottom face, seen only if the view were from below.

```csharp
public const float BottomShade = 0.45
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_CubeGeometry_EastWestShade"></a> EastWestShade

Brightness of the side faces running north–south (normals ±X).

```csharp
public const float EastWestShade = 0.6
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_CubeGeometry_NorthSouthShade"></a> NorthSouthShade

Brightness of the side faces running east–west (normals ±Z).

```csharp
public const float NorthSouthShade = 0.75
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_CubeGeometry_TopShade"></a> TopShade

Brightness of the top face (lit from above).

```csharp
public const float TopShade = 1
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_CubeGeometry_VisibleThreshold"></a> VisibleThreshold

A face counts as visible when its normal points towards the viewer by more than this; faces seen exactly
edge-on are skipped, since they would draw as a line.

```csharp
private const float VisibleThreshold = 0.0001
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

## Methods

### <a id="Cube_Core_CubeGeometry_AddFace_Cube_Core_DrawList_Cube_Core_ViewProjection__System_Numerics_Vector3_System_Numerics_Vector3_System_Numerics_Vector3_System_Numerics_Vector3_System_Numerics_Vector3_System_Single_System_UInt32_System_Int32_"></a> AddFace\(DrawList, in ViewProjection, Vector3, Vector3, Vector3, Vector3, Vector3, float, uint, int\)

Adds one face if it faces the viewer.

```csharp
private static void AddFace(DrawList list, in ViewProjection projection, Vector3 centre, Vector3 normal, Vector3 u, Vector3 v, Vector3 towardsViewer, float depth, uint colour, int tag)
```

#### Parameters

`list` [DrawList](Cube.Core.DrawList.md)

The draw list.

`projection` [ViewProjection](Cube.Core.ViewProjection.md)

The projection.

`centre` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The cube's centre (view coordinates).

`normal` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

Offset from the cube centre to the face centre; also the face's outward normal.

`u` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

One in-face half-axis.

`v` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The other in-face half-axis.

`towardsViewer` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

Direction to the viewer.

`depth` [float](https://learn.microsoft.com/dotnet/api/system.single)

Depth to sort the face by.

`colour` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

Face colour.

`tag` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Owner of the face.

### <a id="Cube_Core_CubeGeometry_AddSolidCube_Cube_Core_DrawList_Cube_Core_CubeSpace__Cube_Core_ViewProjection__System_Numerics_Vector3_System_Single_System_UInt32_System_Int32_"></a> AddSolidCube\(DrawList, in CubeSpace, in ViewProjection, Vector3, float, uint, int\)

Adds a solid, world-axis-aligned cube as its visible faces (at most three). It is part of the contents, so it
turns with them when the cube's alignment changes. All faces share the cube
centre's depth: a convex cube's visible faces never overlap each other, and one depth per cube keeps
its faces together in the painter's sort.

```csharp
public static void AddSolidCube(DrawList list, in CubeSpace space, in ViewProjection projection, Vector3 viewCentre, float halfSize, uint colour, int tag = 0)
```

#### Parameters

`list` [DrawList](Cube.Core.DrawList.md)

The frame's draw list.

`space` [CubeSpace](Cube.Core.CubeSpace.md)

The big cube, for its alignment.

`projection` [ViewProjection](Cube.Core.ViewProjection.md)

The view's projection.

`viewCentre` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The small cube's centre in view coordinates.

`halfSize` [float](https://learn.microsoft.com/dotnet/api/system.single)

Half the small cube's edge, in yalms.

`colour` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

Base colour; side faces are shaded darker.

`tag` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Owner of the faces ([Tag](Cube.Core.DrawPrimitive.md#Cube_Core_DrawPrimitive_Tag)).

### <a id="Cube_Core_CubeGeometry_AddWireCube_Cube_Core_DrawList_Cube_Core_CubeSpace__Cube_Core_ViewProjection__System_UInt32_"></a> AddWireCube\(DrawList, in CubeSpace, in ViewProjection, uint\)

Adds the big cube's 12 edges as lines, each at the depth of its midpoint. The corners are fixed cube-local
points (± half the edge), independent of [Yaw](Cube.Core.CubeSpace.md#Cube_Core_CubeSpace_Yaw), so turning the camera leaves these
primitives unchanged.

```csharp
public static void AddWireCube(DrawList list, in CubeSpace space, in ViewProjection projection, uint colour)
```

#### Parameters

`list` [DrawList](Cube.Core.DrawList.md)

The frame's draw list.

`space` [CubeSpace](Cube.Core.CubeSpace.md)

The cube, for its size.

`projection` [ViewProjection](Cube.Core.ViewProjection.md)

The view's projection.

`colour` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

Edge colour.

### <a id="Cube_Core_CubeGeometry_CornerSign_System_Int32_"></a> CornerSign\(int\)

The 8 corners of a unit cube, as signs: bit 0 → X, bit 1 → Y, bit 2 → Z.

```csharp
private static Vector3 CornerSign(int index)
```

#### Parameters

`index` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Corner index 0–7.

#### Returns

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)
