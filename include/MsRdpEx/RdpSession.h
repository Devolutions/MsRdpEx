#ifndef MSRDPEX_RDP_SESSION_H
#define MSRDPEX_RDP_SESSION_H

#include <MsRdpEx/MsRdpEx.h>

#include <MsRdpEx/OutputMirror.h>

#ifdef __cplusplus
extern "C" {
#endif

typedef struct _MsRdpEx_RdpSession MsRdpEx_RdpSession;

MsRdpEx_OutputMirror* MsRdpEx_RdpSession_GetOutputMirror(MsRdpEx_RdpSession* session);
void MsRdpEx_RdpSession_SetOutputMirror(MsRdpEx_RdpSession* session, MsRdpEx_OutputMirror* outputMirror);

MsRdpEx_RdpSession* MsRdpEx_RdpSession_New();
void MsRdpEx_RdpSession_Free(MsRdpEx_RdpSession* session);

void MsRdpEx_RdpSession_AttachWindow(MsRdpEx_RdpSession* session, HWND hWnd, void* pUserData);

typedef struct _MsRdpEx_SessionManager MsRdpEx_SessionManager;

bool MsRdpEx_SessionManager_Add(MsRdpEx_RdpSession* session);
bool MsRdpEx_SessionManager_Remove(MsRdpEx_RdpSession* session, bool free);

MsRdpEx_RdpSession* MsRdpEx_SessionManager_FindByOutputPresenterHwnd(HWND hWnd);

MsRdpEx_SessionManager* MsRdpEx_SessionManager_Get();
void MsRdpEx_SessionManager_Release();

#ifdef __cplusplus
}
#endif

#endif // MSRDPEX_RDP_SESSION_H
