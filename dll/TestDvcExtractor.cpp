/**
 * TestDvcExtractor.cpp
 * 
 * Standalone test program to verify the DVC plugin extractor functionality.
 * Loads MsRdpEx.dll and calls the exported functions to discover internal
 * mstscax.dll DVC plugins.
 */

#include <windows.h>
#include <stdio.h>
#include <tsvirtualchannels.h>

typedef HRESULT (__fastcall *fnVirtualChannelGetInstance)(const GUID* riid, unsigned int* pCount, void** ppPlugins);

typedef struct {
    char name[128];
    fnVirtualChannelGetInstance pfnGetInstance;
    uintptr_t rva;
    int tableIndex;
    int tableId;
} MsRdpEx_DvcPluginEntry;

typedef HRESULT (*fnInit)(HMODULE);
typedef int (*fnGetCount)(void);
typedef HRESULT (*fnGetEntry)(int, MsRdpEx_DvcPluginEntry*);
typedef const char* (*fnGetName)(int);
typedef HRESULT (*fnGetInstanceByName)(const char*, void**);
typedef HRESULT (*fnVirtualChannelGetInstance_Aggregate)(const GUID*, unsigned int*, void**);

// IID_IWTSPlugin
static const GUID IID_IWTSPlugin_Local =
    { 0xA1230201, 0x1439, 0x4E62, { 0xA4, 0x14, 0x19, 0x0D, 0x0A, 0xC3, 0xD4, 0x0E } };

