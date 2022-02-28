#ifndef MSRDPEX_DETOURS_H
#define MSRDPEX_DETOURS_H

#include <MsRdpEx/MsRdpEx.h>

#include <detours.h>

#define MSRDPEX_DETOUR_ATTACH(_realFn, _hookFn) \
	if (_realFn) DetourAttach((PVOID*)(&_realFn), _hookFn);

#define MSRDPEX_DETOUR_DETACH(_realFn, _hookFn) \
	if (_realFn) DetourDetach((PVOID*)(&_realFn), _hookFn);

#define MSRDPEX_GETPROCADDRESS(_funcPtr, _funcType, _hModule, _funcName) \
	_funcPtr = ( _funcType ) GetProcAddress(_hModule, _funcName);

#ifdef __cplusplus
extern "C" {
#endif



#ifdef __cplusplus
}
#endif

#endif // MSRDPEX_DETOURS_H