#ifndef MSRDPEX_EVENT_SINK_H
#define MSRDPEX_EVENT_SINK_H

#include <MsRdpEx/MsRdpEx.h>

#include "RdpComBase.h"

#define IMsTscAxEvents_OnConnectingId 0x00000001
#define IMsTscAxEvents_OnConnectedId 0x00000002
#define IMsTscAxEvents_OnLoginCompleteId 0x00000003
#define IMsTscAxEvents_OnDisconnectedId 0x00000004
#define IMsTscAxEvents_OnEnterFullScreenModeId 0x00000005
#define IMsTscAxEvents_OnLeaveFullScreenModeId 0x00000006
#define IMsTscAxEvents_OnChannelReceivedDataId 0x00000007
#define IMsTscAxEvents_OnRequestGoFullScreenId 0x00000008
#define IMsTscAxEvents_OnRequestLeaveFullScreenId 0x00000009
#define IMsTscAxEvents_OnFatalErrorId 0x0000000a
#define IMsTscAxEvents_OnWarningId 0x0000000b
#define IMsTscAxEvents_OnRemoteDesktopSizeChangeId 0x0000000c
#define IMsTscAxEvents_OnIdleTimeoutNotificationId 0x0000000d
#define IMsTscAxEvents_OnRequestContainerMinimizeId 0x0000000e
#define IMsTscAxEvents_OnConfirmCloseId 0x0000000f
#define IMsTscAxEvents_OnReceivedTSPublicKeyId 0x00000010
#define IMsTscAxEvents_OnAutoReconnectingId 0x00000011
#define IMsTscAxEvents_OnAuthenticationWarningDisplayedId 0x00000012
#define IMsTscAxEvents_OnAuthenticationWarningDismissedId 0x00000013
#define IMsTscAxEvents_OnRemoteProgramResultId 0x00000014
#define IMsTscAxEvents_OnRemoteProgramDisplayedId 0x00000015
#define IMsTscAxEvents_OnRemoteWindowDisplayedId 0x00000016
#define IMsTscAxEvents_OnLogonErrorId 0x00000017
#define IMsTscAxEvents_OnFocusReleasedId 0x00000018
#define IMsTscAxEvents_OnUserNameAcquiredId 0x00000019
#define IMsTscAxEvents_OnMouseInputModeChangedId 0x0000001a
#define IMsTscAxEvents_OnServiceMessageReceivedId 0x0000001b
#define IMsTscAxEvents_OnConnectionBarPullDownId 0x0000001c
#define IMsTscAxEvents_OnNetworkStatusChangedId 0x0000001d
#define IMsTscAxEvents_OnDevicesButtonPressedId 0x0000001e
#define IMsTscAxEvents_OnAutoReconnectedId 0x0000001f
#define IMsTscAxEvents_OnAutoReconnecting2Id 0x00000020

class CRdpEventSink : public IMsTscAxEvents
{
public:
    // Constructor and Destructor
    CRdpEventSink(HWND hWndParent);
    virtual ~CRdpEventSink();

    // IUnknown methods
    STDMETHOD(QueryInterface)(REFIID riid, void** ppv) override;
    STDMETHOD_(ULONG, AddRef)(void) override;
    STDMETHOD_(ULONG, Release)(void) override;

    // IDispatch methods
    STDMETHOD(GetTypeInfoCount)(UINT* pctinfo) override;
    STDMETHOD(GetTypeInfo)(UINT iTInfo, LCID lcid, ITypeInfo** ppTInfo) override;
    STDMETHOD(GetIDsOfNames)(REFIID riid,
        LPOLESTR* rgszNames, UINT cNames, LCID lcid, DISPID* rgDispId) override;
    STDMETHOD(Invoke)(DISPID dispIdMember,
        REFIID riid, LCID lcid, WORD wFlags,
        DISPPARAMS* pDispParams, VARIANT* pVarResult,
        EXCEPINFO* pExcepInfo, UINT* puArgErr) override;

    // IMsTscAxEvents methods
    STDMETHOD(OnConnecting)(void);
    STDMETHOD(OnConnected)(void);
    STDMETHOD(OnLoginComplete)(void);
    STDMETHOD(OnDisconnected)(long discReason);
    STDMETHOD(OnEnterFullScreenMode)(void);
    STDMETHOD(OnLeaveFullScreenMode)(void);
    STDMETHOD(OnRemoteDesktopSizeChange)(long width, long height);
    STDMETHOD(OnRequestContainerMinimize)(void);
    STDMETHOD(OnConfirmClose)(VARIANT_BOOL* pfAllowClose);

private:
    ULONG m_refCount;
    HWND m_hWndParent;
};

#endif /* MSRDPEX_EVENT_SINK_H */