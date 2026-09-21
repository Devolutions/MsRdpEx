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

        /// <summary>
        /// Controls isolation for new Microsoft RDP gateway RPC bindings in this process.
        /// Defaults to true unless overridden at startup by MSRDPEX_GATEWAY_UNIQUE_BINDING.
        /// Set before connecting; already observed bindings retain their behavior.
        /// Requires ActiveX hooks to be enabled.
        /// </summary>
        /// <exception cref="NotSupportedException">The native DLL does not expose gateway settings.</exception>
        public bool GatewayIsolationEnabled
        {
            get { return GatewaySettings.GetGatewayIsolationEnabled(); }
            set { GatewaySettings.SetGatewayIsolationEnabled(value); }
        }

        private IMsRdpExGatewaySettings GatewaySettings
        {
            get
            {
                return iface as IMsRdpExGatewaySettings ??
                    throw new NotSupportedException("The loaded MsRdpEx DLL does not support gateway isolation settings.");
            }
        }

        public bool AxHookEnabled
        {
            set { iface.SetAxHookEnabled(value); }
        }
    }
}
