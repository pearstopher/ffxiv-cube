---
uid: Cube.Snapshot
summary: *content
---

**The snapshot tool (dev only).** PROJECT.md, "Automated testing": a small console program turns a draw list dumped by the in-game test bridge (`drawlist.json`) into a PNG, so a frame can be looked at without a screenshot of the game. `SnapshotRenderer` paints the dumped primitives in order onto a `Canvas` (convex fills and thick lines with alpha blending), and `PngWriter` encodes it with only the .NET base library. It is not shipped and the plugin doesn't reference it. Run it through `python scripts/ingame_test.py --snapshot`.
