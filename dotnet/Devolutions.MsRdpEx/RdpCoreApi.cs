using System;

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

        public bool LogEnabled
        {
            set { iface.SetLogEnabled(value); }
        }

        public string LogFilePath
        {
            set { iface.SetLogFilePath(value); }
        }
    }
}
