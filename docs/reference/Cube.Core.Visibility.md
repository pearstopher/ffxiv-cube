# <a id="Cube_Core_Visibility"></a> Enum Visibility

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

Whether the cube is drawn, and if not, why (PROJECT.md, "When hidden"). Decided by [VisibilityRules](Cube.Core.VisibilityRules.md#Cube_Core_VisibilityRules).

```csharp
public enum Visibility
```

## Fields

`Shown = 0` 

Drawn normally.



`NotLoggedIn = 1` 

Not logged in, or no character yet: nothing to centre the cube on.



`Loading = 2` 

A loading screen is up.



`Cutscene = 3` 

A cutscene is playing.



`GPose = 4` 

Group pose is active.
