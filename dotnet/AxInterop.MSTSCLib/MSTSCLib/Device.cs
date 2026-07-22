using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace MsRdpEx.Interop
{
    [GeneratedComInterface]
    [Guid("56540617-D281-488C-8738-6A8FDF64A118")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpDeviceCollection
    {
        void RescanDevices([MarshalAs(UnmanagedType.VariantBool)] bool DynRedir);
        IMsRdpDevice GetDeviceByIndex(uint index);
        IMsRdpDevice GetDeviceById(BinaryStringRef DeviceInstanceId);
        uint GetDeviceCount();
    }

    [GeneratedComInterface]
    [Guid("60C3B9C8-9E92-4F5E-A3E7-604A912093EA")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpDevice
    {
        BinaryString GetDeviceInstanceId();
        BinaryString GetFriendlyName();
        BinaryString GetDeviceDescription();
        void SetRedirectionState([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetRedirectionState();
    }
}
