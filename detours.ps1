
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

$TargetArchs = @('x86','x64','arm64')
$TargetConfs = @('Release', 'Debug')

foreach ($TargetArch in $TargetArchs) {
    foreach ($TargetConf in $TargetConfs) {
        $Env:DETOURS_TARGET_PROCESSOR="$($TargetArch.ToUpper())"
        $Env:DETOURS_CONFIG="$TargetConf"
        & nmake clean
        & nmake
    }
}

Set-Location ".."

$PkgDir = "$RepoRoot/dependencies/detours"
Remove-Item -Path $PkgDir -Recurse -ErrorAction SilentlyContinue | Out-Null
New-Item -ItemType Directory -Path $PkgDir -ErrorAction SilentlyContinue | Out-Null
New-Item -ItemType Directory -Path "$PkgDir/lib" -ErrorAction SilentlyContinue | Out-Null

Copy-Item "include" -Destination "$PkgDir/include" -Exclude @("syelog.h") -Recurse

foreach ($TargetArch in $TargetArchs) {
    foreach ($TargetConf in $TargetConfs) {
        $LibDir = "lib." + $TargetArch.ToUpper() + $TargetConf
        New-Item -ItemType Directory -Path "$PkgDir/lib/$TargetArch" -ErrorAction SilentlyContinue | Out-Null
        Copy-Item $LibDir -Destination "$PkgDir/lib/$TargetArch/$TargetConf" -Recurse
    }
}

Pop-Location
