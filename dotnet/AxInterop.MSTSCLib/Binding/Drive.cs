using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace MsRdpEx.Interop
{
    [GeneratedComInterface]
    [Guid("7FF17599-DA2C-4677-AD35-F60C04FE1585")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpDriveCollection
    {
        void RescanDrives(VariantBool DynRedir);
        IMsRdpDrive GetDriveByIndex(uint index);
        uint GetDriveCount();
    }

    [GeneratedComInterface]
    [Guid("D28B5458-F694-47A8-8E61-40356A767E46")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpDrive
    {
        BinaryString GetName();
        void SetRedirectionState(VariantBool RedirState);
        VariantBool GetRedirectionState();
    }
}
