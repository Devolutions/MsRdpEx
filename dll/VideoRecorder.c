
#include "MsRdpEx.h"

#include <MsRdpEx/Environment.h>
#include <MsRdpEx/VideoRecorder.h>

void* MsRdpEx_LoadFunc(HMODULE hModule, const char* name, void** ppFunc)
{
    void* pFunc = GetProcAddress(hModule, name);
    *ppFunc = pFunc;
    if (!pFunc) {
        MsRdpEx_LogPrint(DEBUG, "LoadFunc(%s): not found", name);
    }
    return pFunc;
}

MsRdpEx_VideoRecorder* MsRdpEx_VideoRecorder_New()
{
    HMODULE hModule;
    MsRdpEx_VideoRecorder* ctx;
    const char* libPath = MsRdpEx_GetPath(MSRDPEX_XMF_DLL_PATH);

    if (MsRdpEx_StringIsNullOrEmpty(libPath)) {
        MsRdpEx_LogPrint(DEBUG, "no xmf.dll (cadeau) library detected");
        return NULL;
    }

    hModule = MsRdpEx_LoadLibrary(libPath);

    if (!hModule) {
        MsRdpEx_LogPrint(DEBUG, "Could not load cadeau library: '%s'", libPath);
        return NULL;
    }

    ctx = (MsRdpEx_VideoRecorder*) malloc(sizeof(MsRdpEx_VideoRecorder));

    if (!ctx)
        return NULL;

    ZeroMemory(ctx, sizeof(MsRdpEx_VideoRecorder));

    ctx->hModule = hModule;

    MsRdpEx_LoadFunc(hModule, "XmfRecorder_New", (void**)&ctx->Recorder_New);
    MsRdpEx_LoadFunc(hModule, "XmfRecorder_Init", (void**)&ctx->Recorder_Init);
    MsRdpEx_LoadFunc(hModule, "XmfRecorder_Uninit", (void**)&ctx->Recorder_Uninit);
    MsRdpEx_LoadFunc(hModule, "XmfRecorder_SetFrameSize", (void**)&ctx->Recorder_SetFrameSize);
    MsRdpEx_LoadFunc(hModule, "XmfRecorder_SetFrameRate", (void**)&ctx->Recorder_SetFrameRate);
    MsRdpEx_LoadFunc(hModule, "XmfRecorder_SetVideoQuality", (void**)&ctx->Recorder_SetVideoQuality);
    MsRdpEx_LoadFunc(hModule, "XmfRecorder_SetFileName", (void**)&ctx->Recorder_SetFileName);
    MsRdpEx_LoadFunc(hModule, "XmfRecorder_UpdateFrame", (void**)&ctx->Recorder_UpdateFrame);
    MsRdpEx_LoadFunc(hModule, "XmfRecorder_Timeout", (void**)&ctx->Recorder_Timeout);
    MsRdpEx_LoadFunc(hModule, "XmfRecorder_GetTimeout", (void**)&ctx->Recorder_GetTimeout);
    MsRdpEx_LoadFunc(hModule, "XmfRecorder_Free", (void**)&ctx->Recorder_Free);

    MsRdpEx_LoadFunc(hModule, "XmfWebMMuxer_New", (void**)&ctx->XmfWebMMuxer_New);
    MsRdpEx_LoadFunc(hModule, "XmfWebMMuxer_Remux", (void**)&ctx->XmfWebMMuxer_Remux);
    MsRdpEx_LoadFunc(hModule, "XmfWebMMuxer_Free", (void**)&ctx->XmfWebMMuxer_Free);

    ctx->recorder = ctx->Recorder_New();

    if (!ctx->recorder)
        return NULL;

    return ctx;
}

bool MsRdpEx_VideoRecorder_Init(MsRdpEx_VideoRecorder* ctx)
{
    if (ctx->Recorder_Init) {
        return ctx->Recorder_Init(ctx->recorder);
    }

    return false;
}

