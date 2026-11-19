# <a id="Cube_Game_CollisionReader_RefineState"></a> Class CollisionReader.RefineState

Namespace: [Cube.Game](Cube.Game.md)  
Assembly: Cube.dll  

Everything the node splitting keeps between frames (#127): the cache of pieces, the scratch space it works in
(allocated once, so splitting allocates nothing per frame), and this frame's triangle budget.

```csharp
internal sealed class CollisionReader.RefineState
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[CollisionReader.RefineState](Cube.Game.CollisionReader.RefineState.md)


## Remarks

<b>Lazy refining within the frame budget.</b> Splitting reads every triangle of a node, so only
[TrianglesPerFrame](Cube.Game.CollisionReader.RefineState.md#Cube_Game_CollisionReader_RefineState_TrianglesPerFrame) triangles are read per frame (about 0.1 ms). A node not yet split is drawn as its
coarse box, so the cube never has holes; [EndGather](Cube.Game.CollisionReader.RefineState.md#Cube_Game_CollisionReader_RefineState_EndGather) asks for another gather while any are left.

## Fields

### <a id="Cube_Game_CollisionReader_RefineState_MaxTriangles"></a> MaxTriangles

Nodes with more triangles than this aren't split and stay their coarse box (the #125 leaves hold 9 to 16; a
much bigger node is a different kind of thing and would eat the frame budget).

```csharp
public const int MaxTriangles = 64
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Game_CollisionReader_RefineState_MinSplitSide"></a> MinSplitSide

Only nodes with a side at least this long (world yalms) are split: the #125 measurements found nearly all of
the wasted volume in them, and a smaller box can't hide much.

```csharp
public const float MinSplitSide = 3
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Game_CollisionReader_RefineState_TrianglesPerFrame"></a> TrianglesPerFrame

Triangles read per frame for splitting. A node that starts under the budget is finished even if it ends over it.

```csharp
public const int TrianglesPerFrame = 300
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Properties

### <a id="Cube_Game_CollisionReader_RefineState_NodePieces"></a> NodePieces

Scratch: the current node's pieces.

```csharp
public LocalBox[] NodePieces { get; }
```

#### Property Value

 [LocalBox](Cube.Core.LocalBox.md)\[\]

### <a id="Cube_Game_CollisionReader_RefineState_Pieces"></a> Pieces

The split pieces, keyed by node.

```csharp
public NodePieceCache Pieces { get; }
```

#### Property Value

 [NodePieceCache](Cube.Core.NodePieceCache.md)

### <a id="Cube_Game_CollisionReader_RefineState_Skipped"></a> Skipped

Whether a node was left coarse in this gather for want of budget or room.

```csharp
public bool Skipped { get; set; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Game_CollisionReader_RefineState_Triangles"></a> Triangles

Scratch: the current node's triangle bounds.

```csharp
public LocalBox[] Triangles { get; }
```

#### Property Value

 [LocalBox](Cube.Core.LocalBox.md)\[\]

### <a id="Cube_Game_CollisionReader_RefineState_TrianglesLeft"></a> TrianglesLeft

Triangles that may still be read this frame.

```csharp
public int TrianglesLeft { get; set; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Methods

### <a id="Cube_Game_CollisionReader_RefineState_BeginFrame"></a> BeginFrame\(\)

Resets the per-frame triangle budget.

```csharp
public void BeginFrame()
```

### <a id="Cube_Game_CollisionReader_RefineState_EndGather"></a> EndGather\(\)

Ends a completed gather.

```csharp
public bool EndGather()
```

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

True if some nodes were left coarse and another gather should refine them.
