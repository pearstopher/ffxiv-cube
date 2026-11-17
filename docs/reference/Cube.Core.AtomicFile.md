# <a id="Cube_Core_AtomicFile"></a> Class AtomicFile

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

Writes a file so that readers only ever see the old or the complete new content, never a half-written one.

```csharp
public static class AtomicFile
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[AtomicFile](Cube.Core.AtomicFile.md)


## Remarks

The in-game test bridge writes its results while the test script may be polling for them
(PROJECT.md, "Automated testing"); a partial file would read as broken JSON. The content goes to a
temporary file next to the target first, then replaces the target in one move.

## Fields

### <a id="Cube_Core_AtomicFile_TemporarySuffix"></a> TemporarySuffix

The suffix of the temporary file written before the final move.

```csharp
public const string TemporarySuffix = ".tmp"
```

#### Field Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

## Methods

### <a id="Cube_Core_AtomicFile_WriteAllText_System_String_System_String_"></a> WriteAllText\(string, string\)

Writes <code class="paramref">contents</code> to <code class="paramref">path</code> as UTF-8 (no BOM), replacing any existing file.

```csharp
public static void WriteAllText(string path, string contents)
```

#### Parameters

`path` [string](https://learn.microsoft.com/dotnet/api/system.string)

The file to write.

`contents` [string](https://learn.microsoft.com/dotnet/api/system.string)

The full new contents.
