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

 [BuildInfoTests](Cube.Tests.BuildInfoTests.md)

Checks the build line logged on load (CLAUDE.md: log which build is running and when it was built),
so a stale or wrong-configuration build is obvious.

 [CoreAssemblyTests](Cube.Tests.CoreAssemblyTests.md)

Checks that Core stays free of Dalamud, game structs and Lumina, so all its logic can be unit tested
without the game (CLAUDE.md, "Reference: the previous plugin" layout rule).

 [DocumentationCoverageTests](Cube.Tests.DocumentationCoverageTests.md)

Checks the documentation rules (CLAUDE.md, "Code is the documentation"): every test class has a
summary saying which rule it checks, and every namespace has an overview page for the generated docs. Test methods are named as behaviour and need no summary,
so the compiler doesn't enforce this for the test project; this test does.
