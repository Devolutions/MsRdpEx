using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using MSTSCLib;

namespace MsRdpEx.Interop
{
    [GeneratedComInterface]
    [Guid("AE45252B-AAAB-4504-B681-649D6073A37A")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpCameraRedirConfigCollection
    {
        void Rescan();
        uint GetCount();
        IMsRdpCameraRedirConfig GetByIndex(uint index);
        IMsRdpCameraRedirConfig GetBySymbolicLink(BinaryStringRef SymbolicLink);
        IMsRdpCameraRedirConfig GetByInstanceId(BinaryStringRef InstanceId);
        void AddConfig(BinaryStringRef SymbolicLink, [MarshalAs(UnmanagedType.VariantBool)] bool fRedirected);
        void SetRedirectByDefault([MarshalAs(UnmanagedType.VariantBool)] bool pfRedirect);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetRedirectByDefault();
        void SetEncodeVideo([MarshalAs(UnmanagedType.VariantBool)] bool pfEncode);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetEncodeVideo();
        void SetEncodingQuality(CameraRedirEncodingQuality pEncodingQuality);
        CameraRedirEncodingQuality GetEncodingQuality();
    }

    //public enum CameraRedirEncodingQuality
    //{
    //    encodingQualityLow,
    //    encodingQualityMedium,
    //    encodingQualityHigh,
    //}

    [GeneratedComInterface]
    [Guid("09750604-D625-47C1-9FCD-F09F735705D7")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpCameraRedirConfig
    {
        BinaryString GetFriendlyName();
        BinaryString GetSymbolicLink();
        BinaryString GetInstanceId();
        BinaryString GetParentInstanceId();
        void SetRedirected([MarshalAs(UnmanagedType.VariantBool)] bool pfRedirected);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetRedirected();
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetDeviceExists();
    }
}
