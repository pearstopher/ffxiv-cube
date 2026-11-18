# <a id="Cube_Core_ViewWidget"></a> Class ViewWidget

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

Where the view control goes and how big it is (PROJECT.md, "View orientation control": a small control in a corner
of the window). Kept in Core so the placement rules are unit tested.

```csharp
public static class ViewWidget
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ViewWidget](Cube.Core.ViewWidget.md)


## Remarks

The cube is fitted to a circle filling [FillFraction](Cube.Core.ViewProjection.md#Cube_Core_ViewProjection_FillFraction) of the region's smaller side, so a
corner square up to about 0.18 of that side stays outside the circle and out of the cube's way. The control is kept
between [MinSize](Cube.Core.ViewWidget.md#Cube_Core_ViewWidget_MinSize) (still usable) and [MaxSize](Cube.Core.ViewWidget.md#Cube_Core_ViewWidget_MaxSize) (no need for more in a big window).

## Fields

### <a id="Cube_Core_ViewWidget_MaxSize"></a> MaxSize

Largest control, in pixels.

```csharp
public const float MaxSize = 120
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_ViewWidget_MinSize"></a> MinSize

Smallest control, in pixels, so it stays usable in a small window.

```csharp
public const float MinSize = 48
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_ViewWidget_SizeFraction"></a> SizeFraction

Fraction of the region's smaller side the control takes.

```csharp
public const float SizeFraction = 0.18
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

## Methods

### <a id="Cube_Core_ViewWidget_Position_System_Numerics_Vector2_System_Numerics_Vector2_Cube_Core_WidgetCorner_System_Single_"></a> Position\(Vector2, Vector2, WidgetCorner, float\)

Top-left of the control, in the chosen corner of the drawing region.

```csharp
public static Vector2 Position(Vector2 regionMin, Vector2 regionSize, WidgetCorner corner, float size)
```

#### Parameters

`regionMin` [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

Top-left of the drawing region, in pixels.

`regionSize` [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

Size of the drawing region, in pixels.

`corner` [WidgetCorner](Cube.Core.WidgetCorner.md)

The chosen corner.

`size` [float](https://learn.microsoft.com/dotnet/api/system.single)

The control's side length (from [Size](Cube.Core.ViewWidget.md#Cube_Core_ViewWidget_Size)).

#### Returns

 [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

### <a id="Cube_Core_ViewWidget_Size_System_Numerics_Vector2_"></a> Size\(Vector2\)

The control's side length for a drawing region.

```csharp
public static float Size(Vector2 regionSize)
```

#### Parameters

`regionSize` [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

The drawing region's size, in pixels.

#### Returns

 [float](https://learn.microsoft.com/dotnet/api/system.single)
