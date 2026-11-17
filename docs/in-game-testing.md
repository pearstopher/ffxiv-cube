# In-game testing guide

Every feature is checked in the running game automatically, with nobody playing (PROJECT.md, "Automated testing"). The Debug build contains a **test bridge** that runs a suite of in-game checks and writes the results to a file; one script builds the plugin, waits for those results and reports them. This page covers the one-time setup and how to read the results.

Building and loading the plugin in general is in the [development guide](https://github.com/pearstopher/ffxiv-cube/blob/main/DEVELOPMENT.md). The bridge's design is in the [research notes](https://github.com/pearstopher/ffxiv-cube/blob/main/docs/research-notes.md) (#7).

## One-time setup (owner)

1. Start the game through XIVLauncher and log in to a character.
2. `/xlsettings` > **Experimental** > **Dev Plugin Locations**: add the **Debug** DLL, then **Save and Close**:

   ```
   C:\Users\<you>\Documents\VSCode\ffxiv-cube\Cube\bin\x64\Debug\Cube.dll
   ```

3. `/xlplugins` > **Dev Tools > Installed Dev Plugins**: enable **Cube**, open its settings and turn on **automatic reloading**.
4. Check `/xllog` (filter `Cube`) shows `Cube loaded (Debug build ...)` and, a few seconds later, `Test bridge: N/N checks passed, results in ...` (N is the number of registered checks).
5. Optional: set the `/xllog` level dropdown to **Debug+** (it's saved) to see each check's line.
6. Leave the game running and logged in, ideally somewhere with other players around (a city). Keep the game UI visible: Dalamud hides plugin windows during cutscenes, GPose and loading screens, so checks that need the window fail there.

## Running the checks

From the repo root:

```sh
python scripts/ingame_test.py
```

It builds the Debug plugin, then waits (up to 3 minutes, `--timeout` to change) for results from that build:

- If the build changed the DLL, Dalamud reloads the plugin about half a second later and the bridge runs on load.
- If nothing changed (or with `--no-build`), the script writes `request.json` and the bridge runs again.

Add `--snapshot` to also render the frame the bridge dumped (`drawlist.json`, from the `frame-snapshot` check) to `snapshots/snapshot.png`, with the dev-only `Cube.Snapshot` tool. `snapshots/` is git-ignored. To see what Core draws without the game at all:

```sh
dotnet run --project Cube.Snapshot -- --demo snapshots/demo.png 0.5
```

(the last number is the view's yaw in radians).

| Exit code | Meaning |
| --- | --- |
| 0 | Every check passed. |
| 1 | At least one check failed. The summary says which and why. |
| 2 | Game unreachable: no fresh results in time. The script prints the last `Cube loaded`/`unloaded` line from `dalamud.log`, which shows whether the plugin is loaded at all. |
| 3 | The build failed. |

Claude runs this for every issue that touches the game. If it exits 2, the issue is finished with unit tests and the in-game check is marked pending (CLAUDE.md).

## Reading the results

The bridge writes `%APPDATA%\XIVLauncher\pluginConfigs\Cube\test-bridge\results.json`:

```json
{
  "buildTime": "2026-11-17 12:08:45",
  "dllPath": "C:\\...\\Cube\\bin\\x64\\Debug\\Cube.dll",
  "request": "load",
  "startedAt": "2026-11-17 12:08:47",
  "finishedAt": "2026-11-17 12:08:52",
  "passed": true,
  "checks": [
    {
      "name": "draw-within-budget",
      "passed": true,
      "message": "120 frames: average 0.0012 ms (budget 0.5 ms), 0 bytes allocated in 0 frame(s) (budget 0).",
      "durationMs": 2510.3,
      "metrics": { "frames": 120, "averageMs": 0.0012, "maxMs": 0.004, "allocatedBytes": 0, "framesWithAllocations": 0 }
    }
  ]
}
```

- `buildTime` is the loaded DLL's write time: it must match the build you just made.
- `request` is `load` for the run on load, or the id the script wrote.
- Each check's `message` says what it saw; `metrics` holds the numbers it measured.

The same summary is logged: `Test bridge: N/M checks passed` at Information, and one line per check at Debug.

## Adding checks

Each feature that touches the game adds checks for its behaviour: a class in `Cube/TestBridge/Checks/` deriving from `BridgeCheck`, registered in `Plugin`'s constructor. A check is an iterator: `yield return CheckStep.NextFrame` waits a frame, and it ends with `CheckStep.Pass(...)` or `CheckStep.Fail(...)`. Put back anything it changes in `Cleanup`. The whole `TestBridge` folder is compiled out of Release builds.

## If the checks don't run

| Problem | Fix |
| --- | --- |
| Exit 2, and no `Cube loaded` line in `dalamud.log` | The dev plugin isn't set up or enabled (setup steps 2–3). |
| Exit 2, but `Cube loaded` shows an older build | Automatic reloading is off, or the dev location points at the Release DLL. |
| `window-opens-and-closes` fails with "game UI hidden" | The game was in a cutscene, GPose or loading screen. Run again. |
| `loaded-build-is-current` fails | The game is running a stale build: reload the plugin in `/xlplugins`. |
| `dalamud.log` stopped updating | It stops at 100 MB per game session. Quiet noisy plugins' debug logging and restart the game. |
