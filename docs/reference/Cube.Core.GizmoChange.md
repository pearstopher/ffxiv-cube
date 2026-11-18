# <a id="Cube_Core_GizmoChange"></a> Struct GizmoChange

Namespace: [Cube.Core](Cube.Core.md)  
Assembly: Cube.Core.dll  

What a gizmo drag changed ([Read](Cube.Core.GizmoModel.md#Cube_Core_GizmoModel_Read)).

```csharp
public readonly record struct GizmoChange : IEquatable<GizmoChange>
```

#### Implements

[IEquatable<GizmoChange\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)


## Constructors

### <a id="Cube_Core_GizmoChange__ctor_Cube_Core_ViewOrientation_System_Numerics_Vector3_System_Boolean_System_Boolean_"></a> GizmoChange\(ViewOrientation, Vector3, bool, bool\)

What a gizmo drag changed ([Read](Cube.Core.GizmoModel.md#Cube_Core_GizmoModel_Read)).

```csharp
public GizmoChange(ViewOrientation View, Vector3 Size, bool ViewChanged, bool SizeChanged)
```

#### Parameters

`View` [ViewOrientation](Cube.Core.ViewOrientation.md)

The view after the drag; the old one if it didn't turn.

`Size` [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

The cube size after the drag, clamped; the old one if it wasn't resized.

`ViewChanged` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether the drag turned the view.

`SizeChanged` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether the drag resized or zoomed the cube.

## Properties

### <a id="Cube_Core_GizmoChange_Size"></a> Size

The cube size after the drag, clamped; the old one if it wasn't resized.

```csharp
public Vector3 Size { get; init; }
```

#### Property Value

 [Vector3](https://learn.microsoft.com/dotnet/api/system.numerics.vector3)

### <a id="Cube_Core_GizmoChange_SizeChanged"></a> SizeChanged

Whether the drag resized or zoomed the cube.

```csharp
public bool SizeChanged { get; init; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_Core_GizmoChange_View"></a> View

The view after the drag; the old one if it didn't turn.

```csharp
public ViewOrientation View { get; init; }
```

#### Property Value

 [ViewOrientation](Cube.Core.ViewOrientation.md)

### <a id="Cube_Core_GizmoChange_ViewChanged"></a> ViewChanged

Whether the drag turned the view.

```csharp
public bool ViewChanged { get; init; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)
