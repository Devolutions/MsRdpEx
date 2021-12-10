
#include "MsRdpEx.h"

#include "VideoRecorder.h"

#include "OutputMirror.h"

static MsRdpEx_VideoRecorder* g_VideoRecorder = NULL;

void MsRdpEx_OutputMirror_SetSourceDC(MsRdpEx_OutputMirror* ctx, HDC hSourceDC)
{
	ctx->hSourceDC = hSourceDC;
}

HDC MsRdpEx_OutputMirror_GetShadowDC(MsRdpEx_OutputMirror* ctx)
{
	return ctx->hShadowDC;
}

void MsRdpEx_OutputMirror_SetFrameSize(MsRdpEx_OutputMirror* ctx, uint32_t frameWidth, uint32_t frameHeight)
{
	ctx->bitmapWidth = frameWidth;
	ctx->bitmapHeight = frameHeight;
	ctx->bitmapStep = frameWidth * 4;
}

bool MsRdpEx_OutputMirror_DumpFrame(MsRdpEx_OutputMirror* ctx)
{
	char filename[MSRDPEX_MAX_PATH];

	if (g_VideoRecorder) {
		size_t size = MsRdpEx_VideoRecorder_GetPath(g_VideoRecorder, filename, MSRDPEX_MAX_PATH);
		filename[size - 1] = '\0';
		MsRdpEx_Log("DumpFrame: %s", filename);
		MsRdpEx_VideoRecorder_Update(g_VideoRecorder, ctx->bitmapData,
			0, 0, ctx->bitmapWidth, ctx->bitmapHeight, ctx->bitmapStep);
		MsRdpEx_VideoRecorder_Timeout(g_VideoRecorder);
	} else {
		sprintf_s(filename, MSRDPEX_MAX_PATH, "C:\\Windows\\Temp\\MsRdpEx\\image_%04d.bmp", ctx->captureIndex);
		MsRdpEx_Log("DumpFrame: %s", filename);
		MsRdpEx_WriteBitmapFile(filename, ctx->bitmapData, ctx->bitmapWidth, ctx->bitmapHeight, ctx->bitsPerPixel);
	}

	ctx->captureIndex++;
	return true;
}

bool MsRdpEx_OutputMirror_Init(MsRdpEx_OutputMirror* ctx)
{
	ctx->hShadowDC = CreateCompatibleDC(ctx->hSourceDC);
	ctx->hShadowBitmap = MsRdpEx_CreateDIBSection(ctx->hSourceDC,
		ctx->bitmapWidth, ctx->bitmapHeight, ctx->bitsPerPixel, &ctx->bitmapData);
	ctx->hShadowObject = SelectObject(ctx->hShadowDC, ctx->hShadowBitmap);

	g_VideoRecorder = MsRdpEx_VideoRecorder_New();
	
	if (g_VideoRecorder) {
		MsRdpEx_VideoRecorder_SetFrameSize(g_VideoRecorder, ctx->bitmapWidth, ctx->bitmapHeight);
		MsRdpEx_VideoRecorder_SetDirectory(g_VideoRecorder, "C:\\Windows\\Temp\\MsRdpEx");
		MsRdpEx_VideoRecorder_SetFileName(g_VideoRecorder, "MsRdpEx.webm");
	}

	return true;
}

bool MsRdpEx_OutputMirror_Uninit(MsRdpEx_OutputMirror* ctx)
{
	if (ctx->hShadowDC)
	{
		SelectObject(ctx->hShadowDC, ctx->hShadowObject);
		DeleteObject(ctx->hShadowBitmap);
		ctx->hShadowObject = NULL;
		ctx->hShadowBitmap = NULL;
	}

	if (ctx->hShadowDC)
	{
		DeleteDC(ctx->hShadowDC);
		ctx->hShadowDC = NULL;
	}

	if (g_VideoRecorder) {
		MsRdpEx_VideoRecorder_Free(g_VideoRecorder);
		g_VideoRecorder = NULL;
	}
	
	return true;
}

MsRdpEx_OutputMirror* MsRdpEx_OutputMirror_New()
{
	MsRdpEx_OutputMirror* ctx;

	ctx = (MsRdpEx_OutputMirror*)calloc(1, sizeof(MsRdpEx_OutputMirror));

	if (!ctx)
		return NULL;

	ctx->bitsPerPixel = 32;

	return ctx;
}

void MsRdpEx_OutputMirror_Free(MsRdpEx_OutputMirror* ctx)
{
	if (!ctx)
		return;

	MsRdpEx_OutputMirror_Uninit(ctx);

	free(ctx);
}
