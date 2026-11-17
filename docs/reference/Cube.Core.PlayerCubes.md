# <a id="Cube_Core_PlayerCubes"></a> Class PlayerCubes

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

Draws each player in the cube as a small solid cube in their relation's colour (PROJECT.md, "Players": player cube
size fixed relative to the big cube, about 1/40 of its edge, minimum a few pixels; "Features": the local player is a
cube at the centre in its own colour).

```csharp
public static class PlayerCubes
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[PlayerCubes](Cube.Core.PlayerCubes.md)


## Remarks

Player cubes go into the same [DrawList](Cube.Core.DrawList.md#Cube_Core_DrawList) as the wireframe and are sorted with it, so near cubes cover far
ones (PROJECT.md, "Draw order"). Each player's faces are tagged with the player's index + 1.

## Fields

### <a id="Cube_Core_PlayerCubes_EdgeFraction"></a> EdgeFraction

Player cube edge as a fraction of the big cube's edge (PROJECT.md: about 1/40).

```csharp
public const float EdgeFraction = 0.025
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_PlayerCubes_MinEdgePixels"></a> MinEdgePixels

Smallest on-screen edge of a player cube, in pixels, so players stay visible in a small window.

```csharp
public const float MinEdgePixels = 6
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

## Methods

### <a id="Cube_Core_PlayerCubes_Add_Cube_Core_DrawList_Cube_Core_CubeSpace__Cube_Core_ViewProjection__System_ReadOnlySpan_Cube_Core_NearbyPlayer__Cube_Core_PlayerPalette__"></a> Add\(DrawList, in CubeSpace, in ViewProjection, ReadOnlySpan<NearbyPlayer\>, in PlayerPalette\)

Adds a cube for every player, in their colour.

```csharp
public static void Add(DrawList list, in CubeSpace space, in ViewProjection projection, ReadOnlySpan<NearbyPlayer> players, in PlayerPalette palette)
```

#### Parameters

`list` [DrawList](Cube.Core.DrawList.md)

The frame's draw list.

`space` [CubeSpace](Cube.Core.CubeSpace.md)

The big cube.

`projection` [ViewProjection](Cube.Core.ViewProjection.md)

The projection.

`players` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[NearbyPlayer](Cube.Core.NearbyPlayer.md)\>

This frame's players inside the cube.

`palette` [PlayerPalette](Cube.Core.PlayerPalette.md)

Colours per relation.

### <a id="Cube_Core_PlayerCubes_HalfSize_Cube_Core_CubeSpace__Cube_Core_ViewProjection__"></a> HalfSize\(in CubeSpace, in ViewProjection\)

Half the edge of a player cube, in yalms, for this frame's cube and scale.

```csharp
public static float HalfSize(in CubeSpace space, in ViewProjection projection)
```

#### Parameters

`space` [CubeSpace](Cube.Core.CubeSpace.md)

The big cube.

`projection` [ViewProjection](Cube.Core.ViewProjection.md)

The projection, for the minimum on-screen size.

#### Returns

 [float](https://learn.microsoft.com/dotnet/api/system.single)
