# <a id="Cube_TestBridge_Checks_PlayerHeightCheck"></a> Class PlayerHeightCheck

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

Checks the player height setting (#91; PROJECT.md, "Player height in cube"): it changes which fake players above or
below the player are inside, moves the player's drawn marker off the middle of the box, shifts the collision region,
and resetting to defaults brings back the centred cube.

```csharp
internal sealed class PlayerHeightCheck : BridgeCheck
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BridgeCheck](Cube.TestBridge.BridgeCheck.md) ← 
[PlayerHeightCheck](Cube.TestBridge.Checks.PlayerHeightCheck.md)


## Remarks

<p>
Two fakes are fixed in the world 15 yalms above and below the player. With the default 40-yalm height and the
player centred both are inside; at 0% (player on the bottom face) only the one above is, at 100% only the one below.
The default view looks from above and behind, so the box's up axis points up the screen: at 0% the player's marker
must be drawn below the box's projected centre, at 100% above it, and at 50% on it.
</p>
<p>
The collision part runs only if collision is gathered here: at 0% the next gather's region must be centred half the
height above the player. The settings, fakes and window state are changed in memory only and restored in
[Cleanup](Cube.TestBridge.Checks.PlayerHeightCheck.md#Cube_TestBridge_Checks_PlayerHeightCheck_Cleanup).
</p>

## Fields

### <a id="Cube_TestBridge_Checks_PlayerHeightCheck_CentredTolerance"></a> CentredTolerance

How close the marker must be to the box's centre on screen at 50%, in pixels.

```csharp
private const float CentredTolerance = 0.5
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_PlayerHeightCheck_FakeOffset"></a> FakeOffset

How far above and below the player the two fakes are, in yalms: inside a centred 40 box, outside the other half.

```csharp
private const float FakeOffset = 15
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_PlayerHeightCheck_MinMarkerShift"></a> MinMarkerShift

How far the marker must move off the box's centre on screen at 0% or 100%, in pixels.

```csharp
private const float MinMarkerShift = 5
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_PlayerHeightCheck_RegionTolerance"></a> RegionTolerance

How far the collision region's centre may be from where it should be, in yalms (the player moving).

```csharp
private const float RegionTolerance = 1
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_PlayerHeightCheck_saved"></a> saved

The user's settings before the check, restored by [Cleanup](Cube.TestBridge.Checks.PlayerHeightCheck.md#Cube_TestBridge_Checks_PlayerHeightCheck_Cleanup).

```csharp
private readonly CubeSettings saved
```

#### Field Value

 [CubeSettings](Cube.Core.CubeSettings.md)

### <a id="Cube_TestBridge_Checks_PlayerHeightCheck_wasOpen"></a> wasOpen

The window's open state before the check, restored by [Cleanup](Cube.TestBridge.Checks.PlayerHeightCheck.md#Cube_TestBridge_Checks_PlayerHeightCheck_Cleanup).

```csharp
private bool wasOpen
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Properties

### <a id="Cube_TestBridge_Checks_PlayerHeightCheck_Name"></a> Name

Short unique name, shown in the results, e.g. <code>window-opens-and-closes</code>.

```csharp
public override string Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Cube_TestBridge_Checks_PlayerHeightCheck_Timeout"></a> Timeout

The longest the check may run before it fails as timed out.

```csharp
public override TimeSpan Timeout { get; }
```

#### Property Value

 [TimeSpan](https://learn.microsoft.com/dotnet/api/system.timespan)

#### Remarks

Ten seconds covers checks that wait a few hundred frames; override for longer ones.

## Methods

### <a id="Cube_TestBridge_Checks_PlayerHeightCheck_Cleanup_Cube_TestBridge_BridgeContext_"></a> Cleanup\(BridgeContext\)

Puts back anything the check changed (e.g. the window's open state). Called after every run,
including when the check failed, threw or timed out, so a check never leaves the plugin altered.

```csharp
public override void Cleanup(BridgeContext context)
```

#### Parameters

`context` [BridgeContext](Cube.TestBridge.BridgeContext.md)

The same context the check ran with.

### <a id="Cube_TestBridge_Checks_PlayerHeightCheck_Measure_Cube_CubeWindow_"></a> Measure\(CubeWindow\)

Reads the last frame: whether each fake is drawn, whether the player's own marker is, and how far the player's
projected position is below the box's projected centre on screen (screen Y grows downwards, so positive means
below). The marker is drawn around that projected position.

```csharp
private static PlayerHeightCheck.Sample Measure(CubeWindow window)
```

#### Parameters

`window` [CubeWindow](Cube.CubeWindow.md)

The cube window.

#### Returns

 [PlayerHeightCheck](Cube.TestBridge.Checks.PlayerHeightCheck.md).[Sample](Cube.TestBridge.Checks.PlayerHeightCheck.Sample.md)

### <a id="Cube_TestBridge_Checks_PlayerHeightCheck_Run_Cube_TestBridge_BridgeContext_"></a> Run\(BridgeContext\)

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
