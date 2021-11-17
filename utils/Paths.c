
#include "Utils.h"

EXTERN_C IMAGE_DOS_HEADER __ImageBase;

static char g_CURRENT_MODULE_PATH[MSRDPEX_MAX_PATH] = { 0 };
static char g_CURRENT_LIBRARY_PATH[MSRDPEX_MAX_PATH] = { 0 };

static char g_MSTSC_EXE_PATH[MSRDPEX_MAX_PATH] = { 0 };
static char g_MSTSCAX_DLL_PATH[MSRDPEX_MAX_PATH] = { 0 };

static char g_MSRDC_EXE_PATH[MSRDPEX_MAX_PATH] = { 0 };
static char g_RDCLIENTAX_DLL_PATH[MSRDPEX_MAX_PATH] = { 0 };

bool MsRdpEx_InitPaths(uint32_t pathIds)
{
    if (pathIds & MSRDPEX_CURRENT_MODULE_PATH) {
        GetModuleFileNameA(NULL, g_CURRENT_MODULE_PATH, MSRDPEX_MAX_PATH);
    }
    
    if (pathIds & MSRDPEX_CURRENT_LIBRARY_PATH) {
        GetModuleFileNameA((HINSTANCE)&__ImageBase, g_CURRENT_LIBRARY_PATH, MSRDPEX_MAX_PATH);
    }

    if (pathIds & MSRDPEX_MSTSC_EXE_PATH) {
        strcpy_s(g_MSTSC_EXE_PATH, MSRDPEX_MAX_PATH, "C:\\Windows\\System32\\mstsc.exe");
    }

    if (pathIds & MSRDPEX_MSTSCAX_DLL_PATH) {
        strcpy_s(g_MSTSCAX_DLL_PATH, MSRDPEX_MAX_PATH, "C:\\Windows\\System32\\mstscax.dll");
    }

    if (pathIds & MSRDPEX_MSRDC_EXE_PATH) {
        strcpy_s(g_MSRDC_EXE_PATH, MSRDPEX_MAX_PATH, "C:\\Program Files\\Remote Desktop\\msrdc.exe");
    }

    if (pathIds & MSRDPEX_RDCLIENTAX_DLL_PATH) {
        strcpy_s(g_RDCLIENTAX_DLL_PATH, MSRDPEX_MAX_PATH, "C:\\Program Files\\Remote Desktop\\rdclientax.dll");
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
