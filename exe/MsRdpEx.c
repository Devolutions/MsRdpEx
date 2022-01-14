
#include <MsRdpEx/Process.h>

int WINAPI wWinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPWSTR lpCmdLine, int nShowCmd)
{
    HRESULT hr;
    int argc = 0;
    char** argv = NULL;

    argv = MsRdpEx_GetArgumentVector(&argc);
    hr = MsRdpEx_LaunchProcess(argc, argv, NULL);
    MsRdpEx_FreeArgumentVector(argc, argv);

    return 0;
}
