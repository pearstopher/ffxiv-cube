"""Unit tests for make_repo_json.py: the repo.json entry the release workflow publishes.

Run with: python -m unittest discover -s scripts
"""
import json
import tempfile
import unittest
from pathlib import Path

import make_repo_json


def manifest(version="1.0.0.0", **extra):
    """A manifest shaped like the Cube.json that DalamudPackager writes."""
    data = {
        "Author": "Ane Mora",
        "Name": "Cube",
        "InternalName": "Cube",
        "AssemblyVersion": version,
        "DalamudApiLevel": 15,
        "Tags": ["cube"],
        "Changelog": "not copied",
    }
    data.update(extra)
    return data


class MakeEntryTests(unittest.TestCase):
    """The entry points Dalamud at the tag's release and carries the manifest's fields."""

    def test_download_links_point_at_the_tags_release(self):
        entry = make_repo_json.make_entry(manifest(), "v1.0.0", "pearstopher/ffxiv-cube", 123)
        url = "https://github.com/pearstopher/ffxiv-cube/releases/download/v1.0.0/latest.zip"
        self.assertEqual(url, entry["DownloadLinkInstall"])
        self.assertEqual(url, entry["DownloadLinkUpdate"])
        self.assertEqual(url, entry["DownloadLinkTesting"])
        self.assertEqual(123, entry["LastUpdate"])

    def test_manifest_fields_are_copied_and_unknown_ones_dropped(self):
        entry = make_repo_json.make_entry(manifest(), "v1.0.0", "o/r", 0)
        self.assertEqual("Cube", entry["InternalName"])
        self.assertEqual(15, entry["DalamudApiLevel"])
        self.assertEqual(["cube"], entry["Tags"])
        self.assertNotIn("Changelog", entry)

    def test_missing_required_field_is_an_error(self):
        data = manifest()
        del data["DalamudApiLevel"]
        with self.assertRaises(ValueError):
            make_repo_json.make_entry(data, "v1.0.0", "o/r", 0)

    def test_manifest_version_must_match_the_tag(self):
        with self.assertRaises(ValueError):
            make_repo_json.make_entry(manifest(version="1.0.1.0"), "v1.0.0", "o/r", 0)

    def test_tag_must_be_three_part(self):
        with self.assertRaises(ValueError):
            make_repo_json.tag_to_version("v1.0")
        self.assertEqual("1.2.3.0", make_repo_json.tag_to_version("v1.2.3"))


class MainTests(unittest.TestCase):
    """The script writes a one-entry list that Dalamud can read."""

    def test_writes_a_one_entry_list(self):
        with tempfile.TemporaryDirectory() as tmp:
            src = Path(tmp) / "Cube.json"
            out = Path(tmp) / "repo.json"
            src.write_text(json.dumps(manifest()), encoding="utf-8-sig")
            code = make_repo_json.main(["x", str(src), "v1.0.0", "o/r", str(out)])
            self.assertEqual(0, code)
            data = json.loads(out.read_text(encoding="utf-8"))
            self.assertEqual(1, len(data))
            self.assertEqual("1.0.0.0", data[0]["AssemblyVersion"])

    def test_wrong_argument_count_is_usage_error(self):
        self.assertEqual(2, make_repo_json.main(["x"]))


if __name__ == "__main__":
    unittest.main()
