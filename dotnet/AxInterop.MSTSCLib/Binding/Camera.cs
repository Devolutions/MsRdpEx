using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

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
        IMsRdpCameraRedirConfig GetBySymbolicLink(ReadOnlyBinaryStringRef SymbolicLink);
        IMsRdpCameraRedirConfig GetByInstanceId(ReadOnlyBinaryStringRef InstanceId);
        void AddConfig(ReadOnlyBinaryStringRef SymbolicLink, VariantBool fRedirected);
        void SetRedirectByDefault(VariantBool pfRedirect);
        VariantBool GetRedirectByDefault();
        void SetEncodeVideo(VariantBool pfEncode);
        VariantBool GetEncodeVideo();
        void SetEncodingQuality(CameraRedirEncodingQuality pEncodingQuality);
        CameraRedirEncodingQuality GetEncodingQuality();
    }

    public enum CameraRedirEncodingQuality
    {
        encodingQualityLow,
        encodingQualityMedium,
        encodingQualityHigh,
    }

    [GeneratedComInterface]
    [Guid("09750604-D625-47C1-9FCD-F09F735705D7")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpCameraRedirConfig
    {
        BinaryString GetFriendlyName();
        BinaryString GetSymbolicLink();
        BinaryString GetInstanceId();
        BinaryString GetParentInstanceId();
        void SetRedirected(VariantBool pfRedirected);
        VariantBool GetRedirected();
        VariantBool GetDeviceExists();
    }
}
