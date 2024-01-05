using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace MsRdpEx.Interop
{
    [GeneratedComInterface]
    [Guid("720298C0-A099-46F5-9F82-96921BAE4701")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientTransportSettings : IDispatch
    {
        void SetGatewayHostname(ReadOnlyBinaryStringRef value);
        BinaryString GetGatewayHostname();
        void SetGatewayUsageMethod(uint value);
        uint GetGatewayUsageMethod();
        void SetGatewayProfileUsageMethod(uint value);
        uint GetGatewayProfileUsageMethod();
        void SetGatewayCredsSource(uint value);
        uint GetGatewayCredsSource();
        void SetGatewayUserSelectedCredsSource(uint value);
        uint GetGatewayUserSelectedCredsSource();
        int GetGatewayIsSupported();
        uint GetGatewayDefaultUsageMethod();
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
        void SetGatewayPreAuthServerAddr(ReadOnlyBinaryStringRef value);
        BinaryString GetGatewayPreAuthServerAddr();
        void SetGatewaySupportUrl(ReadOnlyBinaryStringRef value);
        BinaryString GetGatewaySupportUrl();
        void SetGatewayEncryptedOtpCookie(ReadOnlyBinaryStringRef value);
        BinaryString GetGatewayEncryptedOtpCookie();
        void SetGatewayEncryptedOtpCookieSize(uint value);
        uint GetGatewayEncryptedOtpCookieSize();
        void SetGatewayUsername(ReadOnlyBinaryStringRef value);
        BinaryString GetGatewayUsername();
        void SetGatewayDomain(ReadOnlyBinaryStringRef value);
        BinaryString GetGatewayDomain();
        void SetGatewayPassword(ReadOnlyBinaryStringRef value);
    }

    [GeneratedComInterface]
    [Guid("3D5B21AC-748D-41DE-8F30-E15169586BD4")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientTransportSettings3 : IMsRdpClientTransportSettings2
    {
        void SetGatewayCredSourceCookie(uint value);
        uint GetGatewayCredSourceCookie();
        void SetGatewayAuthCookieServerAddr(ReadOnlyBinaryStringRef value);
        BinaryString GetGatewayAuthCookieServerAddr();
        void SetGatewayEncryptedAuthCookie(ReadOnlyBinaryStringRef value);
        BinaryString GetGatewayEncryptedAuthCookie();
        void SetGatewayEncryptedAuthCookieSize(uint value);
        uint GetGatewayEncryptedAuthCookieSize();
        void SetGatewayAuthLoginPage(ReadOnlyBinaryStringRef value);
        BinaryString GetGatewayAuthLoginPage();
    }

    [GeneratedComInterface]
    [Guid("011C3236-4D81-4515-9143-067AB630D299")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientTransportSettings4 : IMsRdpClientTransportSettings3
    {
        void SetGatewayBrokeringType(uint value);
    }
}
