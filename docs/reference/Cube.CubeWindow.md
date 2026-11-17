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
