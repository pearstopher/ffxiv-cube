# <a id="Cube_Core_NameCache"></a> Class NameCache

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

Turns the game's UTF-8 name bytes into strings, once per name: a string is only built when the bytes in an
object slot change (PROJECT.md, "Efficiency and memory": name labels are cached and only rebuilt when a name
changes). Reading the same names every frame allocates nothing.

```csharp
public sealed class NameCache
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[NameCache](Cube.Core.NameCache.md)


## Remarks

One entry per object table slot, allocated up front, so memory is fixed (PROJECT.md: caches have fixed caps).
A slot reused by a different player simply has different bytes, so it can never show a stale name. Names
live only in memory and are never written anywhere (PROJECT.md, "Privacy").

## Constructors

### <a id="Cube_Core_NameCache__ctor_System_Int32_"></a> NameCache\(int\)

Creates an empty cache.

```csharp
public NameCache(int slots)
```

#### Parameters

`slots` [int](https://learn.microsoft.com/dotnet/api/system.int32)

How many object slots to cache, e.g. 200 character slots (plus any Debug fake players).

## Fields

### <a id="Cube_Core_NameCache_MaxNameBytes"></a> MaxNameBytes

The longest name the game stores, in bytes (FFXIVClientStructs: <code>GameObject.Name</code> is 64 bytes).

```csharp
public const int MaxNameBytes = 64
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_Core_NameCache_bytes"></a> bytes

The cached bytes per slot.

```csharp
private readonly byte[][] bytes
```

#### Field Value

 [byte](https://learn.microsoft.com/dotnet/api/system.byte)\[\]\[\]

### <a id="Cube_Core_NameCache_lengths"></a> lengths

How many of each slot's bytes are in use.

```csharp
private readonly int[] lengths
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)\[\]

### <a id="Cube_Core_NameCache_names"></a> names

The cached string per slot (null until first seen).

```csharp
private readonly string?[] names
```

#### Field Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)?\[\]

## Properties

### <a id="Cube_Core_NameCache_Builds"></a> Builds

How many strings have been built so far; a test and the bridge use it to prove names aren't rebuilt every frame.

```csharp
public long Builds { get; private set; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Cube_Core_NameCache_Slots"></a> Slots

How many slots the cache holds.

```csharp
public int Slots { get; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Methods

### <a id="Cube_Core_NameCache_Get_System_Int32_System_ReadOnlySpan_System_Byte__"></a> Get\(int, ReadOnlySpan<byte\>\)

Returns the name for a slot, building a new string only if the bytes differ from last time.

```csharp
public string Get(int slot, ReadOnlySpan<byte> utf8)
```

#### Parameters

`slot` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Object table index.

`utf8` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan\-1)<[byte](https://learn.microsoft.com/dotnet/api/system.byte)\>

The name bytes; anything from the first zero byte on is ignored (the game's buffer is zero-terminated).

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)
