
#include <MsRdpEx/RdpFile.h>

MsRdpEx_RdpFileEntry* MsRdpEx_RdpFileEntry_New(char type, const char* name, const char* value)
{
	MsRdpEx_RdpFileEntry* entry;
	
	if ((type != 'i') && (type != 's')) {
		return NULL;
	}

	entry = (MsRdpEx_RdpFileEntry*) calloc(1, sizeof(MsRdpEx_RdpFileEntry));

	if (!entry)
		return NULL;

	entry->type = type;
	entry->name = _strdup(name);
	entry->value = _strdup(value);

	if (!entry->name || !entry->value)
		goto error;

	return entry;
error:
	MsRdpEx_RdpFileEntry_Free(entry);
	return NULL;
}

bool MsRdpEx_RdpFileEntry_IsMatch(MsRdpEx_RdpFileEntry* entry, char type, const char* name)
{
	if (entry->type != type)
		return false;

	if (!MsRdpEx_StringIEquals(entry->name, name)) {
		return false;
	}

	return true;
}

bool MsRdpEx_RdpFileEntry_GetBoolValue(MsRdpEx_RdpFileEntry* entry, bool* pValue)
{
	if (entry->type != 'i')
		return false;

	int iValue = atoi(entry->value);

	bool bValue = (iValue != 0) ? true : false;

	*pValue = bValue;

	return true;
}

bool MsRdpEx_RdpFileEntry_GetVBoolValue(MsRdpEx_RdpFileEntry* entry, _Out_ VARIANT* pVariant)
{
	bool bVal = false;

	if (!MsRdpEx_RdpFileEntry_GetBoolValue(entry, &bVal))
		return false;

	VariantInit(pVariant);
	pVariant->vt = VT_BOOL;
	pVariant->boolVal = bVal ? VARIANT_TRUE : VARIANT_FALSE;

	return true;
}

bool MsRdpEx_RdpFileEntry_GetIntValue(MsRdpEx_RdpFileEntry* entry, _Out_ VARIANT* pVariant)
{
	if (entry->type != 'i')
		return false;

	int iValue = atoi(entry->value);

	VariantInit(pVariant);
	pVariant->vt = VT_I4;
	pVariant->intVal = iValue;

	return true;
}

void MsRdpEx_RdpFileEntry_Free(MsRdpEx_RdpFileEntry* entry)
{
	if (!entry)
		return;

	if (entry->name) {
		free(entry->name);
		entry->name = NULL;
	}

	if (entry->value) {
		free(entry->value);
		entry->value = NULL;
	}
	
	free(entry);
}

#include <shellapi.h>

char* MsRdpEx_GetRdpFilenameFromCommandLine()
{
	int index;
	int argc = 0;
	char* filename = NULL;
	LPWSTR* argsW = NULL;
	char* cmdlineA = NULL;
	LPCWSTR cmdlineW = GetCommandLineW();

	if (!cmdlineW)
		return NULL;

	argsW = CommandLineToArgvW(cmdlineW, &argc);

	if (!argsW)
		goto exit;

	if (MsRdpEx_ConvertFromUnicode(CP_UTF8, 0, cmdlineW, -1, &cmdlineA, 0, NULL, NULL) < 0) {
		goto exit;
	}

	MsRdpEx_LogPrint(DEBUG, "cmdline(argc=%d): '%s'", argc, cmdlineA);

	if (argc < 2)
		goto exit;

	for (index = 0; index < argc; index++)
	{
		char* argA = NULL;
		MsRdpEx_ConvertFromUnicode(CP_UTF8, 0, argsW[index], -1, &argA, 0, NULL, NULL);
		MsRdpEx_LogPrint(DEBUG, "arg[%d]: '%s'", index, argA);
		free(argA);
	}

	if (MsRdpEx_ConvertFromUnicode(CP_UTF8, 0, argsW[1], -1, &filename, 0, NULL, NULL) < 0) {
		goto exit;
	}

	if (!MsRdpEx_IStringEndsWith(filename, ".rdp")) {
		free(filename);
		filename = NULL;
		goto exit;
	}

exit:
	free(cmdlineA);
	LocalFree(argsW);
	return filename;
}

