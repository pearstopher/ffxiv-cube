# <a id="Cube_TestBridge_Checks_CollisionPhantomsCheck_Result"></a> Class CollisionPhantomsCheck.Result

Namespace: [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)  
Assembly: Cube.dll  

What [Examine](Cube.TestBridge.Checks.CollisionPhantomsCheck.md#Cube_TestBridge_Checks_CollisionPhantomsCheck_Examine) found.

```csharp
private sealed class CollisionPhantomsCheck.Result
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[CollisionPhantomsCheck.Result](Cube.TestBridge.Checks.CollisionPhantomsCheck.Result.md)


## Fields

### <a id="Cube_TestBridge_Checks_CollisionPhantomsCheck_Result_Area"></a> Area

The region examined around the player.

```csharp
public CollisionBox Area
```

#### Field Value

 [CollisionBox](Cube.Core.CollisionBox.md)

### <a id="Cube_TestBridge_Checks_CollisionPhantomsCheck_Result_Centre"></a> Centre

The player's position.

```csharp
public Vector3 Centre
```

#### Field Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_TestBridge_Checks_CollisionPhantomsCheck_Result_ColliderGroups"></a> ColliderGroups

Colliders (not boxes) per group: how many had any box hit.

```csharp
public Dictionary<string, (int Colliders, int Missed)> ColliderGroups
```

#### Field Value

 [Dictionary](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary\-2)<[string](https://learn.microsoft.com/dotnet/api/system.string), \([int](https://learn.microsoft.com/dotnet/api/system.int32) [Colliders](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.int32,system.int32\-.colliders), [int](https://learn.microsoft.com/dotnet/api/system.int32) [Missed](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.int32,system.int32\-.missed)\)\>

### <a id="Cube_TestBridge_Checks_CollisionPhantomsCheck_Result_ColliderHit"></a> ColliderHit

Whether any box of the collider being tested was hit.

```csharp
public bool ColliderHit
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_TestBridge_Checks_CollisionPhantomsCheck_Result_Error"></a> Error

Why nothing could be tested, or <a href="https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/null">null</a>.

```csharp
public string? Error
```

#### Field Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)?

### <a id="Cube_TestBridge_Checks_CollisionPhantomsCheck_Result_Groups"></a> Groups

Boxes and phantoms per group of collider fields.

```csharp
public Dictionary<string, (int Boxes, int Phantoms)> Groups
```

#### Field Value

 [Dictionary](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary\-2)<[string](https://learn.microsoft.com/dotnet/api/system.string), \([int](https://learn.microsoft.com/dotnet/api/system.int32) [Boxes](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.int32,system.int32\-.boxes), [int](https://learn.microsoft.com/dotnet/api/system.int32) [Phantoms](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.int32,system.int32\-.phantoms)\)\>

### <a id="Cube_TestBridge_Checks_CollisionPhantomsCheck_Result_Lines"></a> Lines

Log lines to write.

```csharp
public List<string> Lines
```

#### Field Value

 [List](https://learn.microsoft.com/dotnet/api/system.collections.generic.list\-1)<[string](https://learn.microsoft.com/dotnet/api/system.string)\>

### <a id="Cube_TestBridge_Checks_CollisionPhantomsCheck_Result_PhantomList"></a> PhantomList

Every phantom with its distance from the player, for the nearest-first listing.

```csharp
public List<(float Distance, string Text)> PhantomList
```

#### Field Value

 [List](https://learn.microsoft.com/dotnet/api/system.collections.generic.list\-1)<\([float](https://learn.microsoft.com/dotnet/api/system.single) [Distance](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.single,system.string\-.distance), [string](https://learn.microsoft.com/dotnet/api/system.string) [Text](https://learn.microsoft.com/dotnet/api/system.valuetuple\-system.single,system.string\-.text)\)\>

### <a id="Cube_TestBridge_Checks_CollisionPhantomsCheck_Result_Phantoms"></a> Phantoms

Boxes none of whose rays hit inside them.

```csharp
public int Phantoms
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionPhantomsCheck_Result_PhantomsDropped"></a> PhantomsDropped

Phantoms on colliders the reader's filter skips.

```csharp
public int PhantomsDropped
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionPhantomsCheck_Result_PhantomsKept"></a> PhantomsKept

Phantoms on colliders the reader keeps.

```csharp
public int PhantomsKept
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionPhantomsCheck_Result_PrimitivePhantomsKept"></a> PrimitivePhantomsKept

Phantoms among primitive colliders the reader keeps: what the filter must leave at zero.

```csharp
public int PrimitivePhantomsKept
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_Checks_CollisionPhantomsCheck_Result_Tested"></a> Tested

Boxes tested.

```csharp
public int Tested
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Methods

### <a id="Cube_TestBridge_Checks_CollisionPhantomsCheck_Result_CountCollider_FFXIVClientStructs_FFXIV_Common_Component_BGCollision_Collider__System_Boolean_System_String_"></a> CountCollider\(Collider\*, bool, string\)

Counts the collider just tested into its group.

```csharp
public void CountCollider(Collider* collider, bool kept, string what)
```

#### Parameters

`collider` Collider\*

The collider.

`kept` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether the reader's filter keeps it.

`what` [string](https://learn.microsoft.com/dotnet/api/system.string)

The kind of collider.
