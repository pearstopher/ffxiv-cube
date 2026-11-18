# <a id="Cube_Tests_PlayerSetTests"></a> Class PlayerSetTests

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

Checks which players and NPCs are kept and how they're classified and named (PROJECT.md, "Players": only players
inside the cube are shown; "NPCs": named, unowned NPCs inside the cube are shown, with an on/off setting; "Colours":
self, party/alliance, everyone else, NPCs; "Efficiency and memory": name labels cached
and only rebuilt when a name changes, no per-frame allocations, bounded memory; "Privacy": names stay in memory).

```csharp
public class PlayerSetTests
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[PlayerSetTests](Cube.Tests.PlayerSetTests.md)


## Fields

### <a id="Cube_Tests_PlayerSetTests_Alice"></a> Alice

```csharp
private static readonly byte[] Alice
```

#### Field Value

 [byte](https://learn.microsoft.com/dotnet/api/system.byte)\[\]

### <a id="Cube_Tests_PlayerSetTests_Bob"></a> Bob

```csharp
private static readonly byte[] Bob
```

#### Field Value

 [byte](https://learn.microsoft.com/dotnet/api/system.byte)\[\]

### <a id="Cube_Tests_PlayerSetTests_Centre"></a> Centre

```csharp
private static readonly Vector3 Centre
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

## Methods

### <a id="Cube_Tests_PlayerSetTests_A_full_set_drops_extra_players_instead_of_growing"></a> A\_full\_set\_drops\_extra\_players\_instead\_of\_growing\(\)

```csharp
[Fact]
public void A_full_set_drops_extra_players_instead_of_growing()
```

### <a id="Cube_Tests_PlayerSetTests_A_new_frame_starts_empty"></a> A\_new\_frame\_starts\_empty\(\)

```csharp
[Fact]
public void A_new_frame_starts_empty()
```

### <a id="Cube_Tests_PlayerSetTests_Begin_Cube_Core_PlayerSet_"></a> Begin\(PlayerSet\)

```csharp
private static PlayerSet Begin(PlayerSet set)
```

#### Parameters

`set` [PlayerSet](Cube.Core.PlayerSet.md)

#### Returns

 [PlayerSet](Cube.Core.PlayerSet.md)

### <a id="Cube_Tests_PlayerSetTests_Name_stops_at_the_first_zero_byte"></a> Name\_stops\_at\_the\_first\_zero\_byte\(\)

```csharp
[Fact]
public void Name_stops_at_the_first_zero_byte()
```

### <a id="Cube_Tests_PlayerSetTests_Nameless_npc_is_not_a_named_npc"></a> Nameless\_npc\_is\_not\_a\_named\_npc\(\)

```csharp
[Fact]
public void Nameless_npc_is_not_a_named_npc()
```

### <a id="Cube_Tests_PlayerSetTests_NewSet_System_Int32_System_Int32_"></a> NewSet\(int, int\)

```csharp
private static PlayerSet NewSet(int capacity = 16, int npcCapacity = 0)
```

#### Parameters

