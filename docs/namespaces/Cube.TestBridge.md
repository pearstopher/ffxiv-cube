---
uid: Cube.TestBridge
summary: *content
---

**The in-game test bridge (Debug builds only).** PROJECT.md, "Automated testing": every feature is checked in the running game with no human in game. `BridgeRunner` runs every registered `BridgeCheck` on the framework thread when the plugin loads and whenever the test script drops a request file, then writes `results.json` (stamped with the build time) to `pluginConfigs\Cube\test-bridge\`. Checks are iterators that yield one step per frame, so multi-frame checks read top to bottom. The whole `TestBridge` folder is excluded from Release builds in `Cube.csproj`, and a unit test reads the Release DLL to prove it. Design notes: `docs/research-notes.md` (#7).
