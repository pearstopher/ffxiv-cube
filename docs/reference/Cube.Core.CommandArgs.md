# <a id="Cube_Core_CommandArgs"></a> Class CommandArgs

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

Reads what was typed after <code>/cube</code> (PROJECT.md, "Features": <code>/cube</code> toggles the window, <code>/cube config</code> opens the settings).

```csharp
public static class CommandArgs
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[CommandArgs](Cube.Core.CommandArgs.md)


## Methods

### <a id="Cube_Core_CommandArgs_IsConfig_System_String_"></a> IsConfig\(string?\)

Whether the arguments ask for the settings: <code>config</code>, in any case, ignoring surrounding spaces.

```csharp
public static bool IsConfig(string? args)
```

#### Parameters

`args` [string](https://learn.microsoft.com/dotnet/api/system.string)?

Everything after the command.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)
