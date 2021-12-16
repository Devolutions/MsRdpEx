
#include <MsRdpEx/ArrayList.h>
#include <MsRdpEx/HashTable.h>

#include <MsRdpEx/RdpFile.h>

struct _MsRdpEx_RdpFile
{
	void* dummy;
};

#include <shellapi.h>

char* MsRdpEx_GetRdpFilenameFromCommandLine()
{
	int index;
	int argc = 0;
	char* filename = NULL;
	LPWSTR* argsW = NULL;
	LPCWSTR cmdlineW = GetCommandLineW();

	if (!cmdlineW)
		return NULL;

	argsW = CommandLineToArgvW(cmdlineW, &argc);

	if (!argsW)
		goto exit;

	if (argc < 2)
		goto exit;

	if (MsRdpEx_ConvertFromUnicode(CP_UTF8, 0, argsW[1], -1, &filename, 0, NULL, NULL) < 0) {
		goto exit;
	}

	if (!MsRdpEx_IStringEndsWith(filename, ".rdp")) {
		free(filename);
		filename = NULL;
		goto exit;
	}

exit:
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
				MsRdpEx_Log("RDP(i): %s = %s", name, value);
			}
			else if (*type == 's') /* string type */
			{
				MsRdpEx_Log("RDP(s): %s = %s", name, value);
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

	return ctx;
}

void MsRdpEx_RdpFile_Free(MsRdpEx_RdpFile* ctx)
{
	if (!ctx)
		return;
}
