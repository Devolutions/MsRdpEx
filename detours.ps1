
$RepoRoot = $PSScriptRoot

$SourcesDir = "$RepoRoot/dependencies/sources"
New-Item -ItemType Directory -Path $SourcesDir -ErrorAction SilentlyContinue | Out-Null

Push-Location
Set-Location $SourcesDir

if (-Not (Test-Path -Path "detours")) {
    & 'git' 'clone' 'https://github.com/microsoft/Detours' 'detours'
}

Set-Location "detours/src"

& 'git' 'checkout' '45a76a3' # August 17, 2021

if (-Not (Test-Path Env:VSCMD_ARG_TGT_ARCH)) {
    throw "VSCMD_ARG_TGT_ARCH is not set, use a Visual Studio developer shell"
}

Write-Host "Building Detours for $Env:VSCMD_ARG_TGT_ARCH"

$TargetArchs = @($Env:VSCMD_ARG_TGT_ARCH)
$TargetConfs = @('Release', 'Debug')

foreach ($TargetArch in $TargetArchs) {
    $Env:VSCMD_ARG_TGT_ARCH="$TargetArch"
    $Env:DETOURS_TARGET_PROCESSOR="$($TargetArch.ToUpper())"
    & nmake clean
    foreach ($TargetConf in $TargetConfs) {
        $Env:DETOURS_CONFIG="$TargetConf"
        & nmake
    }
}

Set-Location ".."

$PkgDir = "$RepoRoot/dependencies/detours"
New-Item -ItemType Directory -Path $PkgDir -ErrorAction SilentlyContinue | Out-Null
New-Item -ItemType Directory -Path "$PkgDir/lib" -ErrorAction SilentlyContinue | Out-Null

Remove-Item -Path "$PkgDir/include" -Recurse -ErrorAction SilentlyContinue | Out-Null
Copy-Item "include" -Destination "$PkgDir/include" -Exclude @("syelog.h") -Recurse

foreach ($TargetArch in $TargetArchs) {
    Remove-Item -Path "$PkgDir/lib/$TargetArch" -Recurse -ErrorAction SilentlyContinue | Out-Null
    New-Item -ItemType Directory -Path "$PkgDir/lib/$TargetArch" -ErrorAction SilentlyContinue | Out-Null
    foreach ($TargetConf in $TargetConfs) {
        $LibDir = "lib." + $TargetArch.ToUpper() + $TargetConf
        Write-Host "Copying $LibDir to "$PkgDir/lib/$TargetArch/$TargetConf""
        New-Item -ItemType Directory -Path "$PkgDir/lib/$TargetArch/$TargetConf" | Out-Null
        Copy-Item "$LibDir/*" -Destination "$PkgDir/lib/$TargetArch/$TargetConf" -Recurse
    }
}

Pop-Location
