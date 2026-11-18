# <a id="Cube_Core_BoxExtents"></a> Class BoxExtents

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

The rules for the cube's size (PROJECT.md, "Exact behaviour rules", Cube; changed 2026-11-18, #90): the cube is a
box with three extents in yalms, width (x), height (y) and depth (z), each 10–100, default 40. Zoom scales all three
together, keeping their ratios.

```csharp
public static class BoxExtents
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BoxExtents](Cube.Core.BoxExtents.md)


## Remarks

<p>
A size is a `System.Numerics.Vector3` of full extents in the cube's own axes: X across (towards the right face), Y up,
Z front to back. Keeping it a plain vector lets the settings window bind a slider to each component and the JSON
file store it like the colours.
</p>
<p>
Every reader goes through [Clamp](Cube.Core.BoxExtents.md#Cube_Core_BoxExtents_Clamp), so a corrupt or hand-edited value can never produce an empty
or infinite box. Pure maths, allocation-free, unit tested without the game.
</p>

## Fields

### <a id="Cube_Core_BoxExtents_Default"></a> Default

Default extent on every axis, in yalms (PROJECT.md: default 40).

```csharp
public const float Default = 40
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_BoxExtents_Max"></a> Max

Largest allowed extent, in yalms (PROJECT.md: 10–100).

```csharp
public const float Max = 100
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_BoxExtents_Min"></a> Min

Smallest allowed extent, in yalms (PROJECT.md: 10–100).

```csharp
public const float Min = 10
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

## Properties

### <a id="Cube_Core_BoxExtents_DefaultSize"></a> DefaultSize

The default size: a 40 × 40 × 40 cube (PROJECT.md; "reset to default gives 40×40×40").

```csharp
public static Vector3 DefaultSize { get; }
```

#### Property Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

## Methods

### <a id="Cube_Core_BoxExtents_Clamp_System_Single_"></a> Clamp\(float\)

Clamps one extent to [Min](Cube.Core.BoxExtents.md#Cube_Core_BoxExtents_Min)–[Max](Cube.Core.BoxExtents.md#Cube_Core_BoxExtents_Max); a non-number becomes [Default](Cube.Core.BoxExtents.md#Cube_Core_BoxExtents_Default).

```csharp
public static float Clamp(float extent)
```

#### Parameters

`extent` [float](https://learn.microsoft.com/dotnet/api/system.single)

The requested extent in yalms.

#### Returns

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_BoxExtents_Clamp_System_Numerics_Vector3_"></a> Clamp\(Vector3\)

Clamps each extent of a size separately (see [Clamp](Cube.Core.BoxExtents.md#Cube_Core_BoxExtents_Clamp)).

```csharp
public static Vector3 Clamp(Vector3 size)
```

#### Parameters

`size` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The requested width, height and depth.

#### Returns

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_BoxExtents_Largest_System_Numerics_Vector3_"></a> Largest\(Vector3\)

The largest of a size's three extents; marker sizes and the on-screen fit follow it.

```csharp
public static float Largest(Vector3 size)
```

#### Parameters

`size` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

A size, already clamped.

#### Returns

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_BoxExtents_Smallest_System_Numerics_Vector3_"></a> Smallest\(Vector3\)

The smallest of a size's three extents; it decides how far the box can zoom in.

```csharp
public static float Smallest(Vector3 size)
```

#### Parameters

`size` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

A size, already clamped.

#### Returns

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_BoxExtents_Zoom_System_Numerics_Vector3_System_Single_"></a> Zoom\(Vector3, float\)

Zooms a size: multiplies all three extents by the same factor, so the box keeps its proportions (PROJECT.md:
"Zoom scales all three extents together, keeping their ratios"). A factor below 1 zooms in (less space, so the
contents look bigger).

```csharp
public static Vector3 Zoom(Vector3 size, float factor)
```

#### Parameters

`size` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The current size; clamped first.

`factor` [float](https://learn.microsoft.com/dotnet/api/system.single)

How much to scale every extent by.

#### Returns

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

#### Remarks

The factor itself is clamped, not each extent, so no extent leaves 10–100 and the ratios still hold: zooming
out a 20 × 80 × 40 box by 2 stops at 25 × 100 × 50. A factor that isn't a positive number leaves the size as
it is.

### <a id="Cube_Core_BoxExtents_ZoomRange_System_Numerics_Vector3_"></a> ZoomRange\(Vector3\)

The zoom slider's limits for a box of this shape (#94): from where the smallest extent hits [Min](Cube.Core.BoxExtents.md#Cube_Core_BoxExtents_Min) to
where the largest hits [Max](Cube.Core.BoxExtents.md#Cube_Core_BoxExtents_Max), given as the largest extent (the slider's value).

```csharp
public static (float Low, float High) ZoomRange(Vector3 size)
```

#### Parameters

`size` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The current size; clamped first.

#### Returns

 \([float](https://learn.microsoft.com/dotnet/api/system.single) [Low](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.single,system.single\-.low), [float](https://learn.microsoft.com/dotnet/api/system.single) [High](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.single,system.single\-.high)\)

#### Remarks

Only the shape matters, so the range stays the same while zooming. A 10 × 100 box can't zoom at all, and both
limits are 100.

### <a id="Cube_Core_BoxExtents_ZoomTo_System_Numerics_Vector3_System_Single_"></a> ZoomTo\(Vector3, float\)

Zooms a size so its largest extent becomes <code class="paramref">largest</code>, keeping the ratios: the zoom slider's
value turned back into extents (#94). Clamped like [Zoom](Cube.Core.BoxExtents.md#Cube_Core_BoxExtents_Zoom), so a value outside
[ZoomRange](Cube.Core.BoxExtents.md#Cube_Core_BoxExtents_ZoomRange) stops at the limit.

```csharp
public static Vector3 ZoomTo(Vector3 size, float largest)
```

#### Parameters

`size` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The size to scale; clamped first.

`largest` [float](https://learn.microsoft.com/dotnet/api/system.single)

The wanted largest extent, in yalms.

#### Returns

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)
