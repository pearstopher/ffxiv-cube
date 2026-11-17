# <a id="Cube_Core_CubeSpace"></a> Struct CubeSpace

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

The cube of space around the local player (PROJECT.md, "Exact behaviour rules", Cube): axis-aligned to
the world, centred on the player, with an edge of 10–100 yalms (default 40). Answers "is this point
inside?" and turns world positions into cube-local view coordinates.

```csharp
public readonly struct CubeSpace
```


## Remarks

<p>
World axes follow the game (see <code>docs/research-notes.md</code>, #10): X east, Y up, Z south, so north is −Z.
The cube itself never rotates; only the view around it does, by [Yaw](Cube.Core.CubeSpace.md#Cube_Core_CubeSpace_Yaw) (see [ViewYaw](Cube.Core.ViewYaw.md#Cube_Core_ViewYaw)).
</p>
<p>
View coordinates are in yalms relative to the centre: <code>X</code> to the right on screen, <code>Y</code> up,
<code>Z</code> away from the viewer ("forward"). With yaw 0 (north up), right is east and forward is north.
</p>
<p>
A small immutable struct, so it is rebuilt each frame from the current inputs at no allocation cost.
Points are culled ([TryToView](Cube.Core.CubeSpace.md#Cube_Core_CubeSpace_TryToView)) before anything else is computed for them (PROJECT.md,
"Efficiency": cull before projecting).
</p>

## Constructors

### <a id="Cube_Core_CubeSpace__ctor_System_Numerics_Vector3_System_Single_System_Single_"></a> CubeSpace\(Vector3, float, float\)

Creates the cube around a centre.

```csharp
public CubeSpace(Vector3 centre, float edge, float yaw)
```

#### Parameters

`centre` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The local player's world position.

`edge` [float](https://learn.microsoft.com/dotnet/api/system.single)

Edge length in yalms; clamped to [MinEdge](Cube.Core.CubeSpace.md#Cube_Core_CubeSpace_MinEdge)–[MaxEdge](Cube.Core.CubeSpace.md#Cube_Core_CubeSpace_MaxEdge).

`yaw` [float](https://learn.microsoft.com/dotnet/api/system.single)

The view's yaw in radians (see [ViewYaw](Cube.Core.ViewYaw.md#Cube_Core_ViewYaw)): 0 looks north, increasing clockwise seen from above.

### <a id="Cube_Core_CubeSpace__ctor_Cube_Core_CubeSpace__System_Numerics_Vector3_System_Single_"></a> CubeSpace\(in CubeSpace, Vector3, float\)

Copies a cube with a new centre and edge, keeping its yaw and rotation.

```csharp
private CubeSpace(in CubeSpace source, Vector3 centre, float edge)
```

#### Parameters

`source` [CubeSpace](Cube.Core.CubeSpace.md)

The cube whose yaw (and cached sine and cosine) to keep.

`centre` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The new centre.

`edge` [float](https://learn.microsoft.com/dotnet/api/system.single)

The new edge; clamped like the main constructor.

## Fields

### <a id="Cube_Core_CubeSpace_DefaultEdge"></a> DefaultEdge

Default cube edge in yalms (PROJECT.md).

```csharp
public const float DefaultEdge = 40
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_CubeSpace_MaxEdge"></a> MaxEdge

Largest allowed cube edge in yalms (PROJECT.md: setting 10–100).

```csharp
public const float MaxEdge = 100
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_CubeSpace_MinEdge"></a> MinEdge

Smallest allowed cube edge in yalms (PROJECT.md: setting 10–100).

```csharp
public const float MinEdge = 10
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_CubeSpace_cos"></a> cos

Cosine of [Yaw](Cube.Core.CubeSpace.md#Cube_Core_CubeSpace_Yaw), cached so each point costs a few multiplies.

```csharp
private readonly float cos
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_CubeSpace_sin"></a> sin

Sine of [Yaw](Cube.Core.CubeSpace.md#Cube_Core_CubeSpace_Yaw).

```csharp
private readonly float sin
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

## Properties

### <a id="Cube_Core_CubeSpace_Centre"></a> Centre

The cube's centre in world coordinates: the local player's position.

```csharp
public Vector3 Centre { get; }
```

#### Property Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_CubeSpace_Edge"></a> Edge

Edge length in yalms, already clamped.

```csharp
public float Edge { get; }
```

#### Property Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_CubeSpace_HalfEdge"></a> HalfEdge

Half the edge: the distance from the centre to each face.

```csharp
public float HalfEdge { get; }
```

#### Property Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_CubeSpace_Yaw"></a> Yaw

The view's yaw in radians: 0 looks north, increasing clockwise seen from above.

```csharp
public float Yaw { get; }
```

#### Property Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

## Methods

### <a id="Cube_Core_CubeSpace_ClampEdge_System_Single_"></a> ClampEdge\(float\)

Clamps an edge length to the allowed range; a non-number becomes the default, so a corrupt
setting can never produce an empty or infinite cube.

```csharp
public static float ClampEdge(float edge)
```

#### Parameters

`edge` [float](https://learn.microsoft.com/dotnet/api/system.single)

The requested edge in yalms.

#### Returns

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_CubeSpace_Contains_System_Numerics_Vector3_"></a> Contains\(Vector3\)

Whether a world position is inside the cube. Inclusive: a point exactly on a face, edge or corner
counts as inside.

```csharp
public bool Contains(Vector3 world)
```

#### Parameters

`world` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

A world position.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_CubeSpace_Recentred_System_Numerics_Vector3_System_Single_"></a> Recentred\(Vector3, float\)

The same view rotation around a new centre and edge, without recomputing the sine and cosine. The window
uses it every frame the yaw hasn't changed (PROJECT.md, "Efficiency and memory": recompute only when inputs
change).

```csharp
public CubeSpace Recentred(Vector3 centre, float edge)
```

#### Parameters

`centre` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The new centre.

`edge` [float](https://learn.microsoft.com/dotnet/api/system.single)

The new edge in yalms.

#### Returns

 [CubeSpace](Cube.Core.CubeSpace.md)

### <a id="Cube_Core_CubeSpace_RotateOffset_System_Numerics_Vector3_"></a> RotateOffset\(Vector3\)

Rotates an offset from the centre (world axes) into view axes.

```csharp
public Vector3 RotateOffset(Vector3 offset)
```

#### Parameters

`offset` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

An offset in world axes.

#### Returns

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

#### Remarks

With forward = (sin yaw, −cos yaw) and right = (cos yaw, sin yaw) in world (X, Z): yaw 0 gives
right = east (+X), forward = north (−Z).

### <a id="Cube_Core_CubeSpace_ToView_System_Numerics_Vector3_"></a> ToView\(Vector3\)

Turns a world position into view coordinates (right, up, forward), without culling. Used for the
cube's own corners and for points already known to be inside.

```csharp
public Vector3 ToView(Vector3 world)
```

#### Parameters

`world` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

A world position.

#### Returns

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_CubeSpace_TryToView_System_Numerics_Vector3_System_Numerics_Vector3__"></a> TryToView\(Vector3, out Vector3\)

Culls then transforms: returns false for a point outside the cube (players outside aren't shown at all,
PROJECT.md "Players"), otherwise gives its view coordinates.

```csharp
public bool TryToView(Vector3 world, out Vector3 view)
```

#### Parameters

`world` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

A world position.

`view` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The position in view coordinates, if inside.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)
