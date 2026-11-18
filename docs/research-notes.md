# Research notes

Findings about outside systems (Dalamud, the game) that the code relies on. Each section names the issue it came from.

## In-game test bridge design (#7)

Checked 2026-11-17 against Dalamud's source (`Dalamud/Plugin/Internal/Types/LocalDevPlugin.cs`, `master`), then confirmed live the same day: the full bridge suite ran in game and the script returned exit 0 on a pass and exit 1 when a check was made to fail on purpose.

### Auto-reload timing and the build stamp

- With **automatic reloading** on, Dalamud puts a `FileSystemWatcher` on the dev plugin's DLL (`NotifyFilters.LastWrite`, filtered to that one file). Each change restarts a **500 ms** timer; the reload happens 500 ms after the *last* write, so the several writes of one `dotnet build` cause one reload.
- A reload is skipped if the plugin isn't in the `Loaded` (or a load/unload error) state, e.g. when it's disabled.
- So after `dotnet build` finishes, expect the new build to be loaded within about 1–2 seconds, plus however long the plugin's constructor takes. The script allows far more than that (see "Unreachable game").
- **Measured:** DLL written 12:38:45, `Cube unloaded` at 12:38:46.2, `Cube loaded` at 12:38:47.1, first bridge check passed at 12:38:47.2. About 2 seconds from build to a fresh run.
- **Which build is loaded:** the plugin reads its DLL's last write time fresh from disk on load (`BuildInfo.ReadBuildTime`, not Dalamud's cached `FileInfo`) and logs `Cube loaded (Debug build <version>, built <time>, from <path>)`. The bridge writes the same build time into its results file. The script compares it with the write time of `Cube\bin\x64\Debug\Cube.dll` it just built: equal means the results are from this build.

### Where requests and results live

- `IDalamudPluginInterface.ConfigDirectory` is `%APPDATA%\XIVLauncher\pluginConfigs\<InternalName>\`. The internal name is the assembly name, `Cube`, so on this PC:

  ```
  C:\Users\snows\AppData\Roaming\XIVLauncher\pluginConfigs\Cube\
  ```

- The bridge uses a `test-bridge\` subfolder:
  - `request.json`: written by the script to ask for a run without rebuilding. The bridge deletes it when it starts the run.
  - `results.json`: written by the bridge. Written to `results.json.tmp` first, then moved over the old file (`File.Move(..., overwrite: true)`), so the script never reads a half-written file.
  - `drawlist.json`: the last frame's draw list, for the snapshot tool (#14).
- The folder is outside the repo, so its output can never be committed.

### Running checks on the framework thread, and measuring

- Game state (object table, client state) must be read on the framework thread. `IFramework.Update` runs once per game frame on that thread; `UiBuilder.Draw` (where the window draws) also runs on the game's main thread, once per rendered frame.
- **Check runner:** checks are run as a small state machine advanced from `IFramework.Update`, so a check can span several frames (e.g. "open the window, wait 2 frames, check it drew"). No `async`/`Task` on the hot path.
- **Frame time:** `Stopwatch.GetTimestamp()` before and after the plugin's own draw path (the cube window's `Draw`), not the whole frame: the budget (PROJECT.md: under 0.5 ms average) is about our cost, and the game's frame time is noise.
- **Allocations:** `GC.GetAllocatedBytesForCurrentThread()` before and after the same draw path. It's per thread and exact, so other plugins and Dalamud's own work on other threads don't affect it. Samples go into a fixed-size array (e.g. 120 frames), so measuring allocates nothing itself. The check discards the first frames after the window opens (warm-up: first-use caches, JIT) and then requires **0 bytes** on every frame.
- **Detecting a request without allocating:** a `FileSystemWatcher` on `test-bridge\` sets a `volatile bool` from its own thread-pool thread; `IFramework.Update` only reads the flag. Polling `File.Exists` every frame would cost a system call each frame for nothing.

### Unreachable game

The script decides the game is unreachable, and exits with its own exit code (not the failure code), when:

- no `results.json` with the new build time appears within **3 minutes** of the build finishing (reload takes seconds; 3 minutes covers a slow load or a loading screen), or
- for a request-only run, the bridge hasn't deleted `request.json` within the same time.

It also prints the last `Cube loaded` / `Cube unloaded` line from `dalamud.log`, if any, so it's clear whether the plugin is loaded at all. CLAUDE.md: when unreachable, finish with unit tests and mark the in-game check as pending.

### Compiled out of Release

- All bridge and fake-player code lives under `Cube/TestBridge/` and is excluded from non-Debug builds in `Cube.csproj` (`<Compile Remove="TestBridge/**" Condition="'$(Configuration)' != 'Debug'" />`), and its hook-up in `Plugin` is inside `#if DEBUG`. Excluding the folder means a new bridge file can't be forgotten outside `#if DEBUG`.
- **Proof:** a unit test opens the plugin DLL built with the test's own configuration using `System.Reflection.Metadata` (reads type names without loading the assembly or Dalamud) and asserts: Release → no type in the `Cube.TestBridge` namespace; Debug → the bridge is there. CI tests the Release configuration, so the Release assertion runs on every push.

