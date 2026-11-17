# <a id="Cube_Core_BuildInfo"></a> Class BuildInfo

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

Formats the build details the plugin logs when it loads, so a stale build
(wrong configuration, or not rebuilt) is obvious in <code>/xllog</code>.

```csharp
public static class BuildInfo
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BuildInfo](Cube.Core.BuildInfo.md)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Methods

### <a id="Cube_Core_BuildInfo_Describe_System_String_System_DateTime_System_String_"></a> Describe\(string, DateTime, string?\)

Returns text like <code>Debug build, built 2026-11-11 10:36:02, from C:\...\Cube.dll</code>.

```csharp
public static string Describe(string configuration, DateTime builtAt, string? dllPath)
```

#### Parameters

`configuration` [string](https://learn.microsoft.com/dotnet/api/system.string)

Build configuration, e.g. <code>Debug</code> or <code>Release</code>.

`builtAt` [DateTime](https://learn.microsoft.com/dotnet/api/system.datetime)

When the DLL was written, in local time.

`dllPath` [string](https://learn.microsoft.com/dotnet/api/system.string)?

Full path of the loaded DLL, if known.

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Cube_Core_BuildInfo_ReadBuildTime_System_String_"></a> ReadBuildTime\(string\)

Reads the DLL's last write time fresh from disk, in local time.

```csharp
public static DateTime ReadBuildTime(string dllPath)
```

#### Parameters

`dllPath` [string](https://learn.microsoft.com/dotnet/api/system.string)

Full path of the plugin DLL.

#### Returns

 [DateTime](https://learn.microsoft.com/dotnet/api/system.datetime)

#### Remarks

Don't use a reused `System.IO.FileInfo` (such as Dalamud's <code>AssemblyLocation</code>):
it caches timestamps, so after a hot reload it reports the previous build's time.
