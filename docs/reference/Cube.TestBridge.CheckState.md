# <a id="Cube_TestBridge_CheckState"></a> Enum CheckState

Namespace: [Cube.TestBridge](Cube.TestBridge.md)  
Assembly: Cube.dll  

Whether an in-game check is still going, or how it ended.

```csharp
internal enum CheckState
```

## Fields

`Running = 0` 

Not finished: run the check again next frame.



`Passed = 1` 

The behaviour matched the spec.



`Failed = 2` 

The behaviour didn't match, or the check couldn't run.
