# <a id="Cube_Tests_ReleaseVersionTests"></a> Class ReleaseVersionTests

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

Checks the plugin version in <code>Cube/Cube.csproj</code> is releasable (CLAUDE.md, "CI and releases"): it must be
<code>MAJOR.MINOR.PATCH.0</code> so the release workflow can match it to a <code>vMAJOR.MINOR.PATCH</code> tag, and it must
never be older than the version <code>repo.json</code> already publishes, or Dalamud would not offer the next release
as an update.

```csharp
public class ReleaseVersionTests
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ReleaseVersionTests](Cube.Tests.ReleaseVersionTests.md)


## Remarks

<code>repo.json</code> only exists after the first release (the release workflow writes it), so the check against the
real file passes until then; the comparison itself is tested with fixed documents so it is never untested.

## Methods

### <a id="Cube_Tests_ReleaseVersionTests_CsprojVersion"></a> CsprojVersion\(\)

Reads <code>&lt;Version&gt;</code> from the plugin csproj.

```csharp
private static string CsprojVersion()
```

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Cube_Tests_ReleaseVersionTests_Csproj_version_is_not_older_than_the_published_version"></a> Csproj\_version\_is\_not\_older\_than\_the\_published\_version\(\)

```csharp
[Fact]
public void Csproj_version_is_not_older_than_the_published_version()
```

### <a id="Cube_Tests_ReleaseVersionTests_Csproj_version_matches_the_tag_format"></a> Csproj\_version\_matches\_the\_tag\_format\(\)

```csharp
[Fact]
public void Csproj_version_matches_the_tag_format()
```

### <a id="Cube_Tests_ReleaseVersionTests_IsNotOlder_System_Version_System_Version_"></a> IsNotOlder\(Version, Version\)

True if the csproj version may be released over the published one (same or newer).

```csharp
private static bool IsNotOlder(Version csproj, Version published)
```

#### Parameters

`csproj` [Version](https://learn.microsoft.com/dotnet/api/system.version)

The version in the plugin csproj.

`published` [Version](https://learn.microsoft.com/dotnet/api/system.version)

The version repo.json publishes.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Tests_ReleaseVersionTests_Older_csproj_version_than_published_fails_System_String_System_String_System_Boolean_"></a> Older\_csproj\_version\_than\_published\_fails\(string, string, bool\)

```csharp
[Theory]
[InlineData(new object[] { "1.0.0.0", "1.0.0.0", true })]
[InlineData(new object[] { "1.0.1.0", "1.0.0.0", true })]
[InlineData(new object[] { "1.1.0.0", "1.0.9.0", true })]
[InlineData(new object[] { "1.0.0.0", "1.0.1.0", false })]
[InlineData(new object[] { "0.9.0.0", "1.0.0.0", false })]
public void Older_csproj_version_than_published_fails(string csproj, string published, bool ok)
```

#### Parameters

`csproj` [string](https://learn.microsoft.com/dotnet/api/system.string)

`published` [string](https://learn.microsoft.com/dotnet/api/system.string)

`ok` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Tests_ReleaseVersionTests_PublishedVersion_System_String_"></a> PublishedVersion\(string\)

Reads the published <code>AssemblyVersion</code> from a repo.json document (a one-entry list).

```csharp
private static Version PublishedVersion(string repoJson)
```

#### Parameters

`repoJson` [string](https://learn.microsoft.com/dotnet/api/system.string)

The repo.json text.

#### Returns

 [Version](https://learn.microsoft.com/dotnet/api/system.version)
