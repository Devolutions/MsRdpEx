
#include "MsRdpEx.h"

#include <MsRdpEx/VideoRecorder.h>
#include <MsRdpEx/OutputMirror.h>

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

	bool dumpBitmapUpdates;
	bool videoRecordingEnabled;
	MsRdpEx_VideoRecorder* videoRecorder;
};

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

void MsRdpEx_OutputMirror_GetFrameSize(MsRdpEx_OutputMirror* ctx, uint32_t* frameWidth, uint32_t* frameHeight)
{
	*frameWidth = ctx->bitmapWidth;
	*frameHeight = ctx->bitmapHeight;
}

bool MsRdpEx_OutputMirror_DumpFrame(MsRdpEx_OutputMirror* ctx)
{
	char filename[MSRDPEX_MAX_PATH];

	if (ctx->videoRecordingEnabled && ctx->videoRecorder) {
		MsRdpEx_VideoRecorder_UpdateFrame(ctx->videoRecorder, ctx->bitmapData,
			0, 0, ctx->bitmapWidth, ctx->bitmapHeight, ctx->bitmapStep);
		MsRdpEx_VideoRecorder_Timeout(ctx->videoRecorder);
	}

	if (ctx->dumpBitmapUpdates) {
		const char* appDataPath = MsRdpEx_GetPath(MSRDPEX_APP_DATA_PATH);
		sprintf_s(filename, MSRDPEX_MAX_PATH, "%s\\image_%04d.bmp", appDataPath, ctx->captureIndex);
		MsRdpEx_WriteBitmapFile(filename, ctx->bitmapData, ctx->bitmapWidth, ctx->bitmapHeight, ctx->bitsPerPixel);
	}

	ctx->captureIndex++;
	return true;
}

void MsRdpEx_OutputMirror_SetDumpBitmapUpdates(MsRdpEx_OutputMirror* ctx, bool dumpBitmapUpdates)
{
	ctx->dumpBitmapUpdates = dumpBitmapUpdates;
}

void MsRdpEx_OutputMirror_SetVideoRecordingEnabled(MsRdpEx_OutputMirror* ctx, bool videoRecordingEnabled)
{
	ctx->videoRecordingEnabled = videoRecordingEnabled;
}

bool MsRdpEx_OutputMirror_GetShadowBitmap(MsRdpEx_OutputMirror* ctx,
	HDC* phDC, HBITMAP* phBitmap, uint32_t* pWidth, uint32_t* pHeight)
{
	*phDC = ctx->hShadowDC;
	*phBitmap = ctx->hShadowBitmap;
	*pWidth = ctx->bitmapWidth;
	*pHeight = ctx->bitmapHeight;
	return true;
}

bool MsRdpEx_OutputMirror_Init(MsRdpEx_OutputMirror* ctx)
{
	ctx->hShadowDC = CreateCompatibleDC(ctx->hSourceDC);
	ctx->hShadowBitmap = MsRdpEx_CreateDIBSection(ctx->hSourceDC,
		ctx->bitmapWidth, ctx->bitmapHeight, ctx->bitsPerPixel, &ctx->bitmapData);
	ctx->hShadowObject = SelectObject(ctx->hShadowDC, ctx->hShadowBitmap);

	if (ctx->videoRecordingEnabled) {
		ctx->videoRecorder = MsRdpEx_VideoRecorder_New();

		if (ctx->videoRecorder) {
			char filename[MSRDPEX_MAX_PATH];
			uint64_t timestamp = MsRdpEx_GetUnixTime();
			const char* appDataPath = MsRdpEx_GetPath(MSRDPEX_APP_DATA_PATH);
			sprintf_s(filename, MSRDPEX_MAX_PATH, "%s\\%llu.webm", appDataPath, timestamp);
			MsRdpEx_VideoRecorder_SetFrameSize(ctx->videoRecorder, ctx->bitmapWidth, ctx->bitmapHeight);
			MsRdpEx_VideoRecorder_SetFilename(ctx->videoRecorder, filename);
		}
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

	if (ctx->videoRecorder) {
		MsRdpEx_VideoRecorder_Free(ctx->videoRecorder);
		ctx->videoRecorder = NULL;
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
	ctx->videoRecordingEnabled = false;
	ctx->dumpBitmapUpdates = false;

	return ctx;
}

void MsRdpEx_OutputMirror_Free(MsRdpEx_OutputMirror* ctx)
{
	if (!ctx)
		return;

	MsRdpEx_OutputMirror_Uninit(ctx);

	free(ctx);
}
