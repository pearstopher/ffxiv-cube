# <a id="Cube_Core_NodeSplitter"></a> Class NodeSplitter

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

Splits one collision mesh node's box into pieces wherever its triangles leave a gap the player could walk through
(PROJECT.md, "Collision (phase 2)": the boxes are what the player would bump into; #127, from the #125 research in
<code>docs/research-notes.md</code>).

```csharp
public static class NodeSplitter
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[NodeSplitter](Cube.Core.NodeSplitter.md)


## Remarks

<p>
<b>Why.</b> A mesh leaf node holds only about 9 to 16 triangles, but its box is the tight box around all of them, so
when they lie far apart (a counter's two blocks, a room's floor edge and far wall) the box covers the gap between
them: a big empty square, or a counter with no opening. Splitting each node's triangles at gaps cut the covered
volume in a house to 29% for 14% more boxes.
</p>
<p>
<b>How.</b> Sort the triangles' own bounds along an axis and cut wherever the next one starts at least
[Gap](Cube.Core.NodeSplitter.md#Cube_Core_NodeSplitter_Gap) beyond the furthest end so far; every group is again tested along all three axes, and a group that
can't be cut becomes one box. [Gap](Cube.Core.NodeSplitter.md#Cube_Core_NodeSplitter_Gap) is 0.5 yalm, about the player's width: a narrower gap can't be walked
through, so it needn't be shown. The work is on small spans, with an insertion sort and no allocation, so it can run
in the frame (CLAUDE.md, "Efficiency and memory").
</p>
<p>
<b>Never holes.</b> At most <code>maxPieces</code> boxes come out; when the room runs out, the rest of that group is
covered by one box (a coarser look, never a missing piece). Every triangle's bounds always lie inside one of the
pieces, so the pieces together cover everything the node's own box covered that has a triangle in it.
</p>

## Fields

### <a id="Cube_Core_NodeSplitter_Gap"></a> Gap

The narrowest gap, in yalms, that splits a node: about the player's width.

```csharp
public const float Gap = 0.5
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_NodeSplitter_MaxPiecesPerNode"></a> MaxPiecesPerNode

Most pieces one node is split into, so a scattered node can't flood the box cap.

```csharp
public const int MaxPiecesPerNode = 8
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_NodeSplitter_MinThickness"></a> MinThickness

The thinnest a piece is drawn, in yalms, along each axis. A wall or floor of axis-aligned triangles has no
thickness at all, and a sheet of no thickness would vanish; a few centimetres is enough.

```csharp
public const float MinThickness = 0.05
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

## Methods

### <a id="Cube_Core_NodeSplitter_SortByMin_System_Span_Cube_Core_LocalBox__System_Int32_"></a> SortByMin\(Span<LocalBox\>, int\)

Insertion sort by the low end along an axis; groups are a handful of triangles, so it is the cheapest sort.

```csharp
private static void SortByMin(Span<LocalBox> group, int axis)
```

#### Parameters

`group` [Span](https://learn.microsoft.com/dotnet/api/system.span\-1)<[LocalBox](Cube.Core.LocalBox.md)\>

The boxes to sort in place.

`axis` [int](https://learn.microsoft.com/dotnet/api/system.int32)

0 for X, 1 for Y, 2 for Z.

### <a id="Cube_Core_NodeSplitter_Split_System_Span_Cube_Core_LocalBox__System_Span_Cube_Core_LocalBox__"></a> Split\(Span<LocalBox\>, Span<LocalBox\>\)

Splits the triangles' bounds into pieces. Reorders <code class="paramref">triangles</code> in place (it is the scratch space).

```csharp
public static int Split(Span<LocalBox> triangles, Span<LocalBox> pieces)
```

#### Parameters

`triangles` [Span](https://learn.microsoft.com/dotnet/api/system.span\-1)<[LocalBox](Cube.Core.LocalBox.md)\>

The bounds of each triangle of one node, in the mesh's local space.

`pieces` [Span](https://learn.microsoft.com/dotnet/api/system.span\-1)<[LocalBox](Cube.Core.LocalBox.md)\>

Receives the pieces; also limits how many there can be (up to [MaxPiecesPerNode](Cube.Core.NodeSplitter.md#Cube_Core_NodeSplitter_MaxPiecesPerNode)).

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

How many pieces were written; zero only if there were no triangles.

### <a id="Cube_Core_NodeSplitter_SplitGroup_System_Span_Cube_Core_LocalBox__System_Span_Cube_Core_LocalBox__System_Int32_System_Int32_"></a> SplitGroup\(Span<LocalBox\>, Span<LocalBox\>, int, int\)

Splits one group, writing its pieces from <code class="paramref">start</code>.

```csharp
private static int SplitGroup(Span<LocalBox> group, Span<LocalBox> pieces, int start, int room)
```

#### Parameters

`group` [Span](https://learn.microsoft.com/dotnet/api/system.span\-1)<[LocalBox](Cube.Core.LocalBox.md)\>

The group's triangle bounds.

`pieces` [Span](https://learn.microsoft.com/dotnet/api/system.span\-1)<[LocalBox](Cube.Core.LocalBox.md)\>

The output.

`start` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Where this group's first piece goes.

`room` [int](https://learn.microsoft.com/dotnet/api/system.int32)

How many pieces this group may use.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

How many pieces the group used.

### <a id="Cube_Core_NodeSplitter_Thicken_Cube_Core_LocalBox_"></a> Thicken\(LocalBox\)

Grows a box evenly about its centre along any axis thinner than [MinThickness](Cube.Core.NodeSplitter.md#Cube_Core_NodeSplitter_MinThickness).

```csharp
private static LocalBox Thicken(LocalBox box)
```

#### Parameters

`box` [LocalBox](Cube.Core.LocalBox.md)

The piece.

#### Returns

 [LocalBox](Cube.Core.LocalBox.md)

### <a id="Cube_Core_NodeSplitter_TryCut_System_Span_Cube_Core_LocalBox__System_Int32__"></a> TryCut\(Span<LocalBox\>, out int\)

Finds an axis along which the group falls into two parts with a gap between them, and leaves the group sorted
along that axis so the parts are <code>group[..cut]</code> and <code>group[cut..]</code>.

```csharp
private static bool TryCut(Span<LocalBox> group, out int cut)
```

#### Parameters

`group` [Span](https://learn.microsoft.com/dotnet/api/system.span\-1)<[LocalBox](Cube.Core.LocalBox.md)\>

The group; sorted in place along the axis it cuts on (or the last axis tried).

`cut` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The index the second part starts at.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

False if no axis has a gap of at least [Gap](Cube.Core.NodeSplitter.md#Cube_Core_NodeSplitter_Gap).

### <a id="Cube_Core_NodeSplitter_Union_System_ReadOnlySpan_Cube_Core_LocalBox__"></a> Union\(ReadOnlySpan<LocalBox\>\)

The smallest box holding every box of a non-empty group.

```csharp
private static LocalBox Union(ReadOnlySpan<LocalBox> group)
```

#### Parameters

`group` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[LocalBox](Cube.Core.LocalBox.md)\>

The boxes.

#### Returns

 [LocalBox](Cube.Core.LocalBox.md)