char* MsRdpEx_RdpFile_LoadBuffer(MsRdpEx_RdpFile* ctx, const char* filename)
{
	uint8_t* data;
	size_t size = 0;
	size_t length = 0;
	char* strbuf = NULL;

	data = MsRdpEx_FileLoad(filename, &size, 2);

	if (!data)
		return false;

	if (size < 3)
		goto error;

	if ((data[0] == 0xFF) && (data[1] == 0xFE)) {
		WCHAR* wstr = (WCHAR*) &data[2];
		length = wcslen(wstr);
		if (MsRdpEx_ConvertFromUnicode(CP_UTF8, 0, wstr, length, &strbuf, 0, NULL, NULL) < 0) {
			goto error;
		}
	} else {
		strbuf = _strdup(data);

		if (!strbuf) {
			goto error;
		}
	}

	free(data);
	return strbuf;
error:
	free(data);
	return NULL;
}

bool MsRdpEx_RdpFile_Load(MsRdpEx_RdpFile* ctx, const char* filename)
{
	char* buffer;
	size_t index;
	size_t size;
	size_t length;
	char* line;
	char* type;
	char* d1;
	char* d2;
	char* beg;
	char* name;
	char* value;
	char* tokctx = NULL;
	MsRdpEx_RdpFileEntry* entry;

	buffer = MsRdpEx_RdpFile_LoadBuffer(ctx, filename);

	if (!buffer)
		return false;

	size = strlen(buffer);

	index = 0;
	line = strtok_s(buffer, "\r\n", &tokctx);

	while (line)
	{
		length = strnlen(line, size);

		if (length > 1)
		{
			beg = line;

			d1 = strchr(line, ':');

			if (!d1)
				goto next_line; /* no first delimiter */

			type = &d1[1];
			d2 = strchr(type, ':');

			if (!d2)
				goto next_line; /* no second delimiter */

			if ((d2 - d1) != 2)
				goto next_line; /* improper type length */

			*d1 = 0;
			*d2 = 0;
			name = beg;
			value = &d2[1];

			if (*type == 'i') /* integer type */
			{
				MsRdpEx_LogPrint(DEBUG, "RDP(i): %s = %s", name, value);
				entry = MsRdpEx_RdpFileEntry_New(*type, name, value);

				if (entry) {
					MsRdpEx_ArrayList_Add(ctx->entries, entry);
				}
			}
			else if (*type == 's') /* string type */
			{
				MsRdpEx_LogPrint(DEBUG, "RDP(s): %s = %s", name, value);
				entry = MsRdpEx_RdpFileEntry_New(*type, name, value);

				if (entry) {
					MsRdpEx_ArrayList_Add(ctx->entries, entry);
				}
			}
			else if (*type == 'b') /* binary type */
			{
				
			}
		}

		next_line:
			line = strtok_s(NULL, "\r\n", &tokctx);
			index++;
	}

	free(buffer);
	return true;
}

MsRdpEx_RdpFile* MsRdpEx_RdpFile_New()
{
	MsRdpEx_RdpFile* ctx;

	ctx = (MsRdpEx_RdpFile*) calloc(1, sizeof(MsRdpEx_RdpFile));

	if (!ctx)
		return NULL;

	ctx->entries = MsRdpEx_ArrayList_New(true);

	if (!ctx->entries)
		goto error;

	MsRdpEx_ArrayList_Object(ctx->entries)->fnObjectFree =
		(MSRDPEX_OBJECT_FREE_FN) MsRdpEx_RdpFileEntry_Free;

	return ctx;
error:
	MsRdpEx_RdpFile_Free(ctx);
	return NULL;
}

void MsRdpEx_RdpFile_Free(MsRdpEx_RdpFile* ctx)
{
	if (!ctx)
		return;

	if (ctx->entries) {
		MsRdpEx_ArrayList_Free(ctx->entries);
		ctx->entries = NULL;
	}
}
