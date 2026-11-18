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

First point (pixels); a polygon's first corner, for messages.

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

### <a id="Cube_Core_DrawPrimitive_Corners"></a> Corners

A polygon's corner count ([Polygon](Cube.Core.PrimitiveKind.md#Cube_Core_PrimitiveKind_Polygon)); 0 for other kinds. A byte, so with
[FirstPoint](Cube.Core.DrawPrimitive.md#Cube_Core_DrawPrimitive_FirstPoint) it fits in the padding after [Kind](Cube.Core.DrawPrimitive.md#Cube_Core_DrawPrimitive_Kind): the struct stays the size it was and
sorting moves no extra bytes.

```csharp
public byte Corners
```

#### Field Value

 [byte](https://learn.microsoft.com/dotnet/api/system.byte)

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

### <a id="Cube_Core_DrawPrimitive_FirstPoint"></a> FirstPoint

Where a polygon's corners start in the draw list's point buffer ([Points](Cube.Core.DrawList.md#Cube_Core_DrawList_Points)); 16 bits, so the
buffer holds at most [MaxPointCapacity](Cube.Core.DrawList.md#Cube_Core_DrawList_MaxPointCapacity) corners.

```csharp
public ushort FirstPoint
```

#### Field Value

 [ushort](https://learn.microsoft.com/dotnet/api/system.uint16)

### <a id="Cube_Core_DrawPrimitive_Kind"></a> Kind

Line, quad, label or polygon.

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

Which thing the primitive belongs to: 0 for the scene (the big cube), a positive number for the index of the
player in this frame's [PlayerSet](Cube.Core.PlayerSet.md#Cube_Core_PlayerSet) plus one, and a negative number for a collision box
([TagFor](Cube.Core.CollisionCubes.md#Cube_Core_CollisionCubes_TagFor)). Lets the bridge check each player's cube and each box, and labels find
their player.

```csharp
public int Tag
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)
