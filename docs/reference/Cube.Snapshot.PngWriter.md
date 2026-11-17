# <a id="Cube_Snapshot_PngWriter"></a> Class PngWriter

Namespace: [Cube.Snapshot](Cube.Snapshot.md)  
Assembly: Cube.Snapshot.dll  

Writes a [Canvas](Cube.Snapshot.Canvas.md#Cube_Snapshot_Canvas) as a PNG file (8-bit RGBA, no filtering), using only the .NET base library so
the tool needs no image package.

```csharp
public static class PngWriter
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[PngWriter](Cube.Snapshot.PngWriter.md)


## Fields

### <a id="Cube_Snapshot_PngWriter_CrcTable"></a> CrcTable

CRC-32 lookup table (polynomial 0xEDB88320), as PNG chunks require.

```csharp
private static readonly uint[] CrcTable
```

#### Field Value

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)\[\]

### <a id="Cube_Snapshot_PngWriter_Signature"></a> Signature

The 8-byte PNG file signature.

```csharp
private static readonly byte[] Signature
```

#### Field Value

 [byte](https://learn.microsoft.com/dotnet/api/system.byte)\[\]

## Methods

### <a id="Cube_Snapshot_PngWriter_BuildCrcTable"></a> BuildCrcTable\(\)

Builds the CRC-32 table.

```csharp
private static uint[] BuildCrcTable()
```

#### Returns

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)\[\]

### <a id="Cube_Snapshot_PngWriter_Crc_System_UInt32_System_ReadOnlySpan_System_Byte__"></a> Crc\(uint, ReadOnlySpan<byte\>\)

Continues a CRC-32 over more bytes.

```csharp
private static uint Crc(uint crc, ReadOnlySpan<byte> bytes)
```

#### Parameters

`crc` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

The running value.

`bytes` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

The bytes to add.

#### Returns

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

### <a id="Cube_Snapshot_PngWriter_Encode_Cube_Snapshot_Canvas_"></a> Encode\(Canvas\)

Encodes a canvas as PNG bytes.

```csharp
public static byte[] Encode(Canvas canvas)
```

#### Parameters

`canvas` [Canvas](Cube.Snapshot.Canvas.md)

The image.

#### Returns

 [byte](https://learn.microsoft.com/dotnet/api/system.byte)\[\]

### <a id="Cube_Snapshot_PngWriter_WriteChunk_System_IO_Stream_System_String_System_ReadOnlySpan_System_Byte__"></a> WriteChunk\(Stream, string, ReadOnlySpan<byte\>\)

Writes one chunk: length, type, data, CRC of type and data.

```csharp
private static void WriteChunk(Stream output, string type, ReadOnlySpan<byte> data)
```

#### Parameters

`output` [Stream](https://learn.microsoft.com/dotnet/api/system.io.stream)

Where to write.

`type` [string](https://learn.microsoft.com/dotnet/api/system.string)

Four-letter chunk type.

`data` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

Chunk contents.
