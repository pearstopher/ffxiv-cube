# <a id="Cube_Tests_MeshStyleTests"></a> Class MeshStyleTests

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

Checks the Mesh collision style's rules (PROJECT.md, "Collision style", #100): the real triangles are drawn as they
are and exactly clipped to the cube; nothing is drawn outside it; primitive colliders become their real shapes;
a scene bigger than the buffer is reported, not silently cut; gathering is budgeted and dropped on a zone change;
rebuilding happens only when inputs change; the per-frame path allocates nothing; Boxes stays the default and old
settings files keep it.

```csharp
public class MeshStyleTests
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[MeshStyleTests](Cube.Tests.MeshStyleTests.md)


## Fields

### <a id="Cube_Tests_MeshStyleTests_Edge"></a> Edge

```csharp
private static readonly Vector3 Edge
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Tests_MeshStyleTests_Stamp"></a> Stamp

```csharp
private const ulong Stamp = 7
```

#### Field Value

 [ulong](https://learn.microsoft.com/dotnet/api/system.uint64)

### <a id="Cube_Tests_MeshStyleTests_Zone"></a> Zone

```csharp
private const uint Zone = 132
```

#### Field Value

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

## Methods

### <a id="Cube_Tests_MeshStyleTests_BadStyleValue_IsRepairedToBoxes"></a> BadStyleValue\_IsRepairedToBoxes\(\)

```csharp
[Fact]
public void BadStyleValue_IsRepairedToBoxes()
```

### <a id="Cube_Tests_MeshStyleTests_Box_IsTwelveTrianglesOnItsEightCorners"></a> Box\_IsTwelveTrianglesOnItsEightCorners\(\)

```csharp
[Fact]
public void Box_IsTwelveTrianglesOnItsEightCorners()
```

### <a id="Cube_Tests_MeshStyleTests_BoxesIsTheDefaultStyle_AndOldFilesKeepIt"></a> BoxesIsTheDefaultStyle\_AndOldFilesKeepIt\(\)

```csharp
[Fact]
public void BoxesIsTheDefaultStyle_AndOldFilesKeepIt()
```

### <a id="Cube_Tests_MeshStyleTests_Build_Cube_Core_CubeSpace__Cube_Core_TriangleCache_System_Int32_"></a> Build\(in CubeSpace, TriangleCache, int\)

A clipped mesh built in one go from a cache.

```csharp
private static ClippedMesh Build(in CubeSpace space, TriangleCache cache, int capacity = 4096)
```

#### Parameters

`space` [CubeSpace](Cube.Core.CubeSpace.md)

The cube to clip to.

`cache` [TriangleCache](Cube.Core.TriangleCache.md)

The gathered triangles.

`capacity` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Most edges kept.

#### Returns

 [ClippedMesh](Cube.Core.ClippedMesh.md)

### <a id="Cube_Tests_MeshStyleTests_CacheWith_System_Numerics_Vector3___"></a> CacheWith\(params Vector3\[\]\)

A cache holding the given triangles (three corners each), as if gathered around the origin.

```csharp
private static TriangleCache CacheWith(params Vector3[] corners)
```

#### Parameters

`corners` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)\[\]

The triangles' corners, three per triangle.

#### Returns

 [TriangleCache](Cube.Core.TriangleCache.md)

### <a id="Cube_Tests_MeshStyleTests_CopyFrom_CopiesTheStyle"></a> CopyFrom\_CopiesTheStyle\(\)

```csharp
[Fact]
public void CopyFrom_CopiesTheStyle()
```

### <a id="Cube_Tests_MeshStyleTests_Cube_System_Single_"></a> Cube\(float\)

A cube 40 yalms across at the origin.

```csharp
private static CubeSpace Cube(float yaw = 0)
```

#### Parameters

