# <a id="Cube_Core_CubeSettings"></a> Class CubeSettings

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

The user's settings (PROJECT.md, "Features": cube size (width, height and depth since #90), player height in the cube (#91), orientation mode, labels on/off, colours and opacity; "NPCs":
NPCs on/off; "View orientation control": the saved view orientation and the control's corner; #32: collision on/off
and detail), with their defaults
and the rules for valid values. The plugin's saved configuration derives from this, so the rules are unit tested
without Dalamud.

```csharp
public class CubeSettings
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[CubeSettings](Cube.Core.CubeSettings.md)

#### Derived

[Configuration](Cube.Configuration.md)


## Remarks

Public fields, not properties, so the settings window can bind ImGui controls to them by reference. After any
change, call [Changed](Cube.Core.CubeSettings.md#Cube_Core_CubeSettings_Changed): it repairs bad values and bumps [Revision](Cube.Core.CubeSettings.md#Cube_Core_CubeSettings_Revision), which tells the window
to rebuild its cached colours (PROJECT.md, "Efficiency and memory": recompute only when inputs change).

## Fields

### <a id="Cube_Core_CubeSettings_BoxSizeVersion"></a> BoxSizeVersion

The settings file format that stores the cube as [CubeSize](Cube.Core.CubeSettings.md#Cube_Core_CubeSettings_CubeSize) (#90). Files older than this stored a
single [CubeEdge](Cube.Core.CubeSettings.md#Cube_Core_CubeSettings_CubeEdge), which [Upgrade](Cube.Core.CubeSettings.md#Cube_Core_CubeSettings_Upgrade) turns into three equal extents.

```csharp
public const int BoxSizeVersion = 3
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_CubeSettings_CollisionDetail"></a> CollisionDetail

