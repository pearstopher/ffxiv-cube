# <a id="Cube_TestBridge_BridgeContext"></a> Class BridgeContext

Namespace: [Cube.TestBridge](Cube.TestBridge.md)  
Assembly: Cube.dll  

What an in-game check can see: the plugin's parts and its build stamp. Also collects the numbers a
check measured, which go into the results file next to its pass/fail.

```csharp
internal sealed class BridgeContext
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BridgeContext](Cube.TestBridge.BridgeContext.md)


## Constructors

### <a id="Cube_TestBridge_BridgeContext__ctor_Cube_CubeWindow_System_String_System_DateTime_"></a> BridgeContext\(CubeWindow, string, DateTime\)

What an in-game check can see: the plugin's parts and its build stamp. Also collects the numbers a
check measured, which go into the results file next to its pass/fail.

```csharp
public BridgeContext(CubeWindow window, string dllPath, DateTime builtAt)
```

#### Parameters

`window` [CubeWindow](Cube.CubeWindow.md)

The cube window.

`dllPath` [string](https://learn.microsoft.com/dotnet/api/system.string)

Full path of the loaded plugin DLL.

`builtAt` [DateTime](https://learn.microsoft.com/dotnet/api/system.datetime)

The DLL's write time when the plugin loaded: the build stamp.

## Properties

### <a id="Cube_TestBridge_BridgeContext_BuiltAt"></a> BuiltAt

The build stamp: when the loaded DLL was written, read at load.

```csharp
public DateTime BuiltAt { get; }
```

#### Property Value

 [DateTime](https://learn.microsoft.com/dotnet/api/system.datetime)

### <a id="Cube_TestBridge_BridgeContext_DllPath"></a> DllPath

Full path of the loaded plugin DLL.

```csharp
public string DllPath { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Cube_TestBridge_BridgeContext_Metrics"></a> Metrics

Metrics recorded by the check that is running now; cleared before each check.

```csharp
public Dictionary<string, double> Metrics { get; }
```

#### Property Value

 [Dictionary](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary\-2)<[string](https://learn.microsoft.com/dotnet/api/system.string), [double](https://learn.microsoft.com/dotnet/api/system.double)\>

### <a id="Cube_TestBridge_BridgeContext_Window"></a> Window

The cube window, so checks can open it and read its draw cost.

```csharp
public CubeWindow Window { get; }
```

#### Property Value

 [CubeWindow](Cube.CubeWindow.md)

## Methods

### <a id="Cube_TestBridge_BridgeContext_Metric_System_String_System_Double_"></a> Metric\(string, double\)

Records a measured number for the current check, e.g. <code>averageMs</code>.

```csharp
public void Metric(string name, double value)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)

The metric's name in the results file.

`value` [double](https://learn.microsoft.com/dotnet/api/system.double)

The measured value.
