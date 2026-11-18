# <a id="Cube_Core_CollisionDetail"></a> Enum CollisionDetail

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

How finely collision is drawn (#32, the collision detail setting; PROJECT.md, "Collision (phase 2)": colliders are
shown as simplified boxes). Each level is a depth limit on the walk down a mesh collider's node tree; primitive
colliders (boxes, cylinders, spheres, planes) are one box at every level.

```csharp
public enum CollisionDetail
```

## Fields

`Full = 0` 

The default and #28's recommendation: one box per tree node holding triangles, which follows walls and floors
closely.



`Medium = 1` 

The tree is cut off [MediumDepth](Cube.Core.CollisionDetails.md#Cube_Core_CollisionDetails_MediumDepth) levels down: at most 8 boxes per mesh.



`Coarse = 2` 

One box per collider: the mesh's root node box. Cheapest, but a building or terrain tile is one block.



## Remarks

A mesh's node tree is a bounding tree: each node's box holds its children's (#28's sample: every raycast hit was
inside a node box). Stopping at a depth and keeping that node's box therefore still covers everything below it,
just more loosely, and the bridge's <code>collision-settings</code> check confirms it in game. Fewer, bigger boxes cost
less to walk, clip and draw.
