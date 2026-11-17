# <a id="Cube_Tests_CoreAssemblyTests"></a> Class CoreAssemblyTests

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

Checks that Core stays free of Dalamud, game structs and Lumina, so all its logic can be unit tested
without the game (CLAUDE.md, "Reference: the previous plugin" layout rule).

```csharp
public class CoreAssemblyTests
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[CoreAssemblyTests](Cube.Tests.CoreAssemblyTests.md)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Methods

### <a id="Cube_Tests_CoreAssemblyTests_Core_does_not_reference_game_assemblies_System_String_"></a> Core\_does\_not\_reference\_game\_assemblies\(string\)

```csharp
[Theory]
[InlineData(new object[] { "Dalamud" })]
[InlineData(new object[] { "FFXIVClientStructs" })]
[InlineData(new object[] { "Lumina" })]
[InlineData(new object[] { "InteropGenerator" })]
public void Core_does_not_reference_game_assemblies(string forbiddenPrefix)
```

#### Parameters

`forbiddenPrefix` [string](https://learn.microsoft.com/dotnet/api/system.string)
