
#include "MsRdpEx.h"

#include <MsRdpEx/Environment.h>
#include <MsRdpEx/Stopwatch.h>
#include <MsRdpEx/VideoRecorder.h>
#include <MsRdpEx/RecordingManifest.h>
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
	int64_t startTime;
	int videoRecordingCount;
	bool dumpBitmapUpdates;
	bool videoRecordingEnabled;
	uint32_t videoQualityLevel;
	char recordingPath[MSRDPEX_MAX_PATH];
	char recordingPipeName[MSRDPEX_MAX_PATH];
	char outputPath[MSRDPEX_MAX_PATH];
	char sessionId[MSRDPEX_GUID_STRING_SIZE];
	MsRdpEx_VideoRecorder* videoRecorder;
	MsRdpEx_RecordingManifest* manifest;
	FILE* frameMetadataFile;

    CRITICAL_SECTION lock;
};

void MsRdpEx_OutputMirror_Lock(MsRdpEx_OutputMirror* ctx)
{
	MsRdpEx_Stopwatch stopwatch;
	MsRdpEx_Stopwatch_Init(&stopwatch, MSRDPEX_PROF_TRACE, true);

	EnterCriticalSection(&ctx->lock);

	MsRdpEx_Stopwatch_Print(&stopwatch, MSRDPEX_LOG_TRACE, "MsRdpEx_OutputMirror_Lock");
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

		sprintf_s(filename, MSRDPEX_MAX_PATH, "%s\\%s\\frame_%04d.bmp", ctx->recordingPath, ctx->sessionId, ctx->captureIndex);
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

void MsRdpEx_OutputMirror_SetVideoQualityLevel(MsRdpEx_OutputMirror* ctx, uint32_t videoQualityLevel)
{
	ctx->videoQualityLevel = videoQualityLevel;
}

void MsRdpEx_OutputMirror_SetRecordingPath(MsRdpEx_OutputMirror* ctx, const char* recordingPath)
{
	strcpy_s(ctx->recordingPath, MSRDPEX_MAX_PATH, recordingPath);
}

void MsRdpEx_OutputMirror_SetRecordingPipeName(MsRdpEx_OutputMirror* ctx, const char* recordingPipeName)
{
	strcpy_s(ctx->recordingPipeName, MSRDPEX_MAX_PATH, recordingPipeName);
}

void MsRdpEx_OutputMirror_SetSessionId(MsRdpEx_OutputMirror* ctx, const char* sessionId)
{
	strcpy_s(ctx->sessionId, MSRDPEX_GUID_STRING_SIZE, sessionId);
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

	char* envRecordingPath = MsRdpEx_GetEnv("MSRDPEX_RECORDING_PATH");
	if (envRecordingPath) {
		MsRdpEx_OutputMirror_SetRecordingPath(ctx, envRecordingPath);
		free(envRecordingPath);
	}

	if (MsRdpEx_StringIsNullOrEmpty(ctx->recordingPath)) {
		const char* appDataPath = MsRdpEx_GetPath(MSRDPEX_APP_DATA_PATH);
		sprintf_s(ctx->recordingPath, MSRDPEX_MAX_PATH, "%s\\recordings", appDataPath);
	}

	if (MsRdpEx_StringIsNullOrEmpty(ctx->sessionId)) {
		GUID guid;
		MsRdpEx_GuidGenerate(&guid);
		MsRdpEx_GuidBinToStr((GUID*)&guid, ctx->sessionId, 0);
	}

	if (ctx->videoRecordingEnabled) {
		char filename[MSRDPEX_MAX_PATH];

		if (!ctx->manifest) {
			GUID sessionId;
			MsRdpEx_GuidStrToBin(ctx->sessionId, &sessionId, 0);
			ctx->startTime = MsRdpEx_GetUnixTime();
			ctx->manifest = MsRdpEx_RecordingManifest_New();
			MsRdpEx_RecordingManifest_SetSessionId(ctx->manifest, &sessionId);
			MsRdpEx_RecordingManifest_SetStartTime(ctx->manifest, ctx->startTime);
		}

		sprintf_s(ctx->outputPath, MSRDPEX_MAX_PATH, "%s\\%s", ctx->recordingPath, ctx->sessionId);
		MsRdpEx_MakePath(ctx->outputPath, NULL);

		ctx->videoRecorder = MsRdpEx_VideoRecorder_New();

		if (ctx->videoRecorder) {
			int64_t startTime = (ctx->videoRecordingCount < 1) ? ctx->startTime : MsRdpEx_GetUnixTime();
			sprintf_s(filename, MSRDPEX_MAX_PATH, "%s\\recording-%d.webm", ctx->outputPath, ctx->videoRecordingCount);
			MsRdpEx_RecordingManifest_FinalizeFile(ctx->manifest, 0);
			MsRdpEx_RecordingManifest_AddFile(ctx->manifest, MsRdpEx_FileBase(filename), startTime, 0);
			ctx->videoRecordingCount++;
			MsRdpEx_VideoRecorder_SetFrameSize(ctx->videoRecorder, ctx->bitmapWidth, ctx->bitmapHeight);
			MsRdpEx_VideoRecorder_SetFileName(ctx->videoRecorder, filename);
			MsRdpEx_VideoRecorder_SetVideoQuality(ctx->videoRecorder, ctx->videoQualityLevel);

			if (!MsRdpEx_StringIsNullOrEmpty(ctx->recordingPipeName)) {
				MsRdpEx_VideoRecorder_SetPipeName(ctx->videoRecorder, ctx->recordingPipeName);
			}

			MsRdpEx_VideoRecorder_Init(ctx->videoRecorder);
		}

		if (ctx->dumpBitmapUpdates) {
			char metadata[1024];
			sprintf_s(metadata, sizeof(metadata), "FrameTime|FrameSize|FrameFile|UpdatePos|UpdateSize\n");
			sprintf_s(filename, MSRDPEX_MAX_PATH, "%s\\frame_meta.psv", ctx->outputPath);
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

bool MsRdpEx_OutputMirror_WriteManifestFile(MsRdpEx_OutputMirror* ctx)
{
	bool success = false;
	char filename[MSRDPEX_MAX_PATH];

	if (!ctx->manifest)
		return false;

	MsRdpEx_RecordingManifest_FinalizeFile(ctx->manifest, 0);

	sprintf_s(filename, MSRDPEX_MAX_PATH, "%s\\recording.json", ctx->outputPath);
	success = MsRdpEx_RecordingManifest_WriteJsonFile(ctx->manifest, filename);

	return success;
}

MsRdpEx_OutputMirror* MsRdpEx_OutputMirror_New()
{
	MsRdpEx_OutputMirror* ctx;

	ctx = (MsRdpEx_OutputMirror*)calloc(1, sizeof(MsRdpEx_OutputMirror));

	if (!ctx)
		return NULL;

	ctx->bitsPerPixel = 32;
	ctx->videoRecordingEnabled = false;
	ctx->videoQualityLevel = 5;
	ctx->dumpBitmapUpdates = false;

    InitializeCriticalSectionAndSpinCount(&ctx->lock, 4000);

	return ctx;
}

void MsRdpEx_OutputMirror_Free(MsRdpEx_OutputMirror* ctx)
{
	if (!ctx)
		return;

	if (MsRdpEx_StringIsNullOrEmpty(ctx->recordingPipeName)) {
		MsRdpEx_OutputMirror_WriteManifestFile(ctx);
	}

	MsRdpEx_OutputMirror_Uninit(ctx);

	if (ctx->manifest) {
		MsRdpEx_RecordingManifest_Free(ctx->manifest);
		ctx->manifest = NULL;
	}

    DeleteCriticalSection(&ctx->lock);

	free(ctx);
}
