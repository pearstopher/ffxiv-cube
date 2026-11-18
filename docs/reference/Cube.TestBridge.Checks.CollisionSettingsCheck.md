# <a id="Cube_TestBridge_Checks_CollisionSettingsCheck"></a> Class CollisionSettingsCheck

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

Checks the collision settings in the running game (#32: collision on/off and detail level, saved and applied
immediately; PROJECT.md, "Collision (phase 2)").

```csharp
internal sealed class CollisionSettingsCheck : BridgeCheck
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BridgeCheck](Cube.TestBridge.BridgeCheck.md) ← 
[CollisionSettingsCheck](Cube.TestBridge.Checks.CollisionSettingsCheck.md)


## Remarks

<p>
From the defaults (full detail) it gathers once, then for each coarser level: changing the setting starts a new
gather at once while the old boxes stay drawn, and when it completes every full-detail box lies inside one of the
new, bigger boxes. That proves the reader's assumption ([CollisionDetail](Cube.Core.CollisionDetail.md#Cube_Core_CollisionDetail)) that a mesh node's box holds
its children's, so a coarser level never hides anything the player can bump into. Coarse must also give no more
boxes than full.
</p>
<p>
Then collision off: no boxes are clipped or drawn and no gather runs; and back on: boxes are gathered and drawn
again. Settings change in memory only (saving is the settings window's job, covered by <code>docs/manual-tests.md</code>)
and are restored in [Cleanup](Cube.TestBridge.Checks.CollisionSettingsCheck.md#Cube_TestBridge_Checks_CollisionSettingsCheck_Cleanup).
</p>

## Fields

### <a id="Cube_TestBridge_Checks_CollisionSettingsCheck_FramesOff"></a> FramesOff

Frames collision stays off before checking that no gather ran.

```csharp
private const int FramesOff = 10
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionSettingsCheck_Tolerance"></a> Tolerance

How far a full-detail box's centre may sit outside a coarser box, in yalms (rounding).

```csharp
private const float Tolerance = 0.05
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_TestBridge_Checks_CollisionSettingsCheck_saved"></a> saved

The user's settings before the check, restored by [Cleanup](Cube.TestBridge.Checks.CollisionSettingsCheck.md#Cube_TestBridge_Checks_CollisionSettingsCheck_Cleanup).

```csharp
private readonly CubeSettings saved
```

#### Field Value

 [CubeSettings](Cube.Core.CubeSettings.md)

### <a id="Cube_TestBridge_Checks_CollisionSettingsCheck_wasOpen"></a> wasOpen

The window's open state before the check, restored by [Cleanup](Cube.TestBridge.Checks.CollisionSettingsCheck.md#Cube_TestBridge_Checks_CollisionSettingsCheck_Cleanup).

```csharp
private bool wasOpen
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Properties

### <a id="Cube_TestBridge_Checks_CollisionSettingsCheck_Name"></a> Name

Short unique name, shown in the results, e.g. <code>window-opens-and-closes</code>.

```csharp
public override string Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Cube_TestBridge_Checks_CollisionSettingsCheck_Timeout"></a> Timeout

The longest the check may run before it fails as timed out.

```csharp
public override TimeSpan Timeout { get; }
```

#### Property Value

 [TimeSpan](https://learn.microsoft.com/dotnet/api/system.timespan)

#### Remarks

Ten seconds covers checks that wait a few hundred frames; override for longer ones.

## Methods

### <a id="Cube_TestBridge_Checks_CollisionSettingsCheck_CheckCovered_Cube_Core_CollisionBox___System_ReadOnlySpan_Cube_Core_CollisionBox__Cube_Core_CollisionBox_Cube_Core_CollisionDetail_"></a> CheckCovered\(CollisionBox\[\], ReadOnlySpan<CollisionBox\>, CollisionBox, CollisionDetail\)

Checks every full-detail box whose centre is inside the coarser gather's region has its centre inside one of
the coarser boxes: a coarser level only loosens the shape, it never loses anything.

```csharp
private static string? CheckCovered(CollisionBox[] full, ReadOnlySpan<CollisionBox> coarser, CollisionBox region, CollisionDetail detail)
```

#### Parameters

`full` [CollisionBox](Cube.Core.CollisionBox.md)\[\]

The full-detail boxes.

`coarser` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[CollisionBox](Cube.Core.CollisionBox.md)\>

The coarser level's boxes.

`region` [CollisionBox](Cube.Core.CollisionBox.md)

The region the coarser level was gathered for.

`detail` [CollisionDetail](Cube.Core.CollisionDetail.md)

The coarser level, for the message.

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)?

What was wrong, or null.

### <a id="Cube_TestBridge_Checks_CollisionSettingsCheck_Cleanup_Cube_TestBridge_BridgeContext_"></a> Cleanup\(BridgeContext\)

Puts back anything the check changed (e.g. the window's open state). Called after every run,
including when the check failed, threw or timed out, so a check never leaves the plugin altered.

```csharp
public override void Cleanup(BridgeContext context)
```

#### Parameters

`context` [BridgeContext](Cube.TestBridge.BridgeContext.md)

The same context the check ran with.

### <a id="Cube_TestBridge_Checks_CollisionSettingsCheck_CountCollisionFaces_Cube_CubeWindow_"></a> CountCollisionFaces\(CubeWindow\)

Counts the collision faces in the last frame.

```csharp
private static int CountCollisionFaces(CubeWindow window)
```

#### Parameters

`window` [CubeWindow](Cube.CubeWindow.md)

The cube window.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionSettingsCheck_Inside_System_Numerics_Vector3_Cube_Core_CollisionBox__System_Single_"></a> Inside\(Vector3, in CollisionBox, float\)

Whether a point is inside a box, allowing a tolerance.

```csharp
private static bool Inside(Vector3 point, in CollisionBox box, float tolerance)
```

#### Parameters

`point` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The point.

`box` [CollisionBox](Cube.Core.CollisionBox.md)

The box.

`tolerance` [float](https://learn.microsoft.com/dotnet/api/system.single)

How far outside still counts, in yalms.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_TestBridge_Checks_CollisionSettingsCheck_Run_Cube_TestBridge_BridgeContext_"></a> Run\(BridgeContext\)

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

### <a id="Cube_TestBridge_Checks_CollisionSettingsCheck_WaitForGather_Cube_Core_CollisionCache_System_Int64_"></a> WaitForGather\(CollisionCache, long\)

Waits until a gather begun after <code class="paramref">completedBefore</code> gathers has finished.

```csharp
private static IEnumerable<CheckStep> WaitForGather(CollisionCache cache, long completedBefore)
```

#### Parameters

`cache` [CollisionCache](Cube.Core.CollisionCache.md)

The collision cache.

`completedBefore` [long](https://learn.microsoft.com/dotnet/api/system.int64)

The cache's completed count before the change.

#### Returns

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[CheckStep](Cube.TestBridge.CheckStep.md)\>
