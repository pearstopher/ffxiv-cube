# <a id="Cube_TestBridge_Checks_OrientationCheck"></a> Class OrientationCheck

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

Checks both orientation modes against the live game camera (PROJECT.md, "View"): in Camera mode the view's yaw
matches the camera's heading, in North up mode it is 0 whatever the camera does.

```csharp
internal sealed class OrientationCheck : BridgeCheck
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BridgeCheck](Cube.TestBridge.BridgeCheck.md) ← 
[OrientationCheck](Cube.TestBridge.Checks.OrientationCheck.md)


## Remarks

The camera is read again here, independently of the window, through [CameraReader](Cube.Game.CameraReader.md#Cube_Game_CameraReader) and turned into
a heading with [FromForward](Cube.Core.ViewYaw.md#Cube_Core_ViewYaw_FromForward). A small tolerance covers the camera moving between the window's
draw and this check, plus [ChangeThreshold](Cube.Core.ViewYaw.md#Cube_Core_ViewYaw_ChangeThreshold). If the camera happens to face north, North up
can't be told apart from Camera mode; the <code>cameraYaw</code> metric shows when that was the case. The mode is
changed in memory only and restored afterwards.

## Fields

### <a id="Cube_TestBridge_Checks_OrientationCheck_Tolerance"></a> Tolerance

Largest allowed difference between the view's yaw and the camera heading, in radians (about 1°).

```csharp
private const float Tolerance = 0.02
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_OrientationCheck_wasMode"></a> wasMode

The orientation setting before the check.

```csharp
private OrientationMode wasMode
```

#### Field Value

 [OrientationMode](Cube.Core.OrientationMode.md)

### <a id="Cube_TestBridge_Checks_OrientationCheck_wasOpen"></a> wasOpen

The window's open state before the check.

```csharp
private bool wasOpen
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Properties

### <a id="Cube_TestBridge_Checks_OrientationCheck_Name"></a> Name

Short unique name, shown in the results, e.g. <code>window-opens-and-closes</code>.

```csharp
public override string Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

## Methods

### <a id="Cube_TestBridge_Checks_OrientationCheck_Cleanup_Cube_TestBridge_BridgeContext_"></a> Cleanup\(BridgeContext\)

Puts back anything the check changed (e.g. the window's open state). Called after every run,
including when the check failed, threw or timed out, so a check never leaves the plugin altered.

```csharp
public override void Cleanup(BridgeContext context)
```

#### Parameters

`context` [BridgeContext](Cube.TestBridge.BridgeContext.md)

The same context the check ran with.

### <a id="Cube_TestBridge_Checks_OrientationCheck_Run_Cube_TestBridge_BridgeContext_"></a> Run\(BridgeContext\)

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
