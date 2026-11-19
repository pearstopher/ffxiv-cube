# <a id="Cube_TestBridge_Checks_MeshStyleCheck"></a> Class MeshStyleCheck

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

Checks the Mesh collision style in the running game (#100; PROJECT.md, "Collision style"): the gathered triangles
contain the game's own raycast hits, nothing is clipped or projected outside the cube at any yaw, the style's cost is
measured and reported, it allocates nothing per frame, and a snapshot of the scene is written.

```csharp
internal sealed class MeshStyleCheck : BridgeCheck
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BridgeCheck](Cube.TestBridge.BridgeCheck.md) ← 
[MeshStyleCheck](Cube.TestBridge.Checks.MeshStyleCheck.md)


## Remarks

<p>
<b>Raycasts.</b> Straight down from above the player on a grid; every hit inside the gathered region must lie within
[HitTolerance](Cube.TestBridge.Checks.MeshStyleCheck.md#Cube_TestBridge_Checks_MeshStyleCheck_HitTolerance) of a gathered triangle (as #28 did for boxes). The tolerance covers the flat sides a
16-sided cylinder or an 80-triangle sphere has where the real shape is round.
</p>
<p>
<b>Inside the cube.</b> For four yaws (the cube turns with the camera, so the clip does too) the check waits for the
clipped mesh to be rebuilt and verifies that every kept edge end is inside the cube and every projected end is inside
the cube's outline on screen, and verifies the outline every frame while the rebuild is still going (the stale picture
is clipped to the new cube at projection time).
</p>
<p>
<b>Cost.</b> The frame time of idle Mesh drawing is <em>reported</em> with no limit (PROJECT.md: Mesh mode is an
opt-in exception to the 0.5 ms budget); the check fails only on allocations, on a drawing outside the cube, on a
scene bigger than the buffers (dropped triangles or edges are reported), or on nothing being gathered. Settings change
in memory only and are restored in [Cleanup](Cube.TestBridge.Checks.MeshStyleCheck.md#Cube_TestBridge_Checks_MeshStyleCheck_Cleanup).
</p>

## Constructors

### <a id="Cube_TestBridge_Checks_MeshStyleCheck__ctor_System_String_"></a> MeshStyleCheck\(string\)

Checks the Mesh collision style in the running game (#100; PROJECT.md, "Collision style"): the gathered triangles
contain the game's own raycast hits, nothing is clipped or projected outside the cube at any yaw, the style's cost is
measured and reported, it allocates nothing per frame, and a snapshot of the scene is written.

```csharp
public MeshStyleCheck(string folder)
```

#### Parameters

`folder` [string](https://learn.microsoft.com/dotnet/api/system.string)

The bridge folder the snapshot dump is written to.

#### Remarks

<p>
<b>Raycasts.</b> Straight down from above the player on a grid; every hit inside the gathered region must lie within
[HitTolerance](Cube.TestBridge.Checks.MeshStyleCheck.md#Cube_TestBridge_Checks_MeshStyleCheck_HitTolerance) of a gathered triangle (as #28 did for boxes). The tolerance covers the flat sides a
16-sided cylinder or an 80-triangle sphere has where the real shape is round.
</p>
<p>
<b>Inside the cube.</b> For four yaws (the cube turns with the camera, so the clip does too) the check waits for the
clipped mesh to be rebuilt and verifies that every kept edge end is inside the cube and every projected end is inside
the cube's outline on screen, and verifies the outline every frame while the rebuild is still going (the stale picture
is clipped to the new cube at projection time).
</p>
<p>
<b>Cost.</b> The frame time of idle Mesh drawing is <em>reported</em> with no limit (PROJECT.md: Mesh mode is an
opt-in exception to the 0.5 ms budget); the check fails only on allocations, on a drawing outside the cube, on a
scene bigger than the buffers (dropped triangles or edges are reported), or on nothing being gathered. Settings change
in memory only and are restored in [Cleanup](Cube.TestBridge.Checks.MeshStyleCheck.md#Cube_TestBridge_Checks_MeshStyleCheck_Cleanup).
</p>

## Fields

### <a id="Cube_TestBridge_Checks_MeshStyleCheck_FramesToMeasure"></a> FramesToMeasure

Frames the idle cost is measured over.

```csharp
private const int FramesToMeasure = 120
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_MeshStyleCheck_HitTolerance"></a> HitTolerance

How far a raycast hit may be from a gathered triangle, in yalms.

```csharp
private const float HitTolerance = 0.25
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_MeshStyleCheck_OutlineSlack"></a> OutlineSlack

Slack on the screen outline test, in pixels.

```csharp
private const float OutlineSlack = 0.75
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_MeshStyleCheck_RayLength"></a> RayLength

How far the rays reach, in yalms.

```csharp
private const float RayLength = 30
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_MeshStyleCheck_RayStartHeight"></a> RayStartHeight

How far above the player the rays start, in yalms.

```csharp
private const float RayStartHeight = 2
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_MeshStyleCheck_SnapshotFileName"></a> SnapshotFileName

Name of the snapshot dump, read by <code>Cube.Snapshot</code> like [FileName](Cube.TestBridge.Checks.FrameSnapshotCheck.md#Cube_TestBridge_Checks_FrameSnapshotCheck_FileName).

```csharp
public const string SnapshotFileName = "drawlist-mesh.json"
```

#### Field Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Cube_TestBridge_Checks_MeshStyleCheck_saved"></a> saved

The user's settings before the check, restored by [Cleanup](Cube.TestBridge.Checks.MeshStyleCheck.md#Cube_TestBridge_Checks_MeshStyleCheck_Cleanup).

```csharp
private readonly CubeSettings saved
```

#### Field Value

 [CubeSettings](Cube.Core.CubeSettings.md)

### <a id="Cube_TestBridge_Checks_MeshStyleCheck_wasOpen"></a> wasOpen

The window's open state before the check, restored by [Cleanup](Cube.TestBridge.Checks.MeshStyleCheck.md#Cube_TestBridge_Checks_MeshStyleCheck_Cleanup).

```csharp
private bool wasOpen
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Properties

### <a id="Cube_TestBridge_Checks_MeshStyleCheck_Name"></a> Name

Short unique name, shown in the results, e.g. <code>window-opens-and-closes</code>.

```csharp
public override string Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Cube_TestBridge_Checks_MeshStyleCheck_Timeout"></a> Timeout

The longest the check may run before it fails as timed out.

```csharp
public override TimeSpan Timeout { get; }
```

#### Property Value

 [TimeSpan](https://learn.microsoft.com/dotnet/api/system.timespan)

#### Remarks

Ten seconds covers checks that wait a few hundred frames; override for longer ones.

## Methods

### <a id="Cube_TestBridge_Checks_MeshStyleCheck_CheckInside_Cube_CubeWindow_"></a> CheckInside\(CubeWindow\)

Whether every kept edge end of the finished build lies inside the cube it was clipped for.

```csharp
private static string? CheckInside(CubeWindow window)
```

#### Parameters

`window` [CubeWindow](Cube.CubeWindow.md)

The cube window.

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)?

### <a id="Cube_TestBridge_Checks_MeshStyleCheck_CheckRays_Cube_Core_CubeSpace__Cube_Core_TriangleSet_System_Int32__System_Int32__System_Numerics_Vector3__"></a> CheckRays\(in CubeSpace, TriangleSet, out int, out int, out Vector3\)

Casts rays straight down on a grid around the player and finds a gathered triangle for each hit in the region.

```csharp
private static string? CheckRays(in CubeSpace space, TriangleSet set, out int hits, out int misses, out Vector3 firstMiss)
```

#### Parameters

`space` [CubeSpace](Cube.Core.CubeSpace.md)

The cube.

`set` [TriangleSet](Cube.Core.TriangleSet.md)

The gathered triangles.

`hits` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Rays that hit something inside the gathered region.

`misses` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Of those, hits with no triangle within [HitTolerance](Cube.TestBridge.Checks.MeshStyleCheck.md#Cube_TestBridge_Checks_MeshStyleCheck_HitTolerance).

`firstMiss` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

Where the first such hit was.

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)?

A failure message, or null.

### <a id="Cube_TestBridge_Checks_MeshStyleCheck_Cleanup_Cube_TestBridge_BridgeContext_"></a> Cleanup\(BridgeContext\)

Puts back anything the check changed (e.g. the window's open state). Called after every run,
including when the check failed, threw or timed out, so a check never leaves the plugin altered.

```csharp
public override void Cleanup(BridgeContext context)
```

#### Parameters

`context` [BridgeContext](Cube.TestBridge.BridgeContext.md)

The same context the check ran with.

### <a id="Cube_TestBridge_Checks_MeshStyleCheck_Distance_System_Numerics_Vector3_System_Numerics_Vector3_System_Numerics_Vector3_System_Numerics_Vector3_"></a> Distance\(Vector3, Vector3, Vector3, Vector3\)

The distance from a point to a triangle (Ericson, Real-Time Collision Detection, closest point on a triangle).

```csharp
internal static float Distance(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
```

#### Parameters

`p` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The point.

`a` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

First corner.

`b` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

Second corner.

`c` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

Third corner.

#### Returns

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_MeshStyleCheck_Forward_System_Single_"></a> Forward\(float\)

The camera's forward direction for a cube yaw (see [CubeSpace](Cube.Core.CubeSpace.md#Cube_Core_CubeSpace): forward is (sin yaw, -cos yaw) in X and Z).

```csharp
private static Vector3 Forward(float yaw)
```

#### Parameters

`yaw` [float](https://learn.microsoft.com/dotnet/api/system.single)

The cube's yaw in radians.

#### Returns

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_TestBridge_Checks_MeshStyleCheck_OutlineProblem_Cube_CubeWindow_"></a> OutlineProblem\(CubeWindow\)

Whether every projected segment end of the last frame lies inside the cube's outline on screen.

```csharp
private static string? OutlineProblem(CubeWindow window)
```

#### Parameters

`window` [CubeWindow](Cube.CubeWindow.md)

The cube window.

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)?

### <a id="Cube_TestBridge_Checks_MeshStyleCheck_Run_Cube_TestBridge_BridgeContext_"></a> Run\(BridgeContext\)

Runs the check, one step per frame, on the framework thread. Yield [NextFrame](Cube.TestBridge.CheckStep.md#Cube_TestBridge_CheckStep_NextFrame)
to wait a frame; finish with [Pass](Cube.TestBridge.CheckStep.md#Cube_TestBridge_CheckStep_Pass) or [Fail](Cube.TestBridge.CheckStep.md#Cube_TestBridge_CheckStep_Fail).

```csharp
public override IEnumerable<CheckStep> Run(BridgeContext context)
```

#### Parameters

`context` [BridgeContext](Cube.TestBridge.BridgeContext.md)

The plugin parts the check can look at, and where it records metrics.

#### Returns

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[CheckStep](Cube.TestBridge.CheckStep.md)\>

### <a id="Cube_TestBridge_Checks_MeshStyleCheck_WaitForBuild_Cube_CubeWindow_System_Single_System_Action_System_String__"></a> WaitForBuild\(CubeWindow, float, Action<string\>?\)

Waits until the mesh has been gathered and a build has finished for the cube at a yaw, checking the screen
outline each frame on the way.

```csharp
private static IEnumerable<CheckStep> WaitForBuild(CubeWindow window, float yaw, Action<string>? onProblem)
```

#### Parameters

`window` [CubeWindow](Cube.CubeWindow.md)

The cube window.

`yaw` [float](https://learn.microsoft.com/dotnet/api/system.single)

The yaw the cube is to reach.

`onProblem` [Action](https://learn.microsoft.com/dotnet/api/system.action\-1)<[string](https://learn.microsoft.com/dotnet/api/system.string)\>?

Called with a description when a frame's projected segments lie outside the cube's outline.

#### Returns

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[CheckStep](Cube.TestBridge.CheckStep.md)\>
