param(
    [string] $AssemblyFilePath,
    [string] $FrameworkName = ".NETFramework,Version=v4.8",
    [string] $FrameworkDisplayName = ".NET Framework 4.8"
)

if (-Not (Test-Path $AssemblyFilePath)) {
    throw "Assembly file not found at the specified path: $AssemblyFilePath"
}

$ilasm = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\ilasm.exe"
$ildasm = "C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\x64\ildasm.exe"

if (-Not (Test-Path $ilasm)) {
    throw "ilasm.exe not found at the path: $ilasm"
}

if (-Not (Test-Path $ildasm)) {
    throw "ildasm.exe not found at the path: $ildasm"
}

if (-Not (Get-Command -Name 'rg' -ErrorAction SilentlyContinue)) {
    throw "rg (RipGrep) not found"
}

$AssemblyName = [System.IO.Path]::GetFileNameWithoutExtension($AssemblyFilePath)
$AssemblyDllFile = $AssemblyFilePath
$AssemblyIlfile = "${AssemblyName}.il"
$AssemblyResfile = "${AssemblyName}.res"
& $ildasm "/OUT=$AssemblyIlfile" /NOBAR $AssemblyDllFile

# https://learn.microsoft.com/en-us/dotnet/api/system.runtime.versioning.targetframeworkattribute

# 01 00
# 1A (26)
# 2E 4E 45 54 46 72 61 6D 65 77 6F 72 6B 2C 56 65 72 73 69 6F 6E 3D 76 34 2E 38 ".NETFramework,Version=v4.8"
# 01 00 54 0E
# 14 (20)
# 46 72 61 6D 65 77 6F 72 6B 44 69 73 70 6C 61 79 4E 61 6D 65 "FrameworkDisplayName"
# 12 (18)
# 2E 4E 45 54 20 46 72 61 6D 65 77 6F 72 6B 20 34 2E 38 ".NET Framework 4.8"

function Convert-StringToByteArray {
    param(
        [Parameter(Mandatory=$true,Position=0)]
        [string] $InputString
    )

    if ($InputString.Length -gt 255) {
        throw "String length exceeds the maximum limit for a single byte."
    }

    $lengthByte = [byte]$InputString.Length
    $utf8Bytes = [System.Text.Encoding]::UTF8.GetBytes($InputString)
    return , $lengthByte + $utf8Bytes
}

$ctorBytes = @(0x01, 0x00)
$ctorBytes = $ctorBytes + (Convert-StringToByteArray $FrameworkName)
$ctorBytes = $ctorBytes + @(0x01, 0x00, 0x54, 0x0E)
$ctorBytes = $ctorBytes + (Convert-StringToByteArray "FrameworkDisplayName")
$ctorBytes = $ctorBytes + (Convert-StringToByteArray $FrameworkDisplayName)
$ctorHex = ($ctorBytes | ForEach-Object { $_.ToString("X2") }) -join " "

$lineToAdd = "  .custom instance void [mscorlib]System.Runtime.Versioning.TargetFrameworkAttribute::.ctor(string) = ( $CtorHex )"
$lineAfter = "  .hash algorithm"
$NewContent = rg "$lineAfter" $AssemblyIlfile -r "$lineToAdd`r`n$lineAfter" -N --passthru
Set-Content -Path $AssemblyIlfile -Value $NewContent -Force

& $ilasm $AssemblyIlfile "/OUTPUT=$AssemblyDllfile" /DLL "/RESOURCE=$AssemblyResfile"
@($AssemblyIlfile, $AssemblyResfile) | Remove-Item -Force -ErrorAction SilentlyContinue
