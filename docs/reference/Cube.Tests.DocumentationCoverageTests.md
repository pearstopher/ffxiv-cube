# <a id="Cube_Tests_DocumentationCoverageTests"></a> Class DocumentationCoverageTests

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

Checks the documentation rules (CLAUDE.md, "Code is the documentation"): every test class has a
summary saying which rule it checks, and every namespace has an overview page for the generated docs. Test methods are named as behaviour and need no summary,
so the compiler doesn't enforce this for the test project; this test does.

```csharp
public class DocumentationCoverageTests
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[DocumentationCoverageTests](Cube.Tests.DocumentationCoverageTests.md)


## Methods

### <a id="Cube_Tests_DocumentationCoverageTests_Every_namespace_has_an_overview_page"></a> Every\_namespace\_has\_an\_overview\_page\(\)

```csharp
[Fact]
public void Every_namespace_has_an_overview_page()
```

### <a id="Cube_Tests_DocumentationCoverageTests_Every_test_class_has_a_summary"></a> Every\_test\_class\_has\_a\_summary\(\)

```csharp
[Fact]
public void Every_test_class_has_a_summary()
```
