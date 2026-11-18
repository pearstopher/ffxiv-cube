# <a id="Cube_Core_FootprintCorners"></a> Struct FootprintCorners

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

Inline storage for a [ClippedBox](Cube.Core.ClippedBox.md#Cube_Core_ClippedBox)'s footprint corners, so the struct holds no array.

```csharp
internal struct FootprintCorners
```


## Fields

### <a id="Cube_Core_FootprintCorners_first"></a> first

The first corner; the runtime lays out the rest after it.

```csharp
private Vector2 first
```

#### Field Value

 [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)
