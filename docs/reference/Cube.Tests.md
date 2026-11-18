# <a id="Cube_Tests"></a> Namespace Cube.Tests

**The unit tests, which double as a readable spec.** Test methods are named as behaviour, and each test class's summary names the PROJECT.md or CLAUDE.md rule it checks. Things only the running game can show are checked by the in-game test bridge instead.

### Classes

 [Allocations](Cube.Tests.Allocations.md)

Test helper for the "zero bytes allocated per frame" budget (PROJECT.md, "Efficiency and memory"):
hot-path Core code is run under [AssertNone](Cube.Tests.Allocations.md#Cube_Tests_Allocations_AssertNone) so an accidental allocation
(LINQ, a capturing lambda, boxing, string building) fails a unit test long before it shows up in game.

 [AllocationsTests](Cube.Tests.AllocationsTests.md)

Checks the [Allocations](Cube.Tests.Allocations.md#Cube_Tests_Allocations) helper itself, which enforces PROJECT.md's
"zero bytes allocated per frame" budget in unit tests: it must pass allocation-free code
and catch code that allocates, including small amortised growth.

 [AtomicFileTests](Cube.Tests.AtomicFileTests.md)

Checks the in-game bridge's results are written atomically (PROJECT.md, "Automated testing":
the script polls the results file, so it must never see half a file).

 [BoxExtentsTests](Cube.Tests.BoxExtentsTests.md)

Checks the box size rules (PROJECT.md, "Cube" and "Zoom", #90): each extent 10–100, default 40; zoom scales all
three together keeping their ratios, clamped so no extent leaves 10–100; zooming in means less space.

 [BuildInfoTests](Cube.Tests.BuildInfoTests.md)

Checks the build line logged on load (CLAUDE.md: log which build is running and when it was built),
so a stale or wrong-configuration build is obvious.

 [ProjectionAndDrawListTests.ChannelTolerance](Cube.Tests.ProjectionAndDrawListTests.ChannelTolerance.md)

Compares packed colours allowing one step of rounding per channel.

 [ClippedCollisionTests](Cube.Tests.ClippedCollisionTests.md)

Checks the gathered collision is clipped to the cube for drawing, and only when something changed (PROJECT.md,
"Collision (phase 2)": boxes clipped to the cube; "Efficiency and memory": recompute only when the inputs change,
no per-frame allocations, collision data dropped on a zone change).

 [ClippedPrismTests](Cube.Tests.ClippedPrismTests.md)

Checks collision stays inside the cube at every cube yaw (#93; PROJECT.md, "Collision (phase 2)": each collider is
shown as a simplified box, clipped to the cube; "Cube": in "Rotate contents with camera" the contents turn with
the camera). A box cut by a side of a turned cube is kept as the prism the cut leaves, never as a world-aligned
box that pokes out of the cube; "Efficiency and memory": clipping and drawing allocate nothing.

 [CollisionBoxTests](Cube.Tests.CollisionBoxTests.md)

Checks the collision box model (PROJECT.md, "Collision (phase 2)": colliders inside or crossing the cube are shown
as simplified boxes clipped to the cube; stale data is refreshed when the player moves; "Efficiency and memory":
bounded cache with a hard cap, collision data dropped on a zone change, no per-frame allocations). The box source
(one box per mesh node or primitive collider through its world matrix) is the #28 recommendation in
<code>docs/research-notes.md</code>.

 [CollisionCacheTests](Cube.Tests.CollisionCacheTests.md)

Checks the collision reader's gathering rules (PROJECT.md, "Collision (phase 2)": expensive gathering is spread
over frames with a time budget and cached, areas not yet processed are simply empty, stale data is refreshed when
the player moves; "Efficiency and memory": bounded memory, collision data dropped on a zone change, no per-frame
allocations). The game side is replaced by a fake scene whose colliders each cost a set number of clock ticks.

 [CollisionCubesTests](Cube.Tests.CollisionCubesTests.md)

Checks collision boxes are drawn as solid boxes sorted with the players (PROJECT.md, "Collision (phase 2)": each
collider inside or crossing the cube is shown as a simplified box; "Draw order": far things are drawn first, so
near ones cover far ones; "Cheap drawing": only the visible faces; "Efficiency and memory": no per-frame
allocations).

 [CollisionDetailsTests](Cube.Tests.CollisionDetailsTests.md)

Checks the collision detail levels (#32, collision detail setting; PROJECT.md, "Collision (phase 2)": simplified
boxes): Full walks the whole node tree, Medium stops a few levels down, Coarse keeps one box per collider, and every
level is its own stamp so a change gathers again at once.

 [CoreAssemblyTests](Cube.Tests.CoreAssemblyTests.md)

Checks that Core stays free of Dalamud, game structs and Lumina, so all its logic can be unit tested
without the game (CLAUDE.md, "Reference: the previous plugin" layout rule).

 [CubeSettingsTests](Cube.Tests.CubeSettingsTests.md)

Checks the settings' defaults and repair rules (PROJECT.md, "Features": settings for cube size, orientation mode,
labels on/off, colours and opacity; "NPCs": on by default, purple; "Cube": width, height and depth each 10–100, default 40, zoom keeping their ratios, old single-edge files migrating (#90); "View": Camera
mode by default; #32: collision on, full detail by default; saved files from older versions still load) and the
<code>/cube config</code> command.

 [CubeSpaceTests](Cube.Tests.CubeSpaceTests.md)

Checks the cube of space (PROJECT.md, "Exact behaviour rules": Cube, Players, Culling): centred on the player, a box
whose width, height and depth are each 10–100 yalms (default 40), inside meaning inside on each axis separately,
aligned by its yaw (Camera mode: front face where the camera looks; North up: east right,
north forward); containment uses the aligned cube, so things enter and leave at the sides as the camera turns, and
is inclusive on faces; players outside are not shown at all. Also the zero-allocation rule ("Efficiency and
memory").

 [DocumentationCoverageTests](Cube.Tests.DocumentationCoverageTests.md)

Checks the documentation rules (CLAUDE.md, "Code is the documentation"): every test class has a
summary saying which rule it checks, and every namespace has an overview page for the generated docs. Test methods are named as behaviour and need no summary,
so the compiler doesn't enforce this for the test project; this test does.

 [DrawCostSamplerTests](Cube.Tests.DrawCostSamplerTests.md)

Checks the draw-cost measurements the in-game bridge uses for PROJECT.md's budgets
("Efficiency and memory": under 0.5 ms per frame on average, zero bytes allocated per frame),
and that measuring is itself allocation-free with bounded memory.

 [ErrorThrottleTests](Cube.Tests.ErrorThrottleTests.md)

Checks repeated errors are logged sparingly (CLAUDE.md, "Dalamud rules": every game-facing handler catches and
logs; a handler failing every frame must not flood the log).

 [GizmoModelTests](Cube.Tests.GizmoModelTests.md)

Checks the corner gizmo's shared model matrix (PROJECT.md, "View orientation control"; #90 "Control"): the axis
handles change width, height or depth in the cube's own axes whatever the orientation, the centre handle zooms
keeping the ratios, a turn never changes the size and a resize never changes the view. ImGuizmo's handles are
simulated by scaling the matrix's rows (its local axes) the way its scale operation does.

 [PlayerCubesTests](Cube.Tests.PlayerCubesTests.md)

Checks players are drawn as small coloured cubes (PROJECT.md, "Players": about 1/40 of the big cube's largest extent, minimum
a few pixels; "Colours": one colour each for self, party/alliance, others and NPCs; "NPCs": drawn like players, in
the NPC colour; "Draw order": near cubes cover far ones; "Efficiency and memory": no per-frame allocations).

 [PlayerHeightTests](Cube.Tests.PlayerHeightTests.md)

Checks "Player height in cube" (PROJECT.md, "Exact behaviour rules", Cube; #91): a fraction p of the box's height,
0–1, default 0.5; the box spans <code>player.y - height × p</code> to <code>player.y + height × (1 - p)</code> with x and z
centred on the player; culling uses the shifted box; on screen the box stays centred, so the player's marker moves
off the middle; the setting is saved, older configs load as 50%, and reset to defaults restores it. Also the
zero-allocation rule ("Efficiency and memory").

 [PlayerLabelsTests](Cube.Tests.PlayerLabelsTests.md)

Checks name labels (PROJECT.md, "Features": other players are labelled with their names, labels on/off;
"Labels": the name drawn next to the cube, overlaps are fine; "Draw order": near cubes cover far ones;
"Efficiency and memory": labels cached, no string building per frame).

 [PlayerSetTests](Cube.Tests.PlayerSetTests.md)

Checks which players and NPCs are kept and how they're classified and named (PROJECT.md, "Players": only players
inside the cube are shown; "NPCs": named, unowned NPCs inside the cube are shown, with an on/off setting; "Colours":
self, party/alliance, everyone else, NPCs; "Efficiency and memory": name labels cached
and only rebuilt when a name changes, no per-frame allocations, bounded memory; "Privacy": names stay in memory).

 [PolygonAndOutlineTests](Cube.Tests.PolygonAndOutlineTests.md)

Checks the draw list's polygons and the cube's screen outline (#93; PROJECT.md, "Cheap drawing": one ImGui call per
face; "Efficiency and memory": fixed buffers, nothing allocated per frame, bounded memory; "Collision (phase 2)":
collision clipped to the cube, which the outline is used to check).

 [ProjectionAndDrawListTests](Cube.Tests.ProjectionAndDrawListTests.md)

Checks projection and the draw list (PROJECT.md, "View": fixed-angle view from above and behind, default 30°;
"Draw order": far things first so near cubes cover far ones; "Cheap drawing": only visible faces, the big cube a
wireframe; "Cube on screen never turns by itself": the camera turns the contents, never the wireframe;
"Efficiency and memory": reused buffers, no per-frame allocations).

 [ReleaseBuildTests](Cube.Tests.ReleaseBuildTests.md)

Checks the in-game test bridge is compiled out of Release builds (CLAUDE.md, "Building and testing":
the bridge and fake-player code must not ship), and is present in Debug builds where the in-game checks need it.

 [ReleaseVersionTests](Cube.Tests.ReleaseVersionTests.md)

Checks the plugin version in <code>Cube/Cube.csproj</code> is releasable (CLAUDE.md, "CI and releases"): it must be
<code>MAJOR.MINOR.PATCH.0</code> so the release workflow can match it to a <code>vMAJOR.MINOR.PATCH</code> tag, and it must
never be older than the version <code>repo.json</code> already publishes, or Dalamud would not offer the next release
as an update.

 [SnapshotTests](Cube.Tests.SnapshotTests.md)

Checks the dev-only snapshot renderer (PROJECT.md, "Automated testing": a dev tool turns a dumped draw list
into a PNG) paints what the dump says, in order, and writes a valid PNG.

 [TestPaths](Cube.Tests.TestPaths.md)

Locates files in the repository from a test's output folder, for tests that inspect the repo or build output.

 [ViewOrientationTests.VectorComparer](Cube.Tests.ViewOrientationTests.VectorComparer.md)

 [ViewOrientationTests](Cube.Tests.ViewOrientationTests.md)

Checks the user's view orientation and the projection it drives (PROJECT.md, "View orientation control": the view
control is a rotate gizmo that turns how the cube is shown, any orientation including roll; default from above and
behind at 30°, front face away; orientations saved by the old yaw-and-tilt control carry over; "Efficiency and
memory": no per-frame allocations).

 [ViewSettingsTests](Cube.Tests.ViewSettingsTests.md)

Checks the saved view orientation and the view control's placement (PROJECT.md, "View orientation control": a
small control in a corner of the window, bottom right by default; the chosen orientation is saved; there's a way
to reset to the default).

 [ViewYawTests](Cube.Tests.ViewYawTests.md)

Checks the view's orientation modes (PROJECT.md, "View"): Camera mode follows the game camera's heading so
"forward on screen" matches the game view; North up is fixed with north at the back. The heading comes from
the camera's direction vector (docs/research-notes.md, #10).

 [VisibilityTests](Cube.Tests.VisibilityTests.md)

Checks when the cube is hidden (PROJECT.md, "When hidden": not logged in, during loading screens, cutscenes,
GPose and in PvP areas; shown otherwise).

### Structs

 [CollisionCacheTests.FakeSource](Cube.Tests.CollisionCacheTests.FakeSource.md)

A scene of fixed boxes, one per collider, whose clock advances a set amount per collider offered.

 [ClippedCollisionTests.ListSource](Cube.Tests.ClippedCollisionTests.ListSource.md)

A fake collision scene: one box per collider, gathered in one frame.
