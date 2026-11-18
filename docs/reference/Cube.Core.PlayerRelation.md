# <a id="Cube_Core_PlayerRelation"></a> Enum PlayerRelation

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

How someone in the cube relates to the local player, which picks their colour (PROJECT.md, "Colours": self,
party/alliance members, everyone else, NPCs; one colour each).

```csharp
public enum PlayerRelation : byte
```

## Fields

`Other = 0` 

Every other player.



`Party = 1` 

A member of the local player's party or alliance.



`Self = 2` 

The local player, at the centre of the cube.



`Npc = 3` 

A named NPC (PROJECT.md, "NPCs"): drawn like a player, in its own colour. Never self or party, even when the NPC
fights alongside the player.
