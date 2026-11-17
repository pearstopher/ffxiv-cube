# <a id="Cube_Core_ViewProjection"></a> Struct ViewProjection

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

Projects view coordinates (from [CubeSpace](Cube.Core.CubeSpace.md#Cube_Core_CubeSpace)) onto the window: a fixed-angle view from above
and behind (PROJECT.md, "View": default 30° elevation).

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
The viewer looks forward (+Z in view coordinates) and down by [Elevation](Cube.Core.ViewProjection.md#Cube_Core_ViewProjection_Elevation). On screen, a point
further forward or higher appears higher; [Depth](Cube.Core.ViewProjection.md#Cube_Core_ViewProjection_Depth) grows with distance from the viewer.
</p>

## Constructors

### <a id="Cube_Core_ViewProjection__ctor_System_Single_System_Numerics_Vector2_System_Single_"></a> ViewProjection\(float, Vector2, float\)

Creates a projection.

```csharp
public ViewProjection(float elevation, Vector2 origin, float pixelsPerYalm)
```

#### Parameters

`elevation` [float](https://learn.microsoft.com/dotnet/api/system.single)

Angle of the view above the horizontal, in radians.

`origin` [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

Window position (pixels) of the cube's centre.

`pixelsPerYalm` [float](https://learn.microsoft.com/dotnet/api/system.single)

Scale from yalms to pixels.

## Fields

### <a id="Cube_Core_ViewProjection_DefaultElevationDegrees"></a> DefaultElevationDegrees

Default elevation of the view, in degrees (PROJECT.md).

```csharp
public const float DefaultElevationDegrees = 30
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_ViewProjection_FillFraction"></a> FillFraction

Fraction of the window the cube may fill, leaving a margin for labels.

```csharp
public const float FillFraction = 0.9
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_ViewProjection_cos"></a> cos

Cosine of the elevation.

```csharp
private readonly float cos
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_ViewProjection_sin"></a> sin

Sine of the elevation.

```csharp
private readonly float sin
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

## Properties

### <a id="Cube_Core_ViewProjection_Elevation"></a> Elevation

Angle of the view above the horizontal, in radians.

```csharp
public float Elevation { get; }
```

#### Property Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

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
normal points this way.

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

### <a id="Cube_Core_ViewProjection_Fit_System_Numerics_Vector2_System_Numerics_Vector2_System_Single_System_Single_"></a> Fit\(Vector2, Vector2, float, float\)

The projection that fits a cube of the given size into a window region, centred. The scale allows for
any yaw (a corner can swing out to √2 × half the edge horizontally), so the cube doesn't change size
as the camera turns.

```csharp
public static ViewProjection Fit(Vector2 regionMin, Vector2 regionSize, float halfEdge, float elevation)
```

#### Parameters

`regionMin` [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

Top-left of the drawing region, in pixels.

`regionSize` [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

Size of the drawing region, in pixels.

`halfEdge` [float](https://learn.microsoft.com/dotnet/api/system.single)

Half the cube's edge, in yalms.

`elevation` [float](https://learn.microsoft.com/dotnet/api/system.single)

Elevation of the view, in radians.

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
