
#include <MsRdpEx/RdpProcess.h>

int WINAPI wWinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPWSTR lpCmdLine, int nShowCmd)
{
    HRESULT hr;

    hr = MsRdpEx_LaunchProcess(-1, NULL, NULL, "mstsc");

    return 0;
}
