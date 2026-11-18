# <a id="Cube_Core_CubeAnchor"></a> Class CubeAnchor

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

Decides where the cube is anchored each frame (PROJECT.md, "Exact behaviour rules", Cube, "Jump"; #105): on the
local player, except that while the player is jumping its height is held at the take-off height, so the world
stays still in the cube and the player's marker visibly rises and lands.

```csharp
public sealed class CubeAnchor
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[CubeAnchor](Cube.Core.CubeAnchor.md)


## Remarks

<p>
A small two-state machine, <i>following</i> and <i>held</i>. While following, [Update](Cube.Core.CubeAnchor.md#Cube_Core_CubeAnchor_Update) returns the
player's position unchanged, so everything behaves exactly as before #105. While held, x and z still follow the
player every frame and only y is replaced by the take-off height. Landing ends the hold in the frame the flag
clears, so landing on a ledge higher or lower than the take-off point snaps the cube to the new height, as walking
up or down does.
</p>
<p>
Walking off a ledge, falling, swimming, mounts, flying and being moved by the game never set the jump flag, so they
never hold. The take-off height is the previous frame's height, because the game may raise the jump flag a frame
after the character leaves the ground. The anchor feeds [CubeSpace](Cube.Core.CubeSpace.md#Cube_Core_CubeSpace) as the player position, so the
player-height offset ([PlayerHeight](Cube.Core.PlayerHeight.md#Cube_Core_PlayerHeight)) is applied to the held height the same way. Pure maths with no
game references and no allocations, so it is unit tested without the game.
</p>

## Fields

### <a id="Cube_Core_CubeAnchor_hasLast"></a> hasLast

Whether [lastY](Cube.Core.CubeAnchor.md#Cube_Core_CubeAnchor_lastY) holds a real frame (false at the start and after [Reset](Cube.Core.CubeAnchor.md#Cube_Core_CubeAnchor_Reset)).

```csharp
private bool hasLast
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_CubeAnchor_heldY"></a> heldY

The height the cube is held at while [IsHeld](Cube.Core.CubeAnchor.md#Cube_Core_CubeAnchor_IsHeld).

```csharp
private float heldY
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_CubeAnchor_lastY"></a> lastY

The previous frame's player height, used as the take-off height when a jump starts.

```csharp
private float lastY
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

## Properties

### <a id="Cube_Core_CubeAnchor_HeldY"></a> HeldY

The height the cube is held at; only meaningful while [IsHeld](Cube.Core.CubeAnchor.md#Cube_Core_CubeAnchor_IsHeld).

```csharp
public float HeldY { get; }
```

#### Property Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_CubeAnchor_IsHeld"></a> IsHeld

Whether the cube is currently held at the take-off height.

```csharp
public bool IsHeld { get; private set; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Methods

### <a id="Cube_Core_CubeAnchor_Reset"></a> Reset\(\)

Forgets the previous frame and any hold. Called whenever the cube isn't being built (hidden, no local player),
so a jump can't carry a stale height across a zone change or a loading screen.

```csharp
public void Reset()
```

### <a id="Cube_Core_CubeAnchor_Update_System_Numerics_Vector3_System_Boolean_"></a> Update\(Vector3, bool\)

Gives the position the cube is anchored on this frame.

```csharp
public Vector3 Update(Vector3 player, bool isJumping)
```

#### Parameters

`player` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The local player's world position this frame.

`isJumping` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether the game says the local player is jumping.

#### Returns

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The player's position, with y replaced by the take-off height while the jump lasts.
