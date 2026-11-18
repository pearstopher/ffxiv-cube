# <a id="Cube_TestBridge_Checks_WorstCase"></a> Class WorstCase

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

The most expensive settings and contents the cube can have, shared by the budget and soak checks so both measure
the same thing (PROJECT.md, "Efficiency and memory": the phase 2 budgets hold with collision on; #33).

```csharp
internal static class WorstCase
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[WorstCase](Cube.TestBridge.Checks.WorstCase.md)


## Remarks

Labels and NPCs on, the full crowd of fakes at the player and NPC caps (#23, #74), and collision on at full detail
in the largest cube (#32), whatever the user chose. Settings change in memory only; each check saves them first with
[CopyFrom](Cube.Core.CubeSettings.md#Cube_Core_CubeSettings_CopyFrom) and restores them in its cleanup.

## Methods

### <a id="Cube_TestBridge_Checks_WorstCase_Apply_Cube_CubeWindow_"></a> Apply\(CubeWindow\)

Switches the window to the worst case.

```csharp
public static void Apply(CubeWindow window)
```

#### Parameters

`window` [CubeWindow](Cube.CubeWindow.md)

The cube window.

### <a id="Cube_TestBridge_Checks_WorstCase_WaitForCollision_Cube_Core_CollisionCache_"></a> WaitForCollision\(CollisionCache\)

Starts a fresh collision gather for the worst-case settings and waits until it has completed, so measuring
starts with the boxes drawn and gathering idle (gathering frames are checked by [CollisionReaderCheck](Cube.TestBridge.Checks.CollisionReaderCheck.md#Cube_TestBridge_Checks_CollisionReaderCheck)).

```csharp
public static IEnumerable<CheckStep> WaitForCollision(CollisionCache cache)
```

#### Parameters

`cache` [CollisionCache](Cube.Core.CollisionCache.md)

The collision cache.

#### Returns

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[CheckStep](Cube.TestBridge.CheckStep.md)\>
