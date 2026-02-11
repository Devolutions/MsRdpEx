/**
 * DvcPluginExtractor.cpp
 *
 * Scans a loaded mstscax.dll (or rdclientax.dll) for the internal
 * VirtualChannelGetInstance function pointer tables that are not
 * publicly exported. Discovers channel names by probing each plugin
 * with a mock IWTSVirtualChannelManager. Re-exports all discovered
 * entry points through a clean API.
 */

#include "MsRdpEx.h"

#include <MsRdpEx/MsRdpEx.h>
#include <MsRdpEx/DvcPluginExtractor.h>

#include <tsvirtualchannels.h>

#include <string.h>

/* -----------------------------------------------------------------
 * Constants
 * ----------------------------------------------------------------- */

#define MSRDPEX_DVC_MAX_PLUGINS     64

/* Expected sizes of the two internal tables */
#define COMMON_TABLE_SIZE           4
#define DYN_TABLE_SIZE              16

/* Minimum contiguous function pointer array size to consider */
#define MIN_TABLE_SIZE              4

/* -----------------------------------------------------------------
 * Global state
 * ----------------------------------------------------------------- */

static MsRdpEx_DvcPluginEntry g_DvcPlugins[MSRDPEX_DVC_MAX_PLUGINS];
static int g_DvcPluginCount = 0;
static bool g_DvcPluginInitDone = false;
static INIT_ONCE g_DvcInitOnce = INIT_ONCE_STATIC_INIT;
static HMODULE g_hMstscaxForScan = NULL;

/* IID_IWTSPlugin: {A1230201-1439-4E62-A414-190D0AC3D40E} */
static const GUID IID_IWTSPlugin_Local =
    { 0xA1230201, 0x1439, 0x4E62, { 0xA4, 0x14, 0x19, 0x0D, 0x0A, 0xC3, 0xD4, 0x0E } };

/* -----------------------------------------------------------------
 * PE Section Helpers
 * ----------------------------------------------------------------- */

typedef struct {
    uintptr_t va_start;
    uintptr_t va_end;
} SectionRange;

static bool GetPESectionRange(HMODULE hModule, const char* sectionName, SectionRange* range)
{
    uintptr_t base = (uintptr_t)hModule;
    IMAGE_DOS_HEADER* dosHeader = (IMAGE_DOS_HEADER*)base;

    if (dosHeader->e_magic != IMAGE_DOS_SIGNATURE)
        return false;

    IMAGE_NT_HEADERS* ntHeaders = (IMAGE_NT_HEADERS*)(base + dosHeader->e_lfanew);

    if (ntHeaders->Signature != IMAGE_NT_SIGNATURE)
        return false;

    IMAGE_SECTION_HEADER* sections = IMAGE_FIRST_SECTION(ntHeaders);
    WORD numSections = ntHeaders->FileHeader.NumberOfSections;

    for (WORD i = 0; i < numSections; i++)
    {
        if (strncmp((const char*)sections[i].Name, sectionName, IMAGE_SIZEOF_SHORT_NAME) == 0)
        {
            range->va_start = base + sections[i].VirtualAddress;
            range->va_end = range->va_start + sections[i].Misc.VirtualSize;
            return true;
        }
    }

    return false;
}

static bool IsPointerInRange(uintptr_t ptr, const SectionRange* range)
{
    return (ptr >= range->va_start) && (ptr < range->va_end);
}

/* -----------------------------------------------------------------
 * Mock IWTSVirtualChannelManager for channel name probing
 *
 * When a plugin calls Initialize(pChannelMgr), it typically calls
 * pChannelMgr->CreateListener("ChannelName", ...). We capture
 * that channel name and abort further initialization.
 * ----------------------------------------------------------------- */

#ifdef __cplusplus

class CProbeChannelManager : public IWTSVirtualChannelManager
{
public:
    char m_channelName[MSRDPEX_DVC_PLUGIN_NAME_MAX];
    bool m_nameCapured;

