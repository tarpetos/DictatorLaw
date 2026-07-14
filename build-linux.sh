#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$ROOT_DIR"

BANNERLORD_DIR="${BANNERLORD_DIR:-}"
if [ -z "$BANNERLORD_DIR" ]; then
  echo "Error: BANNERLORD_DIR environment variable is not set." >&2
  echo "Usage: BANNERLORD_DIR=\"/path/to/game\" ./build-linux.sh" >&2
  exit 1
fi

HARMONY_VERSION="2.2.2"
HARMONY_DIR="$ROOT_DIR/.build-tools/harmony"
HARMONY_DLL="$HARMONY_DIR/0Harmony.dll"

if [ ! -f "$HARMONY_DLL" ]; then
  echo "Downloading Harmony v$HARMONY_VERSION for build..."
  mkdir -p "$HARMONY_DIR"
  curl -sSL -o "$HARMONY_DIR/harmony.zip" "https://www.nuget.org/api/v2/package/Lib.Harmony/$HARMONY_VERSION"
  unzip -q -o "$HARMONY_DIR/harmony.zip" "lib/net472/0Harmony.dll" -d "$HARMONY_DIR"
  mv "$HARMONY_DIR/lib/net472/0Harmony.dll" "$HARMONY_DLL"
  rm -rf "$HARMONY_DIR/lib" "$HARMONY_DIR/harmony.zip"
fi

MCM_VERSION="5.9.2"
MCM_DIR="$ROOT_DIR/.build-tools/mcm"
MCM_DLL="$MCM_DIR/MCMv5.dll"

if [ ! -f "$MCM_DLL" ]; then
  echo "Downloading MCM v$MCM_VERSION for build..."
  mkdir -p "$MCM_DIR"
  curl -sSL -o "$MCM_DIR/mcm.zip" "https://www.nuget.org/api/v2/package/Bannerlord.MCM/$MCM_VERSION"
  unzip -q -o "$MCM_DIR/mcm.zip" "lib/netstandard2.0/MCMv5.dll" -d "$MCM_DIR"
  mv "$MCM_DIR/lib/netstandard2.0/MCMv5.dll" "$MCM_DLL"
  rm -rf "$MCM_DIR/lib" "$MCM_DIR/mcm.zip"
fi

BUILD_TOOL="msbuild"
if ! command -v msbuild &> /dev/null; then
    BUILD_TOOL="xbuild"
fi

$BUILD_TOOL DictatorLaw.csproj /t:Rebuild /p:Configuration=Release "/p:BannerlordInstallDir=$BANNERLORD_DIR"

rm -rf dist
mkdir -p dist/DictatorLaw/bin/Win64_Shipping_Client dist/DictatorLaw/ModuleData
cp "bin/Release/DictatorLaw.dll" "dist/DictatorLaw/bin/Win64_Shipping_Client/"

cp SubModule.xml dist/DictatorLaw/
cp -R ModuleData/. dist/DictatorLaw/ModuleData/

echo "Built self-contained dist/DictatorLaw"