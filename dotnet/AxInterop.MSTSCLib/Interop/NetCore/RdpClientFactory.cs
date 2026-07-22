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

        /// <summary>
        /// Creates the non-scriptable RDP control registered as version 10
        /// (<c>MsRdpClient9NotSafeForScripting</c>).
        /// </summary>
        /// <remarks>
        /// The imported type library declares this control's default interface as
        /// <see cref="IMsRdpClient9"/>.
        /// </remarks>
        /// <exception cref="COMException">
        /// The RDP control is not registered or COM cannot activate it.
        /// </exception>
        public static IMsRdpClient9 CreateClient9()
        {
            return Create<IMsRdpClient9>(new Guid("8B918B82-7985-4C24-89DF-C33AD2BBFBCD"));
        }

        /// <summary>
        /// Creates the non-scriptable RDP control registered as version 11
        /// (<c>MsRdpClient10NotSafeForScripting</c>).
        /// </summary>
        /// <remarks>
        /// The imported type library declares this control's default interface as
        /// <see cref="IMsRdpClient10"/>.
        /// </remarks>
        /// <exception cref="COMException">
        /// The RDP control is not registered or COM cannot activate it.
        /// </exception>
        public static IMsRdpClient10 CreateClient10()
        {
            return Create<IMsRdpClient10>(new Guid("A0C63C30-F08D-4AB4-907C-34905D770C7D"));
        }

        /// <summary>
        /// Creates the non-scriptable RDP control registered as version 12
        /// (<c>MsRdpClient11NotSafeForScripting</c>).
        /// </summary>
        /// <remarks>
        /// Version 12 retains <see cref="IMsRdpClient10"/> as its default interface in the
        /// imported type library, so this factory returns that compatible generated interface.
        /// </remarks>
        /// <exception cref="COMException">
        /// The RDP control is not registered or COM cannot activate it.
        /// </exception>
        public static IMsRdpClient10 CreateClient11()
        {
            return Create<IMsRdpClient10>(new Guid("1DF7C823-B2D4-4B54-975A-F2AC5D7CF8B8"));
        }

        /// <summary>
        /// Activates an in-process RDP control and projects it through a generated COM interface.
        /// </summary>
        /// <typeparam name="T">A generated RDP COM interface implemented by the control.</typeparam>
        /// <param name="classId">The CLSID of the RDP control to activate.</param>
        /// <returns>A COM proxy whose lifetime is retained by the returned interface.</returns>
        /// <exception cref="COMException">
        /// The control is not registered or cannot be activated.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// A generated proxy for <typeparamref name="T"/> could not be created.
        /// </exception>
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
