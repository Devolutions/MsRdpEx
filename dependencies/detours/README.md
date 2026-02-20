# Detours (prebuilt package)

This folder contains a **prebuilt** copy of the Microsoft Detours library, packaged in the layout expected by the top-level CMake build.

Expected layout:

- `include/` (Detours headers)
- `lib/<arch>/<config>/detours.lib`
  - `<arch>`: `x86`, `x64`, `arm64`
  - `<config>`: `Debug`, `Release`

## Regenerating / updating

If this folder is missing (or you need to update Detours), run from the repo root:

```powershell
# Builds & packages x86 + x64 + arm64 into dependencies/detours
pwsh -ExecutionPolicy Bypass -File .\scripts\detours-all.ps1
```

The Detours source is cloned into `dependencies/sources/detours`.
