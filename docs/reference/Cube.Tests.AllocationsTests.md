# <a id="Cube_Tests_AllocationsTests"></a> Class AllocationsTests

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

Checks the [Allocations](Cube.Tests.Allocations.md#Cube_Tests_Allocations) helper itself, which enforces PROJECT.md's
"zero bytes allocated per frame" budget in unit tests: it must pass allocation-free code
and catch code that allocates, including small amortised growth.

```csharp
public class AllocationsTests
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[AllocationsTests](Cube.Tests.AllocationsTests.md)


## Fields

### <a id="Cube_Tests_AllocationsTests_buffer"></a> buffer

```csharp
private readonly int[] buffer
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)\[\]

### <a id="Cube_Tests_AllocationsTests_sink"></a> sink

```csharp
private object? sink
```

#### Field Value

 [object](https://learn.microsoft.com/dotnet/api/system.object)?

## Methods

### <a id="Cube_Tests_AllocationsTests_Boxing_is_caught"></a> Boxing\_is\_caught\(\)

```csharp
[Fact]
public void Boxing_is_caught()
```

### <a id="Cube_Tests_AllocationsTests_Code_that_allocates_fails"></a> Code\_that\_allocates\_fails\(\)

```csharp
[Fact]
public void Code_that_allocates_fails()
```

### <a id="Cube_Tests_AllocationsTests_Code_that_does_not_allocate_passes"></a> Code\_that\_does\_not\_allocate\_passes\(\)

```csharp
[Fact]
public void Code_that_does_not_allocate_passes()
```

### <a id="Cube_Tests_AllocationsTests_Growth_on_a_later_call_is_caught"></a> Growth\_on\_a\_later\_call\_is\_caught\(\)

```csharp
[Fact]
public void Growth_on_a_later_call_is_caught()
```

### <a id="Cube_Tests_AllocationsTests_One_time_warm_up_allocations_are_not_counted"></a> One\_time\_warm\_up\_allocations\_are\_not\_counted\(\)

```csharp
[Fact]
public void One_time_warm_up_allocations_are_not_counted()
```