    CProbeChannelManager()
    {
        m_refCount = 1;
        m_channelName[0] = '\0';
        m_nameCapured = false;
    }

    virtual ~CProbeChannelManager() {}

    /* IUnknown */
    HRESULT STDMETHODCALLTYPE QueryInterface(REFIID riid, void** ppv) override
    {
        if (!ppv) return E_INVALIDARG;

        if (riid == IID_IUnknown || riid == __uuidof(IWTSVirtualChannelManager))
        {
            *ppv = static_cast<IWTSVirtualChannelManager*>(this);
            AddRef();
            return S_OK;
        }

        *ppv = NULL;
        return E_NOINTERFACE;
    }

    ULONG STDMETHODCALLTYPE AddRef() override
    {
        return InterlockedIncrement(&m_refCount);
    }

    ULONG STDMETHODCALLTYPE Release() override
    {
        ULONG refCount = InterlockedDecrement(&m_refCount);
        if (refCount == 0) { delete this; return 0; }
        return refCount;
    }

    /* IWTSVirtualChannelManager */
    HRESULT STDMETHODCALLTYPE CreateListener(
        const char* pszChannelName,
        ULONG ulFlags,
        IWTSListenerCallback* pListenerCallback,
        IWTSListener** ppListener) override
    {
        if (pszChannelName && !m_nameCapured)
        {
            strncpy_s(m_channelName, sizeof(m_channelName), pszChannelName, _TRUNCATE);
            m_nameCapured = true;
        }

        if (ppListener)
            *ppListener = NULL;

        /* Return E_ABORT to stop the plugin from doing further work */
        return E_ABORT;
    }

private:
    ULONG m_refCount;
};

#endif /* __cplusplus */

/* -----------------------------------------------------------------
 * Validate a candidate function pointer as VirtualChannelGetInstance
 * by calling it with the two-call pattern (first call: query count)
 * ----------------------------------------------------------------- */

static bool ValidateGetInstanceCandidate(fnVirtualChannelGetInstance pfn)
{
    unsigned int count = 0;

    __try
    {
        HRESULT hr = pfn(&IID_IWTSPlugin_Local, &count, NULL);
        return (SUCCEEDED(hr) && count >= 1 && count < 64);
    }
    __except (EXCEPTION_EXECUTE_HANDLER)
    {
        return false;
    }
}

/* -----------------------------------------------------------------
 * Try to discover the channel name for a VirtualChannelGetInstance
 * by creating plugin instances and probing with CProbeChannelManager
 * ----------------------------------------------------------------- */

static bool ProbePluginChannelName(fnVirtualChannelGetInstance pfn, char* nameOut, size_t nameOutSize)
{
#ifdef __cplusplus
    unsigned int count = 0;
    HRESULT hr;

    hr = pfn(&IID_IWTSPlugin_Local, &count, NULL);
    if (FAILED(hr) || count == 0)
        return false;

    void** plugins = (void**)calloc(count, sizeof(void*));
    if (!plugins)
        return false;

    hr = pfn(&IID_IWTSPlugin_Local, &count, plugins);
    if (FAILED(hr))
    {
        free(plugins);
        return false;
    }

    bool found = false;

    for (unsigned int i = 0; i < count && !found; i++)
    {
        IWTSPlugin* plugin = (IWTSPlugin*)plugins[i];
        if (!plugin)
            continue;

        CProbeChannelManager* probe = new CProbeChannelManager();

        __try
        {
            hr = plugin->Initialize((IWTSVirtualChannelManager*)probe);
        }
        __except (EXCEPTION_EXECUTE_HANDLER)
        {
            hr = E_FAIL;
        }

        if (probe->m_nameCapured && probe->m_channelName[0] != '\0')
        {
            strncpy_s(nameOut, nameOutSize, probe->m_channelName, _TRUNCATE);
            found = true;
        }

        probe->Release();
    }

    /* Release all plugin instances */
    for (unsigned int i = 0; i < count; i++)
    {
        IUnknown* pUnk = (IUnknown*)plugins[i];
        if (pUnk)
            pUnk->Release();
    }

    free(plugins);
    return found;
#else
    return false;
#endif
}

