
#include "MsRdpEx.h"

#include "VideoRecorder.h"

MsRdpEx_VideoRecorder* MsRdpEx_VideoRecorder_New()
{
    HMODULE hModule;
    MsRdpEx_VideoRecorder* ctx;
    const char* filename = "librecording.dll";

    hModule = LoadLibraryA(filename);

    if (!hModule)
        return NULL;

    ctx = (MsRdpEx_VideoRecorder*) malloc(sizeof(MsRdpEx_VideoRecorder));

    if (!ctx)
        return NULL;

    ZeroMemory(ctx, sizeof(MsRdpEx_VideoRecorder));

    ctx->hModule = hModule;
    ctx->Recording_New = (fnRecording_New) GetProcAddress(hModule, "NowRecording_New");
    ctx->Recording_SetSize = (fnRecording_SetSize) GetProcAddress(hModule, "NowRecording_SetSize");
    ctx->Recording_SetFileName = (fnRecording_SetFileName) GetProcAddress(hModule, "NowRecording_SetFileName");
    ctx->Recording_SetDirectory = (fnRecording_SetDirectory)GetProcAddress(hModule, "NowRecording_SetDirectory");
    ctx->Recording_Update = (fnRecording_Update) GetProcAddress(hModule, "NowRecording_Update");
    ctx->Recording_Timeout = (fnRecording_Timeout) GetProcAddress(hModule, "NowRecording_Timeout");
    ctx->Recording_GetTimeout = (fnRecording_GetTimeout) GetProcAddress(hModule, "NowRecording_GetTimeout");
    ctx->Recording_GetPath = (fnRecording_GetPath) GetProcAddress(hModule, "NowRecording_GetPath");
    ctx->Recording_Free = (fnRecording_Free) GetProcAddress(hModule, "NowRecording_Free");

    ctx->recording = ctx->Recording_New();

    return ctx;
}

void MsRdpEx_VideoRecorder_SetFrameSize(MsRdpEx_VideoRecorder* ctx, uint32_t width, uint32_t height)
{
    if (ctx->Recording_SetSize) {
        ctx->Recording_SetSize(ctx->recording, width, height);
    }
}

void MsRdpEx_VideoRecorder_SetFileName(MsRdpEx_VideoRecorder* ctx, const char* filename)
{
    if (ctx->Recording_SetFileName) {
        ctx->Recording_SetFileName(ctx->recording, filename);
    }
}

void MsRdpEx_VideoRecorder_SetDirectory(MsRdpEx_VideoRecorder* ctx, const char* directory)
{
    if (ctx->Recording_SetDirectory) {
        ctx->Recording_SetDirectory(ctx->recording, directory);
    }
}

void MsRdpEx_VideoRecorder_Update(MsRdpEx_VideoRecorder* ctx,
    uint8_t* buffer, uint32_t updateX, uint32_t updateY,
    uint32_t updateWidth, uint32_t updateHeight, uint32_t surfaceStep)
{
    if (ctx->Recording_Update) {
        ctx->Recording_Update(ctx->recording, buffer,
            updateX, updateY, updateWidth, updateHeight, surfaceStep);
    }
}

void MsRdpEx_VideoRecorder_Timeout(MsRdpEx_VideoRecorder* ctx)
{
    if (ctx->Recording_Timeout) {
        ctx->Recording_Timeout(ctx->recording);
    }
}

uint32_t MsRdpEx_VideoRecorder_GetTimeout(MsRdpEx_VideoRecorder* ctx)
{
    if (ctx->Recording_GetTimeout) {
        return ctx->Recording_GetTimeout(ctx->recording);
    }

    return 0;
}

size_t MsRdpEx_VideoRecorder_GetPath(MsRdpEx_VideoRecorder* ctx, char* path, size_t size)
{
    if (ctx->Recording_GetPath) {
        return ctx->Recording_GetPath(ctx->recording, path, size);
    }

    return 0;
}

void MsRdpEx_VideoRecorder_Free(MsRdpEx_VideoRecorder* ctx)
{
    if (!ctx)
        return;

    if (ctx->recording) {
        ctx->Recording_Free(ctx->recording);
        ctx->recording = NULL;
    }
    
    if (ctx->hModule) {
        FreeLibrary(ctx->hModule);
        ctx->hModule = NULL;
    }

    ZeroMemory(ctx, sizeof(MsRdpEx_VideoRecorder));
    
    free(ctx);
}
