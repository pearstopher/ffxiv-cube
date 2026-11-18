"""Build repo.json, the custom plugin repository file Dalamud's plugin installer reads.

Usage:
    python scripts/make_repo_json.py <manifest.json> <tag> <owner/repo> <output repo.json>

<manifest.json> is the Cube.json manifest produced by `dotnet build -c Release` (DalamudPackager).
This script adds the download links for the GitHub Release of <tag> and the update time, and
writes a one-entry repo.json list. The release workflow (.github/workflows/release.yml) runs it
after publishing the release and commits the result to main.
"""

import json
import re
import sys
import time
from pathlib import Path

# Fields copied from the build's manifest into repo.json, in this order.
MANIFEST_FIELDS = [
    "Author",
    "Name",
    "InternalName",
    "AssemblyVersion",
    "Description",
    "Punchline",
    "ApplicableVersion",
    "RepoUrl",
    "IconUrl",
    "Tags",
    "DalamudApiLevel",
    "LoadRequiredState",
    "LoadSync",
    "CanUnloadAsync",
    "LoadPriority",
    "AcceptsFeedback",
]

REQUIRED_FIELDS = ("InternalName", "AssemblyVersion", "DalamudApiLevel")

TAG_PATTERN = re.compile(r"^v(\d+)\.(\d+)\.(\d+)$")


def tag_to_version(tag: str) -> str:
    """v1.2.3 -> 1.2.3.0, the four-part version Dalamud and the csproj use."""
    match = TAG_PATTERN.match(tag)
    if not match:
        raise ValueError(f"tag {tag!r} is not vMAJOR.MINOR.PATCH")
    return ".".join(match.groups()) + ".0"


def make_entry(manifest: dict, tag: str, repo: str, now: int) -> dict:
    """One repo.json entry: the manifest's fields plus download links for the tag's release."""
    missing = [f for f in REQUIRED_FIELDS if f not in manifest]
    if missing:
        raise ValueError(f"manifest is missing required fields: {', '.join(missing)}")

    expected = tag_to_version(tag)
    if manifest["AssemblyVersion"] != expected:
        raise ValueError(f"manifest version {manifest['AssemblyVersion']} does not match tag {tag} ({expected})")

    download = f"https://github.com/{repo}/releases/download/{tag}/latest.zip"
    entry = {field: manifest[field] for field in MANIFEST_FIELDS if field in manifest}
    entry["DownloadLinkInstall"] = download
    entry["DownloadLinkUpdate"] = download
    entry["DownloadLinkTesting"] = download
    entry["LastUpdate"] = now
    return entry


def main(argv: list[str]) -> int:
    if len(argv) != 5:
        print(__doc__, file=sys.stderr)
        return 2

    manifest_path, tag, repo, output_path = argv[1:]
    manifest = json.loads(Path(manifest_path).read_text(encoding="utf-8-sig"))
    entry = make_entry(manifest, tag, repo, int(time.time()))

    Path(output_path).write_text(json.dumps([entry], indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
    print(f"Wrote {output_path}: {entry['InternalName']} {entry['AssemblyVersion']} (API {entry['DalamudApiLevel']}) -> {entry['DownloadLinkInstall']}")
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))
