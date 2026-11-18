# <a id="Cube_Core_BoxProjector"></a> Struct BoxProjector

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

Projects world-axis-aligned boxes (player cubes, collision boxes) onto the window for one frame, as their visible
faces only (PROJECT.md: "Everything it draws is a cube"; "Cheap drawing": only the visible faces of a cube).

```csharp
public readonly struct BoxProjector
```


## Remarks

<p>
The projection is orthographic, so it is linear: one yalm along a world axis is always the same step on screen
and the same step in depth, wherever the box is. The constructor works those steps out once from the cube's
alignment and the view, together with which side of each axis faces the viewer (the same for every box). Each box
then costs one centre projection and a few 2D additions, which matters with thousands of collision boxes
(PROJECT.md, "Efficiency and memory").
</p>
<p>
Boxes are world-axis-aligned contents: the steps include the cube's alignment, so boxes turn with the rest of the
contents when it changes, while the wireframe ([AddWireCube](Cube.Core.CubeGeometry.md#Cube_Core_CubeGeometry_AddWireCube)) stays still. A struct built
on the stack: nothing is allocated.
</p>

## Constructors

### <a id="Cube_Core_BoxProjector__ctor_Cube_Core_CubeSpace__Cube_Core_ViewProjection__"></a> BoxProjector\(in CubeSpace, in ViewProjection\)

Works out the frame's steps and facing.

```csharp
public BoxProjector(in CubeSpace space, in ViewProjection projection)
```

#### Parameters

`space` [CubeSpace](Cube.Core.CubeSpace.md)

The big cube, for its centre and alignment.

`projection` [ViewProjection](Cube.Core.ViewProjection.md)

The view's projection.

## Fields

### <a id="Cube_Core_BoxProjector_VisibleThreshold"></a> VisibleThreshold

A face counts as visible when its normal points towards the viewer by more than this; faces seen exactly
edge-on are skipped, since they would draw as a line.

```csharp
private const float VisibleThreshold = 0.0001
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_BoxProjector_centre"></a> centre

The cube's centre in the world.

```csharp
private readonly Vector3 centre
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_BoxProjector_depthStep"></a> depthStep

The depth step of one yalm along each world axis.

```csharp
private readonly Vector3 depthStep
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_BoxProjector_facing"></a> facing

Which side of each world axis faces the viewer: +1, −1, or 0 when those faces are seen edge-on.

```csharp
private readonly Vector3 facing
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_BoxProjector_origin"></a> origin

The screen position of the cube's centre.

```csharp
private readonly Vector2 origin
```

#### Field Value

 [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

### <a id="Cube_Core_BoxProjector_stepX"></a> stepX

The screen step of one yalm along world X.

```csharp
private readonly Vector2 stepX
```

#### Field Value

 [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

### <a id="Cube_Core_BoxProjector_stepY"></a> stepY

The screen step of one yalm along world Y.

```csharp
private readonly Vector2 stepY
```

#### Field Value

 [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

### <a id="Cube_Core_BoxProjector_stepZ"></a> stepZ

The screen step of one yalm along world Z.

```csharp
private readonly Vector2 stepZ
```

#### Field Value

 [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

## Properties

### <a id="Cube_Core_BoxProjector_SeesTop"></a> SeesTop

Whether the visible horizontal face is the top (the view is from above or level), not the bottom.

```csharp
public bool SeesTop { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Methods

### <a id="Cube_Core_BoxProjector_AddFace_Cube_Core_DrawList_System_Numerics_Vector2_System_Numerics_Vector2_System_Numerics_Vector2_System_Single_System_UInt32_System_Int32_"></a> AddFace\(DrawList, Vector2, Vector2, Vector2, float, uint, int\)

Adds one face as a quad, its corners in order around it.

```csharp
private static void AddFace(DrawList list, Vector2 centre, Vector2 u, Vector2 v, float depth, uint colour, int tag)
```

#### Parameters

`list` [DrawList](Cube.Core.DrawList.md)

The draw list.

`centre` [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

The face's centre on screen.

`u` [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

One in-face half-axis on screen.

`v` [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

The other in-face half-axis on screen.

`depth` [float](https://learn.microsoft.com/dotnet/api/system.single)

The sorting depth.

`colour` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

The face's colour.

`tag` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The owner.

### <a id="Cube_Core_BoxProjector_AddFaces_Cube_Core_DrawList_System_Numerics_Vector2_System_Numerics_Vector3_System_Single_Cube_Core_BoxShades__System_Int32_"></a> AddFaces\(DrawList, Vector2, Vector3, float, in BoxShades, int\)

Adds a box's visible faces (at most three), all at one depth. A face with a zero-length side is skipped: a flat
floor shows its top only, and a line-shaped box nothing.

```csharp
public void AddFaces(DrawList list, Vector2 screen, Vector3 half, float depth, in BoxShades shades, int tag)
```

#### Parameters

`list` [DrawList](Cube.Core.DrawList.md)

The frame's draw list.

`screen` [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

The box centre's screen position ([Screen](Cube.Core.BoxProjector.md#Cube_Core_BoxProjector_Screen)).

`half` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

Half the box's size along the world axes, in yalms.

`depth` [float](https://learn.microsoft.com/dotnet/api/system.single)

The depth all its faces are sorted by.

`shades` [BoxShades](Cube.Core.BoxShades.md)

The face colours ([Shades](Cube.Core.BoxProjector.md#Cube_Core_BoxProjector_Shades)).

`tag` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Owner of the faces ([Tag](Cube.Core.DrawPrimitive.md#Cube_Core_DrawPrimitive_Tag)).

### <a id="Cube_Core_BoxProjector_AddPrism_Cube_Core_DrawList_Cube_Core_ClippedBox__System_Single_Cube_Core_BoxShades__System_Int32_"></a> AddPrism\(DrawList, in ClippedBox, float, in BoxShades, int\)

Adds the visible faces of a collision box clipped to a turned cube (#93): an upright prism on its footprint
polygon ([ClippedBox](Cube.Core.ClippedBox.md#Cube_Core_ClippedBox)). The top (or bottom, seen from below) is one quad or polygon, and each side
whose outward normal faces the viewer is a quad, so nothing reaches past the footprint and so past the cube.

```csharp
public void AddPrism(DrawList list, in ClippedBox box, float depth, in BoxShades shades, int tag)
```

#### Parameters

`list` [DrawList](Cube.Core.DrawList.md)

The frame's draw list.

`box` [ClippedBox](Cube.Core.ClippedBox.md)

The clipped box; its footprint corners go counter-clockwise.

`depth` [float](https://learn.microsoft.com/dotnet/api/system.single)

The depth all its faces are sorted by.

`shades` [BoxShades](Cube.Core.BoxShades.md)

The face colours ([Shades](Cube.Core.BoxProjector.md#Cube_Core_BoxProjector_Shades)).

`tag` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Owner of the faces ([Tag](Cube.Core.DrawPrimitive.md#Cube_Core_DrawPrimitive_Tag)).

#### Remarks

Sides are shaded like a box's: the east/west shade when the side faces more east or west than north or south,
otherwise the north/south shade, so a turned side reads as part of the same solid. All faces share one depth,
as a box's do. A zero-height prism (a flat floor) shows its top only. Nothing is allocated: the projected
corners are on the stack.

### <a id="Cube_Core_BoxProjector_Depth_System_Numerics_Vector3_"></a> Depth\(Vector3\)

The depth of a world point ([Depth](Cube.Core.ViewProjection.md#Cube_Core_ViewProjection_Depth) of its view position).

```csharp
public float Depth(Vector3 world)
```

#### Parameters

`world` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

A world point.

#### Returns

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_BoxProjector_Facing_System_Single_"></a> Facing\(float\)

The sign of the face along an axis that faces the viewer, from the axis's depth step.

```csharp
private static float Facing(float depthStep)
```

#### Parameters

`depthStep` [float](https://learn.microsoft.com/dotnet/api/system.single)

How much depth one yalm along the axis adds.

#### Returns

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_BoxProjector_FarDepth_System_Numerics_Vector3_System_Numerics_Vector3_"></a> FarDepth\(Vector3, Vector3\)

The depth of a box's farthest corner: its centre's depth plus how far each half-size reaches in depth.

```csharp
public float FarDepth(Vector3 world, Vector3 half)
```

#### Parameters

`world` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The box's centre in the world.

`half` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

Half the box's size along the world axes.

#### Returns

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_BoxProjector_Screen_System_Numerics_Vector3_"></a> Screen\(Vector3\)

The screen position of a world point.

```csharp
public Vector2 Screen(Vector3 world)
```

#### Parameters

`world` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

A world point.

#### Returns

 [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

### <a id="Cube_Core_BoxProjector_Shades_System_UInt32_"></a> Shades\(uint\)

The three colours a box's visible faces are drawn in, from its base colour.

```csharp
public BoxShades Shades(uint colour)
```

#### Parameters

`colour` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

The base colour.

#### Returns

 [BoxShades](Cube.Core.BoxShades.md)
