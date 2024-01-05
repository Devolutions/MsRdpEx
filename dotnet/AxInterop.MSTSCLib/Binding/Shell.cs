using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace MsRdpEx.Interop
{
    [GeneratedComInterface]
    [Guid("D012AE6D-C19A-4BFE-B367-201F8911F134")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientShell : IDispatch
    {
        void Launch();
        void SetRdpFileContents(ReadOnlyBinaryStringRef RdpFile);
        BinaryString GetRdpFileContents();
        void SetRdpProperty(ReadOnlyBinaryStringRef Property, Variant value);
        void GetRdpProperty(ReadOnlyBinaryStringRef Property, out Variant value);
        VariantBool GetIsRemoteProgramClientInstalled();
        void SetPublicMode(VariantBool pfPublicMode);
        VariantBool GetPublicMode();
        void ShowTrustedSitesManagementDialog();
    }
}
