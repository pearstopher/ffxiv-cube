# Code reference

Generated from the code's XML doc comments by `scripts/build-docs.py`. Don't edit these files;
improve the comments in the code instead.

## [Cube.Core](Cube.Core.md)

**All the logic, with no Dalamud, game or Lumina references,** so it can be unit tested without launching the game (enforced by `CoreAssemblyTests`). This is where the cube's geometry, projection and drawing order live. Per-frame code here must not allocate (PROJECT.md, "Efficiency and memory").

## [Cube.Game](Cube.Game.md)

**Reading the game.** Small, read-only readers that turn game memory into plain values for `Cube.Core`: the camera's direction now, nearby players later. They only read memory through FFXIVClientStructs on the game's main thread; they hook nothing, call no game functions that change anything, and allocate nothing per frame (PROJECT.md, "Read-only game access" and "Efficiency and memory"). Where each value comes from, and why, is in `docs/research-notes.md` (#10).

## [Cube](Cube.md)

**The plugin: everything that touches Dalamud or the game.** It is kept as thin as possible: it reads game state, hands it to `Cube.Core` and draws the result. Entry point: `Plugin`. The cube is drawn in `CubeWindow`, a Dalamud window toggled by `/cube`. Game access is read-only (PROJECT.md, "Goal"), and every handler is released when the plugin unloads.

## [Cube.Snapshot](Cube.Snapshot.md)

**The snapshot tool (dev only).** PROJECT.md, "Automated testing": a small console program turns a draw list dumped by the in-game test bridge (`drawlist.json`) into a PNG, so a frame can be looked at without a screenshot of the game. `SnapshotRenderer` paints the dumped primitives in order onto a `Canvas` (convex fills and thick lines with alpha blending), and `PngWriter` encodes it with only the .NET base library. It is not shipped and the plugin doesn't reference it. Run it through `python scripts/ingame_test.py --snapshot`.

## [Cube.TestBridge.Checks](Cube.TestBridge.Checks.md)

**The in-game checks (Debug builds only).** One class per behaviour checked in the running game, each naming the PROJECT.md rule it verifies: the loaded build is the one on disk, the window opens and closes, and the draw path stays within the frame-time and zero-allocation budgets. Features that touch the game add their checks here and register them in `Plugin`.

## [Cube.TestBridge](Cube.TestBridge.md)

**The in-game test bridge (Debug builds only).** PROJECT.md, "Automated testing": every feature is checked in the running game with no human in game. `BridgeRunner` runs every registered `BridgeCheck` on the framework thread when the plugin loads and whenever the test script drops a request file, then writes `results.json` (stamped with the build time) to `pluginConfigs\Cube\test-bridge\`. Checks are iterators that yield one step per frame, so multi-frame checks read top to bottom. The whole `TestBridge` folder is excluded from Release builds in `Cube.csproj`, and a unit test reads the Release DLL to prove it. Design notes: `docs/research-notes.md` (#7).

## [Cube.Tests](Cube.Tests.md)

**The unit tests, which double as a readable spec.** Test methods are named as behaviour, and each test class's summary names the PROJECT.md or CLAUDE.md rule it checks. Things only the running game can show are checked by the in-game test bridge instead.
