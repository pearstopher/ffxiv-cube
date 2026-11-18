# Official Dalamud repo standards (#26)

The rules for plugins in the official repository ([goatcorp/DalamudPluginsD17](https://github.com/goatcorp/DalamudPluginsD17)), and how Cube meets them. Checked on 2026-11-17 against the current dalamud.dev pages, read from scratch, plus a read of all the plugin's code.

**Cube is personal use only and is never submitted** (PROJECT.md, "Release and distribution"). This check only makes sure it follows the official rules; no submission files are prepared.

**Summary:** Cube meets every rule. The one gap found (in PvP it would show enemy players around you, which the rules ban) was fixed in #69: the cube is hidden in PvP areas. Otherwise: no plugin dependencies, no server traffic, no hooks, and read-only game access.

## Sources

The four pages in the "Before submitting your plugin" box on [Submitting a plugin](https://dalamud.dev/plugin-publishing/submission):

- [Plugin metadata](https://dalamud.dev/plugin-development/plugin-metadata)
- [Technical considerations](https://dalamud.dev/plugin-development/technical-considerations)
- [Plugin restrictions](https://dalamud.dev/plugin-publishing/restrictions/)
- [AI Usage Policy](https://dalamud.dev/plugin-publishing/ai-policy/)

Also used: the [DalamudPluginsD17 README](https://github.com/goatcorp/DalamudPluginsD17) and the previous plugin's check (Remove PF Duplicates), for the build rules.

## 1. Plugin metadata

Checked against `Cube/Cube.csproj`, which DalamudPackager turns into the `Cube.json` manifest.

| Field | Required | Status | Value or note |
| --- | --- | --- | --- |
| Name | Yes | ✅ | Cube |
| Author | Yes | ✅ | Ane Mora |
| Punchline | Yes | ✅ | "See the space around you as a cube, with nearby players as small cubes." One line. |
| Description | Yes | ✅ | Starts with a summary sentence, then says it only reads and draws, never changes the game or sends anything. Short enough not to need manual wrapping. |
| RepoUrl | No | ✅ | `https://github.com/pearstopher/ffxiv-cube` |
| Tags | No | ✅ | cube, radar, players, nearby, 3d, map |
| IconUrl | No | — | No icon yet. Optional. If one is added, the AI policy prefers a hand-made icon (even a crude one) over an AI-generated one. |
| ImageUrls, CategoryTags, Changelog, FeedbackMessage | No | — | Not set; all optional. `images/cube-window.png` is a README screenshot, not a manifest image. |
| ApplicableVersion, LoadRequiredState, LoadSync, CanUnloadAsync, LoadPriority, AcceptsFeedback | No | ✅ | DalamudPackager defaults. |
| AssemblyVersion, InternalName, DalamudApiLevel | Filled in automatically | ✅ | Left to DalamudPackager, as the page says: `<Version>` 1.0.0.0, `Cube`, and the API level of Dalamud.NET.Sdk 15.0.0. |
| `Dip17Channel` | Must not be set | ✅ | Not set. |

## 2. Technical considerations

| Point | Kind | Status | Why |
| --- | --- | --- | --- |
| Regular windows use the Dalamud Windowing API | Recommendation | ✅ | The cube window and the settings window are `Window`s in one `WindowSystem` (`Plugin`). |
| Use Lumina rather than XIVAPI for game data | Recommendation | ✅ | Cube reads no data sheets and makes no web requests. |
| Don't hurt game performance | Recommendation | ✅ | No allocations in the per-frame path, nothing done while the window is closed or hidden, culling before projection (PROJECT.md, "Efficiency and memory"). Measured in #23 at the 100-player cap: 0 bytes allocated over a 10-minute soak, 0.216 ms per frame. The bridge's budget checks keep it that way. |
| Backend server rules (data minimisation, opt-in telemetry, identifiers, HTTPS, DNS names…) | Mandatory if a server is used | — | Not applicable: no networking at all. |

## 3. Restrictions

| Rule | Status | Why |
| --- | --- | --- |
| No automatic server traffic | ✅ | Cube never sends anything. It reads game memory and draws in its own window. |
| No out-of-spec server traffic | ✅ | No server traffic. |
| No combat changes | ✅ | It shows where players stand, the same players the game already draws around you. It shows nothing about mechanics, enemies, cooldowns or damage, and changes nothing. |
| Doesn't affect Square Enix's revenue | ✅ | |
| No parsing, raid logging, DPS meters or similar | ✅ | No combat data is read or kept. |
| Doesn't collect other players' account IDs | ✅ | Players are told apart by object table slot and `EntityId`, a per-session object ID, never content or account IDs. Names are only drawn on screen, never written to files outside Debug test output, never sent (PROJECT.md, "Privacy"). |
| No hard dependency on rule-breaking plugins | ✅ | No plugin dependencies at all, no IPC (CLAUDE.md, PROJECT.md "Phase 2"). vnavmesh is reference material only. |
| Not only useful out of spec | ✅ | It's a view of normal play. |
| No PvP or competitive advantage | ✅ | In PvP the cube would show enemy players behind you or behind walls, like a radar, so it's hidden in PvP areas (PROJECT.md "When hidden", read from `IClientState.IsPvP`; fixed in #69). |
| Not a "common non-starter" (emote looping, skipping cutscenes or dialogs, auto crafting, auto loot, friend alerts, AOE markers or recolouring, camera zoom, FFLogs, parsers, Fantasia, XIVCombo, anything in PvP) | ✅ | None of these. It reads the camera's direction but never changes the camera. It shows nothing in PvP (#69). |

The restrictions page doesn't mention radars or player-position overlays directly, and says review is subjective. Cube only shows players already loaded around you (the game's own render range), never more, so it doesn't show information the player couldn't already see by turning the camera.

## 4. AI Usage Policy

The policy applies to official submissions, which Cube never makes. For the record:

| Rule | Status | Why |
| --- | --- | --- |
| Disclose AI use in the PR description | — | No submission. |
| Disclosures written by a person | — | No submission. |
| AI-generated assets disclosed in the plugin description | ✅ | There are no AI-generated assets (no icon, no images in the manifest). |
| Tested personally | ✅ | The in-game test bridge runs every feature in the real game (`docs/in-game-testing.md`), and the owner checks by hand where needed (`docs/manual-tests.md`). |
| Able to explain every design decision | ✅ | The generated code reference (`docs/reference/`) documents why each part exists and which PROJECT.md rule it implements. |

## 5. Code review: how the plugin touches the game

All code in `Cube/` and `Cube.Core/` was read. `Cube.Core` is plain logic with no game, Dalamud or network access (a unit test fails if it references Dalamud). Everything that touches the game is in `Cube/`:

| What | Where | What it's for | Reads or changes |
| --- | --- | --- | --- |
| `GameObjectManager`, `Control.LocalPlayer` (FFXIVClientStructs) | `PlayerReader` | Your position (the cube's centre) and the players in the 200 character slots: position, party/alliance flags, name. | Reads only. |
| `CameraManager` scene camera (FFXIVClientStructs) | `CameraReader` | Which way the camera looks, for the Camera orientation mode. | Reads only. |
| `IClientState`, `ICondition` | `GameStateReader` | Logged in, GPose, PvP, loading screens and cutscenes, to hide the cube (PROJECT.md, "When hidden"). | Reads only. |
| `ICommandManager` | `Plugin` | The `/cube` and `/cube config` command. | Registered on load, removed on unload. |
| `IDalamudPluginInterface.UiBuilder`, `WindowSystem` | `Plugin`, `CubeWindow`, `SettingsWindow` | Draws the two windows with ImGui; the installer's open and settings buttons. | Draws in the plugin's own windows only. |
| `SavePluginConfig` | `Configuration` | Saves the settings to `pluginConfigs\Cube.json`. No names or IDs in it. | Writes the plugin's own config file. |
| `IPluginLog` | everywhere | Load/unload lines and errors. | Writes to `dalamud.log` on the player's PC only. |
| `IFramework`, `IObjectTable`, file writes | `TestBridge/` | The in-game test bridge and fake players. | **Debug builds only**: the folder is excluded from Release builds, and a test checks the Release DLL. |

**Not used anywhere:** hooks or signatures (`IGameInteropProvider`), calls to game functions, network access, chat, sending packets or acting for the player, content or account IDs, other plugins' IPC.

**Safety checks:**

- **Error handling:** every game-facing handler (the draw, the command, the installer buttons, saving settings) catches and logs exceptions, so none reach the game (#22).
- **Pointer safety:** the object manager, control, local player, camera manager and camera are null-checked before use; only the 200 character slots are read.
- **Unloading (#22):** the command, UI callbacks, draw callback and windows are all removed, each even if another step fails. Cube changes nothing in the game, so there's nothing to restore.

## Build

| Rule | Status | Why |
| --- | --- | --- |
| Public, open-source Git repo | ✅ | Public GitHub repo, AGPL-3.0. |
| Uses Dalamud.NET.Sdk / DalamudPackager | ✅ | Dalamud.NET.Sdk 15.0.0, pinned in `global.json`. |
| `packages.lock.json` committed | ✅ | `Cube/packages.lock.json` is tracked and `dotnet restore --locked-mode` passes (re-checked 2026-11-17). `Cube.Core` has no packages. |
| No downloads during the build | ✅ | Only the NuGet packages in the lock file. |
| Same commit always builds the same version | ✅ | `<Version>` in the csproj is the real version; `release.yml` checks the tag matches it. |
| Version not based on a timestamp or build counter | ✅ | Hand-set `MAJOR.MINOR.PATCH.0`. |
| Icon 64–512 px, square PNG | — | No icon (optional, see metadata). |

## Phase 2 (collision)

The collision features (#28–#32) must also stay read-only: read the collision the client already has loaded, no hooks that change anything, no dependency on vnavmesh (PROJECT.md, "Phase 2"). The research issue (#28) should check its chosen approach against this page, and #34 rechecks the standards before v1.1.0.

## Follow-up issues

1. **Hide the cube in PvP** (#69). Done.
