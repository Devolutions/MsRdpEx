
#include <MsRdpEx/MsRdpEx.h>

#include <MsRdpEx/Environment.h>

EXTERN_C IMAGE_DOS_HEADER __ImageBase;

static char g_CURRENT_MODULE_PATH[MSRDPEX_MAX_PATH] = { 0 };
static char g_CURRENT_LIBRARY_PATH[MSRDPEX_MAX_PATH] = { 0 };
static char g_MODULE_DIR_PATH[MSRDPEX_MAX_PATH] = { 0 };
static char g_LIBRARY_DIR_PATH[MSRDPEX_MAX_PATH] = { 0 };

static char g_MSRDPEX_EXE_PATH[MSRDPEX_MAX_PATH] = { 0 };
static char g_MSRDPEX_DLL_PATH[MSRDPEX_MAX_PATH] = { 0 };

static char g_MSRDPEX_APP_DATA_PATH[MSRDPEX_MAX_PATH] = { 0 };

static char g_MSTSC_EXE_PATH[MSRDPEX_MAX_PATH] = { 0 };
static char g_MSTSCAX_DLL_PATH[MSRDPEX_MAX_PATH] = { 0 };

static char g_MSRDC_EXE_PATH[MSRDPEX_MAX_PATH] = { 0 };
static char g_RDCLIENTAX_DLL_PATH[MSRDPEX_MAX_PATH] = { 0 };

static char g_DEFAULT_RDP_PATH[MSRDPEX_MAX_PATH] = { 0 };

static char g_XMF_DLL_PATH[MSRDPEX_MAX_PATH] = { 0 };

static char g_VMCONNECT_EXE_PATH[MSRDPEX_MAX_PATH] = { 0 };

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

DWORD MsRdpEx_GetModuleFileName(HMODULE hModule, LPSTR lpFileName, DWORD nSize)
{
    DWORD nStatus = 0;
    DWORD errorCode = 0;
    LPWSTR lpFileNameW = NULL;

    if (nSize < 1)
    {
        errorCode = ERROR_INSUFFICIENT_BUFFER;
        goto exit;
    }

    lpFileNameW = malloc((nSize + 1) * 2);

    if (!lpFileNameW)
    {
        errorCode = ERROR_INSUFFICIENT_BUFFER;
        goto exit;
    }

    lpFileNameW[nSize] = '\0';

    DWORD nStatusW = GetModuleFileNameW(hModule, lpFileNameW, nSize);

    if (nStatusW == 0)
    {
        errorCode = GetLastError();
        goto exit;
    }

    int nStatusA = WideCharToMultiByte(CP_UTF8, 0, lpFileNameW, nStatusW + 1, lpFileName, nSize, NULL, NULL);

    if (nStatusA < 2) {
        errorCode = ERROR_INSUFFICIENT_BUFFER;
        goto exit;
    }

    nStatus = (nStatusA - 1); // number of TCHARS not including terminating null character

exit:
    free(lpFileNameW);
    if (errorCode) {
        SetLastError(errorCode);
        return 0;
    }
    return nStatus;
}

bool MsRdpEx_PathCchDetect(char* pszPath, size_t cchPath, uint32_t pathId)
{
    bool success = true;

    switch (pathId)
    {
        case MSRDPEX_CURRENT_MODULE_PATH:
            MsRdpEx_GetModuleFileName(NULL, pszPath, cchPath);
            break;

        case MSRDPEX_CURRENT_LIBRARY_PATH:
            MsRdpEx_GetModuleFileName((HINSTANCE)&__ImageBase, pszPath, cchPath);
            break;

        default:
            success = false;
            break;
    }

    return success;
}

DWORD MsRdpEx_ExpandEnvironmentStrings(LPCSTR lpSrc, LPSTR lpDst, DWORD nSize)
{
    DWORD nStatus = 0;
    DWORD errorCode = 0;
    LPWSTR lpSrcW = NULL;
    LPWSTR lpDstW = NULL;

    if (MsRdpEx_ConvertToUnicode(CP_UTF8, 0, lpSrc, -1, &lpSrcW, 0) < 1)
    {
        errorCode = ERROR_INSUFFICIENT_BUFFER;
        goto exit;
    }

    if (!lpDst)
    {
        nStatus = ExpandEnvironmentStringsW(lpSrcW, NULL, 0);
        goto exit;
    }

    lpDstW = (LPWSTR)malloc((nSize + 1) * 2);

    if (!lpDstW)
    {
        errorCode = ERROR_INSUFFICIENT_BUFFER;
        goto exit;
    }

    lpDstW[nSize] = '\0';

    DWORD nStatusW = ExpandEnvironmentStringsW(lpSrcW, lpDstW, nSize);

    if (nStatusW == 0)
    {
        errorCode = GetLastError();
        goto exit;
    }

    int nStatusA = WideCharToMultiByte(CP_UTF8, 0, lpDstW, nStatusW + 1, lpDst, nSize, NULL, NULL);

    if (nStatusA < 2) {
        errorCode = ERROR_INSUFFICIENT_BUFFER;
        goto exit;
    }

    nStatus = nStatusA; // number of TCHARS including terminating null character

exit:
    free(lpSrcW);
    free(lpDstW);
    if (errorCode) {
        SetLastError(errorCode);
        return 0;
    }
    return nStatus;
}

