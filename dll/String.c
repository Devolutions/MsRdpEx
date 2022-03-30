
#include <MsRdpEx/MsRdpEx.h>

#include <shellapi.h>

int MsRdpEx_ConvertFromUnicode(UINT CodePage, DWORD dwFlags, LPCWSTR lpWideCharStr, int cchWideChar,
                       LPSTR* lpMultiByteStr, int cbMultiByte, LPCSTR lpDefaultChar,
                       LPBOOL lpUsedDefaultChar)
{
    int status;
    BOOL allocate = FALSE;

    if (!lpWideCharStr)
        return 0;

    if (!lpMultiByteStr)
        return 0;

    if (cchWideChar == -1)
        cchWideChar = (int)(wcslen(lpWideCharStr) + 1);

    if (cbMultiByte == 0)
    {
        cbMultiByte =
            WideCharToMultiByte(CodePage, dwFlags, lpWideCharStr, cchWideChar, NULL, 0, NULL, NULL);
        allocate = TRUE;
    }
    else if (!(*lpMultiByteStr))
        allocate = TRUE;

    if (cbMultiByte < 1)
        return 0;

    if (allocate)
    {
        *lpMultiByteStr = (LPSTR)calloc(1, cbMultiByte + 1);

        if (!(*lpMultiByteStr))
        {
            return 0;
        }
    }

    status = WideCharToMultiByte(CodePage, dwFlags, lpWideCharStr, cchWideChar, *lpMultiByteStr,
                                 cbMultiByte, lpDefaultChar, lpUsedDefaultChar);

    if ((status != cbMultiByte) && allocate)
    {
        status = 0;
    }

    if ((status <= 0) && allocate)
    {
        free(*lpMultiByteStr);
        *lpMultiByteStr = NULL;
    }

    return status;
}

int MsRdpEx_ConvertToUnicode(UINT CodePage, DWORD dwFlags, LPCSTR lpMultiByteStr, int cbMultiByte,
                     LPWSTR* lpWideCharStr, int cchWideChar)
{
    int status;
    BOOL allocate = FALSE;

    if (!lpMultiByteStr)
        return 0;

    if (!lpWideCharStr)
        return 0;

    if (cbMultiByte == -1)
    {
        size_t len = strnlen(lpMultiByteStr, INT_MAX);
        if (len >= INT_MAX)
            return 0;
        cbMultiByte = (int)(len + 1);
    }

    if (cchWideChar == 0)
    {
        cchWideChar = MultiByteToWideChar(CodePage, dwFlags, lpMultiByteStr, cbMultiByte, NULL, 0);
        allocate = TRUE;
    }
    else if (!(*lpWideCharStr))
        allocate = TRUE;

    if (cchWideChar < 1)
        return 0;

    if (allocate)
    {
        *lpWideCharStr = (LPWSTR)calloc(cchWideChar + 1, sizeof(WCHAR));

        if (!(*lpWideCharStr))
        {
            return 0;
        }
    }

    status = MultiByteToWideChar(CodePage, dwFlags, lpMultiByteStr, cbMultiByte, *lpWideCharStr, cchWideChar);

    if (status != cchWideChar)
    {
        if (allocate)
        {
            free(*lpWideCharStr);
            *lpWideCharStr = NULL;
            status = 0;
        }
    }

    return status;
}

bool MsRdpEx_StringEquals(const char* str1, const char* str2)
{
	return strcmp(str1, str2) == 0;
}

bool MsRdpEx_StringIEquals(const char* str1, const char* str2)
{
	return _stricmp(str1, str2) == 0;
}

bool MsRdpEx_StringEndsWith(const char* str, const char* val)
{
    size_t strLen;
    size_t valLen;
    const char* p;

    if (!str || !val)
        return false;

    strLen = strlen(str);
    valLen = strlen(val);

    if ((strLen < 1) || (valLen < 1))
        return false;

    if (valLen > strLen)
        return false;

    p = &str[strLen - valLen];

    if (!strcmp(p, val))
        return true;

    return false;
}