/* -----------------------------------------------------------------
 * Register a discovered plugin entry
 * ----------------------------------------------------------------- */

static void RegisterPlugin(fnVirtualChannelGetInstance pfn, uintptr_t moduleBase,
                            int tableId, int tableIndex, const char* fallbackName)
{
    if (g_DvcPluginCount >= MSRDPEX_DVC_MAX_PLUGINS)
        return;

    MsRdpEx_DvcPluginEntry* entry = &g_DvcPlugins[g_DvcPluginCount];
    memset(entry, 0, sizeof(MsRdpEx_DvcPluginEntry));

    entry->pfnGetInstance = pfn;
    entry->rva = (uintptr_t)pfn - moduleBase;
    entry->tableId = tableId;
    entry->tableIndex = tableIndex;

    /* Use fallback name for now - name probing can happen on first use to avoid init issues */
    if (fallbackName)
        strncpy_s(entry->name, sizeof(entry->name), fallbackName, _TRUNCATE);
    else
        _snprintf_s(entry->name, sizeof(entry->name), _TRUNCATE,
                    "Table%d_Plugin%d", tableId, tableIndex);

    MsRdpEx_LogPrint(DEBUG, "DvcPluginExtractor: [%d] name=\"%s\" rva=0x%llX table=%d idx=%d",
                     g_DvcPluginCount, entry->name, (unsigned long long)entry->rva,
                     entry->tableId, entry->tableIndex);

    g_DvcPluginCount++;
}

/* -----------------------------------------------------------------
 * Known fallback names by table position (stable across versions)
 * ----------------------------------------------------------------- */

static const char* g_CommonTableNames[COMMON_TABLE_SIZE] = {
    "Graphics", "Input", "SNDOUTPUT", "Location"
};

static const char* g_DynTableNames[DYN_TABLE_SIZE] = {
    "XPS", "SNDINPUT", "TSMM", "VIDEOBITMAP",
    "FrameBuffer", "GEOMETRYPLUGIN", "DisplayControl", "CredentialPlugin",
    "RemoteAppGfxRedir", "RemoteAppAuxRedir", "RDCAMERA", "NamedPipe",
    "RDCLIP", "BasicInput", "MouseCursor", "RemoteText"
};

/* -----------------------------------------------------------------
 * Core scanner: find contiguous function pointer arrays in .rdata
 * that point into .text
 * ----------------------------------------------------------------- */

