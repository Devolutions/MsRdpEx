#ifndef MSRDPEX_VIDEO_RECORDER_H
#define MSRDPEX_VIDEO_RECORDER_H

#include <MsRdpEx/MsRdpEx.h>

#ifdef __cplusplus
extern "C" {
#endif

typedef void XmfRecorder;

typedef XmfRecorder* (CDECL * fnRecorder_New)();
typedef void (CDECL* fnRecorder_SetFrameSize)(XmfRecorder* ctx, uint32_t width, uint32_t height);
typedef void (CDECL* fnRecorder_SetFilename)(XmfRecorder* ctx, const char* filename);
typedef void (CDECL* fnRecorder_UpdateFrame)(XmfRecorder* ctx, uint8_t* buffer, uint32_t updateX,
    uint32_t updateY, uint32_t updateWidth, uint32_t updateHeight, uint32_t surfaceStep);
typedef void (CDECL* fnRecorder_Timeout)(XmfRecorder* ctx);
typedef uint32_t (CDECL* fnRecorder_GetTimeout)(XmfRecorder* ctx);
typedef void (CDECL* fnRecorder_Free)(XmfRecorder* ctx);

struct _MsRdpEx_VideoRecorder
{
    HMODULE hModule;
    XmfRecorder* recorder;
    fnRecorder_New Recorder_New;
    fnRecorder_SetFrameSize Recorder_SetFrameSize;
    fnRecorder_SetFilename Recorder_SetFilename;
    fnRecorder_UpdateFrame Recorder_UpdateFrame;
    fnRecorder_Timeout Recorder_Timeout;
    fnRecorder_GetTimeout Recorder_GetTimeout;
    fnRecorder_Free Recorder_Free;
};
typedef struct _MsRdpEx_VideoRecorder MsRdpEx_VideoRecorder;

void MsRdpEx_VideoRecorder_SetFrameSize(MsRdpEx_VideoRecorder* ctx, uint32_t width, uint32_t height);
void MsRdpEx_VideoRecorder_SetFilename(MsRdpEx_VideoRecorder* ctx, const char* filename);

void MsRdpEx_VideoRecorder_UpdateFrame(MsRdpEx_VideoRecorder* ctx,
    uint8_t* buffer, uint32_t updateX, uint32_t updateY,
    uint32_t updateWidth, uint32_t updateHeight, uint32_t surfaceStep);

void MsRdpEx_VideoRecorder_Timeout(MsRdpEx_VideoRecorder* ctx);
uint32_t MsRdpEx_VideoRecorder_GetTimeout(MsRdpEx_VideoRecorder* ctx);

MsRdpEx_VideoRecorder* MsRdpEx_VideoRecorder_New();
void MsRdpEx_VideoRecorder_Free(MsRdpEx_VideoRecorder* ctx);

#ifdef __cplusplus
}
#endif

#endif // MSRDPEX_VIDEO_RECORDER_H
