
#include <MsRdpEx/RdpProcess.h>

int WINAPI wWinMain(
    _In_ HINSTANCE hInstance,
    _In_opt_ HINSTANCE hPrevInstance,
    _In_ LPWSTR lpCmdLine,
    _In_ int nShowCmd)
{
    HRESULT hr;
    const char* axName = NULL;
    char moduleFilePath[MSRDPEX_MAX_PATH];

    if (!GetEnvironmentVariableA("MSRDPEX_AXNAME", NULL, 0))
    {
        axName = "mstsc";

        if (GetModuleFileNameA(NULL, moduleFilePath, MSRDPEX_MAX_PATH))
        {
            if (strstr(moduleFilePath, "msrdc.exe")) {
                axName = "msrdc";
            }
        }
    }

    hr = MsRdpEx_LaunchProcess(-1, NULL, NULL, axName);

    return 0;
}
