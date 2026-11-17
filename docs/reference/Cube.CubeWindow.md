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

## Properties

### <a id="Cube_CubeWindow_DrawCost"></a> DrawCost

Time and allocations of each [Draw](Cube.CubeWindow.md#Cube_CubeWindow_Draw) call, read by the in-game test bridge to check
PROJECT.md's budgets (under 0.5 ms average, zero bytes per frame). Debug builds only.

```csharp
internal DrawCostSampler DrawCost { get; }
```

#### Property Value

 [DrawCostSampler](Cube.Core.DrawCostSampler.md)

#### Remarks

Holds the last 240 frames (about 4 seconds at 60 fps); fixed size, so measuring allocates nothing.

## Methods

### <a id="Cube_CubeWindow_Draw"></a> Draw\(\)

Draws the window's contents, measuring the cost in Debug builds.

```csharp
public override void Draw()
```

### <a id="Cube_CubeWindow_DrawContents"></a> DrawContents\(\)

The actual drawing: the plugin's per-frame draw path. Nothing yet.

```csharp
private void DrawContents()
```
