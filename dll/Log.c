
#include <MsRdpEx/MsRdpEx.h>

#include <MsRdpEx/Environment.h>

static bool g_LogInitialized = false;

static FILE* volatile g_LogFile = NULL;
static bool g_LogEnabled = false;
static char g_LogFilePath[MSRDPEX_MAX_PATH] = { 0 };

static uint32_t g_LogLevel = MSRDPEX_LOG_DEBUG;

#define MSRDPEX_LOG_MAX_LINE    8192

LPCSTR LOG_LEVELS[7] = { "TRACE", "DEBUG", "INFO", "WARN", "ERROR", "FATAL", "OFF" };

bool MsRdpEx_IsLogLevelActive(uint32_t logLevel)
{
    if (!g_LogEnabled)
        return false;

    if (g_LogLevel == MSRDPEX_LOG_OFF)
        return false;

    return logLevel >= g_LogLevel;
}

bool MsRdpEx_LogVA(uint32_t level, const char* format, va_list args)
{
    FILE* logFile = g_LogFile;

    // Re-check the configuration: the caller's level test may have raced with
    // a change. In-flight records can still land just after a disable.
    if (!logFile || !MsRdpEx_IsLogLevelActive(level))
        return true;

    SYSTEMTIME st;
    GetLocalTime(&st);

    DWORD pid = GetCurrentProcessId();
    DWORD tid = GetCurrentThreadId();

    char message[MSRDPEX_LOG_MAX_LINE];
    vsnprintf_s(message, MSRDPEX_LOG_MAX_LINE - 1, _TRUNCATE, format, args);

    fprintf(logFile, "[%s] %04d-%02d-%02d %02d:%02d:%02d.%03d PID:%lu TID:%lu - %s\n",
        LOG_LEVELS[level],
        st.wYear, st.wMonth, st.wDay,
        st.wHour, st.wMinute, st.wSecond, st.wMilliseconds,
        pid, tid,
        message);
    fflush(logFile); // WARNING: performance drag

    return true;
}

bool MsRdpEx_Log(uint32_t level, const char* format, ...)
{
	bool status;
	va_list args;
	va_start(args, format);
	status = MsRdpEx_LogVA(level, format, args);
	va_end(args);
	return status;
}

void MsRdpEx_LogHexDump(const uint8_t* data, size_t size)
{
    size_t i;
    int ln, hn;
    const uint8_t* p = data;
    size_t width = 16;
    size_t offset = 0;
    size_t chunk = 0;
    char line[512] = { 0 };
    char* bin2hex = "0123456789ABCDEF";
    FILE* logFile = g_LogFile;

    if (!logFile || !g_LogEnabled || (g_LogLevel == MSRDPEX_LOG_OFF))
        return;

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

        fwrite(line, 1, strlen(line), logFile);

        offset += chunk;
        p += chunk;
    }
}

// The diagnostic file is opened at most once per process. Configuration
// setters only select what that single open uses, so a writer can never be
// inside fprintf on a handle that another thread replaced or closed.
static void MsRdpEx_LogOpenOnce(const char* mode)
{
    if (!g_LogEnabled || g_LogFile)
        return;

    char defaultPath[MSRDPEX_MAX_PATH];
    const char* path = g_LogFilePath;

    if (g_LogFilePath[0] == '\0') {
        const char* appDataPath = MsRdpEx_GetPath(MSRDPEX_APP_DATA_PATH);

        if (!appDataPath || sprintf_s(defaultPath, MSRDPEX_MAX_PATH,
            "%s\\MsRdpEx.log", appDataPath) < 0) {
            OutputDebugStringA("MsRdpEx: the default diagnostic log path is invalid.\n");
            return;
        }

        path = defaultPath;
    }

    FILE* logFile = MsRdpEx_FileOpen(path, mode);

    if (!logFile) {
        OutputDebugStringA("MsRdpEx: could not open the configured diagnostic log file.\n");
        return;
    }

    if (InterlockedCompareExchangePointer((PVOID volatile*) &g_LogFile, logFile, NULL)) {
        fclose(logFile); // another thread published its handle first
    }
}

void MsRdpEx_LogEnvInit()
{
    char* envvar;

    if (g_LogInitialized)
        return;

    bool logEnabled = MsRdpEx_EnvExists("MSRDPEX_LOG_LEVEL");

    if (logEnabled) {
        // only set if true to avoid overriding current value
        g_LogEnabled = true;
    }

    envvar = MsRdpEx_GetEnv("MSRDPEX_LOG_LEVEL");

    if (envvar) {

        if (MsRdpEx_StringIEquals(envvar, "TRACE")) 
        {
            g_LogLevel = MSRDPEX_LOG_TRACE;
        }
        else if (MsRdpEx_StringIEquals(envvar, "DEBUG")) 
        {
            g_LogLevel = MSRDPEX_LOG_DEBUG;
        }
        else if (MsRdpEx_StringIEquals(envvar, "INFO")) 
        {
            g_LogLevel = MSRDPEX_LOG_INFO;
        }
        else if (MsRdpEx_StringIEquals(envvar, "WARN")) 
        {
            g_LogLevel = MSRDPEX_LOG_WARN;
        }
        else if (MsRdpEx_StringIEquals(envvar, "ERROR")) 
        {
            g_LogLevel = MSRDPEX_LOG_ERROR;
        }
        else if (MsRdpEx_StringIEquals(envvar, "FATAL")) 
        {
            g_LogLevel = MSRDPEX_LOG_FATAL;
        }
        else if (MsRdpEx_StringIEquals(envvar, "OFF")) 
        {
            g_LogLevel = MSRDPEX_LOG_OFF;
        }
        else
        {
            int ival = atoi(envvar);

            if ((ival >= 0) && (ival <= 6)) 
            {
                g_LogLevel = (uint32_t)ival;
            }
        }
    }

    free(envvar);

    envvar = MsRdpEx_GetEnv("MSRDPEX_LOG_FILE_PATH");

    if (envvar) {
        MsRdpEx_SetLogFilePath(envvar);
    }

    free(envvar);

    g_LogInitialized = true;
}

void MsRdpEx_LogOpen()
{
    MsRdpEx_LogEnvInit();

    // Startup keeps the existing truncate behavior for a new process.
    MsRdpEx_LogOpenOnce("wb");
}

void MsRdpEx_LogClose()
{
    FILE* logFile = InterlockedExchangePointer((PVOID volatile*) &g_LogFile, NULL);

    if (logFile) {
        fclose(logFile);
    }
}

void MsRdpEx_SetLogEnabled(bool logEnabled)
{
    g_LogEnabled = logEnabled;

    // Enabling after the DLL has loaded opens the log on first use. Disabling
    // stops new records but intentionally keeps the handle: closing it while
    // other session threads may be writing is not safe without locking.
    if (logEnabled)
        MsRdpEx_LogOpenOnce("ab");
}

void MsRdpEx_SetLogLevel(uint32_t logLevel)
{
    g_LogLevel = logLevel;
}

// Only takes effect while the diagnostic log is not open yet: the destination
// is fixed for the lifetime of the process once logging has started.
void MsRdpEx_SetLogFilePath(const char* logFilePath)
{
    if (!logFilePath || strnlen(logFilePath, MSRDPEX_MAX_PATH) >= MSRDPEX_MAX_PATH) {
        OutputDebugStringA("MsRdpEx: the diagnostic log path is null or too long.\n");
        return;
    }

    strcpy_s(g_LogFilePath, MSRDPEX_MAX_PATH, logFilePath);
}
