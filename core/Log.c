
#include <MsRdpEx/MsRdpEx.h>

FILE* g_LogFile = NULL;

#define MSRDPEX_LOG_MAX_LINE    8192

bool MsRdpEx_LogVA(const char* format, va_list args)
{
    char message[MSRDPEX_LOG_MAX_LINE];
    vsnprintf_s(message, MSRDPEX_LOG_MAX_LINE - 1, _TRUNCATE, format, args);
    strcat_s(message, MSRDPEX_LOG_MAX_LINE - 1, "\n");
    fprintf(g_LogFile, message);
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
    char filename[MSRDPEX_MAX_PATH];
    const char* appDataPath = MsRdpEx_GetPath(MSRDPEX_APP_DATA_PATH);
    sprintf_s(filename, MSRDPEX_MAX_PATH, "%s\\MsRdpEx.log", appDataPath);
    g_LogFile = fopen(filename, "wb");
}

void MsRdpEx_LogClose()
{
    if (g_LogFile) {
        fclose(g_LogFile);
        g_LogFile = NULL;
    }
}
