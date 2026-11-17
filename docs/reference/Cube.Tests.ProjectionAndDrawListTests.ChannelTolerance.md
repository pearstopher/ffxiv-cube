# <a id="Cube_Tests_ProjectionAndDrawListTests_ChannelTolerance"></a> Class ProjectionAndDrawListTests.ChannelTolerance

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

Compares packed colours allowing one step of rounding per channel.

```csharp
private sealed class ProjectionAndDrawListTests.ChannelTolerance : IEqualityComparer<uint>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ProjectionAndDrawListTests.ChannelTolerance](Cube.Tests.ProjectionAndDrawListTests.ChannelTolerance.md)

#### Implements

[IEqualityComparer<uint\>](https://learn.microsoft.com/dotnet/api/system.collections.generic.iequalitycomparer\-1)


## Methods

### <a id="Cube_Tests_ProjectionAndDrawListTests_ChannelTolerance_Equals_System_UInt32_System_UInt32_"></a> Equals\(uint, uint\)

Determines whether the specified objects are equal.

```csharp
public bool Equals(uint x, uint y)
```

#### Parameters

`x` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

The first object of type <code class="paramref">T</code> to compare.

`y` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

The second object of type <code class="paramref">T</code> to compare.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> if the specified objects are equal; otherwise, <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a>.

### <a id="Cube_Tests_ProjectionAndDrawListTests_ChannelTolerance_GetHashCode_System_UInt32_"></a> GetHashCode\(uint\)

Returns a hash code for the specified object.

```csharp
public int GetHashCode(uint obj)
```

#### Parameters

`obj` [uint](https://learn.microsoft.com/dotnet/api/system.uint32)

The `System.Object` for which a hash code is to be returned.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

A hash code for the specified object.

#### Exceptions

 [ArgumentNullException](https://learn.microsoft.com/dotnet/api/system.argumentnullexception)

The type of <code class="paramref">obj</code> is a reference type and <code class="paramref">obj</code> is <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>.
