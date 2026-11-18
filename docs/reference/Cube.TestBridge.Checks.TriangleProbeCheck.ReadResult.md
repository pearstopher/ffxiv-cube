# <a id="Cube_TestBridge_Checks_TriangleProbeCheck_ReadResult"></a> Struct TriangleProbeCheck.ReadResult

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

Counts from reading.

```csharp
private readonly record struct TriangleProbeCheck.ReadResult : IEquatable<TriangleProbeCheck.ReadResult>
```

#### Implements

[IEquatable<TriangleProbeCheck.ReadResult\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)


## Constructors

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_ReadResult__ctor_System_Int32_System_Int32_System_Double_System_Int64_"></a> ReadResult\(int, int, double, long\)

Counts from reading.

```csharp
public ReadResult(int InBounds, int Primitive, double Ms, long Bytes)
```

#### Parameters

`InBounds` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Triangles in nodes crossing the cube (the #28 number).

`Primitive` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Triangles made from primitive colliders.

`Ms` [double](https://learn.microsoft.com/dotnet/api/system.double)

Time in milliseconds.

`Bytes` [long](https://learn.microsoft.com/dotnet/api/system.int64)

Bytes allocated.

## Properties

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_ReadResult_Bytes"></a> Bytes

Bytes allocated.

```csharp
public long Bytes { get; init; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_ReadResult_InBounds"></a> InBounds

Triangles in nodes crossing the cube (the #28 number).

```csharp
public int InBounds { get; init; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_ReadResult_Ms"></a> Ms

Time in milliseconds.

```csharp
public double Ms { get; init; }
```

#### Property Value

 [double](https://learn.microsoft.com/dotnet/api/system.double)

### <a id="Cube_TestBridge_Checks_TriangleProbeCheck_ReadResult_Primitive"></a> Primitive

Triangles made from primitive colliders.

```csharp
public int Primitive { get; init; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)