`capacity` [int](https://learn.microsoft.com/dotnet/api/system.int32)

`npcCapacity` [int](https://learn.microsoft.com/dotnet/api/system.int32)

#### Returns

 [PlayerSet](Cube.Core.PlayerSet.md)

### <a id="Cube_Tests_PlayerSetTests_Npc_cap_drops_extra_npcs_without_pushing_out_players"></a> Npc\_cap\_drops\_extra\_npcs\_without\_pushing\_out\_players\(\)

```csharp
[Fact]
public void Npc_cap_drops_extra_npcs_without_pushing_out_players()
```

### <a id="Cube_Tests_PlayerSetTests_Npc_inside_the_cube_is_kept_as_an_npc_with_its_name"></a> Npc\_inside\_the\_cube\_is\_kept\_as\_an\_npc\_with\_its\_name\(\)

```csharp
[Fact]
public void Npc_inside_the_cube_is_kept_as_an_npc_with_its_name()
```

### <a id="Cube_Tests_PlayerSetTests_Npc_outside_the_cube_is_not_kept"></a> Npc\_outside\_the\_cube\_is\_not\_kept\(\)

```csharp
[Fact]
public void Npc_outside_the_cube_is_not_kept()
```

### <a id="Cube_Tests_PlayerSetTests_Player_inside_the_cube_is_kept_with_its_position_and_name"></a> Player\_inside\_the\_cube\_is\_kept\_with\_its\_position\_and\_name\(\)

```csharp
[Fact]
public void Player_inside_the_cube_is_kept_with_its_position_and_name()
```

### <a id="Cube_Tests_PlayerSetTests_Player_outside_the_cube_is_not_kept"></a> Player\_outside\_the\_cube\_is\_not\_kept\(\)

```csharp
[Fact]
public void Player_outside_the_cube_is_not_kept()
```

### <a id="Cube_Tests_PlayerSetTests_Player_outside_the_cube_is_rejected_before_its_name_is_read"></a> Player\_outside\_the\_cube\_is\_rejected\_before\_its\_name\_is\_read\(\)

```csharp
[Fact]
public void Player_outside_the_cube_is_rejected_before_its_name_is_read()
```

### <a id="Cube_Tests_PlayerSetTests_Player_owned_pet_is_not_treated_as_an_npc"></a> Player\_owned\_pet\_is\_not\_treated\_as\_an\_npc\(\)

```csharp
[Fact]
public void Player_owned_pet_is_not_treated_as_an_npc()
```

### <a id="Cube_Tests_PlayerSetTests_Reading_the_same_npcs_every_frame_does_not_allocate"></a> Reading\_the\_same\_npcs\_every\_frame\_does\_not\_allocate\(\)

```csharp
[Fact]
public void Reading_the_same_npcs_every_frame_does_not_allocate()
```

### <a id="Cube_Tests_PlayerSetTests_Reading_the_same_players_every_frame_does_not_allocate"></a> Reading\_the\_same\_players\_every\_frame\_does\_not\_allocate\(\)

```csharp
[Fact]
public void Reading_the_same_players_every_frame_does_not_allocate()
```

### <a id="Cube_Tests_PlayerSetTests_Relation_is_self_then_party_or_alliance_then_other_System_Boolean_System_Boolean_System_Boolean_Cube_Core_PlayerRelation_"></a> Relation\_is\_self\_then\_party\_or\_alliance\_then\_other\(bool, bool, bool, PlayerRelation\)

```csharp
[Theory]
[InlineData(new object[] { true, false, false, PlayerRelation.Self })]
[InlineData(new object[] { true, true, true, PlayerRelation.Self })]
[InlineData(new object[] { false, true, false, PlayerRelation.Party })]
[InlineData(new object[] { false, false, true, PlayerRelation.Party })]
[InlineData(new object[] { false, false, false, PlayerRelation.Other })]
public void Relation_is_self_then_party_or_alliance_then_other(bool isLocal, bool isParty, bool isAlliance, PlayerRelation expected)
```

#### Parameters

`isLocal` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

`isParty` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

`isAlliance` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

`expected` [PlayerRelation](Cube.Core.PlayerRelation.md)

### <a id="Cube_Tests_PlayerSetTests_Same_name_every_frame_is_built_once_and_reused"></a> Same\_name\_every\_frame\_is\_built\_once\_and\_reused\(\)

```csharp
[Fact]
public void Same_name_every_frame_is_built_once_and_reused()
```

### <a id="Cube_Tests_PlayerSetTests_Slot_beyond_the_name_cache_is_ignored"></a> Slot\_beyond\_the\_name\_cache\_is\_ignored\(\)

```csharp
[Fact]
public void Slot_beyond_the_name_cache_is_ignored()
```

### <a id="Cube_Tests_PlayerSetTests_Slot_reused_by_another_player_gets_the_new_name"></a> Slot\_reused\_by\_another\_player\_gets\_the\_new\_name\(\)

```csharp
[Fact]
public void Slot_reused_by_another_player_gets_the_new_name()
```

### <a id="Cube_Tests_PlayerSetTests_Unowned_npc_with_a_name_is_a_named_npc"></a> Unowned\_npc\_with\_a\_name\_is\_a\_named\_npc\(\)

```csharp
[Fact]
public void Unowned_npc_with_a_name_is_a_named_npc()
```

### <a id="Cube_Tests_PlayerSetTests_With_npcs_off_no_npc_is_kept_but_players_still_are"></a> With\_npcs\_off\_no\_npc\_is\_kept\_but\_players\_still\_are\(\)

```csharp
[Fact]
public void With_npcs_off_no_npc_is_kept_but_players_still_are()
```
