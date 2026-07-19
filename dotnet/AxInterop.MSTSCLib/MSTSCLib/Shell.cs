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
        void SetRdpFileContents(BinaryStringRef RdpFile);
        BinaryString GetRdpFileContents();
        void SetRdpProperty(BinaryStringRef Property, [MarshalUsing(typeof(VariantMarshaller))] object value);
        [return: MarshalUsing(typeof(VariantMarshaller))] object GetRdpProperty(BinaryStringRef Property);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetIsRemoteProgramClientInstalled();
        void SetPublicMode([MarshalAs(UnmanagedType.VariantBool)] bool pfPublicMode);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetPublicMode();
        void ShowTrustedSitesManagementDialog();
    }
}
