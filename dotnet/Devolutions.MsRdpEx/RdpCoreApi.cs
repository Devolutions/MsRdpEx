using System;
using System.Runtime.InteropServices;

namespace MsRdpEx
{
    public class RdpCoreApi
    {
        public IMsRdpExCoreApi iface;

        public RdpCoreApi()
        {
            iface = Bindings.GetCoreApi();
        }

        public void Load()
        {
            iface.Load();
        }

        public void Unload()
        {
            iface.Unload();
        }

        public string MsRdpExDllPath
        {
            get { return MarshalHelpers.PtrToStringUTF8(iface.GetMsRdpExDllPath()); }
        }

        public bool LogEnabled
        {
            set { iface.SetLogEnabled(value); }
        }

        public MsRdpEx_LogLevel LogLevel
        {
            set { iface.SetLogLevel(value); }
        }

        public string LogFilePath
        {
            set { iface.SetLogFilePath(value); }
        }

        public bool PcapEnabled
        {
            set { iface.SetPcapEnabled(value); }
        }

        public string PcapFilePath
        {
            set { iface.SetPcapFilePath(value); }
        }

        public bool AxHookEnabled
        {
            set { iface.SetAxHookEnabled(value); }
        }
    }
}
