# Development guide

How to build Cube on your PC, load it into the game, and check that a change works before opening a PR.

You test **locally**. The build produces a plugin DLL on your machine, and you point Dalamud at it as a *dev plugin*. The `latest.zip` you may see in `bin/` is a build artifact: it is git-ignored and is not how you test.

## 1. One-time setup

You need:

- **XIVLauncher with Dalamud.** Launch the game through XIVLauncher at least once, so that Dalamud's files exist in `%APPDATA%\XIVLauncher\addon\Hooks\dev\`. The build reads Dalamud's DLLs from there.
- **The .NET 10 SDK.** Check it with:

  ```sh
  dotnet --list-sdks
  ```

  You should see a `10.0.x` line. If you don't, install it with `winget install Microsoft.DotNet.SDK.10`, then restart your terminal and VS Code.
- **Python 3**, only to regenerate the code reference (section 7).
- **An editor (optional).** VS Code with the C# Dev Kit extension, Visual Studio, or Rider can all open `Cube.slnx`.

If XIVLauncher is installed somewhere other than the default, set the `DALAMUD_HOME` environment variable to its `addon\Hooks\dev` folder.

## 2. Build

From the repo root:

```sh
dotnet build
```

This makes a **Debug** build:

```
Cube\bin\x64\Debug\Cube.dll
```

Use Debug for day-to-day testing: the in-game test bridge and fake players only exist in Debug builds. A Release build (`dotnet build -c Release`) is what gets shipped. It also packages `bin\x64\Release\Cube\latest.zip`, which only matters for releases.

A good build ends with `Build succeeded. 0 Warning(s) 0 Error(s)`. Missing XML doc comments are build errors (the docs are generated from them), so add a `<summary>` to every new type and member.

Restores use a lock file (`Cube/packages.lock.json`). CI runs `dotnet restore --locked-mode`; if you change a package, commit the updated lock file.

## 3. Load it in game (first time only)

1. Start the game through XIVLauncher and log in.
2. Type `/xlsettings` in chat, then open the **Experimental** tab.
3. Under **Dev Plugin Locations**, paste the **full path to the Debug DLL**, then click **+** and **Save and Close**. For example:

   ```
   C:\Users\<you>\Documents\VSCode\ffxiv-cube\Cube\bin\x64\Debug\Cube.dll
   ```

4. Type `/xlplugins`, then go to **Dev Tools > Installed Dev Plugins**.
5. **Cube** should be listed. Enable it.

Dalamud remembers the location, so you only do steps 2 and 3 once. If you also install the released version later, disable one of the two: never run both at the same time.

