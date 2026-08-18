#include "MsRdpEx.h"
#include "OutputMirrorCapture.h"

static SRWLOCK g_OutputMirrorCreationLock = SRWLOCK_INIT;

MsRdpEx_OutputMirror* MsRdpEx_OutputMirror_GetOrCreate(
    IMsRdpExInstance* instance,
    CMsRdpExtendedSettings* extendedSettings)
{
    MsRdpEx_OutputMirror* outputMirror = NULL;

    if (FAILED(instance->GetOutputMirrorObject((LPVOID*)&outputMirror)) || outputMirror)
        return outputMirror;

    AcquireSRWLockExclusive(&g_OutputMirrorCreationLock);

    if (FAILED(instance->GetOutputMirrorObject((LPVOID*)&outputMirror)) || outputMirror)
    {
        ReleaseSRWLockExclusive(&g_OutputMirrorCreationLock);
        return outputMirror;
    }

    outputMirror = MsRdpEx_OutputMirror_New();
    if (!outputMirror) {
        ReleaseSRWLockExclusive(&g_OutputMirrorCreationLock);
        return NULL;
    }

    MsRdpEx_OutputMirror_SetDumpBitmapUpdates(
        outputMirror, extendedSettings->GetDumpBitmapUpdates());
    MsRdpEx_OutputMirror_SetVideoRecordingEnabled(
        outputMirror, extendedSettings->GetVideoRecordingEnabled());
    MsRdpEx_OutputMirror_SetVideoQualityLevel(
        outputMirror, extendedSettings->GetVideoRecordingQuality());
    MsRdpEx_OutputMirror_SetVideoFrameRate(
        outputMirror, extendedSettings->GetVideoRecordingFrameRate());

    char* recordingPath = extendedSettings->GetRecordingPath();
    if (recordingPath)
    {
        MsRdpEx_OutputMirror_SetRecordingPath(outputMirror, recordingPath);
        free(recordingPath);
    }

    char* recordingPipeName = extendedSettings->GetRecordingPipeName();
    if (recordingPipeName)
    {
        MsRdpEx_OutputMirror_SetRecordingPipeName(outputMirror, recordingPipeName);
        free(recordingPipeName);
    }

    const char* sessionId = extendedSettings->GetRecordingSessionId();
    if (!sessionId)
        sessionId = extendedSettings->GetSessionId();
    MsRdpEx_OutputMirror_SetSessionId(outputMirror, sessionId);

    instance->SetOutputMirrorObject((LPVOID)outputMirror);
    ReleaseSRWLockExclusive(&g_OutputMirrorCreationLock);
    return outputMirror;
}

bool MsRdpEx_OutputMirror_CapturePixels(
    IMsRdpExInstance* instance,
    const uint8_t* pixels,
    uint32_t width,
    uint32_t height,
    uint32_t sourceStride)
{
    CMsRdpExtendedSettings* extendedSettings = NULL;
    MsRdpEx_OutputMirror* outputMirror = NULL;
    uint32_t frameWidth = 0;
    uint32_t frameHeight = 0;
    uint32_t frameStride = 0;
    uint8_t* framePixels = NULL;
    HDC frameDC = NULL;
    HBITMAP frameBitmap = NULL;

    if (!instance || !pixels || !width || !height || sourceStride < (width * 4))
        return false;

    if (!instance->GetExtendedSettings(&extendedSettings) ||
        !extendedSettings->GetOutputMirrorEnabled())
    {
        return false;
    }

    outputMirror = MsRdpEx_OutputMirror_GetOrCreate(instance, extendedSettings);
    if (!outputMirror)
        return false;

    MsRdpEx_OutputMirror_Lock(outputMirror);
    MsRdpEx_OutputMirror_GetFrameSize(outputMirror, &frameWidth, &frameHeight);

    if (frameWidth != width || frameHeight != height)
    {
        MsRdpEx_OutputMirror_Uninit(outputMirror);
        MsRdpEx_OutputMirror_SetSourceDC(outputMirror, NULL);
        MsRdpEx_OutputMirror_SetFrameSize(outputMirror, width, height);
        if (!MsRdpEx_OutputMirror_Init(outputMirror))
        {
            MsRdpEx_OutputMirror_SetFrameSize(outputMirror, 0, 0);
            MsRdpEx_OutputMirror_Unlock(outputMirror);
            return false;
        }
    }

    MsRdpEx_OutputMirror_GetShadowBitmap(
        outputMirror, &frameDC, &frameBitmap, &framePixels,
        &frameWidth, &frameHeight, &frameStride);

    if (!frameDC || !frameBitmap || !framePixels || frameStride < (width * 4))
    {
        MsRdpEx_OutputMirror_Unlock(outputMirror);
        return false;
    }

    for (uint32_t y = 0; y < height; y++)
    {
        memcpy(framePixels + (size_t)y * frameStride,
            pixels + (size_t)y * sourceStride, (size_t)width * 4);
    }

    instance->DumpFrameWithCursor();
    MsRdpEx_OutputMirror_Unlock(outputMirror);
    return true;
}
