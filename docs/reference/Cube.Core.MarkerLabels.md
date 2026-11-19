# <a id="Cube_Core_MarkerLabels"></a> Class MarkerLabels

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

Builds the label text with its markers (#111): <code>[T]</code> in front of the player's current target, <code>[!]</code> in
front of an enemy that is attacking the player, both as <code>[T] [!] Name</code> with the target marker first.

```csharp
public sealed class MarkerLabels
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[MarkerLabels](Cube.Core.MarkerLabels.md)


## Remarks

A prefixed string is only built when a slot's name or markers change, and is then reused every frame (PROJECT.md,
"Efficiency and memory": no per-frame allocation). With no markers the name itself is returned, so the common case
costs nothing. One entry per object slot, allocated up front, like [NameCache](Cube.Core.NameCache.md#Cube_Core_NameCache).

## Constructors

### <a id="Cube_Core_MarkerLabels__ctor_System_Int32_"></a> MarkerLabels\(int\)

Creates an empty cache.

```csharp
public MarkerLabels(int slots)
```

#### Parameters

`slots` [int](https://learn.microsoft.com/dotnet/api/system.int32)

How many object slots to cache; matches [Slots](Cube.Core.NameCache.md#Cube_Core_NameCache_Slots).

## Fields

### <a id="Cube_Core_MarkerLabels_AttackingMarker"></a> AttackingMarker

Prefix for an enemy attacking the player.

```csharp
public const string AttackingMarker = "[!] "
```

#### Field Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Cube_Core_MarkerLabels_TargetMarker"></a> TargetMarker

Prefix for the current target.

```csharp
public const string TargetMarker = "[T] "
```

#### Field Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Cube_Core_MarkerLabels_flags"></a> flags

The markers each slot's label was last built with.

```csharp
private readonly byte[] flags
```

#### Field Value

 [byte](https://learn.microsoft.com/dotnet/api/system.byte)\[\]

### <a id="Cube_Core_MarkerLabels_labels"></a> labels

The built label per slot.

```csharp
private readonly string?[] labels
```

#### Field Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)?\[\]

### <a id="Cube_Core_MarkerLabels_names"></a> names

The name each slot's label was last built from (compared by reference: names are cached strings).

```csharp
private readonly string?[] names
```

#### Field Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)?\[\]

## Properties

### <a id="Cube_Core_MarkerLabels_Builds"></a> Builds

How many prefixed strings have been built; a test uses it to prove they aren't rebuilt every frame.

```csharp
public long Builds { get; private set; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

## Methods

### <a id="Cube_Core_MarkerLabels_Compose_System_Boolean_System_Boolean_System_String_"></a> Compose\(bool, bool, string\)

The label text for a name and its markers.

```csharp
public static string Compose(bool isTarget, bool isAttacking, string name)
```

#### Parameters

`isTarget` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether this is the player's current target (any kind of object).

`isAttacking` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether this is an enemy attacking the player.

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)

The cached name.

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

The text with its markers.

### <a id="Cube_Core_MarkerLabels_Get_System_Int32_System_Boolean_System_Boolean_System_String_"></a> Get\(int, bool, bool, string\)

Returns the label for a slot, rebuilding it only if the name or markers changed.

```csharp
public string Get(int slot, bool isTarget, bool isAttacking, string name)
```

#### Parameters

`slot` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Object table index (below the slot count).

`isTarget` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether this is the player's current target.

`isAttacking` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether this is an enemy attacking the player.

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)

The cached name for the slot.

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

The label text.
