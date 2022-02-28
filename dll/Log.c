
#include <MsRdpEx/MsRdpEx.h>

static FILE* g_LogFile = NULL;
static bool g_LogEnabled = true;
static char g_LogFilePath[MSRDPEX_MAX_PATH] = { 0 };

static uint32_t g_LogLevel = MSRDPEX_LOG_DEBUG;

#define MSRDPEX_LOG_MAX_LINE    8192

bool MsRdpEx_IsLogLevelActive(uint32_t logLevel)
{
    if (!g_LogEnabled)
        return false;

    if (g_LogLevel == MSRDPEX_LOG_OFF)
        return false;

    return logLevel >= g_LogLevel;
}

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

void MsRdpEx_LogHexDump(const uint8_t* data, size_t size)
{
    int i, ln, hn;
	const uint8_t* p = data;
    size_t width = 16;
    size_t offset = 0;
    size_t chunk = 0;
    char line[512];
    char* bin2hex = "0123456789ABCDEF";

    while (offset < size) {
        chunk = size - offset;

        if (chunk >= width)
            chunk = width;

        for (i = 0; i < chunk; i++)
        {
            ln = p[i] & 0xF;
            hn = (p[i] >> 4) & 0xF;

            line[i * 2] = bin2hex[hn];
            line[(i * 2) + 1] = bin2hex[ln];
        }

        line[chunk * 2] = ' ';

        for (i = chunk; i < width; i++) {
            line[i * 2] = ' ';
            line[(i * 2) + 1] = ' ';
        }

        char* side = &line[(width * 2) + 1];

        for (i = 0; i < chunk; i++)
        {
            char c = ((p[i] >= 0x20) && (p[i] < 0x7F)) ? p[i] : '.';
            side[i] = c;
        }
        side[i] = '\n';
        side[i+1] = '\0';

        if (g_LogFile) {
            fwrite(line, 1, strlen(line), g_LogFile);
        }

        offset += chunk;
        p += chunk;
    }
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

void MsRdpEx_SetLogLevel(uint32_t logLevel)
{
    g_LogLevel = logLevel;
}

void MsRdpEx_SetLogFilePath(const char* logFilePath)
{
    strcpy_s(g_LogFilePath, MSRDPEX_MAX_PATH, logFilePath);
}
