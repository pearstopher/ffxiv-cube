# <a id="Cube_Core_ViewYaw"></a> Class ViewYaw

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

Chooses the cube's alignment yaw (PROJECT.md, "Cube"): in <b>Camera</b> mode it follows the game camera's
horizontal direction so "forward in the cube" matches the game view; in <b>North up</b> mode it is fixed with
north at the front. The yaw turns the contents, never the wireframe (see [CubeSpace](Cube.Core.CubeSpace.md#Cube_Core_CubeSpace)).

```csharp
public static class ViewYaw
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ViewYaw](Cube.Core.ViewYaw.md)


## Remarks

Yaw convention: 0 looks north (−Z), increasing clockwise seen from above (towards east, +X). The camera's
direction comes from the scene camera's look-at point minus its position (<code>docs/research-notes.md</code>, #10),
so no game angle convention is assumed.

## Fields

### <a id="Cube_Core_ViewYaw_ChangeThreshold"></a> ChangeThreshold

Smallest yaw change, in radians (about 0.1°), that turns the contents. Smaller changes, such as camera jitter or
float noise while the camera is still, keep the previous yaw so the rotation isn't recomputed (PROJECT.md,
"Efficiency and memory": recompute only when inputs change). Far below what a pixel shows at any window size.

```csharp
public const float ChangeThreshold = 0.002
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_ViewYaw_MinHorizontalLength"></a> MinHorizontalLength

Below this horizontal length the camera is looking (almost) straight up or down and its heading is
meaningless, so the previous yaw is kept rather than spinning the contents.

```csharp
public const float MinHorizontalLength = 0.0001
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_ViewYaw_NorthUp"></a> NorthUp

The yaw for north-up mode: the cube's front face points north.

```csharp
public const float NorthUp = 0
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

## Methods

### <a id="Cube_Core_ViewYaw_For_Cube_Core_OrientationMode_System_Numerics_Vector3_System_Single_"></a> For\(OrientationMode, Vector3, float\)

The yaw for the given mode.

```csharp
public static float For(OrientationMode mode, Vector3 cameraForward, float previous)
```

#### Parameters

`mode` [OrientationMode](Cube.Core.OrientationMode.md)

Camera or north up.

`cameraForward` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The game camera's world direction (used in Camera mode).

`previous` [float](https://learn.microsoft.com/dotnet/api/system.single)

The last yaw, kept if the camera direction is vertical.

#### Returns

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_ViewYaw_FromForward_System_Numerics_Vector3_System_Single_"></a> FromForward\(Vector3, float\)

The yaw that looks along a world direction, using only its horizontal part.

```csharp
public static float FromForward(Vector3 forward, float previous)
```

#### Parameters

`forward` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

A world direction, e.g. camera look-at minus camera position.

`previous` [float](https://learn.microsoft.com/dotnet/api/system.single)

The yaw to keep if the direction is (almost) vertical.

#### Returns

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_ViewYaw_Settle_System_Single_System_Single_"></a> Settle\(float, float\)

Keeps the previous yaw unless the new one differs by at least [ChangeThreshold](Cube.Core.ViewYaw.md#Cube_Core_ViewYaw_ChangeThreshold), measured the
short way round (so −π and π count as equal).

```csharp
public static float Settle(float previous, float next)
```

#### Parameters

`previous` [float](https://learn.microsoft.com/dotnet/api/system.single)

The yaw in use.

`next` [float](https://learn.microsoft.com/dotnet/api/system.single)

This frame's wanted yaw.

#### Returns

 [float](https://learn.microsoft.com/dotnet/api/system.single)

<code class="paramref">previous</code> or <code class="paramref">next</code>.
