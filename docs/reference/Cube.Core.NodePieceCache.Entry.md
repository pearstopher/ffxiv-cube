# <a id="Cube_Core_NodePieceCache_Entry"></a> Struct NodePieceCache.Entry

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

Where one node's pieces are, and which node they were made for.

```csharp
private readonly record struct NodePieceCache.Entry : IEquatable<NodePieceCache.Entry>
```

#### Implements

[IEquatable<NodePieceCache.Entry\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)


## Constructors

### <a id="Cube_Core_NodePieceCache_Entry__ctor_System_Int32_System_Int32_System_UInt32_"></a> Entry\(int, int, uint\)

Where one node's pieces are, and which node they were made for.

```csharp
public Entry(int Start, int Count, uint Check)
```

#### Parameters

`Start` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Index of the first piece.

`Count` [int](https://learn.microsoft.com/dotnet/api/system.int32)

How many pieces.

`Check` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

The node's check number.

## Properties

### <a id="Cube_Core_NodePieceCache_Entry_Check"></a> Check

The node's check number.

```csharp
public uint Check { get; init; }
```

#### Property Value

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

### <a id="Cube_Core_NodePieceCache_Entry_Count"></a> Count

How many pieces.

```csharp
public int Count { get; init; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_NodePieceCache_Entry_Start"></a> Start

Index of the first piece.

```csharp
public int Start { get; init; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)
