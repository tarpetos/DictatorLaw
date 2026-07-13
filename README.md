# Dictator Law

Single-player module for Mount & Blade II: Bannerlord **v1.4.7**. It adds the
`Dictator Law` kingdom policy for the player's kingdom.

## What it does

- Registers the policy through Bannerlord's campaign object API; no policy XML
  is loaded alongside it, so the policy cannot be registered twice.
- While the policy is active, only the ruling clan can make kingdom-decision
  proposals.
- Prevents vassal armies at the source: the mod stops Bannerlord's
  `Kingdom.CreateArmy` action before an army object is created. The ruling clan
  can still create armies normally.

The module affects only `Clan.PlayerClan.Kingdom`. NPC kingdoms are unchanged.

## Release contents

GitHub releases contain a `DictatorLaw` module folder. Extract that folder into
Bannerlord's `Modules` directory and enable it in the launcher. Source builds
produce the same layout in `dist/DictatorLaw`.

## Compatibility and dependencies

- Target game version: Bannerlord v1.4.7.
- Single-player only.
- Harmony is bundled in `bin/Win64_Shipping_Client`; do not install a separate
  copy for this module.
- Existing saves are supported. Enable the module, load a campaign, then use
  the kingdom policies screen to enact Dictator Law.

## Build from WSL/Linux

You must provide the path to your Bannerlord installation using the `BANNERLORD_DIR` environment variable. With Mono
installed, run:

```bash
chmod +x build-linux.sh
BANNERLORD_DIR="/path/to/Mount & Blade II Bannerlord" ./build-linux.sh
```

**Important note for WSL users:** Windows restricts executing `.dll` files generated or copied from WSL environments (
Mark of the Web issue). After copying the module to your game directory on Windows, you must unblock the files. Open
PowerShell and run:

```powershell
Get-ChildItem -Path "C:\Your\Path\To\Mount & Blade II Bannerlord\Modules\DictatorLaw" -Recurse | Unblock-File
```

## Build from Windows

Run the PowerShell script. If your game is not in the default Steam directory, specify the `-BannerlordDir` parameter:

```powershell
.\build-windows.ps1 -BannerlordDir "\path\to\Mount & Blade II Bannerlord"
```

## Install

Copy the contents of `dist/DictatorLaw` to:

```text
Mount & Blade II Bannerlord/Modules/DictatorLaw
```

Then enable **DictatorLaw** in the Bannerlord launcher, after the official
single-player modules. The build script packages only the runtime DLLs required
by this module; source and build artifacts are not installed.