# <a id="Cube_TestBridge_Checks_TriangleProbeCheck_Cost"></a> Struct TriangleProbeCheck.Cost

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

Time of a step.

```csharp
private readonly record struct TriangleProbeCheck.Cost : IEquatable<TriangleProbeCheck.Cost>
```

#### Implements

[IEquatable<TriangleProbeCheck.Cost\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)


## Constructors

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_Cost__ctor_System_Double_System_Int64_"></a> Cost\(double, long\)

Time of a step.

```csharp
public Cost(double Ms, long Bytes)
```

#### Parameters

`Ms` [double](https://learn.microsoft.com/dotnet/api/system.double)

Time in milliseconds.

`Bytes` [long](https://learn.microsoft.com/dotnet/api/system.int64)

Bytes allocated.

## Properties

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_Cost_Bytes"></a> Bytes

Bytes allocated.

```csharp
public long Bytes { get; init; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_Cost_Ms"></a> Ms

Time in milliseconds.

```csharp
public double Ms { get; init; }
```

#### Property Value

 [double](https://learn.microsoft.com/dotnet/api/system.double)
