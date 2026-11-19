# <a id="Cube_Tests_TurnedBoxTests"></a> Class TurnedBoxTests

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

Checks that collision boxes turn to fit their objects (PROJECT.md, "Collision (phase 2)": simplified boxes clipped to
the cube; "Efficiency and memory": no allocations in the per-frame path). A box from a pure yaw keeps its turn, a
tilted or sheared matrix falls back to the world-axis box, and clipping a turned box to a turned cube stays inside
both shapes (#110 research, #123).

```csharp
public class TurnedBoxTests
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[TurnedBoxTests](Cube.Tests.TurnedBoxTests.md)


## Fields

### <a id="Cube_Tests_TurnedBoxTests_Centre"></a> Centre

```csharp
private static readonly Vector3 Centre
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Tests_TurnedBoxTests_Tolerance"></a> Tolerance

```csharp
private const float Tolerance = 0.001
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

## Methods

### <a id="Cube_Tests_TurnedBoxTests_Area_Cube_Core_ClippedBox__"></a> Area\(in ClippedBox\)

```csharp
private static float Area(in ClippedBox box)
```

#### Parameters

`box` [ClippedBox](Cube.Core.ClippedBox.md)

#### Returns

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_TurnedBoxTests_Box_CornersMatchTheMatrix_Cube_Core_CollisionBox__System_Numerics_Matrix4x4__"></a> Box\_CornersMatchTheMatrix\(in CollisionBox, in Matrix4x4\)

```csharp
private static bool Box_CornersMatchTheMatrix(in CollisionBox box, in Matrix4x4 world)
```

#### Parameters

`box` [CollisionBox](Cube.Core.CollisionBox.md)

`world` [Matrix4x4](https://learn.microsoft.com/dotnet/api/system.numerics.matrix4x4)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Tests_TurnedBoxTests_ClippingTurnedBoxes_DoesNotAllocate"></a> ClippingTurnedBoxes\_DoesNotAllocate\(\)

```csharp
[Fact]
public void ClippingTurnedBoxes_DoesNotAllocate()
```

### <a id="Cube_Tests_TurnedBoxTests_DegenerateMatrix_DoesNotThrowAndIsNotTurned"></a> DegenerateMatrix\_DoesNotThrowAndIsNotTurned\(\)

```csharp
[Fact]
public void DegenerateMatrix_DoesNotThrowAndIsNotTurned()
```

### <a id="Cube_Tests_TurnedBoxTests_Fence_System_Single_"></a> Fence\(float\)

```csharp
private static CollisionBox Fence(float yaw)
```

#### Parameters

`yaw` [float](https://learn.microsoft.com/dotnet/api/system.single)

#### Returns

 [CollisionBox](Cube.Core.CollisionBox.md)

### <a id="Cube_Tests_TurnedBoxTests_FenceAt45_ClippedToANorthUpCube_KeepsItsThinFootprint"></a> FenceAt45\_ClippedToANorthUpCube\_KeepsItsThinFootprint\(\)

```csharp
[Fact]
public void FenceAt45_ClippedToANorthUpCube_KeepsItsThinFootprint()
```

### <a id="Cube_Tests_TurnedBoxTests_InsideBox_Cube_Core_CollisionBox__System_Numerics_Vector2_"></a> InsideBox\(in CollisionBox, Vector2\)

```csharp
private static bool InsideBox(in CollisionBox box, Vector2 p)
```

#### Parameters

`box` [CollisionBox](Cube.Core.CollisionBox.md)

`p` [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Tests_TurnedBoxTests_InsideCube_Cube_Core_CubeSpace__System_Numerics_Vector2_"></a> InsideCube\(in CubeSpace, Vector2\)

```csharp
private static bool InsideCube(in CubeSpace cube, Vector2 p)
```

#### Parameters

`cube` [CubeSpace](Cube.Core.CubeSpace.md)

`p` [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Tests_TurnedBoxTests_PureYawMatrix_GivesATurnedBoxKeepingTheLocalSize_System_Single_"></a> PureYawMatrix\_GivesATurnedBoxKeepingTheLocalSize\(float\)

```csharp
[Theory]
[InlineData(new object[] { 0.4 })]
[InlineData(new object[] { 0.7853982 })]
[InlineData(new object[] { 2.2 })]
[InlineData(new object[] { -1 })]
public void PureYawMatrix_GivesATurnedBoxKeepingTheLocalSize(float yaw)
```

#### Parameters

`yaw` [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_TurnedBoxTests_QuarterTurns_LineUpWithTheWorldAxes_System_Single_"></a> QuarterTurns\_LineUpWithTheWorldAxes\(float\)

```csharp
[Theory]
[InlineData(new object[] { 0 })]
[InlineData(new object[] { 1.5707964 })]
[InlineData(new object[] { 3.1415927 })]
[InlineData(new object[] { -1.5707964 })]
public void QuarterTurns_LineUpWithTheWorldAxes(float yaw)
```

#### Parameters

`yaw` [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_TurnedBoxTests_ScaleFoldsIntoTheHalfSize_AndMirroringChangesNothing"></a> ScaleFoldsIntoTheHalfSize\_AndMirroringChangesNothing\(\)

```csharp
[Fact]
public void ScaleFoldsIntoTheHalfSize_AndMirroringChangesNothing()
```

### <a id="Cube_Tests_TurnedBoxTests_ShearedMatrix_FallsBackToTheWorldAxisBox"></a> ShearedMatrix\_FallsBackToTheWorldAxisBox\(\)

```csharp
[Fact]
public void ShearedMatrix_FallsBackToTheWorldAxisBox()
```

### <a id="Cube_Tests_TurnedBoxTests_TiltedMatrix_FallsBackToTheWorldAxisBox"></a> TiltedMatrix\_FallsBackToTheWorldAxisBox\(\)

```csharp
[Fact]
public void TiltedMatrix_FallsBackToTheWorldAxisBox()
```

### <a id="Cube_Tests_TurnedBoxTests_TurnedBoxClippedToTurnedCube_EveryCornerIsInsideBothShapes_System_Single_System_Single_"></a> TurnedBoxClippedToTurnedCube\_EveryCornerIsInsideBothShapes\(float, float\)

```csharp
[Theory]
[InlineData(new object[] { 0, 0.3 })]
[InlineData(new object[] { 0.5, 0 })]
[InlineData(new object[] { 0.5, 0.9 })]
[InlineData(new object[] { 0.785, 0.785 })]
[InlineData(new object[] { 1.2, -0.4 })]
public void TurnedBoxClippedToTurnedCube_EveryCornerIsInsideBothShapes(float boxYaw, float cubeYaw)
```

#### Parameters

`boxYaw` [float](https://learn.microsoft.com/dotnet/api/system.single)

`cubeYaw` [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_TurnedBoxTests_TurnedBoxFuzz_EveryClippedCornerIsInsideBothShapes"></a> TurnedBoxFuzz\_EveryClippedCornerIsInsideBothShapes\(\)

```csharp
[Fact]
public void TurnedBoxFuzz_EveryClippedCornerIsInsideBothShapes()
```

### <a id="Cube_Tests_TurnedBoxTests_TurnedBoxWhollyInsideTheCube_KeepsItsFourTurnedCorners"></a> TurnedBoxWhollyInsideTheCube\_KeepsItsFourTurnedCorners\(\)

```csharp
[Fact]
public void TurnedBoxWhollyInsideTheCube_KeepsItsFourTurnedCorners()
```

### <a id="Cube_Tests_TurnedBoxTests_TurnedBoxWhollyOutsideTheCube_IsDropped_EvenIfItsWorldAxisBoxReachesIn"></a> TurnedBoxWhollyOutsideTheCube\_IsDropped\_EvenIfItsWorldAxisBoxReachesIn\(\)

```csharp
[Fact]
public void TurnedBoxWhollyOutsideTheCube_IsDropped_EvenIfItsWorldAxisBoxReachesIn()
```

### <a id="Cube_Tests_TurnedBoxTests_TurnedBoxes_AreNeverMerged"></a> TurnedBoxes\_AreNeverMerged\(\)

```csharp
[Fact]
public void TurnedBoxes_AreNeverMerged()
```

### <a id="Cube_Tests_TurnedBoxTests_TurnedFence_WorldAxisBoxIsStillTheBoundsAround"></a> TurnedFence\_WorldAxisBoxIsStillTheBoundsAround\(\)

```csharp
[Fact]
public void TurnedFence_WorldAxisBoxIsStillTheBoundsAround()
```

### <a id="Cube_Tests_TurnedBoxTests_TwoTurnedRectangles_CanMakeAnEightCornerFootprint"></a> TwoTurnedRectangles\_CanMakeAnEightCornerFootprint\(\)

```csharp
[Fact]
public void TwoTurnedRectangles_CanMakeAnEightCornerFootprint()
```
