# <a id="Cube_Tests_NodeSplitterTests"></a> Class NodeSplitterTests

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

Checks the node splitting of #127 against PROJECT.md, "Collision (phase 2)": the drawn boxes are what the player
would bump into, so a gap the player could walk through (about 0.5 yalm) must not be covered by a box, and no
triangle may ever be left uncovered. Also the bounded cache that keeps the pieces (PROJECT.md, "Bounded memory").

```csharp
public class NodeSplitterTests
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[NodeSplitterTests](Cube.Tests.NodeSplitterTests.md)


## Methods

### <a id="Cube_Tests_NodeSplitterTests_ABigTriangleSpanningTheGap_KeepsEverythingTogether"></a> ABigTriangleSpanningTheGap\_KeepsEverythingTogether\(\)

```csharp
[Fact]
public void ABigTriangleSpanningTheGap_KeepsEverythingTogether()
```

### <a id="Cube_Tests_NodeSplitterTests_Box_System_Single_System_Single_System_Single_System_Single_System_Single_System_Single_"></a> Box\(float, float, float, float, float, float\)

```csharp
private static LocalBox Box(float x0, float y0, float z0, float x1, float y1, float z1)
```

#### Parameters

`x0` [float](https://learn.microsoft.com/dotnet/api/system.single)

`y0` [float](https://learn.microsoft.com/dotnet/api/system.single)

`z0` [float](https://learn.microsoft.com/dotnet/api/system.single)

`x1` [float](https://learn.microsoft.com/dotnet/api/system.single)

`y1` [float](https://learn.microsoft.com/dotnet/api/system.single)

`z1` [float](https://learn.microsoft.com/dotnet/api/system.single)

#### Returns

 [LocalBox](Cube.Core.LocalBox.md)

### <a id="Cube_Tests_NodeSplitterTests_Cache_IsBounded_AndClearFreesTheRoom"></a> Cache\_IsBounded\_AndClearFreesTheRoom\(\)

```csharp
[Fact]
public void Cache_IsBounded_AndClearFreesTheRoom()
```

### <a id="Cube_Tests_NodeSplitterTests_Cache_LookupAndAdd_AllocateNothing"></a> Cache\_LookupAndAdd\_AllocateNothing\(\)

```csharp
[Fact]
public void Cache_LookupAndAdd_AllocateNothing()
```

### <a id="Cube_Tests_NodeSplitterTests_Cache_ReturnsWhatWasStored_AndMissesOnADifferentCheck"></a> Cache\_ReturnsWhatWasStored\_AndMissesOnADifferentCheck\(\)

```csharp
[Fact]
public void Cache_ReturnsWhatWasStored_AndMissesOnADifferentCheck()
```

### <a id="Cube_Tests_NodeSplitterTests_Cache_StoresAnEmptyNode_AsRefinedWithNoPieces"></a> Cache\_StoresAnEmptyNode\_AsRefinedWithNoPieces\(\)

```csharp
[Fact]
public void Cache_StoresAnEmptyNode_AsRefinedWithNoPieces()
```

### <a id="Cube_Tests_NodeSplitterTests_CounterWithAnOpening_SeventyEightTriangles_LeavesTheOpeningUncovered"></a> CounterWithAnOpening\_SeventyEightTriangles\_LeavesTheOpeningUncovered\(\)

```csharp
[Fact]
public void CounterWithAnOpening_SeventyEightTriangles_LeavesTheOpeningUncovered()
```

### <a id="Cube_Tests_NodeSplitterTests_Covers_Cube_Core_LocalBox_Cube_Core_LocalBox_"></a> Covers\(LocalBox, LocalBox\)

```csharp
private static bool Covers(LocalBox piece, LocalBox triangle)
```

#### Parameters

`piece` [LocalBox](Cube.Core.LocalBox.md)

`triangle` [LocalBox](Cube.Core.LocalBox.md)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Tests_NodeSplitterTests_GapNarrowerThanAPlayer_IsNotSplit"></a> GapNarrowerThanAPlayer\_IsNotSplit\(\)

```csharp
[Fact]
public void GapNarrowerThanAPlayer_IsNotSplit()
```

### <a id="Cube_Tests_NodeSplitterTests_GapOnTheVerticalAxis_Splits"></a> GapOnTheVerticalAxis\_Splits\(\)

```csharp
[Fact]
public void GapOnTheVerticalAxis_Splits()
```

### <a id="Cube_Tests_NodeSplitterTests_ManyScatteredTriangles_RespectTheCapAndStillCoverEveryTriangle"></a> ManyScatteredTriangles\_RespectTheCapAndStillCoverEveryTriangle\(\)

```csharp
[Fact]
public void ManyScatteredTriangles_RespectTheCapAndStillCoverEveryTriangle()
```

### <a id="Cube_Tests_NodeSplitterTests_NoTriangles_GivesNoPieces"></a> NoTriangles\_GivesNoPieces\(\)

```csharp
[Fact]
public void NoTriangles_GivesNoPieces()
```

### <a id="Cube_Tests_NodeSplitterTests_SingleFlatTriangle_GetsAMinimumThickness"></a> SingleFlatTriangle\_GetsAMinimumThickness\(\)

```csharp
[Fact]
public void SingleFlatTriangle_GetsAMinimumThickness()
```

### <a id="Cube_Tests_NodeSplitterTests_SmallerOutputSpan_LimitsThePieces"></a> SmallerOutputSpan\_LimitsThePieces\(\)

```csharp
[Fact]
public void SmallerOutputSpan_LimitsThePieces()
```

### <a id="Cube_Tests_NodeSplitterTests_Splitting_AllocatesNothing"></a> Splitting\_AllocatesNothing\(\)

```csharp
[Fact]
public void Splitting_AllocatesNothing()
```

### <a id="Cube_Tests_NodeSplitterTests_TouchingOrOverlappingTriangles_StayTogether"></a> TouchingOrOverlappingTriangles\_StayTogether\(\)

```csharp
[Fact]
public void TouchingOrOverlappingTriangles_StayTogether()
```

### <a id="Cube_Tests_NodeSplitterTests_TwoBlocksWithAGap_AreTwoPieces"></a> TwoBlocksWithAGap\_AreTwoPieces\(\)

```csharp
[Fact]
public void TwoBlocksWithAGap_AreTwoPieces()
```
