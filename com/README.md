
Open a Visual Studio Command Prompt with PowerShell.

Copy mstscax.dll (or rdclientax.dll) in the current directory:

```powershell
cp "$Env:SystemRoot\System32\mstscax.dll" .
```

Use the MSVC compiler built-in type library importer:

```powershell
echo '#import "mstscax.dll"  named_guids' > import.cpp
cl.exe /c /nologo .\import.cpp && rm .\import.*
(Get-Content mstscax.tlh | Select-Object -Skip 7) | Set-Content mstscax.tlh
(Get-Content mstscax.tli | Select-Object -Skip 7) | Set-Content mstscax.tli
```

Generate C# import library:

```powershell
AxImp.exe /source .\mstscax.dll
(Get-Content AxMSTSCLib.cs | Select-Object -Skip 10) | Set-Content AxMSTSCLib.cs
```
