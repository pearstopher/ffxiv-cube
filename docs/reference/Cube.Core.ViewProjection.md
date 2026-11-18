# <a id="Cube_Core_ViewProjection"></a> Struct ViewProjection

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

Projects view coordinates (from [CubeSpace](Cube.Core.CubeSpace.md#Cube_Core_CubeSpace)) onto the window, as seen from the user's chosen
[ViewOrientation](Cube.Core.ViewOrientation.md#Cube_Core_ViewOrientation) (PROJECT.md, "View orientation control"; default from above and behind at 30°).

```csharp
public readonly struct ViewProjection
```


## Remarks

<p>
The projection is orthographic: things don't shrink with distance, so a player cube is the same size
anywhere in the cube (PROJECT.md, "Players": cube size fixed relative to the big cube) and the painter's
sort only needs one depth per primitive.
</p>
<p>
The viewer looks along [Forward](Cube.Core.ViewOrientation.md#Cube_Core_ViewOrientation_Forward). On screen, a point's position is its component along
the orientation's right and up axes; [Depth](Cube.Core.ViewProjection.md#Cube_Core_ViewProjection_Depth) is its component along the forward axis, so it grows with
distance from the viewer. The axes are copied from the orientation, so each point costs two or three dot products.
</p>

## Constructors

### <a id="Cube_Core_ViewProjection__ctor_Cube_Core_ViewOrientation__System_Numerics_Vector2_System_Single_"></a> ViewProjection\(in ViewOrientation, Vector2, float\)

Creates a projection.

```csharp
public ViewProjection(in ViewOrientation orientation, Vector2 origin, float pixelsPerYalm)
```

#### Parameters

`orientation` [ViewOrientation](Cube.Core.ViewOrientation.md)

How the cube is turned on screen.

`origin` [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

Window position (pixels) of the cube's centre.

`pixelsPerYalm` [float](https://learn.microsoft.com/dotnet/api/system.single)

Scale from yalms to pixels.

## Fields

### <a id="Cube_Core_ViewProjection_FillFraction"></a> FillFraction

Fraction of the window the cube may fill, leaving a margin for labels.

```csharp
public const float FillFraction = 0.9
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

## Properties

### <a id="Cube_Core_ViewProjection_Orientation"></a> Orientation

How the cube is turned on screen.

```csharp
public ViewOrientation Orientation { get; }
```

#### Property Value

 [ViewOrientation](Cube.Core.ViewOrientation.md)

### <a id="Cube_Core_ViewProjection_Origin"></a> Origin

Screen position (pixels) of the cube's centre.

```csharp
public Vector2 Origin { get; }
```

#### Property Value

 [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

### <a id="Cube_Core_ViewProjection_PixelsPerYalm"></a> PixelsPerYalm

Scale from yalms to pixels.

```csharp
public float PixelsPerYalm { get; }
```

#### Property Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_ViewProjection_TowardsViewer"></a> TowardsViewer

Direction from the scene towards the viewer, in view coordinates. A face is visible when its outward
normal points this way; when the view is tilted from below, that makes bottom faces visible and top faces not.

```csharp
public Vector3 TowardsViewer { get; }
```

#### Property Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

## Methods

### <a id="Cube_Core_ViewProjection_Depth_System_Numerics_Vector3_"></a> Depth\(Vector3\)

Distance of a point from the viewer along the view direction; larger is further away.

```csharp
public float Depth(Vector3 view)
```

#### Parameters

`view` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

A point in view coordinates.

#### Returns

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_ViewProjection_Fit_System_Numerics_Vector2_System_Numerics_Vector2_System_Single_Cube_Core_ViewOrientation__"></a> Fit\(Vector2, Vector2, float, in ViewOrientation\)

The projection that fits a cube of the given size into a window region, centred. The scale fits the cube's
bounding sphere (radius √3 × half the edge), so the cube never changes size on screen while the user turns it
with the view control.

```csharp
public static ViewProjection Fit(Vector2 regionMin, Vector2 regionSize, float halfEdge, in ViewOrientation orientation)
```

#### Parameters

`regionMin` [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

Top-left of the drawing region, in pixels.

`regionSize` [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

Size of the drawing region, in pixels.

`halfEdge` [float](https://learn.microsoft.com/dotnet/api/system.single)

Half the cube's edge, in yalms.

`orientation` [ViewOrientation](Cube.Core.ViewOrientation.md)

How the cube is turned on screen.

#### Returns

 [ViewProjection](Cube.Core.ViewProjection.md)

### <a id="Cube_Core_ViewProjection_ToScreen_System_Numerics_Vector3_"></a> ToScreen\(Vector3\)

The window position (pixels) of a point in view coordinates.

```csharp
public Vector2 ToScreen(Vector3 view)
```

#### Parameters

`view` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

A point in view coordinates (right, up, forward), in yalms.

#### Returns

 [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)
