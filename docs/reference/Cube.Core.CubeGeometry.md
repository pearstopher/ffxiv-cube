# <a id="Cube_Core_CubeGeometry"></a> Class CubeGeometry

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

Turns cubes into draw-list primitives: the big cube as a wireframe, and a single solid cube as its visible faces
(PROJECT.md: "Everything it draws is a cube"; "Cheap drawing": only the visible faces of a cube). Also holds the
face shades every solid box uses.

```csharp
public static class CubeGeometry
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[CubeGeometry](Cube.Core.CubeGeometry.md)


## Remarks

The big cube's wireframe is drawn from fixed cube-local corners, so it never turns on screen whatever the camera
does (PROJECT.md: "Cube on screen never turns by itself"). Solid cubes and boxes are world-axis-aligned contents
drawn by [BoxProjector](Cube.Core.BoxProjector.md#Cube_Core_BoxProjector), so they turn with the rest of the contents. Everything is projected with
[ViewProjection](Cube.Core.ViewProjection.md#Cube_Core_ViewProjection) and computed into locals: no allocation.

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

## Methods

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

#### Remarks

A one-off convenience over [BoxProjector](Cube.Core.BoxProjector.md#Cube_Core_BoxProjector); per-frame code that draws many boxes builds one
projector for the frame instead ([PlayerCubes](Cube.Core.PlayerCubes.md#Cube_Core_PlayerCubes), [CollisionCubes](Cube.Core.CollisionCubes.md#Cube_Core_CollisionCubes)).

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
