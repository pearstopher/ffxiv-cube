# <a id="Cube_Tests_EnemyMarkerTests"></a> Class EnemyMarkerTests

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

Checks #111: hostile battle NPCs are enemies with their own colour, the current target's label starts with
<code>[T]</code>, an enemy attacking the player's starts with <code>[!]</code>, both read <code>[T] [!] Name</code>, and the prefixed
strings are cached rather than rebuilt every frame (PROJECT.md, "Efficiency and memory": no per-frame allocation).

```csharp
public class EnemyMarkerTests
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[EnemyMarkerTests](Cube.Tests.EnemyMarkerTests.md)


## Fields

### <a id="Cube_Tests_EnemyMarkerTests_Centre"></a> Centre

```csharp
private static readonly Vector3 Centre
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Tests_EnemyMarkerTests_Wolf"></a> Wolf

```csharp
private static readonly byte[] Wolf
```

#### Field Value

 [byte](https://learn.microsoft.com/dotnet/api/system.byte)\[\]

## Methods

### <a id="Cube_Tests_EnemyMarkerTests_ChangedMarkers_RebuildTheLabel"></a> ChangedMarkers\_RebuildTheLabel\(\)

```csharp
[Fact]
public void ChangedMarkers_RebuildTheLabel()
```

### <a id="Cube_Tests_EnemyMarkerTests_EnemyColour_IsItsOwnAndDiffersFromNpc"></a> EnemyColour\_IsItsOwnAndDiffersFromNpc\(\)

```csharp
[Fact]
public void EnemyColour_IsItsOwnAndDiffersFromNpc()
```

### <a id="Cube_Tests_EnemyMarkerTests_EnemyColour_IsSavedAndRepaired"></a> EnemyColour\_IsSavedAndRepaired\(\)

```csharp
[Fact]
public void EnemyColour_IsSavedAndRepaired()
```

### <a id="Cube_Tests_EnemyMarkerTests_EnemyLabel_HasTargetFirstThenAttacking_System_Boolean_System_Boolean_System_String_"></a> EnemyLabel\_HasTargetFirstThenAttacking\(bool, bool, string\)

```csharp
[Theory]
[InlineData(new object[] { false, false, "Dire Wolf" })]
[InlineData(new object[] { true, false, "[T] Dire Wolf" })]
[InlineData(new object[] { false, true, "[!] Dire Wolf" })]
[InlineData(new object[] { true, true, "[T] [!] Dire Wolf" })]
public void EnemyLabel_HasTargetFirstThenAttacking(bool target, bool attacking, string expected)
```

#### Parameters

`target` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

`attacking` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

`expected` [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Cube_Tests_EnemyMarkerTests_EnemyOffered_IsKeptAsAnEnemyAndCountsAsAnNpc"></a> EnemyOffered\_IsKeptAsAnEnemyAndCountsAsAnNpc\(\)

```csharp
[Fact]
public void EnemyOffered_IsKeptAsAnEnemyAndCountsAsAnNpc()
```

### <a id="Cube_Tests_EnemyMarkerTests_FriendlyNpc_NeverGetsTheAttackingMarker"></a> FriendlyNpc\_NeverGetsTheAttackingMarker\(\)

```csharp
[Fact]
public void FriendlyNpc_NeverGetsTheAttackingMarker()
```

### <a id="Cube_Tests_EnemyMarkerTests_NewSet"></a> NewSet\(\)

```csharp
private static PlayerSet NewSet()
```

#### Returns

 [PlayerSet](Cube.Core.PlayerSet.md)

### <a id="Cube_Tests_EnemyMarkerTests_OnlyAHostileBattleNpc_IsAnEnemy_System_Boolean_System_Boolean_Cube_Core_PlayerRelation_"></a> OnlyAHostileBattleNpc\_IsAnEnemy\(bool, bool, PlayerRelation\)

```csharp
[Theory]
[InlineData(new object[] { true, true, PlayerRelation.Enemy })]
[InlineData(new object[] { true, false, PlayerRelation.Npc })]
[InlineData(new object[] { false, true, PlayerRelation.Npc })]
[InlineData(new object[] { false, false, PlayerRelation.Npc })]
public void OnlyAHostileBattleNpc_IsAnEnemy(bool battle, bool hostile, PlayerRelation expected)
```

#### Parameters

`battle` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

`hostile` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

`expected` [PlayerRelation](Cube.Core.PlayerRelation.md)

### <a id="Cube_Tests_EnemyMarkerTests_SteadyMarkedLabels_AllocateNothing"></a> SteadyMarkedLabels\_AllocateNothing\(\)

```csharp
[Fact]
public void SteadyMarkedLabels_AllocateNothing()
```

### <a id="Cube_Tests_EnemyMarkerTests_TargetedPlayer_GetsTheTargetMarker"></a> TargetedPlayer\_GetsTheTargetMarker\(\)

```csharp
[Fact]
public void TargetedPlayer_GetsTheTargetMarker()
```

### <a id="Cube_Tests_EnemyMarkerTests_UnchangedMarkers_ReuseTheSameStringEveryFrame"></a> UnchangedMarkers\_ReuseTheSameStringEveryFrame\(\)

```csharp
[Fact]
public void UnchangedMarkers_ReuseTheSameStringEveryFrame()
```

### <a id="Cube_Tests_EnemyMarkerTests_UnmarkedLabel_IsTheNameItselfAndBuildsNothing"></a> UnmarkedLabel\_IsTheNameItselfAndBuildsNothing\(\)

```csharp
[Fact]
public void UnmarkedLabel_IsTheNameItselfAndBuildsNothing()
```
