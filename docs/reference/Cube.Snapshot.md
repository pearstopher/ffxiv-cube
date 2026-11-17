# <a id="Cube_Snapshot"></a> Namespace Cube.Snapshot

**The snapshot tool (dev only).** PROJECT.md, "Automated testing": a small console program turns a draw list dumped by the in-game test bridge (`drawlist.json`) into a PNG, so a frame can be looked at without a screenshot of the game. `SnapshotRenderer` paints the dumped primitives in order onto a `Canvas` (convex fills and thick lines with alpha blending), and `PngWriter` encodes it with only the .NET base library. It is not shipped and the plugin doesn't reference it. Run it through `python scripts/ingame_test.py --snapshot`.

### Classes

 [Canvas](Cube.Snapshot.Canvas.md)

An RGBA image in memory that lines and filled polygons can be drawn onto, with alpha blending like ImGui's.
Just enough rasterising to see what the plugin drew; not a general graphics library.

 [PngWriter](Cube.Snapshot.PngWriter.md)

Writes a [Canvas](Cube.Snapshot.Canvas.md#Cube_Snapshot_Canvas) as a PNG file (8-bit RGBA, no filtering), using only the .NET base library so
the tool needs no image package.

 [SnapshotRenderer](Cube.Snapshot.SnapshotRenderer.md)

Renders a draw-list dump (<code>drawlist.json</code>, written by the bridge's <code>frame-snapshot</code> check) onto a
[Canvas](Cube.Snapshot.Canvas.md#Cube_Snapshot_Canvas), in the dump's order: the list is already sorted far to near, so painting in order
reproduces what ImGui drew.
