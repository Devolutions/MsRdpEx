
#include "MsRdpEx.h"

#include "Recorder.h"

void MsRdpEx_Recorder_SetSourceDC(MsRdpEx_Recorder* ctx, HDC hSourceDC)
{
	ctx->hSourceDC = hSourceDC;
}

HDC MsRdpEx_Recorder_GetShadowDC(MsRdpEx_Recorder* ctx)
{
	return ctx->hShadowDC;
}

void MsRdpEx_Recorder_SetFrameSize(MsRdpEx_Recorder* ctx, uint32_t frameWidth, uint32_t frameHeight)
{
	ctx->bitmapWidth = frameWidth;
	ctx->bitmapHeight = frameHeight;
}

bool MsRdpEx_Recorder_DumpFrame(MsRdpEx_Recorder* ctx)
{
	char filename[MSRDPEX_MAX_PATH];
	sprintf_s(filename, MSRDPEX_MAX_PATH, "C:\\Windows\\Temp\\MsRdpEx\\image_%04d.bmp", ctx->captureIndex);
	MsRdpEx_WriteBitmapFile(filename, ctx->bitmapData, ctx->bitmapWidth, ctx->bitmapHeight, ctx->bitsPerPixel);
	ctx->captureIndex++;
	return true;
}

bool MsRdpEx_Recorder_Init(MsRdpEx_Recorder* ctx)
{
	ctx->hShadowDC = CreateCompatibleDC(ctx->hSourceDC);
	ctx->hShadowBitmap = MsRdpEx_CreateDIBSection(ctx->hSourceDC,
		ctx->bitmapWidth, ctx->bitmapHeight, ctx->bitsPerPixel, &ctx->bitmapData);
	ctx->hShadowObject = SelectObject(ctx->hShadowDC, ctx->hShadowBitmap);
	return true;
}

bool MsRdpEx_Recorder_Uninit(MsRdpEx_Recorder* ctx)
{
	SelectObject(ctx->hShadowDC, ctx->hShadowObject);
	DeleteObject(ctx->hShadowBitmap);
	ReleaseDC(NULL, ctx->hShadowDC);
	DeleteDC(ctx->hShadowDC);
	return true;
}

MsRdpEx_Recorder* MsRdpEx_Recorder_New()
{
	MsRdpEx_Recorder* ctx;

	ctx = (MsRdpEx_Recorder*)calloc(1, sizeof(MsRdpEx_Recorder));

	if (!ctx)
		return NULL;

	ctx->bitsPerPixel = 32;

	return ctx;
}

void MsRdpEx_Recorder_Free(MsRdpEx_Recorder* ctx)
{
	if (!ctx)
		return;

	MsRdpEx_Recorder_Uninit(ctx);

	free(ctx);
}
