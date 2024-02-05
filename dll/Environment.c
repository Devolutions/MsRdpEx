
#include <MsRdpEx/Environment.h>

#include <UserEnv.h>

bool MsRdpEx_SetEnv(const char* name, const char* value)
{
	bool result = false;
	WCHAR* nameW = NULL;
	WCHAR* valueW = NULL;

	if (!name)
	{
		goto exit;
	}

	if (MsRdpEx_ConvertToUnicode(CP_UTF8, 0, name, -1, &nameW, 0) < 1) {
		goto exit;
	}

	if (value) { // optional
		if (MsRdpEx_ConvertToUnicode(CP_UTF8, 0, value, -1, &valueW, 0) < 1) {
			goto exit;
		}
	}

	result = SetEnvironmentVariableW(nameW, valueW) ? true : false;

exit:
	free(nameW);
	free(valueW);
	return result;
}

char* MsRdpEx_GetEnv(const char* name)
{
	DWORD nSizeW;
	WCHAR* nameW = NULL;
	WCHAR* valueW = NULL;
	char* value = NULL;

	if (!name)
	{
		goto exit;
	}

	if (MsRdpEx_ConvertToUnicode(CP_UTF8, 0, name, -1, &nameW, 0) < 1) {
		goto exit;
	}

	nSizeW = GetEnvironmentVariableW(nameW, NULL, 0);

	if (!nSizeW)
		return NULL;

	valueW = (WCHAR*) malloc((nSizeW + 1) * 2);

	if (!valueW) {
		goto exit;
	}

	nSizeW = GetEnvironmentVariableW(nameW, valueW, nSizeW);

	valueW[nSizeW] = '\0';

	if (MsRdpEx_ConvertFromUnicode(CP_UTF8, 0, valueW, -1, &value, 0, NULL, NULL) < 0) {
		goto exit;
	}

exit:
	free(nameW);
	free(valueW);
	return value;
}

bool MsRdpEx_EnvExists(const char* name)
{
	if (!name)
		return false;

	return GetEnvironmentVariableA(name, NULL, 0) ? true : false;
}

bool MsRdpEx_GetEnvBool(const char* name, bool defaultValue)
{
	char* env;
	bool value = defaultValue;

	env = MsRdpEx_GetEnv(name);

	if (!env)
		return value;

	if ((strcmp(env, "1") == 0) || (_stricmp(env, "TRUE") == 0))
		value = true;
	else if ((strcmp(env, "0") == 0) || (_stricmp(env, "FALSE") == 0))
		value = false;

	free(env);

	return value;
}

int MsRdpEx_GetEnvInt(const char* name, int defaultValue)
{
	char* env;
	int value = defaultValue;

	env = MsRdpEx_GetEnv(name);

	if (!env)
		return value;

	value = atoi(env);

	free(env);

	return value;
}

char** MsRdpEx_GetEnvironmentVariables(int* envc)
{
	int count;
	int index;
	size_t length;
	char* env = NULL;
	char** envs = NULL;
	LPCWSTR pEnvW = NULL;
	LPVOID lpEnvBlock = NULL;

	if (!CreateEnvironmentBlock(&lpEnvBlock, NULL, TRUE)) {
		return NULL;
	}

	count = 0;
	pEnvW = (WCHAR*) lpEnvBlock;

	while (true)
	{
		length = wcslen(pEnvW);

		if (length < 1)
			break;

		pEnvW = &pEnvW[length + 1];
		count++;
	}

	envs = (char**) calloc(count + 1, sizeof(char*));

	if (!envs)
		goto fail;

	pEnvW = (WCHAR*) lpEnvBlock;

	for (index = 0; index < count; index++)
	{
		length = wcslen(pEnvW);

		env = NULL;

		if (MsRdpEx_ConvertFromUnicode(CP_UTF8, 0, pEnvW, -1, &env, 0, NULL, NULL) < 0) {
			goto fail;
		}

		envs[index] = env;

		pEnvW = &pEnvW[length + 1];
	}

	*envc = count;

	DestroyEnvironmentBlock(lpEnvBlock);
	return envs;
fail:
	DestroyEnvironmentBlock(lpEnvBlock);
	free(envs);
	return NULL;
}

void MsRdpEx_FreeEnvironmentVariables(int envc, char** envs)
{
	int index;

	if (!envs)
		return;

	for (index = 0; index < envc; index++) {
		free(envs[index]);
	}

	free(envs);
}

char* MsRdpEx_ReadTextFromNamedPipe(const char* pipeName)
{
	size_t size = 0;
	size_t length = 0;
	DWORD readBytes = 0;
	char* text = NULL;
	char* buffer = NULL;
	char* tempBuffer = NULL;
	HANDLE pipeHandle = NULL;
	char filename[MSRDPEX_MAX_PATH];

	if (!pipeName)
		return NULL;

	sprintf_s(filename, sizeof(filename) - 1, "\\\\.\\pipe\\%s", pipeName);

	pipeHandle = CreateFileA(filename, GENERIC_READ, 0, NULL, OPEN_EXISTING, 0, NULL);

	if (pipeHandle == INVALID_HANDLE_VALUE) {
		goto exit;
	}

	size = 1024;
	buffer = (char*) malloc(size);

	if (!buffer)
		goto exit;

	length = 0;
	while (1) {
		if (!ReadFile(pipeHandle, buffer + length, size - length, &readBytes, NULL) || (readBytes == 0)) {
			// If no bytes were read or an error occurred, break out of the loop.
			break;
		}

		length += readBytes;
		if ((size - length) <= 1) {
			tempBuffer = (char*) realloc(buffer, size * 2);
			if (!tempBuffer) {
				goto exit;
			}
			buffer = tempBuffer;
			size = size * 2;
		}
	}
	buffer[length] = '\0';
	text = _strdup(buffer);

exit:
	if (buffer) {
		SecureZeroMemory(buffer, size);
		free(buffer);
	}
	if (pipeHandle && (pipeHandle != INVALID_HANDLE_VALUE)) {
		CloseHandle(pipeHandle);
	}
	return text;
}
