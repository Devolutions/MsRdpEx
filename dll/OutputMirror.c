
#include "MsRdpEx.h"

#include <MsRdpEx/Environment.h>
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
	uint64_t captureBaseTime;
	char capturePath[MSRDPEX_MAX_PATH];

	bool dumpBitmapUpdates;
	bool videoRecordingEnabled;
	MsRdpEx_VideoRecorder* videoRecorder;
	FILE* frameMetadataFile;

    CRITICAL_SECTION lock;
};

void MsRdpEx_OutputMirror_Lock(MsRdpEx_OutputMirror* ctx)
{
	EnterCriticalSection(&ctx->lock);
}

void MsRdpEx_OutputMirror_Unlock(MsRdpEx_OutputMirror* ctx)
{
	LeaveCriticalSection(&ctx->lock);
}

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
	uint64_t captureTime;
	char filename[MSRDPEX_MAX_PATH];

	captureTime = GetTickCount64() - ctx->captureBaseTime;

	if (ctx->videoRecordingEnabled && ctx->videoRecorder) {
		MsRdpEx_VideoRecorder_UpdateFrame(ctx->videoRecorder, ctx->bitmapData,
			0, 0, ctx->bitmapWidth, ctx->bitmapHeight, ctx->bitmapStep);
		MsRdpEx_VideoRecorder_Timeout(ctx->videoRecorder);
	}

	if (ctx->dumpBitmapUpdates) {
		char metadata[1024];

		sprintf_s(filename, MSRDPEX_MAX_PATH, "%s\\frame_%04d.bmp", ctx->capturePath, ctx->captureIndex);
		MsRdpEx_WriteBitmapFile(filename, ctx->bitmapData, ctx->bitmapWidth, ctx->bitmapHeight, ctx->bitsPerPixel);

		sprintf_s(metadata, sizeof(metadata), "%llu|%dx%d|%s|%dx%d|%dx%d\n",
			captureTime,
			ctx->bitmapWidth, ctx->bitmapHeight,
			MsRdpEx_FileBase(filename),
			0, 0, ctx->bitmapWidth, ctx->bitmapHeight);
		fwrite(metadata, 1, strlen(metadata), ctx->frameMetadataFile);
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
	HDC* phDC, HBITMAP* phBitmap, uint8_t** pBitmapData,
	uint32_t* pBitmapWidth, uint32_t* pBitmapHeight, uint32_t* pBitmapStep)
{
	if (phDC)
		*phDC = ctx->hShadowDC;

	if (phBitmap)
		*phBitmap = ctx->hShadowBitmap;

	if (pBitmapData)
		*pBitmapData = ctx->bitmapData;

	if (pBitmapWidth)
		*pBitmapWidth = ctx->bitmapWidth;

	if (pBitmapHeight)
		*pBitmapHeight = ctx->bitmapHeight;

	if (pBitmapStep)
		*pBitmapStep = ctx->bitmapStep;
	
	return true;
}

bool MsRdpEx_OutputMirror_Init(MsRdpEx_OutputMirror* ctx)
{
	ctx->hShadowDC = CreateCompatibleDC(ctx->hSourceDC);
	ctx->hShadowBitmap = MsRdpEx_CreateDIBSection(ctx->hSourceDC,
		ctx->bitmapWidth, ctx->bitmapHeight, ctx->bitsPerPixel, &ctx->bitmapData);
	ctx->hShadowObject = SelectObject(ctx->hShadowDC, ctx->hShadowBitmap);

	ctx->captureBaseTime = GetTickCount64();

	char* capturePath = MsRdpEx_GetEnv("MSRDPEX_CAPTURE_PATH");

	if (capturePath) {
		strcpy_s(ctx->capturePath, MSRDPEX_MAX_PATH, capturePath);
		free(capturePath);
	} else {
		const char* appDataPath = MsRdpEx_GetPath(MSRDPEX_APP_DATA_PATH);
		sprintf_s(ctx->capturePath, MSRDPEX_MAX_PATH, "%s\\capture", appDataPath);
	}

	if (ctx->videoRecordingEnabled) {
		char filename[MSRDPEX_MAX_PATH];
		uint64_t timestamp = MsRdpEx_GetUnixTime();

		MsRdpEx_MakePath(ctx->capturePath, NULL);

		ctx->videoRecorder = MsRdpEx_VideoRecorder_New();

		if (ctx->videoRecorder) {
			char* videoFileName = MsRdpEx_GetEnv("MSRDPEX_VIDEO_FILENAME");

			if (videoFileName) {
				strcpy_s(filename, MSRDPEX_MAX_PATH, videoFileName);
				free(videoFileName);
			} else {
				sprintf_s(filename, MSRDPEX_MAX_PATH, "%s\\%llu.webm", ctx->capturePath, timestamp);
			}

			MsRdpEx_VideoRecorder_SetFrameSize(ctx->videoRecorder, ctx->bitmapWidth, ctx->bitmapHeight);
			MsRdpEx_VideoRecorder_SetFileName(ctx->videoRecorder, filename);
			MsRdpEx_VideoRecorder_Init(ctx->videoRecorder);
		}

		if (ctx->dumpBitmapUpdates) {
			char metadata[1024];
			sprintf_s(metadata, sizeof(metadata), "FrameTime|FrameSize|FrameFile|UpdatePos|UpdateSize\n");
			sprintf_s(filename, MSRDPEX_MAX_PATH, "%s\\frame_meta.psv", ctx->capturePath);
			ctx->frameMetadataFile = MsRdpEx_FileOpen(filename, "wb");
			fwrite(metadata, 1, strlen(metadata), ctx->frameMetadataFile);
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
		MsRdpEx_VideoRecorder_Uninit(ctx->videoRecorder);
		MsRdpEx_VideoRecorder_Remux(ctx->videoRecorder, NULL);
		MsRdpEx_VideoRecorder_Free(ctx->videoRecorder);
		ctx->videoRecorder = NULL;
	}

	if (ctx->frameMetadataFile) {
		fclose(ctx->frameMetadataFile);
		ctx->frameMetadataFile = NULL;
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

    InitializeCriticalSectionAndSpinCount(&ctx->lock, 4000);

	return ctx;
}

void MsRdpEx_OutputMirror_Free(MsRdpEx_OutputMirror* ctx)
{
	if (!ctx)
		return;

	MsRdpEx_OutputMirror_Uninit(ctx);

    DeleteCriticalSection(&ctx->lock);

	free(ctx);
}
