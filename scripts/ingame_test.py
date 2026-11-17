"""Run the in-game checks against the running game and report the results.

Builds the Debug plugin, waits for the Debug-only test bridge in the game to write fresh results, prints a
summary and exits with a code saying what happened (PROJECT.md, "Automated testing"; design in
docs/research-notes.md, #7):

    0  every check passed
    1  at least one check failed
    2  game unreachable: no fresh results in time (game closed, logged out of Windows, plugin not loaded)
    3  the build failed

Fresh results are matched by build time (after a build that changed the DLL, Dalamud reloads the plugin and
the bridge runs on load) or by request id (when the DLL didn't change, the script writes request.json and
the bridge runs again).

Usage: python scripts/ingame_test.py [--no-build] [--timeout SECONDS]
"""
import argparse
import datetime
import json
import os
import pathlib
import subprocess
import sys
import time
import uuid

ROOT = pathlib.Path(__file__).resolve().parent.parent
DLL = ROOT / "Cube" / "bin" / "x64" / "Debug" / "Cube.dll"
XIVLAUNCHER = pathlib.Path(os.environ.get("APPDATA", "")) / "XIVLauncher"
BRIDGE = XIVLAUNCHER / "pluginConfigs" / "Cube" / "test-bridge"
RESULTS = BRIDGE / "results.json"
REQUEST = BRIDGE / "request.json"
DALAMUD_LOG = XIVLAUNCHER / "dalamud.log"

PASSED, FAILED, UNREACHABLE, BUILD_FAILED = 0, 1, 2, 3


def build_stamp(mtime: float) -> str:
    """Formats a file time the way the bridge writes buildTime (local time, whole seconds)."""
    return datetime.datetime.fromtimestamp(mtime).strftime("%Y-%m-%d %H:%M:%S")


def is_fresh(results: dict, stamp: str, request_id: str | None) -> bool:
    """True if the results come from this build, and from our request if we made one."""
    if results.get("buildTime") != stamp:
        return False
    return request_id is None or results.get("request") == request_id


def summary(results: dict) -> list[str]:
    """One line per check, then a total."""
    lines = []
    checks = results.get("checks", [])
    for check in checks:
        mark = "PASS" if check.get("passed") else "FAIL"
        lines.append(f"  {mark}  {check.get('name')}: {check.get('message')}")
    failed = sum(1 for c in checks if not c.get("passed"))
    lines.append(f"{len(checks) - failed}/{len(checks)} checks passed (build {results.get('buildTime')})")
    return lines


def exit_code(results: dict) -> int:
    """PASSED only if the bridge says so and every check passed (an empty run is a failure)."""
    checks = results.get("checks", [])
    ok = bool(results.get("passed")) and len(checks) > 0 and all(c.get("passed") for c in checks)
    return PASSED if ok else FAILED


def read_results() -> dict | None:
    """Reads results.json, or None if it's missing or unreadable (the bridge writes it atomically)."""
    try:
        return json.loads(RESULTS.read_text(encoding="utf-8"))
    except (OSError, ValueError):
        return None


def last_plugin_log_line() -> str | None:
    """The last 'Cube loaded' or 'Cube unloaded' line in dalamud.log, to show whether the plugin is loaded at all."""
    try:
        with DALAMUD_LOG.open("rb") as log:
            log.seek(max(0, log.seek(0, os.SEEK_END) - 4_000_000))
            text = log.read().decode("utf-8", errors="replace")
    except OSError:
        return None
    lines = [line for line in text.splitlines() if "Cube loaded" in line or "Cube unloaded" in line]
    return lines[-1].strip() if lines else None


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("--no-build", action="store_true", help="skip dotnet build; just ask the bridge to run")
    parser.add_argument("--timeout", type=float, default=180, help="seconds to wait for results (default 180)")
    args = parser.parse_args()

    before = DLL.stat().st_mtime if DLL.exists() else None
    if not args.no_build:
        print("Building (Debug)...", flush=True)
        if subprocess.run(["dotnet", "build", "--nologo", "-v", "q"], cwd=ROOT).returncode != 0:
            print("Build failed.")
            return BUILD_FAILED
    if not DLL.exists():
        print(f"No plugin DLL at {DLL}.")
        return BUILD_FAILED

    stamp = build_stamp(DLL.stat().st_mtime)
    request_id = None
    if args.no_build or DLL.stat().st_mtime == before:
        # The DLL didn't change, so Dalamud won't reload the plugin: ask the bridge for a run instead.
        request_id = uuid.uuid4().hex
        BRIDGE.mkdir(parents=True, exist_ok=True)
        REQUEST.write_text(request_id, encoding="utf-8")
        print(f"Requested a run (build {stamp}, request {request_id}); waiting up to {args.timeout:.0f} s...", flush=True)
    else:
        print(f"Waiting up to {args.timeout:.0f} s for the game to reload build {stamp}...", flush=True)

    deadline = time.monotonic() + args.timeout
    while time.monotonic() < deadline:
        results = read_results()
        if results is not None and is_fresh(results, stamp, request_id):
            print("\n".join(summary(results)))
            print(f"Results: {RESULTS}")
            return exit_code(results)
        time.sleep(1)

    print(f"Game unreachable: no results for build {stamp} within {args.timeout:.0f} s.")
    line = last_plugin_log_line()
    print(f"Last plugin line in dalamud.log: {line}" if line else "dalamud.log has no 'Cube loaded' line: the dev plugin isn't set up or enabled.")
    return UNREACHABLE


if __name__ == "__main__":
    sys.exit(main())
