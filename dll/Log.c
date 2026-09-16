
#include <MsRdpEx/MsRdpEx.h>

#include <MsRdpEx/Environment.h>
#include "Log.h"

static bool g_LogInitialized = false;
static SRWLOCK g_LogLock = SRWLOCK_INIT;
static __declspec(thread) bool g_LogInProgress = false;

void MsRdpEx_LogPrepareForProcessExit(void)
{
    // Suppress all logger entry points on the terminating thread, including
    // LogClose. Do not acquire abandoned locks or touch a possibly locked FILE.
    // This leaves the rest of DLL teardown (including recording finalization)
    // intact. The OS reclaims the diagnostic file handle at process exit.
    g_LogInProgress = true;
}

static FILE* g_LogFile = NULL;
static bool g_LogEnabled = false;
static char g_LogFilePath[MSRDPEX_MAX_PATH] = { 0 };
static bool g_LogFilePathValid = true;

static uint32_t g_LogLevel = MSRDPEX_LOG_DEBUG;

#define MSRDPEX_LOG_MAX_LINE    8192

LPCSTR LOG_LEVELS[7] = { "TRACE", "DEBUG", "INFO", "WARN", "ERROR", "FATAL", "OFF" };

// Opening a file can invoke hooks which log on the same thread. Suppress those
// nested calls before acquiring the non-recursive lock.
static bool MsRdpEx_LogEnter(void)
{
    if (g_LogInProgress)
        return false;

    g_LogInProgress = true;
    AcquireSRWLockExclusive(&g_LogLock);
    return true;
}

static void MsRdpEx_LogLeave(void)
{
    ReleaseSRWLockExclusive(&g_LogLock);
    g_LogInProgress = false;
}

static bool MsRdpEx_LogLevelActiveLocked(uint32_t logLevel)
{
    return g_LogEnabled && g_LogFile && g_LogLevel < MSRDPEX_LOG_OFF &&
        logLevel >= g_LogLevel && logLevel < MSRDPEX_LOG_OFF;
}

bool MsRdpEx_IsLogLevelActive(uint32_t logLevel)
{
    if (!MsRdpEx_LogEnter())
        return false;

    bool active = MsRdpEx_LogLevelActiveLocked(logLevel);
    MsRdpEx_LogLeave();
    return active;
}

bool MsRdpEx_LogVA(uint32_t level, const char* format, va_list args)
{
    if (!MsRdpEx_LogEnter())
        return true;

    // The caller's level check may have raced with a configuration change.
    if (!MsRdpEx_LogLevelActiveLocked(level)) {
        MsRdpEx_LogLeave();
        return true;
    }

    SYSTEMTIME st;
    GetLocalTime(&st);

    DWORD pid = GetCurrentProcessId();
    DWORD tid = GetCurrentThreadId();

    char message[MSRDPEX_LOG_MAX_LINE];
    vsnprintf_s(message, MSRDPEX_LOG_MAX_LINE - 1, _TRUNCATE, format, args);

    int written = fprintf(g_LogFile, "[%s] %04d-%02d-%02d %02d:%02d:%02d.%03d PID:%lu TID:%lu - %s\n",
        LOG_LEVELS[level],
        st.wYear, st.wMonth, st.wDay,
        st.wHour, st.wMinute, st.wSecond, st.wMilliseconds,
        pid, tid,
        message);
    int flushed = fflush(g_LogFile); // WARNING: performance drag
    MsRdpEx_LogLeave();

    return written >= 0 && flushed == 0;
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
    if (!MsRdpEx_LogEnter())
        return;

    if (!g_LogEnabled || !g_LogFile || g_LogLevel >= MSRDPEX_LOG_OFF) {
        MsRdpEx_LogLeave();
        return;
    }

    size_t i;
    int ln, hn;
    const uint8_t* p = data;
    size_t width = 16;
    size_t offset = 0;
    size_t chunk = 0;
    char line[512] = { 0 };
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

    fflush(g_LogFile);
    MsRdpEx_LogLeave();
}

