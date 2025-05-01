#ifndef MSRDPEX_STOPWATCH_H
#define MSRDPEX_STOPWATCH_H

#include <MsRdpEx/MsRdpEx.h>

#ifdef __cplusplus
extern "C" {
#endif

	struct msrdpex_stopwatch
	{
		LARGE_INTEGER time;
		ULONGLONG tickCount;
		bool enabled;
		bool highPrecision;
	};

	typedef struct msrdpex_stopwatch MsRdpEx_Stopwatch;

	void MsRdpEx_ProfilingInit();

	void MsRdpEx_Stopwatch_Init(MsRdpEx_Stopwatch* stopwatch, uint32_t profilingLevel, bool start);
	void MsRdpEx_Stopwatch_InitEx(MsRdpEx_Stopwatch* stopwatch, uint32_t profilingLevel, bool start, bool highPrecision);
	void MsRdpEx_Stopwatch_Start(MsRdpEx_Stopwatch* stopwatch);
	double MsRdpEx_Stopwatch_GetTime(MsRdpEx_Stopwatch* stopwatch);
	void MsRdpEx_Stopwatch_Print(MsRdpEx_Stopwatch* stopwatch, uint32_t logLevel, const char* message);

#ifdef __cplusplus
}
#endif

#endif /* MSRDPEX_STOPWATCH_H */
