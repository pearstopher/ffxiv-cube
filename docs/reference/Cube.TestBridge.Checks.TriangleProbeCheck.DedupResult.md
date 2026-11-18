# <a id="Cube_TestBridge_Checks_TriangleProbeCheck_DedupResult"></a> Struct TriangleProbeCheck.DedupResult

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

Counts from edge deduplication.

```csharp
private readonly record struct TriangleProbeCheck.DedupResult : IEquatable<TriangleProbeCheck.DedupResult>
```

#### Implements

[IEquatable<TriangleProbeCheck.DedupResult\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)


## Constructors

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_DedupResult__ctor_System_Int32_System_Int32_System_Double_System_Int64_"></a> DedupResult\(int, int, double, long\)

Counts from edge deduplication.

```csharp
public DedupResult(int Shared, int Coplanar, double Ms, long Bytes)
```

#### Parameters

`Shared` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Edges used by two triangles.

`Coplanar` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Shared edges whose two triangles are coplanar.

`Ms` [double](https://learn.microsoft.com/dotnet/api/system.double)

Time in milliseconds.

`Bytes` [long](https://learn.microsoft.com/dotnet/api/system.int64)

Bytes allocated.

## Properties

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_DedupResult_Bytes"></a> Bytes

Bytes allocated.

```csharp
public long Bytes { get; init; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_DedupResult_Coplanar"></a> Coplanar

Shared edges whose two triangles are coplanar.

```csharp
public int Coplanar { get; init; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_DedupResult_Ms"></a> Ms

Time in milliseconds.

```csharp
public double Ms { get; init; }
```

#### Property Value

 [double](https://learn.microsoft.com/dotnet/api/system.double)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_DedupResult_Shared"></a> Shared

Edges used by two triangles.

```csharp
public int Shared { get; init; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)
