# <a id="Cube_TestBridge_Checks"></a> Namespace Cube.TestBridge.Checks

**The in-game checks (Debug builds only).** One class per behaviour checked in the running game, each naming the PROJECT.md rule it verifies: the loaded build is the one on disk, the window opens and closes, and the draw path stays within the frame-time and zero-allocation budgets. Features that touch the game add their checks here and register them in `Plugin`.

### Classes

 [BuildStampCheck](Cube.TestBridge.Checks.BuildStampCheck.md)

Checks the plugin that ran the checks is the build on disk: the build time read at load equals the
DLL's current write time. If not, the game is running a stale build and every other result is
about old code (CLAUDE.md: the startup build line makes "my change didn't take effect" obvious).

 [DrawBudgetCheck](Cube.TestBridge.Checks.DrawBudgetCheck.md)

Checks the draw path stays within PROJECT.md's phase 1 budgets ("Efficiency and memory"):
under 0.5 ms per frame on average and zero bytes allocated per frame in steady state.
A budget failure is a failed test, not a note (CLAUDE.md).

 [DrawListMatchesCoreCheck](Cube.TestBridge.Checks.DrawListMatchesCoreCheck.md)

Checks the window drew exactly what Core produced (PROJECT.md, "Automated testing": the primitives drawn
match Core's output): rebuilds the last frame independently from the same cube space and projection,
compares it primitive by primitive, and checks one ImGui call was issued per primitive.

 [FrameSnapshotCheck](Cube.TestBridge.Checks.FrameSnapshotCheck.md)

Dumps one frame's draw list to <code>drawlist.json</code> in the bridge folder (PROJECT.md, "Automated testing": the
bridge dumps the frame's draw list; the snapshot tool turns it into a PNG so Claude can see what was drawn).

 [GameStateProbeCheck](Cube.TestBridge.Checks.GameStateProbeCheck.md)

Research probe for #10 (reading players, party and camera): reads the same facts two ways and records
both, so one in-game run settles the open questions in <code>docs/research-notes.md</code>.

 [PlayersMatchObjectTableCheck](Cube.TestBridge.Checks.PlayersMatchObjectTableCheck.md)

Checks the players the window found match an independent read of Dalamud's object table (PROJECT.md,
"Automated testing": the players drawn match an independent read of the object table; "Players": every player
character inside the cube, nobody outside; "Colours": self, party/alliance, other).

 [WindowOpenCloseCheck](Cube.TestBridge.Checks.WindowOpenCloseCheck.md)

Checks the cube window draws when opened and stops drawing when closed (PROJECT.md, "Features":
<code>/cube</code> toggles the window; "Efficiency": skip everything when the window is closed).
