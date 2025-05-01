
#include <MsRdpEx/Stopwatch.h>
#include <MsRdpEx/Environment.h>

static bool g_ProfilingInitialized = false;
static uint32_t g_ProfilingLevel = MSRDPEX_PROF_OFF;

static bool MsRdpEx_ProfilingLevelActive(uint32_t profilingLevel)
{
	if (g_ProfilingLevel == MSRDPEX_PROF_OFF)
		return false;

	return profilingLevel >= g_ProfilingLevel;
}

static void MsRdpEx_SetProfilingLevel(uint32_t profilingLevel)
{
	g_ProfilingLevel = profilingLevel;
}

void MsRdpEx_ProfilingInit()
{
    char* envvar = NULL;

    if (g_ProfilingInitialized)
        return;

    if (!MsRdpEx_EnvExists("MSRDPEX_PROF_LEVEL"))
    {
        return;
    }

    envvar = MsRdpEx_GetEnv("MSRDPEX_PROF_LEVEL");

    if (envvar) 
    {
        int ival = atoi(envvar);

        if ((ival >= 0) && (ival <= 3)) 
        {
            MsRdpEx_SetProfilingLevel((uint32_t)ival);
        }
    }

    free(envvar);

    g_ProfilingInitialized = true;
}

void MsRdpEx_Stopwatch_Init(MsRdpEx_Stopwatch* stopwatch, uint32_t profilingLevel, bool start)
{
    MsRdpEx_Stopwatch_InitEx(stopwatch, profilingLevel, start, true);
}

void MsRdpEx_Stopwatch_InitEx(MsRdpEx_Stopwatch* stopwatch, uint32_t profilingLevel, bool start, bool highPrecision)
{
    stopwatch->enabled = MsRdpEx_ProfilingLevelActive(profilingLevel);
	stopwatch->highPrecision = highPrecision;

    if (start)
    {
        MsRdpEx_Stopwatch_Start(stopwatch);
    }
}

void MsRdpEx_Stopwatch_Start(MsRdpEx_Stopwatch* stopwatch)
{
    if (stopwatch->enabled)
    {
        if (stopwatch->highPrecision)
        {
            QueryPerformanceCounter(&stopwatch->time);
        }
        else
        {
            stopwatch->tickCount = GetTickCount64();
        }
    }
}

double MsRdpEx_Stopwatch_GetTime(MsRdpEx_Stopwatch* stopwatch)
{
    if (stopwatch->enabled)
    {
        if (stopwatch->highPrecision)
        {
            LARGE_INTEGER frequency;
            LARGE_INTEGER stop;
            LARGE_INTEGER elapsed;

            QueryPerformanceFrequency(&frequency);
            QueryPerformanceCounter(&stop);

            elapsed.QuadPart = stop.QuadPart - stopwatch->time.QuadPart;
            elapsed.QuadPart *= 1000;
            elapsed.QuadPart /= frequency.QuadPart;

            return (double)elapsed.QuadPart;
        }
        else
        {
            ULONGLONG elapsed = GetTickCount64() - stopwatch->tickCount;
            return (double) elapsed;
        }
    }

    return -1;
}

void MsRdpEx_Stopwatch_Print(MsRdpEx_Stopwatch* stopwatch, uint32_t logLevel, const char* message)
{
    if (!stopwatch->enabled)
    {
        return;
    }

	if (!MsRdpEx_IsLogLevelActive(logLevel))
	{
		return;
	}

    MsRdpEx_Log(logLevel, "%s [%.3fms]", message, MsRdpEx_Stopwatch_GetTime(stopwatch));
}
