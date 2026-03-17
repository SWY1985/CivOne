#!/usr/bin/env bash

set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
PROJECT_PATH="$ROOT_DIR/runtime/sdl/CivOne.SDL.csproj"
OUTPUT_DIR="${1:-$ROOT_DIR/artifacts/publish}"
CONFIGURATION="${CONFIGURATION:-Release}"
SELF_CONTAINED="${SELF_CONTAINED:-false}"
RUNTIMES=(
  "win-x64"
  "win-arm64"
  "osx-x64"
  "osx-arm64"
  "linux-x64"
  "linux-arm64"
)

mkdir -p "$OUTPUT_DIR"

for runtime in "${RUNTIMES[@]}"; do
  echo "Publishing $runtime..."
  dotnet publish "$PROJECT_PATH" \
    -c "$CONFIGURATION" \
    -r "$runtime" \
    --self-contained "$SELF_CONTAINED" \
    -o "$OUTPUT_DIR/$runtime"
done
