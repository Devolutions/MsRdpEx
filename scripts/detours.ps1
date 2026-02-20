param(
    [string] $GitRepo = 'https://github.com/microsoft/Detours',
    [string] $GitCommit = '4b8c659' # July 24, 2023
)

$RepoRoot = (Split-Path -Parent $PSScriptRoot)

$SourcesDir = "$RepoRoot/dependencies/sources"
New-Item -ItemType Directory -Path $SourcesDir -ErrorAction SilentlyContinue | Out-Null

Push-Location
Set-Location $SourcesDir

if (-Not (Test-Path -Path "detours")) {
    & 'git' 'clone' $GitRepo 'detours'
}

Set-Location "detours/src"

& 'git' 'checkout' $GitCommit

$ResolvedCommit = $null
try {
    $ResolvedCommit = (& 'git' 'rev-parse' 'HEAD' 2>$null).Trim()
} catch {
    $ResolvedCommit = $null
}

if (-not [string]::IsNullOrWhiteSpace($ResolvedCommit)) {
    Write-Host "Detours commit: requested=$GitCommit resolved=$ResolvedCommit"
} else {
    Write-Host "Detours commit: requested=$GitCommit"
}

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

$CommitToRecord = if (-not [string]::IsNullOrWhiteSpace($ResolvedCommit)) { $ResolvedCommit } else { $GitCommit }
Set-Content -LiteralPath "$PkgDir/git-commit.txt" -Value "$CommitToRecord`n" -Encoding ASCII

Remove-Item -Path "$PkgDir/include" -Recurse -ErrorAction SilentlyContinue | Out-Null
Copy-Item "include" -Destination "$PkgDir/include" -Exclude @("syelog.h") -Recurse

foreach ($TargetArch in $TargetArchs) {
    Remove-Item -Path "$PkgDir/lib/$TargetArch" -Recurse -ErrorAction SilentlyContinue | Out-Null
    New-Item -ItemType Directory -Path "$PkgDir/lib/$TargetArch" -ErrorAction SilentlyContinue | Out-Null
    foreach ($TargetConf in $TargetConfs) {
        $LibDir = "lib." + $TargetArch.ToUpper() + $TargetConf
        Write-Host "Copying $LibDir to $PkgDir/lib/$TargetArch/$TargetConf"
        New-Item -ItemType Directory -Path "$PkgDir/lib/$TargetArch/$TargetConf" | Out-Null
        Copy-Item "$LibDir/*" -Destination "$PkgDir/lib/$TargetArch/$TargetConf" -Recurse
    }
}

Pop-Location
