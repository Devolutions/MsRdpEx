#ifndef MSRDPEX_RECORDER_H
#define MSRDPEX_RECORDER_H

#include <MsRdpEx/MsRdpEx.h>

#ifdef __cplusplus
extern "C" {
#endif

struct _MsRdpEx_Recorder
{
	uint8_t* bitmapData;
	uint32_t bitmapWidth;
	uint32_t bitmapHeight;
	uint32_t bitsPerPixel;

	HDC hSourceDC;
	HDC hShadowDC;
	HBITMAP hShadowBitmap;
	HGDIOBJ hShadowObject;
	uint32_t captureIndex;
};
typedef struct _MsRdpEx_Recorder MsRdpEx_Recorder;

void MsRdpEx_Recorder_SetSourceDC(MsRdpEx_Recorder* ctx, HDC hSourceDC);
HDC MsRdpEx_Recorder_GetShadowDC(MsRdpEx_Recorder* ctx);

void MsRdpEx_Recorder_SetFrameSize(MsRdpEx_Recorder* ctx, uint32_t frameWidth, uint32_t frameHeight);
bool MsRdpEx_Recorder_DumpFrame(MsRdpEx_Recorder* ctx);

bool MsRdpEx_Recorder_Init(MsRdpEx_Recorder* ctx);

MsRdpEx_Recorder* MsRdpEx_Recorder_New();
void MsRdpEx_Recorder_Free(MsRdpEx_Recorder* ctx);

#ifdef __cplusplus
}
#endif

#endif // MSRDPEX_RECORDER_H
