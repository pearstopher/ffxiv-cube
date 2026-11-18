# <a id="Cube_Core_OrientationMode"></a> Enum OrientationMode

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

How the cube is aligned, and so what happens to its contents (PROJECT.md, "Cube": the orientation setting). The
cube's frame stays still on screen in both modes.

```csharp
public enum OrientationMode
```

## Fields

`Camera = 0` 

"Rotate contents with camera", the default: the cube is aligned to the camera's horizontal direction, so
"forward in the cube" matches the game view and the contents rotate around the player as the camera turns.



`NorthUp = 1` 

"Keep contents fixed (north stays put)": the cube is aligned to the world with north at the front.
