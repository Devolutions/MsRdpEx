
#include <MsRdpEx/ArrayList.h>

#include <MsRdpEx/RecordingManifest.h>

struct _MsRdpEx_JrecFile
{
    char* fileName;
    int64_t startTime;
    int64_t duration;
};

struct _MsRdpEx_RecordingManifest
{
    GUID sessionId;
    int64_t startTime;
    int64_t duration;
    MsRdpEx_ArrayList* files;
};

void MsRdpEx_JrecFile_SetFileName(MsRdpEx_JrecFile* file, const char* fileName)
{
    if (file->fileName) {
        free(file->fileName);
        file->fileName = NULL;
    }

    if (fileName) {
        file->fileName = _strdup(fileName);
    }
}

MsRdpEx_JrecFile* MsRdpEx_JrecFile_New()
{
    MsRdpEx_JrecFile* file;

    file = (MsRdpEx_JrecFile*) calloc(1, sizeof(MsRdpEx_JrecFile));

    if (!file)
        return NULL;

    return file;
}

void MsRdpEx_JrecFile_Free(MsRdpEx_JrecFile* file)
{
    if (!file)
        return;

    MsRdpEx_JrecFile_SetFileName(file, NULL);
    free(file);
}

void MsRdpEx_RecordingManifest_SetSessionId(MsRdpEx_RecordingManifest* ctx, GUID* sessionId)
{
    MsRdpEx_GuidCopy(&ctx->sessionId, sessionId);
}

void MsRdpEx_RecordingManifest_SetStartTime(MsRdpEx_RecordingManifest* ctx, int64_t startTime)
{
    ctx->startTime = startTime;
}

void MsRdpEx_RecordingManifest_SetDuration(MsRdpEx_RecordingManifest* ctx, int64_t duration)
{
    ctx->duration = duration;
}

bool MsRdpEx_RecordingManifest_AddFile(MsRdpEx_RecordingManifest* ctx, const char* fileName, int64_t startTime, int64_t duration)
{
    MsRdpEx_JrecFile* file;

    file = MsRdpEx_JrecFile_New();

    if (!file)
        return false;

    MsRdpEx_JrecFile_SetFileName(file, fileName);
    file->startTime = startTime;
    file->duration = duration;

    MsRdpEx_ArrayList_Add(ctx->files, (void*)file);

    return true;
}

void MsRdpEx_RecordingManifest_FinalizeFile(MsRdpEx_RecordingManifest* ctx, int64_t endTime)
{
    MsRdpEx_JrecFile* file;

    if (!endTime)
        endTime = MsRdpEx_GetUnixTime();

    file = (MsRdpEx_JrecFile*) MsRdpEx_ArrayList_GetTail(ctx->files);

    if (file) {
        if (!file->duration) {
            file->duration = endTime - file->startTime;
        }
    }
}

bool MsRdpEx_RecordingManifest_UpdateTotalDuration(MsRdpEx_RecordingManifest* ctx)
{
    int64_t totalDuration = 0;

    MsRdpEx_ArrayListIt* it = MsRdpEx_ArrayList_It(ctx->files, MSRDPEX_ITERATOR_FLAG_EXCLUSIVE);

    while (!MsRdpEx_ArrayListIt_Done(it))
    {
        MsRdpEx_JrecFile* file = (MsRdpEx_JrecFile*) MsRdpEx_ArrayListIt_Next(it);
        totalDuration += file->duration;
    }

    MsRdpEx_ArrayListIt_Finish(it);

    ctx->duration = totalDuration;

    return true;
}

char* MsRdpEx_RecordingManifest_WriteJsonData(MsRdpEx_RecordingManifest* ctx)
{
    char fileData[512];
    char* jsonData = NULL;
    char sessionId[MSRDPEX_GUID_STRING_SIZE];
    MsRdpEx_GuidBinToStr((GUID*)&ctx->sessionId, sessionId, 0);

    MsRdpEx_RecordingManifest_UpdateTotalDuration(ctx);

    char header[512];
    sprintf_s(header, sizeof(header),
        "{\n"
        "  \"sessionId\": \"%s\",\n"
        "  \"startTime\": %lld,\n"
        "  \"duration\": %lld,\n"
        "  \"files\": [\n",
        sessionId, ctx->startTime, ctx->duration);

    const char* footer = "  ]\n}";

    MsRdpEx_ArrayListIt* it = MsRdpEx_ArrayList_It(ctx->files, MSRDPEX_ITERATOR_FLAG_EXCLUSIVE);

    int numFiles = MsRdpEx_ArrayList_Count(ctx->files);
    size_t bufferSize = strlen(header) + strlen(footer) + (numFiles * sizeof(fileData)) + 1;

    jsonData = (char*) calloc(bufferSize, 1);

    if (!jsonData) {
        MsRdpEx_ArrayListIt_Finish(it);
        return NULL;
    }

    strcat_s(jsonData, bufferSize, header);

    int fileIndex = 0;
    while (!MsRdpEx_ArrayListIt_Done(it))
    {
        MsRdpEx_JrecFile* file = (MsRdpEx_JrecFile*) MsRdpEx_ArrayListIt_Next(it);

        sprintf_s(fileData, sizeof(fileData) - 1,
            "    {\n"
            "      \"fileName\": \"%s\",\n"
            "      \"startTime\": %lld,\n"
            "      \"duration\": %lld\n"
            "    }%s\n",
            file->fileName, file->startTime, file->duration,
            ((fileIndex + 1) < numFiles) ? "," : "");

        strcat_s(jsonData, bufferSize, fileData);
        fileIndex++;
    }

    strcat_s(jsonData, bufferSize, footer);

    MsRdpEx_ArrayListIt_Finish(it);

    return jsonData;
}

bool MsRdpEx_RecordingManifest_WriteJsonFile(MsRdpEx_RecordingManifest* ctx, const char* filename)
{
    bool success = false;
    char* jsonData = MsRdpEx_RecordingManifest_WriteJsonData(ctx);

    if (jsonData) {
        success = MsRdpEx_FileSave(filename, (uint8_t*)jsonData, strlen(jsonData), 0);
        free(jsonData);
    }

    return success;
}

MsRdpEx_RecordingManifest* MsRdpEx_RecordingManifest_New()
{
    MsRdpEx_RecordingManifest* ctx;

    ctx = (MsRdpEx_RecordingManifest*) calloc(1, sizeof(MsRdpEx_RecordingManifest));

    if (!ctx)
        return NULL;

    ctx->files = MsRdpEx_ArrayList_New(true);

    if (!ctx->files)
        goto error;

    MsRdpEx_ArrayList_Object(ctx->files)->fnObjectFree =
        (MSRDPEX_OBJECT_FREE_FN) MsRdpEx_JrecFile_Free;

    return ctx;
error:
    MsRdpEx_RecordingManifest_Free(ctx);
    return NULL;
}

void MsRdpEx_RecordingManifest_Free(MsRdpEx_RecordingManifest* ctx)
{
    if (!ctx)
        return;

    if (ctx->files) {
        MsRdpEx_ArrayList_Free(ctx->files);
        ctx->files = NULL;
    }

    free(ctx);
}