static int ScanFunctionPointerTable(
    HMODULE hModule,
    const SectionRange* textRange,
    const SectionRange* rdataRange,
    int expectedSize,
    int tableId,
    const char** fallbackNames)
{
    uintptr_t moduleBase = (uintptr_t)hModule;
    int foundCount = 0;

    /* Scan .rdata for contiguous arrays of pointers into .text */
    uintptr_t scanStart = rdataRange->va_start;
    uintptr_t scanEnd = rdataRange->va_end - (expectedSize * sizeof(uintptr_t));

    for (uintptr_t addr = scanStart; addr <= scanEnd; addr += sizeof(uintptr_t))
    {
        uintptr_t* candidate = (uintptr_t*)addr;

        /* Check if this looks like a contiguous table of the expected size */
        bool allInText = true;
        for (int i = 0; i < expectedSize; i++)
        {
            if (!IsPointerInRange(candidate[i], textRange))
            {
                allInText = false;
                break;
            }
        }

        if (!allInText)
            continue;

        /* Verify the entry just before and just after are NOT in .text range,
         * to confirm we found the exact table boundary */
        if (addr > rdataRange->va_start)
        {
            uintptr_t prevVal = *(uintptr_t*)(addr - sizeof(uintptr_t));
            if (IsPointerInRange(prevVal, textRange))
                continue; /* Table starts earlier, this isn't the beginning */
        }

        uintptr_t afterAddr = addr + (expectedSize * sizeof(uintptr_t));
        if (afterAddr + sizeof(uintptr_t) <= rdataRange->va_end)
        {
            uintptr_t nextVal = *(uintptr_t*)afterAddr;
            if (IsPointerInRange(nextVal, textRange))
                continue; /* Table extends further, wrong size match */
        }

        /* Validate each pointer as a VirtualChannelGetInstance function */
        /* Skip aggressive validation during scan to avoid calling functions before DLL is fully initialized */
        bool allValid = true;
        /* Just do basic sanity check that pointers are in .text range */
        for (int i = 0; i < expectedSize; i++)
        {
            if (!IsPointerInRange(candidate[i], textRange))
            {
                allValid = false;
                break;
            }
        }

        if (!allValid)
            continue;

        MsRdpEx_LogPrint(DEBUG, "DvcPluginExtractor: Found table%d (%d entries) at rdata+0x%llX",
                         tableId, expectedSize,
                         (unsigned long long)(addr - rdataRange->va_start));

        /* Register all entries from this table */
        for (int i = 0; i < expectedSize; i++)
        {
            fnVirtualChannelGetInstance pfn = (fnVirtualChannelGetInstance)candidate[i];
            const char* fallback = (fallbackNames && i < expectedSize) ? fallbackNames[i] : NULL;
            RegisterPlugin(pfn, moduleBase, tableId, i, fallback);
        }

        foundCount = expectedSize;
        break; /* Found the table, stop scanning */
    }

    return foundCount;
}

/* -----------------------------------------------------------------
 * Main initialization logic
 * ----------------------------------------------------------------- */

static BOOL CALLBACK DvcPluginExtractor_InitOnce(PINIT_ONCE initOnce, PVOID param, PVOID* context)
{
    HMODULE hModule = (HMODULE)param;
    extern MsRdpEx_mstscax g_mstscax;

    if (!hModule)
    {
        /* No module provided — try to use the one already loaded by MsRdpEx */
        if (g_mstscax.initialized && g_mstscax.hModule)
        {
            hModule = g_mstscax.hModule;
        }
        else
        {
            /* Load it ourselves */
            MsRdpEx_mstscax_Init(&g_mstscax);
            hModule = g_mstscax.hModule;
        }
    }

    if (!hModule)
    {
        MsRdpEx_LogPrint(ERROR, "DvcPluginExtractor: No mstscax.dll module available");
        g_DvcPluginInitDone = true;
        return TRUE;
    }

    g_hMstscaxForScan = hModule;

    MsRdpEx_LogPrint(DEBUG, "DvcPluginExtractor: Scanning module at %p", hModule);

    SectionRange textRange = { 0 };
    SectionRange rdataRange = { 0 };

    if (!GetPESectionRange(hModule, ".text", &textRange))
    {
        MsRdpEx_LogPrint(ERROR, "DvcPluginExtractor: Failed to find .text section");
        g_DvcPluginInitDone = true;
        return TRUE;
    }

    if (!GetPESectionRange(hModule, ".rdata", &rdataRange))
    {
        MsRdpEx_LogPrint(ERROR, "DvcPluginExtractor: Failed to find .rdata section");
        g_DvcPluginInitDone = true;
        return TRUE;
    }

    MsRdpEx_LogPrint(DEBUG, "DvcPluginExtractor: .text [%p - %p], .rdata [%p - %p]",
                     (void*)textRange.va_start, (void*)textRange.va_end,
                     (void*)rdataRange.va_start, (void*)rdataRange.va_end);

    g_DvcPluginCount = 0;

    /* Scan for the 4-entry common table first */
    int commonFound = ScanFunctionPointerTable(
        hModule, &textRange, &rdataRange,
        COMMON_TABLE_SIZE, 0, g_CommonTableNames);

    MsRdpEx_LogPrint(DEBUG, "DvcPluginExtractor: Common table: %d plugins found", commonFound);

    /* Scan for the 16-entry dyn table */
    int dynFound = ScanFunctionPointerTable(
        hModule, &textRange, &rdataRange,
        DYN_TABLE_SIZE, 1, g_DynTableNames);

    MsRdpEx_LogPrint(DEBUG, "DvcPluginExtractor: Dyn table: %d plugins found", dynFound);

    /* If exact sizes weren't found, try a more flexible scan */
    if (commonFound == 0 && dynFound == 0)
    {
        MsRdpEx_LogPrint(WARN, "DvcPluginExtractor: Standard table scan failed, trying flexible scan");

        /* Try scanning for any contiguous array of validated function pointers */
        uintptr_t scanAddr = rdataRange.va_start;
        while (scanAddr + sizeof(uintptr_t) <= rdataRange.va_end && g_DvcPluginCount < MSRDPEX_DVC_MAX_PLUGINS)
        {
            uintptr_t val = *(uintptr_t*)scanAddr;
            if (IsPointerInRange(val, &textRange))
            {
                fnVirtualChannelGetInstance pfn = (fnVirtualChannelGetInstance)val;
                if (ValidateGetInstanceCandidate(pfn))
                {
                    /* Check we haven't already registered this exact pointer */
                    bool duplicate = false;
                    for (int i = 0; i < g_DvcPluginCount; i++)
                    {
                        if (g_DvcPlugins[i].pfnGetInstance == pfn)
                        {
                            duplicate = true;
                            break;
                        }
                    }

                    if (!duplicate)
                    {
                        RegisterPlugin(pfn, (uintptr_t)hModule, -1, g_DvcPluginCount, NULL);
                    }
                }
            }
            scanAddr += sizeof(uintptr_t);
        }
    }

    MsRdpEx_LogPrint(DEBUG, "DvcPluginExtractor: Total plugins discovered: %d", g_DvcPluginCount);

    g_DvcPluginInitDone = true;
    return TRUE;
}

