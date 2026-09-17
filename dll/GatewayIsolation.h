#pragma once

#include <MsRdpEx/MsRdpEx.h>

bool MsRdpEx_GetGatewayIsolationEnabled();
void MsRdpEx_SetGatewayIsolationEnabled(bool enabled);
void MsRdpEx_AttachGatewayIsolationHooks();
void MsRdpEx_DetachGatewayIsolationHooks();
void MsRdpEx_GatewayIsolationHooksCommitted(LONG error);
void MsRdpEx_GatewayIsolationPrepareForProcessExit();
