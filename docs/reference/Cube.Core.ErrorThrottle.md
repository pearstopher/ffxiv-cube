# <a id="Cube_Core_ErrorThrottle"></a> Class ErrorThrottle

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

Decides when a repeated error is worth logging. A handler that fails every frame would otherwise write 60 log
lines a second and bury everything else (CLAUDE.md, "Dalamud rules": catch and log, never let an exception reach
the game).

```csharp
public sealed class ErrorThrottle
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ErrorThrottle](Cube.Core.ErrorThrottle.md)


## Remarks

The first error is always reported; after that at most one per [Interval](Cube.Core.ErrorThrottle.md#Cube_Core_ErrorThrottle_Interval), and each report says how
many were skipped. Works on `System.Diagnostics.Stopwatch` timestamps passed in, so it is tested
without waiting. No allocation.

## Constructors

### <a id="Cube_Core_ErrorThrottle__ctor_System_Int64_"></a> ErrorThrottle\(long\)

Decides when a repeated error is worth logging. A handler that fails every frame would otherwise write 60 log
lines a second and bury everything else (CLAUDE.md, "Dalamud rules": catch and log, never let an exception reach
the game).

```csharp
public ErrorThrottle(long interval)
```

#### Parameters

`interval` [long](https://learn.microsoft.com/dotnet/api/system.int64)

Smallest time between reports, in timestamp ticks.

#### Remarks

The first error is always reported; after that at most one per [Interval](Cube.Core.ErrorThrottle.md#Cube_Core_ErrorThrottle_Interval), and each report says how
many were skipped. Works on `System.Diagnostics.Stopwatch` timestamps passed in, so it is tested
without waiting. No allocation.

## Fields

### <a id="Cube_Core_ErrorThrottle_lastReport"></a> lastReport

When the last report was allowed; null before the first error.

```csharp
private long? lastReport
```

#### Field Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)?

## Properties

### <a id="Cube_Core_ErrorThrottle_Interval"></a> Interval

Smallest time between reports, in timestamp ticks.

```csharp
public long Interval { get; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Cube_Core_ErrorThrottle_Skipped"></a> Skipped

Errors since the last report that weren't reported.

```csharp
public long Skipped { get; private set; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Cube_Core_ErrorThrottle_Total"></a> Total

Every error seen, reported or not. The bridge checks it stays 0.

```csharp
public long Total { get; private set; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

## Methods

### <a id="Cube_Core_ErrorThrottle_ShouldReport_System_Int64_System_Int64__"></a> ShouldReport\(long, out long\)

Counts an error and says whether to log it. When it returns true, <code class="paramref">skippedBefore</code> is how many
errors were swallowed since the previous report.

```csharp
public bool ShouldReport(long now, out long skippedBefore)
```

#### Parameters

`now` [long](https://learn.microsoft.com/dotnet/api/system.int64)

The current timestamp.

`skippedBefore` [long](https://learn.microsoft.com/dotnet/api/system.int64)

Errors not reported since the last report.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)
