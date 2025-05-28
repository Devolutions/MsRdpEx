
#include "MsRdpEx.h"

#include <MsRdpEx/Environment.h>
#include <MsRdpEx/NamedPipe.h>
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

bool MsRdpEx_VideoRecorder_Init(MsRdpEx_VideoRecorder* ctx)
{
    bool success = false;

    if (ctx->useBipBuffer) {
        if (ctx->np_name) {
            ctx->np_handle = MsRdpEx_NamedPipe_Open(ctx->np_name);

            if (!ctx->np_handle) {
                MsRdpEx_LogPrint(WARN, "failed to open named pipe '%s'", ctx->np_name);
            }
        }
        else {
            if (!ctx->fp) {
                ctx->fp = MsRdpEx_FileOpen(ctx->filename, "wb");
            }
        }
    }

    if (ctx->XmfRecorder_Init) {
        success = ctx->XmfRecorder_Init(ctx->recorder);
    }

    return success;
}

void MsRdpEx_VideoRecorder_Uninit(MsRdpEx_VideoRecorder* ctx)
{
    if (ctx->XmfRecorder_Uninit) {
        ctx->XmfRecorder_Uninit(ctx->recorder);
    }

    if (ctx->useBipBuffer) {
        if (ctx->np_handle) {
            MsRdpEx_NamedPipe_Close(ctx->np_handle);
            ctx->np_handle = NULL;
        }

        if (ctx->fp) {
            fclose(ctx->fp);
            ctx->fp = NULL;
        }
    }
}

void MsRdpEx_VideoRecorder_SetFrameSize(MsRdpEx_VideoRecorder* ctx, uint32_t width, uint32_t height)
{
    if (ctx->XmfRecorder_SetFrameSize) {
        ctx->XmfRecorder_SetFrameSize(ctx->recorder, width, height);
    }
}

void MsRdpEx_VideoRecorder_SetFrameRate(MsRdpEx_VideoRecorder* ctx, uint32_t frameRate)
{
    if (ctx->XmfRecorder_SetFrameRate) {
        ctx->XmfRecorder_SetFrameRate(ctx->recorder, frameRate);
    }
}

void MsRdpEx_VideoRecorder_SetVideoQuality(MsRdpEx_VideoRecorder* ctx, uint32_t videoQuality)
{
    if (ctx->XmfRecorder_SetVideoQuality) {
        ctx->XmfRecorder_SetVideoQuality(ctx->recorder, videoQuality);
    }
}

void MsRdpEx_VideoRecorder_SetFileName(MsRdpEx_VideoRecorder* ctx, const char* filename)
{
    strcpy_s(ctx->filename, MSRDPEX_MAX_PATH, filename);

    if (ctx->XmfRecorder_SetFileName) {
        ctx->XmfRecorder_SetFileName(ctx->recorder, filename);
    }
}

void MsRdpEx_VideoRecorder_SetPipeName(MsRdpEx_VideoRecorder* ctx, const char* pipeName)
{
    if (ctx->np_name) {
        free(ctx->np_name);
        ctx->np_name = NULL;
    }
    
    if (pipeName) {
        ctx->np_name = _strdup(pipeName);
    }
}

int MsRdpEx_VideoRecorder_WriteToOutputStream(MsRdpEx_VideoRecorder* ctx, const uint8_t* data, size_t size)
{
    int status = -1;

    if (ctx->np_handle) {
        status = MsRdpEx_NamedPipe_Write(ctx->np_handle, data, size);
    }
    else if (ctx->fp) {
        status = (int)fwrite(data, 1, size, ctx->fp);
    }
    
    return status;
}

int MsRdpEx_VideoRecorder_FlushBipBuffer(MsRdpEx_VideoRecorder* ctx)
{
    int writeSize;
    int status = 0;
    size_t blockSize = 0;
    uint8_t* block = NULL;

    size_t usedSize = ctx->XmfBipBuffer_UsedSize(ctx->bb);

    if (usedSize < 1)
        return 0;

    block = ctx->XmfBipBuffer_ReadTryReserve(ctx->bb, 0, &blockSize);

    if (block) {
        writeSize = MsRdpEx_VideoRecorder_WriteToOutputStream(ctx, block, blockSize);

        if (writeSize < 0) {
            MsRdpEx_LogPrint(DEBUG, "FlushBipBuffer: writeSize: %d (%d)", status, blockSize);
            return -1;
        }

        ctx->XmfBipBuffer_ReadCommit(ctx->bb, writeSize);
        status += writeSize;
    }

    block = ctx->XmfBipBuffer_ReadTryReserve(ctx->bb, 0, &blockSize);

    if (block) {
        writeSize = MsRdpEx_VideoRecorder_WriteToOutputStream(ctx, block, blockSize);

        if (writeSize < 0) {
            MsRdpEx_LogPrint(DEBUG, "FlushBipBuffer: writeSize: %d (%d)", status, blockSize);
            return -1;
        }

        ctx->XmfBipBuffer_ReadCommit(ctx->bb, writeSize);
        status += writeSize;
    }

    if (status != usedSize) {
        MsRdpEx_LogPrint(DEBUG, "FlushBipBuffer: %d (%d)", status, (int) usedSize);
    }

    return status;
}

