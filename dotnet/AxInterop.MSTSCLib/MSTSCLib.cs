using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace MSTSCLib
{
    #region IDispatch

    [GeneratedComInterface]
    [Guid("00000000-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public partial interface IUnknown
    {
        // IUnknown methods are provided by the infrastructure
    }

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
            nint pDispParams, // /* [annotation][out][in] */ _In_ DISPPARAMS *pDispParams
            nint pVarResult, // /* [annotation][out] */ _Out_opt_ VARIANT *pVarResult
            nint pExcepInfo, // /* [annotation][out] */ _Out_opt_ EXCEPINFO *pExcepInfo
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

    #endregion

    #region IMsRdpClient

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

    #region Advanced Settings

    [GeneratedComInterface]
    [Guid("89ACB528-2557-4D16-8625-226A30E97E9A")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientAdvancedSettings8 : IDispatch // : IMsRdpClientAdvancedSettings7
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

        int GTetDisableRdpdr();

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

        void SetRDPPort();
        void GetRDPPort();
        //[DispId(108)]
        //int RDPPort
        //{

        //    [DispId(108)]
        //    get;

        //    [DispId(108)]
        //    [param: In]
        //    set;
        //}

        void SetEnableMouse();
        void GetEnableMouse();
        //[DispId(109)]
        //int EnableMouse
        //{

        //    [DispId(109)]
        //    get;

        //    [DispId(109)]
        //    [param: In]
        //    set;
        //}

        void SetDisableCtrlAltDel();
        void GetDisableCtrlAltDel();
        //[DispId(110)]
        //int DisableCtrlAltDel
        //{

        //    [DispId(110)]
        //    get;

        //    [DispId(110)]
        //    [param: In]
        //    set;
        //}

        void SetEnableWindowsKey();
        void GetEnableWindowsKey();
        //[DispId(111)]
        //int EnableWindowsKey
        //{

        //    [DispId(111)]
        //    get;

        //    [DispId(111)]
        //    [param: In]
        //    set;
        //}

        void SetDoubleClickDetect();
        void GetDoubleClickDetect();
        //[DispId(112)]
        //int DoubleClickDetect
        //{

        //    [DispId(112)]
        //    get;

        //    [DispId(112)]
        //    [param: In]
        //    set;
        //}

        void SetMaximizeShell();
        void GetMaximizeShell();
        //[DispId(113)]
        //int MaximizeShell
        //{

        //    [DispId(113)]
        //    get;

        //    [DispId(113)]
        //    [param: In]
        //    set;
        //}

        void SetHotKeyFullScreen();
        void GetHotKeyFullScreen();
        //[DispId(114)]
        //int HotKeyFullScreen
        //{

        //    [DispId(114)]
        //    get;

        //    [DispId(114)]
        //    [param: In]
        //    set;
        //}

        void SetHotKeyCtrlEsc();
        void GetHotKeyCtrlEsc();
        //[DispId(115)]
        //int HotKeyCtrlEsc
        //{

        //    [DispId(115)]
        //    get;

        //    [DispId(115)]
        //    [param: In]
        //    set;
        //}

        void SetHotKeyAltEsc();
        void GetHotKeyAltEsc();
        //[DispId(116)]
        //int HotKeyAltEsc
        //{

        //    [DispId(116)]
        //    get;

        //    [DispId(116)]
        //    [param: In]
        //    set;
        //}

        void SetHotKeyAltTab();
        void GetHotKeyAltTab();
        //[DispId(117)]
        //int HotKeyAltTab
        //{

        //    [DispId(117)]
        //    get;

        //    [DispId(117)]
        //    [param: In]
        //    set;
        //}

        void SetHotKeyAltShiftTab();
        void GetHotKeyAltShiftTab();
        //[DispId(118)]
        //int HotKeyAltShiftTab
        //{

        //    [DispId(118)]
        //    get;

        //    [DispId(118)]
        //    [param: In]
        //    set;
        //}

        void SetHotKeyAltSpace();
        void GetHotKeyAltSpace();
        //[DispId(119)]
        //int HotKeyAltSpace
        //{

        //    [DispId(119)]
        //    get;

        //    [DispId(119)]
        //    [param: In]
        //    set;
        //}

        void SetHotKeyCtrlAltDel();
        void GetHotKeyCtrlAltDel();
        //[DispId(120)]
        //int HotKeyCtrlAltDel
        //{

        //    [DispId(120)]
        //    get;

        //    [DispId(120)]
        //    [param: In]
        //    set;
        //}

        void SetorderDrawThreshold();
        void GetorderDrawThreshold();
        //[DispId(123)]
        //int orderDrawThreshold
        //{

        //    [DispId(123)]
        //    get;

        //    [DispId(123)]
        //    [param: In]
        //    set;
        //}

        void SetBitmapCacheSize();
        void GetBitmapCacheSize();
        //[DispId(124)]
        //int BitmapCacheSize
        //{

        //    [DispId(124)]
        //    get;

        //    [DispId(124)]
        //    [param: In]
        //    set;
        //}

        void SetBitmapVirtualCacheSize();
        void GetBitmapVirtualCacheSize();
        //[DispId(125)]
        //int BitmapVirtualCacheSize
        //{

        //    [DispId(125)]
        //    get;

        //    [DispId(125)]
        //    [param: In]
        //    set;
        //}

        void SetScaleBitmapCachesByBPP();
        void GetScaleBitmapCachesByBPP();
        //[DispId(175)]
        //int ScaleBitmapCachesByBPP
        //{

        //    [DispId(175)]
        //    get;

        //    [DispId(175)]
        //    [param: In]
        //    set;
        //}

        void SetNumBitmapCaches();
        void GetNumBitmapCaches();
        //[DispId(126)]
        //int NumBitmapCaches
        //{

        //    [DispId(126)]
        //    get;

        //    [DispId(126)]
        //    [param: In]
        //    set;
        //}

        void SetCachePersistenceActive();
        void GetCachePersistenceActive();
        //[DispId(127)]
        //int CachePersistenceActive
        //{

        //    [DispId(127)]
        //    get;

        //    [DispId(127)]
        //    [param: In]
        //    set;
        //}

        void SetPersistCacheDirectory();
        //[DispId(138)]
        //string PersistCacheDirectory
        //{

        //    [DispId(138)]
        //    [param: In]
        //    [param: MarshalAs(UnmanagedType.BStr)]
        //    set;
        //}

        void SetBrushSupportLevel();
        void GetBrushSupportLevel();
        //[DispId(156)]
        //int brushSupportLevel
        //{

        //    [DispId(156)]
        //    get;

        //    [DispId(156)]
        //    [param: In]
        //    set;
        //}

        void SetMinInputSendInterval();
        void GetMinInputSendInterval();
        //[DispId(157)]
        //int minInputSendInterval
        //{

        //    [DispId(157)]
        //    get;

        //    [DispId(157)]
        //    [param: In]
        //    set;
        //}

        void SetInputEventsAtOnce();
        void GetInputEventsAtOnce();
        //[DispId(158)]
        //int InputEventsAtOnce
        //{

        //    [DispId(158)]
        //    get;

        //    [DispId(158)]
        //    [param: In]
        //    set;
        //}

        void GetMaxEventCount();
        void SetMaxEventCount();
        //[DispId(159)]
        //int maxEventCount
        //{

        //    [DispId(159)]
        //    get;

        //    [DispId(159)]
        //    [param: In]
        //    set;
        //}

        void SetKeepAliveInterval();
        void GetKeepAliveInterval();
        //[DispId(160)]
        //int KeepAliveInterval
        //{

        //    [DispId(160)]
        //    get;

        //    [DispId(160)]
        //    [param: In]
        //    set;
        //}

        void SetShutdownTimeout();
        void GetShutdownTimeout();
        //[DispId(163)]
        //int ShutdownTimeout
        //{

        //    [DispId(163)]
        //    get;

        //    [DispId(163)]
        //    [param: In]
        //    set;
        //}

        void SetOverallConnectionTimeout();
        void GetOverallConnectionTimeout();
        //[DispId(164)]
        //int OverallConnectionTimeout
        //{

        //    [DispId(164)]
        //    get;

        //    [DispId(164)]
        //    [param: In]
        //    set;
        //}

        void SetSingleConnectionTimeout();
        void GetSingleConnectionTimeout();
        //[DispId(165)]
        //int SingleConnectionTimeout
        //{

        //    [DispId(165)]
        //    get;

        //    [DispId(165)]
        //    [param: In]
        //    set;
        //}

        void SetKeyboardType();
        void GetKeyboardType();
        //[DispId(166)]
        //int KeyboardType
        //{

        //    [DispId(166)]
        //    get;

        //    [DispId(166)]
        //    [param: In]
        //    set;
        //}

        void SetKeyboardSubType();
        void GetKeyboardSubType();
        //[DispId(167)]
        //int KeyboardSubType
        //{

        //    [DispId(167)]
        //    get;

        //    [DispId(167)]
        //    [param: In]
        //    set;
        //}

        void SetKeyboardFunctionKey();
        void GetKeyboardFunctionKey();
        //[DispId(168)]
        //int KeyboardFunctionKey
        //{

        //    [DispId(168)]
        //    get;

        //    [DispId(168)]
        //    [param: In]
        //    set;
        //}

        void SetWinceFixedPalette();
        void GetWinceFixedPalette();
        //[DispId(169)]
        //int WinceFixedPalette
        //{

        //    [DispId(169)]
        //    get;

        //    [DispId(169)]
        //    [param: In]
        //    set;
        //}

        void SetConnectToServerConsole();
        void GetConnectToServerConsole();
        //[DispId(178)]
        //bool ConnectToServerConsole
        //{

        //    [DispId(178)]
        //    get;

        //    [DispId(178)]
        //    [param: In]
        //    set;
        //}

        void SetBitmapPersistence();
        void GetBitmapPersistence();
        //[DispId(182)]
        //int BitmapPersistence
        //{

        //    [DispId(182)]
        //    get;

        //    [DispId(182)]
        //    [param: In]
        //    set;
        //}

        void SetMinutesToIdleTimeout();
        void GetMinutesToIdleTimeout();
        //[DispId(183)]
        //int MinutesToIdleTimeout
        //{

        //    [DispId(183)]
        //    get;

        //    [DispId(183)]
        //    [param: In]
        //    set;
        //}

        void SetSmartSizing();
        void GetSmartSizing();
        //[DispId(184)]
        //bool SmartSizing
        //{

        //    [DispId(184)]
        //    get;

        //    [DispId(184)]
        //    [param: In]
        //    set;
        //}

        void SetRdpdrLocalPrintingDocName();
        void GetRdpdrLocalPrintingDocName();
        //[DispId(185)]
        //string RdpdrLocalPrintingDocName
        //{

        //    [DispId(185)]
        //    [return: MarshalAs(UnmanagedType.BStr)]
        //    get;

        //    [DispId(185)]
        //    [param: In]
        //    [param: MarshalAs(UnmanagedType.BStr)]
        //    set;
        //}

        void SetRdpdrClipCleanTempDirString();
        void GetRdpdrClipCleanTempDirString();
        //[DispId(201)]
        //string RdpdrClipCleanTempDirString
        //{

        //    [DispId(201)]
        //    [return: MarshalAs(UnmanagedType.BStr)]
        //    get;

        //    [DispId(201)]
        //    [param: In]
        //    [param: MarshalAs(UnmanagedType.BStr)]
        //    set;
        //}

        void SetRdpdrClipPasteInfoString();
        void GetRdpdrClipPasteInfoString();
        //[DispId(202)]
        //string RdpdrClipPasteInfoString
        //{

        //    [DispId(202)]
        //    [return: MarshalAs(UnmanagedType.BStr)]
        //    get;

        //    [DispId(202)]
        //    [param: In]
        //    [param: MarshalAs(UnmanagedType.BStr)]
        //    set;
        //}

        void SetClearTextPassword();
        //[DispId(186)]
        //string ClearTextPassword
        //{

        //    [DispId(186)]
        //    [param: In]
        //    [param: MarshalAs(UnmanagedType.BStr)]
        //    set;
        //}

        void SetDisplayConnectionBar();
        void GetDisplayConnectionBar();
        //[DispId(187)]
        //bool DisplayConnectionBar
        //{

        //    [DispId(187)]
        //    get;

        //    [DispId(187)]
        //    [param: In]
        //    set;
        //}

        void SetPinConnectionBar();
        void GetPinConnectionBar();
        //[DispId(188)]
        //bool PinConnectionBar
        //{

        //    [DispId(188)]
        //    get;

        //    [DispId(188)]
        //    [param: In]
        //    set;
        //}

        void SetGrabFocusOnConnect();
        void GetGrabFocusOnConnect();
        //[DispId(189)]
        //bool GrabFocusOnConnect
        //{

        //    [DispId(189)]
        //    get;

        //    [DispId(189)]
        //    [param: In]
        //    set;
        //}

        void SetLoadBalanceInfo();
        void GetLoadBalanceInfo();
        //[DispId(190)]
        //string LoadBalanceInfo
        //{

        //    [DispId(190)]
        //    [return: MarshalAs(UnmanagedType.BStr)]
        //    get;

        //    [DispId(190)]
        //    [param: In]
        //    [param: MarshalAs(UnmanagedType.BStr)]
        //    set;
        //}

        void SetRedirectDrives();
        void GetRedirectDrives();
        //[DispId(191)]
        //bool RedirectDrives
        //{

        //    [DispId(191)]
        //    get;

        //    [DispId(191)]
        //    [param: In]
        //    set;
        //}

        void SetRedirectPrinters();
        void GetRedirectPrinters();
        //[DispId(192)]
        //bool RedirectPrinters
        //{

        //    [DispId(192)]
        //    get;

        //    [DispId(192)]
        //    [param: In]
        //    set;
        //}

        void SetRedirectPorts();
        void GetRedirectPorts();
        //[DispId(193)]
        //bool RedirectPorts
        //{

        //    [DispId(193)]
        //    get;

        //    [DispId(193)]
        //    [param: In]
        //    set;
        //}

        void SetRedirectSmartCards();
        void GetRedirectSmartCards();
        //[DispId(194)]
        //bool RedirectSmartCards
        //{

        //    [DispId(194)]
        //    get;

        //    [DispId(194)]
        //    [param: In]
        //    set;
        //}

        void SetBitmapVirtualCache16BppSize();
        void GetBitmapVirtualCache16BppSize();
        //[DispId(195)]
        //int BitmapVirtualCache16BppSize
        //{

        //    [DispId(195)]
        //    get;

        //    [DispId(195)]
        //    [param: In]
        //    set;
        //}

        void SetBitmapVirtualCache24BppSize();
        void GetBitmapVirtualCache24BppSize();
        //[DispId(196)]
        //int BitmapVirtualCache24BppSize
        //{

        //    [DispId(196)]
        //    get;

        //    [DispId(196)]
        //    [param: In]
        //    set;
        //}

        void SetPerformanceFlags();
        void GetPerformanceFlags();
        //[DispId(200)]
        //int PerformanceFlags
        //{

        //    [DispId(200)]
        //    get;

        //    [DispId(200)]
        //    [param: In]
        //    set;
        //}

        void SetConnectWithEndpoint();
        //[DispId(203)]
        //IntPtr ConnectWithEndpoint
        //{

        //    [DispId(203)]
        //    [param: In]
        //    [param: MarshalAs(UnmanagedType.Struct)]
        //    set;
        //}

        void SetNotifyTSPublicKey();
        void GetNotifyTSPublicKey();
        //[DispId(204)]
        //bool NotifyTSPublicKey
        //{

        //    [DispId(204)]
        //    get;

        //    [DispId(204)]
        //    [param: In]
        //    set;
        //}

        void GetCanAutoReconnect();
        //[DispId(205)]
        //bool CanAutoReconnect
        //{

        //    [DispId(205)]
        //    get;
        //}

        void SetEnableAutoReconnect();
        void GetEnableAutoReconnect();
        //[DispId(206)]
        //bool EnableAutoReconnect
        //{

        //    [DispId(206)]
        //    get;

        //    [DispId(206)]
        //    [param: In]
        //    set;
        //}

        void SetMaxReconnectAttempts();
        void GetMaxReconnectAttempts();
        //[DispId(207)]
        //int MaxReconnectAttempts
        //{

        //    [DispId(207)]
        //    get;

        //    [DispId(207)]
        //    [param: In]
        //    set;
        //}

        void SetConnectionBarShowMinimizeButton();
        void GetConnectionBarShowMinimizeButton();
        //[DispId(210)]
        //bool ConnectionBarShowMinimizeButton
        //{

        //    [DispId(210)]
        //    get;

        //    [DispId(210)]
        //    [param: In]
        //    set;
        //}

        void SetConnectionBarShowRestoreButton();
        void GetConnectionBarShowRestoreButton();
        //[DispId(211)]
        //bool ConnectionBarShowRestoreButton
        //{

        //    [DispId(211)]
        //    get;

        //    [DispId(211)]
        //    [param: In]
        //    set;
        //}

        void SetAuthenticationLevel();
        void GetAuthenticationLevel();
        //[DispId(212)]
        //uint AuthenticationLevel
        //{

        //    [DispId(212)]
        //    [param: In]
        //    set;

        //    [DispId(212)]
        //    get;
        //}

        void SetRedirectClipboard();
        void GetRedirectClipboard();
        //[DispId(213)]
        //bool RedirectClipboard
        //{

        //    [DispId(213)]
        //    get;

        //    [DispId(213)]
        //    [param: In]
        //    set;
        //}

        void SetAudioRedirectionMode();
        void GetAudioRedirectionMode();
        //[DispId(215)]
        //uint AudioRedirectionMode
        //{

        //    [DispId(215)]
        //    [param: In]
        //    set;

        //    [DispId(215)]
        //    get;
        //}

        void SetConnectionBarShowPinButton();
        void GetConnectionBarShowPinButton();
        //[DispId(216)]
        //bool ConnectionBarShowPinButton
        //{

        //    [DispId(216)]
        //    get;

        //    [DispId(216)]
        //    [param: In]
        //    set;
        //}

        void SetPublicMode();
        void GetPublicMode();
        //[DispId(217)]
        //bool PublicMode
        //{

        //    [DispId(217)]
        //    get;

        //    [DispId(217)]
        //    [param: In]
        //    set;
        //}

        void SetRedirectDevices();
        void GetRedirectDevices();
        //[DispId(218)]
        //bool RedirectDevices
        //{

        //    [DispId(218)]
        //    get;

        //    [DispId(218)]
        //    [param: In]
        //    set;
        //}

        void SetRedirectPOSDevices();
        void GetRedirectPOSDevices();
        //[DispId(219)]
        //bool RedirectPOSDevices
        //{

        //    [DispId(219)]
        //    get;

        //    [DispId(219)]
        //    [param: In]
        //    set;
        //}

        void SetBitmapVirtualCache32BppSize();
        void GetBitmapVirtualCache32BppSize();
        //[DispId(220)]
        //int BitmapVirtualCache32BppSize
        //{

        //    [DispId(220)]
        //    get;

        //    [DispId(220)]
        //    [param: In]
        //    set;
        //}

        void SetRelativeMouseMode();
        void GetRelativeMouseMode();
        //[DispId(221)]
        //bool RelativeMouseMode
        //{

        //    [DispId(221)]
        //    get;

        //    [DispId(221)]
        //    [param: In]
        //    set;
        //}

        void SetAuthenticationServiceClass();
        void GetAuthenticationServiceClass();
        //[DispId(222)]
        //string AuthenticationServiceClass
        //{

        //    [DispId(222)]
        //    [return: MarshalAs(UnmanagedType.BStr)]
        //    get;

        //    [DispId(222)]
        //    [param: In]
        //    [param: MarshalAs(UnmanagedType.BStr)]
        //    set;
        //}

        void SetPCB();
        void GetPCB();
        //[DispId(223)]
        //string PCB
        //{

        //    [DispId(223)]
        //    [return: MarshalAs(UnmanagedType.BStr)]
        //    get;

        //    [DispId(223)]
        //    [param: In]
        //    [param: MarshalAs(UnmanagedType.BStr)]
        //    set;
        //}

        void SetHotKeyFocusReleaseLeft();
        void GetHotKeyFocusReleaseLeft();
        //[DispId(224)]
        //int HotKeyFocusReleaseLeft
        //{

        //    [DispId(224)]
        //    get;

        //    [DispId(224)]
        //    [param: In]
        //    set;
        //}

        void SetHotKeyFocusReleaseRight();
        void GetHotKeyFocusReleaseRight();
        //[DispId(225)]
        //int HotKeyFocusReleaseRight
        //{

        //    [DispId(225)]
        //    get;

        //    [DispId(225)]
        //    [param: In]
        //    set;
        //}

        [DispId(17)]
        void SetEnableCredSspSupport(VariantBool value);

        [DispId(17)]
        VariantBool EnableCredSspSupport();

        //[DispId(226)]
        //uint AuthenticationType
        //{

        //    [DispId(226)]
        //    get;
        //}

        //[DispId(227)]
        //bool ConnectToAdministerServer
        //{

        //    [DispId(227)]
        //    get;

        //    [DispId(227)]
        //    [param: In]
        //    set;
        //}

        //[DispId(228)]
        //bool AudioCaptureRedirectionMode
        //{

        //    [DispId(228)]
        //    get;

        //    [DispId(228)]
        //    [param: In]
        //    set;
        //}

        //[DispId(229)]
        //uint VideoPlaybackMode
        //{

        //    [DispId(229)]
        //    get;

        //    [DispId(229)]
        //    [param: In]
        //    set;
        //}

        //[DispId(230)]
        //bool EnableSuperPan
        //{

        //    [DispId(230)]
        //    get;

        //    [DispId(230)]
        //    [param: In]
        //    set;
        //}

        //[DispId(231)]
        //uint SuperPanAccelerationFactor
        //{

        //    [DispId(231)]
        //    get;

        //    [DispId(231)]
        //    [param: In]
        //    set;
        //}

        //[DispId(232)]
        //bool NegotiateSecurityLayer
        //{

        //    [DispId(232)]
        //    get;

        //    [DispId(232)]
        //    [param: In]
        //    set;
        //}

        //[DispId(233)]
        //uint AudioQualityMode
        //{

        //    [DispId(233)]
        //    get;

        //    [DispId(233)]
        //    [param: In]
        //    set;
        //}

        //[DispId(234)]
        //bool RedirectDirectX
        //{

        //    [DispId(234)]
        //    get;

        //    [DispId(234)]
        //    [param: In]
        //    set;
        //}

        //[DispId(235)]
        //uint NetworkConnectionType
        //{

        //    [DispId(235)]
        //    get;

        //    [DispId(235)]
        //    [param: In]
        //    set;
        //}

        //[DispId(236)]
        //bool BandwidthDetection
        //{

        //    [DispId(236)]
        //    get;

        //    [DispId(236)]
        //    [param: In]
        //    set;
        //}

        //void ClientProtocolSpec();
        ////[DispId(237)]
        ////[ComAliasName("MSTSCLib.ClientSpec")]
        ////ClientSpec ClientProtocolSpec
        ////{
        ////    
        ////    [DispId(237)]
        ////    [return: ComAliasName("MSTSCLib.ClientSpec")]
        ////    get;
        ////    
        ////    [DispId(237)]
        ////    [param: In]
        ////    [param: ComAliasName("MSTSCLib.ClientSpec")]
        ////    set;
        ////}
    }

    #endregion

    #region IMsRdpClientTransportSettings

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
        //[DispId(222)]
        //uint GatewayCredSharing
        //{

        //    [DispId(222)]
        //    get;

        //    [DispId(222)]
        //    [param: In]
        //    set;
        //}

        //[DispId(217)]
        //uint GatewayPreAuthRequirement
        //{

        //    [DispId(217)]
        //    get;

        //    [DispId(217)]
        //    [param: In]
        //    set;
        //}

        //[DispId(218)]
        //string GatewayPreAuthServerAddr
        //{

        //    [DispId(218)]
        //    [return: MarshalAs(UnmanagedType.BStr)]
        //    get;

        //    [DispId(218)]
        //    [param: In]
        //    [param: MarshalAs(UnmanagedType.BStr)]
        //    set;
        //}

        //[DispId(219)]
        //string GatewaySupportUrl
        //{

        //    [DispId(219)]
        //    [return: MarshalAs(UnmanagedType.BStr)]
        //    get;

        //    [DispId(219)]
        //    [param: In]
        //    [param: MarshalAs(UnmanagedType.BStr)]
        //    set;
        //}

        //[DispId(220)]
        //string GatewayEncryptedOtpCookie
        //{

        //    [DispId(220)]
        //    [return: MarshalAs(UnmanagedType.BStr)]
        //    get;

        //    [DispId(220)]
        //    [param: In]
        //    [param: MarshalAs(UnmanagedType.BStr)]
        //    set;
        //}

        //[DispId(221)]
        //uint GatewayEncryptedOtpCookieSize
        //{

        //    [DispId(221)]
        //    get;

        //    [DispId(221)]
        //    [param: In]
        //    set;
        //}

        //[DispId(223)]
        //string GatewayUsername
        //{

        //    [DispId(223)]
        //    [return: MarshalAs(UnmanagedType.BStr)]
        //    get;

        //    [DispId(223)]
        //    [param: In]
        //    [param: MarshalAs(UnmanagedType.BStr)]
        //    set;
        //}

        //[DispId(224)]
        //string GatewayDomain
        //{

        //    [DispId(224)]
        //    [return: MarshalAs(UnmanagedType.BStr)]
        //    get;

        //    [DispId(224)]
        //    [param: In]
        //    [param: MarshalAs(UnmanagedType.BStr)]
        //    set;
        //}

        //[DispId(225)]
        //string GatewayPassword
        //{

        //    [DispId(225)]
        //    [param: In]
        //    [param: MarshalAs(UnmanagedType.BStr)]
        //    set;
        //}
    }

    [GeneratedComInterface]
    [Guid("3D5B21AC-748D-41DE-8F30-E15169586BD4")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientTransportSettings3 : IMsRdpClientTransportSettings2
    {
        //[DispId(226)]
        //uint GatewayCredSourceCookie
        //{

        //    [DispId(226)]
        //    get;

        //    [DispId(226)]
        //    [param: In]
        //    set;
        //}

        //[DispId(227)]
        //string GatewayAuthCookieServerAddr
        //{

        //    [DispId(227)]
        //    [return: MarshalAs(UnmanagedType.BStr)]
        //    get;

        //    [DispId(227)]
        //    [param: In]
        //    [param: MarshalAs(UnmanagedType.BStr)]
        //    set;
        //}

        //[DispId(228)]
        //string GatewayEncryptedAuthCookie
        //{

        //    [DispId(228)]
        //    [return: MarshalAs(UnmanagedType.BStr)]
        //    get;

        //    [DispId(228)]
        //    [param: In]
        //    [param: MarshalAs(UnmanagedType.BStr)]
        //    set;
        //}

        //[DispId(229)]
        //uint GatewayEncryptedAuthCookieSize
        //{

        //    [DispId(229)]
        //    get;

        //    [DispId(229)]
        //    [param: In]
        //    set;
        //}

        //[DispId(230)]
        //string GatewayAuthLoginPage
        //{

        //    [DispId(230)]
        //    [return: MarshalAs(UnmanagedType.BStr)]
        //    get;

        //    [DispId(230)]
        //    [param: In]
        //    [param: MarshalAs(UnmanagedType.BStr)]
        //    set;
        //}
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
                        return ComInterfaceMarshaller<IUnknown>.ConvertToManaged((void*)variant.Content1);
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