void MsRdpEx_VideoRecorder_Uninit(MsRdpEx_VideoRecorder* ctx)
{
    if (ctx->Recorder_Uninit) {
        ctx->Recorder_Uninit(ctx->recorder);
    }
}

void MsRdpEx_VideoRecorder_SetFrameSize(MsRdpEx_VideoRecorder* ctx, uint32_t width, uint32_t height)
{
    if (ctx->Recorder_SetFrameSize) {
        ctx->Recorder_SetFrameSize(ctx->recorder, width, height);
    }
}

void MsRdpEx_VideoRecorder_SetFrameRate(MsRdpEx_VideoRecorder* ctx, uint32_t frameRate)
{
    if (ctx->Recorder_SetFrameRate) {
        ctx->Recorder_SetFrameRate(ctx->recorder, frameRate);
    }
}

void MsRdpEx_VideoRecorder_SetVideoQuality(MsRdpEx_VideoRecorder* ctx, uint32_t videoQuality)
{
    if (ctx->Recorder_SetVideoQuality) {
        ctx->Recorder_SetVideoQuality(ctx->recorder, videoQuality);
    }
}

void MsRdpEx_VideoRecorder_SetFileName(MsRdpEx_VideoRecorder* ctx, const char* filename)
{
    strcpy_s(ctx->filename, MSRDPEX_MAX_PATH, filename);

    if (ctx->Recorder_SetFileName) {
        ctx->Recorder_SetFileName(ctx->recorder, filename);
    }
}

void MsRdpEx_VideoRecorder_UpdateFrame(MsRdpEx_VideoRecorder* ctx,
    uint8_t* buffer, uint32_t updateX, uint32_t updateY,
    uint32_t updateWidth, uint32_t updateHeight, uint32_t surfaceStep)
{
    if (ctx->Recorder_UpdateFrame) {
        ctx->Recorder_UpdateFrame(ctx->recorder, buffer,
            updateX, updateY, updateWidth, updateHeight, surfaceStep);
    }
}

void MsRdpEx_VideoRecorder_Timeout(MsRdpEx_VideoRecorder* ctx)
{
    if (ctx->Recorder_Timeout) {
        ctx->Recorder_Timeout(ctx->recorder);
    }
}

uint32_t MsRdpEx_VideoRecorder_GetTimeout(MsRdpEx_VideoRecorder* ctx)
{
    if (ctx->Recorder_GetTimeout) {
        return ctx->Recorder_GetTimeout(ctx->recorder);
    }

    return 0;
}

bool MsRdpEx_VideoRecorder_Remux(MsRdpEx_VideoRecorder* ctx, const char* filename)
{
    char tempFile[MSRDPEX_MAX_PATH];

    if (!filename)
        filename = ctx->filename;

    if (!MsRdpEx_FileExists(filename))
        return false;

    if (!(ctx->XmfWebMMuxer_New && ctx->XmfWebMMuxer_Remux && ctx->XmfWebMMuxer_Free))
        return false;

    XmfWebMMuxer* muxer = ctx->XmfWebMMuxer_New();

    if (!muxer)
        return false;

    sprintf_s(tempFile, MSRDPEX_MAX_PATH, "%s.tmp", filename);
    ctx->XmfWebMMuxer_Remux(muxer, filename, tempFile);
    MsRdpEx_MoveFile(tempFile, filename, MOVEFILE_REPLACE_EXISTING);
    ctx->XmfWebMMuxer_Free(muxer);

    return true;
}

void MsRdpEx_VideoRecorder_Free(MsRdpEx_VideoRecorder* ctx)
{
    if (!ctx)
        return;

    if (ctx->recorder) {
        ctx->Recorder_Free(ctx->recorder);
        ctx->recorder = NULL;
    }
    
    if (ctx->hModule) {
        FreeLibrary(ctx->hModule);
        ctx->hModule = NULL;
    }

    ZeroMemory(ctx, sizeof(MsRdpEx_VideoRecorder));
    
    free(ctx);
}
