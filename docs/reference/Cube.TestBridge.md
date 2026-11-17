# <a id="Cube_TestBridge"></a> Namespace Cube.TestBridge

**The in-game test bridge (Debug builds only).** PROJECT.md, "Automated testing": every feature is checked in the running game with no human in game. `BridgeRunner` runs every registered `BridgeCheck` on the framework thread when the plugin loads and whenever the test script drops a request file, then writes `results.json` (stamped with the build time) to `pluginConfigs\Cube\test-bridge\`. Checks are iterators that yield one step per frame, so multi-frame checks read top to bottom. The whole `TestBridge` folder is excluded from Release builds in `Cube.csproj`, and a unit test reads the Release DLL to prove it. Design notes: `docs/research-notes.md` (#7).

### Namespaces

 [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)

### Classes

 [BridgeCheck](Cube.TestBridge.BridgeCheck.md)

A named in-game check, registered with [Register](Cube.TestBridge.BridgeRunner.md#Cube_TestBridge_BridgeRunner_Register). Each feature that touches
the game adds checks for its behaviour (CLAUDE.md, "Building and testing"), so the running game
verifies it with no human in game.

 [BridgeContext](Cube.TestBridge.BridgeContext.md)

What an in-game check can see: the plugin's parts and its build stamp. Also collects the numbers a
check measured, which go into the results file next to its pass/fail.

 [BridgeRunner](Cube.TestBridge.BridgeRunner.md)

The Debug-only in-game test bridge (PROJECT.md, "Automated testing"): runs every registered
[BridgeCheck](Cube.TestBridge.BridgeCheck.md#Cube_TestBridge_BridgeCheck) on the framework thread when the plugin loads and whenever a request file
appears, and writes the results, stamped with the build time, for the test script to read.

 [BridgeRunner.CheckResult](Cube.TestBridge.BridgeRunner.CheckResult.md)

One check's outcome, as written to the results file.

### Structs

 [CheckStep](Cube.TestBridge.CheckStep.md)

One step of an in-game check. A check is written as an iterator that yields [NextFrame](Cube.TestBridge.CheckStep.md#Cube_TestBridge_CheckStep_NextFrame)
to wait a frame and ends with [Pass](Cube.TestBridge.CheckStep.md#Cube_TestBridge_CheckStep_Pass) or [Fail](Cube.TestBridge.CheckStep.md#Cube_TestBridge_CheckStep_Fail), so a check that needs
several frames (open the window, wait for it to draw, close it) reads top to bottom.

### Enums

 [CheckState](Cube.TestBridge.CheckState.md)

Whether an in-game check is still going, or how it ended.
