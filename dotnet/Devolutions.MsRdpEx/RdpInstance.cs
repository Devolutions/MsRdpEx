using System;

namespace MsRdpEx
{
    public class RdpInstance
    {
        public IMsRdpExInstance iface;
        
        public RdpInstance(IMsRdpExInstance iface) {
            this.iface = iface;
        }

        public Guid SessionId
        {
            get { Guid val; iface.GetSessionId(out val); return val; }
        }

        public bool OutputMirrorEnabled {
            get { bool val; iface.GetOutputMirrorEnabled(out val); return val; }
            set { iface.SetOutputMirrorEnabled(value); }
        }

        public bool VideoRecordingEnabled
        {
            get { bool val; iface.GetVideoRecordingEnabled(out val); return val; }
            set { iface.SetVideoRecordingEnabled(value); }
        }

        public bool DumpBitmapUpdates
        {
            get { bool val; iface.GetDumpBitmapUpdates(out val); return val; }
            set { iface.SetDumpBitmapUpdates(value); }
        }

        public bool GetShadowBitmap(ref IntPtr phDC, ref IntPtr phBitmap, ref UInt32 pWidth, ref UInt32 pHeight)
        {
            return iface.GetShadowBitmap(ref phDC, ref phBitmap, ref pWidth, ref pHeight);
        }
    }
}