bool MsRdpEx_IStringEndsWith(const char* str, const char* val)
{
    int strLen;
    int valLen;
    const char* p;

    if (!str || !val)
        return false;

    strLen = strlen(str);
    valLen = strlen(val);

    if ((strLen < 1) || (valLen < 1))
        return false;

    if (valLen > strLen)
        return false;

    p = &str[strLen - valLen];

    if (!_stricmp(p, val))
        return true;

    return false;
}

static GUID GUID_NIL =
{
	0x00000000, 0x0000, 0x0000,
	{ 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
};

#include <rpc.h>

bool MsRdpEx_GuidGenerate(GUID* guid)
{
	if (!guid)
		return false;

    UuidCreateSequential((UUID*)guid);

	return true;
}

void MsRdpEx_GuidCopy(GUID* dst, const GUID* src)
{
	CopyMemory(dst, src, sizeof(GUID));
}

int MsRdpEx_GuidCompare(const GUID* guid1, const GUID* guid2)
{
	int index;

	if (!guid1)
		guid1 = &GUID_NIL;

	if (!guid2)
		guid2 = &GUID_NIL;

	if (guid1->Data1 != guid2->Data1)
		return (guid1->Data1 < guid2->Data1) ? -1 : 1;

	if (guid1->Data2 != guid2->Data2)
		return (guid1->Data2 < guid2->Data2) ? -1 : 1;

	if (guid1->Data3 != guid2->Data3)
		return (guid1->Data3 < guid2->Data3) ? -1 : 1;

	for (index = 0; index < 8; index++)
	{
		if (guid1->Data4[index] != guid2->Data4[index])
			return (guid1->Data4[index] < guid2->Data4[index]) ? -1 : 1;
	}

	return 0;
}

bool MsRdpEx_GuidIsEqual(const GUID* guid1, const GUID* guid2)
{
	return ((MsRdpEx_GuidCompare(guid1, guid2) == 0) ? true : false);
}

bool MsRdpEx_GuidIsNil(const GUID* guid)
{
	return MsRdpEx_GuidIsEqual(guid, &GUID_NIL);
}

bool MsRdpEx_GuidSetNil(GUID* guid)
{
	if (!guid)
		return false;

	CopyMemory((void*) guid, (void*) &GUID_NIL, 16);

	return true;
}

bool MsRdpEx_GuidBinToStr(const GUID* guid, char* str, uint32_t flags)
{
    const char* fmt_lc = "%08x-%04x-%04x-%02x%02x-%02x%02x%02x%02x%02x%02x";
    const char* fmt_uc = "%08X-%04X-%04X-%02X%02X-%02X%02X%02X%02X%02X%02X";
    const char* fmt = (flags & MSRDPEX_STRING_FLAG_UPPERCASE) ? fmt_uc : fmt_lc;

    if (!str || !guid)
        return false;

    /**
     * Format is 32 hex digits partitioned in 5 groups:
     * xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
     */
    sprintf_s(str, 37, fmt,
        guid->Data1, guid->Data2, guid->Data3,
        guid->Data4[0], guid->Data4[1],
        guid->Data4[2], guid->Data4[3], guid->Data4[4],
        guid->Data4[5], guid->Data4[6], guid->Data4[7]);

    if (!(flags & MSRDPEX_STRING_FLAG_NO_TERMINATOR))
        str[36] = '\0';

    return true;
}

bool MsRdpEx_GuidStrToBin(const char* str, GUID* guid, uint32_t flags)
{
	int index;
	uint8_t bin[36];

	if (!str || !guid)
		return false;

	if (strlen(str) != 36)
		return false;

	if ((str[8] != '-') || (str[13] != '-') ||
		(str[18] != '-') || (str[23] != '-'))
	{
		return false;
	}

	for (index = 0; index < 36; index++)
	{
		if ((index == 8) || (index == 13) || (index == 18) || (index == 23))
			continue;

		if ((str[index] >= '0') && (str[index] <= '9'))
			bin[index] = str[index] - '0';
		else if ((str[index] >= 'a') && (str[index] <= 'f'))
			bin[index] = str[index] - 'a' + 10;
		else if ((str[index] >= 'A') && (str[index] <= 'F'))
			bin[index] = str[index] - 'A' + 10;
		else
			return false;
	}

	guid->Data1 = ((bin[0] << 28) | (bin[1] << 24) | (bin[2] << 20) |
		       (bin[3] << 16) | (bin[4] << 12) | (bin[5] << 8) | (bin[6] << 4) | bin[7]);
	guid->Data2 = ((bin[9] << 12) | (bin[10] << 8) | (bin[11] << 4) | bin[12]);
	guid->Data3 = ((bin[14] << 12) | (bin[15] << 8) | (bin[16] << 4) | bin[17]);
	guid->Data4[0] = ((bin[19] << 4) | bin[20]);
	guid->Data4[1] = ((bin[21] << 4) | bin[22]);
	guid->Data4[2] = ((bin[24] << 4) | bin[25]);
	guid->Data4[3] = ((bin[26] << 4) | bin[27]);
	guid->Data4[4] = ((bin[28] << 4) | bin[29]);
	guid->Data4[5] = ((bin[30] << 4) | bin[31]);
	guid->Data4[6] = ((bin[32] << 4) | bin[33]);
	guid->Data4[7] = ((bin[34] << 4) | bin[35]);

	return true;
}

uint8_t* MsRdpEx_HexToBin(const char* hex, uint8_t* bin, int size, uint32_t flags)
{
    char c;
    int i, ln, hn;

    if (!bin)
        bin = (uint8_t*)malloc(size);

    if (!bin)
        return NULL;

    for (i = 0; i < size; i++)
    {
        hn = ln = 0;

        c = hex[(i * 2) + 0];

        if ((c >= '0') && (c <= '9'))
            hn = c - '0';
        else if ((c >= 'a') && (c <= 'f'))
            hn = (c - 'a') + 10;
        else if ((c >= 'A') && (c <= 'F'))
            hn = (c - 'A') + 10;

        c = hex[(i * 2) + 1];

        if ((c >= '0') && (c <= '9'))
            ln = c - '0';
        else if ((c >= 'a') && (c <= 'f'))
            ln = (c - 'a') + 10;
        else if ((c >= 'A') && (c <= 'F'))
            ln = (c - 'A') + 10;

        bin[i] = (hn << 4) | ln;
    }

    return bin;
}

char* MsRdpEx_BinToHex(const uint8_t* bin, char* hex, int size, uint32_t flags)
{
    int i, ln, hn;
    char* bin2hex;
    char bin2hex_lc[] = "0123456789abcdef";
    char bin2hex_uc[] = "0123456789ABCDEF";

    bin2hex = (flags & MSRDPEX_STRING_FLAG_UPPERCASE) ? bin2hex_uc : bin2hex_lc;

    if (!hex)
        hex = (char*)malloc((size + 1) * 2);

    if (!hex)
        return NULL;

    for (i = 0; i < size; i++)
    {
        ln = bin[i] & 0xF;
        hn = (bin[i] >> 4) & 0xF;

        hex[i * 2] = bin2hex[hn];
        hex[(i * 2) + 1] = bin2hex[ln];
    }

    if (!(flags & MSRDPEX_STRING_FLAG_NO_TERMINATOR))
        hex[size * 2] = '\0';

    return hex;
}

char* MsRdpEx_StringJoin(char* value[], size_t count, const char sep)
{
    char* str = NULL;
    char* result = NULL;
    size_t len = 0;

    if (!value || !count)
        return NULL;

    for (int i = 0; i < count; i++)
    {
        str = value[i];

        if (!str)
            continue;

        len += strlen(str) + 1;
    }

    result = (char*)calloc(len, sizeof(char));

    if (!result)
        return NULL;

    strcpy(result, value[0]);

    len = strlen(result);

    for (int i = 1; i < (int)count; i++)
    {
        if (!value[i])
            continue;

        result[len] = sep;
        result[len + 1] = '\0';

        strcat(result, value[i]);

        len = strlen(result);
    }

    result[len] = '\0';

    return result;
}

char** MsRdpEx_GetStringVectorFromBlock(int* argc, const char* argb)
{
    int index = 0;
    int length = 0;
    char* arg = NULL;
    char** args = NULL;

    *argc = 0;

    if (!argb) {
        goto exit;
    }

    index = 0;
    arg = (char*) argb;
    do
    {
        length = strlen(arg);
        arg = &arg[length + 1];

        if (length > 0) {
            index++;
        }
    } while (length > 0);

    *argc = index;

    args = (char**) calloc(*argc + 1, sizeof(char*));

    if (!args)
        goto exit;

    index = 0;
    arg = (char*) argb;
    do
    {
        length = strlen(arg);
        args[index] = _strdup(arg);

        if (length > 0) {
            arg = &arg[length + 1];
            index++;
        }
    } while (length > 0);

exit:
    return args;
}

char* MsRdpEx_GetStringBlockFromVector(int argc, char** argv)
{
    int index = 0;
    int offset = 0;
    int length = 0;
    size_t size = 0;
    char* arg = NULL;
    char* argb = NULL;

    for (index = 0; index < argc; index++) {
        arg = argv[index];
        length = strlen(arg);
        size += (length + 1);
    }

    size += 1;

    argb = (char*) calloc(1, size);

    for (index = 0; index < argc; index++) {
        arg = argv[index];
        length = strlen(arg);
        memcpy(&argv[offset], arg, (length + 1));
        offset += (length + 1);
    }

    return argb;
}

char* MsRdpEx_CloneStringBlock(const char* argb)
{
    int length = 0;
    size_t size = 0;
    char* arg = NULL;
    char** args = NULL;
    char* copyb = NULL;

    if (!argb)
        return NULL;

    arg = (char*) argb;
    do
    {
        length = strlen(arg);
        arg = &arg[length + 1];
        size += (length + 1);
    } while (length > 0);

    size += 1;
    copyb = (char*) calloc(1, size);

    if (!copyb)
        return NULL;

    memcpy(copyb, argb, size);
    return copyb;
}

void MsRdpEx_FreeStringBlock(const char* argb)
{
    free((void*) argb);
}

void MsRdpEx_FreeStringVector(int argc, char** argv)
{
    int index;

    if (!argv)
        return;

    for (index = 0; index < argc; index++) {
        free(argv[index]);
    }

    free(argv);
}

char** MsRdpEx_GetArgumentVector(int* argc)
{
	int index;
	char* arg = NULL;
    char** args = NULL;
	LPWSTR* argsW = NULL;
	LPCWSTR cmdlineW = GetCommandLineW();

	if (!cmdlineW)
		return NULL;

	argsW = CommandLineToArgvW(cmdlineW, argc);

	if (!argsW)
		goto exit;

    args = (char**) calloc(*argc + 1, sizeof(char*));

    if (!args)
        goto exit;

    WCHAR moduleFileName[MSRDPEX_MAX_PATH];
    GetModuleFileNameW(NULL, moduleFileName, MSRDPEX_MAX_PATH);

    if (MsRdpEx_ConvertFromUnicode(CP_UTF8, 0, moduleFileName, -1, &arg, 0, NULL, NULL) < 0) {
        goto exit;
    }

    args[0] = arg;

    for (index = 0; index < *argc; index++) {
        arg = NULL;

        if (MsRdpEx_ConvertFromUnicode(CP_UTF8, 0, argsW[index], -1, &arg, 0, NULL, NULL) < 0) {
            goto exit;
        }

        args[index + 1] = arg;
    }

    *argc = *argc + 1;

exit:
	LocalFree(argsW);
	return args;
}

void MsRdpEx_FreeArgumentVector(int argc, char** argv)
{
    int index;

    if (!argv)
        return;

    for (index = 0; index < argc; index++) {
        free(argv[index]);
    }

    free(argv);
}
