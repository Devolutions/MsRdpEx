# MsRdpEx DVC Plugin Extractor

## Overview

The DVC Plugin Extractor allows external RDP clients to access internal Dynamic Virtual Channel (DVC) plugins from `mstscax.dll` that are not publicly exported. It scans the loaded `mstscax.dll` module for internal function pointer tables and re-exports them through a clean API.

## Discovered Plugins

The scanner currently discovers **4 internal DVC plugins** from the common table:

| Index | Plugin Name | Description |
|-------|-------------|-------------|
| 0 | Graphics | Graphics pipeline DVC |
| 1 | Input | Input redirection |
| 2 | SNDOUTPUT | Audio output |
| 3 | Location | Location services |

Additional plugins in the dynamic table (16 entries) include: DisplayControl, RDCLIP, RDCAMERA, and others.

## API Functions

### Initialization

```c
HRESULT MsRdpEx_DvcPluginExtractor_Init(HMODULE hMstscax);
```
- Initializes the scanner with the loaded mstscax.dll module
- Pass `NULL` to auto-detect (MsRdpEx will load it)
- Returns `S_OK` if plugins were discovered

### Query Plugin Count

```c
int MsRdpEx_DvcPluginGetCount(void);
```
- Returns the number of discovered internal DVC plugins
- Call after initialization

### Get Plugin Entry

```c
typedef struct {
    char name[128];                           // Channel name (e.g., "Graphics")
    fnVirtualChannelGetInstance pfnGetInstance; // Function pointer
    uintptr_t rva;                           // Relative virtual address
    int tableIndex;                          // Index within table
    int tableId;                             // Table ID (0 or 1)
} MsRdpEx_DvcPluginEntry;

HRESULT MsRdpEx_DvcPluginGetEntry(int index, MsRdpEx_DvcPluginEntry* entry);
```
- Retrieves details for a specific plugin by index
- `pfnGetInstance` is the function pointer you can call directly

### Get Plugin by Name

```c
HRESULT MsRdpEx_DvcPluginGetEntryByName(const char* name, MsRdpEx_DvcPluginEntry* entry);
```
- Retrieves a plugin entry by channel name (case-insensitive)

### Get Plugin Name

```c
const char* MsRdpEx_DvcPluginGetName(int index);
```
- Returns the channel name for a given index
- Returns `NULL` if index is invalid

### Aggregate Access

```c
typedef HRESULT (__fastcall *fnVirtualChannelGetInstance)(
    const GUID* riid, 
    unsigned int* pCount, 
    void** ppPlugins);

HRESULT MsRdpEx_VirtualChannelGetInstance(
    const GUID* riid,
    unsigned int* pCount,
    void** ppPlugins);
```
- Aggregates all discovered plugins into a single call
- Uses standard two-call pattern:
  1. Call with `ppPlugins = NULL` to get count
  2. Call with allocated array to retrieve `IWTSPlugin*` instances

## Usage Example

### Basic Enumeration

```c
#include <windows.h>
#include <stdio.h>

typedef HRESULT (*fnInit)(HMODULE);
typedef int (*fnGetCount)(void);
typedef const char* (*fnGetName)(int);

int main() {
    // Load MsRdpEx.dll
    HMODULE hDll = LoadLibrary("MsRdpEx.dll");
    if (!hDll) {
        printf("Failed to load MsRdpEx.dll\n");
        return 1;
    }

    // Get exported functions
    fnInit Init = (fnInit)GetProcAddress(hDll, "MsRdpEx_DvcPluginExtractor_Init");
    fnGetCount GetCount = (fnGetCount)GetProcAddress(hDll, "MsRdpEx_DvcPluginGetCount");
    fnGetName GetName = (fnGetName)GetProcAddress(hDll, "MsRdpEx_DvcPluginGetName");

    // Initialize (auto-loads mstscax.dll)
    HRESULT hr = Init(NULL);
    if (FAILED(hr)) {
        printf("Scanner initialization failed\n");
        FreeLibrary(hDll);
        return 1;
    }

    // Enumerate plugins
    int count = GetCount();
    printf("Found %d internal DVC plugins:\n", count);
    
    for (int i = 0; i < count; i++) {
        const char* name = GetName(i);
        printf("  [%d] %s\n", i, name);
    }

    FreeLibrary(hDll);
    return 0;
}
```

### Calling Plugin Entry Points

```c
#include <MsRdpEx/DvcPluginExtractor.h>

// IID_IWTSPlugin
static const GUID IID_IWTSPlugin = 
    { 0xA1230201, 0x1439, 0x4E62, { 0xA4, 0x14, 0x19, 0x0D, 0x0A, 0xC3, 0xD4, 0x0E } };

void CallGraphicsPlugin() {
    MsRdpEx_DvcPluginEntry entry;
    HRESULT hr = MsRdpEx_DvcPluginGetEntryByName("Graphics", &entry);
    
    if (SUCCEEDED(hr)) {
        // Get the function pointer
        fnVirtualChannelGetInstance pfn = entry.pfnGetInstance;
        
        // First call: query count
        unsigned int count = 0;
        hr = pfn(&IID_IWTSPlugin, &count, NULL);
        
        if (SUCCEEDED(hr) && count > 0) {
            // Allocate array
            IWTSPlugin** plugins = (IWTSPlugin**)calloc(count, sizeof(IWTSPlugin*));
            
            // Second call: retrieve instances
            hr = pfn(&IID_IWTSPlugin, &count, (void**)plugins);
            
            if (SUCCEEDED(hr)) {
                // Use the IWTSPlugin instances
                for (unsigned int i = 0; i < count; i++) {
                    // Call plugin methods...
                    plugins[i]->Release();
                }
            }
            
            free(plugins);
        }
    }
}
```

## Important Notes

1. **COM Initialization Required**: Call `CoInitialize()` before using plugin instances
2. **RDP Context**: Some plugins may crash if called outside full RDP client context
3. **Function Pointers Only**: The extractor provides function pointers; calling them requires proper infrastructure
4. **Lazy Initialization**: Scanner runs on first API call, not during DLL load
5. **Thread Safety**: Uses `INIT_ONCE` for thread-safe initialization

## Build Requirements

- Windows SDK with `tsvirtualchannels.h`
- Microsoft Detours library
- CMake 3.10+
- MSVC 19.0+

## Test Program

Run `TestDvcExtractor.exe` to verify the scanner:

```
cd build-x64\Debug
.\TestDvcExtractor.exe
```

Or use the PowerShell wrapper:

```
.\test_scanner.ps1
```

## Return Values

- `S_OK (0x00000000)`: Success  
- `E_INVALIDARG (0x80070057)`: Invalid parameter
- `ERROR_NOT_FOUND (0x80070490)`: Plugin not found
- `HRESULT_FROM_WIN32(ERROR_NOT_FOUND)`: No plugins discovered

## Version Compatibility

Tested with:
- Windows 11 23H2
- mstscax.dll version 10.0.22631.4460
- Works with both x64 and x86 builds

## License

Same as MsRdpEx project (Apache 2.0)
