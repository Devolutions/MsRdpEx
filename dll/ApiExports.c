
#include "MsRdpEx.h"

#include <MsRdpEx/MsRdpEx.h>

#include <MsRdpEx/NameResolver.h>
#include <MsRdpEx/RdpSession.h>
#include <MsRdpEx/Process.h>

#include <MsRdpEx/OutputMirror.h>

bool MsRdpEx_GetShadowBitmap(HWND hWnd, HDC* phDC, HBITMAP* phBitmap, uint32_t* pWidth, uint32_t* pHeight)
{
    MsRdpEx_RdpSession* session = NULL;
    MsRdpEx_OutputMirror* outputMirror = NULL;

    session = MsRdpEx_SessionManager_FindByOutputPresenterHwnd(hWnd);

    if (!session)
        return false;

    outputMirror = session->outputMirror;

    if (!outputMirror)
        return false;

    *phDC = outputMirror->hShadowDC;
    *phBitmap = outputMirror->hShadowBitmap;
    *pWidth = outputMirror->bitmapWidth;
    *pHeight = outputMirror->bitmapHeight;

    return true;
}
