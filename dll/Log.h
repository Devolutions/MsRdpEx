#ifndef MSRDPEX_LOG_INTERNAL_H
#define MSRDPEX_LOG_INTERNAL_H

#ifdef __cplusplus
extern "C" {
#endif

// Only for DLL_PROCESS_DETACH with a non-null reserved pointer: other threads
// have terminated and may own the logger lock or a CRT stream lock.
void MsRdpEx_LogPrepareForProcessExit(void);

#ifdef __cplusplus
}
#endif

#endif /* MSRDPEX_LOG_INTERNAL_H */
