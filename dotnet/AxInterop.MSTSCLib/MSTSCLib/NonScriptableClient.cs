using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using MSTSCLib;

namespace MsRdpEx.Interop
{
    [GeneratedComInterface]
    [Guid("C1E6743A-41C1-4A74-832A-0DD06C1C7A0E")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsTscNonScriptable
    {
        void SetClearTextPassword(BinaryStringRef value);
        void SetPortablePassword(BinaryStringRef value);
        BinaryString GetPortablePassword();
        void SetPortableSalt(BinaryStringRef value);
        BinaryString GetPortableSalt();
        void SetBinaryPassword(BinaryStringRef value);
        BinaryString GetBinaryPassword();
        void SetBinarySalt(BinaryStringRef value);
        BinaryString GetBinarySalt();
        void ResetPassword();
    }

    [GeneratedComInterface]
    [Guid("2F079C4C-87B2-4AFD-97AB-20CDB43038AE")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientNonScriptable : IMsTscNonScriptable
    {
        void NotifyRedirectDeviceChange(nuint wParam, nint lParam);
        void SendKeys(int numKeys, VariantBool* pbArrayKeyUp, int* plKeyData);
    }

    [GeneratedComInterface]
    [Guid("17A5E535-4072-4FA4-AF32-C8D0D47345E9")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientNonScriptable2 : IMsRdpClientNonScriptable
    {
        void SetUIParentWindowHandle(nint value);
        nint GetUIParentWindowHandle();
    }

    [GeneratedComInterface]
    [Guid("B3378D90-0728-45C7-8ED7-B6159FB92219")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientNonScriptable3 : IMsRdpClientNonScriptable2
    {
        void SetShowRedirectionWarningDialog([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetShowRedirectionWarningDialog();
        void SetPromptForCredentials([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetPromptForCredentials();
        void SetNegotiateSecurityLayer([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetNegotiateSecurityLayer();
        void SetEnableCredSspSupport([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetEnableCredSspSupport();
        void SetRedirectDynamicDrives([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetRedirectDynamicDrives();
        void SetRedirectDynamicDevices([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetRedirectDynamicDevices();
        IMsRdpDeviceCollection GetDeviceCollection();
        IMsRdpDriveCollection GetDriveCollection();
        void SetWarnAboutSendingCredentials([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetWarnAboutSendingCredentials();
        void SetWarnAboutClipboardRedirection([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetWarnAboutClipboardRedirection();
        void SetConnectionBarText(BinaryStringRef value);
        BinaryString GetConnectionBarText();
    }

    [GeneratedComInterface]
    [Guid("F50FA8AA-1C7D-4F59-B15C-A90CACAE1FCB")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientNonScriptable4 : IMsRdpClientNonScriptable3
    {
        void SetRedirectionWarningType(RedirectionWarningType value);
        RedirectionWarningType GetRedirectionWarningType();
        void SetMarkRdpSettingsSecure([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetMarkRdpSettingsSecure();
        void SetPublisherCertificateChain([MarshalUsing(typeof(VariantMarshaller))] in object value);
        [return: MarshalUsing(typeof(VariantMarshaller))] object GetPublisherCertificateChain();
        void SetWarnAboutPrinterRedirection([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetWarnAboutPrinterRedirection();
        void SetAllowCredentialSaving([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetAllowCredentialSaving();
        void SetPromptForCredsOnClient([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetPromptForCredsOnClient();
        void SetLaunchedViaClientShellInterface([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetLaunchedViaClientShellInterface();
        void SetTrustedZoneSite([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetTrustedZoneSite();
    }

    //public enum RedirectionWarningType
    //{
    //    RedirectionWarningTypeDefault,
    //    RedirectionWarningTypeUnsigned,
    //    RedirectionWarningTypeUnknown,
    //    RedirectionWarningTypeUser,
    //    RedirectionWarningTypeThirdPartySigned,
    //    RedirectionWarningTypeTrusted,
    //    RedirectionWarningTypeMax = RedirectionWarningTypeTrusted,
    //}

    [GeneratedComInterface]
    [Guid("4F6996D5-D7B1-412C-B0FF-063718566907")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientNonScriptable5 : IMsRdpClientNonScriptable4
    {
        void SetUseMultimon([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetUseMultimon();
        uint GetRemoteMonitorCount();
        void GetRemoteMonitorsBoundingBox(out int pLeft, out int pTop, out int pRight, out int pBottom);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetRemoteMonitorLayoutMatchesLocal();
        void SetDisableConnectionBar([MarshalAs(UnmanagedType.VariantBool)] bool value);
        void SetDisableRemoteAppCapsCheck([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetDisableRemoteAppCapsCheck();
        void SetWarnAboutDirectXRedirection([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetWarnAboutDirectXRedirection();
        void SetAllowPromptingForCredentials([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetAllowPromptingForCredentials();
    }

    [GeneratedComInterface]
    [Guid("05293249-B28B-4BD8-BE64-1B2F496B910E")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientNonScriptable6 : IMsRdpClientNonScriptable5
    {
        void SendLocation2D(double latitude, double longitude);
        void SendLocation3D(double latitude, double longitude, int altitude);
    }

    [GeneratedComInterface]
    [Guid("71B4A60A-FE21-46D8-A39B-8E32BA0C5ECC")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientNonScriptable7 : IMsRdpClientNonScriptable6
    {
        IMsRdpCameraRedirConfigCollection GetCameraRedirConfigCollection();
        void DisableDpiCursorScalingForProcess();
        IMsRdpClipboard GetClipboard();
    }

    [GeneratedComInterface]
    [Guid("B2B3FA47-3F11-4148-AD24-DFF8684A16D0")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientNonScriptable8 : IMsRdpClientNonScriptable7
    {
        Guid GetCorrelationId();
        void StartWorkspaceExtension([MarshalAs(UnmanagedType.VariantBool)] bool isWebHosted, BinaryStringRef workspaceId, byte* publisherThumbPrint, int publisherThumbPrintLength);
        void SetSupportsWorkspaceReconnect([MarshalAs(UnmanagedType.VariantBool)] bool value);
    }

    public static unsafe partial class InteropExtensions
    {
        public static void StartWorkspaceExtension(this IMsRdpClientNonScriptable8 client, [MarshalAs(UnmanagedType.VariantBool)] bool isWebHosted, BinaryStringRef workspaceId, ReadOnlySpan<byte> publisherThumbPrint)
        {
            fixed (byte* publisherThumbPrintPointer = publisherThumbPrint)
                client.StartWorkspaceExtension(isWebHosted, workspaceId, publisherThumbPrintPointer, publisherThumbPrint.Length);
        }
    }
}
