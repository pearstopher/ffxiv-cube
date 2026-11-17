# Code reference

Generated from the code's XML doc comments by `scripts/build-docs.py`. Don't edit these files;
improve the comments in the code instead.

## [Cube.Core](Cube.Core.md)

**All the logic, with no Dalamud, game or Lumina references,** so it can be unit tested without launching the game (enforced by `CoreAssemblyTests`). This is where the cube's geometry, projection and drawing order live. Per-frame code here must not allocate (PROJECT.md, "Efficiency and memory").

## [Cube](Cube.md)

**The plugin: everything that touches Dalamud or the game.** It is kept as thin as possible: it reads game state, hands it to `Cube.Core` and draws the result. Entry point: `Plugin`. The cube is drawn in `CubeWindow`, a Dalamud window toggled by `/cube`. Game access is read-only (PROJECT.md, "Goal"), and every handler is released when the plugin unloads.

## [Cube.Tests](Cube.Tests.md)

**The unit tests, which double as a readable spec.** Test methods are named as behaviour, and each test class's summary names the PROJECT.md or CLAUDE.md rule it checks. Things only the running game can show are checked by the in-game test bridge instead.