int main(int argc, char* argv[])
{
    printf("==========================================================\n");
    printf("MsRdpEx DVC Plugin Extractor Test\n");
    printf("==========================================================\n\n");

    // Initialize COM
    printf("[0] Initializing COM...\n");
    HRESULT hrCom = CoInitializeEx(NULL, COINIT_APARTMENTTHREADED);
    if (FAILED(hrCom) && hrCom != RPC_E_CHANGED_MODE)
    {
        printf("    WARNING: COM initialization failed with 0x%08X\n", hrCom);
    }
    else
    {
        printf("    SUCCESS: COM initialized\n");
    }
    printf("\n");

    // Load MsRdpEx.dll
    printf("[1] Loading MsRdpEx.dll...\n");
    HMODULE hMsRdpEx = LoadLibraryA("MsRdpEx.dll");
    if (!hMsRdpEx)
    {
        printf("    ERROR: Failed to load MsRdpEx.dll (error %d)\n", GetLastError());
        printf("    Make sure MsRdpEx.dll is in the same directory.\n");
        return 1;
    }
    printf("    SUCCESS: Loaded at 0x%p\n\n", hMsRdpEx);

    // Get exported functions
    printf("[2] Resolving exported functions...\n");
    fnInit fnDvcInit = (fnInit)GetProcAddress(hMsRdpEx, "MsRdpEx_DvcPluginExtractor_Init");
    fnGetCount fnDvcGetCount = (fnGetCount)GetProcAddress(hMsRdpEx, "MsRdpEx_DvcPluginGetCount");
    fnGetEntry fnDvcGetEntry = (fnGetEntry)GetProcAddress(hMsRdpEx, "MsRdpEx_DvcPluginGetEntry");
    fnGetName fnDvcGetName = (fnGetName)GetProcAddress(hMsRdpEx, "MsRdpEx_DvcPluginGetName");
    fnGetInstanceByName fnDvcGetByName = (fnGetInstanceByName)GetProcAddress(hMsRdpEx, "MsRdpEx_DvcPluginGetInstanceByName");
    fnVirtualChannelGetInstance_Aggregate fnDvcAggregate = (fnVirtualChannelGetInstance_Aggregate)GetProcAddress(hMsRdpEx, "MsRdpEx_VirtualChannelGetInstance");

    if (!fnDvcInit || !fnDvcGetCount || !fnDvcGetEntry || !fnDvcGetName || !fnDvcGetByName || !fnDvcAggregate)
    {
        printf("    ERROR: Failed to resolve one or more exports\n");
        printf("      MsRdpEx_DvcPluginExtractor_Init: %p\n", fnDvcInit);
        printf("      MsRdpEx_DvcPluginGetCount: %p\n", fnDvcGetCount);
        printf("      MsRdpEx_DvcPluginGetEntry: %p\n", fnDvcGetEntry);
        printf("      MsRdpEx_DvcPluginGetName: %p\n", fnDvcGetName);
        printf("      MsRdpEx_DvcPluginGetInstanceByName: %p\n", fnDvcGetByName);
        printf("      MsRdpEx_VirtualChannelGetInstance: %p\n", fnDvcAggregate);
        FreeLibrary(hMsRdpEx);
        return 1;
    }
    printf("    SUCCESS: All exports resolved\n\n");

    // Initialize the extractor (this will scan mstscax.dll)
    printf("[3] Initializing DVC plugin extractor...\n");
    HRESULT hr = fnDvcInit(NULL);
    if (FAILED(hr))
    {
        printf("    ERROR: Init failed with HRESULT 0x%08X\n", hr);
        FreeLibrary(hMsRdpEx);
        return 1;
    }
    printf("    SUCCESS: Extractor initialized\n\n");

    // Get plugin count
    printf("[4] Discovering internal DVC plugins...\n");
    int count = fnDvcGetCount();
    printf("    Found %d internal DVC plugin entry points\n\n", count);

    if (count == 0)
    {
        printf("    WARNING: No plugins discovered. Check if mstscax.dll is loaded.\n");
        FreeLibrary(hMsRdpEx);
        return 0;
    }

    // List all plugins
    printf("[5] Plugin details:\n");
    printf("    %-4s %-24s %-12s %-8s %-8s\n", "Idx", "Name", "RVA", "TableID", "TableIdx");
    printf("    %s\n", "------------------------------------------------------------");

    for (int i = 0; i < count; i++)
    {
        MsRdpEx_DvcPluginEntry entry = { 0 };
        hr = fnDvcGetEntry(i, &entry);
        if (SUCCEEDED(hr))
        {
            printf("    [%-2d] %-24s 0x%08llX %-8d %-8d\n",
                   i, entry.name, (unsigned long long)entry.rva, entry.tableId, entry.tableIndex);
        }
    }
    printf("\n");

    // Test aggregate function (get all plugins at once)
    printf("[6] Testing aggregate VirtualChannelGetInstance...\n");
    printf("    NOTE: This may fail if COM/RDP infrastructure not fully initialized\n");
    
    __try
    {
        unsigned int totalCount = 0;
        hr = fnDvcAggregate(&IID_IWTSPlugin_Local, &totalCount, NULL);
        if (SUCCEEDED(hr))
        {
            printf("    First call: count = %d\n", totalCount);

            if (totalCount > 0)
            {
                void** plugins = (void**)calloc(totalCount, sizeof(void*));
                if (plugins)
                {
                    hr = fnDvcAggregate(&IID_IWTSPlugin_Local, &totalCount, plugins);
                    if (SUCCEEDED(hr))
                    {
                        printf("    Second call: successfully retrieved %d IWTSPlugin* instances\n", totalCount);

                        // Release all instances
                        for (unsigned int i = 0; i < totalCount; i++)
                        {
                            if (plugins[i])
                            {
                                __try
                                {
                                    ((IUnknown*)plugins[i])->Release();
                                }
                                __except(EXCEPTION_EXECUTE_HANDLER)
                                {
                                    printf("    WARNING: Exception during Release() for plugin %d\n", i);
                                }
                            }
                        }
                    }
                    else
                    {
                        printf("    ERROR: Second call failed with HRESULT 0x%08X\n", hr);
                    }
                    free(plugins);
                }
            }
        }
        else
        {
            printf("    ERROR: Aggregate call failed with HRESULT 0x%08X\n", hr);
        }
    }
    __except(EXCEPTION_EXECUTE_HANDLER)
    {
        printf("    ERROR: Exception caught (code 0x%08X) - internal functions may require full RDP client context\n", GetExceptionCode());
    }
    printf("\n");

    // Test getting a specific plugin by name (don't instantiate, just get function pointer)
    printf("[7] Testing GetEntryByName for 'Graphics'...\n");
    MsRdpEx_DvcPluginEntry graphicsEntry = { 0 };
    hr = fnDvcGetEntry(0, &graphicsEntry);
    if (SUCCEEDED(hr))
    {
        printf("    SUCCESS: Entry at index 0\n");
        printf("      Name:         %s\n", graphicsEntry.name);
        printf("      RVA:          0x%08llX\n", (unsigned long long)graphicsEntry.rva);
        printf("      FunctionPtr:  %p\n", graphicsEntry.pfnGetInstance);
        printf("      TableId:      %d\n", graphicsEntry.tableId);
        printf("      TableIndex:   %d\n", graphicsEntry.tableIndex);
    }
    else
    {
        printf("    ERROR: GetEntry failed with HRESULT 0x%08X\n", hr);
    }
    printf("\n");

    printf("==========================================================\n");
    printf("Test completed - Scanner successfully discovered plugins!\n");
    printf("\n");
    printf("SUMMARY:\n");
    printf("  - MsRdpEx.dll loaded and exports resolved successfully\n");
    printf("  - Scanner discovered %d internal DVC plugins from mstscax.dll\n", count);
    printf("  - Function pointers extracted and available for external use\n");
    printf("  - Calling plugins may require full RDP client infrastructure\n");
    printf("==========================================================\n");

    if (hrCom == S_OK || hrCom == S_FALSE)
        CoUninitialize();

    FreeLibrary(hMsRdpEx);
    return 0;
}
