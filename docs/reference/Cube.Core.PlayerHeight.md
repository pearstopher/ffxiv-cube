# <a id="Cube_Core_PlayerHeight"></a> Class PlayerHeight

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

The rules for where the player sits vertically in the box (PROJECT.md, "Exact behaviour rules", Cube, "Player height
in cube"; #91): a fraction <code>p</code> of the box's height, from 0 (player on the bottom face) to 1 (player on the top
face), default 0.5 (centred). The box spans <code>player.y - height × p</code> to <code>player.y + height × (1 - p)</code>;
x and z stay centred on the player.

```csharp
public static class PlayerHeight
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[PlayerHeight](Cube.Core.PlayerHeight.md)


## Remarks

<p>
Rather than teach every reader about an off-centre player, the shift is applied once: [BoxCentre](Cube.Core.PlayerHeight.md#Cube_Core_PlayerHeight_BoxCentre) turns
the player's position into the box's centre, and [CubeSpace](Cube.Core.CubeSpace.md#Cube_Core_CubeSpace) is built around that centre. Culling,
projection and the collision region then all use the shifted box with no further changes, and the projection keeps
drawing the box centred in the widget and turning it around its own centre, as PROJECT.md asks.
</p>
<p>
Because <code>p</code> is a fraction of the height, it keeps working when #90's height slider or zoom changes the height.
Pure maths, allocation-free, unit tested without the game.
</p>

## Fields

### <a id="Cube_Core_PlayerHeight_Default"></a> Default

Default: the player is centred vertically, as before #91.

```csharp
public const float Default = 0.5
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_PlayerHeight_Max"></a> Max

Highest value: the player is on the top face, so the box shows only what's below them.

```csharp
public const float Max = 1
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_PlayerHeight_Min"></a> Min

Lowest value: the player is on the bottom face, so the box shows only what's above them.

```csharp
public const float Min = 0
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

## Methods

### <a id="Cube_Core_PlayerHeight_BoxCentre_System_Numerics_Vector3_System_Single_System_Single_"></a> BoxCentre\(Vector3, float, float\)

The box's centre for a player position: the player's x and z, and a y shifted so the player sits at
<code class="paramref">fraction</code> of the height from the bottom face.

```csharp
public static Vector3 BoxCentre(Vector3 player, float height, float fraction)
```

#### Parameters

`player` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The local player's world position.

`height` [float](https://learn.microsoft.com/dotnet/api/system.single)

The box's height in yalms, already clamped ([Size](Cube.Core.CubeSpace.md#Cube_Core_CubeSpace_Size).Y).

`fraction` [float](https://learn.microsoft.com/dotnet/api/system.single)

The player height setting; clamped first.

#### Returns

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

#### Remarks

The box spans centre ± height/2, so a centre of <code>player.y + height × (0.5 - p)</code> gives a bottom face at
<code>player.y - height × p</code> and a top face at <code>player.y + height × (1 - p)</code> (PROJECT.md). At the default
the shift is exactly zero, so nothing changes from the centred cube.

### <a id="Cube_Core_PlayerHeight_Clamp_System_Single_"></a> Clamp\(float\)

Clamps a fraction to 0–1; a non-number becomes [Default](Cube.Core.PlayerHeight.md#Cube_Core_PlayerHeight_Default).

```csharp
public static float Clamp(float fraction)
```

#### Parameters

`fraction` [float](https://learn.microsoft.com/dotnet/api/system.single)

The requested player height, as a fraction of the box's height.

#### Returns

 [float](https://learn.microsoft.com/dotnet/api/system.single)
