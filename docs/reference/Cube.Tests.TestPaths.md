# <a id="Cube_Tests_TestPaths"></a> Class TestPaths

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

Locates files in the repository from a test's output folder, for tests that inspect the repo or build output.

```csharp
internal static class TestPaths
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[TestPaths](Cube.Tests.TestPaths.md)


## Methods

### <a id="Cube_Tests_TestPaths_RepoRoot"></a> RepoRoot\(\)

Finds the repository root (the folder holding <code>Cube.slnx</code>) above the test's output folder.

```csharp
public static string RepoRoot()
```

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)
