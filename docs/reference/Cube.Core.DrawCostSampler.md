# <a id="Cube_Core_DrawCostSampler"></a> Class DrawCostSampler

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

Keeps the cost of the last few frames of the plugin's draw path: time taken and bytes allocated.
The in-game test bridge reads it to check the budgets in PROJECT.md ("Efficiency and memory"):
under 0.5 ms per frame on average and zero bytes allocated per frame in steady state.

```csharp
public sealed class DrawCostSampler
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[DrawCostSampler](Cube.Core.DrawCostSampler.md)


## Remarks

Samples go into fixed-size arrays allocated once, so recording a frame allocates nothing and the
memory used never grows (PROJECT.md: bounded memory). When full, the oldest sample is overwritten.
Times are kept as raw `System.Diagnostics.Stopwatch` ticks and converted only when read.

## Constructors

### <a id="Cube_Core_DrawCostSampler__ctor_System_Int32_System_Int64_"></a> DrawCostSampler\(int, long\)

Creates an empty sampler.

```csharp
public DrawCostSampler(int capacity, long ticksPerSecond)
```

#### Parameters

`capacity` [int](https://learn.microsoft.com/dotnet/api/system.int32)

How many frames to keep; the newest win.

`ticksPerSecond` [long](https://learn.microsoft.com/dotnet/api/system.int64)

The clock's frequency, normally `System.Diagnostics.Stopwatch.Frequency`.

## Fields

### <a id="Cube_Core_DrawCostSampler_bytes"></a> bytes

Bytes allocated per recorded frame, parallel to [ticks](Cube.Core.DrawCostSampler.md#Cube_Core_DrawCostSampler_ticks).

```csharp
private readonly long[] bytes
```

#### Field Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)\[\]

### <a id="Cube_Core_DrawCostSampler_next"></a> next

Where the next sample is written.

```csharp
private int next
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_DrawCostSampler_ticks"></a> ticks

Elapsed ticks per recorded frame, used as a ring buffer.

```csharp
private readonly long[] ticks
```

#### Field Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)\[\]

### <a id="Cube_Core_DrawCostSampler_ticksPerSecond"></a> ticksPerSecond

Ticks per second of the clock the times were measured with.

```csharp
private readonly long ticksPerSecond
```

#### Field Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

## Properties

### <a id="Cube_Core_DrawCostSampler_AverageMilliseconds"></a> AverageMilliseconds

Average time per held frame, in milliseconds; 0 when empty.

```csharp
public double AverageMilliseconds { get; }
```

#### Property Value

 [double](https://learn.microsoft.com/dotnet/api/system.double)

### <a id="Cube_Core_DrawCostSampler_Capacity"></a> Capacity

How many frames the sampler can hold.

```csharp
public int Capacity { get; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_DrawCostSampler_Count"></a> Count

How many frames are currently held (at most [Capacity](Cube.Core.DrawCostSampler.md#Cube_Core_DrawCostSampler_Capacity)).

```csharp
public int Count { get; private set; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_DrawCostSampler_FramesWithAllocations"></a> FramesWithAllocations

How many held frames allocated anything. The phase 1 budget is zero.

```csharp
public int FramesWithAllocations { get; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_DrawCostSampler_LifetimeAllocatedBytes"></a> LifetimeAllocatedBytes

Bytes allocated over every frame ever recorded, never reset. A soak check compares it before and after
minutes of drawing, longer than the held frames cover.

```csharp
public long LifetimeAllocatedBytes { get; private set; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Cube_Core_DrawCostSampler_LifetimeTicks"></a> LifetimeTicks

Clock ticks spent over every frame ever recorded, never reset.

```csharp
public long LifetimeTicks { get; private set; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Cube_Core_DrawCostSampler_MaxMilliseconds"></a> MaxMilliseconds

The slowest held frame, in milliseconds; 0 when empty.

```csharp
public double MaxMilliseconds { get; }
```

#### Property Value

 [double](https://learn.microsoft.com/dotnet/api/system.double)

### <a id="Cube_Core_DrawCostSampler_TotalAllocatedBytes"></a> TotalAllocatedBytes

Bytes allocated over all held frames.

```csharp
public long TotalAllocatedBytes { get; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Cube_Core_DrawCostSampler_TotalRecorded"></a> TotalRecorded

How many frames were recorded since creation, never reset. A check compares it before and after
to tell whether the window drew at all.

```csharp
public long TotalRecorded { get; private set; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

## Methods

### <a id="Cube_Core_DrawCostSampler_Record_System_Int64_System_Int64_"></a> Record\(long, long\)

Records one frame's cost. Allocation-free, so it can run inside the measured path's caller every frame.

```csharp
public void Record(long elapsedTicks, long allocatedBytes)
```

#### Parameters

`elapsedTicks` [long](https://learn.microsoft.com/dotnet/api/system.int64)

Time the draw path took, in clock ticks.

`allocatedBytes` [long](https://learn.microsoft.com/dotnet/api/system.int64)

Bytes the draw path allocated on its thread.

### <a id="Cube_Core_DrawCostSampler_Reset"></a> Reset\(\)

Forgets the held frames (but not [TotalRecorded](Cube.Core.DrawCostSampler.md#Cube_Core_DrawCostSampler_TotalRecorded)), so a check can measure steady state
after a warm-up period.

```csharp
public void Reset()
```

### <a id="Cube_Core_DrawCostSampler_ToMilliseconds_System_Int64_"></a> ToMilliseconds\(long\)

Converts a tick count from this sampler's clock to milliseconds.

```csharp
public double ToMilliseconds(long value)
```

#### Parameters

`value` [long](https://learn.microsoft.com/dotnet/api/system.int64)

A tick count, e.g. a difference of [LifetimeTicks](Cube.Core.DrawCostSampler.md#Cube_Core_DrawCostSampler_LifetimeTicks).

#### Returns

 [double](https://learn.microsoft.com/dotnet/api/system.double)
