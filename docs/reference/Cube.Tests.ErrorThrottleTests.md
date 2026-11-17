# <a id="Cube_Tests_ErrorThrottleTests"></a> Class ErrorThrottleTests

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

Checks repeated errors are logged sparingly (CLAUDE.md, "Dalamud rules": every game-facing handler catches and
logs; a handler failing every frame must not flood the log).

```csharp
public class ErrorThrottleTests
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ErrorThrottleTests](Cube.Tests.ErrorThrottleTests.md)


## Methods

### <a id="Cube_Tests_ErrorThrottleTests_Counting_errors_does_not_allocate"></a> Counting\_errors\_does\_not\_allocate\(\)

```csharp
[Fact]
public void Counting_errors_does_not_allocate()
```

### <a id="Cube_Tests_ErrorThrottleTests_Errors_within_the_interval_are_counted_but_not_reported"></a> Errors\_within\_the\_interval\_are\_counted\_but\_not\_reported\(\)

```csharp
[Fact]
public void Errors_within_the_interval_are_counted_but_not_reported()
```

### <a id="Cube_Tests_ErrorThrottleTests_First_error_is_always_reported"></a> First\_error\_is\_always\_reported\(\)

```csharp
[Fact]
public void First_error_is_always_reported()
```

### <a id="Cube_Tests_ErrorThrottleTests_Next_report_after_the_interval_says_how_many_were_skipped"></a> Next\_report\_after\_the\_interval\_says\_how\_many\_were\_skipped\(\)

```csharp
[Fact]
public void Next_report_after_the_interval_says_how_many_were_skipped()
```
