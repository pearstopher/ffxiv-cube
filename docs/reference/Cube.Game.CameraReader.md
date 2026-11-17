# <a id="Cube_Game_CameraReader"></a> Class CameraReader

Namespace: [Cube.Game](Cube.Game.md)  
Assembly: Cube.dll  

Reads which way the game camera is looking, for the Camera orientation mode (PROJECT.md, "View": the view's
yaw follows the game camera so "forward on screen" matches the game view).

```csharp
internal static class CameraReader
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[CameraReader](Cube.Game.CameraReader.md)


## Remarks

Uses the scene camera's look-at point minus its position, a world-space direction with no angle convention
to guess (<code>docs/research-notes.md</code>, #10). A plain memory read on the framework/draw thread: no hooks,
no game functions called, no allocation.

## Methods

### <a id="Cube_Game_CameraReader_TryGetForward_System_Numerics_Vector3__"></a> TryGetForward\(out Vector3\)

Gets the camera's world-space forward direction (not normalised).

```csharp
public static bool TryGetForward(out Vector3 forward)
```

#### Parameters

`forward` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The direction, if a camera exists.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

False when there is no game camera (e.g. not logged in); the caller keeps its previous yaw.
