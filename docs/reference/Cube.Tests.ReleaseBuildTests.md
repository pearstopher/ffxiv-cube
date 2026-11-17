# <a id="Cube_Tests_ReleaseBuildTests"></a> Class ReleaseBuildTests

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

Checks the in-game test bridge is compiled out of Release builds (CLAUDE.md, "Building and testing":
the bridge and fake-player code must not ship), and is present in Debug builds where the in-game checks need it.

```csharp
public class ReleaseBuildTests
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ReleaseBuildTests](Cube.Tests.ReleaseBuildTests.md)


## Remarks

Reads the plugin DLL built with the same configuration as this test, using only its metadata, so neither
the plugin nor Dalamud is loaded. CI runs the tests in Release, so the Release case is checked on every push.

## Fields

### <a id="Cube_Tests_ReleaseBuildTests_Configuration"></a> Configuration

```csharp
private const string Configuration = "Debug"
```

#### Field Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

## Methods

### <a id="Cube_Tests_ReleaseBuildTests_PluginNamespaces"></a> PluginNamespaces\(\)

Returns the namespaces of all types in the plugin DLL of this test's configuration.

```csharp
private static HashSet<string> PluginNamespaces()
```

#### Returns

 [HashSet](https://learn.microsoft.com/dotnet/api/system.collections.generic.hashset\-1)<[string](https://learn.microsoft.com/dotnet/api/system.string)\>

### <a id="Cube_Tests_ReleaseBuildTests_Test_bridge_is_only_in_debug_builds"></a> Test\_bridge\_is\_only\_in\_debug\_builds\(\)

```csharp
[Fact]
public void Test_bridge_is_only_in_debug_builds()
```