## Reading players, party and camera (#10)

Checked 2026-11-17 by reading MiniMappingway (`Service/FinderService.cs`, `Manager/NaviMapManager.cs`), Mappy (`MapRenderer.GameObject.cs`, `.Player.cs`, `.GroupMember.cs`), FFXIVClientStructs (`GameObjectManager`, `GameObject`, `Character`, `Camera`, `CameraBase`, `Graphics.Scene.Camera`) and Dalamud's `UiBuilder`. The bridge check `probe-players-party-camera` then measured the uncertain parts live (2026-11-17); results are noted under each point.

### Enumerating players without allocating

- **Both reference plugins use Dalamud's `IObjectTable`** (Mappy: `foreach (var obj in Service.ObjectTable.Reverse())` filtered by kind and distance; MiniMappingway: one index per frame, `ObjectTable[i]`, `ObjectKind.Pc`, `obj.Name.ToString()`). Those allocate: LINQ, `IGameObject` wrappers, a new name string per read. Fine for them, not for our zero-allocation budget.
- **Our hot path reads FFXIVClientStructs directly:** `GameObjectManager.Instance()->Objects.IndexSorted[i]` for `i` in 0–199. Per FFXIVClientStructs' notes, slots 0–199 are the networked characters (players and battle NPCs at even indexes, their mounts/minions at odd ones); other players are never above 199. Each non-null slot with `ObjectKind == Pc` is a player.
  - Position: `GameObject.Position` (`Vector3`, world space, yalms).
  - Name: `GameObject.Name`, a fixed 64-byte UTF-8 buffer. Compare the bytes with the cached label's bytes and only build a new `string` when they differ (PROJECT.md: labels cached, rebuilt only when a name changes).
  - Identity for caching: `GameObject.EntityId` (or the slot index; a slot can be reused by someone else, so the cache keys on both).
  - Local player: `IObjectTable.LocalPlayer` (one wrapper, read once per frame is acceptable only if it doesn't allocate; otherwise slot 0 of the object manager). **Chosen and confirmed live:** `Control.Instance()->LocalPlayer` (`PlayerReader`), a raw pointer with no wrapper; `players-match-object-table` checks it is the same entity as `IObjectTable.LocalPlayer`, and it passes. The direct read and Dalamud's object table agreed on the player count (1, then 2 when someone walked by).
- **World axes:** X east, Z south, Y up (the map uses X and Z: Mappy's `new Vector2(Position.X, Position.Z)`), so north is −Z. The cube is axis-aligned to these (PROJECT.md: north is a fixed face).
- **Independent read for the bridge:** the bridge counts players through Dalamud's `IObjectTable` (allocations are fine in checks) and compares with the direct read; the probe check does this now, and the player-drawing checks (#15, #17) will compare positions too.

### Party and alliance

- `Character.IsPartyMember` and `Character.IsAllianceMember` (bits of `Character.RelationFlags`) per character: what MiniMappingway uses, no lookup needed, no allocation. Chosen.
- Alternative (Mappy): `GroupManager.Instance()->MainGroup.IsEntityIdInParty(entityId)` / `IsEntityIdInAlliance`. Kept as a cross-check for the bridge.
- Self is identified first (the local player), so "self, party/alliance, everyone else" (PROJECT.md, "Colours") is: local player → self; else either flag set → party; else other.

### Camera yaw

- `CameraManager.Instance()->Camera` (`FFXIVClientStructs.FFXIV.Client.Game.Control`) is the normal in-game camera. `Camera.DirH` is its horizontal angle in radians; FFXIVClientStructs' comment says "0 is north, increases CW". Character `Rotation` uses 0 = facing south (+Z).
  - **Measured (one sample):** `cameraDirH` = 1.554 while the scene camera looked almost due west (forward XZ = (−7.05, +0.59)). So `DirH` is *not* the look direction under "0 north, CW" (west would be ±3π/2 / −π/2). It matches the bearing *of the camera from what it looks at* (east, π/2) to within about 4° (that bearing computed from the scene camera is 1.487). The small gap may be camera smoothing between the two structs. This ambiguity is why the scene camera vector below is the recommended source.
- Independent source: the scene camera, `Camera.SceneCamera` (`Graphics.Scene.Camera`): `Position` and `LookAtVector` are world-space points. `forward = LookAtVector - Position` projected onto XZ gives the view direction with no angle convention to guess. **Recommended for the Camera orientation mode:** compute the yaw from that vector, so the cube's "forward on screen" is exactly where the game camera looks. The probe records both (`cameraDirH`, `sceneYawAtan2XZ`) so the relation is known.
- Mappy's alternative (`AreaMapNumberArray.ConeRotation`, the minimap cone) is UI data and only valid while the map addon is updating; not used.

### When to hide

| Situation | How to detect | Notes |
| --- | --- | --- |
| Not logged in | `IClientState.IsLoggedIn` false, or no local player | Also covers the title screen and character select. |
| Loading screen | `ICondition[ConditionFlag.BetweenAreas]` or `[BetweenAreas51]` | MiniMappingway instead checks the `_LocationTitle`/`FadeMiddle` addons; the condition flags are simpler and documented. |
| Cutscene | `ICondition[OccupiedInCutSceneEvent]`, `[WatchingCutscene]`, `[WatchingCutscene78]` | |
| GPose | `IClientState.IsGPosing` | |
| PvP area (#69) | `IClientState.IsPvP` | True in Crystalline Conflict, Frontline, Rival Wings and the Wolves' Den. `IsPvPExcludingDen` would skip the lobby; hiding there too is simpler. |

Dalamud already hides plugin windows during cutscenes, GPose and when the user hides the game UI, **but only if the user's Dalamud settings say so** (`UiBuilder`: `ToggleUiHideDuringCutscenes`, `ToggleUiHideDuringGpose`, `ToggleUiHide`). PROJECT.md requires hiding regardless, so the plugin checks the conditions itself (#21) and draws nothing then; Dalamud's own hiding is a bonus.

### Read-only and within the rules

Everything above is a memory read on the framework thread: no hooks, no signatures of our own, no game functions called, nothing sent. Nearby players' positions and names are what the client already shows in game (and what Mappy and MiniMappingway draw on maps). Names are only drawn on screen (PROJECT.md, "Privacy"); the probe logs counts and the local player's position, never names.

## Named NPCs in the object table (#73)

Checked 2026-11-17 against FFXIVClientStructs (`GameObjectManager.ObjectArrays`, `GameObject`, `ObjectKind`, `BattleNpcSubKind`, from the dev hooks folder's XML docs), then measured live with the bridge probe `probe-players-party-camera`, which now also watches the whole object table for 4 seconds (`NpcProbe`) and logs a `Probe #73` line with counts only, never names.

### Live sample (one city sample, standing still)

| What | Value |
| --- | --- |
| Object table length (`Objects.IndexSorted`) | **819** |
| Frames sampled in 4 s | 66 (the game runs at ~16 fps in the background) |
| Event NPCs per frame, max | 71, at slots **489–608** |
| ...with an empty name | 25 (invisible event helpers; never drawn) |
| Named NPCs per frame (filter below), max | 46 in the table, **22 inside the largest cube** (100 yalms) |
| Battle NPCs | 0 (none in a city) |
| Other objects | minion (`Companion`) at slot 13, event objects at 611–628, aetherytes at 612–616 |
| Name changes | 0 across 46 distinct NPCs |

The probe runs on every bridge run, so the next run in a FATE or dungeon adds battle NPC numbers to `dalamud.log` (`Probe #73`) without any code change.

### Where NPCs live

- **Battle NPCs** (enemies, guards, pets, chocobo companions, Trust members) are networked characters: slots **0–199**, even indexes, sharing those 100 slots with players (FFXIVClientStructs' notes, the same range `PlayerReader` already walks). So players and battle NPCs together are at most 100.
- **Event NPCs** (townspeople, quest NPCs) are client-side objects above 199; measured at 489–608. The table is 819 long, so the reader walks the whole table once per frame (or 0–199 for battle NPCs plus 200–818 for event NPCs).
- Minions, mounts, event objects and aetherytes have other `ObjectKind`s and are skipped by kind.

### Filter rule for #74

An object is a **named NPC** when all of these hold:

1. `ObjectKind` is `EventNpc` or `BattleNpc`;
2. its name's first byte isn't 0 (drops the nameless event helpers measured above);
3. `OwnerId` is `0xE0000000` (no owner), and for battle NPCs `BattleNpcSubKind` is not `Pet` or `Buddy` (PROJECT.md, "NPCs": player-owned objects are left out). Checking both is cheap and covers either field being unset.

Not yet measured: whether `BNpcPart` (weak spots) carry a name (if not, rule 2 drops them), and whether Trust members (`NpcPartyMember`) have an owner (no dungeon sample yet). Either way the rule is the spec's: owned objects are left out, unowned named ones are shown.

### Cap and budget

- **Recommended NPC cap: 50**, filled in object table order (battle NPCs first, since they sit in the lower slots), with the overflow counted like `PlayerSet.Dropped`. The live sample needed 22 in the largest cube.
- **Why not more:** draw cost is about linear in drawn entries. At the player cap (101) `draw-within-budget` measures 0.22–0.29 ms; 151 entries is about 0.43 ms, inside the 0.5 ms budget. 200 entries would be about 0.55 ms, over it. #74's budget and soak checks run players and NPCs at their caps together, so the real number decides; lower the cap if they fail.
- **Name cache:** keyed by object table slot, so it needs one slot per table index: 819 (plus the Debug fake slots), fixed at load. At 64 bytes of name per slot that is about 52 KB.
- **Measured in #74** (Debug build, city, 101 players and 50 NPCs, all labelled): `draw-within-budget` 0.443 ms and 0.467 ms on two runs, 0 bytes. Inside the budget but with little room; if a later change pushes it over, lower `PlayerReader.MaxNpcs` first.

### Name changes

None seen. The name cache already compares the name bytes every frame and rebuilds only when they differ (PROJECT.md: labels rebuilt only when a name changes), so an NPC renamed mid-fight is handled without extra work.

### Read-only and within the rules

Same as players: a memory read of the object table on the framework thread, no hooks, no game functions, nothing sent. NPC names and positions are what the game already shows.

## Efficiency pass (#23)

Measured 2026-11-17 in game, Debug build, with the bridge (`draw-within-budget`, `soak-ten-minutes-at-the-cap`).

### Worst case: the player cap

- The game can have at most **100 players** around: one per even character slot (`PlayerReader.MaxPlayers`). The bridge's `FakePlayers.UseCrowd` fills the cube with 100 fakes (plus the real local player), every one with a cube and a 21-character label, about the longest name the game allows.
- At the cap: **101 players, 416 primitives, 0.23 ms average per frame, 0 bytes allocated** (120 measured frames after warm-up). The budget is 0.5 ms and 0 bytes. With the 7 standard fakes the same path costs about 0.04 ms, so the cost is roughly linear in players, and most of it is ImGui work for the labels and faces, which the spec requires.
- `draw-within-budget` now always measures at the cap, so a regression in the worst case fails every run.

### Soak: ten minutes at the cap

- **Result:** 28,835 frames over 10 minutes with 101 players: **0 bytes allocated** by the draw path, **0.216 ms** average, 0 name strings built after warm-up. Over the same 10 minutes the whole process's managed heap went from 1,704 MB to 1,729 MB with 724 gen-0 collections: that's Dalamud and the other plugins (see below).

- Run it with `python scripts/ingame_test.py --soak` (about 11 minutes). Soak-only checks run only for a request id starting `soak-`; normal runs skip them.
- **What "flat" means here.** The process's managed heap (`GC.GetTotalMemory`) is shared with Dalamud and every other plugin, so it is recorded (`heapStartBytes`, `heapEndBytes`, `gen0Collections`) but not judged. The plugin's own share can't grow: every buffer (draw list, player set, name cache, samplers) is allocated once at load with a fixed size, and the soak proves the draw path allocates nothing per frame.
- **Name strings.** A real player walking into view has their name string built once (PROJECT.md: labels are rebuilt only when a name changes). Those frames are counted separately (`CubeWindow.NameBuildFrames`) rather than against the zero-byte budget, in both `draw-within-budget` and the soak.

### Waste found and removed

- Nothing allocating was left in the draw path; the earlier issues had already removed it and the checks had held it at 0 bytes. This pass added the measurements that prove it at the worst case and over time.
- Release builds now size the name cache for the 200 real character slots only; the extra 100 slots for fake players exist in Debug builds only.

## ImGuizmo's view manipulator (#63)

Checked 2026-11-17 against `Dalamud.Bindings.ImGuizmo.dll` in the dev hooks folder (reflection over its public API) and ImGuizmo's C++ source (`CedricGuillemet/ImGuizmo`, `src/ImGuizmo.cpp`). The Dalamud.NET.Sdk references the binding automatically, so the plugin needs no extra package.

### The calls

- `ImGuizmo.SetDrawlist()`: draw into the current window's draw list. ImGuizmo also uses that window's name to decide whether the mouse is over "its" window.
- `ImGuizmo.SetRect(x, y, w, h)`: the area ImGuizmo's context maps to.
- `ImGuizmo.ViewManipulate(ref Matrix4x4 view, ref Matrix4x4 projection, ImGuizmoOperation, ImGuizmoMode, ref Matrix4x4 model, float length, Vector2 position, Vector2 size, uint backgroundColour)`: draws the x/y/z cube and edits `view` in place. There is also a short overload (`view, length, position, size, colour`).
- `BeginFrame()` isn't needed: it only opens a full-screen invisible window and sets its draw list, which `SetDrawlist()` replaces.
- The binding has no `IsUsingViewManipulate` or per-widget ID calls, so Dalamud ships an older ImGuizmo than current master.

### How it behaves

- `view` is a right-handed camera view matrix, the same layout as `Matrix4x4.CreateLookAt` (columns = camera right, up, back; translation in the last row). The widget orbits around the point `length` in front of the camera.
- Dragging rotates around world up (mouse X) and the camera's right axis (mouse Y), keeps world up as the up vector, and clamps at straight up/down: **it never rolls**. So yaw + pitch is enough to store the view (`ViewOrientation`).
- Clicking a face, edge or corner animates the view towards it over about 40 frames, editing `view` each frame. Looking straight down or up keeps a horizontal right vector, so the heading is still well defined.
- **Use the 9-argument overload.** The short overload reads mouse-over and (in newer versions) handedness from state set by the last `Manipulate` call; this plugin never calls `Manipulate`. The long one sets that state first from the given projection (a right-handed perspective) and model (identity).
- **Mouse.** It reads the mouse directly and only starts a drag when no ImGui item is active, so an invisible button over it would block it. Without one, ImGui would treat the drag as a drag on empty window space and move the window, so the cube window sets `NoMove` while the mouse is over the widget or a turn is in progress.
- It has no double-click handling, so "reset" is a button in the settings window.
- Measured with the bridge: drawing the widget every frame keeps `draw-within-budget` at 0 bytes allocated per frame.
