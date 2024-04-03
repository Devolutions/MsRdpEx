#ifndef MSRDPEX_WIN_MAIN_H
#define MSRDPEX_WIN_MAIN_H

#include <MsRdpEx/MsRdpEx.h>

#ifdef __cplusplus
extern "C" {
#endif

int MsRdpEx_WinMain(
    HINSTANCE hInstance,
    HINSTANCE hPrevInstance,
    LPWSTR lpCmdLine,
    int nCmdShow,
    const char* axName);

#ifdef __cplusplus
}
#endif

#endif /* MSRDPEX_WIN_MAIN_H */
