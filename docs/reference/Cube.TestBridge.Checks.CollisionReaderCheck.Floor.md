# <a id="Cube_TestBridge_Checks_CollisionReaderCheck_Floor"></a> Enum CollisionReaderCheck.Floor

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

What [FindFloor](Cube.TestBridge.Checks.CollisionReaderCheck.md#Cube_TestBridge_Checks_CollisionReaderCheck_FindFloor) found.

```csharp
private enum CollisionReaderCheck.Floor
```

## Fields

`InABox = 0` 

The floor hit is inside a gathered box.



`NotInABox = 1` 

The ray hit a floor that no gathered box holds.



`NoRayHit = 2` 

The ray hit nothing.
