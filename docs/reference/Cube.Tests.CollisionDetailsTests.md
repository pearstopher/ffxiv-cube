# <a id="Cube_Tests_CollisionDetailsTests"></a> Class CollisionDetailsTests

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

Checks the collision detail levels (#32, collision detail setting; PROJECT.md, "Collision (phase 2)": simplified
boxes): Full walks the whole node tree, Medium stops a few levels down, Coarse keeps one box per collider, and every
level is its own stamp so a change gathers again at once.

```csharp
public class CollisionDetailsTests
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[CollisionDetailsTests](Cube.Tests.CollisionDetailsTests.md)


## Methods

### <a id="Cube_Tests_CollisionDetailsTests_Coarse_detail_keeps_only_the_root_box_of_each_mesh"></a> Coarse\_detail\_keeps\_only\_the\_root\_box\_of\_each\_mesh\(\)

```csharp
[Fact]
public void Coarse_detail_keeps_only_the_root_box_of_each_mesh()
```

### <a id="Cube_Tests_CollisionDetailsTests_Different_scenes_have_different_stamps_at_the_same_detail"></a> Different\_scenes\_have\_different\_stamps\_at\_the\_same\_detail\(\)

```csharp
[Fact]
public void Different_scenes_have_different_stamps_at_the_same_detail()
```

### <a id="Cube_Tests_CollisionDetailsTests_Each_detail_level_has_its_own_stamp_for_the_same_scene"></a> Each\_detail\_level\_has\_its\_own\_stamp\_for\_the\_same\_scene\(\)

```csharp
[Fact]
public void Each_detail_level_has_its_own_stamp_for_the_same_scene()
```

### <a id="Cube_Tests_CollisionDetailsTests_Full_detail_walks_the_whole_tree"></a> Full\_detail\_walks\_the\_whole\_tree\(\)

```csharp
[Fact]
public void Full_detail_walks_the_whole_tree()
```

### <a id="Cube_Tests_CollisionDetailsTests_Medium_detail_is_between_coarse_and_full"></a> Medium\_detail\_is\_between\_coarse\_and\_full\(\)

```csharp
[Fact]
public void Medium_detail_is_between_coarse_and_full()
```