// These helpers require g_LogLock and must not call the public logging setters.
static void MsRdpEx_LogCloseLocked(void)
{
    if (g_LogFile) {
        fclose(g_LogFile);
        g_LogFile = NULL;
    }
}

static bool MsRdpEx_LogSetPathLocked(const char* path)
{
    if (!path || strnlen(path, MSRDPEX_MAX_PATH) >= MSRDPEX_MAX_PATH) {
        MsRdpEx_LogCloseLocked();
        g_LogFilePathValid = false;
        OutputDebugStringA("MsRdpEx: the diagnostic log path is null or too long.\n");
        return false;
    }

    if (!g_LogFilePathValid || strcmp(g_LogFilePath, path) != 0) {
        MsRdpEx_LogCloseLocked();
        strcpy_s(g_LogFilePath, MSRDPEX_MAX_PATH, path);
    }

    g_LogFilePathValid = true;
    return true;
}

static void MsRdpEx_LogOpenLocked(const char* mode)
{
    if (!g_LogEnabled || g_LogFile || !g_LogFilePathValid)
        return;

    char defaultPath[MSRDPEX_MAX_PATH];
    const char* path = g_LogFilePath;
    if (g_LogFilePath[0] == '\0') {
        const char* appDataPath = MsRdpEx_GetPath(MSRDPEX_APP_DATA_PATH);
        if (!appDataPath || snprintf(defaultPath, MSRDPEX_MAX_PATH,
            "%s\\MsRdpEx.log", appDataPath) >= MSRDPEX_MAX_PATH) {
            g_LogFilePathValid = false;
            OutputDebugStringA("MsRdpEx: the default diagnostic log path is invalid.\n");
            return;
        }
        // Keep the requested empty path so identical configuration does not
        // close and reopen an already-open default log.
        path = defaultPath;
    }

    g_LogFile = MsRdpEx_FileOpen(path, mode);
    if (!g_LogFile)
        OutputDebugStringA("MsRdpEx: could not open the configured diagnostic log file.\n");
}

static void MsRdpEx_LogEnvInitLocked(void)
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
        MsRdpEx_LogSetPathLocked(envvar);
    }

    free(envvar);

    g_LogInitialized = true;
}

void MsRdpEx_LogEnvInit()
{
    if (!MsRdpEx_LogEnter())
        return;

    MsRdpEx_LogEnvInitLocked();
    MsRdpEx_LogLeave();
}

void MsRdpEx_LogOpen()
{
    if (!MsRdpEx_LogEnter())
        return;

    bool initializing = !g_LogInitialized;
    MsRdpEx_LogEnvInitLocked();
    MsRdpEx_LogOpenLocked(initializing ? "wb" : "ab");
    MsRdpEx_LogLeave();
}

void MsRdpEx_LogClose()
{
    if (!MsRdpEx_LogEnter())
        return;

    MsRdpEx_LogCloseLocked();
    MsRdpEx_LogLeave();
}

void MsRdpEx_SetLogEnabled(bool logEnabled)
{
    if (!MsRdpEx_LogEnter())
        return;

    g_LogEnabled = logEnabled;
    if (logEnabled)
        MsRdpEx_LogOpenLocked("ab");
    else
        MsRdpEx_LogCloseLocked();

    MsRdpEx_LogLeave();
}

void MsRdpEx_SetLogLevel(uint32_t logLevel)
{
    if (!MsRdpEx_LogEnter())
        return;

    g_LogLevel = logLevel;
    MsRdpEx_LogLeave();
}

void MsRdpEx_SetLogFilePath(const char* logFilePath)
{
    if (!MsRdpEx_LogEnter())
        return;

    if (MsRdpEx_LogSetPathLocked(logFilePath))
        MsRdpEx_LogOpenLocked("ab");

    MsRdpEx_LogLeave();
}
