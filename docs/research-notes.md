# Research notes

Findings about outside systems (Dalamud, the game) that the code relies on. Each section names the issue it came from.

## In-game test bridge design (#7)

Checked 2026-11-17 against Dalamud's source (`Dalamud/Plugin/Internal/Types/LocalDevPlugin.cs`, `master`). The game wasn't running with the dev plugin loaded, so the live spike is still pending: the first in-game run of the bridge (#8) confirms the timings below, and this section gets updated with what it measured.

### Auto-reload timing and the build stamp

- With **automatic reloading** on, Dalamud puts a `FileSystemWatcher` on the dev plugin's DLL (`NotifyFilters.LastWrite`, filtered to that one file). Each change restarts a **500 ms** timer; the reload happens 500 ms after the *last* write, so the several writes of one `dotnet build` cause one reload.
- A reload is skipped if the plugin isn't in the `Loaded` (or a load/unload error) state, e.g. when it's disabled.
- So after `dotnet build` finishes, expect the new build to be loaded within about 1–2 seconds, plus however long the plugin's constructor takes. The script allows far more than that (see "Unreachable game").
- **Which build is loaded:** the plugin reads its DLL's last write time fresh from disk on load (`BuildInfo.ReadBuildTime`, not Dalamud's cached `FileInfo`) and logs `Cube loaded (Debug build <version>, built <time>, from <path>)`. The bridge writes the same build time into its results file. The script compares it with the write time of `Cube\bin\x64\Debug\Cube.dll` it just built: equal means the results are from this build.

### Where requests and results live

- `IDalamudPluginInterface.ConfigDirectory` is `%APPDATA%\XIVLauncher\pluginConfigs\<InternalName>\`. The internal name is the assembly name, `Cube`, so on this PC:

  ```
  C:\Users\snows\AppData\Roaming\XIVLauncher\pluginConfigs\Cube\
  ```

  (No `Cube` folder exists there yet: the plugin hasn't been loaded on this PC.)
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
