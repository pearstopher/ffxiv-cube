# <a id="Cube_TestBridge_BridgeRunner_CheckResult"></a> Class BridgeRunner.CheckResult

Namespace: [Cube.TestBridge](Cube.TestBridge.md)  
Assembly: Cube.dll  

One check's outcome, as written to the results file.

```csharp
private sealed record BridgeRunner.CheckResult : IEquatable<BridgeRunner.CheckResult>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BridgeRunner.CheckResult](Cube.TestBridge.BridgeRunner.CheckResult.md)

#### Implements

[IEquatable<BridgeRunner.CheckResult\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)


## Constructors

### <a id="Cube_TestBridge_BridgeRunner_CheckResult__ctor_System_String_System_Boolean_System_String_System_Double_System_Collections_Generic_Dictionary_System_String_System_Double__"></a> CheckResult\(string, bool, string, double, Dictionary<string, double\>\)

One check's outcome, as written to the results file.

```csharp
public CheckResult(string Name, bool Passed, string Message, double DurationMs, Dictionary<string, double> Metrics)
```

#### Parameters

`Name` [string](https://learn.microsoft.com/dotnet/api/system.string)

The check's name.

`Passed` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether it passed.

`Message` [string](https://learn.microsoft.com/dotnet/api/system.string)

What was seen.

`DurationMs` [double](https://learn.microsoft.com/dotnet/api/system.double)

How long it ran, in milliseconds.

`Metrics` [Dictionary](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary\-2)<[string](https://learn.microsoft.com/dotnet/api/system.string), [double](https://learn.microsoft.com/dotnet/api/system.double)\>

Numbers it measured.

## Properties

### <a id="Cube_TestBridge_BridgeRunner_CheckResult_DurationMs"></a> DurationMs

How long it ran, in milliseconds.

```csharp
public double DurationMs { get; init; }
```

#### Property Value

 [double](https://learn.microsoft.com/dotnet/api/system.double)

### <a id="Cube_TestBridge_BridgeRunner_CheckResult_Message"></a> Message

What was seen.

```csharp
public string Message { get; init; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Cube_TestBridge_BridgeRunner_CheckResult_Metrics"></a> Metrics

Numbers it measured.

```csharp
public Dictionary<string, double> Metrics { get; init; }
```

#### Property Value

 [Dictionary](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary\-2)<[string](https://learn.microsoft.com/dotnet/api/system.string), [double](https://learn.microsoft.com/dotnet/api/system.double)\>

### <a id="Cube_TestBridge_BridgeRunner_CheckResult_Name"></a> Name

The check's name.

```csharp
public string Name { get; init; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Cube_TestBridge_BridgeRunner_CheckResult_Passed"></a> Passed

Whether it passed.

```csharp
public bool Passed { get; init; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)
