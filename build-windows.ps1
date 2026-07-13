param (
    [string]$BannerlordDir = "C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord"
)

$ErrorActionPreference = "Stop"
$RootDir =$PSScriptRoot
Set-Location $RootDir

$HarmonyDir = Join-Path$RootDir "vendor\extracted\Modules\Bannerlord.Harmony\bin\Win64_Shipping_Client"
$HarmonyDll = Join-Path$HarmonyDir "0Harmony.dll"

if (-Not (Test-Path $HarmonyDll)) {
    Write-Error "Missing Bannerlord.Harmony dependency files. Restore vendor\extracted or download the packaged release."
    exit 1
}

msbuild DictatorLaw.csproj /t:Rebuild /p:Configuration=Release "/p:BannerlordInstallDir=$BannerlordDir"

if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed"
    exit $LASTEXITCODE
}

$DistDir = Join-Path$RootDir "dist\DictatorLaw"
if (Test-Path $DistDir) { Remove-Item -Recurse -Force$DistDir }
$BinDir = Join-Path$DistDir "bin\Win64_Shipping_Client"
New-Item -ItemType Directory -Force -Path $BinDir | Out-Null
$ModuleDataDir = Join-Path$DistDir "ModuleData"
New-Item -ItemType Directory -Force -Path $ModuleDataDir | Out-Null

Copy-Item (Join-Path $RootDir "bin\Release\DictatorLaw.dll") -Destination $BinDir

$Assemblies = @("0Harmony.dll", "Mono.Cecil.dll", "Mono.Cecil.Mdb.dll", "Mono.Cecil.Pdb.dll", "Mono.Cecil.Rocks.dll", "MonoMod.Core.dll", "MonoMod.Backports.dll", "MonoMod.Iced.dll", "MonoMod.ILHelpers.dll", "MonoMod.Utils.dll", "System.ValueTuple.dll")
foreach ($assembly in$Assemblies) {
    Copy-Item (Join-Path $HarmonyDir $assembly) -Destination$BinDir
}

Copy-Item (Join-Path $RootDir "SubModule.xml") -Destination $DistDir
Copy-Item (Join-Path $RootDir "ModuleData\*") -Destination $ModuleDataDir -Recurse

Write-Host "Built self-contained dist\DictatorLaw"