# <a id="Cube_Tests_AtomicFileTests"></a> Class AtomicFileTests

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

Checks the in-game bridge's results are written atomically (PROJECT.md, "Automated testing":
the script polls the results file, so it must never see half a file).

```csharp
public sealed class AtomicFileTests : IDisposable
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[AtomicFileTests](Cube.Tests.AtomicFileTests.md)

#### Implements

[IDisposable](https://learn.microsoft.com/dotnet/api/system.idisposable)


## Fields

### <a id="Cube_Tests_AtomicFileTests_folder"></a> folder

```csharp
private readonly string folder
```

#### Field Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

## Methods

### <a id="Cube_Tests_AtomicFileTests_Dispose"></a> Dispose\(\)

Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.

```csharp
public void Dispose()
```

### <a id="Cube_Tests_AtomicFileTests_Replaces_existing_file_and_leaves_no_temporary_file"></a> Replaces\_existing\_file\_and\_leaves\_no\_temporary\_file\(\)

```csharp
[Fact]
public void Replaces_existing_file_and_leaves_no_temporary_file()
```

### <a id="Cube_Tests_AtomicFileTests_Writes_new_file"></a> Writes\_new\_file\(\)

```csharp
[Fact]
public void Writes_new_file()
```

### <a id="Cube_Tests_AtomicFileTests_Writes_utf8_without_a_byte_order_mark"></a> Writes\_utf8\_without\_a\_byte\_order\_mark\(\)

```csharp
[Fact]
public void Writes_utf8_without_a_byte_order_mark()
```
