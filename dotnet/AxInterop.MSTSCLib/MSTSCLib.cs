using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace MSTSCLib
{
    #region IDispatch

    [GeneratedComInterface]
    [Guid("00020400-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IDispatch
    {
        void GetTypeInfoCount( // virtual HRESULT STDMETHODCALLTYPE GetTypeInfoCount
            out int count); // /* [out] */ __RPC__out UINT *pctinfo

        void GetTypeInfo( // virtual HRESULT STDMETHODCALLTYPE GetTypeInfo
            int index, // /* [in] */ UINT iTInfo
            int language, // /* [in] */ LCID lcid
            [MarshalAs(UnmanagedType.Interface)] out object type); // /* [out] */ __RPC__deref_out_opt ITypeInfo **ppTInfo

        void GetIDsOfNames( // virtual HRESULT STDMETHODCALLTYPE GetIDsOfNames
            in Guid riid, // /* [in] */ __RPC__in REFIID riid
            nint* rgszNames, // /* [size_is][in] */ __RPC__in_ecount_full(cNames) LPOLESTR *rgszNames
            int cNames, // /* [range][in] */ __RPC__in_range(0,16384) UINT cNames
            int lcid, // /* [in] */ LCID lcid
            int* rgDispId); // /* [size_is][out] */ __RPC__out_ecount_full(cNames) DISPID* rgDispId

        void Invoke( // virtual /* [local] */ HRESULT STDMETHODCALLTYPE Invoke
            int dispIdMember, // /* [annotation][in] */ _In_ DISPID dispIdMember
            in Guid riid, // /* [annotation][in] */ _In_ REFIID riid
            int lcid, // /* [annotation][in] */ _In_ LCID lcid
            IDispatchFlags wFlags, // /* [annotation][in] */ _In_ WORD wFlags
            DISPPARAMS* pDispParams, // /* [annotation][out][in] */ _In_ DISPPARAMS *pDispParams
            VARIANT* pVarResult, // /* [annotation][out] */ _Out_opt_ VARIANT *pVarResult
            EXCEPINFO* pExcepInfo, // /* [annotation][out] */ _Out_opt_ EXCEPINFO *pExcepInfo
            int* puArgErr); // /* [annotation][out] */ _Out_opt_ UINT *puArgErr
    }

    [Flags]
    public enum IDispatchFlags : short
    {
        None = 0,
        Method = 0x1, // DISPATCH_METHOD = 0x1
        PropertyGet = 0x2, // DISPATCH_PROPERTYGET = 0x2
        PropertyPut = 0x4, // DISPATCH_PROPERTYPUT = 0x4
        PropertyPutRef = 0x8, // DISPATCH_PROPERTYPUTREF = 0x8
    }

    public unsafe struct DISPPARAMS // struct DISPPARAMS
    {
        public VARIANT* Arguments; // /* [size_is] */ VARIANTARG *rgvarg;
        public int* NamedArguments; // /* [size_is] */ DISPID *rgdispidNamedArgs;
        public int ArgumentCount; // UINT cArgs;
        public int NamedArgumentCount; // UINT cNamedArgs;
    }

    public unsafe struct EXCEPINFO // struct EXCEPINFO
    {
        public short Code; // WORD wCode;
        public short Reserved1; // WORD wReserved;
        public nint Source; // BSTR bstrSource;
        public nint Description; // BSTR bstrDescription;
        public nint HelpFile; // BSTR bstrHelpFile;
        public int HelpContext; // DWORD dwHelpContext;
        public nint Reserved2; // PVOID pvReserved;
        public delegate* unmanaged[Stdcall]<EXCEPINFO*, int> DeferredFillIn; // HRESULT(__stdcall* pfnDeferredFillIn)(struct tagEXCEPINFO *);
        public int SCode; // SCODE scode;
    }

    #endregion

    #region Client

    [GeneratedComInterface]
    [Guid("327BB5CD-834E-4400-AEF2-B30E15E5D682")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsTscAx_Redist : IDispatch
    {
    }

    [GeneratedComInterface]
    [Guid("8C11EFAE-92C3-11D1-BC1E-00C04FA31489")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsTscAx : IMsTscAx_Redist
    {
        public void SetServer([MarshalAs(UnmanagedType.BStr)] string value);

        [return: MarshalAs(UnmanagedType.BStr)]
        public string GetServer();

        void SetDomain([MarshalAs(UnmanagedType.BStr)] string value);

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetDomain();

        public void SetUserName([MarshalAs(UnmanagedType.BStr)] string value);

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetUserName();

        void SetDisconnectedText([MarshalAs(UnmanagedType.BStr)] string value);

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetDisconnectedText();

        void SetConnectingText([MarshalAs(UnmanagedType.BStr)] string value);

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetConnectingText();

        short GetConnected();

        void SetDesktopWidth(int value);

        int GetDesktopWidth();

        void SetDesktopHeight(int value);

        int GetDesktopHeight();

        void SetStartConnected(int value);

        int GetStartConnected();

        int GetHorizontalScrollBarVisible();

        int GetVerticalScrollBarVisible();

        void SetFullScreenTitle([MarshalAs(UnmanagedType.BStr)] string value);

        int GetCipherStrength();

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetVersion();

        int GetSecuredSettingsEnabled();

        [return: MarshalAs(UnmanagedType.Interface)]
        object /*IMsTscSecuredSettings*/ GetSecuredSettings();

        [return: MarshalAs(UnmanagedType.Interface)]
        object /*IMsTscAdvancedSettings*/ GetAdvancedSettings();

        [return: MarshalAs(UnmanagedType.Interface)]
        object /*IMsTscDebug*/ GetDebugger();

        void Connect();

        void Disconnect();

        void CreateVirtualChannels([MarshalAs(UnmanagedType.BStr)] string newVal);

        void SendOnVirtualChannel([MarshalAs(UnmanagedType.BStr)] string chanName, [MarshalAs(UnmanagedType.BStr)] string ChanData);
    }

    [GeneratedComInterface]
    [Guid("92B4A539-7115-4B7C-A5A9-E5D9EFC2780A")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClient : IMsTscAx
    {
        [DispId(100)]
        void SetColorDepth(int value);

        [DispId(100)]
        int GetColorDepth();

        [DispId(101)]
        [return: MarshalAs(UnmanagedType.Interface)]
        object /*IMsRdpClientAdvancedSettings*/ GetAdvancedSettings2();

        void SecuredSettings2();
        //[DispId(102)]
        //IMsRdpClientSecuredSettings SecuredSettings2
        //{
        //    
        //    [DispId(102)]
        //    [return: MarshalAs(UnmanagedType.Interface)]
        //    get;
        //}

        void ExtendedDisconnectReason();
        //[ComAliasName("MSTSCLib.ExtendedDisconnectReasonCode")]
        //[DispId(103)]
        //ExtendedDisconnectReasonCode ExtendedDisconnectReason
        //{
        //    
        //    [DispId(103)]
        //    [return: ComAliasName("MSTSCLib.ExtendedDisconnectReasonCode")]
        //    get;
        //}

        [DispId(104)]
        void SetFullScreen(VariantBool value);

        [DispId(104)]
        VariantBool GetFullScreen();

        [DispId(35)]
        void SetVirtualChannelOptions([MarshalAs(UnmanagedType.BStr)] string chanName, int chanOptions);

        [DispId(36)]
        int GetVirtualChannelOptions([MarshalAs(UnmanagedType.BStr)] string chanName);

        [DispId(37)]
        int /*ControlCloseStatus*/ RequestClose();
    }

    [GeneratedComInterface]
    [Guid("E7E17DC4-3B71-4BA7-A8E6-281FFADCA28F")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClient2 : IMsRdpClient
    {
        [return: MarshalAs(UnmanagedType.Interface)]
        object /*IMsRdpClientAdvancedSettings2*/ GetAdvancedSettings3();

        void SetConnectedStatusText([MarshalAs(UnmanagedType.BStr)] string value);

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetConnectedStatusText();
    }

    [GeneratedComInterface]
    [Guid("91B7CBC5-A72E-4FA0-9300-D647D7E897FF")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClient3 : IMsRdpClient2
    {
        [return: MarshalAs(UnmanagedType.Interface)]
        object /*IMsRdpClientAdvancedSettings3*/ GetAdvancedSettings4();
    }

    [GeneratedComInterface]
    [Guid("095E0738-D97D-488B-B9F6-DD0E8D66C0DE")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClient4 : IMsRdpClient3
    {
        [return: MarshalAs(UnmanagedType.Interface)]
        object /*IMsRdpClientAdvancedSettings4*/ GetAdvancedSettings5();
    }

    [GeneratedComInterface]
    [Guid("4EB5335B-6429-477D-B922-E06A28ECD8BF")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClient5 : IMsRdpClient4
    {
        [return: MarshalAs(UnmanagedType.Interface)]
        object /*IMsRdpClientTransportSettings*/ GetTransportSettings();

        [return: MarshalAs(UnmanagedType.Interface)]
        object /*IMsRdpClientAdvancedSettings5*/ GetAdvancedSettings6();

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetErrorDescription(uint disconnectReason, uint ExtendedDisconnectReason);

        [return: MarshalAs(UnmanagedType.Interface)]
        object /*ITSRemoteProgram*/ GetRemoteProgram();

        [return: MarshalAs(UnmanagedType.Interface)]
        object /*IMsRdpClientShell*/ GetMsRdpClientShell();
    }

    [GeneratedComInterface]
    [Guid("D43B7D80-8517-4B6D-9EAC-96AD6800D7F2")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClient6 : IMsRdpClient5
    {
        [return: MarshalAs(UnmanagedType.Interface)]
        object /*IMsRdpClientAdvancedSettings6*/ GetAdvancedSettings7();

        [return: MarshalAs(UnmanagedType.Interface)]
        object /*IMsRdpClientTransportSettings2*/ GetTransportSettings2();
    }

    [GeneratedComInterface]
    [Guid("B2A5B5CE-3461-444A-91D4-ADD26D070638")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClient7 : IMsRdpClient6
    {
        [return: MarshalAs(UnmanagedType.Interface)]
        object /*IMsRdpClientAdvancedSettings7*/ GetAdvancedSettings8();

        [return: MarshalAs(UnmanagedType.Interface)]
        object /*IMsRdpClientTransportSettings3*/ GetTransportSettings3();

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetStatusText(uint statusCode);

        [return: MarshalAs(UnmanagedType.Interface)]
        object /*IMsRdpClientSecuredSettings2*/ GetSecuredSettings3();

        ITSRemoteProgram2 GetRemoteProgram2();

    }

    [GeneratedComInterface]
    [Guid("4247E044-9271-43A9-BC49-E2AD9E855D62")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClient8 : IMsRdpClient7
    {
        void SendRemoteAction(/*RemoteSessionActionType*/ int actionType);

        IMsRdpClientAdvancedSettings8 GetAdvancedSettings9();

        ControlReconnectStatus Reconnect(uint ulWidth, uint ulHeight);

    }

    [GeneratedComInterface]
    [Guid("28904001-04B6-436C-A55B-0AF1A0883DC9")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClient9 : IMsRdpClient8
    {
        IMsRdpClientTransportSettings4 GetTransportSettings4();

        void SyncSessionDisplaySettings();

        void UpdateSessionDisplaySettings(uint ulDesktopWidth, uint ulDesktopHeight, uint ulPhysicalWidth, uint ulPhysicalHeight, uint ulOrientation, uint ulDesktopScaleFactor, uint ulDeviceScaleFactor);

        void AttachEvent([MarshalAs(UnmanagedType.BStr)] string eventName, IDispatch callback);

        void DetachEvent([MarshalAs(UnmanagedType.BStr)] string eventName, IDispatch callback);
    }

    #endregion

    #region Remote Program

    [GeneratedComInterface]
    [Guid("FDD029F9-467A-4C49-8529-64B521DBD1B4")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface ITSRemoteProgram : IDispatch
    {
        void SetRemoteProgramMode(VariantBool value);

        VariantBool GetRemoteProgramMode();

        void ServerStartProgram([MarshalAs(UnmanagedType.BStr)] string bstrExecutablePath, [MarshalAs(UnmanagedType.BStr)] string bstrFilePath, [MarshalAs(UnmanagedType.BStr)] string bstrWorkingDirectory, VariantBool vbExpandEnvVarInWorkingDirectoryOnServer, [MarshalAs(UnmanagedType.BStr)] string bstrArguments, VariantBool vbExpandEnvVarInArgumentsOnServer);
    }

    [GeneratedComInterface]
    [Guid("92C38A7D-241A-418C-9936-099872C9AF20")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface ITSRemoteProgram2 : ITSRemoteProgram
    {
        void SetRemoteApplicationName([MarshalAs(UnmanagedType.BStr)] string value);

        void SetRemoteApplicationProgram([MarshalAs(UnmanagedType.BStr)] string value);

        void SetRemoteApplicationArgs([MarshalAs(UnmanagedType.BStr)] string value);
    }

    [GeneratedComInterface]
    [Guid("4B84EA77-ACEA-418C-881A-4A8C28AB1510")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface ITSRemoteProgram3 : ITSRemoteProgram2
    {
        void ServerStartApp([MarshalAs(UnmanagedType.BStr)] string bstrAppUserModelId, [MarshalAs(UnmanagedType.BStr)] string bstrArguments, VariantBool vbExpandEnvVarInArgumentsOnServer);
    }

    #endregion

    #region Advanced Settings

    [GeneratedComInterface]
    [Guid("809945CC-4B3B-4A92-A6B0-DBF9B5F2EF2D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsTscAdvancedSettings : IDispatch
    {
        void SetCompress(int value);

        int GetCompress();

        void SetBitmapPeristence(int value);

        int GetBitmapPeristence();

        void SetAllowBackgroundInput(int value);

        int GetAllowBackgroundInput();

        void SetKeyBoardLayoutStr([MarshalAs(UnmanagedType.BStr)] string value);

        void SetPluginDlls([MarshalAs(UnmanagedType.BStr)] string value);

        void SetIconFile([MarshalAs(UnmanagedType.BStr)] string value);

        void SetIconIndex(int value);

        void SetContainerHandledFullScreen(int value);

        int GetContainerHandledFullScreen();

        void SetDisableRdpdr(int value);

        int GetDisableRdpdr();
    }

    [GeneratedComInterface]
    [Guid("3C65B4AB-12B3-465B-ACD4-B8DAD3BFF9E2")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientAdvancedSettings : IMsTscAdvancedSettings
    {
        void SetSmoothScroll(int value);

        int GetSmoothScroll();

        void SetAcceleratorPassthrough(int value);

        int GetAcceleratorPassthrough();

        void SetShadowBitmap(int value);

        int GetShadowBitmap();

        void SetTransportType(int value);

        int GetTransportType();

        void SetSasSequence(int value);

        int GetSasSequence();

        void SetEncryptionEnabled(int value);

        int GetEncryptionEnabled();

        void SetDedicatedTerminal(int value);

        int GetDedicatedTerminal();

        void SetRDPPort(int value);

        int GetRDPPort();

        void SetEnableMouse(int value);

        int GetEnableMouse();

        void SetDisableCtrlAltDel(int value);

        int GetDisableCtrlAltDel();

        void SetEnableWindowsKey(int value);

        int GetEnableWindowsKey();

        void SetDoubleClickDetect(int value);

        int GetDoubleClickDetect();

        void SetMaximizeShell(int value);

        int GetMaximizeShell();

        void SetHotKeyFullScreen(int value);

        int GetHotKeyFullScreen();

        void SetHotKeyCtrlEsc(int value);

        int GetHotKeyCtrlEsc();

        void SetHotKeyAltEsc(int value);

        int GetHotKeyAltEsc();

        void SetHotKeyAltTab(int value);

        int GetHotKeyAltTab();

        void SetHotKeyAltShiftTab(int value);

        int GetHotKeyAltShiftTab();

        void SetHotKeyAltSpace(int value);

        int GetHotKeyAltSpace();

        void SetHotKeyCtrlAltDel(int value);

        int GetHotKeyCtrlAltDel();

        void SetOrderDrawThreshold(int value);

        int GetOrderDrawThreshold();

        void SetBitmapCacheSize(int value);

        int GetBitmapCacheSize();

        void SetBitmapVirtualCacheSize(int value);

        int GetBitmapVirtualCacheSize();

        void SetScaleBitmapCachesByBPP(int value);

        int GetScaleBitmapCachesByBPP();

        void SetNumBitmapCaches(int value);

        int GetNumBitmapCaches();

        void SetCachePersistenceActive(int value);

        int GetCachePersistenceActive();

        void SetPersistCacheDirectory([MarshalAs(UnmanagedType.BStr)] string value);

        void SetBrushSupportLevel(int value);

        int GetBrushSupportLevel();

        void SetMinInputSendInterval(int value);

        int GetMinInputSendInterval();

        void SetInputEventsAtOnce(int value);

        int GetInputEventsAtOnce();

        void GetMaxEventCount(int value);

        int SetMaxEventCount();

        void SetKeepAliveInterval(int value);

        int GetKeepAliveInterval();

        void SetShutdownTimeout(int value);

        int GetShutdownTimeout();

        void SetOverallConnectionTimeout(int value);

        int GetOverallConnectionTimeout();

        void SetSingleConnectionTimeout(int value);

        int GetSingleConnectionTimeout();

        void SetKeyboardType(int value);

        int GetKeyboardType();

        void SetKeyboardSubType(int value);

        int GetKeyboardSubType();

        void SetKeyboardFunctionKey(int value);

        int GetKeyboardFunctionKey();

        void SetWinceFixedPalette(int value);

        int GetWinceFixedPalette();

        void SetConnectToServerConsole(VariantBool value);

        VariantBool GetConnectToServerConsole();

        void SetBitmapPersistence(int value);

        int GetBitmapPersistence();

        void SetMinutesToIdleTimeout(int value);

        int GetMinutesToIdleTimeout();

        void SetSmartSizing(VariantBool value);

        VariantBool GetSmartSizing();

        void SetRdpdrLocalPrintingDocName([MarshalAs(UnmanagedType.BStr)] string value);

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetRdpdrLocalPrintingDocName();

        void SetRdpdrClipCleanTempDirString([MarshalAs(UnmanagedType.BStr)] string value);

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetRdpdrClipCleanTempDirString();

        void SetRdpdrClipPasteInfoString([MarshalAs(UnmanagedType.BStr)] string value);

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetRdpdrClipPasteInfoString();

        void SetClearTextPassword([MarshalAs(UnmanagedType.BStr)] string value);

        void SetDisplayConnectionBar(VariantBool value);

        VariantBool GetDisplayConnectionBar();

        void SetPinConnectionBar(VariantBool value);

        VariantBool GetPinConnectionBar();

        void SetGrabFocusOnConnect(VariantBool value);

        VariantBool GetGrabFocusOnConnect();

        void SetLoadBalanceInfo([MarshalAs(UnmanagedType.BStr)] string value);

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetLoadBalanceInfo();

        void SetRedirectDrives(VariantBool value);

        VariantBool GetRedirectDrives();

        void SetRedirectPrinters(VariantBool value);

        VariantBool GetRedirectPrinters();

        void SetRedirectPorts(VariantBool value);

        VariantBool GetRedirectPorts();

        void SetRedirectSmartCards(VariantBool value);

        VariantBool GetRedirectSmartCards();

        void SetBitmapVirtualCache16BppSize(int value);

        int GetBitmapVirtualCache16BppSize();

        void SetBitmapVirtualCache24BppSize(int value);

        int GetBitmapVirtualCache24BppSize();

        void SetPerformanceFlags(int value);

        int GetPerformanceFlags();

        void SetConnectWithEndpoint(nint value);

        void SetNotifyTSPublicKey(VariantBool value);

        VariantBool GetNotifyTSPublicKey();
    }

    [GeneratedComInterface]
    [Guid("9AC42117-2B76-4320-AA44-0E616AB8437B")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientAdvancedSettings2 : IMsRdpClientAdvancedSettings
    {
        VariantBool GetCanAutoReconnect();

        void SetEnableAutoReconnect(VariantBool value);

        VariantBool GetEnableAutoReconnect();

        void SetMaxReconnectAttempts(int value);

        int GetMaxReconnectAttempts();
    }

    [GeneratedComInterface]
    [Guid("19CD856B-C542-4C53-ACEE-F127E3BE1A59")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientAdvancedSettings3 : IMsRdpClientAdvancedSettings2
    {
        void SetConnectionBarShowMinimizeButton(VariantBool value);

        VariantBool GetConnectionBarShowMinimizeButton();

        void SetConnectionBarShowRestoreButton(VariantBool value);

        VariantBool GetConnectionBarShowRestoreButton();
    }

    [GeneratedComInterface]
    [Guid("FBA7F64E-7345-4405-AE50-FA4A763DC0DE")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientAdvancedSettings4 : IMsRdpClientAdvancedSettings3
    {
        void SetAuthenticationLevel(uint value);

        uint GetAuthenticationLevel();
    }

    [GeneratedComInterface]
    [Guid("FBA7F64E-6783-4405-DA45-FA4A763DABD0")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientAdvancedSettings5 : IMsRdpClientAdvancedSettings4
    {
        void SetRedirectClipboard(VariantBool value);

        VariantBool GetRedirectClipboard();

        void SetAudioRedirectionMode(uint value);

        uint GetAudioRedirectionMode();

        void SetConnectionBarShowPinButton(VariantBool value);

        VariantBool GetConnectionBarShowPinButton();

        void SetPublicMode(VariantBool value);

        VariantBool GetPublicMode();

        void SetRedirectDevices(VariantBool value);

        VariantBool GetRedirectDevices();

        void SetRedirectPOSDevices(VariantBool value);

        VariantBool GetRedirectPOSDevices();

        void SetBitmapVirtualCache32BppSize(int value);

        int GetBitmapVirtualCache32BppSize();
    }

    [GeneratedComInterface]
    [Guid("222C4B5D-45D9-4DF0-A7C6-60CF9089D285")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientAdvancedSettings6 : IMsRdpClientAdvancedSettings5
    {
        void SetRelativeMouseMode(VariantBool value);

        VariantBool GetRelativeMouseMode();

        void SetAuthenticationServiceClass([MarshalAs(UnmanagedType.BStr)] string value);

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetAuthenticationServiceClass();

        void SetPCB([MarshalAs(UnmanagedType.BStr)] string value);

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetPCB();

        void SetHotKeyFocusReleaseLeft(int value);

        int GetHotKeyFocusReleaseLeft();

        void SetHotKeyFocusReleaseRight(int value);

        int GetHotKeyFocusReleaseRight();

        void SetEnableCredSspSupport(VariantBool value);

        VariantBool EnableCredSspSupport();

        uint GetAuthenticationType();

        void SetConnectToAdministerServer(VariantBool value);

        VariantBool GetConnectToAdministerServer();
    }

    [GeneratedComInterface]
    [Guid("26036036-4010-4578-8091-0DB9A1EDF9C3")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientAdvancedSettings7 : IMsRdpClientAdvancedSettings6
    {
        void SetAudioCaptureRedirectionMode(VariantBool value);

        VariantBool GetAudioCaptureRedirectionMode();

        void SetVideoPlaybackMode(uint value);

        uint GetVideoPlaybackMode();

        void SetEnableSuperPan(VariantBool value);

        VariantBool GetEnableSuperPan();

        void SetSuperPanAccelerationFactor(uint value);

        uint GetSuperPanAccelerationFactor();

        void SetNegotiateSecurityLayer(VariantBool value);

        VariantBool GetNegotiateSecurityLayer();

        void SetAudioQualityMode(uint value);

        uint GetAudioQualityMode();

        void SetRedirectDirectX(VariantBool value);

        VariantBool GetRedirectDirectX();

        void SetNetworkConnectionType(uint value);

        uint GetNetworkConnectionType();
    }

    [GeneratedComInterface]
    [Guid("89ACB528-2557-4D16-8625-226A30E97E9A")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientAdvancedSettings8 : IMsRdpClientAdvancedSettings7
    {
        void SetBandwidthDetection(VariantBool value);

        VariantBool GetBandwidthDetection();

        void SetClientProtocolSpec(ClientSpec value);

        ClientSpec GetClientProtocolSpec();
    }

    public enum ClientSpec
    {
        FullMode,
        ThinClientMode,
        SmallCacheMode,
    }

    #endregion

    #region Transport Settings

    [GeneratedComInterface]
    [Guid("720298C0-A099-46F5-9F82-96921BAE4701")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientTransportSettings : IDispatch
    {
        void SetGatewayHostname([MarshalAs(UnmanagedType.BStr)] string value);

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetGatewayHostname();

        void SetGatewayUsageMethod(uint value);

        uint GetGatewayUsageMethod();

        void SetGatewayProfileUsageMethod(uint value);

        uint GetGatewayProfileUsageMethod();

        void SetGatewayCredsSource(uint value);

        uint GetGatewayCredsSource();

        void SetGatewayUserSelectedCredsSource(uint value);

        uint GetGatewayUserSelectedCredsSource();

        int GetGatewayIsSupported();

        uint GatewayDefaultUsageMethod();
    }

    [GeneratedComInterface]
    [Guid("67341688-D606-4C73-A5D2-2E0489009319")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientTransportSettings2 : IMsRdpClientTransportSettings
    {
        void SetGatewayCredSharing(uint value);

        uint GetGatewayCredSharing();

        void SetGatewayPreAuthRequirement(uint value);

        uint GetGatewayPreAuthRequirement();

        void SetGatewayPreAuthServerAddr([MarshalAs(UnmanagedType.BStr)] string value);

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetGatewayPreAuthServerAddr();

        void SetGatewaySupportUrl([MarshalAs(UnmanagedType.BStr)] string value);

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetGatewaySupportUrl();

        void SetGatewayEncryptedOtpCookie([MarshalAs(UnmanagedType.BStr)] string value);

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetGatewayEncryptedOtpCookie();

        void SetGatewayEncryptedOtpCookieSize(uint value);

        uint GetGatewayEncryptedOtpCookieSize();

        void SetGatewayUsername([MarshalAs(UnmanagedType.BStr)] string value);

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetGatewayUsername();

        void SetGatewayDomain([MarshalAs(UnmanagedType.BStr)] string value);

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetGatewayDomain();

        void SetGatewayPassword([MarshalAs(UnmanagedType.BStr)] string value);
    }

    [GeneratedComInterface]
    [Guid("3D5B21AC-748D-41DE-8F30-E15169586BD4")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientTransportSettings3 : IMsRdpClientTransportSettings2
    {
        void SetGatewayCredSourceCookie(uint value);

        uint GetGatewayCredSourceCookie();

        void SetGatewayAuthCookieServerAddr([MarshalAs(UnmanagedType.BStr)] string value);

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetGatewayAuthCookieServerAddr();

        void SetGatewayEncryptedAuthCookie([MarshalAs(UnmanagedType.BStr)] string value);

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetGatewayEncryptedAuthCookie();

        void SetGatewayEncryptedAuthCookieSize(uint value);

        uint GetGatewayEncryptedAuthCookieSize();

        void SetGatewayAuthLoginPage([MarshalAs(UnmanagedType.BStr)] string value);

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetGatewayAuthLoginPage();
    }

    [GeneratedComInterface]
    [Guid("011C3236-4D81-4515-9143-067AB630D299")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientTransportSettings4 : IMsRdpClientTransportSettings3
    {
        uint GetGatewayBrokeringType();
    }

    #endregion

    [GeneratedComInterface]
    [Guid("302D8188-0052-4807-806A-362B628F9AC5")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpExtendedSettings
    {
        void SetProperty([MarshalAs(UnmanagedType.BStr)] string PropertyName, in VARIANT value);
        void GetProperty([MarshalAs(UnmanagedType.BStr)] string PropertyName, out VARIANT value);
    }

    #region Variant

    [Flags]
    public enum VariantType : ushort // typedef unsigned short VARTYPE
    {
        Empty = 0, // VT_EMPTY = 0,
        Null = 1, // VT_NULL = 1,
        Int16 = 2, // VT_I2 = 2,
        Int32 = 3, // VT_I4 = 3,
        Float32 = 4, // VT_R4 = 4,
        Float64 = 5, // VT_R8 = 5,
        Currency = 6, // VT_CY = 6,
        Date = 7, // VT_DATE = 7,
        ComString = 8, // VT_BSTR = 8,
        IDispatch = 9, // VT_DISPATCH = 9,
        ErrorCode = 10, // VT_ERROR = 10,
        Boolean = 11, // VT_BOOL = 11,
        Variant = 12, // VT_VARIANT = 12,
        IUnknown = 13, // VT_UNKNOWN = 13,
        Decimal = 14, // VT_DECIMAL = 14,
        Int8 = 16, // VT_I1 = 16,
        UInt8 = 17, // VT_UI1 = 17,
        UInt16 = 18, // VT_UI2 = 18,
        UInt32 = 19, // VT_UI4 = 19,
        Int64 = 20, // VT_I8 = 20,
        UInt64 = 21, // VT_UI8 = 21,
        // VT_INT = 22,
        // VT_UINT = 23,
        Void = 24, // VT_VOID = 24,
        ResultCode = 25, // VT_HRESULT = 25,
        Pointer = 26, // VT_PTR = 26,
        SafeArray = 27, // VT_SAFEARRAY = 27,
        NativeArray = 28, // VT_CARRAY = 28,
        // VT_USERDEFINED = 29,
        AnsiString = 30, // VT_LPSTR = 30,
        UnicodeString = 31, // VT_LPWSTR = 31,
        // VT_RECORD = 36,
        // VT_INT_PTR = 37,
        // VT_UINT_PTR = 38,
        // VT_FILETIME = 64,
        // VT_BLOB = 65,
        IStream = 66, // VT_STREAM = 66,
        IStorage = 67, // VT_STORAGE = 67,
        // VT_STREAMED_OBJECT = 68,
        // VT_STORED_OBJECT = 69,
        // VT_BLOB_OBJECT = 70,
        // VT_CF = 71,
        ComClassId = 72, // VT_CLSID = 72,
        // VT_VERSIONED_STREAM = 73,
        // VT_BSTR_BLOB = 0xfff,

        VectorModifier = 0x1000, // VT_VECTOR = 0x1000,
        ArrayModifier = 0x2000, // VT_ARRAY = 0x2000,
        ByRefModifier = 0x4000, // VT_BYREF = 0x4000,
        // VT_RESERVED = 0x8000,

        Invalid = 0xFFFF, // VT_ILLEGAL = 0xffff,
        InvalidType = Invalid & TypeMask, // VT_ILLEGALMASKED = 0xfff,
        TypeMask = 0x0FFF, // VT_TYPEMASK = 0xfff,
    }

    public struct VARIANT
    {
        public VariantType Type;
        public ushort Header1;
        public ushort Header2;
        public ushort Header3;
        public nint Content1;
        public nint Content2;
    }

    public struct VariantBool
    {
        public static implicit operator VariantBool(bool value) => new VariantBool(value);
        public static implicit operator bool(VariantBool value) => value.Value != 0;

        public static VariantBool True => new(TrueValue);
        public static VariantBool False => new(FalseValue);

        public const short TrueValue = -1;
        public const short FalseValue = 0;

        public short Value;

        public VariantBool(short value) => Value = value;
        public VariantBool(bool value) => Value = value ? TrueValue : FalseValue;
    }

    #endregion

    public static unsafe partial class IMsRdpExtendedSettingsExtensions
    {
#if NET8_0_OR_GREATER
        [LibraryImport("oleaut32", EntryPoint = "VariantClear", SetLastError = false, StringMarshalling = StringMarshalling.Utf16)]
        private static partial int VariantClear(VARIANT* value);
#else
        [DllImport("oleaut32", EntryPoint = "VariantClear", SetLastError = false, CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern int VariantClear(VARIANT* value);
#endif

        public static object GetProperty(this IMsRdpExtendedSettings settings, string PropertyName)
        {
            VARIANT variant = default; // VT_EMPTY
            try
            {
                settings.GetProperty(PropertyName, out variant);

                switch (variant.Type)
                {
                    case VariantType.Boolean:
                        return variant.Content1 != 0;
                    case VariantType.IUnknown:
#if NET8_0_OR_GREATER
                        return ComInterfaceMarshaller<object>.ConvertToManaged((void*)variant.Content1);
#else
                        return Marshal.GetObjectForIUnknown(variant.Content1);
#endif
                    default:
                        throw new NotSupportedException();
                }
            }
            finally
            {
                int hr;
                if ((hr = VariantClear(&variant)) < 0)
                    Environment.FailFast("Fatal interop failure.", Marshal.GetExceptionForHR(hr));
            }
        }

        public static void SetProperty(this IMsRdpExtendedSettings settings, string PropertyName, bool value)
        {
            VARIANT variant = default;
            variant.Type = VariantType.Boolean;
            variant.Content1 = value ? -1 : 0;
            settings.SetProperty(PropertyName, variant);
        }

        public static void SetProperty(this IMsRdpExtendedSettings settings, string PropertyName, int value)
        {
            VARIANT variant = default;
            variant.Type = VariantType.Int32;
            variant.Content1 = value;
            settings.SetProperty(PropertyName, variant);
        }

        public static void SetProperty(this IMsRdpExtendedSettings settings, string PropertyName, string value)
        {
            VARIANT variant = default;
            variant.Type = VariantType.ComString;
            variant.Content1 = Marshal.StringToBSTR(value);
            try
            {
                settings.SetProperty(PropertyName, variant);
            }
            finally
            {
                Marshal.FreeBSTR(variant.Content1);
            }
        }
    }

    #region Non Scriptable

    [GeneratedComInterface]
    [Guid("C1E6743A-41C1-4A74-832A-0DD06C1C7A0E")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public partial interface IMsTscNonScriptable
    {
        void SetClearTextPassword([MarshalAs(UnmanagedType.BStr)] string value);

        void SetPortablePassword([MarshalAs(UnmanagedType.BStr)] string value);

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetPortablePassword();

        void SetPortableSalt([MarshalAs(UnmanagedType.BStr)] string value);

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetPortableSalt();

        void SetBinaryPassword([MarshalAs(UnmanagedType.BStr)] string value);

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetBinaryPassword();

        void SetBinarySalt([MarshalAs(UnmanagedType.BStr)] string value);

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetBinarySalt();

        void ResetPassword();
    }

    [GeneratedComInterface]
    [Guid("2F079C4C-87B2-4AFD-97AB-20CDB43038AE")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public partial interface IMsRdpClientNonScriptable : IMsTscNonScriptable
    {
        void NotifyRedirectDeviceChange([ComAliasName("MSTSCLib.UINT_PTR")] ulong wParam, [ComAliasName("MSTSCLib.LONG_PTR")] long lParam);

        void SendKeys(int numKeys, ref VariantBool pbArrayKeyUp, ref int plKeyData);
    }

    [GeneratedComInterface]
    [Guid("17A5E535-4072-4FA4-AF32-C8D0D47345E9")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public partial interface IMsRdpClientNonScriptable2 : IMsRdpClientNonScriptable
    {
        void SetUIParentWindowHandle(nint value);

        nint GetUIParentWindowHandle();
    }

    [GeneratedComInterface]
    [Guid("B3378D90-0728-45C7-8ED7-B6159FB92219")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public partial interface IMsRdpClientNonScriptable3 : IMsRdpClientNonScriptable2
    {
        void SetShowRedirectionWarningDialog(VariantBool value);

        VariantBool GetShowRedirectionWarningDialog();

        void SetPromptForCredentials(VariantBool value);

        VariantBool GetPromptForCredentials();

        void SetNegotiateSecurityLayer(VariantBool value);

        VariantBool GetNegotiateSecurityLayer();

        void SetEnableCredSspSupport(VariantBool value);

        VariantBool GetEnableCredSspSupport();

        void SetRedirectDynamicDrives(VariantBool value);

        VariantBool GetRedirectDynamicDrives();

        void SetRedirectDynamicDevices(VariantBool value);

        VariantBool GetRedirectDynamicDevices();

        [return: MarshalAs(UnmanagedType.Interface)] object /*IMsRdpDeviceCollection*/ GetDeviceCollection();

        [return: MarshalAs(UnmanagedType.Interface)] object /*IMsRdpDriveCollection*/ GetDriveCollection();

        void SetWarnAboutSendingCredentials(VariantBool value);

        VariantBool GetWarnAboutSendingCredentials();

        void SetWarnAboutClipboardRedirection(VariantBool value);

        VariantBool GetWarnAboutClipboardRedirection();

        void SetConnectionBarText([MarshalAs(UnmanagedType.BStr)] string value);

        [return: MarshalAs(UnmanagedType.BStr)] string GetConnectionBarText();
    }

    [GeneratedComInterface]
    [Guid("F50FA8AA-1C7D-4F59-B15C-A90CACAE1FCB")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public partial interface IMsRdpClientNonScriptable4 : IMsRdpClientNonScriptable3
    {
        void SetRedirectionWarningType(int /*RedirectionWarningType*/ value);

        int /*RedirectionWarningType*/ GetRedirectionWarningType();

        void SetMarkRdpSettingsSecure(VariantBool value);

        VariantBool GetMarkRdpSettingsSecure();

        void SetPublisherCertificateChain(nint value);

        nint GetPublisherCertificateChain();

        void SetWarnAboutPrinterRedirection(VariantBool value);

        VariantBool GetWarnAboutPrinterRedirection();

        void SetAllowCredentialSaving(VariantBool value);

        VariantBool GetAllowCredentialSaving();

        void SetPromptForCredsOnClient(VariantBool value);

        VariantBool GetPromptForCredsOnClient();

        void SetLaunchedViaClientShellInterface(VariantBool value);

        VariantBool GetLaunchedViaClientShellInterface();

        void SetTrustedZoneSite(VariantBool value);

        VariantBool GetTrustedZoneSite();
    }

    [GeneratedComInterface]
    [Guid("4F6996D5-D7B1-412C-B0FF-063718566907")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public partial interface IMsRdpClientNonScriptable5 : IMsRdpClientNonScriptable4
    {
        void SetUseMultimon(VariantBool value);

        VariantBool GetUseMultimon();

        uint GetRemoteMonitorCount();

        void GetRemoteMonitorsBoundingBox(out int pLeft, out int pTop, out int pRight, out int pBottom);

        VariantBool GetRemoteMonitorLayoutMatchesLocal();

        void SetDisableConnectionBar(VariantBool value);

        void SetDisableRemoteAppCapsCheck(VariantBool value);

        VariantBool GetDisableRemoteAppCapsCheck();

        void SetWarnAboutDirectXRedirection(VariantBool value);

        VariantBool GetWarnAboutDirectXRedirection();

        void SetAllowPromptingForCredentials(VariantBool value);

        VariantBool GetAllowPromptingForCredentials();
    }

    [GeneratedComInterface]
    [Guid("05293249-B28B-4BD8-BE64-1B2F496B910E")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public partial interface IMsRdpClientNonScriptable6 : IMsRdpClientNonScriptable5
    {
        void SendLocation2D(double latitude, double longitude);

        void SendLocation3D(double latitude, double longitude, int altitude);
    }

    [GeneratedComInterface]
    [Guid("71B4A60A-FE21-46D8-A39B-8E32BA0C5ECC")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public partial interface IMsRdpClientNonScriptable7 : IMsRdpClientNonScriptable6
    {
        [return: MarshalAs(UnmanagedType.Interface)]
        object /*IMsRdpCameraRedirConfigCollection*/ GetCameraRedirConfigCollection();

        void DisableDpiCursorScalingForProcess();

        [return: MarshalAs(UnmanagedType.Interface)]
        object /*IMsRdpClipboard*/ GetClipboard();
    }

    #endregion

    [GeneratedComInterface]
    [Guid("FDD029F9-9574-4DEF-8529-64B521CCCAA4")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public partial interface IMsRdpPreferredRedirectionInfo
    {
        void SetUseRedirectionServerName(VariantBool value);
        VariantBool GetUseRedirectionServerName();
    }

    [ComImport]
    [Guid("336D5562-EFA8-482E-8CB3-C5C0FC7A7DB6")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public unsafe partial interface IMsTscAxEvents // : IDispatch
    {
        [DispId(1)]
        void OnConnecting();

        [DispId(2)]
        void OnConnected();

        [DispId(3)]
        void OnLoginComplete();

        [DispId(4)]
        void OnDisconnected(int discReason);

        [DispId(5)]
        void OnEnterFullScreenMode();

        [DispId(6)]
        void OnLeaveFullScreenMode();

        [DispId(7)]
        void OnChannelReceivedData([MarshalAs(UnmanagedType.BStr)] string chanName, [MarshalAs(UnmanagedType.BStr)] string data);

        [DispId(8)]
        void OnRequestGoFullScreen();

        [DispId(9)]
        void OnRequestLeaveFullScreen();

        [DispId(10)]
        void OnFatalError(int errorCode);

        [DispId(11)]
        void OnWarning(int warningCode);

        [DispId(12)]
        void OnRemoteDesktopSizeChange(int width, int height);

        [DispId(13)]
        void OnIdleTimeoutNotification();

        [DispId(14)]
        void OnRequestContainerMinimize();

        [DispId(15)]
        void OnConfirmClose(out VariantBool pfAllowClose);

        [DispId(16)]
        void OnReceivedTSPublicKey([MarshalAs(UnmanagedType.BStr)] string publicKey, /*out*/ref VariantBool pfContinueLogon);

        [DispId(17)]
        void OnAutoReconnecting(int disconnectReason, int attemptCount, [ComAliasName("MSTSCLib.AutoReconnectContinueState")] /*out*/ref AutoReconnectContinueState pArcContinueStatus);

        [DispId(18)]
        void OnAuthenticationWarningDisplayed();

        [DispId(19)]
        void OnAuthenticationWarningDismissed();

        [DispId(20)]
        void OnRemoteProgramResult([MarshalAs(UnmanagedType.BStr)] string bstrRemoteProgram, [ComAliasName("MSTSCLib.RemoteProgramResult")] RemoteProgramResult lError, VariantBool vbIsExecutable);

        [DispId(21)]
        void OnRemoteProgramDisplayed(VariantBool vbDisplayed, uint uDisplayInformation);

        [DispId(29)]
        void OnRemoteWindowDisplayed(VariantBool vbDisplayed, [ComAliasName("MSTSCLib.wireHWND")] in RemotableHandle hwnd, [ComAliasName("MSTSCLib.RemoteWindowDisplayedAttribute")] RemoteWindowDisplayedAttribute windowAttribute);

        [DispId(22)]
        void OnLogonError(int lError);

        [DispId(23)]
        void OnFocusReleased(int iDirection);

        [DispId(24)]
        void OnUserNameAcquired([MarshalAs(UnmanagedType.BStr)] string bstrUserName);

        [DispId(26)]
        void OnMouseInputModeChanged(VariantBool fMouseModeRelative);

        [DispId(28)]
        void OnServiceMessageReceived([MarshalAs(UnmanagedType.BStr)] string serviceMessage);

        [DispId(30)]
        void OnConnectionBarPullDown();

        [DispId(32)]
        void OnNetworkStatusChanged(uint qualityLevel, int bandwidth, int rtt);

        [DispId(35)]
        void OnDevicesButtonPressed();

        [DispId(33)]
        void OnAutoReconnected();

        [DispId(34)]
        void OnAutoReconnecting2(int disconnectReason, VariantBool networkAvailable, int attemptCount, int maxAttemptCount);
    }

    public enum ControlReconnectStatus
    {
        controlReconnectStarted,
        controlReconnectBlocked,
    }

    public enum AutoReconnectContinueState
    {
        autoReconnectContinueAutomatic,
        autoReconnectContinueStop,
        autoReconnectContinueManual,
    }

    public enum RemoteProgramResult
    {
        remoteAppResultOk,
        remoteAppResultLocked,
        remoteAppResultProtocolError,
        remoteAppResultNotInWhitelist,
        remoteAppResultNetworkPathDenied,
        remoteAppResultFileNotFound,
        remoteAppResultFailure,
        remoteAppResultHookNotLoaded,
    }

    public enum RemoteWindowDisplayedAttribute
    {
        remoteAppWindowNone,
        remoteAppWindowDisplayed,
        remoteAppShellIconDisplayed,
    }

    public struct RemotableHandle
    {
        public int ContextFlag;
        public int HandleValue;
    }
}
