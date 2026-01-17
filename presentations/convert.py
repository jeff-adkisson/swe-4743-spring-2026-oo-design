#!/usr/bin/env python3
#
# =============================================================================
# convert.py
# PPTX -> HTML + PDF batch converter (macOS) and README.md updater
# =============================================================================
#
# USAGE
# -----
# From the presentations directory:
#   python3.12 convert.py
#
# Prerequisites (one-time):
#   - Python 3.12 (required, not 3.13)
#   - LibreOffice (soffice CLI)
#   - This script will create .venv-pptxhtml and install pptx-to-html5
#
# NOTES
# -----
# - Safe to re-run: outputs are overwritten and README is regenerated.
# - Animations are intentionally removed (static HTML + PDF output).
#
# =============================================================================

from __future__ import annotations

import glob
import os
from pathlib import Path
import shutil
import subprocess
import sys
import tempfile


VENV_DIR = Path(".venv-pptxhtml")
PYTHON_BIN = "python3.12"
README_PATH = Path("..") / "README.md"


def run(cmd: list[str], *, check: bool = True) -> subprocess.CompletedProcess[str]:
    # Run a subprocess command with optional error checking.
    return subprocess.run(cmd, check=check, text=True)


def ensure_python312() -> None:
    # Verify python3.12 is available on PATH.
    if shutil.which(PYTHON_BIN) is None:
        print("ERROR: python3.12 not found.")
        print("Install it with: brew install python@3.12")
        sys.exit(1)


def ensure_venv() -> Path:
    # Create the virtual environment if missing.
    if not VENV_DIR.is_dir():
        print(f"Creating virtual environment: {VENV_DIR}")
        run([PYTHON_BIN, "-m", "venv", str(VENV_DIR)])
    return VENV_DIR


def venv_bin_path(name: str) -> Path:
    # Resolve a binary path inside the venv.
    return VENV_DIR / "bin" / name


def ensure_pip_up_to_date() -> None:
    # Upgrade pip inside the venv.
    venv_python = venv_bin_path("python")
    run([str(venv_python), "-m", "pip", "install", "--upgrade", "pip"], check=True)


def ensure_pptx_to_html() -> Path:
    # Install or validate pptx-to-html5 in the venv.
    pptx_to_html = venv_bin_path("pptx-to-html")
    if not pptx_to_html.is_file():
        print("Installing pptx-to-html5...")
        venv_pip = venv_bin_path("pip")
        run([str(venv_pip), "install", "pptx-to-html5"], check=True)
    if not pptx_to_html.is_file():
        print("ERROR: pptx-to-html not found in venv after install.")
        sys.exit(1)
    return pptx_to_html


def ensure_soffice() -> None:
    # Verify LibreOffice CLI is available for PDF export.
    if shutil.which("soffice") is None:
        print("ERROR: LibreOffice (soffice) not found.")
        print("Install it with: brew install --cask libreoffice")
        sys.exit(1)


def cleanup_temp_output() -> None:
    # Remove stray temp HTML output folders.
    for path in glob.glob("~$*_html"):
        try:
            shutil.rmtree(path)
        except FileNotFoundError:
            continue


def convert_pptx(pptx_to_html: Path) -> None:
    # Convert all PPTX files to HTML and PDF, overwriting outputs.
    pptx_files = sorted(Path(".").glob("*.pptx"))
    for pptx in pptx_files:
        if pptx.name.startswith("~$"):
            print(f"Skipping temp file: {pptx.name}")
            continue

        base = pptx.stem
        outdir = Path(f"{base}_html")

        print(f"Converting {pptx.name} -> HTML ({outdir}/) (overwrite)")
        shutil.rmtree(outdir, ignore_errors=True)
        outdir.mkdir(parents=True, exist_ok=True)
        run([str(pptx_to_html), str(pptx), "-o", str(outdir)])

        print(f"Converting {pptx.name} -> PDF (overwrite)")
        run(["soffice", "--headless", "--convert-to", "pdf", str(pptx)])



def title_from_base(base: str) -> tuple[str, str]:
    # Derive the deck number and display title from a filename stem.
    if "-" in base:
        number, remainder = base.split("-", 1)
    else:
        number, remainder = base, base
    words = remainder.replace("-", " ").split()
    title = " ".join(word[:1].upper() + word[1:] for word in words)
    return number, title


def update_readme() -> None:
    # Rewrite the README Presentations section with current PPTX outputs.
    if not README_PATH.is_file():
        print(f"ERROR: README not found at {README_PATH}")
        sys.exit(1)

    content = README_PATH.read_text(encoding="utf-8").splitlines()
    new_lines: list[str] = []
    found = False
    for line in content:
        if line.strip() == "## Presentations":
            found = True
            break
        new_lines.append(line)

    if new_lines and new_lines[-1] != "":
        new_lines.append("")
    elif not new_lines:
        new_lines.append("")

    new_lines.append("## Presentations")
    new_lines.append("")

    for pptx in sorted(Path(".").glob("*.pptx")):
        base = pptx.stem
        number, title = title_from_base(base)
        new_lines.append(f"#### {number}. {title}")
        new_lines.append(f"- [PowerPoint](presentations/{base}.pptx)")
        new_lines.append(f"- [PDF](presentations/{base}.pdf)")
        new_lines.append(f"- [HTML](presentations/{base}_html/index.html)")
        links_md = Path(f"{base}_links.md")
        if links_md.is_file():
            links_lines = links_md.read_text(encoding="utf-8").splitlines()
            new_lines.extend(links_lines)
        new_lines.append("")

    # Write atomically to keep README consistent if interrupted.
    with tempfile.NamedTemporaryFile("w", delete=False, encoding="utf-8") as tmp:
        tmp.write("\n".join(new_lines))
        tmp_path = Path(tmp.name)

    tmp_path.replace(README_PATH)
    if found:
        print(f"Updated {README_PATH} (## Presentations rewritten).")
    else:
        print(f"Added {README_PATH} (## Presentations added).")


def main() -> int:
    # Orchestrate environment setup, conversion, and README update.
    print("== PPTX -> HTML + PDF batch conversion ==")

    script_dir = Path(__file__).resolve().parent
    os.chdir(script_dir)

    ensure_python312()
    ensure_venv()
    ensure_pip_up_to_date()
    pptx_to_html = ensure_pptx_to_html()
    ensure_soffice()

    cleanup_temp_output()
    convert_pptx(pptx_to_html)
    print("Conversions complete.")

    update_readme()
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
