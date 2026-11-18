# <a id="Cube_Tests_CollisionCacheTests"></a> Class CollisionCacheTests

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

Checks the collision reader's gathering rules (PROJECT.md, "Collision (phase 2)": expensive gathering is spread
over frames with a time budget and cached, areas not yet processed are simply empty, stale data is refreshed when
the player moves; "Efficiency and memory": bounded memory, collision data dropped on a zone change, no per-frame
allocations). The game side is replaced by a fake scene whose colliders each cost a set number of clock ticks.

```csharp
public class CollisionCacheTests
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[CollisionCacheTests](Cube.Tests.CollisionCacheTests.md)


## Fields

### <a id="Cube_Tests_CollisionCacheTests_Edge"></a> Edge

```csharp
private const float Edge = 40
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_CollisionCacheTests_Stamp"></a> Stamp

```csharp
private const ulong Stamp = 7
```

#### Field Value

 [ulong](https://learn.microsoft.com/dotnet/api/system.uint64)

### <a id="Cube_Tests_CollisionCacheTests_Zone"></a> Zone

```csharp
private const uint Zone = 132
```

#### Field Value

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

## Methods

### <a id="Cube_Tests_CollisionCacheTests_Area_not_yet_gathered_is_empty"></a> Area\_not\_yet\_gathered\_is\_empty\(\)

```csharp
[Fact]
public void Area_not_yet_gathered_is_empty()
```

### <a id="Cube_Tests_CollisionCacheTests_Cache_System_Int64_"></a> Cache\(long\)

```csharp
private static CollisionCache Cache(long budget = 1000)
```

#### Parameters

`budget` [long](https://learn.microsoft.com/dotnet/api/system.int64)

#### Returns

 [CollisionCache](Cube.Core.CollisionCache.md)

### <a id="Cube_Tests_CollisionCacheTests_Detail_change_during_a_gather_starts_it_again"></a> Detail\_change\_during\_a\_gather\_starts\_it\_again\(\)

```csharp
[Fact]
public void Detail_change_during_a_gather_starts_it_again()
```

### <a id="Cube_Tests_CollisionCacheTests_Each_frame_resumes_at_the_next_collider_by_index"></a> Each\_frame\_resumes\_at\_the\_next\_collider\_by\_index\(\)

```csharp
[Fact]
public void Each_frame_resumes_at_the_next_collider_by_index()
```

### <a id="Cube_Tests_CollisionCacheTests_First_zone_is_not_counted_as_a_change"></a> First\_zone\_is\_not\_counted\_as\_a\_change\(\)

```csharp
[Fact]
public void First_zone_is_not_counted_as_a_change()
```

### <a id="Cube_Tests_CollisionCacheTests_Gather_is_spread_over_frames_when_the_budget_runs_out"></a> Gather\_is\_spread\_over\_frames\_when\_the\_budget\_runs\_out\(\)

```csharp
[Fact]
public void Gather_is_spread_over_frames_when_the_budget_runs_out()
```

### <a id="Cube_Tests_CollisionCacheTests_Gathering_and_idle_frames_allocate_nothing"></a> Gathering\_and\_idle\_frames\_allocate\_nothing\(\)

```csharp
[Fact]
public void Gathering_and_idle_frames_allocate_nothing()
```

### <a id="Cube_Tests_CollisionCacheTests_Invalidate_keeps_the_boxes_and_gathers_again"></a> Invalidate\_keeps\_the\_boxes\_and\_gathers\_again\(\)

```csharp
[Fact]
public void Invalidate_keeps_the_boxes_and_gathers_again()
```

### <a id="Cube_Tests_CollisionCacheTests_Many_zone_changes_reuse_the_same_two_buffers"></a> Many\_zone\_changes\_reuse\_the\_same\_two\_buffers\(\)

```csharp
[Fact]
public void Many_zone_changes_reuse_the_same_two_buffers()
```

### <a id="Cube_Tests_CollisionCacheTests_Memory_is_bounded_by_the_cap"></a> Memory\_is\_bounded\_by\_the\_cap\(\)

```csharp
[Fact]
public void Memory_is_bounded_by_the_cap()
```

### <a id="Cube_Tests_CollisionCacheTests_Moving_past_the_threshold_refreshes_around_the_new_position"></a> Moving\_past\_the\_threshold\_refreshes\_around\_the\_new\_position\(\)

```csharp
[Fact]
public void Moving_past_the_threshold_refreshes_around_the_new_position()
```

### <a id="Cube_Tests_CollisionCacheTests_New_detail_level_gathers_again_while_the_old_boxes_stay_drawn"></a> New\_detail\_level\_gathers\_again\_while\_the\_old\_boxes\_stay\_drawn\(\)

```csharp
[Fact]
public void New_detail_level_gathers_again_while_the_old_boxes_stay_drawn()
```

### <a id="Cube_Tests_CollisionCacheTests_Nothing_is_read_while_the_cached_boxes_are_current"></a> Nothing\_is\_read\_while\_the\_cached\_boxes\_are\_current\(\)

```csharp
[Fact]
public void Nothing_is_read_while_the_cached_boxes_are_current()
```

### <a id="Cube_Tests_CollisionCacheTests_Old_boxes_stay_drawn_while_a_refresh_is_gathered"></a> Old\_boxes\_stay\_drawn\_while\_a\_refresh\_is\_gathered\(\)

```csharp
[Fact]
public void Old_boxes_stay_drawn_while_a_refresh_is_gathered()
```

### <a id="Cube_Tests_CollisionCacheTests_Scene_System_Int32_"></a> Scene\(int\)

```csharp
private static CollisionBox[] Scene(int count)
```

#### Parameters

`count` [int](https://learn.microsoft.com/dotnet/api/system.int32)

#### Returns

 [CollisionBox](Cube.Core.CollisionBox.md)\[\]

### <a id="Cube_Tests_CollisionCacheTests_Scene_change_during_a_gather_starts_it_again"></a> Scene\_change\_during\_a\_gather\_starts\_it\_again\(\)

```csharp
[Fact]
public void Scene_change_during_a_gather_starts_it_again()
```

### <a id="Cube_Tests_CollisionCacheTests_Scene_shorter_than_the_resume_point_finishes_the_gather"></a> Scene\_shorter\_than\_the\_resume\_point\_finishes\_the\_gather\(\)

```csharp
[Fact]
public void Scene_shorter_than_the_resume_point_finishes_the_gather()
```

### <a id="Cube_Tests_CollisionCacheTests_Slow_collider_still_makes_progress_one_per_frame"></a> Slow\_collider\_still\_makes\_progress\_one\_per\_frame\(\)

```csharp
[Fact]
public void Slow_collider_still_makes_progress_one_per_frame()
```

### <a id="Cube_Tests_CollisionCacheTests_Small_scene_is_gathered_in_one_frame_when_within_budget"></a> Small\_scene\_is\_gathered\_in\_one\_frame\_when\_within\_budget\(\)

```csharp
[Fact]
public void Small_scene_is_gathered_in_one_frame_when_within_budget()
```

### <a id="Cube_Tests_CollisionCacheTests_Zone_change_drops_the_boxes_at_once"></a> Zone\_change\_drops\_the\_boxes\_at\_once\(\)

```csharp
[Fact]
public void Zone_change_drops_the_boxes_at_once()
```
