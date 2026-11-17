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

 [BuildInfoTests](Cube.Tests.BuildInfoTests.md)

Checks the build line logged on load (CLAUDE.md: log which build is running and when it was built),
so a stale or wrong-configuration build is obvious.

 [ProjectionAndDrawListTests.ChannelTolerance](Cube.Tests.ProjectionAndDrawListTests.ChannelTolerance.md)

Compares packed colours allowing one step of rounding per channel.

 [CoreAssemblyTests](Cube.Tests.CoreAssemblyTests.md)

Checks that Core stays free of Dalamud, game structs and Lumina, so all its logic can be unit tested
without the game (CLAUDE.md, "Reference: the previous plugin" layout rule).

 [CubeSpaceTests](Cube.Tests.CubeSpaceTests.md)

Checks the cube of space (PROJECT.md, "Exact behaviour rules": Cube, View, Players): axis-aligned, centred on
the player, edge 10–100 yalms (default 40); players outside are not shown at all; containment is inclusive on
faces; the view turns by yaw (north up: east right, north forward). Also the zero-allocation rule
("Efficiency and memory").

 [DocumentationCoverageTests](Cube.Tests.DocumentationCoverageTests.md)

Checks the documentation rules (CLAUDE.md, "Code is the documentation"): every test class has a
summary saying which rule it checks, and every namespace has an overview page for the generated docs. Test methods are named as behaviour and need no summary,
so the compiler doesn't enforce this for the test project; this test does.

 [DrawCostSamplerTests](Cube.Tests.DrawCostSamplerTests.md)

Checks the draw-cost measurements the in-game bridge uses for PROJECT.md's budgets
("Efficiency and memory": under 0.5 ms per frame on average, zero bytes allocated per frame),
and that measuring is itself allocation-free with bounded memory.

 [PlayerCubesTests](Cube.Tests.PlayerCubesTests.md)

Checks players are drawn as small coloured cubes (PROJECT.md, "Players": about 1/40 of the big cube's edge, minimum
a few pixels; "Colours": one colour each for self, party/alliance and others; "Draw order": near cubes cover far
ones; "Efficiency and memory": no per-frame allocations).

 [PlayerSetTests](Cube.Tests.PlayerSetTests.md)

Checks which players are kept and how they're classified and named (PROJECT.md, "Players": only players inside
the cube are shown; "Colours": self, party/alliance, everyone else; "Efficiency and memory": name labels cached
and only rebuilt when a name changes, no per-frame allocations, bounded memory; "Privacy": names stay in memory).

 [ProjectionAndDrawListTests](Cube.Tests.ProjectionAndDrawListTests.md)

Checks projection and the draw list (PROJECT.md, "View": fixed-angle view from above and behind, default 30°;
"Draw order": far things first so near cubes cover far ones; "Cheap drawing": only visible faces, the big cube a
wireframe; "Efficiency and memory": reused buffers, no per-frame allocations).

 [ReleaseBuildTests](Cube.Tests.ReleaseBuildTests.md)

Checks the in-game test bridge is compiled out of Release builds (CLAUDE.md, "Building and testing":
the bridge and fake-player code must not ship), and is present in Debug builds where the in-game checks need it.

 [SnapshotTests](Cube.Tests.SnapshotTests.md)

Checks the dev-only snapshot renderer (PROJECT.md, "Automated testing": a dev tool turns a dumped draw list
into a PNG) paints what the dump says, in order, and writes a valid PNG.

 [TestPaths](Cube.Tests.TestPaths.md)

Locates files in the repository from a test's output folder, for tests that inspect the repo or build output.

 [ViewYawTests](Cube.Tests.ViewYawTests.md)

Checks the view's orientation modes (PROJECT.md, "View"): Camera mode follows the game camera's heading so
"forward on screen" matches the game view; North up is fixed with north at the back. The heading comes from
the camera's direction vector (docs/research-notes.md, #10).
