"""Regenerate the Markdown code reference in docs/reference/ from the code's XML doc comments.

The reference is committed so it can be read directly on GitHub (no GitHub Pages needed). CI runs this
script and fails if docs/reference/ changes, so the committed reference always matches the code.

Usage: python scripts/build-docs.py        (needs `dotnet tool restore` once)

DocFX writes the Markdown; this script then tidies it for GitHub:
- adds each namespace's overview from docs/namespaces/<namespace>.md (DocFX only applies those to the HTML site);
- turns DocFX <xref> tags, which GitHub can't render, into links to the right page;
- replaces commit hashes in links to other repos with the branch name, and drops the 'Inherited Members' lists
  (from .NET and Dalamud), so the files only change when this code does;
- writes docs/reference/README.md, an index of the namespaces.
"""
import pathlib
import re
import shutil
import subprocess
import sys

ROOT = pathlib.Path(__file__).resolve().parent.parent
DOCS = ROOT / "docs"
REF = DOCS / "reference"


def namespace_overview(ns: str) -> str:
    """Returns the body of docs/namespaces/<ns>.md without its YAML header, or '' if there is none."""
    path = DOCS / "namespaces" / f"{ns}.md"
    if not path.exists():
        return ""
    text = path.read_text(encoding="utf-8")
    return re.sub(r"\A---.*?---\s*", "", text, flags=re.S).strip()


def xref_to_link(match: re.Match, pages: set[str]) -> str:
    """Turns <xref href="Cube.Plugin.Dispose"> into a link to the page that documents it."""
    uid = match.group(1)
    parts = uid.split("(")[0].split(".")
    for i in range(len(parts), 0, -1):
        page = ".".join(parts[:i])
        if page in pages:
            anchor = re.sub(r"[^A-Za-z0-9]", "_", uid.split("(")[0])
            return f"[{parts[-1]}]({page}.md#{anchor})"
    return f"`{uid}`"


def main() -> int:
    if REF.exists():
        shutil.rmtree(REF)
    result = subprocess.run(["dotnet", "docfx", "metadata", str(DOCS / "docfx.reference.json")], cwd=ROOT)
    if result.returncode != 0:
        return result.returncode

    pages = {p.stem for p in REF.glob("*.md")}
    namespaces = []
    for path in sorted(REF.glob("*.md")):
        text = path.read_text(encoding="utf-8")
        text = re.sub(r'<xref href="([^"]+)"[^>]*></xref>', lambda m: xref_to_link(m, pages), text)
        # DocFX fills the ref in these links from *this* repo's checkout (a commit hash locally, "40/merge" on
        # a PR build), so normalise whatever it is to master.
        text = re.sub(r"(https://github\.com/goatcorp/Dalamud/blob/).+?/(Dalamud/)", r"\1master/\2", text)
        # Inherited members come from .NET and Dalamud, not this code, and change with the Dalamud version CI
        # downloads, so they'd make the committed reference go stale for no reason. Drop those sections.
        text = re.sub(r"#### Inherited Members\n.*?(?=\n#{1,4} |\Z)", "", text, flags=re.S)
        heading = re.match(r"# <a id=\"[^\"]*\"></a> Namespace (\S+)", text)
        if heading:
            ns = heading.group(1)
            overview = namespace_overview(ns)
            namespaces.append((ns, overview))
            if overview:
                text = text.replace(heading.group(0), heading.group(0) + "\n\n" + overview, 1)
        path.write_text(text.replace("\r\n", "\n").rstrip() + "\n", encoding="utf-8", newline="\n")

    index = ["# Code reference", "",
             "Generated from the code's XML doc comments by `scripts/build-docs.py`. Don't edit these files;",
             "improve the comments in the code instead.", ""]
    for ns, overview in namespaces:
        index.append(f"## [{ns}]({ns}.md)\n\n{overview}\n")
    (REF / "README.md").write_text("\n".join(index).rstrip() + "\n", encoding="utf-8", newline="\n")
    (REF / "toc.yml").unlink(missing_ok=True)
    return 0


if __name__ == "__main__":
    sys.exit(main())
