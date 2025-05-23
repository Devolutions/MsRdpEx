#ifndef MSRDPEX_VIDEO_RECORDER_H
#define MSRDPEX_VIDEO_RECORDER_H

#include <MsRdpEx/MsRdpEx.h>

#ifdef __cplusplus
extern "C" {
#endif

typedef void XmfRecorder;

typedef XmfRecorder* (CDECL * fnRecorder_New)();
typedef bool (CDECL* fnRecorder_Init)(XmfRecorder* ctx);
typedef void (CDECL* fnRecorder_Uninit)(XmfRecorder* ctx);
typedef void (CDECL* fnRecorder_SetFrameSize)(XmfRecorder* ctx, uint32_t width, uint32_t height);
typedef void (CDECL* fnRecorder_SetFrameRate)(XmfRecorder* ctx, uint32_t frameRate);
typedef void (CDECL* fnRecorder_SetVideoQuality)(XmfRecorder* ctx, uint32_t videoQuality);
typedef void (CDECL* fnRecorder_SetFileName)(XmfRecorder* ctx, const char* filename);
typedef void (CDECL* fnRecorder_UpdateFrame)(XmfRecorder* ctx, uint8_t* buffer, uint32_t updateX,
    uint32_t updateY, uint32_t updateWidth, uint32_t updateHeight, uint32_t surfaceStep);
typedef void (CDECL* fnRecorder_Timeout)(XmfRecorder* ctx);
typedef uint32_t (CDECL* fnRecorder_GetTimeout)(XmfRecorder* ctx);
typedef void (CDECL* fnRecorder_Free)(XmfRecorder* ctx);

typedef void XmfWebMMuxer;
typedef XmfWebMMuxer* (CDECL* fnXmfWebMMuxer_New)();
typedef int (CDECL* fnXmfWebMMuxer_Remux)(XmfWebMMuxer* ctx, const char* inputFile, const char* outputFile);
typedef void (CDECL* fnXmfWebMMuxer_Free)(XmfWebMMuxer* ctx);

struct _MsRdpEx_VideoRecorder
{
    HMODULE hModule;
    XmfRecorder* recorder;
    fnRecorder_New Recorder_New;
    fnRecorder_Init Recorder_Init;
    fnRecorder_Uninit Recorder_Uninit;
    fnRecorder_SetFrameSize Recorder_SetFrameSize;
    fnRecorder_SetFrameRate Recorder_SetFrameRate;
    fnRecorder_SetVideoQuality Recorder_SetVideoQuality;
    fnRecorder_SetFileName Recorder_SetFileName;
    fnRecorder_UpdateFrame Recorder_UpdateFrame;
    fnRecorder_Timeout Recorder_Timeout;
    fnRecorder_GetTimeout Recorder_GetTimeout;
    fnRecorder_Free Recorder_Free;

    fnXmfWebMMuxer_New XmfWebMMuxer_New;
    fnXmfWebMMuxer_Remux XmfWebMMuxer_Remux;
    fnXmfWebMMuxer_Free XmfWebMMuxer_Free;

    char filename[MSRDPEX_MAX_PATH];
};
typedef struct _MsRdpEx_VideoRecorder MsRdpEx_VideoRecorder;

bool MsRdpEx_VideoRecorder_Init(MsRdpEx_VideoRecorder* ctx);
void MsRdpEx_VideoRecorder_Uninit(MsRdpEx_VideoRecorder* ctx);

void MsRdpEx_VideoRecorder_SetFrameSize(MsRdpEx_VideoRecorder* ctx, uint32_t width, uint32_t height);
void MsRdpEx_VideoRecorder_SetFrameRate(MsRdpEx_VideoRecorder* ctx, uint32_t frameRate);
void MsRdpEx_VideoRecorder_SetVideoQuality(MsRdpEx_VideoRecorder* ctx, uint32_t videoQuality);

void MsRdpEx_VideoRecorder_SetFileName(MsRdpEx_VideoRecorder* ctx, const char* filename);

void MsRdpEx_VideoRecorder_UpdateFrame(MsRdpEx_VideoRecorder* ctx,
    uint8_t* buffer, uint32_t updateX, uint32_t updateY,
    uint32_t updateWidth, uint32_t updateHeight, uint32_t surfaceStep);

void MsRdpEx_VideoRecorder_Timeout(MsRdpEx_VideoRecorder* ctx);
uint32_t MsRdpEx_VideoRecorder_GetTimeout(MsRdpEx_VideoRecorder* ctx);

bool MsRdpEx_VideoRecorder_Remux(MsRdpEx_VideoRecorder* ctx, const char* filename);

MsRdpEx_VideoRecorder* MsRdpEx_VideoRecorder_New();
void MsRdpEx_VideoRecorder_Free(MsRdpEx_VideoRecorder* ctx);

#ifdef __cplusplus
}
#endif

#endif // MSRDPEX_VIDEO_RECORDER_H
