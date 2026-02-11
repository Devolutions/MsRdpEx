#ifndef MSRDPEX_DVC_PLUGIN_EXTRACTOR_H
#define MSRDPEX_DVC_PLUGIN_EXTRACTOR_H

#include <MsRdpEx/MsRdpEx.h>

#ifdef __cplusplus
extern "C" {
#endif

/**
 * Function pointer type matching the internal mstscax.dll
 * VirtualChannelGetInstance calling convention:
 *
 *   HRESULT __fastcall fn(const GUID* riid, unsigned int* pCount, void** ppPlugins);
 *
 * Two-call pattern:
 *   1) fn(riid, &count, NULL)       -> returns S_OK, sets count
 *   2) fn(riid, &count, ppArray)    -> fills ppArray with IWTSPlugin* pointers
 */
typedef HRESULT (__fastcall *fnVirtualChannelGetInstance)(const GUID* riid, unsigned int* pCount, void** ppPlugins);

#define MSRDPEX_DVC_PLUGIN_NAME_MAX     128

typedef struct _MsRdpEx_DvcPluginEntry
{
    char name[MSRDPEX_DVC_PLUGIN_NAME_MAX];     /* Channel name discovered via probing */
    fnVirtualChannelGetInstance pfnGetInstance;   /* Raw function pointer inside mstscax.dll */
    uintptr_t rva;                               /* RVA within mstscax.dll (for diagnostics) */
    int tableIndex;                              /* Index within the source table (0-based) */
    int tableId;                                 /* 0 = common (4-entry), 1 = dyn (16-entry) */
} MsRdpEx_DvcPluginEntry;

/**
 * Initialize the DVC plugin extractor by scanning a loaded mstscax.dll / rdclientax.dll.
 * If hModule is NULL, the system mstscax.dll will be loaded automatically.
 * Returns S_OK on success, or an error HRESULT.
 */
HRESULT CDECL MsRdpEx_DvcPluginExtractor_Init(HMODULE hModule);

/**
 * Free resources allocated by the extractor. Call on shutdown.
 */
void CDECL MsRdpEx_DvcPluginExtractor_Uninit(void);

/**
 * Get the number of discovered internal DVC plugin entry points.
 */
int CDECL MsRdpEx_DvcPluginGetCount(void);

/**
 * Get a plugin entry by index (0 to GetCount()-1).
 * Returns S_OK on success, E_INVALIDARG if index is out of range.
 */
HRESULT CDECL MsRdpEx_DvcPluginGetEntry(int index, MsRdpEx_DvcPluginEntry* entry);

/**
 * Get a plugin entry by channel name (case-insensitive match).
 * Returns S_OK on success, HRESULT_FROM_WIN32(ERROR_NOT_FOUND) if not found.
 */
HRESULT CDECL MsRdpEx_DvcPluginGetEntryByName(const char* name, MsRdpEx_DvcPluginEntry* entry);

/**
 * Get the display/channel name for a plugin at the given index.
 * Returns NULL if index is out of range.
 */
const char* CDECL MsRdpEx_DvcPluginGetName(int index);

/**
 * Aggregate VirtualChannelGetInstance export that follows the same
 * two-call pattern as the internal mstscax.dll functions.
 * Returns all discovered internal DVC plugins in a single call.
 *
 * Usage:
 *   unsigned int count = 0;
 *   MsRdpEx_VirtualChannelGetInstance(&IID_IWTSPlugin, &count, NULL);
 *   IWTSPlugin** plugins = (IWTSPlugin**)malloc(count * sizeof(void*));
 *   MsRdpEx_VirtualChannelGetInstance(&IID_IWTSPlugin, &count, (void**)plugins);
 */
HRESULT CDECL MsRdpEx_VirtualChannelGetInstance(const GUID* riid, unsigned int* pCount, void** ppPlugins);

/**
 * Get a single IWTSPlugin* instance by channel name.
 * The caller must Release() the returned object when done.
 */
HRESULT CDECL MsRdpEx_DvcPluginGetInstanceByName(const char* pluginName, void** ppPlugin);

#ifdef __cplusplus
}
#endif

#endif /* MSRDPEX_DVC_PLUGIN_EXTRACTOR_H */
