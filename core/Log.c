
#include <MsRdpEx/MsRdpEx.h>

static FILE* g_LogFile = NULL;
static bool g_LogEnabled = true;
static char g_LogFilePath[MSRDPEX_MAX_PATH] = { 0 };

#define MSRDPEX_LOG_MAX_LINE    8192

bool MsRdpEx_LogVA(const char* format, va_list args)
{
    if (!g_LogFile)
        return true;

    char message[MSRDPEX_LOG_MAX_LINE];
    vsnprintf_s(message, MSRDPEX_LOG_MAX_LINE - 1, _TRUNCATE, format, args);
    strcat_s(message, MSRDPEX_LOG_MAX_LINE - 1, "\n");

    if (g_LogFile) {
        fprintf(g_LogFile, message);
        fflush(g_LogFile); // WARNING: performance drag
    }

    return true;
}

bool MsRdpEx_Log(const char* format, ...)
{
	bool status;
	va_list args;
	va_start(args, format);
	status = MsRdpEx_LogVA(format, args);
	va_end(args);
	return status;
}

void MsRdpEx_LogOpen()
{
    if (!g_LogEnabled)
        return;

    if (g_LogFilePath[0] == '\0') {
        const char* appDataPath = MsRdpEx_GetPath(MSRDPEX_APP_DATA_PATH);
        sprintf_s(g_LogFilePath, MSRDPEX_MAX_PATH, "%s\\MsRdpEx.log", appDataPath);
    }

    g_LogFile = fopen(g_LogFilePath, "wb");
}

void MsRdpEx_LogClose()
{
    if (g_LogFile) {
        fclose(g_LogFile);
        g_LogFile = NULL;
    }
}

void MsRdpEx_SetLogEnabled(bool logEnabled)
{
    g_LogEnabled = logEnabled;
}

void MsRdpEx_SetLogFilePath(const char* logFilePath)
{
    strcpy_s(g_LogFilePath, MSRDPEX_MAX_PATH, logFilePath);
}
