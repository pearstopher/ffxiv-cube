# <a id="Cube_Core_NodePieceCache"></a> Class NodePieceCache

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

Remembers the pieces each collision mesh node was split into ([NodeSplitter](Cube.Core.NodeSplitter.md#Cube_Core_NodeSplitter)), so the split is worked out
once per node and a gather only transforms the kept pieces (#127). Bounded and dropped on zone change, like the box
sets (PROJECT.md, "Bounded memory").

```csharp
public sealed class NodePieceCache
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[NodePieceCache](Cube.Core.NodePieceCache.md)


## Remarks

<p>
<b>Keyed by node address.</b> A node's triangles don't change while its mesh is loaded, so its split is fixed. The
address alone could be reused by a different mesh after the old one unloads, so each entry also stores a
<code>check</code> number (the reader makes it from the node's triangle count and bounds); a lookup whose check differs is
a miss, and the stale entry is replaced.
</p>
<p>
<b>Fixed size.</b> Pieces live in one array of [DefaultCapacity](Cube.Core.NodePieceCache.md#Cube_Core_NodePieceCache_DefaultCapacity) pieces (24 bytes each, about 96 KB) and
the index is a dictionary created once at that size, so nothing grows with time in game. A node that can't be split
cheaply (too many triangles) is stored with no pieces, meaning "use the node's own box", so it isn't tried again.
When the cache is full [Add](Cube.Core.NodePieceCache.md#Cube_Core_NodePieceCache_Add) says so, and the reader drops everything and refines again from what is
near the player (the region has a fixed box cap, so that is bounded too).
</p>

## Constructors

### <a id="Cube_Core_NodePieceCache__ctor_System_Int32_"></a> NodePieceCache\(int\)

Creates an empty cache.

```csharp
public NodePieceCache(int capacity = 4000)
```

#### Parameters

`capacity` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Most pieces (and so most nodes) kept.

## Fields

### <a id="Cube_Core_NodePieceCache_DefaultCapacity"></a> DefaultCapacity

Most pieces kept (the #125 recommendation: 1,042 nodes made 1,183 pieces in a house).

```csharp
public const int DefaultCapacity = 4000
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_NodePieceCache_entries"></a> entries

Where each node's pieces are.

```csharp
private readonly Dictionary<ulong, NodePieceCache.Entry> entries
```

#### Field Value

 [Dictionary](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary\-2)<[ulong](https://learn.microsoft.com/dotnet/api/system.uint64), [NodePieceCache](Cube.Core.NodePieceCache.md).[Entry](Cube.Core.NodePieceCache.Entry.md)\>

### <a id="Cube_Core_NodePieceCache_pieces"></a> pieces

The kept pieces, one entry's pieces after another.

```csharp
private readonly LocalBox[] pieces
```

#### Field Value

 [LocalBox](Cube.Core.LocalBox.md)\[\]

### <a id="Cube_Core_NodePieceCache_used"></a> used

How many slots of [pieces](Cube.Core.NodePieceCache.md#Cube_Core_NodePieceCache_pieces) are used.

```csharp
private int used
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Properties

### <a id="Cube_Core_NodePieceCache_Capacity"></a> Capacity

The most pieces kept.

```csharp
public int Capacity { get; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_NodePieceCache_Nodes"></a> Nodes

How many nodes are remembered.

```csharp
public int Nodes { get; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_NodePieceCache_Pieces"></a> Pieces

How many pieces are kept.

```csharp
public int Pieces { get; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Methods

### <a id="Cube_Core_NodePieceCache_Add_System_UInt64_System_UInt32_System_ReadOnlySpan_Cube_Core_LocalBox__"></a> Add\(ulong, uint, ReadOnlySpan<LocalBox\>\)

Stores a node's pieces.

```csharp
public bool Add(ulong key, uint check, ReadOnlySpan<LocalBox> nodePieces)
```

#### Parameters

`key` [ulong](https://learn.microsoft.com/dotnet/api/system.uint64)

The node's address.

`check` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

The node's check number.

`nodePieces` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[LocalBox](Cube.Core.LocalBox.md)\>

The pieces; empty means "draw the node's own box".

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

False if there is no room (the node stays unrefined).

### <a id="Cube_Core_NodePieceCache_Clear"></a> Clear\(\)

Drops everything (zone change). The buffers are kept for reuse.

```csharp
public void Clear()
```

### <a id="Cube_Core_NodePieceCache_TryGet_System_UInt64_System_UInt32_System_ReadOnlySpan_Cube_Core_LocalBox___"></a> TryGet\(ulong, uint, out ReadOnlySpan<LocalBox\>\)

Looks a node up.

```csharp
public bool TryGet(ulong key, uint check, out ReadOnlySpan<LocalBox> found)
```

#### Parameters

`key` [ulong](https://learn.microsoft.com/dotnet/api/system.uint64)

The node's address.

`check` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

The node's check number; must match what was stored.

`found` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[LocalBox](Cube.Core.LocalBox.md)\>

The node's pieces; empty for a node that is to be drawn as its own box.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

True if the node has been refined (even to "no pieces").
