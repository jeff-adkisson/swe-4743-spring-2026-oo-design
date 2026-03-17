#!/usr/bin/env bash

set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$ROOT_DIR"

SITE_URL="http://127.0.0.1:4000/"
LOG_FILE="${TMPDIR:-/tmp}/jekyll-serve.log"

echo "Installing bundle dependencies..."
bundle install

if lsof -iTCP:4000 -sTCP:LISTEN -n -P >/dev/null 2>&1; then
  echo "Jekyll appears to already be running on port 4000."
else
  echo "Starting Jekyll server..."
  bundle exec jekyll serve --livereload >"$LOG_FILE" 2>&1 &
  JEKYLL_PID=$!

  for _ in {1..60}; do
    if curl -fsS "$SITE_URL" >/dev/null 2>&1; then
      break
    fi
    sleep 1
  done

  if ! curl -fsS "$SITE_URL" >/dev/null 2>&1; then
    echo "Jekyll did not start successfully."
    echo "Check the log at: $LOG_FILE"
    kill "$JEKYLL_PID" >/dev/null 2>&1 || true
    exit 1
  fi

  echo "Jekyll started with PID $JEKYLL_PID."
  echo "Server log: $LOG_FILE"
fi

echo "Opening $SITE_URL"
open "$SITE_URL"
