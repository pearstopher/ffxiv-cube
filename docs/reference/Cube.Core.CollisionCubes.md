# <a id="Cube_Core_CollisionCubes"></a> Class CollisionCubes

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

Draws the collision around the player as solid boxes in the cube, sorted together with the players (PROJECT.md,
"Collision (phase 2)": each collider inside or crossing the cube is shown as a simplified box, clipped to the cube;
"Draw order": far things first, so near ones cover far ones).

```csharp
public static class CollisionCubes
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[CollisionCubes](Cube.Core.CollisionCubes.md)


## Remarks

<p>
<b>Input.</b> The boxes come already clipped to the cube ([ClippedCollision](Cube.Core.ClippedCollision.md#Cube_Core_ClippedCollision)), in world coordinates,
and are drawn like player cubes ([BoxProjector](Cube.Core.BoxProjector.md#Cube_Core_BoxProjector)): in world axes, so they turn with the rest of the
contents, shaded the same way, as their visible faces only. A box cut by a side of a turned cube is drawn as the
prism the cut leaves ([AddPrism](Cube.Core.BoxProjector.md#Cube_Core_BoxProjector_AddPrism)), so it stays inside the cube at any yaw (#93); every
other box is a plain box ([AddFaces](Cube.Core.BoxProjector.md#Cube_Core_BoxProjector_AddFaces)). Each box's faces carry the tag
[TagFor](Cube.Core.CollisionCubes.md#Cube_Core_CollisionCubes_TagFor) gives it (negative, so they never clash with the scene's 0 or a player's index + 1).
</p>
<p>
<b>Sorting depth.</b> A player cube is small, so its centre's depth sorts it well. A collision box can be as big as
the cube (a floor, a wall), and sorting it by its centre would often paint it over a player standing on it or next
to it. Each box is sorted by its <em>farthest</em> corner instead ([FarDepth](Cube.Core.BoxProjector.md#Cube_Core_BoxProjector_FarDepth)): a floor or
wall then goes down before the small things around it, and a box resting on a bigger one is painted after it. The
cost is that a player behind a wall is painted over the wall rather than under it; the boxes are drawn see-through
([Alpha](Cube.Core.CollisionCubes.md#Cube_Core_CollisionCubes_Alpha)), so that reads as "behind" anyway, and PROJECT.md says exact per-pixel depth isn't required.
</p>

## Fields

### <a id="Cube_Core_CollisionCubes_Alpha"></a> Alpha

The boxes' own opacity before the opacity setting is applied: half see-through, so players and boxes behind a
wall still show through it.

```csharp
public const float Alpha = 0.5
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_CollisionCubes_MaxFacesPerBox"></a> MaxFacesPerBox

Most faces one clipped box can add: its top and four sides. Each edge of a footprint cut from a turned square
faces one of eight ways (either way along the two world axes and the cube's two), each at most once, and at
most two of the four world-axis ways and two of the cube's face the viewer. A plain box adds at most three.

```csharp
public const int MaxFacesPerBox = 5
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_CollisionCubes_Rgb"></a> Rgb

The collision colour: a muted grey-blue, unlike every player and NPC colour, so the scenery reads as scenery.

```csharp
public static readonly Vector3 Rgb
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

## Methods

### <a id="Cube_Core_CollisionCubes_Add_Cube_Core_DrawList_Cube_Core_BoxProjector__System_ReadOnlySpan_Cube_Core_ClippedBox__System_UInt32_"></a> Add\(DrawList, in BoxProjector, ReadOnlySpan<ClippedBox\>, uint\)

Adds every clipped box's visible faces, in the collision colour, each sorted by its farthest corner.

```csharp
public static void Add(DrawList list, in BoxProjector projector, ReadOnlySpan<ClippedBox> boxes, uint colour)
```

#### Parameters

`list` [DrawList](Cube.Core.DrawList.md)

The frame's draw list.

`projector` [BoxProjector](Cube.Core.BoxProjector.md)

This frame's box projector.

`boxes` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[ClippedBox](Cube.Core.ClippedBox.md)\>

This frame's boxes, already clipped to the cube (world coordinates).

`colour` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

The packed collision colour, opacity applied.

### <a id="Cube_Core_CollisionCubes_IndexOf_System_Int32_"></a> IndexOf\(int\)

The clipped-list index of the box a collision tag belongs to; the inverse of [TagFor](Cube.Core.CollisionCubes.md#Cube_Core_CollisionCubes_TagFor).

```csharp
public static int IndexOf(int tag)
```

#### Parameters

`tag` [int](https://learn.microsoft.com/dotnet/api/system.int32)

A collision tag.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_CollisionCubes_IsCollisionTag_System_Int32_"></a> IsCollisionTag\(int\)

Whether a primitive belongs to a collision box (its tag is negative).

```csharp
public static bool IsCollisionTag(int tag)
```

#### Parameters

`tag` [int](https://learn.microsoft.com/dotnet/api/system.int32)

A [Tag](Cube.Core.DrawPrimitive.md#Cube_Core_DrawPrimitive_Tag).

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_CollisionCubes_TagFor_System_Int32_"></a> TagFor\(int\)

The [Tag](Cube.Core.DrawPrimitive.md#Cube_Core_DrawPrimitive_Tag) of the faces of the clipped box at an index: −(index + 1).

```csharp
public static int TagFor(int index)
```

#### Parameters

`index` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The box's index in the clipped list.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)
