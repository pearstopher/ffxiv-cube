# <a id="Cube_Tests_ViewOrientationTests_VectorComparer"></a> Class ViewOrientationTests.VectorComparer

Namespace: [Cube.Tests](Cube.Tests.md)  
Assembly: Cube.Tests.dll  

```csharp
private sealed class ViewOrientationTests.VectorComparer : IEqualityComparer<Vector3>, IEqualityComparer<Vector2>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ViewOrientationTests.VectorComparer](Cube.Tests.ViewOrientationTests.VectorComparer.md)

#### Implements

[IEqualityComparer<Vector3\>](https://learn.microsoft.com/dotnet/api/system.collections.generic.iequalitycomparer\-1), 
[IEqualityComparer<Vector2\>](https://learn.microsoft.com/dotnet/api/system.collections.generic.iequalitycomparer\-1)


## Constructors

### <a id="Cube_Tests_ViewOrientationTests_VectorComparer__ctor_System_Int32_"></a> VectorComparer\(int\)

```csharp
public VectorComparer(int digits)
```

#### Parameters

`digits` [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Fields

### <a id="Cube_Tests_ViewOrientationTests_VectorComparer_tolerance"></a> tolerance

```csharp
private readonly float tolerance
```

#### Field Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

## Methods

### <a id="Cube_Tests_ViewOrientationTests_VectorComparer_Equals_System_Numerics_Vector3_System_Numerics_Vector3_"></a> Equals\(Vector3, Vector3\)

Determines whether the specified objects are equal.

```csharp
public bool Equals(Vector3 x, Vector3 y)
```

#### Parameters

`x` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The first object of type <code class="paramref">T</code> to compare.

`y` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The second object of type <code class="paramref">T</code> to compare.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> if the specified objects are equal; otherwise, <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a>.

### <a id="Cube_Tests_ViewOrientationTests_VectorComparer_Equals_System_Numerics_Vector2_System_Numerics_Vector2_"></a> Equals\(Vector2, Vector2\)

Determines whether the specified objects are equal.

```csharp
public bool Equals(Vector2 x, Vector2 y)
```

#### Parameters

`x` [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

The first object of type <code class="paramref">T</code> to compare.

`y` [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

The second object of type <code class="paramref">T</code> to compare.

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

<a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">true</a> if the specified objects are equal; otherwise, <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool">false</a>.

### <a id="Cube_Tests_ViewOrientationTests_VectorComparer_GetHashCode_System_Numerics_Vector3_"></a> GetHashCode\(Vector3\)

Returns a hash code for the specified object.

```csharp
public int GetHashCode(Vector3 obj)
```

#### Parameters

`obj` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The `System.Object` for which a hash code is to be returned.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

A hash code for the specified object.

#### Exceptions

 [ArgumentNullException](https://learn.microsoft.com/dotnet/api/system.argumentnullexception)

The type of <code class="paramref">obj</code> is a reference type and <code class="paramref">obj</code> is <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>.

### <a id="Cube_Tests_ViewOrientationTests_VectorComparer_GetHashCode_System_Numerics_Vector2_"></a> GetHashCode\(Vector2\)

Returns a hash code for the specified object.

```csharp
public int GetHashCode(Vector2 obj)
```

#### Parameters

`obj` [Vector2](https://learn.microsoft.com/dotnet/api/system.numerics.vector2)

The `System.Object` for which a hash code is to be returned.

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

A hash code for the specified object.

#### Exceptions

 [ArgumentNullException](https://learn.microsoft.com/dotnet/api/system.argumentnullexception)

The type of <code class="paramref">obj</code> is a reference type and <code class="paramref">obj</code> is <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>.
