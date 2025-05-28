#ifndef MSRDPEX_VIDEO_RECORDER_H
#define MSRDPEX_VIDEO_RECORDER_H

#include <MsRdpEx/MsRdpEx.h>

#ifdef __cplusplus
extern "C" {
#endif

typedef void XmfRecorder;
typedef void XmfWebMMuxer;
typedef void XmfBipBlock;
typedef void XmfBipBuffer;

typedef XmfRecorder* (CDECL * fnXmfRecorder_New)();
typedef bool (CDECL* fnXmfRecorder_Init)(XmfRecorder* ctx);
typedef void (CDECL* fnXmfRecorder_Uninit)(XmfRecorder* ctx);
typedef void (CDECL* fnXmfRecorder_SetFrameSize)(XmfRecorder* ctx, uint32_t width, uint32_t height);
typedef void (CDECL* fnXmfRecorder_SetFrameRate)(XmfRecorder* ctx, uint32_t frameRate);
typedef void (CDECL* fnXmfRecorder_SetVideoQuality)(XmfRecorder* ctx, uint32_t videoQuality);
typedef void (CDECL* fnXmfRecorder_SetFileName)(XmfRecorder* ctx, const char* filename);
typedef void (CDECL* fnXmfRecorder_SetBipBuffer)(XmfRecorder* ctx, XmfBipBuffer* bb);
typedef void (CDECL* fnXmfRecorder_UpdateFrame)(XmfRecorder* ctx, uint8_t* buffer, uint32_t updateX,
    uint32_t updateY, uint32_t updateWidth, uint32_t updateHeight, uint32_t surfaceStep);
typedef void (CDECL* fnXmfRecorder_Timeout)(XmfRecorder* ctx);
typedef uint32_t (CDECL* fnXmfRecorder_GetTimeout)(XmfRecorder* ctx);
typedef void (CDECL* fnXmfRecorder_Free)(XmfRecorder* ctx);

typedef XmfWebMMuxer* (CDECL* fnXmfWebMMuxer_New)();
typedef int (CDECL* fnXmfWebMMuxer_Remux)(XmfWebMMuxer* ctx, const char* inputFile, const char* outputFile);
typedef void (CDECL* fnXmfWebMMuxer_Free)(XmfWebMMuxer* ctx);

typedef bool (CDECL* fnXmfBipBuffer_Grow)(XmfBipBuffer* ctx, size_t size);
typedef void (CDECL* fnXmfBipBuffer_Clear)(XmfBipBuffer* ctx);
typedef size_t(CDECL* fnXmfBipBuffer_UsedSize)(XmfBipBuffer* ctx);
typedef size_t(CDECL* fnXmfBipBuffer_BufferSize)(XmfBipBuffer* ctx);
typedef uint8_t* (CDECL* fnXmfBipBuffer_WriteReserve)(XmfBipBuffer* ctx, size_t size);
typedef uint8_t* (CDECL* fnXmfBipBuffer_WriteTryReserve)(XmfBipBuffer* ctx, size_t size, size_t* reserved);
typedef void (CDECL* fnXmfBipBuffer_WriteCommit)(XmfBipBuffer* ctx, size_t size);
typedef uint8_t* (CDECL* fnXmfBipBuffer_ReadReserve)(XmfBipBuffer* ctx, size_t size);
typedef uint8_t* (CDECL* fnXmfBipBuffer_ReadTryReserve)(XmfBipBuffer* ctx, size_t size, size_t* reserved);
typedef void (CDECL* fnXmfBipBuffer_ReadCommit)(XmfBipBuffer* ctx, size_t size);
typedef int (CDECL* fnXmfBipBuffer_Read)(XmfBipBuffer* ctx, uint8_t* data, size_t size);
typedef int (CDECL* fnXmfBipBuffer_Write)(XmfBipBuffer* ctx, const uint8_t* data, size_t size);
typedef void (CDECL* fnXmfBipBuffer_SetSignaledState)(XmfBipBuffer* ctx, bool signaled);
typedef bool (CDECL* fnXmfBipBuffer_GetSignaledState)(XmfBipBuffer* ctx);
typedef XmfBipBuffer* (CDECL* fnXmfBipBuffer_New)(size_t size);
typedef void (CDECL* fnXmfBipBuffer_Free)(XmfBipBuffer* ctx);

struct _MsRdpEx_VideoRecorder
{
    HMODULE hModule;
    XmfRecorder* recorder;
    fnXmfRecorder_New XmfRecorder_New;
    fnXmfRecorder_Init XmfRecorder_Init;
    fnXmfRecorder_Uninit XmfRecorder_Uninit;
    fnXmfRecorder_SetFrameSize XmfRecorder_SetFrameSize;
    fnXmfRecorder_SetFrameRate XmfRecorder_SetFrameRate;
    fnXmfRecorder_SetVideoQuality XmfRecorder_SetVideoQuality;
    fnXmfRecorder_SetFileName XmfRecorder_SetFileName;
    fnXmfRecorder_SetBipBuffer XmfRecorder_SetBipBuffer;
    fnXmfRecorder_UpdateFrame XmfRecorder_UpdateFrame;
    fnXmfRecorder_Timeout XmfRecorder_Timeout;
    fnXmfRecorder_GetTimeout XmfRecorder_GetTimeout;
    fnXmfRecorder_Free XmfRecorder_Free;

    fnXmfWebMMuxer_New XmfWebMMuxer_New;
    fnXmfWebMMuxer_Remux XmfWebMMuxer_Remux;
    fnXmfWebMMuxer_Free XmfWebMMuxer_Free;

    fnXmfBipBuffer_Grow XmfBipBuffer_Grow;
    fnXmfBipBuffer_Clear XmfBipBuffer_Clear;
    fnXmfBipBuffer_UsedSize XmfBipBuffer_UsedSize;
    fnXmfBipBuffer_BufferSize XmfBipBuffer_BufferSize;
    fnXmfBipBuffer_WriteReserve XmfBipBuffer_WriteReserve;
    fnXmfBipBuffer_WriteTryReserve XmfBipBuffer_WriteTryReserve;
    fnXmfBipBuffer_WriteCommit XmfBipBuffer_WriteCommit;
    fnXmfBipBuffer_ReadReserve XmfBipBuffer_ReadReserve;
    fnXmfBipBuffer_ReadTryReserve XmfBipBuffer_ReadTryReserve;
    fnXmfBipBuffer_ReadCommit XmfBipBuffer_ReadCommit;
    fnXmfBipBuffer_Read XmfBipBuffer_Read;
    fnXmfBipBuffer_Write XmfBipBuffer_Write;
    fnXmfBipBuffer_SetSignaledState XmfBipBuffer_SetSignaledState;
    fnXmfBipBuffer_GetSignaledState XmfBipBuffer_GetSignaledState;
    fnXmfBipBuffer_New XmfBipBuffer_New;
    fnXmfBipBuffer_Free XmfBipBuffer_Free;

    FILE* fp;
    HANDLE np_handle;
    char* np_name;
    bool useBipBuffer;
    XmfBipBuffer* bb;
    size_t bipBufferSize;
    char filename[MSRDPEX_MAX_PATH];
};
typedef struct _MsRdpEx_VideoRecorder MsRdpEx_VideoRecorder;

bool MsRdpEx_VideoRecorder_Init(MsRdpEx_VideoRecorder* ctx);
void MsRdpEx_VideoRecorder_Uninit(MsRdpEx_VideoRecorder* ctx);

void MsRdpEx_VideoRecorder_SetFrameSize(MsRdpEx_VideoRecorder* ctx, uint32_t width, uint32_t height);
void MsRdpEx_VideoRecorder_SetFrameRate(MsRdpEx_VideoRecorder* ctx, uint32_t frameRate);
void MsRdpEx_VideoRecorder_SetVideoQuality(MsRdpEx_VideoRecorder* ctx, uint32_t videoQuality);

void MsRdpEx_VideoRecorder_SetFileName(MsRdpEx_VideoRecorder* ctx, const char* filename);
void MsRdpEx_VideoRecorder_SetPipeName(MsRdpEx_VideoRecorder* ctx, const char* pipeName);

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
