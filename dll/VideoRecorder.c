
#include "MsRdpEx.h"

#include <MsRdpEx/VideoRecorder.h>

void* MsRdpEx_LoadFunc(HMODULE hModule, const char* name, void** ppFunc)
{
    void* pFunc = GetProcAddress(hModule, name);
    *ppFunc = pFunc;
    if (!pFunc) {
        MsRdpEx_Log("LoadFunc(%s): not found", name);
    }
    return pFunc;
}

MsRdpEx_VideoRecorder* MsRdpEx_VideoRecorder_New()
{
    HMODULE hModule;
    MsRdpEx_VideoRecorder* ctx;
    const char* filename = "libxmf.dll";

    hModule = LoadLibraryA(filename);

    if (!hModule)
        return NULL;

    ctx = (MsRdpEx_VideoRecorder*) malloc(sizeof(MsRdpEx_VideoRecorder));

    if (!ctx)
        return NULL;

    ZeroMemory(ctx, sizeof(MsRdpEx_VideoRecorder));

    ctx->hModule = hModule;
    MsRdpEx_LoadFunc(hModule, "XmfRecorder_New", (void**)&ctx->Recorder_New);
    MsRdpEx_LoadFunc(hModule, "XmfRecorder_SetFrameSize", (void**)&ctx->Recorder_SetFrameSize);
    MsRdpEx_LoadFunc(hModule, "XmfRecorder_SetFilename", (void**)&ctx->Recorder_SetFilename);
    MsRdpEx_LoadFunc(hModule, "XmfRecorder_UpdateFrame", (void**)&ctx->Recorder_UpdateFrame);
    MsRdpEx_LoadFunc(hModule, "XmfRecorder_Timeout", (void**)&ctx->Recorder_Timeout);
    MsRdpEx_LoadFunc(hModule, "XmfRecorder_GetTimeout", (void**)&ctx->Recorder_GetTimeout);
    MsRdpEx_LoadFunc(hModule, "XmfRecorder_Free", (void**)&ctx->Recorder_Free);

    ctx->recorder = ctx->Recorder_New();

    if (!ctx->recorder)
        return NULL;

    return ctx;
}

void MsRdpEx_VideoRecorder_SetFrameSize(MsRdpEx_VideoRecorder* ctx, uint32_t width, uint32_t height)
{
    if (ctx->Recorder_SetFrameSize) {
        ctx->Recorder_SetFrameSize(ctx->recorder, width, height);
    }
}

void MsRdpEx_VideoRecorder_SetFilename(MsRdpEx_VideoRecorder* ctx, const char* filename)
{
    if (ctx->Recorder_SetFilename) {
        ctx->Recorder_SetFilename(ctx->recorder, filename);
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
