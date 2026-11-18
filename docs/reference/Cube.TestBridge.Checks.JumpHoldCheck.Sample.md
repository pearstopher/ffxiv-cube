# <a id="Cube_TestBridge_Checks_JumpHoldCheck_Sample"></a> Struct JumpHoldCheck.Sample

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

One frame's measurements.

```csharp
private readonly record struct JumpHoldCheck.Sample : IEquatable<JumpHoldCheck.Sample>
```

#### Implements

[IEquatable<JumpHoldCheck.Sample\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)


## Constructors

### <a id="Cube_TestBridge_Checks_JumpHoldCheck_Sample__ctor_System_Single_System_Single_System_Single_System_Boolean_System_Boolean_System_Single_System_Single_"></a> Sample\(float, float, float, bool, bool, float, float\)

One frame's measurements.

```csharp
public Sample(float CentreY, float ThingViewY, float ThingScreenY, bool ThingDrawn, bool HasMarker, float MarkerViewY, float MarkerScreenY)
```

#### Parameters

`CentreY` [float](https://learn.microsoft.com/dotnet/api/system.single)

The cube's centre height in the world.

`ThingViewY` [float](https://learn.microsoft.com/dotnet/api/system.single)

The fixed thing's height in the cube.

`ThingScreenY` [float](https://learn.microsoft.com/dotnet/api/system.single)

The fixed thing's projected screen y, in pixels.

`ThingDrawn` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether the fixed thing is in the frame's players.

`HasMarker` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether the player's own marker is in the frame's players.

`MarkerViewY` [float](https://learn.microsoft.com/dotnet/api/system.single)

The marker's height in the cube.

`MarkerScreenY` [float](https://learn.microsoft.com/dotnet/api/system.single)

The marker's projected screen y, in pixels.

## Properties

### <a id="Cube_TestBridge_Checks_JumpHoldCheck_Sample_CentreY"></a> CentreY

The cube's centre height in the world.

```csharp
public float CentreY { get; init; }
```

#### Property Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_JumpHoldCheck_Sample_HasMarker"></a> HasMarker

Whether the player's own marker is in the frame's players.

```csharp
public bool HasMarker { get; init; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_TestBridge_Checks_JumpHoldCheck_Sample_MarkerScreenY"></a> MarkerScreenY

The marker's projected screen y, in pixels.

```csharp
public float MarkerScreenY { get; init; }
```

#### Property Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_JumpHoldCheck_Sample_MarkerViewY"></a> MarkerViewY

The marker's height in the cube.

```csharp
public float MarkerViewY { get; init; }
```

#### Property Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_JumpHoldCheck_Sample_ThingDrawn"></a> ThingDrawn

Whether the fixed thing is in the frame's players.

```csharp
public bool ThingDrawn { get; init; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_TestBridge_Checks_JumpHoldCheck_Sample_ThingScreenY"></a> ThingScreenY

The fixed thing's projected screen y, in pixels.

```csharp
public float ThingScreenY { get; init; }
```

#### Property Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_JumpHoldCheck_Sample_ThingViewY"></a> ThingViewY

The fixed thing's height in the cube.

```csharp
public float ThingViewY { get; init; }
```

#### Property Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)
