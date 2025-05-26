
#include "../dll/AxHost/RdpWinMain.h"

int WINAPI wWinMain(
    _In_ HINSTANCE hInstance,
    _In_opt_ HINSTANCE hPrevInstance,
    _In_ LPWSTR lpCmdLine,
    _In_ int nShowCmd)
{
    return MsRdpEx_WinMain(hInstance, hPrevInstance, lpCmdLine, nShowCmd, "vmconnect");
}
