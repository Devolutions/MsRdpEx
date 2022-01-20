
#include <MsRdpEx/MsRdpEx.h>

EXTERN_C IMAGE_DOS_HEADER __ImageBase;

static char g_CURRENT_MODULE_PATH[MSRDPEX_MAX_PATH] = { 0 };
static char g_CURRENT_LIBRARY_PATH[MSRDPEX_MAX_PATH] = { 0 };

static char g_MSRDPEX_EXE_PATH[MSRDPEX_MAX_PATH] = { 0 };
static char g_MSRDPEX_DLL_PATH[MSRDPEX_MAX_PATH] = { 0 };

static char g_MSRDPEX_APP_DATA_PATH[MSRDPEX_MAX_PATH] = { 0 };

static char g_MSTSC_EXE_PATH[MSRDPEX_MAX_PATH] = { 0 };
static char g_MSTSCAX_DLL_PATH[MSRDPEX_MAX_PATH] = { 0 };

static char g_MSRDC_EXE_PATH[MSRDPEX_MAX_PATH] = { 0 };
static char g_RDCLIENTAX_DLL_PATH[MSRDPEX_MAX_PATH] = { 0 };

bool MsRdpEx_PathCchRenameExtension(char* pszPath, size_t cchPath, const char* pszExt)
{
    size_t length = strlen(pszPath);
    char* pszOldExt = strrchr(pszPath, '.');

    if (!pszOldExt)
        return false;

    size_t cchOldExt = strlen(pszOldExt);
    size_t cchNewExt = strlen(pszExt);

    if (cchOldExt != cchNewExt)
        return false;

    strcpy(&pszPath[length - cchNewExt], pszExt);

    return true;
}

bool MsRdpEx_PathCchDetect(char* pszPath, size_t cchPath, uint32_t pathId)
{
    bool success = true;

    switch (pathId)
    {
        case MSRDPEX_CURRENT_MODULE_PATH:
            GetModuleFileNameA(NULL, pszPath, cchPath);
            break;

        case MSRDPEX_CURRENT_LIBRARY_PATH:
            GetModuleFileNameA((HINSTANCE)&__ImageBase, pszPath, cchPath);
            break;

        default:
            success = false;
            break;
    }

    return success;
}

