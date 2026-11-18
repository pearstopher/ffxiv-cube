# <a id="Cube_TestBridge_Checks_SettingsCheck"></a> Class SettingsCheck

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

Checks each setting changes the drawn frame straight away, and that putting the settings back restores it
(PROJECT.md, "Features": settings for cube size, orientation mode, labels on/off, colours and opacity; "NPCs": NPCs
on/off and the NPC colour).

```csharp
internal sealed class SettingsCheck : BridgeCheck
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BridgeCheck](Cube.TestBridge.BridgeCheck.md) ← 
[SettingsCheck](Cube.TestBridge.Checks.SettingsCheck.md)


## Remarks

Changes the settings in memory only, never saving them, and restores the user's values in
[Cleanup](Cube.TestBridge.Checks.SettingsCheck.md#Cube_TestBridge_Checks_SettingsCheck_Cleanup) even if a step fails. Uses the standard fakes (#16, #74): the colour step changes the
"others" colour, the NPC steps change the NPC colour and then turn NPCs off.

## Fields

### <a id="Cube_TestBridge_Checks_SettingsCheck_NpcTestColour"></a> NpcTestColour

A second unused colour for the NPC colour step, different from [TestColour](Cube.TestBridge.Checks.SettingsCheck.md#Cube_TestBridge_Checks_SettingsCheck_TestColour).

```csharp
private static readonly Vector3 NpcTestColour
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_TestBridge_Checks_SettingsCheck_TestColour"></a> TestColour

A colour no default uses, so a face in it can only come from the changed setting.

```csharp
private static readonly Vector3 TestColour
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_TestBridge_Checks_SettingsCheck_saved"></a> saved

The user's settings before the check.

```csharp
private readonly CubeSettings saved
```

#### Field Value

 [CubeSettings](Cube.Core.CubeSettings.md)

### <a id="Cube_TestBridge_Checks_SettingsCheck_wasOpen"></a> wasOpen

The window's open state before the check.

```csharp
private bool wasOpen
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Properties

### <a id="Cube_TestBridge_Checks_SettingsCheck_Name"></a> Name

Short unique name, shown in the results, e.g. <code>window-opens-and-closes</code>.

```csharp
public override string Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Cube_TestBridge_Checks_SettingsCheck_Timeout"></a> Timeout

The longest the check may run before it fails as timed out.

```csharp
public override TimeSpan Timeout { get; }
```

#### Property Value

 [TimeSpan](https://learn.microsoft.com/dotnet/api/system.timespan)

#### Remarks

Ten seconds covers checks that wait a few hundred frames; override for longer ones.

## Methods

### <a id="Cube_TestBridge_Checks_SettingsCheck_CheckColours_Cube_CubeWindow_System_UInt32_System_Int32__"></a> CheckColours\(CubeWindow, uint, out int\)

Checks other players' faces are shades of the wanted colour (alpha included) and the edges carry the
opacity too.

```csharp
private static string? CheckColours(CubeWindow window, uint want, out int others)
```

#### Parameters

`window` [CubeWindow](Cube.CubeWindow.md)

The cube window.

`want` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

The packed colour other players should have.

`others` [int](https://learn.microsoft.com/dotnet/api/system.int32)

How many other players' faces were checked.

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)?

What was wrong, or null if all is well.

### <a id="Cube_TestBridge_Checks_SettingsCheck_Cleanup_Cube_TestBridge_BridgeContext_"></a> Cleanup\(BridgeContext\)

Puts back anything the check changed (e.g. the window's open state). Called after every run,
including when the check failed, threw or timed out, so a check never leaves the plugin altered.

```csharp
public override void Cleanup(BridgeContext context)
```

#### Parameters

`context` [BridgeContext](Cube.TestBridge.BridgeContext.md)

The same context the check ran with.

### <a id="Cube_TestBridge_Checks_SettingsCheck_CountFaces_Cube_CubeWindow_Cube_Core_PlayerRelation_System_UInt32_System_String__"></a> CountFaces\(CubeWindow, PlayerRelation, uint, out string?\)

Counts the faces drawn for one relation and checks each is a shade of the wanted colour.

```csharp
private static int CountFaces(CubeWindow window, PlayerRelation relation, uint want, out string? problem)
```

#### Parameters

`window` [CubeWindow](Cube.CubeWindow.md)

The cube window.

`relation` [PlayerRelation](Cube.Core.PlayerRelation.md)

Whose faces to count.

`want` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

The packed colour they should be shades of; 0 to only count.

`problem` [string](https://learn.microsoft.com/dotnet/api/system.string)?

What was wrong with a face, or null.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

How many faces that relation has.

### <a id="Cube_TestBridge_Checks_SettingsCheck_CountLabels_Cube_CubeWindow_"></a> CountLabels\(CubeWindow\)

Counts label primitives in the last frame.

```csharp
private static int CountLabels(CubeWindow window)
```

#### Parameters

`window` [CubeWindow](Cube.CubeWindow.md)

The cube window.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_SettingsCheck_IsShadeOf_System_UInt32_System_UInt32_"></a> IsShadeOf\(uint, uint\)

Whether a face colour is one of the cube shades of a base colour.

```csharp
private static bool IsShadeOf(uint face, uint baseColour)
```

#### Parameters

`face` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

The face's colour.

`baseColour` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

The player's colour.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_TestBridge_Checks_SettingsCheck_Run_Cube_TestBridge_BridgeContext_"></a> Run\(BridgeContext\)

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
