#ifndef MSRDPEX_OUTPUT_MIRROR_H
#define MSRDPEX_OUTPUT_MIRROR_H

#include <MsRdpEx/MsRdpEx.h>

#include <MsRdpEx/VideoRecorder.h>

#ifdef __cplusplus
extern "C" {
#endif

typedef struct _MsRdpEx_OutputMirror MsRdpEx_OutputMirror;

void MsRdpEx_OutputMirror_SetSourceDC(MsRdpEx_OutputMirror* ctx, HDC hSourceDC);
HDC MsRdpEx_OutputMirror_GetShadowDC(MsRdpEx_OutputMirror* ctx);

void MsRdpEx_OutputMirror_SetFrameSize(MsRdpEx_OutputMirror* ctx, uint32_t frameWidth, uint32_t frameHeight);
void MsRdpEx_OutputMirror_GetFrameSize(MsRdpEx_OutputMirror* ctx, uint32_t* frameWidth, uint32_t* frameHeight);

bool MsRdpEx_OutputMirror_DumpFrame(MsRdpEx_OutputMirror* ctx);

void MsRdpEx_OutputMirror_SetDumpBitmapUpdates(MsRdpEx_OutputMirror* ctx, bool dumpBitmapUpdates);
void MsRdpEx_OutputMirror_SetVideoRecordingEnabled(MsRdpEx_OutputMirror* ctx, bool videoRecordingEnabled);
void MsRdpEx_OutputMirror_SetVideoQualityLevel(MsRdpEx_OutputMirror* ctx, uint32_t videoQualityLevel);
void MsRdpEx_OutputMirror_SetRecordingPath(MsRdpEx_OutputMirror* ctx, const char* recordingPath);
void MsRdpEx_OutputMirror_SetSessionId(MsRdpEx_OutputMirror* ctx, const char* sessionId);

bool MsRdpEx_OutputMirror_GetShadowBitmap(MsRdpEx_OutputMirror* ctx,
	HDC* phDC, HBITMAP* phBitmap, uint8_t** pBitmapData,
	uint32_t* pBitmapWidth, uint32_t* pBitmapHeight, uint32_t* pBitmapStep);

void MsRdpEx_OutputMirror_Lock(MsRdpEx_OutputMirror* ctx);
void MsRdpEx_OutputMirror_Unlock(MsRdpEx_OutputMirror* ctx);

bool MsRdpEx_OutputMirror_Init(MsRdpEx_OutputMirror* ctx);
bool MsRdpEx_OutputMirror_Uninit(MsRdpEx_OutputMirror* ctx);

MsRdpEx_OutputMirror* MsRdpEx_OutputMirror_New();
void MsRdpEx_OutputMirror_Free(MsRdpEx_OutputMirror* ctx);

#ifdef __cplusplus
}
#endif

#endif // MSRDPEX_OUTPUT_MIRROR_H
