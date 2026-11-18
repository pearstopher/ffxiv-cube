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

 [FakePlayersCheck](Cube.TestBridge.Checks.FakePlayersCheck.md)

Checks fake players reach the cube like real ones (PROJECT.md, "Automated testing": synthetic players so rendering
checks work with nobody nearby; "Players": inside shown, outside not at all): with the standard fakes enabled, every
fake inside the cube is in the window's player set with its relation, and the one outside isn't.

 [FrameSnapshotCheck](Cube.TestBridge.Checks.FrameSnapshotCheck.md)

Dumps one frame's draw list to <code>drawlist.json</code> in the bridge folder (PROJECT.md, "Automated testing": the
bridge dumps the frame's draw list; the snapshot tool turns it into a PNG so Claude can see what was drawn).

 [GameStateProbeCheck](Cube.TestBridge.Checks.GameStateProbeCheck.md)

Research probe for #10 (reading players, party and camera): reads the same facts two ways and records
both, so one in-game run settles the open questions in <code>docs/research-notes.md</code>.

 [HiddenStatesCheck](Cube.TestBridge.Checks.HiddenStatesCheck.md)

Checks the cube is hidden when logged out, during loading screens, cutscenes and GPose, and comes back
afterwards (PROJECT.md, "When hidden": the window is just empty then, never an error; "Efficiency and memory": no
work and no allocations while there's nothing to draw).

 [LabelsCheck](Cube.TestBridge.Checks.LabelsCheck.md)

Checks every other player in the cube gets exactly one name label showing their name, next to their cube and on
top of it, and the local player gets none (PROJECT.md, "Features": other players are labelled with their names;
"Labels": the name drawn next to the cube). Uses the standard fake players (#16) so it works with nobody around.

 [NoErrorsCaughtCheck](Cube.TestBridge.Checks.NoErrorsCaughtCheck.md)

Checks no game-facing handler has caught an exception since the plugin loaded (CLAUDE.md, "Dalamud rules": wrap
every handler in try/catch and log). Runs after the other checks, so it also covers everything they exercised.

 [OrientationCheck](Cube.TestBridge.Checks.OrientationCheck.md)

Checks both orientation modes against the live game camera (PROJECT.md, "View"): in Camera mode the view's yaw
matches the camera's heading, in North up mode it is 0 whatever the camera does.

 [PlayerCubesCheck](Cube.TestBridge.Checks.PlayerCubesCheck.md)

Checks every player in the cube is drawn as a cube in their relation's colour (PROJECT.md, "Players" and "Colours";
"Draw order": far first), using the standard fake players (#16) so it works with nobody around.

 [PlayersMatchObjectTableCheck](Cube.TestBridge.Checks.PlayersMatchObjectTableCheck.md)

Checks the players the window found match an independent read of Dalamud's object table (PROJECT.md,
"Automated testing": the players drawn match an independent read of the object table; "Players": every player
character inside the cube, nobody outside; "Colours": self, party/alliance, other).

 [SettingsCheck](Cube.TestBridge.Checks.SettingsCheck.md)

Checks each setting changes the drawn frame straight away, and that putting the settings back restores it
(PROJECT.md, "Features": settings for cube size, orientation mode, labels on/off, colours and opacity).

 [SettingsWindowCheck](Cube.TestBridge.Checks.SettingsWindowCheck.md)

Checks the settings window draws and the settings are saved in the expected file (PROJECT.md, "Features":
a settings window, <code>/cube config</code>; the acceptance rule that settings are saved on change).

 [SoakCheck](Cube.TestBridge.Checks.SoakCheck.md)

Soak: draws the worst case for ten minutes and checks nothing grows (PROJECT.md, "Efficiency and memory": zero
bytes allocated per frame in steady state, bounded memory, nothing grows with time spent in game). Runs only when
asked for with <code>ingame_test.py --soak</code>.

 [WindowOpenCloseCheck](Cube.TestBridge.Checks.WindowOpenCloseCheck.md)

Checks the cube window draws when opened and stops drawing when closed (PROJECT.md, "Features":
<code>/cube</code> toggles the window; "Efficiency": skip everything when the window is closed).
