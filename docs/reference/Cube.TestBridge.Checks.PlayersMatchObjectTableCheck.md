# <a id="Cube_TestBridge_Checks_PlayersMatchObjectTableCheck"></a> Class PlayersMatchObjectTableCheck

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

Checks the players the window found match an independent read of Dalamud's object table (PROJECT.md,
"Automated testing": the players drawn match an independent read of the object table; "Players": every player
character inside the cube, nobody outside; "Colours": self, party/alliance, other).

```csharp
internal sealed class PlayersMatchObjectTableCheck : BridgeCheck
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BridgeCheck](Cube.TestBridge.BridgeCheck.md) ← 
[PlayersMatchObjectTableCheck](Cube.TestBridge.Checks.PlayersMatchObjectTableCheck.md)


## Remarks

The window reads FFXIVClientStructs directly (allocation-free); this check reads Dalamud's
`Dalamud.Plugin.Services.IObjectTable` and its `Dalamud.Game.ClientState.Objects.Enums.StatusFlags` instead, so a mistake in either shows up as a
difference. Both are taken within one frame of each other, so players within half a yalm of a face may cross it
in between; they are ignored. Fake players (Debug, #16) are left out of the comparison.

## Constructors

### <a id="Cube_TestBridge_Checks_PlayersMatchObjectTableCheck__ctor_Dalamud_Plugin_Services_IObjectTable_"></a> PlayersMatchObjectTableCheck\(IObjectTable\)

Checks the players the window found match an independent read of Dalamud's object table (PROJECT.md,
"Automated testing": the players drawn match an independent read of the object table; "Players": every player
character inside the cube, nobody outside; "Colours": self, party/alliance, other).

```csharp
public PlayersMatchObjectTableCheck(IObjectTable objects)
```

#### Parameters

`objects` [IObjectTable](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Plugin/Services/IObjectTable.cs)

Dalamud's object table.

#### Remarks

The window reads FFXIVClientStructs directly (allocation-free); this check reads Dalamud's
`Dalamud.Plugin.Services.IObjectTable` and its `Dalamud.Game.ClientState.Objects.Enums.StatusFlags` instead, so a mistake in either shows up as a
difference. Both are taken within one frame of each other, so players within half a yalm of a face may cross it
in between; they are ignored. Fake players (Debug, #16) are left out of the comparison.

## Fields

### <a id="Cube_TestBridge_Checks_PlayersMatchObjectTableCheck_BoundaryTolerance"></a> BoundaryTolerance

Players this close to a face may legitimately differ between the two reads.

```csharp
private const float BoundaryTolerance = 0.5
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_PlayersMatchObjectTableCheck_FramesToWait"></a> FramesToWait

Frames to wait for the window to draw after opening it.

```csharp
private const int FramesToWait = 30
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_PlayersMatchObjectTableCheck_wasOpen"></a> wasOpen

The window's open state before the check, restored by [Cleanup](Cube.TestBridge.Checks.PlayersMatchObjectTableCheck.md#Cube_TestBridge_Checks_PlayersMatchObjectTableCheck_Cleanup).

```csharp
private bool wasOpen
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Properties

### <a id="Cube_TestBridge_Checks_PlayersMatchObjectTableCheck_Name"></a> Name

Short unique name, shown in the results, e.g. <code>window-opens-and-closes</code>.

```csharp
public override string Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

## Methods

### <a id="Cube_TestBridge_Checks_PlayersMatchObjectTableCheck_Cleanup_Cube_TestBridge_BridgeContext_"></a> Cleanup\(BridgeContext\)

Puts back anything the check changed (e.g. the window's open state). Called after every run,
including when the check failed, threw or timed out, so a check never leaves the plugin altered.

```csharp
public override void Cleanup(BridgeContext context)
```

#### Parameters

`context` [BridgeContext](Cube.TestBridge.BridgeContext.md)

The same context the check ran with.

### <a id="Cube_TestBridge_Checks_PlayersMatchObjectTableCheck_Compare_Cube_TestBridge_BridgeContext_Cube_CubeWindow_"></a> Compare\(BridgeContext, CubeWindow\)

Compares the window's players with the object table, one frame's worth.

```csharp
private CheckStep Compare(BridgeContext context, CubeWindow window)
```

#### Parameters

`context` [BridgeContext](Cube.TestBridge.BridgeContext.md)

For metrics.

`window` [CubeWindow](Cube.CubeWindow.md)

The cube window.

#### Returns

 [CheckStep](Cube.TestBridge.CheckStep.md)

### <a id="Cube_TestBridge_Checks_PlayersMatchObjectTableCheck_Run_Cube_TestBridge_BridgeContext_"></a> Run\(BridgeContext\)

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

### <a id="Cube_TestBridge_Checks_PlayersMatchObjectTableCheck_Within_Cube_Core_CubeSpace__System_Numerics_Vector3_System_Single_"></a> Within\(in CubeSpace, Vector3, float\)

Whether a point is within a given half-size of the cube's centre on every axis (unclamped, unlike [CubeSpace](Cube.Core.CubeSpace.md#Cube_Core_CubeSpace)).

```csharp
private static bool Within(in CubeSpace space, Vector3 point, float half)
```

#### Parameters

`space` [CubeSpace](Cube.Core.CubeSpace.md)

The cube.

`point` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

A world position.

`half` [float](https://learn.microsoft.com/dotnet/api/system.single)

Half-size to test against.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)
