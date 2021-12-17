#ifndef MSRDPEX_OUTPUT_MIRROR_H
#define MSRDPEX_OUTPUT_MIRROR_H

#include <MsRdpEx/MsRdpEx.h>

#include <MsRdpEx/VideoRecorder.h>

#ifdef __cplusplus
extern "C" {
#endif

struct _MsRdpEx_OutputMirror
{
	uint8_t* bitmapData;
	uint32_t bitmapWidth;
	uint32_t bitmapHeight;
	uint32_t bitsPerPixel;
	uint32_t bitmapStep;

	HDC hSourceDC;
	HDC hShadowDC;
	HBITMAP hShadowBitmap;
	HGDIOBJ hShadowObject;
	uint32_t captureIndex;

	MsRdpEx_VideoRecorder* videoRecorder;
};
typedef struct _MsRdpEx_OutputMirror MsRdpEx_OutputMirror;

void MsRdpEx_OutputMirror_SetSourceDC(MsRdpEx_OutputMirror* ctx, HDC hSourceDC);
HDC MsRdpEx_OutputMirror_GetShadowDC(MsRdpEx_OutputMirror* ctx);

void MsRdpEx_OutputMirror_SetFrameSize(MsRdpEx_OutputMirror* ctx, uint32_t frameWidth, uint32_t frameHeight);
bool MsRdpEx_OutputMirror_DumpFrame(MsRdpEx_OutputMirror* ctx);

bool MsRdpEx_OutputMirror_Init(MsRdpEx_OutputMirror* ctx);

MsRdpEx_OutputMirror* MsRdpEx_OutputMirror_New();
void MsRdpEx_OutputMirror_Free(MsRdpEx_OutputMirror* ctx);

#ifdef __cplusplus
}
#endif

#endif // MSRDPEX_OUTPUT_MIRROR_H
