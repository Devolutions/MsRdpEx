#ifndef MSRDPEX_RDP_FILE_H
#define MSRDPEX_RDP_FILE_H

#include <MsRdpEx/MsRdpEx.h>

#ifdef __cplusplus
extern "C" {
#endif

typedef struct _MsRdpEx_RdpFile MsRdpEx_RdpFile;

char* MsRdpEx_GetRdpFilenameFromCommandLine();

bool MsRdpEx_RdpFile_Load(MsRdpEx_RdpFile* ctx, const char* filename);

MsRdpEx_RdpFile* MsRdpEx_RdpFile_New();
void MsRdpEx_RdpFile_Free(MsRdpEx_RdpFile* ctx);

#ifdef __cplusplus
}
#endif

#endif // MSRDPEX_RDP_FILE_H
