using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace MsRdpEx.Interop
{
    [GeneratedComInterface]
    [Guid("C1E6743A-41C1-4A74-832A-0DD06C1C7A0E")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsTscNonScriptable
    {
        void SetClearTextPassword(ReadOnlyBinaryStringRef value);
        void SetPortablePassword(ReadOnlyBinaryStringRef value);
        BinaryString GetPortablePassword();
        void SetPortableSalt(ReadOnlyBinaryStringRef value);
        BinaryString GetPortableSalt();
        void SetBinaryPassword(ReadOnlyBinaryStringRef value);
        BinaryString GetBinaryPassword();
        void SetBinarySalt(ReadOnlyBinaryStringRef value);
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

    public static unsafe partial class InteropExtensions
    {
        public static void SendKeys(this IMsRdpClientNonScriptable client, ReadOnlySpan<bool> keyUp, ReadOnlySpan<int> keyData)
        {
            if (keyUp.Length != keyData.Length)
                throw new InvalidOperationException();

            // Documentation says 20 is the maximum number of keys this API can send, so we also use it as the safety limit for stackalloc.
            // If the documentation is wrong and the library supports more inputs this can be removed but the stackalloc needs a soft limit
            // and fall back to array allocation when exceeding it:
            //
            // Span<VariantBool> tempKeyUp = keyUp.Length <= 512 ? stackalloc VariantBool[keyUp.Length] : new VariantBool[keyUp.Length];
            //
            if (keyUp.Length > 20)
                throw new InvalidOperationException();

            Span<VariantBool> keyUpBuffer = stackalloc VariantBool[keyUp.Length];
            for (int i = 0; i < keyUp.Length; i++)
                keyUpBuffer[i] = keyUp[i];

            fixed (VariantBool* pKeyUp = keyUpBuffer)
            fixed (int* pKeyData = keyData)
                client.SendKeys(keyUp.Length, pKeyUp, pKeyData);
        }

        public static void SendKeys(this IMsRdpClientNonScriptable client, ReadOnlySpan<VariantBool> keyUp, ReadOnlySpan<int> keyData)
        {
            if (keyUp.Length != keyData.Length)
                throw new InvalidOperationException();

            fixed (VariantBool* pKeyUp = keyUp)
            fixed (int* pKeyData = keyData)
                client.SendKeys(keyUp.Length, pKeyUp, pKeyData);
        }
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
        IMsRdpDeviceCollection GetDeviceCollection();
        IMsRdpDriveCollection GetDriveCollection();
        void SetWarnAboutSendingCredentials(VariantBool value);
        VariantBool GetWarnAboutSendingCredentials();
        void SetWarnAboutClipboardRedirection(VariantBool value);
        VariantBool GetWarnAboutClipboardRedirection();
        void SetConnectionBarText(ReadOnlyBinaryStringRef value);
        BinaryString GetConnectionBarText();
    }

    [GeneratedComInterface]
    [Guid("F50FA8AA-1C7D-4F59-B15C-A90CACAE1FCB")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientNonScriptable4 : IMsRdpClientNonScriptable3
    {
        void SetRedirectionWarningType(RedirectionWarningType value);
        RedirectionWarningType GetRedirectionWarningType();
        void SetMarkRdpSettingsSecure(VariantBool value);
        VariantBool GetMarkRdpSettingsSecure();
        void SetPublisherCertificateChain(in Variant value);
        void GetPublisherCertificateChain(out Variant value);
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

    public enum RedirectionWarningType
    {
        RedirectionWarningTypeDefault,
        RedirectionWarningTypeUnsigned,
        RedirectionWarningTypeUnknown,
        RedirectionWarningTypeUser,
        RedirectionWarningTypeThirdPartySigned,
        RedirectionWarningTypeTrusted,
        RedirectionWarningTypeMax = RedirectionWarningTypeTrusted,
    }

    [GeneratedComInterface]
    [Guid("4F6996D5-D7B1-412C-B0FF-063718566907")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientNonScriptable5 : IMsRdpClientNonScriptable4
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
        void StartWorkspaceExtension(VariantBool isWebHosted, ReadOnlyBinaryStringRef workspaceId, byte* publisherThumbPrint, int publisherThumbPrintLength);
        void SetSupportsWorkspaceReconnect(VariantBool value);
    }

    public static unsafe partial class InteropExtensions
    {
        public static void StartWorkspaceExtension(this IMsRdpClientNonScriptable8 client, VariantBool isWebHosted, ReadOnlyBinaryStringRef workspaceId, ReadOnlySpan<byte> publisherThumbPrint)
        {
            fixed (byte* publisherThumbPrintPointer = publisherThumbPrint)
                client.StartWorkspaceExtension(isWebHosted, workspaceId, publisherThumbPrintPointer, publisherThumbPrint.Length);
        }
    }
}
