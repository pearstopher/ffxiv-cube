# <a id="Cube_TestBridge_Checks_JumpHoldCheck"></a> Class JumpHoldCheck

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

Checks "Jump" (#105; PROJECT.md, "Exact behaviour rules", Cube): with a simulated jump flag and a scripted height for
the local player (no game input, so no automation), the cube's height stays at the take-off height, a thing fixed in
the world doesn't move vertically in the cube or on screen, the player's marker rises and falls, x and z still
follow, the marker is kept at the top face when the jump leaves the box, and landing on a higher spot snaps the
cube to the new height in the first frame.

```csharp
internal sealed class JumpHoldCheck : BridgeCheck
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BridgeCheck](Cube.TestBridge.BridgeCheck.md) ← 
[JumpHoldCheck](Cube.TestBridge.Checks.JumpHoldCheck.md)


## Remarks

The real jump flag can't be tested here (the bridge can't make the character jump); that is the owner's manual
session in <code>docs/manual-tests.md</code> ("Jump"). Everything changed here is in memory and restored in
[Cleanup](Cube.TestBridge.Checks.JumpHoldCheck.md#Cube_TestBridge_Checks_JumpHoldCheck_Cleanup).

## Fields

### <a id="Cube_TestBridge_Checks_JumpHoldCheck_Path"></a> Path

The scripted rise and fall above take-off, in yalms.

```csharp
private static readonly float[] Path
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)\[\]

### <a id="Cube_TestBridge_Checks_JumpHoldCheck_Tolerance"></a> Tolerance

How far the held cube's centre or a fixed thing's view height may drift, in yalms (rounding only).

```csharp
private const float Tolerance = 0.01
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_JumpHoldCheck_saved"></a> saved

The user's settings before the check, restored by [Cleanup](Cube.TestBridge.Checks.JumpHoldCheck.md#Cube_TestBridge_Checks_JumpHoldCheck_Cleanup).

```csharp
private readonly CubeSettings saved
```

#### Field Value

 [CubeSettings](Cube.Core.CubeSettings.md)

### <a id="Cube_TestBridge_Checks_JumpHoldCheck_wasOpen"></a> wasOpen

The window's open state before the check, restored by [Cleanup](Cube.TestBridge.Checks.JumpHoldCheck.md#Cube_TestBridge_Checks_JumpHoldCheck_Cleanup).

```csharp
private bool wasOpen
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Properties

### <a id="Cube_TestBridge_Checks_JumpHoldCheck_Name"></a> Name

Short unique name, shown in the results, e.g. <code>window-opens-and-closes</code>.

```csharp
public override string Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Cube_TestBridge_Checks_JumpHoldCheck_Timeout"></a> Timeout

The longest the check may run before it fails as timed out.

```csharp
public override TimeSpan Timeout { get; }
```

#### Property Value

 [TimeSpan](https://learn.microsoft.com/dotnet/api/system.timespan)

#### Remarks

Ten seconds covers checks that wait a few hundred frames; override for longer ones.

## Methods

### <a id="Cube_TestBridge_Checks_JumpHoldCheck_Cleanup_Cube_TestBridge_BridgeContext_"></a> Cleanup\(BridgeContext\)

Puts back anything the check changed (e.g. the window's open state). Called after every run,
including when the check failed, threw or timed out, so a check never leaves the plugin altered.

```csharp
public override void Cleanup(BridgeContext context)
```

#### Parameters

`context` [BridgeContext](Cube.TestBridge.BridgeContext.md)

The same context the check ran with.

### <a id="Cube_TestBridge_Checks_JumpHoldCheck_Measure_Cube_CubeWindow_System_Numerics_Vector3_"></a> Measure\(CubeWindow, Vector3\)

Reads the last frame: the cube's centre height, the fixed thing's height in the cube and on screen, whether it
was drawn, and the player's own marker's height in the cube and on screen.

```csharp
private static JumpHoldCheck.Sample Measure(CubeWindow window, Vector3 thing)
```

#### Parameters

`window` [CubeWindow](Cube.CubeWindow.md)

The cube window.

`thing` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The fixed thing's world position.

#### Returns

 [JumpHoldCheck](Cube.TestBridge.Checks.JumpHoldCheck.md).[Sample](Cube.TestBridge.Checks.JumpHoldCheck.Sample.md)

### <a id="Cube_TestBridge_Checks_JumpHoldCheck_Run_Cube_TestBridge_BridgeContext_"></a> Run\(BridgeContext\)

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
