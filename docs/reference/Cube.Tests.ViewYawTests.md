# <a id="Cube_Tests_ViewYawTests"></a> Class ViewYawTests

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

Checks the view's orientation modes (PROJECT.md, "View"): Camera mode follows the game camera's heading so
"forward on screen" matches the game view; North up is fixed with north at the back. The heading comes from
the camera's direction vector (docs/research-notes.md, #10).

```csharp
public class ViewYawTests
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ViewYawTests](Cube.Tests.ViewYawTests.md)


## Methods

### <a id="Cube_Tests_ViewYawTests_Camera_heading_becomes_yaw_clockwise_from_north_System_Single_System_Single_System_Single_"></a> Camera\_heading\_becomes\_yaw\_clockwise\_from\_north\(float, float, float\)

```csharp
[Theory]
[InlineData(new object[] { 0, -1, 0 })]
[InlineData(new object[] { 1, 0, 1.5707964 })]
[InlineData(new object[] { 0, 1, 3.1415927 })]
[InlineData(new object[] { -1, 0, -1.5707964 })]
public void Camera_heading_becomes_yaw_clockwise_from_north(float x, float z, float expected)
```

#### Parameters

`x` [float](https://learn.microsoft.com/dotnet/api/system.single)

`z` [float](https://learn.microsoft.com/dotnet/api/system.single)

`expected` [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Tests_ViewYawTests_Camera_is_the_default_mode"></a> Camera\_is\_the\_default\_mode\(\)

```csharp
[Fact]
public void Camera_is_the_default_mode()
```

### <a id="Cube_Tests_ViewYawTests_Camera_looking_straight_down_keeps_the_previous_yaw"></a> Camera\_looking\_straight\_down\_keeps\_the\_previous\_yaw\(\)

```csharp
[Fact]
public void Camera_looking_straight_down_keeps_the_previous_yaw()
```

### <a id="Cube_Tests_ViewYawTests_Camera_mode_follows_the_camera"></a> Camera\_mode\_follows\_the\_camera\(\)

```csharp
[Fact]
public void Camera_mode_follows_the_camera()
```

### <a id="Cube_Tests_ViewYawTests_Camera_pitch_does_not_change_yaw"></a> Camera\_pitch\_does\_not\_change\_yaw\(\)

```csharp
[Fact]
public void Camera_pitch_does_not_change_yaw()
```

### <a id="Cube_Tests_ViewYawTests_North_up_mode_ignores_the_camera"></a> North\_up\_mode\_ignores\_the\_camera\(\)

```csharp
[Fact]
public void North_up_mode_ignores_the_camera()
```

### <a id="Cube_Tests_ViewYawTests_Real_yaw_change_turns_the_view"></a> Real\_yaw\_change\_turns\_the\_view\(\)

```csharp
[Fact]
public void Real_yaw_change_turns_the_view()
```

### <a id="Cube_Tests_ViewYawTests_Switching_to_north_up_always_applies"></a> Switching\_to\_north\_up\_always\_applies\(\)

```csharp
[Fact]
public void Switching_to_north_up_always_applies()
```

### <a id="Cube_Tests_ViewYawTests_Tiny_yaw_change_keeps_the_previous_yaw"></a> Tiny\_yaw\_change\_keeps\_the\_previous\_yaw\(\)

```csharp
[Fact]
public void Tiny_yaw_change_keeps_the_previous_yaw()
```

### <a id="Cube_Tests_ViewYawTests_Yaw_change_is_measured_the_short_way_round"></a> Yaw\_change\_is\_measured\_the\_short\_way\_round\(\)

```csharp
[Fact]
public void Yaw_change_is_measured_the_short_way_round()
```
