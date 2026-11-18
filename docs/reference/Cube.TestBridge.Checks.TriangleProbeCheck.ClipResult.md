# <a id="Cube_TestBridge_Checks_TriangleProbeCheck_ClipResult"></a> Struct TriangleProbeCheck.ClipResult

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

Counts from clipping.

```csharp
private readonly record struct TriangleProbeCheck.ClipResult : IEquatable<TriangleProbeCheck.ClipResult>
```

#### Implements

[IEquatable<TriangleProbeCheck.ClipResult\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)


## Constructors

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_ClipResult__ctor_System_Int32_System_Int32_System_Int32_System_Double_System_Int64_"></a> ClipResult\(int, int, int, double, long\)

Counts from clipping.

```csharp
public ClipResult(int Inside, int Outside, int Cut, double Ms, long Bytes)
```

#### Parameters

`Inside` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Triangles wholly inside.

`Outside` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Triangles wholly outside one plane.

`Cut` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Triangles cut by the box.

`Ms` [double](https://learn.microsoft.com/dotnet/api/system.double)

Time in milliseconds.

`Bytes` [long](https://learn.microsoft.com/dotnet/api/system.int64)

Bytes allocated.

## Properties

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_ClipResult_Bytes"></a> Bytes

Bytes allocated.

```csharp
public long Bytes { get; init; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_ClipResult_Cut"></a> Cut

Triangles cut by the box.

```csharp
public int Cut { get; init; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_ClipResult_Inside"></a> Inside

Triangles wholly inside.

```csharp
public int Inside { get; init; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_ClipResult_Ms"></a> Ms

Time in milliseconds.

```csharp
public double Ms { get; init; }
```

#### Property Value

 [double](https://learn.microsoft.com/dotnet/api/system.double)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_ClipResult_Outside"></a> Outside

Triangles wholly outside one plane.

```csharp
public int Outside { get; init; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)