How finely collision is drawn (#32); [Full](Cube.Core.CollisionDetail.md#Cube_Core_CollisionDetail_Full) by default.

```csharp
public CollisionDetail CollisionDetail
```

#### Field Value

 [CollisionDetail](Cube.Core.CollisionDetail.md)

### <a id="Cube_Core_CubeSettings_CollisionStyle"></a> CollisionStyle

How collision is drawn (#100): [Boxes](Cube.Core.CollisionStyle.md#Cube_Core_CollisionStyle_Boxes) (the default) or the slower
[Mesh](Cube.Core.CollisionStyle.md#Cube_Core_CollisionStyle_Mesh). Files saved before [CollisionStyleVersion](Cube.Core.CubeSettings.md#Cube_Core_CubeSettings_CollisionStyleVersion) load Boxes.

```csharp
public CollisionStyle CollisionStyle
```

#### Field Value

 [CollisionStyle](Cube.Core.CollisionStyle.md)

### <a id="Cube_Core_CubeSettings_CollisionStyleVersion"></a> CollisionStyleVersion

The settings file format that stores [CollisionStyle](Cube.Core.CubeSettings.md#Cube_Core_CubeSettings_CollisionStyle) (#100). Older files load Boxes, the style they
always showed.

```csharp
public const int CollisionStyleVersion = 5
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_CubeSettings_CubeEdge"></a> CubeEdge

The single edge files saved before [BoxSizeVersion](Cube.Core.CubeSettings.md#Cube_Core_CubeSettings_BoxSizeVersion) stored. Only read from those files, so their
size carries over as a cube with that edge; never written again.

```csharp
public float CubeEdge
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_CubeSettings_CubeSize"></a> CubeSize

The cube's width (X), height (Y) and depth (Z) in yalms (PROJECT.md, "Cube": each 1–100, default 40; #90).
Change it with [SetSize](Cube.Core.CubeSettings.md#Cube_Core_CubeSettings_SetSize) (zoom: [Zoom](Cube.Core.BoxExtents.md#Cube_Core_BoxExtents_Zoom) or [ZoomSlider](Cube.Core.ZoomSlider.md#Cube_Core_ZoomSlider)), or bind a
slider to a component and call
[Changed](Cube.Core.CubeSettings.md#Cube_Core_CubeSettings_Changed).

```csharp
public Vector3 CubeSize
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_CubeSettings_DefaultEnemyColour"></a> DefaultEnemyColour

Default enemy colour: red, so hostile battle NPCs stand apart from friendly ones (#111).

```csharp
public static readonly Vector3 DefaultEnemyColour
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_CubeSettings_DefaultInteractableColour"></a> DefaultInteractableColour

Default colour for text-labelled interactable objects: teal, distinct from the other kinds (#112).

```csharp
public static readonly Vector3 DefaultInteractableColour
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_CubeSettings_DefaultNpcColour"></a> DefaultNpcColour

Default NPC colour: purple, so NPCs stand apart from players (PROJECT.md, "Colours").

```csharp
public static readonly Vector3 DefaultNpcColour
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_CubeSettings_DefaultOtherColour"></a> DefaultOtherColour

Default colour for everyone else.

```csharp
public static readonly Vector3 DefaultOtherColour
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_CubeSettings_DefaultPartyColour"></a> DefaultPartyColour

Default party and alliance colour.

```csharp
public static readonly Vector3 DefaultPartyColour
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_CubeSettings_DefaultSelfColour"></a> DefaultSelfColour

Default player colours: blue for self, green for party/alliance, orange for others.

```csharp
public static readonly Vector3 DefaultSelfColour
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_CubeSettings_EnemyColour"></a> EnemyColour

Hostile battle NPCs' colour (#111). Files saved before it existed load the default.

```csharp
public Vector3 EnemyColour
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_CubeSettings_InteractableColour"></a> InteractableColour

Interactable objects' colour (#112). Files saved before it existed load the default.

```csharp
public Vector3 InteractableColour
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_CubeSettings_MinOpacity"></a> MinOpacity

Lowest opacity allowed, so the cubes can be made faint but never vanish entirely.

```csharp
public const float MinOpacity = 0.1
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_CubeSettings_NamePosition"></a> NamePosition

Whether names sit beside or above their cubes (#112); [Side](Cube.Core.NamePosition.md#Cube_Core_NamePosition_Side) by default.

```csharp
public NamePosition NamePosition
```

#### Field Value

 [NamePosition](Cube.Core.NamePosition.md)

### <a id="Cube_Core_CubeSettings_NpcColour"></a> NpcColour

Named NPCs' colour.

```csharp
public Vector3 NpcColour
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_CubeSettings_Opacity"></a> Opacity

Opacity of everything drawn except labels (which stay readable), [MinOpacity](Cube.Core.CubeSettings.md#Cube_Core_CubeSettings_MinOpacity)–1.

```csharp
public float Opacity
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_CubeSettings_Orientation"></a> Orientation

How the view turns (PROJECT.md, "View": Camera by default).

```csharp
public OrientationMode Orientation
```

#### Field Value

 [OrientationMode](Cube.Core.OrientationMode.md)

### <a id="Cube_Core_CubeSettings_OtherColour"></a> OtherColour

Everyone else's colour.

```csharp
public Vector3 OtherColour
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_CubeSettings_PartyColour"></a> PartyColour

Party and alliance members' colour.

```csharp
public Vector3 PartyColour
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_CubeSettings_PlayerHeight"></a> PlayerHeight

Where the player sits vertically in the box, as a fraction of its height from the bottom face (PROJECT.md,
"Player height in cube", #91): 0–1, default 0.5 (centred). The settings window shows it as 0–100%. Files saved
before [PlayerHeightVersion](Cube.Core.CubeSettings.md#Cube_Core_CubeSettings_PlayerHeightVersion) load it as 0.5.

```csharp
public float PlayerHeight
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_CubeSettings_PlayerHeightVersion"></a> PlayerHeightVersion

The settings file format that stores [PlayerHeight](Cube.Core.CubeSettings.md#Cube_Core_CubeSettings_PlayerHeight) (#91). Older files load it as centred (50%),
as PROJECT.md asks.

```csharp
public const int PlayerHeightVersion = 4
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_CubeSettings_RotationViewVersion"></a> RotationViewVersion

The settings file format that stores the view as [ViewRotation](Cube.Core.CubeSettings.md#Cube_Core_CubeSettings_ViewRotation). Files older than this stored the
old control's heading and tilt instead, which [Upgrade](Cube.Core.CubeSettings.md#Cube_Core_CubeSettings_Upgrade) converts.

```csharp
public const int RotationViewVersion = 2
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_CubeSettings_SelfColour"></a> SelfColour

The local player's colour (red, green, blue, each 0–1).

```csharp
public Vector3 SelfColour
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_CubeSettings_ShowCollision"></a> ShowCollision

Whether collision is shown (#32; PROJECT.md, "Collision (phase 2)"), default on. When off, the collision scene
isn't read at all and the gathered boxes are dropped, like NPCs off. A config file saved before this setting
existed loads it as on.

```csharp
public bool ShowCollision
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_CubeSettings_ShowEnemies"></a> ShowEnemies

Whether hostile battle NPCs are shown (#112), default on.

```csharp
public bool ShowEnemies
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_CubeSettings_ShowInteractables"></a> ShowInteractables

Whether text-labelled interactable objects are shown (#112), default on. When off the reader skips them.

```csharp
public bool ShowInteractables
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_CubeSettings_ShowLabels"></a> ShowLabels

Whether other players', NPCs' and objects' names are drawn (PROJECT.md: labels on/off).

```csharp
public bool ShowLabels
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_CubeSettings_ShowNpcs"></a> ShowNpcs

Whether named NPCs are shown (PROJECT.md, "NPCs": a setting turns them on or off, default on). When off, the
game reader doesn't even look at NPCs. A config file saved before this setting existed loads it as on.

```csharp
public bool ShowNpcs
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_CubeSettings_ShowOwnName"></a> ShowOwnName

Whether the local player's own name is drawn next to their cube (#112), default on.

```csharp
public bool ShowOwnName
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_CubeSettings_ShowPlayers"></a> ShowPlayers

Whether other players (party, alliance and everyone else) are shown (#112), default on. The local player is always shown.

```csharp
public bool ShowPlayers
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_CubeSettings_ViewPitchDegrees"></a> ViewPitchDegrees

The old control's tilt, in degrees; read like [ViewYawDegrees](Cube.Core.CubeSettings.md#Cube_Core_CubeSettings_ViewYawDegrees). Defaults to 30° (the default view).

```csharp
public float ViewPitchDegrees
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_CubeSettings_ViewRotation"></a> ViewRotation

The view control's rotation (PROJECT.md, "View orientation control": the chosen orientation is saved, any
orientation including roll). Use [View](Cube.Core.CubeSettings.md#Cube_Core_CubeSettings_View) to read it.

```csharp
public Quaternion ViewRotation
```

#### Field Value

 [Quaternion](https://learn.microsoft.com/dotnet/api/system.numerics.quaternion)

### <a id="Cube_Core_CubeSettings_ViewWidgetCorner"></a> ViewWidgetCorner

Which corner of the cube window the view control is drawn in (PROJECT.md: bottom right by default).

```csharp
public WidgetCorner ViewWidgetCorner
```

#### Field Value

 [WidgetCorner](Cube.Core.WidgetCorner.md)

### <a id="Cube_Core_CubeSettings_ViewYawDegrees"></a> ViewYawDegrees

The old yaw-and-tilt control's heading, in degrees (#63). Only read from files saved before
[RotationViewVersion](Cube.Core.CubeSettings.md#Cube_Core_CubeSettings_RotationViewVersion), so their view carries over (PROJECT.md); never written again.

```csharp
public float ViewYawDegrees
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

## Properties

### <a id="Cube_Core_CubeSettings_Revision"></a> Revision

Goes up by one on every [Changed](Cube.Core.CubeSettings.md#Cube_Core_CubeSettings_Changed); readers compare it with the revision they last built from.
Not saved (`System.Runtime.Serialization.IgnoreDataMemberAttribute` is honoured by the JSON
serialiser Dalamud uses for configurations, without Core referencing it).

```csharp
[IgnoreDataMember]
public int Revision { get; private set; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_CubeSettings_View"></a> View

The saved view orientation.

```csharp
[IgnoreDataMember]
public ViewOrientation View { get; }
```

#### Property Value

 [ViewOrientation](Cube.Core.ViewOrientation.md)

## Methods

### <a id="Cube_Core_CubeSettings_Changed"></a> Changed\(\)

Repairs out-of-range or corrupt values (e.g. from a hand-edited config file) and marks the settings as
changed. A bad file can never produce a broken cube or an invisible one.

```csharp
public void Changed()
```

### <a id="Cube_Core_CubeSettings_CopyFrom_Cube_Core_CubeSettings_"></a> CopyFrom\(CubeSettings\)

Copies every setting from another instance, then marks them changed. Used to undo test changes.

```csharp
public void CopyFrom(CubeSettings other)
```

#### Parameters

`other` [CubeSettings](Cube.Core.CubeSettings.md)

The settings to copy.

### <a id="Cube_Core_CubeSettings_Pack_System_Numerics_Vector3_System_Single_"></a> Pack\(Vector3, float\)

Packs an RGB colour with an alpha.

```csharp
private static uint Pack(Vector3 rgb, float alpha)
```

#### Parameters

`rgb` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

Red, green and blue.

`alpha` [float](https://learn.microsoft.com/dotnet/api/system.single)

Opacity.

#### Returns

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

### <a id="Cube_Core_CubeSettings_Palette"></a> Palette\(\)

The player and NPC colours at the chosen opacity, packed for drawing.

```csharp
public PlayerPalette Palette()
```

#### Returns

 [PlayerPalette](Cube.Core.PlayerPalette.md)

### <a id="Cube_Core_CubeSettings_ResetSize"></a> ResetSize\(\)

Puts the cube back to its default size, 40 × 40 × 40 (the settings window's "Reset size").

```csharp
public void ResetSize()
```

### <a id="Cube_Core_CubeSettings_ResetView"></a> ResetView\(\)

Puts the view back to the default (PROJECT.md: "There's a way to reset to the default").

```csharp
public void ResetView()
```

### <a id="Cube_Core_CubeSettings_SetSize_System_Numerics_Vector3_"></a> SetSize\(Vector3\)

Stores a new cube size (from the scale gizmo's axis handles), clamped, and marks the settings changed.

```csharp
public void SetSize(Vector3 size)
```

#### Parameters

`size` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The width, height and depth the user chose.

### <a id="Cube_Core_CubeSettings_SetView_Cube_Core_ViewOrientation__"></a> SetView\(in ViewOrientation\)

Stores a new view orientation (from the view control) and marks the settings changed.

```csharp
public void SetView(in ViewOrientation view)
```

#### Parameters

`view` [ViewOrientation](Cube.Core.ViewOrientation.md)

The orientation the user chose.

### <a id="Cube_Core_CubeSettings_ShouldSerializeCubeEdge"></a> ShouldSerializeCubeEdge\(\)

Never writes the old single edge, so saved files only hold [CubeSize](Cube.Core.CubeSettings.md#Cube_Core_CubeSettings_CubeSize); see [ShouldSerializeViewYawDegrees](Cube.Core.CubeSettings.md#Cube_Core_CubeSettings_ShouldSerializeViewYawDegrees).

```csharp
public bool ShouldSerializeCubeEdge()
```

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Always false.

### <a id="Cube_Core_CubeSettings_ShouldSerializeViewPitchDegrees"></a> ShouldSerializeViewPitchDegrees\(\)

Never writes the old tilt; see [ShouldSerializeViewYawDegrees](Cube.Core.CubeSettings.md#Cube_Core_CubeSettings_ShouldSerializeViewYawDegrees).

```csharp
public bool ShouldSerializeViewPitchDegrees()
```

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Always false.

### <a id="Cube_Core_CubeSettings_ShouldSerializeViewYawDegrees"></a> ShouldSerializeViewYawDegrees\(\)

Tells the JSON serialiser Dalamud uses (Newtonsoft, by naming convention) never to write the old heading, so
saved files only hold [ViewRotation](Cube.Core.CubeSettings.md#Cube_Core_CubeSettings_ViewRotation).

```csharp
public bool ShouldSerializeViewYawDegrees()
```

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Always false.

### <a id="Cube_Core_CubeSettings_Upgrade_System_Int32_"></a> Upgrade\(int\)

Brings settings loaded from an older file format up to date, then repairs them with [Changed](Cube.Core.CubeSettings.md#Cube_Core_CubeSettings_Changed).
Files before [RotationViewVersion](Cube.Core.CubeSettings.md#Cube_Core_CubeSettings_RotationViewVersion) stored the view as a heading and tilt; they become the same
orientation (PROJECT.md: orientations saved by the old control carry over). A file from before the view
control existed has neither, so the old fields' defaults give the default view. Files before
[BoxSizeVersion](Cube.Core.CubeSettings.md#Cube_Core_CubeSettings_BoxSizeVersion) stored one cube edge; it becomes all three extents (#90). Files before
[PlayerHeightVersion](Cube.Core.CubeSettings.md#Cube_Core_CubeSettings_PlayerHeightVersion) had no player height; it becomes 50%, the centred cube they showed (#91). Files
before [CollisionStyleVersion](Cube.Core.CubeSettings.md#Cube_Core_CubeSettings_CollisionStyleVersion) had no collision style; it becomes Boxes (#100).

```csharp
public void Upgrade(int fileVersion)
```

#### Parameters

`fileVersion` [int](https://learn.microsoft.com/dotnet/api/system.int32)

The format version the settings were saved with.

### <a id="Cube_Core_CubeSettings_ValidColour_System_Numerics_Vector3_System_Numerics_Vector3_"></a> ValidColour\(Vector3, Vector3\)

Clamps each channel to 0–1; a colour with a non-number channel is replaced by the default.

```csharp
private static Vector3 ValidColour(Vector3 colour, Vector3 fallback)
```

#### Parameters

`colour` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The stored colour.

`fallback` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The default to use if it's corrupt.

#### Returns

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_CubeSettings_WithOpacity_System_Numerics_Vector3_System_Single_"></a> WithOpacity\(Vector3, float\)

A colour at the chosen opacity, packed for drawing; used for the wireframe.

```csharp
public uint WithOpacity(Vector3 rgb, float alpha)
```

#### Parameters

`rgb` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

Red, green and blue, 0–1.

`alpha` [float](https://learn.microsoft.com/dotnet/api/system.single)

The colour's own opacity before the setting is applied.

#### Returns

 [uint](https://learn.microsoft.com/dotnet/api/system.uint32)
