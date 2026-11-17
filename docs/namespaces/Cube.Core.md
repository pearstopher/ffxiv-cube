---
uid: Cube.Core
summary: *content
---

**All the logic, with no Dalamud, game or Lumina references,** so it can be unit tested without launching the game (enforced by `CoreAssemblyTests`). This is where the cube's geometry, projection and drawing order live. Per-frame code here must not allocate (PROJECT.md, "Efficiency and memory").
