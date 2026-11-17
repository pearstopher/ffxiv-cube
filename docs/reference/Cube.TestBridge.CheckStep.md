# <a id="Cube_TestBridge_CheckStep"></a> Struct CheckStep

Namespace: [Cube.TestBridge](Cube.TestBridge.md)  
Assembly: Cube.dll  

One step of an in-game check. A check is written as an iterator that yields [NextFrame](Cube.TestBridge.CheckStep.md#Cube_TestBridge_CheckStep_NextFrame)
to wait a frame and ends with [Pass](Cube.TestBridge.CheckStep.md#Cube_TestBridge_CheckStep_Pass) or [Fail](Cube.TestBridge.CheckStep.md#Cube_TestBridge_CheckStep_Fail), so a check that needs
several frames (open the window, wait for it to draw, close it) reads top to bottom.

```csharp
internal readonly record struct CheckStep : IEquatable<CheckStep>
```

#### Implements

[IEquatable<CheckStep\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)


## Constructors

### <a id="Cube_TestBridge_CheckStep__ctor_Cube_TestBridge_CheckState_System_String_"></a> CheckStep\(CheckState, string?\)

One step of an in-game check. A check is written as an iterator that yields [NextFrame](Cube.TestBridge.CheckStep.md#Cube_TestBridge_CheckStep_NextFrame)
to wait a frame and ends with [Pass](Cube.TestBridge.CheckStep.md#Cube_TestBridge_CheckStep_Pass) or [Fail](Cube.TestBridge.CheckStep.md#Cube_TestBridge_CheckStep_Fail), so a check that needs
several frames (open the window, wait for it to draw, close it) reads top to bottom.

```csharp
public CheckStep(CheckState State, string? Message)
```

#### Parameters

`State` [CheckState](Cube.TestBridge.CheckState.md)

Running, passed or failed.

`Message` [string](https://learn.microsoft.com/dotnet/api/system.string)?

What was seen; required when finished, so the results explain themselves.

## Properties

### <a id="Cube_TestBridge_CheckStep_Message"></a> Message

What was seen; required when finished, so the results explain themselves.

```csharp
public string? Message { get; init; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)?

### <a id="Cube_TestBridge_CheckStep_NextFrame"></a> NextFrame

Wait for the next frame, then continue the check.

```csharp
public static CheckStep NextFrame { get; }
```

#### Property Value

 [CheckStep](Cube.TestBridge.CheckStep.md)

### <a id="Cube_TestBridge_CheckStep_State"></a> State

Running, passed or failed.

```csharp
public CheckState State { get; init; }
```

#### Property Value

 [CheckState](Cube.TestBridge.CheckState.md)

## Methods

### <a id="Cube_TestBridge_CheckStep_Fail_System_String_"></a> Fail\(string\)

Ends the check as failed.

```csharp
public static CheckStep Fail(string message)
```

#### Parameters

`message` [string](https://learn.microsoft.com/dotnet/api/system.string)

What was expected and what was seen instead.

#### Returns

 [CheckStep](Cube.TestBridge.CheckStep.md)

### <a id="Cube_TestBridge_CheckStep_Pass_System_String_"></a> Pass\(string\)

Ends the check as passed.

```csharp
public static CheckStep Pass(string message)
```

#### Parameters

`message` [string](https://learn.microsoft.com/dotnet/api/system.string)

What was seen, e.g. the measured value.

#### Returns

 [CheckStep](Cube.TestBridge.CheckStep.md)
