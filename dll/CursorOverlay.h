#ifndef MSRDPEX_CURSOR_OVERLAY_H
#define MSRDPEX_CURSOR_OVERLAY_H

#include <MsRdpEx/OutputMirror.h>

#ifdef __cplusplus
extern "C" {
#endif

typedef struct _MsRdpEx_CursorOverlay MsRdpEx_CursorOverlay;

MsRdpEx_CursorOverlay* MsRdpEx_CursorOverlay_New();
void MsRdpEx_CursorOverlay_Free(MsRdpEx_CursorOverlay* ctx);

bool MsRdpEx_CursorOverlay_SetShape(MsRdpEx_CursorOverlay* ctx, HCURSOR cursor);
bool MsRdpEx_CursorOverlay_SetPosition(MsRdpEx_CursorOverlay* ctx, int32_t x, int32_t y, bool visible);
void MsRdpEx_CursorOverlay_DumpFrame(MsRdpEx_CursorOverlay* ctx, MsRdpEx_OutputMirror* outputMirror);

#ifdef __cplusplus
}
#endif

#endif /* MSRDPEX_CURSOR_OVERLAY_H */