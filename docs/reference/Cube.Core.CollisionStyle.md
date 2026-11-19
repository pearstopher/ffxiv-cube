# <a id="Cube_Core_CollisionStyle"></a> Enum CollisionStyle

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

How collision is drawn (#100; PROJECT.md, "Collision style"): as simplified boxes, or as the game's own triangles.

```csharp
public enum CollisionStyle
```

## Fields

`Boxes = 0` 

Simplified boxes, one per collider or mesh node piece ([CollisionCache](Cube.Core.CollisionCache.md#Cube_Core_CollisionCache)). The default.



`Mesh = 1` 

The real collision triangles as a wireframe ([TriangleCache](Cube.Core.TriangleCache.md#Cube_Core_TriangleCache), [ClippedMesh](Cube.Core.ClippedMesh.md#Cube_Core_ClippedMesh)). Slower:
nothing is simplified, so busy places cost more than the boxes' budget.



## Remarks

Boxes stays the default and the only style that keeps PROJECT.md's 0.5 ms budget. Mesh is an opt-in exception to
it: the scene's real triangles are drawn as they are, so its cost depends on the scene and is reported, not limited.
Old settings files have no style and load as [Boxes](Cube.Core.CollisionStyle.md#Cube_Core_CollisionStyle_Boxes).
