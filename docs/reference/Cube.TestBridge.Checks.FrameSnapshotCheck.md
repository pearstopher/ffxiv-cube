# <a id="Cube_TestBridge_Checks_FrameSnapshotCheck"></a> Class FrameSnapshotCheck

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

Dumps one frame's draw list to <code>drawlist.json</code> in the bridge folder (PROJECT.md, "Automated testing": the
bridge dumps the frame's draw list; the snapshot tool turns it into a PNG so Claude can see what was drawn).

```csharp
internal sealed class FrameSnapshotCheck : BridgeCheck
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BridgeCheck](Cube.TestBridge.BridgeCheck.md) ← 
[FrameSnapshotCheck](Cube.TestBridge.Checks.FrameSnapshotCheck.md)


## Remarks

The format is read by <code>Cube.Snapshot.SnapshotRenderer</code>. It contains only geometry and colours; player names
are never written, only each label's length (PROJECT.md, "Privacy"). Written atomically, like the results.

## Constructors

### <a id="Cube_TestBridge_Checks_FrameSnapshotCheck__ctor_System_String_"></a> FrameSnapshotCheck\(string\)

Dumps one frame's draw list to <code>drawlist.json</code> in the bridge folder (PROJECT.md, "Automated testing": the
bridge dumps the frame's draw list; the snapshot tool turns it into a PNG so Claude can see what was drawn).

```csharp
public FrameSnapshotCheck(string folder)
```

#### Parameters

`folder` [string](https://learn.microsoft.com/dotnet/api/system.string)

The bridge folder to write into.

#### Remarks

The format is read by <code>Cube.Snapshot.SnapshotRenderer</code>. It contains only geometry and colours; player names
are never written, only each label's length (PROJECT.md, "Privacy"). Written atomically, like the results.

## Fields

### <a id="Cube_TestBridge_Checks_FrameSnapshotCheck_FileName"></a> FileName

Name of the dump file.

```csharp
public const string FileName = "drawlist.json"
```

#### Field Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Cube_TestBridge_Checks_FrameSnapshotCheck_FramesToWait"></a> FramesToWait

Frames to wait for the window to draw after opening it.

```csharp
private const int FramesToWait = 30
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_FrameSnapshotCheck_wasOpen"></a> wasOpen

The window's open state before the check, restored by [Cleanup](Cube.TestBridge.Checks.FrameSnapshotCheck.md#Cube_TestBridge_Checks_FrameSnapshotCheck_Cleanup).

```csharp
private bool wasOpen
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Properties

### <a id="Cube_TestBridge_Checks_FrameSnapshotCheck_Name"></a> Name

Short unique name, shown in the results, e.g. <code>window-opens-and-closes</code>.

```csharp
public override string Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

## Methods

### <a id="Cube_TestBridge_Checks_FrameSnapshotCheck_Cleanup_Cube_TestBridge_BridgeContext_"></a> Cleanup\(BridgeContext\)

Puts back anything the check changed (e.g. the window's open state). Called after every run,
including when the check failed, threw or timed out, so a check never leaves the plugin altered.

```csharp
public override void Cleanup(BridgeContext context)
```

#### Parameters

`context` [BridgeContext](Cube.TestBridge.BridgeContext.md)

The same context the check ran with.

### <a id="Cube_TestBridge_Checks_FrameSnapshotCheck_Dump_Cube_CubeWindow_"></a> Dump\(CubeWindow\)

Serialises the window's last frame. Also used by [ViewControlCheck](Cube.TestBridge.Checks.ViewControlCheck.md#Cube_TestBridge_Checks_ViewControlCheck) for its angle snapshots.

```csharp
internal static string Dump(CubeWindow window)
```

#### Parameters

`window` [CubeWindow](Cube.CubeWindow.md)

The cube window.

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Cube_TestBridge_Checks_FrameSnapshotCheck_Run_Cube_TestBridge_BridgeContext_"></a> Run\(BridgeContext\)

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

### <a id="Cube_TestBridge_Checks_FrameSnapshotCheck_WritePoint_System_Text_Json_Utf8JsonWriter_System_String_System_Numerics_Vector2_"></a> WritePoint\(Utf8JsonWriter, string?, Vector2\)

Writes <code>[x, y]</code>, as a named property or an array element.

```csharp
private static void WritePoint(Utf8JsonWriter json, string? name, Vector2 point)
```

#### Parameters

`json` [Utf8JsonWriter](https://learn.microsoft.com/dotnet/api/system.text.json.utf8jsonwriter)

The writer.

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)?

Property name, or null inside an array.

`point` [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

The point.
