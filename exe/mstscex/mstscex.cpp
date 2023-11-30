#include <MsRdpEx/RdpProcess.h>

int WINAPI wWinMain(
    _In_ HINSTANCE hInstance,
    _In_opt_ HINSTANCE hPrevInstance,
    _In_ LPWSTR lpCmdLine,
    _In_ int nShowCmd)
{
    HRESULT hr;

    hr = MsRdpEx_LaunchProcess(-1, NULL, NULL, "mstsc");

    return 0;
}
