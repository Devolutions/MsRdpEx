
#include <MsRdpEx/Process.h>

int WINAPI wWinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPWSTR lpCmdLine, int nShowCmd)
{
    int status;
    int argc = 0;
    char** argv = NULL;

    argv = MsRdpEx_GetArgumentVector(&argc);
    status = MsRdpEx_LaunchProcessMain(argc, argv, NULL);
    MsRdpEx_FreeArgumentVector(argc, argv);

    return status;
}
