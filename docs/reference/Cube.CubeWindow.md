# <a id="Cube_CubeWindow"></a> Class CubeWindow

Namespace: [Cube](Cube.md)  
Assembly: Cube.dll  

The window the cube is drawn in (PROJECT.md, "Features": a Dalamud window toggled by <code>/cube</code>).

```csharp
internal sealed class CubeWindow : Window, IWindow
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[Window](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs) ← 
[CubeWindow](Cube.CubeWindow.md)

#### Implements

[IWindow](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/IWindow.cs)

#### Inherited Members

[Window.Toggle\(\)](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs), 
[Window.BringToFront\(\)](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs), 
[Window.PreOpenCheck\(\)](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs), 
[Window.DrawConditions\(\)](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs), 
[Window.PreDraw\(\)](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs), 
[Window.PostDraw\(\)](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs), 
[Window.Draw\(\)](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs), 
[Window.OnOpen\(\)](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs), 
[Window.OnClose\(\)](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs), 
[Window.OnSafeToRemove\(\)](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs), 
[Window.Update\(\)](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs), 
[Window.Namespace](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs), 
[Window.WindowName](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs), 
[Window.IsFocused](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs), 
[Window.IsHovered](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs), 
[Window.RespectCloseHotkey](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs), 
[Window.InhibitAtkCollision](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs), 
[Window.DisableWindowSounds](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs), 
[Window.OnOpenSfxId](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs), 
[Window.OnCloseSfxId](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs), 
[Window.DisableFadeInFadeOut](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs), 
[Window.Position](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs), 
[Window.PositionCondition](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs), 
[Window.Size](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs), 
[Window.SizeCondition](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs), 
[Window.SizeConstraints](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs), 
[Window.Collapsed](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs), 
[Window.CollapsedCondition](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs), 
[Window.Flags](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs), 
[Window.ForceMainWindow](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs), 
[Window.BgAlpha](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs), 
[Window.ShowCloseButton](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs), 
[Window.AllowPinning](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs), 
[Window.AllowClickthrough](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs), 
[Window.AllowBackgroundBlur](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs), 
[Window.IsPinned](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs), 
[Window.IsClickthrough](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs), 
[Window.TitleBarButtons](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs), 
[Window.IsOpen](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs), 
[Window.RequestFocus](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs), 
[Window.IsTopMost](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Interface/Windowing/Window.cs), 
[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Remarks

Empty for now; drawing arrives with the projection and draw list work.

## Constructors

### <a id="Cube_CubeWindow__ctor"></a> CubeWindow\(\)

Creates the window, closed, with a sensible minimum size.

```csharp
public CubeWindow()
```

## Methods

### <a id="Cube_CubeWindow_Draw"></a> Draw\(\)

Draws the window's contents. Nothing yet.

```csharp
public override void Draw()
```
