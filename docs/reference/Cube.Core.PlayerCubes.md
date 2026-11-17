# <a id="Cube_Core_PlayerCubes"></a> Class PlayerCubes

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

Draws each player in the cube as a small solid cube in their relation's colour, with a name label for other
players (PROJECT.md, "Players": player cube size fixed relative to the big cube, about 1/40 of its edge, minimum a
few pixels; "Features": the local player is a cube at the centre in its own colour, other players are labelled
with their names; "Labels": the name drawn next to the cube, overlaps are fine).

```csharp
public static class PlayerCubes
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[PlayerCubes](Cube.Core.PlayerCubes.md)


## Remarks

Player cubes and labels go into the same [DrawList](Cube.Core.DrawList.md#Cube_Core_DrawList) as the wireframe and are sorted with it, so near
cubes cover far ones (PROJECT.md, "Draw order"). A label shares its cube's depth and is added after the cube's
faces, so it is painted right on top of its own cube and under anything nearer. Each player's faces and label are
tagged with the player's index + 1.

## Fields

### <a id="Cube_Core_PlayerCubes_EdgeFraction"></a> EdgeFraction

Player cube edge as a fraction of the big cube's edge (PROJECT.md: about 1/40).

```csharp
public const float EdgeFraction = 0.025
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_PlayerCubes_LabelGap"></a> LabelGap

Space between a cube's widest possible outline and its label, in pixels.

```csharp
public const float LabelGap = 3
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

### <a id="Cube_Core_PlayerCubes_WidestReach"></a> WidestReach

Furthest a cube's outline reaches sideways from its centre, per unit of half-edge: half its horizontal
diagonal (√2) once the view has turned it 45°. Using the worst case keeps the label clear of the cube at
every yaw without measuring the faces.

```csharp
private const float WidestReach = 1.4143
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

## Properties

### <a id="Cube_Core_PlayerCubes_LabelColour"></a> LabelColour

Label text colour: near-white, readable over the dark window and every palette colour.

```csharp
public static uint LabelColour { get; }
```

#### Property Value

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

## Methods

### <a id="Cube_Core_PlayerCubes_Add_Cube_Core_DrawList_Cube_Core_CubeSpace__Cube_Core_ViewProjection__System_ReadOnlySpan_Cube_Core_NearbyPlayer__Cube_Core_PlayerPalette__System_Boolean_"></a> Add\(DrawList, in CubeSpace, in ViewProjection, ReadOnlySpan<NearbyPlayer\>, in PlayerPalette, bool\)

Adds a cube for every player, in their colour, and a name label for everyone but the local player.

```csharp
public static void Add(DrawList list, in CubeSpace space, in ViewProjection projection, ReadOnlySpan<NearbyPlayer> players, in PlayerPalette palette, bool labels = true)
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

`labels` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether to add name labels (PROJECT.md, "Features": labels on/off setting).

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
