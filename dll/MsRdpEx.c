
#include "MsRdpEx.h"

#include "MreUtils.h"

static FILE* g_LogFile = NULL;

static MsRdpEx_AxDll* g_AxDll = NULL;
static MsRdpEx_AxDll* g_mstscax = NULL;
static MsRdpEx_AxDll* g_rdclientax = NULL;

HRESULT DllCanUnloadNow()
{
    fprintf(g_LogFile, "DllCanUnloadNow\n");
    return g_AxDll->DllCanUnloadNow();
}

HRESULT DllGetClassObject(REFCLSID rclsid, REFIID riid, LPVOID* ppv)
{
    fprintf(g_LogFile, "DllGetClassObject\n");
    return g_AxDll->DllGetClassObject(rclsid, riid, ppv);
}

HRESULT DllRegisterServer()
{
    fprintf(g_LogFile, "DllRegisterServer\n");
    return g_AxDll->DllRegisterServer();
}

HRESULT DllUnregisterServer()
{
    fprintf(g_LogFile, "DllUnregisterServer\n");
    return g_AxDll->DllUnregisterServer();
}

uint64_t DllGetTscCtlVer()
{
    fprintf(g_LogFile, "DllGetTscCtlVer\n");
    return g_AxDll->DllGetTscCtlVer();
}

HRESULT DllSetAuthProperties(uint64_t properties)
{
    fprintf(g_LogFile, "DllSetAuthProperties\n");
    return g_AxDll->DllSetAuthProperties(properties);
}

HRESULT DllSetClaimsToken(uint64_t a1, uint64_t a2, WCHAR* a3)
{
    fprintf(g_LogFile, "DllSetClaimsToken\n");
    return g_AxDll->DllSetClaimsToken(a1, a2, a3);
}

HRESULT DllGetClaimsToken(WCHAR* a1, WCHAR* a2, WCHAR* a3, uint64_t a4, HWND a5, WCHAR** a6, WCHAR** a7, WCHAR* a8, WCHAR* a9)
{
    fprintf(g_LogFile, "DllGetClaimsToken\n");
    return g_AxDll->DllGetClaimsToken(a1, a2, a3, a4, a5, a6, a7, a8, a9);
}

HRESULT DllLogoffClaimsToken(WCHAR* a1)
{
    fprintf(g_LogFile, "DllLogoffClaimsToken\n");
    return g_AxDll->DllLogoffClaimsToken(a1);
}

HRESULT DllCancelAuthentication()
{
    fprintf(g_LogFile, "DllCancelAuthentication\n");
    return g_AxDll->DllCancelAuthentication();
}

HRESULT DllDeleteSavedCreds(WCHAR* a1, WCHAR* a2)
{
    fprintf(g_LogFile, "DllDeleteSavedCreds\n");
    return g_AxDll->DllDeleteSavedCreds(a1, a2);
}

// MsRdpEx_AxDll

MsRdpEx_AxDll* MsRdpEx_AxDll_New(const char* filename)
{
	HMODULE hModule;
	MsRdpEx_AxDll* dll;

	hModule = LoadLibraryA(filename);

	if (!hModule)
		return NULL;

	dll = (MsRdpEx_AxDll*) malloc(sizeof(MsRdpEx_AxDll));

	if (!dll)
		return NULL;

	ZeroMemory(dll, sizeof(MsRdpEx_AxDll));

	dll->hModule = hModule;

    dll->DllCanUnloadNow = (fnDllCanUnloadNow) GetProcAddress(hModule, "DllCanUnloadNow");
    dll->DllGetClassObject = (fnDllGetClassObject) GetProcAddress(hModule, "DllGetClassObject");
    dll->DllRegisterServer = (fnDllRegisterServer) GetProcAddress(hModule, "DllRegisterServer");
    dll->DllUnregisterServer = (fnDllUnregisterServer) GetProcAddress(hModule, "DllUnregisterServer");
    dll->DllGetTscCtlVer = (fnDllGetTscCtlVer) GetProcAddress(hModule, "DllGetTscCtlVer");
    dll->DllSetAuthProperties = (fnDllSetAuthProperties) GetProcAddress(hModule, "DllSetAuthProperties");
    dll->DllGetClaimsToken = (fnDllGetClaimsToken) GetProcAddress(hModule, "DllGetClaimsToken");
    dll->DllSetClaimsToken = (fnDllSetClaimsToken) GetProcAddress(hModule, "DllSetClaimsToken");
    dll->DllLogoffClaimsToken = (fnDllLogoffClaimsToken) GetProcAddress(hModule, "DllLogoffClaimsToken");
    dll->DllCancelAuthentication = (fnDllCancelAuthentication) GetProcAddress(hModule, "DllCancelAuthentication");
    dll->DllDeleteSavedCreds = (fnDllDeleteSavedCreds) GetProcAddress(hModule, "DllDeleteSavedCreds");

	return dll;
}

void MsRdpEx_AxDll_Free(MsRdpEx_AxDll* dll)
{
    if (!dll)
        return;
    
    if (dll->hModule) {
        FreeLibrary(dll->hModule);
        dll->hModule = NULL;
    }

    ZeroMemory(dll, sizeof(MsRdpEx_AxDll));
    
    free(dll);
}

// Logger

void MreLog_Open()
{
    char filename[1024];
    strcpy_s(filename, 1024, "C:\\Windows\\Temp\\MsRdpEx.log");
    g_LogFile = fopen(filename, "wb");
}

void MreLog_Close()
{
    if (g_LogFile) {
        fclose(g_LogFile);
        g_LogFile = NULL;
    }
}

// DLL Main

void MsRdpEx_Load()
{
    const char* filename;
    char ModuleFileName[1024];
    char ModuleVersion[64];

    GetModuleFileNameA(NULL, ModuleFileName, 1024);
    MreFile_GetVersion(ModuleFileName, ModuleVersion);
    filename = MreFile_Base(ModuleFileName);

    MreLog_Open();

    fprintf(g_LogFile, "ModuleFileName: %s (%s)\n", filename, ModuleVersion);

    g_mstscax = MsRdpEx_AxDll_New("C:\\Windows\\System32\\mstscax.dll");
    //g_rdclientax = MsRdpEx_AxDll_New("C:\\Program Files\\Remote Desktop\\rdclientax.dll");

    g_AxDll = g_mstscax;

    if (g_rdclientax) {
        g_AxDll = g_rdclientax;
    }
}

void MsRdpEx_Unload()
{
    if (g_mstscax) {
        MsRdpEx_AxDll_Free(g_mstscax);
        g_mstscax = NULL;
    }

    if (g_rdclientax) {
        MsRdpEx_AxDll_Free(g_rdclientax);
        g_rdclientax = NULL;
    }

    g_AxDll = NULL;
    MreLog_Close();
}

BOOL WINAPI DllMain(HINSTANCE hInstance, DWORD dwReason, LPVOID reserved)
{
    switch (dwReason) 
    { 
        case DLL_PROCESS_ATTACH:
            MsRdpEx_Load();
            break;

        case DLL_PROCESS_DETACH:
            MsRdpEx_Unload();
            break;

        case DLL_THREAD_ATTACH:
            break;

        case DLL_THREAD_DETACH:
            break;
    }

    return TRUE;
}
