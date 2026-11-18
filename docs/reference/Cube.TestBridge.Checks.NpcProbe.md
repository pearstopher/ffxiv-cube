# <a id="Cube_TestBridge_Checks_NpcProbe"></a> Class NpcProbe

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

Running totals for the NPC research in [GameStateProbeCheck](Cube.TestBridge.Checks.GameStateProbeCheck.md#Cube_TestBridge_Checks_GameStateProbeCheck) (#73): per-frame counts by
category, the largest count seen in any frame, the slot range each kind used, and name changes.

```csharp
internal sealed class NpcProbe
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[NpcProbe](Cube.TestBridge.Checks.NpcProbe.md)


## Remarks

The maximum per frame, not the total, is what sizes the NPC cap for #74: the cap bounds how many NPCs one
frame can hold. A check may allocate (it isn't the draw path), so plain dictionaries are used.

## Fields

### <a id="Cube_TestBridge_Checks_NpcProbe_current"></a> current

This frame's count per category, reset by [BeginFrame](Cube.TestBridge.Checks.NpcProbe.md#Cube_TestBridge_Checks_NpcProbe_BeginFrame).

```csharp
private readonly Dictionary<string, int> current
```

#### Field Value

 [Dictionary](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary\-2)<[string](https://learn.microsoft.com/dotnet/api/system.string), [int](https://learn.microsoft.com/dotnet/api/system.int32)\>

### <a id="Cube_TestBridge_Checks_NpcProbe_max"></a> max

The largest per-frame count seen so far, per category.

```csharp
private readonly Dictionary<string, int> max
```

#### Field Value

 [Dictionary](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary\-2)<[string](https://learn.microsoft.com/dotnet/api/system.string), [int](https://learn.microsoft.com/dotnet/api/system.int32)\>

### <a id="Cube_TestBridge_Checks_NpcProbe_names"></a> names

Last name bytes seen per NPC (entity id and slot), to count renames without logging names.

```csharp
private readonly Dictionary<ulong, byte[]> names
```

#### Field Value

 [Dictionary](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary\-2)<[ulong](https://learn.microsoft.com/dotnet/api/system.uint64), [byte](https://learn.microsoft.com/dotnet/api/system.byte)\[\]\>

### <a id="Cube_TestBridge_Checks_NpcProbe_slots"></a> slots

Lowest and highest object table index each kind was seen at.

```csharp
private readonly Dictionary<string, (int Min, int Max)> slots
```

#### Field Value

 [Dictionary](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary\-2)<[string](https://learn.microsoft.com/dotnet/api/system.string), \([int](https://learn.microsoft.com/dotnet/api/system.int32) [Min](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.int32,system.int32\-.min), [int](https://learn.microsoft.com/dotnet/api/system.int32) [Max](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.int32,system.int32\-.max)\)\>

## Properties

### <a id="Cube_TestBridge_Checks_NpcProbe_Frames"></a> Frames

Gets the number of frames sampled.

```csharp
public int Frames { get; private set; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_NpcProbe_MaxNamed"></a> MaxNamed

Gets the most NPCs passing the proposed #74 filter seen in one frame, anywhere in the table.

```csharp
public int MaxNamed { get; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_NpcProbe_MaxNamedInMaxCube"></a> MaxNamedInMaxCube

Gets the most NPCs passing the filter seen in one frame within the largest cube.

```csharp
public int MaxNamedInMaxCube { get; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_NpcProbe_NameChanges"></a> NameChanges

Gets how many times an NPC still in the same slot showed a different name than before.

```csharp
public int NameChanges { get; private set; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_NpcProbe_TableLength"></a> TableLength

Gets or sets the length of the game's index-sorted object table.

```csharp
public int TableLength { get; set; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Methods

### <a id="Cube_TestBridge_Checks_NpcProbe_Add_System_String_"></a> Add\(string\)

Adds one to this frame's count for a category.

```csharp
private void Add(string key)
```

#### Parameters

`key` [string](https://learn.microsoft.com/dotnet/api/system.string)

The category.

### <a id="Cube_TestBridge_Checks_NpcProbe_BeginFrame"></a> BeginFrame\(\)

Starts a frame: clears this frame's counts.

```csharp
public void BeginFrame()
```

### <a id="Cube_TestBridge_Checks_NpcProbe_CountNpc_System_Int32_FFXIVClientStructs_FFXIV_Client_Game_Object_ObjectKind_FFXIVClientStructs_FFXIV_Client_Game_Object_BattleNpcSubKind_System_Boolean_System_Int32_System_Boolean_"></a> CountNpc\(int, ObjectKind, BattleNpcSubKind, bool, int, bool\)

Counts one event or battle NPC under every category it belongs to.

```csharp
public void CountNpc(int slot, ObjectKind kind, BattleNpcSubKind subKind, bool owned, int nameLength, bool inMaxCube)
```

#### Parameters

`slot` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Its object table index.

`kind` ObjectKind

<code>EventNpc</code> or <code>BattleNpc</code>.

`subKind` BattleNpcSubKind

Its battle NPC sub-kind (ignored for event NPCs).

`owned` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether <code>OwnerId</code> names an owner.

`nameLength` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Bytes of name before the terminator; 0 means no name.

`inMaxCube` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether it is inside the largest cube the settings allow.

### <a id="Cube_TestBridge_Checks_NpcProbe_CountOther_FFXIVClientStructs_FFXIV_Client_Game_Object_ObjectKind_System_Int32_"></a> CountOther\(ObjectKind, int\)

Counts an object that is neither a player nor an NPC (minions, mounts, event objects...).

```csharp
public void CountOther(ObjectKind kind, int slot)
```

#### Parameters

`kind` ObjectKind

Its object kind.

`slot` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Its object table index.

### <a id="Cube_TestBridge_Checks_NpcProbe_EndFrame"></a> EndFrame\(\)

Ends a frame: folds this frame's counts into the maxima.

```csharp
public void EndFrame()
```

### <a id="Cube_TestBridge_Checks_NpcProbe_Record_Cube_TestBridge_BridgeContext_Dalamud_Plugin_Services_IPluginLog_"></a> Record\(BridgeContext, IPluginLog\)

Writes every total as a metric and one summary line to the log (counts only, no names).

```csharp
public void Record(BridgeContext context, IPluginLog log)
```

#### Parameters

`context` [BridgeContext](Cube.TestBridge.BridgeContext.md)

Where the metrics go.

`log` [IPluginLog](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Plugin/Services/IPluginLog.cs)

Where the summary goes, at Information.

### <a id="Cube_TestBridge_Checks_NpcProbe_Slot_System_String_System_Int32_"></a> Slot\(string, int\)

Widens a category's slot range to include a slot.

```csharp
private void Slot(string key, int slot)
```

#### Parameters

`key` [string](https://learn.microsoft.com/dotnet/api/system.string)

The category.

`slot` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The object table index seen.

### <a id="Cube_TestBridge_Checks_NpcProbe_TrackName_System_UInt64_System_ReadOnlySpan_System_Byte__"></a> TrackName\(ulong, ReadOnlySpan<byte\>\)

Remembers an NPC's name and counts a change if the same NPC had a different name before.

```csharp
public void TrackName(ulong key, ReadOnlySpan<byte> name)
```

#### Parameters

`key` [ulong](https://learn.microsoft.com/dotnet/api/system.uint64)

Entity id and slot together, so a reused slot isn't mistaken for a rename.

`name` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

The name bytes, without the terminator.
