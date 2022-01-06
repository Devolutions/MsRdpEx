using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MsRdpEx
{
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("13F6E86F-EE7D-44D1-AA94-1136B784441D")]
    public interface IMsRdpExCoreApi
    {
        int Reset();
    }

    public static class Bindings
    {
        private static Guid IID_IMsRdpExCoreApi = new Guid(0x13F6E86F, 0xEE7D, 0x44D1,0xAA, 0x94, 0x11, 0x36, 0xB7, 0x84, 0x44, 0x1D);

        public static IMsRdpExCoreApi GetCoreApi()
        {
            object instance = null;
            MsRdpEx_QueryInterface(ref IID_IMsRdpExCoreApi, out instance);
            return (IMsRdpExCoreApi)instance;
        }

        [DllImport("MsRdpEx.dll")]
        public static extern uint MsRdpEx_QueryInterface(ref Guid riid,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppvObject);

        [DllImport("MsRdpEx.dll")]
        public static extern bool MsRdpEx_GetShadowBitmap(IntPtr hWnd,
            ref IntPtr phDC, ref IntPtr phBitmap, ref UInt32 pWidth, ref UInt32 pHeight);
    }
}
