# <a id="Cube_TestBridge_Checks_PlayerHeightCheck_Sample"></a> Struct PlayerHeightCheck.Sample

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

One frame's measurements.

```csharp
private readonly record struct PlayerHeightCheck.Sample : IEquatable<PlayerHeightCheck.Sample>
```

#### Implements

[IEquatable<PlayerHeightCheck.Sample\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)


## Constructors

### <a id="Cube_TestBridge_Checks_PlayerHeightCheck_Sample__ctor_System_Boolean_System_Boolean_System_Boolean_System_Single_"></a> Sample\(bool, bool, bool, float\)

One frame's measurements.

```csharp
public Sample(bool Above, bool Below, bool HasMarker, float MarkerShift)
```

#### Parameters

`Above` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether the fake above the player was drawn.

`Below` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether the fake below the player was drawn.

`HasMarker` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether the player's own marker was drawn.

`MarkerShift` [float](https://learn.microsoft.com/dotnet/api/system.single)

How far the player's projected position is below the box's centre on screen, in pixels.

## Properties

### <a id="Cube_TestBridge_Checks_PlayerHeightCheck_Sample_Above"></a> Above

Whether the fake above the player was drawn.

```csharp
public bool Above { get; init; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_TestBridge_Checks_PlayerHeightCheck_Sample_Below"></a> Below

Whether the fake below the player was drawn.

```csharp
public bool Below { get; init; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_TestBridge_Checks_PlayerHeightCheck_Sample_HasMarker"></a> HasMarker

Whether the player's own marker was drawn.

```csharp
public bool HasMarker { get; init; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_TestBridge_Checks_PlayerHeightCheck_Sample_MarkerShift"></a> MarkerShift

How far the player's projected position is below the box's centre on screen, in pixels.

```csharp
public float MarkerShift { get; init; }
```

#### Property Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)
