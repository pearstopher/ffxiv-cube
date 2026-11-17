---
uid: Cube.Game
summary: *content
---

**Reading the game.** Small, read-only readers that turn game memory into plain values for `Cube.Core`: the camera's direction now, nearby players later. They only read memory through FFXIVClientStructs on the game's main thread; they hook nothing, call no game functions that change anything, and allocate nothing per frame (PROJECT.md, "Read-only game access" and "Efficiency and memory"). Where each value comes from, and why, is in `docs/research-notes.md` (#10).
