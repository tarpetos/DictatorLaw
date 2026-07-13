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

HARMONY_DIR="$ROOT_DIR/vendor/extracted/Modules/Bannerlord.Harmony/bin/Win64_Shipping_Client"
HARMONY_DLL="$HARMONY_DIR/0Harmony.dll"

if [ ! -f "$HARMONY_DLL" ]; then
  echo "Missing Bannerlord.Harmony dependency files. Restore vendor/extracted or download the packaged release." >&2
  exit 1
fi

BUILD_TOOL="msbuild"
if ! command -v msbuild &> /dev/null; then
    BUILD_TOOL="xbuild"
fi

$BUILD_TOOL DictatorLaw.csproj /t:Rebuild /p:Configuration=Release "/p:BannerlordInstallDir=$BANNERLORD_DIR"

rm -rf dist
mkdir -p dist/DictatorLaw/bin/Win64_Shipping_Client dist/DictatorLaw/ModuleData
cp bin/Release/DictatorLaw.dll dist/DictatorLaw/bin/Win64_Shipping_Client/

for assembly in 0Harmony.dll Mono.Cecil.dll Mono.Cecil.Mdb.dll Mono.Cecil.Pdb.dll Mono.Cecil.Rocks.dll MonoMod.Core.dll MonoMod.Backports.dll MonoMod.Iced.dll MonoMod.ILHelpers.dll MonoMod.Utils.dll System.ValueTuple.dll; do
  cp "$HARMONY_DIR/$assembly" dist/DictatorLaw/bin/Win64_Shipping_Client/
done

cp SubModule.xml dist/DictatorLaw/
cp -R ModuleData/. dist/DictatorLaw/ModuleData/

echo "Built self-contained dist/DictatorLaw"