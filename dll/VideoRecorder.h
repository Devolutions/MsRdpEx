#ifndef MSRDPEX_VIDEO_RECORDER_H
#define MSRDPEX_VIDEO_RECORDER_H

#include <MsRdpEx/MsRdpEx.h>

#ifdef __cplusplus
extern "C" {
#endif

typedef void NowRecording;

typedef NowRecording* (CDECL * fnRecording_New)();
typedef void (CDECL* fnRecording_SetSize)(NowRecording* ctx, uint32_t width, uint32_t height);
typedef void (CDECL* fnRecording_SetFileName)(NowRecording* ctx, const char* filename);
typedef void (CDECL* fnRecording_SetDirectory)(NowRecording* ctx, const char* directory);
typedef void (CDECL* fnRecording_Update)(NowRecording* ctx, uint8_t* buffer, uint32_t updateX,
    uint32_t updateY, uint32_t updateWidth, uint32_t updateHeight, uint32_t surfaceStep);
typedef void (CDECL* fnRecording_Timeout)(NowRecording* ctx);
typedef uint32_t (CDECL* fnRecording_GetTimeout)(NowRecording* ctx);
typedef size_t (CDECL* fnRecording_GetPath)(NowRecording* ctx, char* path, size_t size);
typedef void (CDECL* fnRecording_Free)(NowRecording* ctx);

struct _MsRdpEx_VideoRecorder
{
    HMODULE hModule;
    NowRecording* recording;
    fnRecording_New Recording_New;
    fnRecording_SetSize Recording_SetSize;
    fnRecording_SetFileName Recording_SetFileName;
    fnRecording_SetDirectory Recording_SetDirectory;
    fnRecording_Update Recording_Update;
    fnRecording_Timeout Recording_Timeout;
    fnRecording_GetTimeout Recording_GetTimeout;
    fnRecording_GetPath Recording_GetPath;
    fnRecording_Free Recording_Free;
};
typedef struct _MsRdpEx_VideoRecorder MsRdpEx_VideoRecorder;

void MsRdpEx_VideoRecorder_SetFrameSize(MsRdpEx_VideoRecorder* ctx, uint32_t width, uint32_t height);
void MsRdpEx_VideoRecorder_SetFileName(MsRdpEx_VideoRecorder* ctx, const char* filename);
void MsRdpEx_VideoRecorder_SetDirectory(MsRdpEx_VideoRecorder* ctx, const char* directory);

void MsRdpEx_VideoRecorder_Update(MsRdpEx_VideoRecorder* ctx,
    uint8_t* buffer, uint32_t updateX, uint32_t updateY,
    uint32_t updateWidth, uint32_t updateHeight, uint32_t surfaceStep);

void MsRdpEx_VideoRecorder_Timeout(MsRdpEx_VideoRecorder* ctx);
uint32_t MsRdpEx_VideoRecorder_GetTimeout(MsRdpEx_VideoRecorder* ctx);

size_t MsRdpEx_VideoRecorder_GetPath(MsRdpEx_VideoRecorder* ctx, char* path, size_t size);

MsRdpEx_VideoRecorder* MsRdpEx_VideoRecorder_New();
void MsRdpEx_VideoRecorder_Free(MsRdpEx_VideoRecorder* ctx);

#ifdef __cplusplus
}
#endif

#endif // MSRDPEX_VIDEO_RECORDER_H
