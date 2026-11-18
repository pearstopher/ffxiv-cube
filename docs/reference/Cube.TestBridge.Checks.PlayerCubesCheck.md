# <a id="Cube_TestBridge_Checks_PlayerCubesCheck"></a> Class PlayerCubesCheck

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

Checks every player and named NPC in the cube is drawn as a cube in their relation's colour (PROJECT.md, "Players",
"NPCs" and "Colours"; "Draw order": far first), using the standard fake players and fake NPC (#16, #74) so it works
with nobody around.

```csharp
internal sealed class PlayerCubesCheck : BridgeCheck
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BridgeCheck](Cube.TestBridge.BridgeCheck.md) ← 
[PlayerCubesCheck](Cube.TestBridge.Checks.PlayerCubesCheck.md)


## Remarks

Each player's faces carry the player's index + 1 as their [Tag](Cube.Core.DrawPrimitive.md#Cube_Core_DrawPrimitive_Tag). For each player the
check expects one to three visible faces, every one a shade of the palette colour for the player's relation, and
the whole list in far-to-near order.

## Fields

### <a id="Cube_TestBridge_Checks_PlayerCubesCheck_FramesToWait"></a> FramesToWait

Frames to wait for the window to draw.

```csharp
private const int FramesToWait = 30
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_PlayerCubesCheck_wasOpen"></a> wasOpen

The window's open state before the check.

```csharp
private bool wasOpen
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Properties

### <a id="Cube_TestBridge_Checks_PlayerCubesCheck_Name"></a> Name

Short unique name, shown in the results, e.g. <code>window-opens-and-closes</code>.

```csharp
public override string Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

## Methods

### <a id="Cube_TestBridge_Checks_PlayerCubesCheck_Cleanup_Cube_TestBridge_BridgeContext_"></a> Cleanup\(BridgeContext\)

Puts back anything the check changed (e.g. the window's open state). Called after every run,
including when the check failed, threw or timed out, so a check never leaves the plugin altered.

```csharp
public override void Cleanup(BridgeContext context)
```

#### Parameters

`context` [BridgeContext](Cube.TestBridge.BridgeContext.md)

The same context the check ran with.

### <a id="Cube_TestBridge_Checks_PlayerCubesCheck_IsShadeOf_System_UInt32_System_UInt32_"></a> IsShadeOf\(uint, uint\)

Whether a face colour is one of the cube shades of a base colour.

```csharp
private static bool IsShadeOf(uint face, uint baseColour)
```

#### Parameters

`face` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

The face's colour.

`baseColour` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

The player's palette colour.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_TestBridge_Checks_PlayerCubesCheck_Run_Cube_TestBridge_BridgeContext_"></a> Run\(BridgeContext\)

Runs the check, one step per frame, on the framework thread. Yield [NextFrame](Cube.TestBridge.CheckStep.md#Cube_TestBridge_CheckStep_NextFrame)
to wait a frame; finish with [Pass](Cube.TestBridge.CheckStep.md#Cube_TestBridge_CheckStep_Pass) or [Fail](Cube.TestBridge.CheckStep.md#Cube_TestBridge_CheckStep_Fail).

```csharp
public override IEnumerable<CheckStep> Run(BridgeContext context)
```

#### Parameters

`context` [BridgeContext](Cube.TestBridge.BridgeContext.md)

The plugin parts the check can look at, and where it records metrics.

#### Returns

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[CheckStep](Cube.TestBridge.CheckStep.md)\>

### <a id="Cube_TestBridge_Checks_PlayerCubesCheck_Verify_Cube_TestBridge_BridgeContext_Cube_CubeWindow_"></a> Verify\(BridgeContext, CubeWindow\)

Checks the last frame's player cubes.

```csharp
private static CheckStep Verify(BridgeContext context, CubeWindow window)
```

#### Parameters

`context` [BridgeContext](Cube.TestBridge.BridgeContext.md)

For metrics.

`window` [CubeWindow](Cube.CubeWindow.md)

The cube window.

#### Returns

 [CheckStep](Cube.TestBridge.CheckStep.md)
