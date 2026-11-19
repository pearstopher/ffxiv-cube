# <a id="Cube_Tests_ClippedFuzzTests"></a> Class ClippedFuzzTests

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

Fuzzes the clipper at 45° (#115; PROJECT.md, "Collision (phase 2)": clipped to the cube): whatever the box, a box
drawn as a plain rectangle must lie wholly inside the cube.

```csharp
public class ClippedFuzzTests
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ClippedFuzzTests](Cube.Tests.ClippedFuzzTests.md)


## Methods

### <a id="Cube_Tests_ClippedFuzzTests_PlainRectangleResult_IsInsideCube_ForRandomBoxes"></a> PlainRectangleResult\_IsInsideCube\_ForRandomBoxes\(\)

```csharp
[Fact]
public void PlainRectangleResult_IsInsideCube_ForRandomBoxes()
```

### <a id="Cube_Tests_ClippedFuzzTests_SliverAtCubeCorner_IsNotDrawnAsItsBoundingRectangle"></a> SliverAtCubeCorner\_IsNotDrawnAsItsBoundingRectangle\(\)

```csharp
[Fact]
public void SliverAtCubeCorner_IsNotDrawnAsItsBoundingRectangle()
```
