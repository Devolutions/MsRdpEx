# AGENTS.md

This file contains repo-specific guidance for *any* AI agent (Copilot, Claude, Cursor, etc.) working in this repository.

## What this repo is

**MsRdpEx** (“Microsoft RDP Extensions”) is a Windows-focused project that:

- Builds a native hooking DLL: `MsRdpEx.dll` (Detours-based)
- Builds launcher EXEs: `mstscex.exe`, `msrdcex.exe`, `vmconnectex.exe`
- Includes an optional .NET package: `Devolutions.MsRdpEx` (NuGet)
- Includes an MSI installer built with WiX v4

Key folders:

- `dll/`: native DLL implementation
- `exe/`: native launcher EXEs
- `channels/`: DVC-related code (e.g., `DvcServer`)
- `dotnet/`: C# projects + NuGet packaging
- `installer/`: WiX v4 project + sources
- `dependencies/detours/`: **prebuilt** Detours package used by CMake
- `scripts/`: PowerShell build helpers (notably Detours regeneration)

## Agent operating rules (keep diffs safe)

- Prefer **small, targeted** changes. Avoid mass reformatting or mechanical renames.
- Don’t change packaging/versioning unless the task explicitly asks for it.
- Treat any credential-like strings as secrets:
  - Do not add logging that could capture passwords/tokens.
  - The `.RDP` options `ClearTextPassword`/`GatewayPassword` are “testing only”; avoid persisting them.
- Avoid editing `.github/workflows/*` unless explicitly requested (they’re owned by DevOps).
- Keep Windows compatibility in mind (this is not a cross-platform repo).

## Build prerequisites (local)

Typical local prerequisites:

- Windows 10/11
- Visual Studio 2022 (MSVC v143) or newer with “Desktop development with C++”
- CMake (Visual Studio generator)
- PowerShell 7 (`pwsh`) for scripts
- For managed builds: .NET SDK 8.x (and .NET Framework targeting packs if building `net48`)

## Build: native (DLL + EXEs)

CMake expects a **prebuilt Detours package** in `dependencies/detours/`.

Example x64 build:

```powershell
cmake -G "Visual Studio 17 2022" -A x64 -DWITH_DOTNET=OFF -B build-x64
cmake --build build-x64 --config Release
```

Outputs are placed under the build directory (e.g. `build-x64/Release/`).

Architectures:

- `-A Win32` (x86)
- `-A x64`
- `-A ARM64`

## Detours dependency (important)

This repo *commits* a prebuilt Detours layout under `dependencies/detours/` and ignores other `dependencies/*` content.

- `dependencies/*` is ignored by `.gitignore`
- `dependencies/detours/` is explicitly **not** ignored (it’s meant to be present)
- `dependencies/sources/` is ignored (Detours source clone lives here)

If Detours is missing or needs updating, regenerate it from the repo root:

```powershell
pwsh -ExecutionPolicy Bypass -File .\scripts\detours-all.ps1
```

Notes:

- `scripts/detours.ps1` expects a Visual Studio developer environment (via `VSCMD_ARG_TGT_ARCH`).
- `scripts/detours-all.ps1` can optionally install `VsDevShell` for the current user (`-InstallVsDevShell`).

## Build: managed (NuGet package)

The .NET build is wired through CMake as *external MSBuild projects*.

Managed-only CMake build (used by CI):

```powershell
cmake -G "Visual Studio 17 2022" -A x64 -DWITH_DOTNET=ON -DWITH_NATIVE=OFF -B build-dotnet
cmake --build build-dotnet --config Release
dotnet pack .\dotnet\Devolutions.MsRdpEx -o package
```

Packaging note:

- The NuGet package includes native `MsRdpEx.dll` for `win-x86`, `win-x64`, `win-arm64` from `dependencies/MsRdpEx/<arch>/MsRdpEx.dll`.
- If you’re producing a NuGet package locally, ensure those native binaries are present (CI populates them by building native per-arch first).

## Build: MSI installer (WiX v4)

The MSI build consumes the native binaries from `dependencies/MsRdpEx/<arch>/` and is driven by `installer/MsRdpEx.sln`.

CI-style build (per arch):

```powershell
# x64 example
$platform = 'x64' # Win32, x64, ARM64
dotnet build /p:Configuration=Release /p:Platform=$platform installer/MsRdpEx.sln
```

The workflow also rewrites `installer/Variables.wxi` to set `ProductVersion` (MSI short version) during packaging. Avoid manual edits unless asked.

## Versioning (very important)

- The canonical version is `dotnet/Devolutions.MsRdpEx/Devolutions.MsRdpEx.csproj` `<Version>`.
- The top-level `CMakeLists.txt` parses that `<Version>` to define `MSRDPEX_VERSION`.
- CI may rewrite `<Version>` during release packaging.

Guideline for agents: **don’t bump `<Version>`** unless the task explicitly requests it.

## COM interop / generated artifacts

The `com/` folder contains generated artifacts related to `mstscax.dll` / `rdclientax.dll` interop.

- Follow `com/README.md` if you need to regenerate `.tlh/.tli`, `mstscax.idl`, or C# interop assemblies.
- Prefer not to hand-edit generated outputs unless regeneration is part of the task.

## Debugging / runtime notes

Extended logging is controlled via environment variables (examples):

```powershell
$Env:MSRDPEX_LOG_ENABLED="1"
$Env:MSRDPEX_LOG_LEVEL="DEBUG"  # TRACE is extremely verbose
```

Default log location is under `%LocalAppData%\MsRdpEx\MsRdpEx.log` unless overridden.

## If you change build/packaging behavior

When a task touches build logic, try to validate locally:

- Native: `cmake --build <builddir> --config Release`
- Managed: `dotnet pack .\dotnet\Devolutions.MsRdpEx -o package`

If you can’t run a full build (missing toolchain), keep the changes minimal and explain what should be validated by a human (arch matrix, MSI packaging, etc.).
