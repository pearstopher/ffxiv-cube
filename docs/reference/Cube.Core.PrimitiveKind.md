# <a id="Cube_Core_PrimitiveKind"></a> Enum PrimitiveKind

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

What a [DrawPrimitive](Cube.Core.DrawPrimitive.md#Cube_Core_DrawPrimitive) draws.

```csharp
public enum PrimitiveKind : byte
```

## Fields

`Line = 0` 

A line from [A](Cube.Core.DrawPrimitive.md#Cube_Core_DrawPrimitive_A) to [B](Cube.Core.DrawPrimitive.md#Cube_Core_DrawPrimitive_B): an edge of the big wireframe cube.



`Quad = 1` 

A filled quadrilateral [A](Cube.Core.DrawPrimitive.md#Cube_Core_DrawPrimitive_A)–[D](Cube.Core.DrawPrimitive.md#Cube_Core_DrawPrimitive_D): one visible face of a solid cube.



`Label = 2` 

A player's name label, anchored at [A](Cube.Core.DrawPrimitive.md#Cube_Core_DrawPrimitive_A) (left end, vertically centred; or, when
[B](Cube.Core.DrawPrimitive.md#Cube_Core_DrawPrimitive_B).X is 1 (#112), the middle of the text's bottom edge). The text is
not stored here: it is the name of the player [Tag](Cube.Core.DrawPrimitive.md#Cube_Core_DrawPrimitive_Tag) points to, so the draw list stays
a plain struct array and names are never copied (PROJECT.md, "Labels"; "Efficiency and memory").



`Polygon = 3` 

A filled convex polygon of 3 to [MaxCorners](Cube.Core.ClippedBox.md#Cube_Core_ClippedBox_MaxCorners) corners, stored in the draw list's point
buffer ([Points](Cube.Core.DrawList.md#Cube_Core_DrawList_Points)): the top or bottom of a collision box clipped to a turned cube (#93),
whose footprint isn't a quad. One ImGui call, so a see-through face shows no seams where quads would meet.
