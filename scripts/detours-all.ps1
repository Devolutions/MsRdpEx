#!/usr/bin/env pwsh

[CmdletBinding()]
param(
    [string] $GitRepo = 'https://github.com/microsoft/Detours',
    [string] $GitCommit = '4b8c659',
    [string[]] $Arch = @('x86', 'x64', 'arm64'),
    [switch] $InstallVsDevShell
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Escape-SingleQuote([string] $Value) {
    if ($null -eq $Value) { return '' }
    return ($Value -replace "'", "''")
}

function Invoke-DetoursBuildForArch {
    param(
        [Parameter(Mandatory = $true)]
        [string] $TargetArch,
        [Parameter(Mandatory = $true)]
        [string] $ShellExe,
        [Parameter(Mandatory = $true)]
        [string] $RepoRoot,
        [Parameter(Mandatory = $true)]
        [string] $DetoursScriptPath,
        [Parameter(Mandatory = $true)]
        [string] $GitRepo,
        [Parameter(Mandatory = $true)]
        [string] $GitCommit,
        [Parameter(Mandatory = $true)]
        [bool] $InstallVsDevShell
    )

    Write-Host "==> Detours: building for $TargetArch (commit: $GitCommit)" -ForegroundColor Cyan

    $repoRootEsc = Escape-SingleQuote $RepoRoot
    $detoursEsc = Escape-SingleQuote $DetoursScriptPath
    $gitRepoEsc = Escape-SingleQuote $GitRepo
    $gitCommitEsc = Escape-SingleQuote $GitCommit

    $ensureVsDevShell = if ($InstallVsDevShell) {
        "if (-not (Get-Module -ListAvailable -Name VsDevShell)) { Install-Module -Name VsDevShell -Scope CurrentUser -Force -AllowClobber }"
    } else {
        "if (-not (Get-Module -ListAvailable -Name VsDevShell)) { throw 'PowerShell module VsDevShell is not installed. Install it (Install-Module VsDevShell -Scope CurrentUser) or re-run scripts/detours-all.ps1 -InstallVsDevShell.' }"
    }

    $childLines = @(
        'Set-StrictMode -Version Latest',
        "`$ErrorActionPreference = 'Stop'",
        "Set-Location -LiteralPath '$repoRootEsc'",
        $ensureVsDevShell,
        'Import-Module -Name VsDevShell -ErrorAction Stop',
        "Enter-VsDevShell $TargetArch",
        "& '$detoursEsc' -GitRepo '$gitRepoEsc' -GitCommit '$gitCommitEsc'"
    )

    $childCommand = ($childLines -join "`n")
    $encoded = [Convert]::ToBase64String([Text.Encoding]::Unicode.GetBytes($childCommand))

    & $ShellExe -NoProfile -ExecutionPolicy Bypass -EncodedCommand $encoded

    $exitCode = $LASTEXITCODE
    if ($exitCode -ne 0) {
        throw "Detours build failed for $TargetArch (exit code: $exitCode)"
    }
}

$repoRoot = (Split-Path -Parent $PSScriptRoot)
$detoursScriptPath = Join-Path $PSScriptRoot 'detours.ps1'
if (-not (Test-Path -LiteralPath $detoursScriptPath)) {
    throw "Expected detours script at: $detoursScriptPath"
}

# Assume PowerShell 7 (pwsh). Prefer reusing the currently running pwsh binary.
$shellExe = $null
try {
    $shellExe = (Get-Process -Id $PID -ErrorAction Stop).Path
} catch {
    $shellExe = (Get-Command -Name 'pwsh' -ErrorAction Stop).Source
}

foreach ($targetArch in $Arch) {
    Invoke-DetoursBuildForArch -TargetArch $targetArch -ShellExe $shellExe -RepoRoot $repoRoot -DetoursScriptPath $detoursScriptPath -GitRepo $GitRepo -GitCommit $GitCommit -InstallVsDevShell ([bool]$InstallVsDevShell)
}

Write-Host "Detours build complete for: $($Arch -join ', ')" -ForegroundColor Green
Write-Host "Output directory: dependencies/detours" -ForegroundColor Green
