using System;
using System.Runtime.InteropServices;

namespace MsRdpEx_App
{
    public class RdpLoader
    {
        [DllImport("MsRdpEx.dll")]
        public static extern UInt64 DllPreCleanUp();

        public RdpLoader()
        {
            UInt64 status = DllPreCleanUp();
        }
    }
}