> **Point the game at the Debug DLL.** `dotnet build` only updates `bin\x64\Debug\`. If the location points at `bin\x64\Release\` instead, the game keeps running whatever was last built as Release, which can be days old.

### Automatic reloading (recommended)

In `/xlplugins` > **Dev Tools > Installed Dev Plugins**, open the plugin's settings and turn on **automatic reloading**. Dalamud then watches the exact DLL your dev location points at, and reloads the plugin about half a second after that file changes. Building is enough; you don't need to touch the game.

## 4. Check that it loaded

Type `/xllog` to open the Dalamud log, and type `Cube` in its filter box. You should see:

```
[Cube] Cube loaded (Debug build 1.0.0.0, built 2026-11-17 10:36:02, from C:\...\Cube\bin\x64\Debug\Cube.dll)
```

**Check this line every time you test.** The configuration, version, build time and path tell you exactly which build is running:

- **The build time is older than your last `dotnet build`:** the game is running a stale build. Check that the path is the Debug folder, then reload.
- **The path says `Release`:** your dev location points at the wrong folder (see the note in section 3).

Disable the plugin in `/xlplugins` and you should see `Cube unloaded`. Red errors (`ERR`) or exceptions mentioning the plugin are bugs.

The same log is saved to `%APPDATA%\XIVLauncher\dalamud.log` (`dalamud.old.log` for the previous session).

## 5. The edit and test loop

1. Change the code (and its doc comments).
2. Run `dotnet build` and `dotnet test`.
3. With automatic reloading on, the plugin reloads by itself.
4. In `/xllog`, confirm the new `loaded (...)` line shows the build time you just built.
5. Check the change in game. `/cube` toggles the cube window.
6. Run `python scripts/build-docs.py` and commit `docs/reference/` with the change.

If the build fails because the DLL is in use, disable the plugin in game, build, then enable it again.

In-game checks are automated: the Debug-only test bridge runs a suite of checks on every load and writes a results file, and one script builds, waits for fresh results and fails if any check fails:

```sh
python scripts/ingame_test.py
```

Setup, exit codes and how to read the results are in `docs/in-game-testing.md`. Only what the bridge can't check goes in `docs/manual-tests.md`.

The script's own decisions are unit tested: `python -m unittest discover -s scripts`.

## 6. Unit tests

The cube maths, projection, draw ordering and colour rules live in `Cube.Core`, a library that doesn't reference Dalamud, the game or Lumina. That lets `Cube.Tests` (xUnit) test it without launching anything. A test fails if Core ever references Dalamud, game structs or Lumina, so game-specific code belongs in the plugin project.

```sh
dotnet test
```

- Name tests as behaviour (`Player_outside_the_cube_is_not_drawn`) and give each test class a `<summary>` naming the PROJECT.md rule it checks; a test enforces the summary.
- Hot-path code must not allocate (PROJECT.md, "Efficiency and memory"). Wrap it in `Allocations.AssertNone(() => ...)` in its tests; the helper fails the test if anything is allocated after a warm-up run.

## 7. Documentation

The "how it works" docs are generated from the code's XML doc comments; there's no hand-written code guide. Two outputs:

- **`docs/reference/`** (committed Markdown, readable on GitHub, linked from the README). Regenerate it after any code or comment change:

  ```sh
  dotnet tool restore        # once
  python scripts/build-docs.py
  ```

  CI fails if the committed reference doesn't match the code.
- **The DocFX HTML site** (optional, not linked anywhere): `dotnet docfx docs/docfx.json --serve`, then open http://localhost:8080. CI builds it with warnings as errors.

Each namespace's overview lives in `docs/namespaces/<namespace>.md`; a test fails if a namespace has none. Hand-written articles (this guide, the README) are included in the site from `docs/articles/`.

## 8. Before opening a PR

- [ ] `dotnet build` succeeds with 0 warnings
- [ ] `dotnet test` passes
- [ ] `docs/reference/` regenerated and committed
- [ ] The plugin loads and unloads cleanly in game, with no errors in `/xllog`, and the in-game checks pass (or are marked pending)
- [ ] The PR body says `Closes #<issue>`

## Troubleshooting

| Problem | Fix |
| --- | --- |
| Build error about Dalamud references or `DALAMUD_HOME` | Run the game with Dalamud once, or set `DALAMUD_HOME` (see section 1). |
| `dotnet` is not recognized | Install the .NET 10 SDK, then restart the terminal or VS Code. |
| Build error CS1591 or SA1600 (missing XML comment) | Add a `<summary>` to the type or member named in the error. Docs are generated from these, so they're required. |
| `NU1004` / lock file error on restore | A package changed without updating `Cube/packages.lock.json`. Run `dotnet restore --force-evaluate` and commit the lock file. |
| CI says `docs/reference/` is out of date | Run `python scripts/build-docs.py` and commit the result. |
| The plugin isn't listed under Installed Dev Plugins | Check that the path in `/xlsettings` points at the `.dll` file itself and that the file exists. |
| The plugin is listed but won't load | Look in `/xllog` for the exception. After a game patch, Dalamud may need an update first. |
| My change didn't take effect | Compare the build time in the `loaded (...)` line with your last build. If it's older, point the dev location at Debug (section 3), then rebuild or reload. |
