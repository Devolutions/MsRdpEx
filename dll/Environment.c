
#include <MsRdpEx/Environment.h>

#include <UserEnv.h>

bool MsRdpEx_SetEnv(const char* name, const char* value)
{
	return SetEnvironmentVariableA(name, value) ? true : false;
}

char* MsRdpEx_GetEnv(const char* name)
{
	uint32_t size;
	char* env = NULL;

	size = GetEnvironmentVariableA(name, NULL, 0);

	if (!size)
		return NULL;

	env = (char*) malloc(size);

	if (!env)
		return NULL;

	if (GetEnvironmentVariableA(name, env, size) != size - 1)
	{
		free(env);
		return NULL;
	}

	return env;
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
