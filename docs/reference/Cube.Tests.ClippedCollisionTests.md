# <a id="Cube_Tests_ClippedCollisionTests"></a> Class ClippedCollisionTests

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

Checks the gathered collision is clipped to the cube for drawing, and only when something changed (PROJECT.md,
"Collision (phase 2)": boxes clipped to the cube; "Efficiency and memory": recompute only when the inputs change,
no per-frame allocations, collision data dropped on a zone change).

```csharp
public class ClippedCollisionTests
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ClippedCollisionTests](Cube.Tests.ClippedCollisionTests.md)


## Fields

### <a id="Cube_Tests_ClippedCollisionTests_Crossing"></a> Crossing

```csharp
private static readonly CollisionBox Crossing
```

#### Field Value

 [CollisionBox](Cube.Core.CollisionBox.md)

### <a id="Cube_Tests_ClippedCollisionTests_Edge"></a> Edge

```csharp
private const float Edge = 40
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_ClippedCollisionTests_Inside"></a> Inside

```csharp
private static readonly CollisionBox Inside
```

#### Field Value

 [CollisionBox](Cube.Core.CollisionBox.md)

### <a id="Cube_Tests_ClippedCollisionTests_Outside"></a> Outside

```csharp
private static readonly CollisionBox Outside
```

#### Field Value

 [CollisionBox](Cube.Core.CollisionBox.md)

### <a id="Cube_Tests_ClippedCollisionTests_Zone"></a> Zone

```csharp
private const uint Zone = 132
```

#### Field Value

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

## Methods

### <a id="Cube_Tests_ClippedCollisionTests_A_new_gather_is_clipped_again"></a> A\_new\_gather\_is\_clipped\_again\(\)

```csharp
[Fact]
public void A_new_gather_is_clipped_again()
```

### <a id="Cube_Tests_ClippedCollisionTests_Boxes_are_clipped_to_the_cube_and_those_outside_are_left_out"></a> Boxes\_are\_clipped\_to\_the\_cube\_and\_those\_outside\_are\_left\_out\(\)

```csharp
[Fact]
public void Boxes_are_clipped_to_the_cube_and_those_outside_are_left_out()
```

### <a id="Cube_Tests_ClippedCollisionTests_Clipping_every_frame_does_not_allocate"></a> Clipping\_every\_frame\_does\_not\_allocate\(\)

```csharp
[Fact]
public void Clipping_every_frame_does_not_allocate()
```

### <a id="Cube_Tests_ClippedCollisionTests_Collision_dropped_on_a_zone_change_is_no_longer_drawn"></a> Collision\_dropped\_on\_a\_zone\_change\_is\_no\_longer\_drawn\(\)

```csharp
[Fact]
public void Collision_dropped_on_a_zone_change_is_no_longer_drawn()
```

### <a id="Cube_Tests_ClippedCollisionTests_Gathered_System_Numerics_Vector3_Cube_Core_CollisionBox___"></a> Gathered\(Vector3, params CollisionBox\[\]\)

```csharp
private static CollisionCache Gathered(Vector3 centre, params CollisionBox[] boxes)
```

#### Parameters

`centre` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

`boxes` [CollisionBox](Cube.Core.CollisionBox.md)\[\]

#### Returns

 [CollisionCache](Cube.Core.CollisionCache.md)

### <a id="Cube_Tests_ClippedCollisionTests_Moving_or_turning_the_cube_clips_again"></a> Moving\_or\_turning\_the\_cube\_clips\_again\(\)

```csharp
[Fact]
public void Moving_or_turning_the_cube_clips_again()
```

### <a id="Cube_Tests_ClippedCollisionTests_Same_cube_and_same_gather_is_not_clipped_again"></a> Same\_cube\_and\_same\_gather\_is\_not\_clipped\_again\(\)

```csharp
[Fact]
public void Same_cube_and_same_gather_is_not_clipped_again()
```
