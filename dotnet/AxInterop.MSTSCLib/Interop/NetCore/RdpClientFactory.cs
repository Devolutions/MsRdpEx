using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Runtime.Versioning;

namespace MSTSCLib
{
    [SupportedOSPlatform("windows")]
    public static partial class RdpClientFactory
    {
        private const uint CLSCTX_INPROC_SERVER = 0x1;
        private static readonly Guid IID_IUnknown = new("00000000-0000-0000-C000-000000000046");

        public static IMsRdpClient10 CreateClient10()
        {
            return Create<IMsRdpClient10>(new Guid("A0C63C30-F08D-4AB4-907C-34905D770C7D"));
        }

        public static T Create<T>(Guid classId) where T : class
        {
            unsafe
            {
                Guid interfaceId = IID_IUnknown;
                int hr = CoCreateInstance(&classId, 0, CLSCTX_INPROC_SERVER, &interfaceId, out nint unknown);
                Marshal.ThrowExceptionForHR(hr);

                try
                {
                    object instance = ComInterfaceMarshaller<object>.ConvertToManaged((void*)unknown)!;
                    return ProxyObject.Pack<T>(instance) ?? throw new InvalidOperationException("Could not create RDP client proxy.");
                }
                finally
                {
                    ComInterfaceMarshaller<object>.Free((void*)unknown);
                }
            }
        }

        [LibraryImport("ole32.dll")]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
        private static unsafe partial int CoCreateInstance(
            Guid* classId,
            nint outer,
            uint classContext,
            Guid* interfaceId,
            out nint instance);
    }
}
