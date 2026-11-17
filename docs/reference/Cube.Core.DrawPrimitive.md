# <a id="Cube_Core_DrawPrimitive"></a> Struct DrawPrimitive

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

One thing to draw, already projected to window pixels: the plugin turns each into one ImGui call
(PROJECT.md, "Cheap drawing": the fewest primitives that do the job).

```csharp
public struct DrawPrimitive
```


## Remarks

A plain struct kept in [DrawList](Cube.Core.DrawList.md#Cube_Core_DrawList)'s reused array, so building a frame allocates nothing.

## Fields

### <a id="Cube_Core_DrawPrimitive_A"></a> A

First point (pixels).

```csharp
public Vector2 A
```

#### Field Value

 [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

### <a id="Cube_Core_DrawPrimitive_B"></a> B

Second point (pixels).

```csharp
public Vector2 B
```

#### Field Value

 [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

### <a id="Cube_Core_DrawPrimitive_C"></a> C

Third point of a quad (pixels).

```csharp
public Vector2 C
```

#### Field Value

 [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

### <a id="Cube_Core_DrawPrimitive_Colour"></a> Colour

Colour, packed as ImGui expects ([Colour](Cube.Core.DrawPrimitive.md#Cube_Core_DrawPrimitive_Colour)).

```csharp
public uint Colour
```

#### Field Value

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

### <a id="Cube_Core_DrawPrimitive_D"></a> D

Fourth point of a quad (pixels).

```csharp
public Vector2 D
```

#### Field Value

 [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

### <a id="Cube_Core_DrawPrimitive_Depth"></a> Depth

Distance from the viewer, for the painter's sort; larger is drawn first.

```csharp
public float Depth
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_DrawPrimitive_Kind"></a> Kind

Line, quad or label.

```csharp
public PrimitiveKind Kind
```

#### Field Value

 [PrimitiveKind](Cube.Core.PrimitiveKind.md)

### <a id="Cube_Core_DrawPrimitive_Sequence"></a> Sequence

Order added this frame; breaks depth ties so equal-depth primitives never swap and flicker.

```csharp
public int Sequence
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_DrawPrimitive_Tag"></a> Tag

Which thing the primitive belongs to: 0 for the scene (the big cube), otherwise the index of the player in
this frame's [PlayerSet](Cube.Core.PlayerSet.md#Cube_Core_PlayerSet) plus one. Lets the bridge check each player's cube, and labels find
their player.

```csharp
public int Tag
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)
