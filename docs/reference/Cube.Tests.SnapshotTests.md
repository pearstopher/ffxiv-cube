# <a id="Cube_Tests_SnapshotTests"></a> Class SnapshotTests

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

Checks the dev-only snapshot renderer (PROJECT.md, "Automated testing": a dev tool turns a dumped draw list
into a PNG) paints what the dump says, in order, and writes a valid PNG.

```csharp
public class SnapshotTests
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[SnapshotTests](Cube.Tests.SnapshotTests.md)


## Fields

### <a id="Cube_Tests_SnapshotTests_Green"></a> Green

```csharp
private const uint Green = 4278255360
```

#### Field Value

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

### <a id="Cube_Tests_SnapshotTests_Red"></a> Red

```csharp
private const uint Red = 4278190335
```

#### Field Value

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

## Methods

### <a id="Cube_Tests_SnapshotTests_Core_frame_paints_the_wireframe_and_a_cube_at_the_centre"></a> Core\_frame\_paints\_the\_wireframe\_and\_a\_cube\_at\_the\_centre\(\)

```csharp
[Fact]
public void Core_frame_paints_the_wireframe_and_a_cube_at_the_centre()
```

### <a id="Cube_Tests_SnapshotTests_Dump_is_painted_in_order_so_later_primitives_cover_earlier_ones"></a> Dump\_is\_painted\_in\_order\_so\_later\_primitives\_cover\_earlier\_ones\(\)

```csharp
[Fact]
public void Dump_is_painted_in_order_so_later_primitives_cover_earlier_ones()
```

### <a id="Cube_Tests_SnapshotTests_Half_transparent_colour_blends_with_the_background"></a> Half\_transparent\_colour\_blends\_with\_the\_background\(\)

```csharp
[Fact]
public void Half_transparent_colour_blends_with_the_background()
```

### <a id="Cube_Tests_SnapshotTests_Inflate_System_Byte___"></a> Inflate\(byte\[\]\)

```csharp
private static byte[] Inflate(byte[] png)
```

#### Parameters

`png` [byte](https://learn.microsoft.com/dotnet/api/system.byte)\[\]

#### Returns

 [byte](https://learn.microsoft.com/dotnet/api/system.byte)\[\]

### <a id="Cube_Tests_SnapshotTests_Line_is_drawn_between_its_ends"></a> Line\_is\_drawn\_between\_its\_ends\(\)

```csharp
[Fact]
public void Line_is_drawn_between_its_ends()
```

### <a id="Cube_Tests_SnapshotTests_Png_chunk_checksums_are_valid"></a> Png\_chunk\_checksums\_are\_valid\(\)

```csharp
[Fact]
public void Png_chunk_checksums_are_valid()
```

### <a id="Cube_Tests_SnapshotTests_Png_has_the_signature_size_and_pixels"></a> Png\_has\_the\_signature\_size\_and\_pixels\(\)

```csharp
[Fact]
public void Png_has_the_signature_size_and_pixels()
```

### <a id="Cube_Tests_SnapshotTests_Quad_fills_its_inside_only"></a> Quad\_fills\_its\_inside\_only\(\)

```csharp
[Fact]
public void Quad_fills_its_inside_only()
```

### <a id="Cube_Tests_SnapshotTests_Winding_order_does_not_matter"></a> Winding\_order\_does\_not\_matter\(\)

```csharp
[Fact]
public void Winding_order_does_not_matter()
```
