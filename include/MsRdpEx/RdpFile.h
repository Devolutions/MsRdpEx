#ifndef MSRDPEX_RDP_FILE_H
#define MSRDPEX_RDP_FILE_H

#include <MsRdpEx/MsRdpEx.h>

#include <MsRdpEx/ArrayList.h>

#include <oleauto.h>

#ifdef __cplusplus
extern "C" {
#endif

struct _MsRdpEx_RdpFileEntry
{
	char type;
	char* name;
	char* value;
};
typedef struct _MsRdpEx_RdpFileEntry MsRdpEx_RdpFileEntry;

bool MsRdpEx_RdpFileEntry_IsMatch(MsRdpEx_RdpFileEntry* entry, char type, const char* name);

bool MsRdpEx_RdpFileEntry_GetBoolValue(MsRdpEx_RdpFileEntry* entry, bool* pValue);
bool MsRdpEx_RdpFileEntry_GetVBoolValue(MsRdpEx_RdpFileEntry* entry, VARIANT* pVariant);

MsRdpEx_RdpFileEntry* MsRdpEx_RdpFileEntry_New(char type, const char* name, const char* value);
void MsRdpEx_RdpFileEntry_Free(MsRdpEx_RdpFileEntry* entry);

struct _MsRdpEx_RdpFile
{
	MsRdpEx_ArrayList* entries;
};
typedef struct _MsRdpEx_RdpFile MsRdpEx_RdpFile;

char* MsRdpEx_GetRdpFilenameFromCommandLine();

bool MsRdpEx_RdpFile_Load(MsRdpEx_RdpFile* ctx, const char* filename);

MsRdpEx_RdpFile* MsRdpEx_RdpFile_New();
void MsRdpEx_RdpFile_Free(MsRdpEx_RdpFile* ctx);

#ifdef __cplusplus
}
#endif

#endif // MSRDPEX_RDP_FILE_H
