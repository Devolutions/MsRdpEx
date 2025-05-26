#ifndef MSRDPEX_RECORDING_MANIFEST_H
#define MSRDPEX_RECORDING_MANIFEST_H

#include "MsRdpEx.h"

#ifdef __cplusplus
extern "C" {
#endif

typedef struct _MsRdpEx_JrecFile MsRdpEx_JrecFile;

typedef struct _MsRdpEx_RecordingManifest MsRdpEx_RecordingManifest;

void MsRdpEx_RecordingManifest_SetSessionId(MsRdpEx_RecordingManifest* ctx, GUID* sessionId);
void MsRdpEx_RecordingManifest_SetStartTime(MsRdpEx_RecordingManifest* ctx, int64_t startTime);
void MsRdpEx_RecordingManifest_SetDuration(MsRdpEx_RecordingManifest* ctx, int64_t duration);

bool MsRdpEx_RecordingManifest_AddFile(MsRdpEx_RecordingManifest* ctx, const char* fileName, int64_t startTime, int64_t duration);
void MsRdpEx_RecordingManifest_FinalizeFile(MsRdpEx_RecordingManifest* ctx, int64_t endTime);

char* MsRdpEx_RecordingManifest_WriteJsonData(MsRdpEx_RecordingManifest* ctx);
bool MsRdpEx_RecordingManifest_WriteJsonFile(MsRdpEx_RecordingManifest* ctx, const char* filename);

MsRdpEx_RecordingManifest* MsRdpEx_RecordingManifest_New();
void MsRdpEx_RecordingManifest_Free(MsRdpEx_RecordingManifest* ctx);

#ifdef __cplusplus
}
#endif

#endif /* MSRDPEX_RECORDING_MANIFEST_H */
