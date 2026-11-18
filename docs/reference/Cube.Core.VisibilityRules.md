# <a id="Cube_Core_VisibilityRules"></a> Class VisibilityRules

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

Decides whether to draw (PROJECT.md, "When hidden": not logged in, loading screens, cutscenes, GPose and PvP
areas; the window is just empty then, never an error).

```csharp
public static class VisibilityRules
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[VisibilityRules](Cube.Core.VisibilityRules.md)


## Remarks

When hidden, the window skips everything else too: no player reading, no geometry (PROJECT.md, "Efficiency and
memory": do work only when needed). If several apply, the first in the order below is reported.

## Methods

### <a id="Cube_Core_VisibilityRules_For_Cube_Core_GameState__"></a> For\(in GameState\)

Whether the cube is drawn in this state, and if not, why.

```csharp
public static Visibility For(in GameState state)
```

#### Parameters

`state` [GameState](Cube.Core.GameState.md)

This frame's game state.

#### Returns

 [Visibility](Cube.Core.Visibility.md)
