
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

Generate interface definition file (IDL) from type library:

 * Launch OleView.exe from an elevated command prompt
 * File -> View TypeLib, then select mstscax.dll
 * File -> Save As..., use file name mstscax.idl

The resulting .IDL file needs to be manually edited to fix type declaration ordering before midl.exe can compile it again into a .tlb file.

```powershell
midl.exe .\mstscax.idl /notlb /header mstscax.h /iid mstscax_i.c
```
