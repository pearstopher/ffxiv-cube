# <a id="Cube_TestBridge_BridgeRunner"></a> Class BridgeRunner

Namespace: [Cube.TestBridge](Cube.TestBridge.md)  
Assembly: Cube.dll  

The Debug-only in-game test bridge (PROJECT.md, "Automated testing"): runs every registered
[BridgeCheck](Cube.TestBridge.BridgeCheck.md#Cube_TestBridge_BridgeCheck) on the framework thread when the plugin loads and whenever a request file
appears, and writes the results, stamped with the build time, for the test script to read.

```csharp
internal sealed class BridgeRunner : IDisposable
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[BridgeRunner](Cube.TestBridge.BridgeRunner.md)

#### Implements

[IDisposable](https://learn.microsoft.com/dotnet/api/system.idisposable)


## Remarks

<p>
Files live in <code>pluginConfigs\Cube\test-bridge\</code> (see <code>docs/research-notes.md</code>, #7):
the script writes <code>request.json</code> (its text is echoed back as the request id), the bridge deletes it
when it starts and writes <code>results.json</code> atomically with [AtomicFile](Cube.Core.AtomicFile.md#Cube_Core_AtomicFile) when done.
</p>
<p>
When idle the per-frame cost is one bool read: a `System.IO.FileSystemWatcher` sets
[requestSeen](Cube.TestBridge.BridgeRunner.md#Cube_TestBridge_BridgeRunner_requestSeen) from its own thread, so nothing polls the disk and nothing is allocated.
Allocations during a run are fine: the draw-path budget is measured inside
[Draw](Cube.CubeWindow.md#Cube_CubeWindow_Draw) only, and the bridge runs in `Dalamud.Plugin.Services.IFramework.Update`.
</p>

## Constructors

### <a id="Cube_TestBridge_BridgeRunner__ctor_Dalamud_Plugin_Services_IFramework_Dalamud_Plugin_Services_IPluginLog_System_String_Cube_TestBridge_BridgeContext_"></a> BridgeRunner\(IFramework, IPluginLog, string, BridgeContext\)

Creates the bridge, its folder and the request watcher, and hooks the framework update.

```csharp
public BridgeRunner(IFramework framework, IPluginLog log, string configDirectory, BridgeContext context)
```

#### Parameters

`framework` [IFramework](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Plugin/Services/IFramework.cs)

Dalamud's framework service.

`log` [IPluginLog](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Plugin/Services/IPluginLog.cs)

The plugin log.

`configDirectory` [string](https://learn.microsoft.com/dotnet/api/system.string)

The plugin's config directory (<code>IDalamudPluginInterface.ConfigDirectory</code>).

`context` [BridgeContext](Cube.TestBridge.BridgeContext.md)

What checks can see.

## Fields

### <a id="Cube_TestBridge_BridgeRunner_RequestFileName"></a> RequestFileName

Name of the request file the script writes to ask for a run.

```csharp
public const string RequestFileName = "request.json"
```

#### Field Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Cube_TestBridge_BridgeRunner_ResultsFileName"></a> ResultsFileName

Name of the results file the bridge writes after each run.

```csharp
public const string ResultsFileName = "results.json"
```

#### Field Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Cube_TestBridge_BridgeRunner_SoakRequestPrefix"></a> SoakRequestPrefix

A request id starting with this asks for a soak run (<code>ingame_test.py --soak</code>).

```csharp
public const string SoakRequestPrefix = "soak-"
```

#### Field Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Cube_TestBridge_BridgeRunner_checkIndex"></a> checkIndex

Index of the check being run.

```csharp
private int checkIndex
```

#### Field Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Cube_TestBridge_BridgeRunner_checkStarted"></a> checkStarted

When the running check started, as a `System.Diagnostics.Stopwatch` timestamp.

```csharp
private long checkStarted
```

#### Field Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Cube_TestBridge_BridgeRunner_checks"></a> checks

Registered checks, run in registration order.

```csharp
private readonly List<BridgeCheck> checks
```

#### Field Value

 [List](https://learn.microsoft.com/dotnet/api/system.collections.generic.list\-1)<[BridgeCheck](Cube.TestBridge.BridgeCheck.md)\>

### <a id="Cube_TestBridge_BridgeRunner_context"></a> context

What checks can see.

```csharp
private readonly BridgeContext context
```

#### Field Value

 [BridgeContext](Cube.TestBridge.BridgeContext.md)

### <a id="Cube_TestBridge_BridgeRunner_folder"></a> folder

The bridge's folder inside the plugin's config directory.

```csharp
private readonly string folder
```

#### Field Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Cube_TestBridge_BridgeRunner_framework"></a> framework

The framework, whose update event drives the checks.

```csharp
private readonly IFramework framework
```

#### Field Value

 [IFramework](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Plugin/Services/IFramework.cs)

### <a id="Cube_TestBridge_BridgeRunner_log"></a> log

The plugin log; runs are summarised at Information, check details at Debug.

```csharp
private readonly IPluginLog log
```

#### Field Value

 [IPluginLog](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Plugin/Services/IPluginLog.cs)

### <a id="Cube_TestBridge_BridgeRunner_request"></a> request

Why the run started: <code>load</code>, or the request file's text.

```csharp
private string request
```

#### Field Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Cube_TestBridge_BridgeRunner_requestSeen"></a> requestSeen

Set by the watcher thread when a request file appears; read each frame.

```csharp
private volatile bool requestSeen
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_TestBridge_BridgeRunner_results"></a> results

Results of the run in progress.

```csharp
private readonly List<BridgeRunner.CheckResult> results
```

#### Field Value

 [List](https://learn.microsoft.com/dotnet/api/system.collections.generic.list\-1)<[BridgeRunner](Cube.TestBridge.BridgeRunner.md).[CheckResult](Cube.TestBridge.BridgeRunner.CheckResult.md)\>

### <a id="Cube_TestBridge_BridgeRunner_runOnLoad"></a> runOnLoad

True until the first run (the one on load) has started.

```csharp
private bool runOnLoad
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_TestBridge_BridgeRunner_runStarted"></a> runStarted

When the run started, local time.

```csharp
private DateTime runStarted
```

#### Field Value

 [DateTime](https://learn.microsoft.com/dotnet/api/system.datetime)

### <a id="Cube_TestBridge_BridgeRunner_running"></a> running

Whether a run is in progress.

```csharp
private bool running
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_TestBridge_BridgeRunner_steps"></a> steps

The running check's steps.

```csharp
private IEnumerator<CheckStep>? steps
```

#### Field Value

 [IEnumerator](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerator\-1)<[CheckStep](Cube.TestBridge.CheckStep.md)\>?

### <a id="Cube_TestBridge_BridgeRunner_watcher"></a> watcher

Watches [folder](Cube.TestBridge.BridgeRunner.md#Cube_TestBridge_BridgeRunner_folder) for the request file.

```csharp
private readonly FileSystemWatcher watcher
```

#### Field Value

 [FileSystemWatcher](https://learn.microsoft.com/dotnet/api/system.io.filesystemwatcher)

## Properties

### <a id="Cube_TestBridge_BridgeRunner_Folder"></a> Folder

The bridge's folder, where checks may write their own output (e.g. the frame dump).

```csharp
public string Folder { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Cube_TestBridge_BridgeRunner_IsSoakRun"></a> IsSoakRun

Whether this run is a soak run: the request id starts with [SoakRequestPrefix](Cube.TestBridge.BridgeRunner.md#Cube_TestBridge_BridgeRunner_SoakRequestPrefix). Soak runs take
minutes, so they run only the [SoakOnly](Cube.TestBridge.BridgeCheck.md#Cube_TestBridge_BridgeCheck_SoakOnly) checks, and normal runs skip those.

```csharp
private bool IsSoakRun { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Cube_TestBridge_BridgeRunner_RequestPath"></a> RequestPath

Full path of the request file.

```csharp
private string RequestPath { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

## Methods

### <a id="Cube_TestBridge_BridgeRunner_Dispose"></a> Dispose\(\)

Unhooks the framework and stops watching. A run in progress is abandoned after cleaning up its check.

```csharp
public void Dispose()
```

### <a id="Cube_TestBridge_BridgeRunner_EndCheck_Cube_TestBridge_CheckState_System_String_"></a> EndCheck\(CheckState, string\)

Records the current check's result, runs its cleanup, and moves on.

```csharp
private void EndCheck(CheckState state, string message)
```

#### Parameters

`state` [CheckState](Cube.TestBridge.CheckState.md)

Passed or failed.

`message` [string](https://learn.microsoft.com/dotnet/api/system.string)

What was seen.

### <a id="Cube_TestBridge_BridgeRunner_FinishRun"></a> FinishRun\(\)

Writes the results file and logs a one-line summary.

```csharp
private void FinishRun()
```

### <a id="Cube_TestBridge_BridgeRunner_NextCheck"></a> NextCheck\(\)

Moves to the next check that belongs in this kind of run, or finishes the run.

```csharp
private void NextCheck()
```

### <a id="Cube_TestBridge_BridgeRunner_OnRequestFile_System_Object_System_IO_FileSystemEventArgs_"></a> OnRequestFile\(object, FileSystemEventArgs\)

Watcher callback (thread pool): only sets a flag; the framework thread does the work.

```csharp
private void OnRequestFile(object sender, FileSystemEventArgs e)
```

#### Parameters

`sender` [object](https://learn.microsoft.com/dotnet/api/system.object)

The watcher.

`e` [FileSystemEventArgs](https://learn.microsoft.com/dotnet/api/system.io.filesystemeventargs)

Which file changed.

### <a id="Cube_TestBridge_BridgeRunner_OnUpdate_Dalamud_Plugin_Services_IFramework_"></a> OnUpdate\(IFramework\)

Runs once per frame on the framework thread. Idle: two bool reads. Running: advances the current check one step.
Never lets an exception reach the game (CLAUDE.md, "Dalamud rules").

```csharp
private void OnUpdate(IFramework fw)
```

#### Parameters

`fw` [IFramework](https://github.com/goatcorp/Dalamud/blob/master/Dalamud/Plugin/Services/IFramework.cs)

The framework.

### <a id="Cube_TestBridge_BridgeRunner_Register_Cube_TestBridge_BridgeCheck_"></a> Register\(BridgeCheck\)

Adds a check to every run. Call while setting the plugin up.

```csharp
public void Register(BridgeCheck check)
```

#### Parameters

`check` [BridgeCheck](Cube.TestBridge.BridgeCheck.md)

The check.

### <a id="Cube_TestBridge_BridgeRunner_ResultsJson_System_Boolean_"></a> ResultsJson\(bool\)

Builds the results document. The script matches <code>buildTime</code> against the DLL it built, and
<code>request</code> against the request it wrote, to know the results are fresh.

```csharp
private string ResultsJson(bool passed)
```

#### Parameters

`passed` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether every check passed.

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Cube_TestBridge_BridgeRunner_StartRun"></a> StartRun\(\)

Starts a run: reads and deletes the request, if any, and resets the results.

```csharp
private void StartRun()
```

### <a id="Cube_TestBridge_BridgeRunner_Step"></a> Step\(\)

Advances the current check one step, ending it on pass, fail, exception or timeout.

```csharp
private void Step()
```