`yaw` [float](https://learn.microsoft.com/dotnet/api/system.single)

How far the cube is turned, in radians.

#### Returns

 [CubeSpace](Cube.Core.CubeSpace.md)

### <a id="Cube_Tests_MeshStyleTests_Cylinder_HasItsRimOnTheUnitCircleAndCapsAtItsEnds"></a> Cylinder\_HasItsRimOnTheUnitCircleAndCapsAtItsEnds\(\)

```csharp
[Fact]
public void Cylinder_HasItsRimOnTheUnitCircleAndCapsAtItsEnds()
```

### <a id="Cube_Tests_MeshStyleTests_EdgeSharedByTwoTriangles_IsKeptOnce"></a> EdgeSharedByTwoTriangles\_IsKeptOnce\(\)

```csharp
[Fact]
public void EdgeSharedByTwoTriangles_IsKeptOnce()
```

### <a id="Cube_Tests_MeshStyleTests_Gather_IsSpreadOverFramesWithinTheBudget_AndFinishes"></a> Gather\_IsSpreadOverFramesWithinTheBudget\_AndFinishes\(\)

```csharp
[Fact]
public void Gather_IsSpreadOverFramesWithinTheBudget_AndFinishes()
```

### <a id="Cube_Tests_MeshStyleTests_KeptEdgesBeyondTheBuffer_AreCountedNotSilentlyDropped"></a> KeptEdgesBeyondTheBuffer\_AreCountedNotSilentlyDropped\(\)

```csharp
[Fact]
public void KeptEdgesBeyondTheBuffer_AreCountedNotSilentlyDropped()
```

### <a id="Cube_Tests_MeshStyleTests_Plane_IsTwoTrianglesInItsOwnXAndYPlane"></a> Plane\_IsTwoTrianglesInItsOwnXAndYPlane\(\)

```csharp
[Fact]
public void Plane_IsTwoTrianglesInItsOwnXAndYPlane()
```

### <a id="Cube_Tests_MeshStyleTests_Project_OnlyRunsAgainWhenItsInputsChange"></a> Project\_OnlyRunsAgainWhenItsInputsChange\(\)

```csharp
[Fact]
public void Project_OnlyRunsAgainWhenItsInputsChange()
```

### <a id="Cube_Tests_MeshStyleTests_ProjectedSegments_AreInsideTheCubesOutline"></a> ProjectedSegments\_AreInsideTheCubesOutline\(\)

```csharp
[Fact]
public void ProjectedSegments_AreInsideTheCubesOutline()
```

### <a id="Cube_Tests_MeshStyleTests_Rebuild_IsSpreadOverFramesAndStartedBuildsAlwaysFinish"></a> Rebuild\_IsSpreadOverFramesAndStartedBuildsAlwaysFinish\(\)

```csharp
[Fact]
public void Rebuild_IsSpreadOverFramesAndStartedBuildsAlwaysFinish()
```

### <a id="Cube_Tests_MeshStyleTests_Release_FreesTheBuffers_AndBoxesModeHoldsNothing"></a> Release\_FreesTheBuffers\_AndBoxesModeHoldsNothing\(\)

```csharp
[Fact]
public void Release_FreesTheBuffers_AndBoxesModeHoldsNothing()
```

### <a id="Cube_Tests_MeshStyleTests_SegmentClip_CutsAtTheFace"></a> SegmentClip\_CutsAtTheFace\(\)

```csharp
[Fact]
public void SegmentClip_CutsAtTheFace()
```

### <a id="Cube_Tests_MeshStyleTests_SegmentClip_LeavesAnInsideSegmentUntouched_AndRejectsOutsideOnes"></a> SegmentClip\_LeavesAnInsideSegmentUntouched\_AndRejectsOutsideOnes\(\)

```csharp
[Fact]
public void SegmentClip_LeavesAnInsideSegmentUntouched_AndRejectsOutsideOnes()
```

### <a id="Cube_Tests_MeshStyleTests_SharedEdgeCutByTheCube_IsStillKeptOnce"></a> SharedEdgeCutByTheCube\_IsStillKeptOnce\(\)

```csharp
[Fact]
public void SharedEdgeCutByTheCube_IsStillKeptOnce()
```

### <a id="Cube_Tests_MeshStyleTests_Sphere_HasAllItsCornersOnTheSurface"></a> Sphere\_HasAllItsCornersOnTheSurface\(\)

```csharp
[Fact]
public void Sphere_HasAllItsCornersOnTheSurface()
```

### <a id="Cube_Tests_MeshStyleTests_StaleResult_IsClippedToTheNewCubeWhenProjected"></a> StaleResult\_IsClippedToTheNewCubeWhenProjected\(\)

```csharp
[Fact]
public void StaleResult_IsClippedToTheNewCubeWhenProjected()
```

### <a id="Cube_Tests_MeshStyleTests_SteadyState_UpdateAndProject_AllocateNothing"></a> SteadyState\_UpdateAndProject\_AllocateNothing\(\)

```csharp
[Fact]
public void SteadyState_UpdateAndProject_AllocateNothing()
```

### <a id="Cube_Tests_MeshStyleTests_TriangleCrossingAFace_IsClippedExactlyAtTheFace"></a> TriangleCrossingAFace\_IsClippedExactlyAtTheFace\(\)

```csharp
[Fact]
public void TriangleCrossingAFace_IsClippedExactlyAtTheFace()
```

### <a id="Cube_Tests_MeshStyleTests_TriangleOutsideTheCube_IsDropped"></a> TriangleOutsideTheCube\_IsDropped\(\)

```csharp
[Fact]
public void TriangleOutsideTheCube_IsDropped()
```

### <a id="Cube_Tests_MeshStyleTests_TriangleSet_KeepsATriangleThatOnlyCrossesIntoTheRegion"></a> TriangleSet\_KeepsATriangleThatOnlyCrossesIntoTheRegion\(\)

```csharp
[Fact]
public void TriangleSet_KeepsATriangleThatOnlyCrossesIntoTheRegion()
```

### <a id="Cube_Tests_MeshStyleTests_TriangleSet_RejectsTrianglesOutsideTheRegionAndNonFiniteOnes"></a> TriangleSet\_RejectsTrianglesOutsideTheRegionAndNonFiniteOnes\(\)

```csharp
[Fact]
public void TriangleSet_RejectsTrianglesOutsideTheRegionAndNonFiniteOnes()
```

### <a id="Cube_Tests_MeshStyleTests_TriangleWhollyInside_IsKeptAsItIs"></a> TriangleWhollyInside\_IsKeptAsItIs\(\)

```csharp
[Fact]
public void TriangleWhollyInside_IsKeptAsItIs()
```

### <a id="Cube_Tests_MeshStyleTests_TrianglesBeyondTheBuffer_AreCountedNotSilentlyDropped"></a> TrianglesBeyondTheBuffer\_AreCountedNotSilentlyDropped\(\)

```csharp
[Fact]
public void TrianglesBeyondTheBuffer_AreCountedNotSilentlyDropped()
```

### <a id="Cube_Tests_MeshStyleTests_TurnedCube_NothingKeptLiesOutsideIt_System_Single_"></a> TurnedCube\_NothingKeptLiesOutsideIt\(float\)

```csharp
[Theory]
[InlineData(new object[] { 0 })]
[InlineData(new object[] { 0.5 })]
[InlineData(new object[] { 1.2 })]
[InlineData(new object[] { 2.9 })]
public void TurnedCube_NothingKeptLiesOutsideIt(float yaw)
```

#### Parameters

`yaw` [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_MeshStyleTests_Update_OnlyRebuildsWhenItsInputsChange"></a> Update\_OnlyRebuildsWhenItsInputsChange\(\)

```csharp
[Fact]
public void Update_OnlyRebuildsWhenItsInputsChange()
```

### <a id="Cube_Tests_MeshStyleTests_View_Cube_Core_CubeSpace__"></a> View\(in CubeSpace\)

The default view fitted to a 400 pixel square window.

```csharp
private static ViewProjection View(in CubeSpace space)
```

#### Parameters

`space` [CubeSpace](Cube.Core.CubeSpace.md)

The cube.

#### Returns

 [ViewProjection](Cube.Core.ViewProjection.md)

### <a id="Cube_Tests_MeshStyleTests_ZoneChange_DropsTheTriangles"></a> ZoneChange\_DropsTheTriangles\(\)

```csharp
[Fact]
public void ZoneChange_DropsTheTriangles()
```
