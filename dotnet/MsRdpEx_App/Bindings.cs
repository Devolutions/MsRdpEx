using System;
using System.Runtime.InteropServices;

namespace MsRdpEx
{
    public static class Bindings
    {
        [DllImport("MsRdpEx.dll")]
        public static extern UInt64 DllPreCleanUp();

        [DllImport("MsRdpEx.dll")]
        public static extern bool MsRdpEx_GetShadowBitmap(IntPtr hWnd,
            ref IntPtr phDC, ref IntPtr phBitmap, ref UInt32 pWidth, ref UInt32 pHeight);
    }
}
