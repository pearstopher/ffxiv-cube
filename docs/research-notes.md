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

## Telling the player is jumping (#105)

### Source

`ICondition[ConditionFlag.Jumping]` and `[ConditionFlag.Jumping61]` (both exist in Dalamud's `ConditionFlag`). A jump counts if either is set, as the cutscene and loading flags do above. Both are plain flag reads on the framework thread: read-only, no hooks, nothing sent, nothing automated (the plugin never makes the character jump).

### What is not yet known

Which flag the game raises, whether it is set for the whole airborne time or only the rise, and whether it lags the height by a frame. The plugin copes with a late flag by taking the previous frame's height as the take-off height. A Debug build logs `Jump trace: Jumping=… Jumping61=… y=…` every frame while either flag is set and for 0.5 s after. The owner's manual session (`docs/manual-tests.md`, "Jump") fills this in; **findings pending**: stand-still jump, running jump, jump onto and off a ledge, walking off a ledge.

### Bridge

The bridge can't jump, so it simulates one: `CubeWindow.SimulatedJump` and `SimulatedPlayerY` replace the flag and the player's height. That checks everything the plugin does with the flag, but not the flag itself.

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

## ImGuizmo's rotate gizmo (#79)

The owner found the view manipulator above awkward, so #79 replaced it with `ImGuizmo.Manipulate(ref view, ref projection, ImGuizmoOperation.Rotate, ImGuizmoMode.Local, ref model)` (the 5-argument overload compiles against the binding). From ImGuizmo's source:

- `Manipulate` sets up its own context (mouse over the draw list's window, matrices) every call, unlike `ViewManipulate`'s short overload.
- It edits `model` in place while a ring is dragged. In local mode the turn is applied in the model's own frame (`delta * model`), so the x, y and z rings stay around the cube's own axes. The outer ring turns around the camera's axis, which is how roll happens.
- **Size.** The outer ring's radius is a fixed 6% of the rectangle's height (`screenRotateSize`), and the axis rings are about the same size at the default gizmo size (0.1 in clip space). The gizmo size and orthographic mode are global to the ImGuizmo library, which every plugin shares, so the plugin doesn't change them; it hands `SetRect` a rectangle 7 times the corner square, centred on it, instead. ImGuizmo doesn't clip to the rectangle.
- The plugin keeps a fixed camera (+Z, looking at the centre) and hands the cube's rotation as the model matrix, with Z mirrored because ImGuizmo's world is right-handed (`ViewOrientation.ToGizmoModel`). `Matrix4x4.Decompose` removes any scale or drift when reading it back.
- The binding has `IsOver()` and `IsUsing()`, but the `NoMove` handling from #63 already covers the window-drag problem, so they aren't needed.

## Where to get collision data (#28)

Checked 2026-11-17 against FFXIVClientStructs' `BGCollision` sources (`BGCollisionModule`, `SceneManager`, `Scene`, `Collider*`, `MeshPCB`) and vnavmesh's `Debug/DebugGameCollision.cs` and `SceneExtractor.cs`, then measured live with the bridge probe `probe-collision` (`CollisionProbeCheck`), which logs a `Probe #28` line on every bridge run.

### What the game has loaded

- `Framework.Instance()->BGCollisionModule->SceneManager->Scenes` is a list that always holds one scene. `Scene->Colliders` is every collider the client has loaded: what the game's own movement and camera raycasts test against. Collision is streamed in around the camera (`SceneManager.StreamingSphere`, radius **160** yalms in the sample), which covers the largest cube (100 yalms, so 87 yalms to a corner).
- **Mesh colliders** (`ColliderMesh`) are most of it: placed objects and terrain tiles. Each has a world matrix (`World`), a world box (`WorldBoundingBox`) and a `MeshPCB` whose `RootNode` is a binary tree of nodes. Each node has a local box (`LocalBounds`) and up to 256 vertices and its triangles.
- **Primitive colliders** (`ColliderBox`, `ColliderCylinder`, `ColliderSphere`, `ColliderPlane`) are unit shapes (-1..1) put in place by their `World` matrix.
- **Streamed colliders** (`ColliderStreamed`) only add and remove terrain mesh colliders, which then appear in the same list. So the plugin never needs to read the streamed collider's own fields (vnavmesh notes those moved by 8 bytes in 7.4).
- `VisibilityFlags & 1` marks colliders the raycasts use; `LayerMask` bit 0 is the layer FFXIVClientStructs' raycast wrapper (and so movement) queries.

### Live sample (housing interior, standing still, 33 frames)

| What | Value |
| --- | --- |
| Colliders in the scene / crossing the largest cube | 300 / 300 (271 mesh, 29 box) |
| ...not raycast-active / not on layer 0 | 11 / 4 |
| Collider world box, largest side | mean 5.4, max 30 yalms |
| Mesh nodes with triangles crossing the cube | **402** (largest side mean 7.3, max 24.3), 15,038 triangles, 533 nodes visited |
| Same at the default cube (40 yalms) | 300 colliders, 402 nodes (a house interior fits in either) |
| **Collider walk** (all colliders, world boxes, test against cube) | **0.031 ms, 0 bytes** |
| **Node walk** (collider walk plus each crossing mesh's node tree, skipping subtrees outside the cube) | **0.127 ms, 0 bytes** |
| **Triangle voxel pass** (every triangle's bounds into 2-yalm cells, 50³ grid) | **2.4 ms, 0 bytes** |
| 400 downward raycasts (`RaycastMaterialFilter`, 5-yalm grid) | 0.65 ms (about 1.6 µs a ray) |
| Cells filled by node boxes / touched by triangles | 1,500 (1.2%) / 1,166 (0.9%) |
| Ray hits inside a node box / a triangle cell | 20/20 / 20/20 (380 rays missed: outside the house there's nothing) |

Every raycast hit lies in a node box and a triangle cell: the scene walk sees the same geometry the game's own raycast does.

### The candidates

1. **Collider boxes** (one per collider): cheapest, but a mesh collider can be a whole building or a terrain tile, and its box would fill the space you walk through. Too coarse alone; useful as the first cull.
2. **Mesh node boxes** (one per tree node holding triangles): a memory read, 0.13 ms for everything in the largest cube, no allocations. In the sample they fill only about 30% more space than the triangles themselves, so they follow the walls and floors closely. **Recommended.**
3. **Raycasts**: 1.6 µs a ray is cheap, but a ray only finds the first surface along one line. Sampling a cube with them needs thousands of rays (a 2-yalm grid in three directions is about 7,500 rays, about 12 ms) and still misses thin and overhanging things. They are good as a cross-check (as the probe uses them), not as the source.
4. **Voxels from triangles** (the spec's fallback): the most faithful shape, but 2.4 ms for a full pass in a small interior, so it must be spread over frames and cached. Keep it in reserve for where node boxes turn out too coarse (likely large terrain nodes outdoors).

### Recommendation for #29–#32

- **Read the collision scene directly.** Walk `Scene->Colliders`; take mesh colliders through their node tree (node `LocalBounds` through `World` to a world box), and primitive colliders through their `World` matrix. Skip colliders without `VisibilityFlags & 1` (the game ignores them too).
- **Cull first:** collider world box against the cube, then each node's box, skipping whole subtrees outside it. That is what keeps the walk to 533 nodes.
- **Cache and refresh only on change** (PROJECT.md, "Efficiency and memory"): rebuild the box list when the player has moved a set fraction of the cube, the collider count or first collider pointer changes, or the zone changes; drop it on zone change. The 0.13 ms walk fits in one frame, so no frame spreading is needed for boxes; a triangle voxel pass would need it.
- **Bounded cache:** a fixed array of boxes (24 bytes each). The sample needed 402 + 29; a cap of about 2,000 (48 KB) leaves room for busier places, with overflow counted like the player cap. Confirm with an outdoor sample (below).
- **Draw order and clipping** are #31's: boxes are clipped to the cube (PROJECT.md, "Collision").

### Still to measure

- **Outdoors and in a city.** The sample is a small house interior with no terrain. The probe runs on every bridge run, so the next run outdoors logs a `Probe #28` line with terrain numbers (node count, node box size, walk time) without any code change. If terrain node boxes come out much larger than the walls (say a largest side over 10 yalms on average), switch terrain meshes to the voxel pass, spread over frames.
- **Thread timing.** The collision module updates under its own lock (`UpdateTaskLock`, exclusive during its update task). The probe reads it on the framework thread, as vnavmesh's debug view does, and saw no errors; #30 keeps the read in the framework update, wrapped in `try`/`catch`, and its soak check should show it stays clean.
- **vnavmesh's debug view.** vnavmesh is installed, but its view is visual. The comparison is automated here against the game's own raycast instead (above). A side-by-side look belongs with #31, once there are boxes to look at; it goes in `docs/manual-tests.md` there.

### Read-only and within the rules

Everything above is a memory read of the client's own collision scene on the framework thread, plus calls to read-only getters (`GetColliderType`) and the game's raycast (which takes the module's shared lock itself). Nothing is written, nothing is hooked (vnavmesh hooks the raycast in its debug view; this plugin doesn't), nothing is sent. No dependency on vnavmesh: its source was only read.

## Phase 2 verification (#33)

Measured 2026-11-18 in game, Debug build, standing still in the housing interior from #28, with the bridge (`draw-within-budget`, `collision-zone-changes`, `soak-ten-minutes-at-the-cap`). The budget and soak checks now run the worst case whatever the user chose (`WorstCase`): 100 fake players and 50 fake NPCs, all labelled, and collision on at full detail in the largest cube (100 yalms).

### Frame budgets with collision on

| What | Value |
| --- | --- |
| `draw-within-budget`: 101 players, 50 NPCs, 358 collision boxes, 120 frames | **0.376 ms** and **0.404 ms** average on two runs, **0 bytes** (budget 0.5 ms, 0 bytes) |
| `collision-reader`: 10 forced gathers over 20 frames | average 0.12 ms, max 0.22 ms per gathering frame, 0 bytes (limit 1 ms) |
| Soak, 10 min: 6,644 drawn frames (the game was in the background, about 11 fps) | **0.383 ms** average, **0 bytes**, 0 name strings built |
| Soak gathering: 19 gathers over 38 frames | 0.122 ms average per gathering frame, **0 bytes** |

The draw cost with all three caps (0.38–0.40 ms) is in the same range as #74's players-and-NPCs measurement without collision (0.44–0.47 ms in a city): run-to-run noise from what else the game is doing is as large as the collision boxes' share. Inside the budget, but with little room: if a later change pushes it over, lower `PlayerReader.MaxNpcs` first (as #74 said), then the box cap.

### Memory across zone changes

- No human is in game to teleport, so the bridge simulates zone changes: `CollisionReader.SimulatedZone` (Debug only) replaces the game's zone id. The cache sees what it sees after a teleport: a new zone id, so it drops both box sets and gathers afresh. Only the loaded scene stays the same.
- `collision-zone-changes` (every run): 11 changes (10 simulated, then back to the real zone). Each was followed by a gather of the same 358 boxes into **one of the same two sets** (capacity 2,000), so a new zone never allocates new buffers. 0 bytes allocated gathering or drawing.
- Soak: a zone change every 30 seconds, **19 changes, all 19 gathered again**, 0 bytes. The process heap went from 1,444 MB to 1,453 MB with 498 gen-0 collections over the 10 minutes; as in #23 that is Dalamud and the other plugins (the plugin's own draw and gather paths allocated nothing), so it is recorded, not judged.
- `CollisionCache.ZoneChanges` counts real zone changes too, so a run after real teleports shows them in the metrics.

### Still to measure

- **Outdoors and in a city** (carried over from #28): every number above is from the same housing interior. The probe and these checks run on every bridge run, so the first run outdoors records terrain numbers without a code change.

## Drawing the real collision triangles (#99)

Measured 2026-11-18 in game with the Debug-only bridge check `probe-triangles` (`TriangleProbeCheck`, logs a `Probe #99` line and records every number below as a metric). It reads triangles the way #29–#30 read boxes (collider walk, node tree cull, then each triangle through the mesh's world matrix), clips them to a box turned 30° (#93: clipping in box space), deduplicates shared edges, projects and depth-sorts them, then issues real ImGui calls on the cube window's own draw list through `CubeWindow.ProbeDraw`. Read-only as before: memory reads only, no hooks, nothing sent.

### The sample

One place only: a closed area with the player at about (7, 0.5, 0), **563 colliders, 1,047 leaf nodes, 95,786 triangles** (95,630 from meshes, 600 from primitives turned into triangles: box 12, plane 2, 8-sided cylinder 32, icosahedron sphere 20). All of it lies inside the 100-yalm cube and nearly all inside the 40-yalm one (96,230 kept near the cube; at 40 yalms 86,024 wholly inside, 8,473 wholly outside, 1,733 cut, giving 88,714 triangles after clipping). The earlier #28 house interior had 15,038. No outdoor zone was sampled (the game can't be moved from here); the probe runs on every bridge run, so a run outdoors adds a `Probe #99` line without code changes.

### Cost of each step (96k triangles, 0 bytes allocated in every step)

| Step | 100-yalm cube | 40-yalm cube | Per triangle |
| --- | --- | --- | --- |
| Read (cull, transform to world) | 8.5 ms | 8.5 ms | about 88 ns |
| Clip to the turned box | 9.3 ms | 9.8 ms | about 100 ns |
| Deduplicate edges (hash table) | 31.5 ms | 26.9 ms | about 330 ns |
| Project, shade, depth sort | 16.0 ms | 14.8 ms | about 170 ns |
| **Whole rebuild** | **about 65 ms** | **about 60 ms** | |

The node-box walk of #28 read the same scene in 0.32 ms: the triangles are what cost, not the walk. Edges: 145,784 unique (134,944 at 40 yalms) from 142,886 shared. 38,907 shared edges (27%) lie between coplanar triangles and would vanish from a wireframe if coplanar triangles were merged.

### Cost of drawing in ImGui (60,000 primitives, the probe's cap; 0 bytes in every cell)

| Mode | View idle (cached projection) | View turning (project + sort + draw) | Per primitive, idle / turning |
| --- | --- | --- | --- |
| (a) Wireframe, unique edges | 2.2 ms | 6.6 ms | 37 / 111 ns |
| (b) Filled, flat shaded, sorted | 3.9 ms | 14.4–14.7 ms | 66 / 246 ns |
| (c) Both | at the cap before any edge was drawn: same as (b), 3.9 / 18.7 ms | | |

Both cube sizes gave the same numbers because both hit the 60,000 cap. Extrapolated linearly, drawing all 96k triangles filled would cost about 6.3 ms idle and 23 ms turning; all 145k edges about 5.3 ms idle. These are the CPU cost of filling ImGui's draw list; GPU cost is on top and wasn't measured. ImGui rebuilds its draw list every frame, so the "idle" cost is paid every frame, not only when the view changes.

### What fits the 0.5 ms frame budget

Phase 2's budget is 0.5 ms for the whole frame, and the frame already uses 0.39 ms with 101 players, 50 NPCs and 945 collision boxes (`draw-within-budget` in the same run). Taking the whole 0.5 ms for triangles alone, with nothing else drawn:

| Mode | Triangles or edges that fit |
| --- | --- |
| Wireframe, view idle | about 13,000 edges |
| Filled, view idle | about 7,500 triangles |
| Wireframe, view turning | about 4,500 edges |
| Filled, view turning | about 2,000 triangles |

The sample has 96,000 triangles, and #28's house interior 15,000. So **real triangles don't fit the budget in common places**, even at 40 yalms (88,714 after clipping), and a view that is being turned is far worse than an idle one.

### Reducing the load

- **Skip tiny triangles:** under 1 px at 300 px across, that is 1,922 of 88,714 (2%) at 40 yalms but **43,226 of 96,230 (45%)** at 100 yalms. Worth doing for the largest cube; not enough alone.
- **Merge coplanar triangles** would remove about 27% of wireframe edges here; filled triangles would need real polygon merging, which wasn't measured.
- **Cap with nearest first:** the only sure way to keep a bound. The caps in the table above are the most that fits; beyond them boxes (as now) or nothing.
- **Spread the rebuild over frames:** the 60–65 ms rebuild is about 130 frames of 0.5 ms; fine for a cache that only changes when the player moves a set fraction of the cube (#29–#30 already work this way), not for a turning view, which needs the projection redone every frame.
- **Memory:** about 108 bytes per triangle for the world, clipped and projected buffers plus 36 bytes per edge, plus a hash table for dedup (24 MB at the probe's size; a table for a 20,000-triangle cap would be about 1 MB). A cap of 20,000 triangles is roughly 3 MB, so memory isn't the problem; time is.

### Recommendation

- **Don't build "all the real triangles" inside PROJECT.md's 0.5 ms and 0 bytes budget.** Nothing allocates, but ImGui submission alone is 66 ns a filled triangle (idle) and 246 ns (turning), and the scene around the player has 15,000–96,000 of them.
- Options for the owner to choose between (commented on #99 and #100): (1) keep boxes (the current approach); (2) real triangles only nearest the player up to a cap (about 2,000 filled or 4,500 lines, so that a turning view still fits) and boxes beyond; (3) loosen the collision budget, e.g. to about 2 ms for wireframe drawing of 20,000+ edges while idle, which makes the plugin's per-frame cost many times the current one; (4) draw the triangles into a cached texture and show that while idle (not measured here; it would need a render target, which is a new kind of dependency).
- The probe code stays in the bridge (Debug only) so the numbers can be repeated after any change.

## Phantom boxes (#107)

Some drawn collision boxes sit where the player can walk straight through. The Debug-only `collision-phantoms` check
(`CollisionPhantomsCheck`) finds them in the running game: for every collider box within 150 yalms of the player
(primitive colliders, and mesh nodes holding triangles; 4000 boxes at most) it casts the game's own movement raycast
(`BGCollisionModule.RaycastMaterialFilter`) through the box (3 x 3 straight down, and two heights in from each side)
and counts a box as a phantom when no ray hits **its own collider** inside it. It logs, in `Phantoms #107` lines,
the phantom share per group of collider fields, the kinds of collider the rays hit at all, and the nearest phantoms.

Sample (housing area, player at about -696, 25, -713; 4000 boxes):

| Group | Result |
| --- | --- |
| Primitive colliders with `ObjectMaterialValue` `2000`, `2004`, `2005`, `2006`, `200A`, `3000`, `3005` (no `0x4000` bit), layer 1, raycast-active | about 160 colliders, **every one** a phantom; no ray ever hit one |
| Primitive colliders with `4000/1000`, `4005/1000`, `6004`, `6406` (`0x4000` bit) | 0 phantoms; the rays hit all of them |
| Primitive colliders on layer `A` (bit 0 clear) | all phantoms (the movement raycast is layer 0 only); already dropped by the layer rule |
| Mesh colliders, material `0` or `7000`, layer 1 | rays hit both kinds (10591 and 6745 hits); not filtered |
| Mesh node boxes with no own ray hit | about 1000 of 3600, nearly all in material-0 meshes; the 17 rays miss sparse triangles in a large node box, or an object on top of terrain is hit first. This is box coarseness, not filtering (cause 3 in the issue), left to #100 |

Findings:

- The movement layer (`LayerMask` bit 0) was never tested by the reader (#28 only tested it in the probe) and is now.
  It removes the layer-`A` primitives (about 45 in the sample).
- The field that separates blocking from non-blocking primitives is `ObjectMaterialValue` bit `0x4000`: every
  primitive the game's raycast hit had it and none that lacked it was hit. Primitives without it are the invisible
  trigger and event volumes. Meshes are not filtered on material.
- Phantom count among kept primitive colliders: about 160 before the fix, 0 after (the check fails if any remain).
  Mesh node phantoms (about 1000 of 3600 nodes at this spot) are counted separately and remain.

## Reading plan usage (#109)

The `/work-all` runner needs the current 5-hour and weekly plan usage before it starts each issue.

- **Works:** a one-turn probe, `claude -p "reply ok" --model haiku --max-turns 1 --output-format stream-json --verbose`,
  emits a `rate_limit_event` line. Its `rate_limit_info.unifiedWindows.five_hour` and `.seven_day` each have
  `utilization` (0 to 1), `resetsAt` (Unix seconds) and `status`. Measured 2026-11-18: 5-hour 0.21, weekly 0.82.
  The probe costs a few tokens on the cheapest model.
- **Not used:** the status line `rate_limits` field (only runs in interactive sessions, not `-p`), and the
  undocumented OAuth usage endpoint (needs the stored login token).
- `scripts/check-usage.ps1` wraps the probe. It prints `OK`, `UNKNOWN` or `STOP` and exits 1 only on `STOP`
  (5-hour >= 90% or weekly >= 95%). Unreadable usage never blocks. `-FromFile <saved stream-json>` tests the parsing.
- Thresholds are the issue's proposals; a measured cost per issue is not yet available (run history has token
  counts, not plan percentages), so they are unchanged.

## Turning collision boxes to fit their objects (#110)

Desk research from the code (`CollisionReader`, `CollisionBox`, `CollisionClipper`, `ClippedBox`) and FFXIVClientStructs' collider types. No measurement in game yet: the numbers below are geometry, and a Debug probe for real fit is part of the proposed implementation.

### Does the game already give the rotation?

**Yes. Nothing has to be worked out from vertices.**

- Every collider has a `World` matrix (`Matrix4x3`): mesh colliders, and the box, cylinder, sphere and plane primitives. It already holds the object's rotation (and scale).
- A mesh node's `LocalBounds` is a box in the mesh's **own** space, so it is already tight to the object. `CollisionBox.FromTransformed` takes that local box through `World` and then throws the rotation away by wrapping the result in a world-axis box (Arvo's method). That wrapping is the whole waste. Keeping the local min/max plus the matrix gives an exact oriented box.
- Primitives are unit shapes (-1..1) placed by `World`, so they are exact oriented boxes the same way.
- So the "minimal-area rectangle / PCA / trial angles" options are not needed. They would only matter for data with no transform; here every box has one. The fence at 45° is a collider turned 45° in its `World`; its nodes' local boxes are thin and straight.

### Cost: computed once, cached

- Gathering stays one matrix read per node, as today (0.12 ms per gathering frame, 0 bytes). Extracting the yaw is a few ALU ops per box: `atan2(M13, M11)` or just the stored `(cos, sin)` pair of the matrix's X axis in the ground plane. No allocation.
- Cache record: centre (3) + half-size (3) + `cos`, `sin` (2) = 8 floats, **32 bytes** instead of 24. The 2,000-box cap goes from 48 KB to 64 KB. The world AABB is derived on demand (cheap) for the existing region cull.
- Matrices that are not a pure turn about the vertical axis (pitch or roll, or shear) are the exception. Check `M12`, `M32`, `M21`, `M23` close to zero. When true (expected for nearly everything: placed housing items, fences, walls, terrain tiles at identity), use the oriented box. When false, fall back to today's axis box, so a ramp or tilted prop is never wrong, only as coarse as now. (A full 8-corner oriented box would also draw fine, but then the cube clipping needs a general convex clip; not worth it for a rare case.)
- Non-uniform scale is handled: it is folded into the half-size (column lengths of the matrix).

### Drawing and culling cost

- An upright prism with a turned rectangular footprint is **already a shape the drawing code supports**: `ClippedBox` holds a footprint polygon of up to 8 corners (`MaxCorners`), counter-clockwise, drawn by the prism path that #93 added for the turned cube. A box turned θ inside a cube turned by yaw is two rectangles intersected: at most 8 corners, exactly the existing limit. A box wholly inside the cube is just its 4 rotated corners (still 8 vertices and 12 edges, same as a box).
- **Clipping:** `CollisionClipper` today cuts the cube's footprint against four **axis** lines (Sutherland–Hodgman, `Cut(axis, value)`). With a turned box the four lines are general half-planes (a normal and an offset), the same loop with a dot product instead of picking X or Z. Roughly 2x the arithmetic per cut, on the boxes that reach the cube only; the cheap rejects (height, AABB against the footprint's bounds) stay first and cut most boxes before any polygon work. Still allocation-free, buffers on the stack.
- **Culling:** the region test uses the box's world AABB (computed from centre, half-size and yaw: `|cos|·hx + |sin|·hz`). That AABB is as large as today's, so cull results are unchanged, never worse; the exact test is done by the clip, which returns "nothing" for a box outside the cube. So culling does **not** get harder. A separating-axis test could cull a few more boxes early but isn't needed.
- Projection is unchanged: 8 corners, 12 edges per box. The per-frame work is the same; only the clip's cut arithmetic changes, and only for turned boxes.
- When the cube itself is turned (the camera's yaw), every box is already handled as a polygon, so there is no new "slow path". In north-up mode today's axis-aligned fast path (`footprint.IsAxisAligned`) applies only to boxes that are not turned themselves; turned boxes take the polygon path, which is the path every box takes whenever the cube is turned.

### How much it helps (geometry)

Footprint area, a box of length L and width t turned 45°, axis box vs oriented box:

| Object | Axis box | Oriented box | Waste removed |
| --- | --- | --- | --- |
| Square 2 × 2 yalm crate turned 45° | 2.83 × 2.83 = 8.0 | 4.0 | 2.0x tighter |
| Fence segment 10 × 0.3, turned 45° | 7.3 × 7.3 = 53 | 3.0 | **17.7x tighter** |
| Fence segment 10 × 0.3, turned 30° | 8.9 × 5.2 = 46 | 3.0 | 15x tighter |
| Wall 20 × 0.5 turned 45° | 14.5 × 14.5 = 210 | 10 | 21x tighter |
| Anything at 0°, 90° | same | same | none (e.g. most buildings, terrain tiles) |

An axis box around a thin diagonal object swallows a large empty square, which is what looks wrong (and why the phantom-box share in #107 is high for fences). A long fence is also many nodes, each thin: the oriented fence becomes a tidy chain, not a staircase of overlapping squares. **Box counts do not change**: one box per node either way; the saving is covered space and visual fit, not draw cost. The real fit on the game's actual data (covered volume with each method, how many boxes are turned at all) is not measured; the implementation issue should start by adding that to the Debug probe (`CollisionProbeCheck`), logging both volumes and the share of turned boxes.

### Risks and related

- **#115** (`collision-inside-cube` fails: a `Quad` corner is outside the cube's outline at 45°) is in the same clip path. Oriented boxes would exercise it harder; fix #115 first, or as the first step of the implementation.
- Tests to write first: a box turned by θ clipped to a cube turned by φ for several angles (every corner of the result lies inside both shapes; wholly inside and wholly outside cases; 8-corner worst case); yaw extraction from matrices including scale and mirror; tilted matrices fall back to the axis box.
- Doc and spec impact: PROJECT.md says "simplified box"; an oriented box still is one. `CollisionBox`'s doc ("world-axis-aligned, like the player cubes") and `docs/reference/` change with it.

### Recommendation

Do it. The game already provides the rotation per collider, so the oriented box costs no extra reads, one cached `(cos, sin)` pair per box (+8 bytes), and one more general clip loop on the polygon path that already exists for a turned cube. Frame cost and allocations stay as now (to be checked by the existing `draw-within-budget` check). Gain is large for thin diagonal things (about 15–20x tighter footprint for fences and walls) and nothing for axis-aligned ones. Proposed implementation issue, after #115:

1. Probe: log covered volume (axis vs oriented) and the share of turned boxes at a real location.
2. `CollisionBox` keeps centre, half-size and yaw (with a pure-yaw check; tilted falls back to an axis box); reader builds it from `World`.
3. `CollisionClipper` cuts against the box's own edges; tests as above; `collision-inside-cube` and `draw-within-budget` bridge checks pass.

### Measured at a real location (#123)

In-game probe (`collision-reader` metrics, housing ward): 219 of 376 kept boxes are turned. Total covered volume 22.35 million vs 22.45 million cubic yalms (0.4% less), because a few huge terrain boxes dominate the sum; the gain is per object (thin diagonal fences and walls), as the geometry table above predicts.

## Enemy and attacking detection (#111)

Chosen reads, from FFXIVClientStructs (no hooks, no game calls beyond getters):

- **Enemy:** a battle NPC whose `Character.IsHostile` is set (`PlayerSet.ClassifyNpc`). Event NPCs are never enemies.
- **Target:** `TargetSystem.GetTargetObject()` (hard target, else soft target) compared with each object's pointer.
- **Attacking:** the enemy's entity id is in `UIState.Hater.Haters` (the list behind the enmity UI), checked only for enemies.

The fake-player bridge check (`enemy-markers`) covers the colour and label text. The real `IsHostile` and `Hater` reads need a fight to confirm; steps are in `docs/manual-tests.md`. Reliability of the hater list is not yet verified in game.

## Text-labelled interactable objects (#112)

Probe: the Debug bridge check `probe-interactables` logged every object near the player that isn't a character or NPC (`Probe #112` in `dalamud.log`), once in a housing ward. Fields compared: object kind, name, `NamePlateIconId`, targetable flag.

| Kind | Examples seen | Verdict |
| --- | --- | --- |
| `EventObj` with a name | "Entrance", "Apartment Building Entrance", "Aethernet Shard", "Market Board", "Summoning Bell", "Placard" | text label: **shown** |
| `EventObj` with an empty name | 784 sightings, all nameless | icon-only markers (arrows) and invisible triggers: left out |
| `HousingEventObject` | letter boxes, chocobo stables, sprinklers, lamps, fountains | furniture/gardening: left out (a separate object kind) |
| `Companion` | "Fox Kit" | minion: left out |

`NamePlateIconId` was 0 for every object, so it can't separate icons from text. The targetable flag changes with distance for the same object ("Entrance" appears both ways), so it isn't used either. The rule is therefore **kind is `EventObj` and the name is non-empty** (`PlayerSet.IsTextInteractable`). Not seen in this one session: aetherytes (a separate `Aetheryte` kind), treasure coffers and gathering points; they are not included. Cap: 30 per frame (a ward showed a few dozen in range), with their own count so they can't push out players or NPCs.

## Matching the game's real collision more closely (#125)

Owner report from inside a house: a large square drawn where nothing can be seen or touched, and a counter with an opening drawn as one long box with no gap. Both mean the drawn boxes cover space the player can walk through (PROJECT.md, "Phase 2": collision is what the player would bump into).

Measured 2026-11-18 with the new Debug-only check `probe-collision-fit` (`CollisionFitProbeCheck`, `Fit #125` lines in `dalamud.log`, metrics `fit*`). It reads every blocking mesh node that holds triangles within 40 yalms of the player and compares the box drawn today with boxes built from the node's own triangles. Read-only: memory reads and the game's movement raycast. Two samples: outdoors in a housing area (532 nodes, 30,538 triangles) and then a house interior (1,042 nodes, 95,630 triangles; layout ids `A59DD5`, `A59DF6`, `A59E17` are the room's shell pieces).

### Where the wrong boxes come from

- **Not the inner nodes.** All 532 and 1,042 nodes that hold triangles are leaves; no inner node holds triangles of its own. The first idea (an inner node's box spanning its whole subtree) is false.
- **Cause:** a leaf holds only about 9 to 16 triangles (up to 256 allowed) and its box is the **tight box around all of them**. When those few triangles lie far apart, for example the floor edge, a wall on the far side and a ceiling strip of a room, or the two blocks of a counter on either side of an opening, the box is a big mostly-empty block spanning the gap. The box for a leaf is exactly as tight as the node's triangles allow, so the node box equals the tight box (100% in both samples): nothing in the current reader is "too loose". It is **too coarse**: one box per node when the node's triangles form several separate pieces.
- Typical offenders in the house: boxes about 20 x 6.5 x 24 yalms (2,000 to 6,500 cubic yalms) holding 9 to 12 triangles. Every one of the 15 worst whole boxes except one had no ray hit anywhere inside (the same 3 x 3 + side rays as #107), which is the large square the owner saw. Split at gaps, the same nodes become 2 to 6 boxes of about 1 to 150 cubic yalms, and almost every piece has a ray hit.
- A wall or floor made of axis-aligned triangles has zero thickness on one axis, so a piece of one is a flat sheet: the drawing needs a minimum thickness for it (a few centimetres is enough; the existing box code already draws the thin 1-yalm slab from #101).

### Options measured

Candidate A: split each node's triangles into connected groups of 0.5-yalm cells. Candidate B (**better**): sort the triangles' own bounds along each axis and cut wherever a gap of at least 0.5 yalm separates them, recursively; each group becomes one box (0.5 yalm is about the player's width, so a narrower gap cannot be walked through and need not be shown). No cells, no hash table.

| Housing interior, 40-yalm region | Today | Candidate B |
| --- | --- | --- |
| Boxes for 1,042 nodes | 1,042 | **1,183** (+14%; 88 nodes split) |
| Volume covered (cubic yalms) | 98,844 | **28,905** (29%) |
| Nodes with a side of 3 yalms or more (709 nodes, 59,097 triangles) | 97,755 | **27,941**, 850 boxes |
| Ray hits inside the boxes of the 79 worst nodes | 20 of 79 whole boxes | **147 of 194** pieces |

The outdoors sample had two huge terrain boxes (22 and 11 million cubic yalms, 15 triangles each, 222 yalms away, no ray hit) that the cell groups reduced to a few thousand: coarse mostly-empty nodes are common, not a house peculiarity.

Candidate B's cost, measured as an upper bound (Debug build, allocating lists): 69 ms for the 95,630 triangles, about 0.7 microseconds each (the earlier #99 read of a triangle through its matrix is 88 ns on top). 59% of the triangles are in the nodes with a side of 3 yalms or more, which hold nearly all the wasted volume. An allocation-free version should be several times faster, but a full gather of a busy room is still tens of milliseconds, far above the 0.2 ms the cache spends per frame, so it cannot be done on every gather.

### Recommendation

**Refine each node once, in the mesh's own space, and keep the result.** The sweep depends only on the node's triangles, which don't change while the mesh is loaded, so:

1. Compute a node's pieces the first time the node is reached, in the mesh's local space (so the #123 turn still applies), and keep them in a bounded cache keyed by node address (about 24 to 32 bytes a piece; 1,042 nodes made 1,183 pieces, so a cap of about 4,000 pieces is 128 KB). Drop it on zone change like the box sets. A gather then only transforms cached pieces, as cheap as today.
2. Do the refining **lazily within the frame budget**: a few hundred triangles a frame (about 0.1 ms), largest nodes first (side of 3 yalms or more; those are 59,097 of 95,630 triangles but nearly all the waste). A node not yet refined is drawn as today's coarse box, so the cube never has holes, only the coarse look it has now, and it sharpens within about a second or two when the player stands in a new place.
3. Skip nodes that cannot be improved cheaply: a node whose triangles fit well already (about 90% of nodes at 100% fill) still gets one sweep when first seen but costs nothing afterwards.
4. Keep the minimum drawn thickness for flat pieces, and cap pieces per node (for example 8; merge the closest two beyond that) so a scattered node cannot flood the box cap.
5. Add a bridge check: for the sample region, refined boxes cover every ray hit that the coarse boxes did, and the share of boxes with a ray hit rises (here from 25% to 76% of the worst nodes).

Limits to expect: a single big tilted triangle (a ramp, a roof) still has a large axis box, and the sweep cannot split inside one triangle; the node's local box plus #123's turn helps when the object is turned, not for a tilted plane. Ray-test misses on very thin sheets (a few centimetres) remain in any box-based approach. The mesh itself is still not drawn: triangles stay out of reach of the budget (#99).
