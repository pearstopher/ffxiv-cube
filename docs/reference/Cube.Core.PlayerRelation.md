# <a id="Cube_Core_PlayerRelation"></a> Enum PlayerRelation

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

How a player relates to the local player, which picks their colour (PROJECT.md, "Colours": self,
party/alliance members, everyone else; one colour each).

```csharp
public enum PlayerRelation : byte
```

## Fields

`Other = 0` 

Everyone else.



`Party = 1` 

A member of the local player's party or alliance.



`Self = 2` 

The local player, at the centre of the cube.
