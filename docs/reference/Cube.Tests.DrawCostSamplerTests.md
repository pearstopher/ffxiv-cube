# <a id="Cube_Tests_DrawCostSamplerTests"></a> Class DrawCostSamplerTests

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

Checks the draw-cost measurements the in-game bridge uses for PROJECT.md's budgets
("Efficiency and memory": under 0.5 ms per frame on average, zero bytes allocated per frame),
and that measuring is itself allocation-free with bounded memory.

```csharp
public class DrawCostSamplerTests
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[DrawCostSamplerTests](Cube.Tests.DrawCostSamplerTests.md)


## Fields

### <a id="Cube_Tests_DrawCostSamplerTests_Millisecond"></a> Millisecond

```csharp
private const long Millisecond = 1
```

#### Field Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

## Methods

### <a id="Cube_Tests_DrawCostSamplerTests_Allocations_are_totalled_and_frames_that_allocate_are_counted"></a> Allocations\_are\_totalled\_and\_frames\_that\_allocate\_are\_counted\(\)

```csharp
[Fact]
public void Allocations_are_totalled_and_frames_that_allocate_are_counted()
```

### <a id="Cube_Tests_DrawCostSamplerTests_Average_and_max_time_are_in_milliseconds"></a> Average\_and\_max\_time\_are\_in\_milliseconds\(\)

```csharp
[Fact]
public void Average_and_max_time_are_in_milliseconds()
```

### <a id="Cube_Tests_DrawCostSamplerTests_Empty_sampler_reports_zero"></a> Empty\_sampler\_reports\_zero\(\)

```csharp
[Fact]
public void Empty_sampler_reports_zero()
```

### <a id="Cube_Tests_DrawCostSamplerTests_Invalid_capacity_or_clock_is_rejected_System_Int32_System_Int64_"></a> Invalid\_capacity\_or\_clock\_is\_rejected\(int, long\)

```csharp
[Theory]
[InlineData(new object[] { 0, 1000 })]
[InlineData(new object[] { 4, 0 })]
public void Invalid_capacity_or_clock_is_rejected(int capacity, long ticksPerSecond)
```

#### Parameters

`capacity` [int](https://learn.microsoft.com/dotnet/api/system.int32)

`ticksPerSecond` [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Cube_Tests_DrawCostSamplerTests_Lifetime_totals_cover_every_frame_even_after_wrapping_and_reset"></a> Lifetime\_totals\_cover\_every\_frame\_even\_after\_wrapping\_and\_reset\(\)

```csharp
[Fact]
public void Lifetime_totals_cover_every_frame_even_after_wrapping_and_reset()
```

### <a id="Cube_Tests_DrawCostSamplerTests_Oldest_frames_are_dropped_when_full"></a> Oldest\_frames\_are\_dropped\_when\_full\(\)

```csharp
[Fact]
public void Oldest_frames_are_dropped_when_full()
```

### <a id="Cube_Tests_DrawCostSamplerTests_Recording_and_reading_do_not_allocate"></a> Recording\_and\_reading\_do\_not\_allocate\(\)

```csharp
[Fact]
public void Recording_and_reading_do_not_allocate()
```

### <a id="Cube_Tests_DrawCostSamplerTests_Reset_forgets_samples_but_keeps_the_running_total"></a> Reset\_forgets\_samples\_but\_keeps\_the\_running\_total\(\)

```csharp
[Fact]
public void Reset_forgets_samples_but_keeps_the_running_total()
```