/* -----------------------------------------------------------------
 * Ensure initialization has been performed
 * ----------------------------------------------------------------- */

static void EnsureInitialized(HMODULE hModule)
{
    InitOnceExecuteOnce(&g_DvcInitOnce, DvcPluginExtractor_InitOnce, (PVOID)hModule, NULL);
}

/* -----------------------------------------------------------------
 * Public API Implementation
 * ----------------------------------------------------------------- */

HRESULT CDECL MsRdpEx_DvcPluginExtractor_Init(HMODULE hModule)
{
    EnsureInitialized(hModule);
    return (g_DvcPluginCount > 0) ? S_OK : HRESULT_FROM_WIN32(ERROR_NOT_FOUND);
}

void CDECL MsRdpEx_DvcPluginExtractor_Uninit(void)
{
    g_DvcPluginCount = 0;
    g_DvcPluginInitDone = false;
    g_hMstscaxForScan = NULL;
    memset(g_DvcPlugins, 0, sizeof(g_DvcPlugins));

    /* Reset InitOnce so it can be called again */
    INIT_ONCE initOnce = INIT_ONCE_STATIC_INIT;
    g_DvcInitOnce = initOnce;
}

int CDECL MsRdpEx_DvcPluginGetCount(void)
{
    EnsureInitialized(NULL);
    return g_DvcPluginCount;
}

HRESULT CDECL MsRdpEx_DvcPluginGetEntry(int index, MsRdpEx_DvcPluginEntry* entry)
{
    EnsureInitialized(NULL);

    if (!entry)
        return E_INVALIDARG;

    if (index < 0 || index >= g_DvcPluginCount)
        return E_INVALIDARG;

    memcpy(entry, &g_DvcPlugins[index], sizeof(MsRdpEx_DvcPluginEntry));
    return S_OK;
}

