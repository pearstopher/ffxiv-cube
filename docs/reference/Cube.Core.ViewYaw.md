# <a id="Cube_Core_ViewYaw"></a> Class ViewYaw

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

Chooses the yaw of the view around the cube (PROJECT.md, "View"): in <b>Camera</b> mode it follows the
game camera so "forward on screen" matches the game view; in <b>North up</b> mode it is fixed with
north at the back.

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

### <a id="Cube_Core_ViewYaw_MinHorizontalLength"></a> MinHorizontalLength

Below this horizontal length the camera is looking (almost) straight up or down and its heading is
meaningless, so the previous yaw is kept rather than spinning the view.

```csharp
public const float MinHorizontalLength = 0.0001
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_ViewYaw_NorthUp"></a> NorthUp

The yaw for north-up mode: north at the back of the view.

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
