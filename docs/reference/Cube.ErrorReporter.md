# <a id="Cube_ErrorReporter"></a> Class ErrorReporter

Namespace: [Cube](Cube.md)  
Assembly: Cube.dll  

Logs exceptions caught in one game-facing handler, throttled with [ErrorThrottle](Cube.Core.ErrorThrottle.md#Cube_Core_ErrorThrottle) so a handler that
fails every frame logs about once every ten seconds (CLAUDE.md, "Dalamud rules": wrap every game-facing handler in
try/catch and log; never let an exception reach the game).

```csharp
internal sealed class ErrorReporter
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ErrorReporter](Cube.ErrorReporter.md)


## Constructors

### <a id="Cube_ErrorReporter__ctor_System_String_"></a> ErrorReporter\(string\)

Logs exceptions caught in one game-facing handler, throttled with [ErrorThrottle](Cube.Core.ErrorThrottle.md#Cube_Core_ErrorThrottle) so a handler that
fails every frame logs about once every ten seconds (CLAUDE.md, "Dalamud rules": wrap every game-facing handler in
try/catch and log; never let an exception reach the game).

```csharp
public ErrorReporter(string where)
```

#### Parameters

`where` [string](https://learn.microsoft.com/dotnet/api/system.string)

What failed, for the log line, e.g. "Cube window".

## Properties

### <a id="Cube_ErrorReporter_Throttle"></a> Throttle

The throttle, also read by the bridge: its [Total](Cube.Core.ErrorThrottle.md#Cube_Core_ErrorThrottle_Total) must stay 0.

```csharp
public ErrorThrottle Throttle { get; }
```

#### Property Value

 [ErrorThrottle](Cube.Core.ErrorThrottle.md)

### <a id="Cube_ErrorReporter_Where"></a> Where

What this reporter covers.

```csharp
public string Where { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

## Methods

### <a id="Cube_ErrorReporter_Report_System_Exception_"></a> Report\(Exception\)

Counts the exception and logs it unless one was logged in the last ten seconds.

```csharp
public void Report(Exception ex)
```

#### Parameters

`ex` [Exception](https://learn.microsoft.com/dotnet/api/system.exception)

The exception caught.