bool MsRdpEx_InitPaths(uint32_t pathIds)
{
    pathIds |= (MSRDPEX_CURRENT_MODULE_PATH | MSRDPEX_CURRENT_LIBRARY_PATH);

    if (pathIds & MSRDPEX_CURRENT_MODULE_PATH) {
        MsRdpEx_PathCchDetect(g_CURRENT_MODULE_PATH, MSRDPEX_MAX_PATH, MSRDPEX_CURRENT_MODULE_PATH);

        if (pathIds & MSRDPEX_MODULE_DIR_PATH) {
            char* dirname = MsRdpEx_FileDir(g_CURRENT_MODULE_PATH);

            if (dirname) {
                strcpy_s(g_MODULE_DIR_PATH, MSRDPEX_MAX_PATH, dirname);
                free(dirname);
            }
        }
    }
    
    if (pathIds & MSRDPEX_CURRENT_LIBRARY_PATH) {
        MsRdpEx_PathCchDetect(g_CURRENT_LIBRARY_PATH, MSRDPEX_MAX_PATH, MSRDPEX_CURRENT_LIBRARY_PATH);

        if (pathIds & MSRDPEX_LIBRARY_DIR_PATH) {
            char* dirname = MsRdpEx_FileDir(g_CURRENT_LIBRARY_PATH);

            if (dirname) {
                strcpy_s(g_LIBRARY_DIR_PATH, MSRDPEX_MAX_PATH, dirname);
                free(dirname);
            }
        }
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
        MsRdpEx_ExpandEnvironmentStrings("%LocalAppData%\\MsRdpEx", g_MSRDPEX_APP_DATA_PATH, MSRDPEX_MAX_PATH);
        MsRdpEx_MakePath(g_MSRDPEX_APP_DATA_PATH, NULL);
    }

    if (pathIds & MSRDPEX_MSTSC_EXE_PATH) {
        MsRdpEx_ExpandEnvironmentStrings("%SystemRoot%\\System32\\mstsc.exe", g_MSTSC_EXE_PATH, MSRDPEX_MAX_PATH);
    }

    if (pathIds & MSRDPEX_MSTSCAX_DLL_PATH) {
        MsRdpEx_ExpandEnvironmentStrings("%SystemRoot%\\System32\\mstscax.dll", g_MSTSCAX_DLL_PATH, MSRDPEX_MAX_PATH);
    }

    if (pathIds & MSRDPEX_MSRDC_EXE_PATH) {
        MsRdpEx_ExpandEnvironmentStrings("%ProgramFiles%\\Remote Desktop\\msrdc.exe", g_MSRDC_EXE_PATH, MSRDPEX_MAX_PATH);

        if (!MsRdpEx_FileExists(g_MSRDC_EXE_PATH)) {
            MsRdpEx_ExpandEnvironmentStrings("%LocalAppData%\\Apps\\Remote Desktop\\msrdc.exe", g_MSRDC_EXE_PATH, MSRDPEX_MAX_PATH);
        }
    }

    if (pathIds & MSRDPEX_RDCLIENTAX_DLL_PATH) {
        MsRdpEx_ExpandEnvironmentStrings("%ProgramFiles%\\Remote Desktop\\rdclientax.dll", g_RDCLIENTAX_DLL_PATH, MSRDPEX_MAX_PATH);

        if (!MsRdpEx_FileExists(g_RDCLIENTAX_DLL_PATH)) {
            MsRdpEx_ExpandEnvironmentStrings("%LocalAppData%\\Apps\\Remote Desktop\\rdclientax.dll", g_RDCLIENTAX_DLL_PATH, MSRDPEX_MAX_PATH);
        }
    }

    if (pathIds & MSRDPEX_DEFAULT_RDP_PATH) {
        MsRdpEx_ExpandEnvironmentStrings("%UserProfile%\\Documents\\Default.rdp", g_DEFAULT_RDP_PATH, MSRDPEX_MAX_PATH);
    }

    if (pathIds & MSRDPEX_XMF_DLL_PATH) {
        char* envPath = MsRdpEx_GetEnv("MSRDPEX_XMF_DLL");

        if (envPath) {
            strcpy_s(g_XMF_DLL_PATH, MSRDPEX_MAX_PATH, envPath);
            free(envPath);
        }
        else {
            char libPath[MSRDPEX_MAX_PATH];
            sprintf_s(libPath, MSRDPEX_MAX_PATH, "%s%s", g_LIBRARY_DIR_PATH, "xmf.dll");

            if (MsRdpEx_FileExists(libPath)) {
                strcpy_s(g_XMF_DLL_PATH, MSRDPEX_MAX_PATH, libPath);
            }
        }
    }

    if (pathIds & MSRDPEX_VMCONNECT_EXE_PATH) {
        MsRdpEx_ExpandEnvironmentStrings("%SystemRoot%\\System32\\vmconnect.exe", g_VMCONNECT_EXE_PATH, MSRDPEX_MAX_PATH);
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

        case MSRDPEX_DEFAULT_RDP_PATH:
            path = (const char*) g_DEFAULT_RDP_PATH;
            break;

        case MSRDPEX_MODULE_DIR_PATH:
            path = (const char*) g_MODULE_DIR_PATH;
            break;

        case MSRDPEX_LIBRARY_DIR_PATH:
            path = (const char*) g_LIBRARY_DIR_PATH;
            break;

        case MSRDPEX_XMF_DLL_PATH:
            path = (const char*) g_XMF_DLL_PATH;
            break;

        case MSRDPEX_VMCONNECT_EXE_PATH:
            path = (const char*) g_VMCONNECT_EXE_PATH;
            break;
    }

    return path;
}
