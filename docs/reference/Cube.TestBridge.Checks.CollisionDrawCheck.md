# <a id="Cube_TestBridge_Checks_CollisionDrawCheck"></a> Class CollisionDrawCheck

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

Checks the collision boxes are drawn in the running game (#31; PROJECT.md, "Collision (phase 2)": each collider
inside or crossing the cube is shown as a simplified box, clipped to the cube; "Draw order": far things first, with
the players).

```csharp
internal sealed class CollisionDrawCheck : BridgeCheck
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BridgeCheck](Cube.TestBridge.BridgeCheck.md) ← 
[CollisionDrawCheck](Cube.TestBridge.Checks.CollisionDrawCheck.md)


## Remarks

<p>
With the default settings (default view, from above), once a gather has completed: the cube holds clipped boxes,
every one inside the cube's height and footprint; every collision face belongs to one of them, at most three per
box, in the collision colour; and the floor under the player (found with the game's own downward raycast, as in
[CollisionReaderCheck](Cube.TestBridge.Checks.CollisionReaderCheck.md#Cube_TestBridge_Checks_CollisionReaderCheck)) is painted before the player's own cube, so the player stands on it rather than
under it. That the whole frame matches Core is [DrawListMatchesCoreCheck](Cube.TestBridge.Checks.DrawListMatchesCoreCheck.md#Cube_TestBridge_Checks_DrawListMatchesCoreCheck)'s job, and the snapshot
([FrameSnapshotCheck](Cube.TestBridge.Checks.FrameSnapshotCheck.md#Cube_TestBridge_Checks_FrameSnapshotCheck)) shows the boxes for the side-by-side with vnavmesh's debug view in
<code>docs/manual-tests.md</code>.
</p>

## Fields

### <a id="Cube_TestBridge_Checks_CollisionDrawCheck_Diagonal"></a> Diagonal

The cube's horizontal half-diagonal per unit of half-edge: the furthest its footprint reaches at any yaw.

```csharp
private const float Diagonal = 1.41422
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_CollisionDrawCheck_FramesAfterGather"></a> FramesAfterGather

Frames drawn after the gather before the frame is checked.

```csharp
private const int FramesAfterGather = 2
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionDrawCheck_Tolerance"></a> Tolerance

How far a clipped box may reach past the cube, in yalms (rounding).

```csharp
private const float Tolerance = 0.01
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_CollisionDrawCheck_saved"></a> saved

The user's settings before the check, restored by [Cleanup](Cube.TestBridge.Checks.CollisionDrawCheck.md#Cube_TestBridge_Checks_CollisionDrawCheck_Cleanup).

```csharp
private readonly CubeSettings saved
```

#### Field Value

 [CubeSettings](Cube.Core.CubeSettings.md)

### <a id="Cube_TestBridge_Checks_CollisionDrawCheck_wasOpen"></a> wasOpen

The window's open state before the check, restored by [Cleanup](Cube.TestBridge.Checks.CollisionDrawCheck.md#Cube_TestBridge_Checks_CollisionDrawCheck_Cleanup).

```csharp
private bool wasOpen
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Properties

### <a id="Cube_TestBridge_Checks_CollisionDrawCheck_Name"></a> Name

Short unique name, shown in the results, e.g. <code>window-opens-and-closes</code>.

```csharp
public override string Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Cube_TestBridge_Checks_CollisionDrawCheck_Timeout"></a> Timeout

The longest the check may run before it fails as timed out.

```csharp
public override TimeSpan Timeout { get; }
```

#### Property Value

 [TimeSpan](https://learn.microsoft.com/dotnet/api/system.timespan)

#### Remarks

Ten seconds covers checks that wait a few hundred frames; override for longer ones.

## Methods

### <a id="Cube_TestBridge_Checks_CollisionDrawCheck_CheckBoxesInCube_Cube_Core_CubeSpace__System_ReadOnlySpan_Cube_Core_CollisionBox__"></a> CheckBoxesInCube\(in CubeSpace, ReadOnlySpan<CollisionBox\>\)

Checks every clipped box lies within the cube's height and the reach of its footprint at any yaw.

```csharp
private static string? CheckBoxesInCube(in CubeSpace space, ReadOnlySpan<CollisionBox> boxes)
```

#### Parameters

`space` [CubeSpace](Cube.Core.CubeSpace.md)

The frame's cube.

`boxes` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[CollisionBox](Cube.Core.CollisionBox.md)\>

The clipped boxes.

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)?

What was wrong, or null.

### <a id="Cube_TestBridge_Checks_CollisionDrawCheck_CheckFaces_Cube_CubeWindow_System_Int32_System_Int32__System_Int32__"></a> CheckFaces\(CubeWindow, int, out int, out int\)

Checks every collision face belongs to a clipped box, in the collision colour, at most three per box.

```csharp
private static string? CheckFaces(CubeWindow window, int boxCount, out int faces, out int boxesWithFaces)
```

#### Parameters

`window` [CubeWindow](Cube.CubeWindow.md)

The cube window.

`boxCount` [int](https://learn.microsoft.com/dotnet/api/system.int32)

How many boxes were clipped.

`faces` [int](https://learn.microsoft.com/dotnet/api/system.int32)

How many collision faces were drawn.

`boxesWithFaces` [int](https://learn.microsoft.com/dotnet/api/system.int32)

How many boxes had at least one face.

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)?

What was wrong, or null.

### <a id="Cube_TestBridge_Checks_CollisionDrawCheck_CheckFloorBeforeSelf_Cube_CubeWindow_System_ReadOnlySpan_Cube_Core_CollisionBox__Cube_Core_CollisionBox_System_Int32__System_Int32__"></a> CheckFloorBeforeSelf\(CubeWindow, ReadOnlySpan<CollisionBox\>, CollisionBox, out int, out int\)

Checks the floor box's faces are all painted before the local player's cube.

```csharp
private static string? CheckFloorBeforeSelf(CubeWindow window, ReadOnlySpan<CollisionBox> boxes, CollisionBox floor, out int floorLast, out int selfFirst)
```

#### Parameters

`window` [CubeWindow](Cube.CubeWindow.md)

The cube window.

`boxes` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[CollisionBox](Cube.Core.CollisionBox.md)\>

The clipped boxes.

`floor` [CollisionBox](Cube.Core.CollisionBox.md)

The box holding the floor under the player.

`floorLast` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Draw-list index of the floor box's last face, or -1.

`selfFirst` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Draw-list index of the player's first face, or -1.

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)?

What was wrong, or null.

### <a id="Cube_TestBridge_Checks_CollisionDrawCheck_Cleanup_Cube_TestBridge_BridgeContext_"></a> Cleanup\(BridgeContext\)

Puts back anything the check changed (e.g. the window's open state). Called after every run,
including when the check failed, threw or timed out, so a check never leaves the plugin altered.

```csharp
public override void Cleanup(BridgeContext context)
```

#### Parameters

`context` [BridgeContext](Cube.TestBridge.BridgeContext.md)

The same context the check ran with.

### <a id="Cube_TestBridge_Checks_CollisionDrawCheck_Run_Cube_TestBridge_BridgeContext_"></a> Run\(BridgeContext\)

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
