#!/usr/bin/env bash
#
# =============================================================================
# ./convert.sh
# PPTX → HTML + PDF Batch Converter (macOS)
# and README.md Updater for Presentations section
# =============================================================================
#
# DESCRIPTION
# ----------
# This script converts all PowerPoint (.pptx) files in the current
# `presentations/` directory into:
#
#   1. HTML presentations (one folder per PPTX)
#   2. PDF files (one PDF per PPTX)
#
# It also regenerates the "## Presentations" section of the parent README.md
# to include links to:
#   - the original PowerPoint
#   - the generated PDF
#   - the generated HTML
#
# The script is SAFE TO RE-RUN. Every execution:
#   - overwrites existing HTML folders
#   - overwrites existing PDFs
#   - fully regenerates the README "## Presentations" section
#
#
# DIRECTORY ASSUMPTIONS
# ---------------------
# Repository layout must look like:
#
#   repo-root/
#   ├── README.md
#   └── presentations/
#       ├── *.pptx
#       ├── convert.sh   (this script)
#       └── .venv-pptxhtml/  (auto-created)
#
#
# PREREQUISITES (ONE-TIME SETUP)
# ------------------------------
#
# 1) Homebrew (macOS package manager)
#    https://brew.sh
#
# 2) Python 3.12 (required — NOT 3.13)
#    brew install python@3.12
#
# 3) LibreOffice (used for PPTX → PDF)
#    brew install --cask libreoffice
#
#
# WHAT THE SCRIPT AUTOMATICALLY HANDLES
# -------------------------------------
# - Creates a Python virtual environment (.venv-pptxhtml) if missing
# - Installs / upgrades pip
# - Installs pptx-to-html5 inside the venv
# - Skips Office temp files (~$*.pptx)
# - Cleans up stray "~$*_html" temp folders
# - Ensures README.md stays consistent with the deck files
#
#
# USAGE
# -----
# From the presentations directory:
#
#   chmod +x convert.sh
#   ./convert.sh
#
# Then open any generated deck:
#
#   open 01-course-introduction_html/index.html
#
#
# NOTES
# -----
# - Animations are intentionally removed (static HTML + PDF output)
# - HTML output is suitable for GitHub Pages
# - PDFs are suitable for LMS uploads
#
# =============================================================================

set -euo pipefail

# Always run from the script's directory (~/.../presentations)
cd "$(dirname "$0")"

VENV_DIR=".venv-pptxhtml"
PYTHON_BIN="python3.12"
PPTX_TO_HTML="$VENV_DIR/bin/pptx-to-html"

# Parent README (presentations is a subfolder; README.md is in the parent)
README_PATH="../README.md"

echo "== PPTX → HTML + PDF batch conversion =="

# --- Ensure Python 3.12 exists ---
if ! command -v "$PYTHON_BIN" >/dev/null 2>&1; then
  echo "ERROR: python3.12 not found."
  echo "Install it with: brew install python@3.12"
  exit 1
fi

# --- Create venv if missing ---
if [[ ! -d "$VENV_DIR" ]]; then
  echo "Creating virtual environment: $VENV_DIR"
  "$PYTHON_BIN" -m venv "$VENV_DIR"
fi

# --- Activate venv ---
# shellcheck disable=SC1090
source "$VENV_DIR/bin/activate"

# --- Upgrade pip ---
python -m pip install --upgrade pip >/dev/null

# --- Install pptx-to-html5 if missing ---
if [[ ! -x "$PPTX_TO_HTML" ]]; then
  echo "Installing pptx-to-html5..."
  pip install pptx-to-html5
fi

if [[ ! -x "$PPTX_TO_HTML" ]]; then
  echo "ERROR: pptx-to-html not found in venv after install."
  exit 1
fi

# --- Ensure LibreOffice CLI exists (for PDF export) ---
if ! command -v soffice >/dev/null 2>&1; then
  echo "ERROR: LibreOffice (soffice) not found."
  echo "Install it with: brew install --cask libreoffice"
  exit 1
fi

# Clean up any Office temp output folders from previous runs
rm -rf "~$"*"_html" 2>/dev/null || true

shopt -s nullglob

# --- Convert all PPTX (always overwrite outputs) ---
for pptx in *.pptx; do
  # Skip Office temp/lock files like "~$deck.pptx"
  [[ "$pptx" == "~$"* ]] && { echo "Skipping temp file: $pptx"; continue; }

  base="${pptx%.pptx}"
  outdir="${base}_html"

  echo "Converting $pptx → HTML ($outdir/) (overwrite)"
  rm -rf "$outdir"
  mkdir -p "$outdir"
  "$PPTX_TO_HTML" "$pptx" -o "$outdir"

  echo "Converting $pptx → PDF (overwrite)"
  soffice --headless --convert-to pdf "$pptx" >/dev/null
done

echo "✓ Conversions complete."

# --- Update README.md (rewrite the entire '## Presentations' section) ---
if [[ ! -f "$README_PATH" ]]; then
  echo "ERROR: README not found at $README_PATH"
  exit 1
fi

tmpfile="$(mktemp)"

# 1) Copy everything BEFORE '## Presentations' (remove that section entirely)
awk '
  BEGIN { found=0 }
  /^## Presentations[[:space:]]*$/ { found=1; exit }
  { print }
' "$README_PATH" > "$tmpfile"

# Ensure there's exactly one blank line before the new section
printf "\n" >> "$tmpfile"

# 2) Add a fresh '## Presentations' section at the end
printf "## Presentations\n\n" >> "$tmpfile"

# 3) For each PPTX, add the formatted entry
for pptx in $(ls -1 *.pptx | sort); do
  base="${pptx%.pptx}"

  number="${base%%-*}"
  remainder="${base#${number}-}"
title="$(echo "${remainder//-/ }" | awk '{ for (i=1; i<=NF; i++) { $i = toupper(substr($i,1,1)) substr($i,2) } print }')"

  printf -- "#### %s. %s\n" "$number" "$title" >> "$tmpfile"
  printf -- "- [PowerPoint](presentations/%s.pptx)\n" "$base" >> "$tmpfile"
  printf -- "- [PDF](presentations/%s.pdf)\n" "$base" >> "$tmpfile"
  printf -- "- [HTML](presentations/%s_html/index.html)\n\n" "$base" >> "$tmpfile"
done

# Replace README atomically
mv "$tmpfile" "$README_PATH"

echo "✓ Updated $README_PATH (## Presentations rewritten)."