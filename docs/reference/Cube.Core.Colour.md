# <a id="Cube_Core_Colour"></a> Class Colour

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

Packs and shades colours in the 32-bit layout ImGui uses (<code>0xAABBGGRR</code>), without Core depending on ImGui.

```csharp
public static class Colour
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[Colour](Cube.Core.Colour.md)


## Methods

### <a id="Cube_Core_Colour_Pack_System_Single_System_Single_System_Single_System_Single_"></a> Pack\(float, float, float, float\)

Packs red, green, blue and alpha (each 0–1, clamped) as <code>0xAABBGGRR</code>.

```csharp
public static uint Pack(float r, float g, float b, float a)
```

#### Parameters

`r` [float](https://learn.microsoft.com/dotnet/api/system.single)

Red.

`g` [float](https://learn.microsoft.com/dotnet/api/system.single)

Green.

`b` [float](https://learn.microsoft.com/dotnet/api/system.single)

Blue.

`a` [float](https://learn.microsoft.com/dotnet/api/system.single)

Alpha (opacity).

#### Returns

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

### <a id="Cube_Core_Colour_Shade_System_UInt32_System_Single_"></a> Shade\(uint, float\)

Darkens a colour's red, green and blue by a factor, keeping alpha. Side faces of a cube are drawn darker
than the top so the cubes read as solid 3D shapes.

```csharp
public static uint Shade(uint colour, float factor)
```

#### Parameters

`colour` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

A packed colour.

`factor` [float](https://learn.microsoft.com/dotnet/api/system.single)

0 (black) to 1 (unchanged).

#### Returns

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

### <a id="Cube_Core_Colour_ToByte_System_Single_"></a> ToByte\(float\)

Converts 0–1 to a byte, clamped and rounded.

```csharp
private static uint ToByte(float value)
```

#### Parameters

`value` [float](https://learn.microsoft.com/dotnet/api/system.single)

A channel value.

#### Returns

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)
