# <a id="Cube_Core_ZoomSlider"></a> Struct ZoomSlider

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

The settings window's zoom slider (#94; PROJECT.md, "Zoom"): one value, the box's largest extent in yalms, that
scales width, height and depth together keeping their ratios. It saves nothing of its own: its value always comes
from the current extents, so moving a width/height/depth slider or a gizmo handle moves it too.

```csharp
public struct ZoomSlider
```


## Remarks

<p>
While the slider is dragged, every value is applied to the size the drag started from (see [Drag](Cube.Core.ZoomSlider.md#Cube_Core_ZoomSlider_Drag)),
not to the previous frame's result. Dragging back and forth therefore never lets rounding build up: returning to
the starting value gives exactly the starting box.
</p>
<p>
The value and range are cached against the size they were worked out for and only recomputed when it changes, so
an open settings window does no work for the slider on frames where nothing moves. A plain struct with no
allocations; the window keeps one as a field.
</p>

## Fields

### <a id="Cube_Core_ZoomSlider_cachedFor"></a> cachedFor

The size [Value](Cube.Core.ZoomSlider.md#Cube_Core_ZoomSlider_Value), [Low](Cube.Core.ZoomSlider.md#Cube_Core_ZoomSlider_Low) and [High](Cube.Core.ZoomSlider.md#Cube_Core_ZoomSlider_High) were worked out for.

```csharp
private Vector3 cachedFor
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_ZoomSlider_dragBase"></a> dragBase

The size a drag started from; every value of the drag scales this, see [Drag](Cube.Core.ZoomSlider.md#Cube_Core_ZoomSlider_Drag).

```csharp
private Vector3 dragBase
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_ZoomSlider_dragging"></a> dragging

Whether a drag is in progress, so [dragBase](Cube.Core.ZoomSlider.md#Cube_Core_ZoomSlider_dragBase) holds its starting size.

```csharp
private bool dragging
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Properties

### <a id="Cube_Core_ZoomSlider_High"></a> High

The slider's upper limit: where the largest extent would be 100.

```csharp
public float High { readonly get; private set; }
```

#### Property Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_ZoomSlider_Low"></a> Low

The slider's lower limit: where the smallest extent would be 10 ([ZoomRange](Cube.Core.BoxExtents.md#Cube_Core_BoxExtents_ZoomRange)).

```csharp
public float Low { readonly get; private set; }
```

#### Property Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_ZoomSlider_Value"></a> Value

The slider's value for the last size given to [Update](Cube.Core.ZoomSlider.md#Cube_Core_ZoomSlider_Update): its largest extent.

```csharp
public float Value { readonly get; private set; }
```

#### Property Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

## Methods

### <a id="Cube_Core_ZoomSlider_Drag_System_Numerics_Vector3_System_Single_"></a> Drag\(Vector3, float\)

Applies a slider value: the size whose largest extent is <code class="paramref">value</code>, with the ratios of the size
the drag started from. The first call of a drag records <code class="paramref">currentSize</code> as that start.

```csharp
public Vector3 Drag(Vector3 currentSize, float value)
```

#### Parameters

`currentSize` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The cube's size now; used only on the first call of a drag.

`value` [float](https://learn.microsoft.com/dotnet/api/system.single)

The slider's new value, the wanted largest extent in yalms.

#### Returns

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The new size, clamped so no extent leaves 10–100.

### <a id="Cube_Core_ZoomSlider_EndDrag"></a> EndDrag\(\)

Ends a drag, so the next [Drag](Cube.Core.ZoomSlider.md#Cube_Core_ZoomSlider_Drag) starts from the size at that point.

```csharp
public void EndDrag()
```

### <a id="Cube_Core_ZoomSlider_Update_System_Numerics_Vector3_"></a> Update\(Vector3\)

Brings [Value](Cube.Core.ZoomSlider.md#Cube_Core_ZoomSlider_Value), [Low](Cube.Core.ZoomSlider.md#Cube_Core_ZoomSlider_Low) and [High](Cube.Core.ZoomSlider.md#Cube_Core_ZoomSlider_High) up to date with the current size, working
them out again only if it changed since the last call.

```csharp
public void Update(Vector3 size)
```

#### Parameters

`size` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The cube's current width, height and depth.
