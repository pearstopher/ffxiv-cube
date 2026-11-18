# <a id="Cube_Core_CubeSettings"></a> Class CubeSettings

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

The user's settings (PROJECT.md, "Features": cube size, orientation mode, labels on/off, colours and opacity; "NPCs":
NPCs on/off; "View
orientation control": the saved view orientation and the control's corner),
with their defaults and the rules for valid values. The plugin's saved configuration derives from this, so the
rules are unit tested without Dalamud.

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

### <a id="Cube_Core_CubeSettings_CubeEdge"></a> CubeEdge

The cube's edge in yalms (PROJECT.md: 10–100, default 40).

```csharp
public float CubeEdge
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

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

### <a id="Cube_Core_CubeSettings_MinOpacity"></a> MinOpacity

Lowest opacity allowed, so the cubes can be made faint but never vanish entirely.

```csharp
public const float MinOpacity = 0.1
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

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

### <a id="Cube_Core_CubeSettings_SelfColour"></a> SelfColour

The local player's colour (red, green, blue, each 0–1).

```csharp
public Vector3 SelfColour
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_CubeSettings_ShowLabels"></a> ShowLabels

Whether other players' names are drawn (PROJECT.md: labels on/off).

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

### <a id="Cube_Core_CubeSettings_ViewPitchDegrees"></a> ViewPitchDegrees

The view control's tilt above the horizontal, in degrees (default 30°, ±90° at most).

```csharp
public float ViewPitchDegrees
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Cube_Core_CubeSettings_ViewWidgetCorner"></a> ViewWidgetCorner

Which corner of the cube window the view control is drawn in (PROJECT.md: bottom right by default).

```csharp
public WidgetCorner ViewWidgetCorner
```

#### Field Value

 [WidgetCorner](Cube.Core.WidgetCorner.md)

### <a id="Cube_Core_CubeSettings_ViewYawDegrees"></a> ViewYawDegrees

The view control's heading, in degrees (PROJECT.md, "View orientation control": the chosen orientation is
saved). Degrees, so a hand-edited file is readable. Use [View](Cube.Core.CubeSettings.md#Cube_Core_CubeSettings_View) to read it.

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

### <a id="Cube_Core_CubeSettings_ResetView"></a> ResetView\(\)

Puts the view back to the default (PROJECT.md: "There's a way to reset to the default").

```csharp
public void ResetView()
```

### <a id="Cube_Core_CubeSettings_SetView_Cube_Core_ViewOrientation__"></a> SetView\(in ViewOrientation\)

Stores a new view orientation (from the view control) and marks the settings changed.

```csharp
public void SetView(in ViewOrientation view)
```

#### Parameters

`view` [ViewOrientation](Cube.Core.ViewOrientation.md)

The orientation the user chose.

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