void MsRdpEx_VideoRecorder_UpdateFrame(MsRdpEx_VideoRecorder* ctx,
    uint8_t* buffer, uint32_t updateX, uint32_t updateY,
    uint32_t updateWidth, uint32_t updateHeight, uint32_t surfaceStep)
{
    if (!ctx->XmfRecorder_UpdateFrame)
        return;

    ctx->XmfRecorder_UpdateFrame(ctx->recorder, buffer,
        updateX, updateY, updateWidth, updateHeight, surfaceStep);

    if (ctx->useBipBuffer) {
        MsRdpEx_VideoRecorder_FlushBipBuffer(ctx);
    }
}

void MsRdpEx_VideoRecorder_Timeout(MsRdpEx_VideoRecorder* ctx)
{
    if (ctx->XmfRecorder_Timeout) {
        ctx->XmfRecorder_Timeout(ctx->recorder);
    }
}

uint32_t MsRdpEx_VideoRecorder_GetTimeout(MsRdpEx_VideoRecorder* ctx)
{
    if (ctx->XmfRecorder_GetTimeout) {
        return ctx->XmfRecorder_GetTimeout(ctx->recorder);
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

bool MsRdpEx_VideoRecorder_LoadFuncs(MsRdpEx_VideoRecorder* ctx)
{
    MsRdpEx_LoadFunc(ctx->hModule, "XmfRecorder_New", (void**)&ctx->XmfRecorder_New);
    MsRdpEx_LoadFunc(ctx->hModule, "XmfRecorder_Init", (void**)&ctx->XmfRecorder_Init);
    MsRdpEx_LoadFunc(ctx->hModule, "XmfRecorder_Uninit", (void**)&ctx->XmfRecorder_Uninit);
    MsRdpEx_LoadFunc(ctx->hModule, "XmfRecorder_SetFrameSize", (void**)&ctx->XmfRecorder_SetFrameSize);
    MsRdpEx_LoadFunc(ctx->hModule, "XmfRecorder_SetFrameRate", (void**)&ctx->XmfRecorder_SetFrameRate);
    MsRdpEx_LoadFunc(ctx->hModule, "XmfRecorder_SetVideoQuality", (void**)&ctx->XmfRecorder_SetVideoQuality);
    MsRdpEx_LoadFunc(ctx->hModule, "XmfRecorder_SetFileName", (void**)&ctx->XmfRecorder_SetFileName);
    MsRdpEx_LoadFunc(ctx->hModule, "XmfRecorder_SetBipBuffer", (void**)&ctx->XmfRecorder_SetBipBuffer);
    MsRdpEx_LoadFunc(ctx->hModule, "XmfRecorder_UpdateFrame", (void**)&ctx->XmfRecorder_UpdateFrame);
    MsRdpEx_LoadFunc(ctx->hModule, "XmfRecorder_Timeout", (void**)&ctx->XmfRecorder_Timeout);
    MsRdpEx_LoadFunc(ctx->hModule, "XmfRecorder_GetTimeout", (void**)&ctx->XmfRecorder_GetTimeout);
    MsRdpEx_LoadFunc(ctx->hModule, "XmfRecorder_Free", (void**)&ctx->XmfRecorder_Free);

    MsRdpEx_LoadFunc(ctx->hModule, "XmfWebMMuxer_New", (void**)&ctx->XmfWebMMuxer_New);
    MsRdpEx_LoadFunc(ctx->hModule, "XmfWebMMuxer_Remux", (void**)&ctx->XmfWebMMuxer_Remux);
    MsRdpEx_LoadFunc(ctx->hModule, "XmfWebMMuxer_Free", (void**)&ctx->XmfWebMMuxer_Free);

    MsRdpEx_LoadFunc(ctx->hModule, "XmfBipBuffer_Grow", (void**)&ctx->XmfBipBuffer_Grow);
    MsRdpEx_LoadFunc(ctx->hModule, "XmfBipBuffer_Clear", (void**)&ctx->XmfBipBuffer_Clear);
    MsRdpEx_LoadFunc(ctx->hModule, "XmfBipBuffer_UsedSize", (void**)&ctx->XmfBipBuffer_UsedSize);
    MsRdpEx_LoadFunc(ctx->hModule, "XmfBipBuffer_BufferSize", (void**)&ctx->XmfBipBuffer_BufferSize);
    MsRdpEx_LoadFunc(ctx->hModule, "XmfBipBuffer_WriteReserve", (void**)&ctx->XmfBipBuffer_WriteReserve);
    MsRdpEx_LoadFunc(ctx->hModule, "XmfBipBuffer_WriteTryReserve", (void**)&ctx->XmfBipBuffer_WriteTryReserve);
    MsRdpEx_LoadFunc(ctx->hModule, "XmfBipBuffer_WriteCommit", (void**)&ctx->XmfBipBuffer_WriteCommit);
    MsRdpEx_LoadFunc(ctx->hModule, "XmfBipBuffer_ReadReserve", (void**)&ctx->XmfBipBuffer_ReadReserve);
    MsRdpEx_LoadFunc(ctx->hModule, "XmfBipBuffer_ReadTryReserve", (void**)&ctx->XmfBipBuffer_ReadTryReserve);
    MsRdpEx_LoadFunc(ctx->hModule, "XmfBipBuffer_ReadCommit", (void**)&ctx->XmfBipBuffer_ReadCommit);
    MsRdpEx_LoadFunc(ctx->hModule, "XmfBipBuffer_Read", (void**)&ctx->XmfBipBuffer_Read);
    MsRdpEx_LoadFunc(ctx->hModule, "XmfBipBuffer_Write", (void**)&ctx->XmfBipBuffer_Write);
    MsRdpEx_LoadFunc(ctx->hModule, "XmfBipBuffer_SetSignaledState", (void**)&ctx->XmfBipBuffer_SetSignaledState);
    MsRdpEx_LoadFunc(ctx->hModule, "XmfBipBuffer_GetSignaledState", (void**)&ctx->XmfBipBuffer_GetSignaledState);
    MsRdpEx_LoadFunc(ctx->hModule, "XmfBipBuffer_New", (void**)&ctx->XmfBipBuffer_New);
    MsRdpEx_LoadFunc(ctx->hModule, "XmfBipBuffer_Free", (void**)&ctx->XmfBipBuffer_Free);

    return true;
}

MsRdpEx_VideoRecorder* MsRdpEx_VideoRecorder_New()
{
    HMODULE hModule;
    MsRdpEx_VideoRecorder* ctx = NULL;
    const char* libPath = MsRdpEx_GetPath(MSRDPEX_XMF_DLL_PATH);

    if (MsRdpEx_StringIsNullOrEmpty(libPath)) {
        MsRdpEx_LogPrint(DEBUG, "no xmf.dll (cadeau) library detected");
        return NULL;
    }

    hModule = MsRdpEx_LoadLibrary(libPath);

    if (!hModule) {
        MsRdpEx_LogPrint(DEBUG, "Could not load cadeau library: '%s'", libPath);
        goto error;
    }

    ctx = (MsRdpEx_VideoRecorder*) malloc(sizeof(MsRdpEx_VideoRecorder));

    if (!ctx)
        return NULL;

    ZeroMemory(ctx, sizeof(MsRdpEx_VideoRecorder));

    ctx->useBipBuffer = true;
    ctx->bipBufferSize = 0x1000000; // 1024 * 1024 * 16;

    ctx->hModule = hModule;

    if (!MsRdpEx_VideoRecorder_LoadFuncs(ctx))
        goto error;

    ctx->recorder = ctx->XmfRecorder_New();

    if (!ctx->recorder)
        goto error;

    if (ctx->useBipBuffer)
    {
        ctx->bb = ctx->XmfBipBuffer_New(ctx->bipBufferSize);

        if (!ctx->bb)
            goto error;

        ctx->XmfRecorder_SetBipBuffer(ctx->recorder, ctx->bb);
    }

    return ctx;
error:
    MsRdpEx_VideoRecorder_Free(ctx);
    return NULL;
}

void MsRdpEx_VideoRecorder_Free(MsRdpEx_VideoRecorder* ctx)
{
    if (!ctx)
        return;

    if (ctx->recorder) {
        ctx->XmfRecorder_Free(ctx->recorder);
        ctx->recorder = NULL;
    }
    
    if (ctx->bb) {
        ctx->XmfBipBuffer_Free(ctx->bb);
        ctx->bb = NULL;
    }

    if (ctx->hModule) {
        FreeLibrary(ctx->hModule);
        ctx->hModule = NULL;
    }

    if (ctx->fp) {
        fclose(ctx->fp);
        ctx->fp = NULL;
    }

    if (ctx->np_name) {
        free(ctx->np_name);
        ctx->np_name = NULL;
    }

    ZeroMemory(ctx, sizeof(MsRdpEx_VideoRecorder));
    
    free(ctx);
}
