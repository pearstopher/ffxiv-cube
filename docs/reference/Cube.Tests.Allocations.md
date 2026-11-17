# <a id="Cube_Tests_Allocations"></a> Class Allocations

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

Test helper for the "zero bytes allocated per frame" budget (PROJECT.md, "Efficiency and memory"):
hot-path Core code is run under [AssertNone](Cube.Tests.Allocations.md#Cube_Tests_Allocations_AssertNone) so an accidental allocation
(LINQ, a capturing lambda, boxing, string building) fails a unit test long before it shows up in game.

```csharp
internal static class Allocations
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[Allocations](Cube.Tests.Allocations.md)


## Remarks

Allocations are measured with `System.GC.GetAllocatedBytesForCurrentThread`, the same counter
the in-game bridge uses, so the two agree. The action runs once first, unmeasured, so one-time costs
(JIT, static initialisers, a cache filled on first use) don't count: the budget is about steady state.
Create the delegate before calling; the helper itself allocates nothing while measuring.

## Methods

### <a id="Cube_Tests_Allocations_AssertNone_System_Action_System_Int32_"></a> AssertNone\(Action, int\)

Fails the test if <code class="paramref">action</code> allocates any managed memory after its warm-up run.

```csharp
public static void AssertNone(Action action, int iterations = 10)
```

#### Parameters

`action` [Action](https://learn.microsoft.com/dotnet/api/system.action)

The code to measure, e.g. one frame's worth of Core work.

`iterations` [int](https://learn.microsoft.com/dotnet/api/system.int32)

How many times to run it while measuring; more runs catch amortised growth such as a list resizing every few calls.

### <a id="Cube_Tests_Allocations_Measure_System_Action_System_Int32_"></a> Measure\(Action, int\)

Returns the bytes <code class="paramref">action</code> allocates over <code class="paramref">iterations</code> runs, after one unmeasured warm-up run.

```csharp
public static long Measure(Action action, int iterations = 10)
```

#### Parameters

`action` [Action](https://learn.microsoft.com/dotnet/api/system.action)

The code to measure.

`iterations` [int](https://learn.microsoft.com/dotnet/api/system.int32)

How many measured runs.

#### Returns

 [long](https://learn.microsoft.com/dotnet/api/system.int64)