bool MsRdpEx_InitPaths(uint32_t pathIds)
{
    pathIds |= (MSRDPEX_CURRENT_MODULE_PATH | MSRDPEX_CURRENT_LIBRARY_PATH);

    if (pathIds & MSRDPEX_CURRENT_MODULE_PATH) {
        MsRdpEx_PathCchDetect(g_CURRENT_MODULE_PATH, MSRDPEX_MAX_PATH, MSRDPEX_CURRENT_MODULE_PATH);
    }
    
    if (pathIds & MSRDPEX_CURRENT_LIBRARY_PATH) {
        MsRdpEx_PathCchDetect(g_CURRENT_LIBRARY_PATH, MSRDPEX_MAX_PATH, MSRDPEX_CURRENT_LIBRARY_PATH);
    }

    const char* ModuleFileName = MsRdpEx_FileBase(g_CURRENT_MODULE_PATH);
    const char* LibraryFileName = MsRdpEx_FileBase(g_CURRENT_LIBRARY_PATH);

    if (pathIds & MSRDPEX_EXECUTABLE_PATH) {
        if (MsRdpEx_StringIEquals(ModuleFileName, "MsRdpEx.exe")) {
            strcpy_s(g_MSRDPEX_EXE_PATH, MSRDPEX_MAX_PATH, g_CURRENT_MODULE_PATH);
        }
        else if (MsRdpEx_StringIEquals(LibraryFileName, "MsRdpEx.dll")) {
            strcpy_s(g_MSRDPEX_EXE_PATH, MSRDPEX_MAX_PATH, g_CURRENT_LIBRARY_PATH);
            MsRdpEx_PathCchRenameExtension(g_MSRDPEX_EXE_PATH, MSRDPEX_MAX_PATH, ".exe");
        }
    }

    if (pathIds & MSRDPEX_LIBRARY_PATH) {
        if (MsRdpEx_StringIEquals(LibraryFileName, "MsRdpEx.dll")) {
            strcpy_s(g_MSRDPEX_DLL_PATH, MSRDPEX_MAX_PATH, g_CURRENT_LIBRARY_PATH);
        }
        else if (MsRdpEx_StringIEquals(ModuleFileName, "MsRdpEx.exe")) {
            strcpy_s(g_MSRDPEX_DLL_PATH, MSRDPEX_MAX_PATH, g_CURRENT_MODULE_PATH);
            MsRdpEx_PathCchRenameExtension(g_MSRDPEX_DLL_PATH, MSRDPEX_MAX_PATH, ".dll");
        }
    }

    if (pathIds & MSRDPEX_APP_DATA_PATH) {
        ExpandEnvironmentStringsA("%LocalAppData%\\MsRdpEx", g_MSRDPEX_APP_DATA_PATH, MSRDPEX_MAX_PATH);
        MsRdpEx_MakePath(g_MSRDPEX_APP_DATA_PATH, NULL);
    }

    if (pathIds & MSRDPEX_MSTSC_EXE_PATH) {
        ExpandEnvironmentStringsA("%SystemRoot%\\System32\\mstsc.exe", g_MSTSC_EXE_PATH, MSRDPEX_MAX_PATH);
    }

    if (pathIds & MSRDPEX_MSTSCAX_DLL_PATH) {
        ExpandEnvironmentStringsA("%SystemRoot%\\System32\\mstscax.dll", g_MSTSCAX_DLL_PATH, MSRDPEX_MAX_PATH);
    }

    if (pathIds & MSRDPEX_MSRDC_EXE_PATH) {
        ExpandEnvironmentStringsA("%ProgramFiles%\\Remote Desktop\\msrdc.exe", g_MSRDC_EXE_PATH, MSRDPEX_MAX_PATH);

        if (!MsRdpEx_FileExists(g_MSRDC_EXE_PATH)) {
            ExpandEnvironmentStringsA("%LocalAppData%\\Apps\\Remote Desktop\\msrdc.exe", g_MSRDC_EXE_PATH, MSRDPEX_MAX_PATH);
        }
    }

    if (pathIds & MSRDPEX_RDCLIENTAX_DLL_PATH) {
        ExpandEnvironmentStringsA("%ProgramFiles%\\Remote Desktop\\rdclientax.dll", g_RDCLIENTAX_DLL_PATH, MSRDPEX_MAX_PATH);

        if (!MsRdpEx_FileExists(g_RDCLIENTAX_DLL_PATH)) {
            ExpandEnvironmentStringsA("%LocalAppData%\\Apps\\Remote Desktop\\rdclientax.dll", g_RDCLIENTAX_DLL_PATH, MSRDPEX_MAX_PATH);
        }
    }

    return true;
}

const char* MsRdpEx_GetPath(uint32_t pathId)
{
    const char* path = NULL;

    switch (pathId)
    {
        case MSRDPEX_CURRENT_MODULE_PATH:
            path = (const char*) g_CURRENT_MODULE_PATH;
            break;

        case MSRDPEX_CURRENT_LIBRARY_PATH:
            path = (const char*) g_CURRENT_LIBRARY_PATH;
            break;

        case MSRDPEX_EXECUTABLE_PATH:
                path = (const char*) g_MSRDPEX_EXE_PATH;
                break;

        case MSRDPEX_LIBRARY_PATH:
                path = (const char*) g_MSRDPEX_DLL_PATH;
                break;

        case MSRDPEX_APP_DATA_PATH:
            path = (const char*) g_MSRDPEX_APP_DATA_PATH;
            break;

        case MSRDPEX_MSTSC_EXE_PATH:
            path = (const char*) g_MSTSC_EXE_PATH;
            break;

        case MSRDPEX_MSTSCAX_DLL_PATH:
            path = (const char*) g_MSTSCAX_DLL_PATH;
            break;

        case MSRDPEX_MSRDC_EXE_PATH:
            path = (const char*) g_MSRDC_EXE_PATH;
            break;

        case MSRDPEX_RDCLIENTAX_DLL_PATH:
            path = (const char*) g_RDCLIENTAX_DLL_PATH;
            break;
    }

    return path;
}