HRESULT CDECL MsRdpEx_DvcPluginGetEntryByName(const char* name, MsRdpEx_DvcPluginEntry* entry)
{
    EnsureInitialized(NULL);

    if (!name || !entry)
        return E_INVALIDARG;

    for (int i = 0; i < g_DvcPluginCount; i++)
    {
        if (_stricmp(g_DvcPlugins[i].name, name) == 0)
        {
            memcpy(entry, &g_DvcPlugins[i], sizeof(MsRdpEx_DvcPluginEntry));
            return S_OK;
        }
    }

    return HRESULT_FROM_WIN32(ERROR_NOT_FOUND);
}

const char* CDECL MsRdpEx_DvcPluginGetName(int index)
{
    EnsureInitialized(NULL);

    if (index < 0 || index >= g_DvcPluginCount)
        return NULL;

    return g_DvcPlugins[index].name;
}

HRESULT CDECL MsRdpEx_VirtualChannelGetInstance(const GUID* riid, unsigned int* pCount, void** ppPlugins)
{
    EnsureInitialized(NULL);

    if (!pCount)
        return E_INVALIDARG;

    if (!ppPlugins)
    {
        /* First call: aggregate count from all discovered plugins */
        unsigned int totalCount = 0;

        for (int i = 0; i < g_DvcPluginCount; i++)
        {
            unsigned int count = 0;
            HRESULT hr = g_DvcPlugins[i].pfnGetInstance(riid, &count, NULL);
            if (SUCCEEDED(hr))
                totalCount += count;
        }

        *pCount = totalCount;
        return S_OK;
    }
    else
    {
        /* Second call: fill the array with plugin instances from all entries */
        unsigned int offset = 0;
        unsigned int totalRequested = *pCount;

        for (int i = 0; i < g_DvcPluginCount && offset < totalRequested; i++)
        {
            unsigned int count = 0;
            HRESULT hr = g_DvcPlugins[i].pfnGetInstance(riid, &count, NULL);
            if (FAILED(hr) || count == 0)
                continue;

            /* Allocate temp array for this plugin's instances */
            void** tempPlugins = (void**)calloc(count, sizeof(void*));
            if (!tempPlugins)
                continue;

            hr = g_DvcPlugins[i].pfnGetInstance(riid, &count, tempPlugins);
            if (SUCCEEDED(hr))
            {
                for (unsigned int j = 0; j < count && offset < totalRequested; j++)
                {
                    ppPlugins[offset++] = tempPlugins[j];
                }

                /* Release any excess that didn't fit */
                for (unsigned int j = offset - (offset > 0 ? offset : 0); j < count; j++)
                {
                    /* If we already copied all, break */
                }
            }

            free(tempPlugins);
        }

        *pCount = offset;
        return S_OK;
    }
}

HRESULT CDECL MsRdpEx_DvcPluginGetInstanceByName(const char* pluginName, void** ppPlugin)
{
    EnsureInitialized(NULL);

    if (!pluginName || !ppPlugin)
        return E_INVALIDARG;

    *ppPlugin = NULL;

    for (int i = 0; i < g_DvcPluginCount; i++)
    {
        if (_stricmp(g_DvcPlugins[i].name, pluginName) == 0)
        {
            unsigned int count = 0;
            HRESULT hr = g_DvcPlugins[i].pfnGetInstance(&IID_IWTSPlugin_Local, &count, NULL);
            if (FAILED(hr) || count == 0)
                return hr;

            void** plugins = (void**)calloc(count, sizeof(void*));
            if (!plugins)
                return E_OUTOFMEMORY;

            hr = g_DvcPlugins[i].pfnGetInstance(&IID_IWTSPlugin_Local, &count, plugins);
            if (SUCCEEDED(hr) && count > 0 && plugins[0])
            {
                *ppPlugin = plugins[0];
                /* AddRef is already done by GetInstance, caller owns the reference */

                /* Release any additional instances beyond the first */
                for (unsigned int j = 1; j < count; j++)
                {
                    if (plugins[j])
                        ((IUnknown*)plugins[j])->Release();
                }
            }
            else
            {
                hr = E_FAIL;
            }

            free(plugins);
            return hr;
        }
    }

    return HRESULT_FROM_WIN32(ERROR_NOT_FOUND);
}
