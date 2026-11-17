# <a id="Cube_Tests_BuildInfoTests"></a> Class BuildInfoTests

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

Checks the build line logged on load (CLAUDE.md: log which build is running and when it was built),
so a stale or wrong-configuration build is obvious.

```csharp
public class BuildInfoTests
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BuildInfoTests](Cube.Tests.BuildInfoTests.md)


## Fields

### <a id="Cube_Tests_BuildInfoTests_BuiltAt"></a> BuiltAt

```csharp
private static readonly DateTime BuiltAt
```

#### Field Value

 [DateTime](https://learn.microsoft.com/dotnet/api/system.datetime)

### <a id="Cube_Tests_BuildInfoTests_PluginVersion"></a> PluginVersion

```csharp
private static readonly Version PluginVersion
```

#### Field Value

 [Version](https://learn.microsoft.com/dotnet/api/system.version)

## Methods

### <a id="Cube_Tests_BuildInfoTests_Build_time_is_read_fresh_after_the_file_changes"></a> Build\_time\_is\_read\_fresh\_after\_the\_file\_changes\(\)

```csharp
[Fact]
public void Build_time_is_read_fresh_after_the_file_changes()
```

### <a id="Cube_Tests_BuildInfoTests_Describes_configuration_version_time_and_path"></a> Describes\_configuration\_version\_time\_and\_path\(\)

```csharp
[Fact]
public void Describes_configuration_version_time_and_path()
```

### <a id="Cube_Tests_BuildInfoTests_Omits_the_path_when_unknown_System_String_"></a> Omits\_the\_path\_when\_unknown\(string?\)

```csharp
[Theory]
[InlineData(null)]
[InlineData(new object[] { "" })]
[InlineData(new object[] { "  " })]
public void Omits_the_path_when_unknown(string? path)
```

#### Parameters

`path` [string](https://learn.microsoft.com/dotnet/api/system.string)?

### <a id="Cube_Tests_BuildInfoTests_Time_format_does_not_depend_on_the_current_culture"></a> Time\_format\_does\_not\_depend\_on\_the\_current\_culture\(\)

```csharp
[Fact]
public void Time_format_does_not_depend_on_the_current_culture()
```
