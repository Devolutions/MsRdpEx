using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using MsRdpEx;

namespace MsRdpEx_App
{
    public class MsRdpExManager
    {
        private static readonly RdpCoreApi coreApi;

        private static bool axHookEnabled = true;

        public RdpCoreApi CoreApi { get => coreApi; }

        public bool AxHookEnabled { get => axHookEnabled; }

        private static RdpCoreApi LoadCoreApi()
        {
            RdpCoreApi coreApi = new RdpCoreApi();

            string logFilePath = Environment.ExpandEnvironmentVariables("%LocalAppData%\\MsRdpEx\\HostApp.log");
            string pcapFilePath = Environment.ExpandEnvironmentVariables("%LocalAppData%\\MsRdpEx\\capture.pcap");

            coreApi.LogEnabled = true;
            coreApi.LogLevel = MsRdpEx_LogLevel.Trace;
            coreApi.LogFilePath = logFilePath;
            coreApi.PcapEnabled = false;
            coreApi.PcapFilePath = pcapFilePath;
            coreApi.AxHookEnabled = axHookEnabled;
            coreApi.Load();

            return coreApi;
        }

        private static MsRdpExManager instance = null;
        private static readonly object padlock = new object();

        public static MsRdpExManager Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new MsRdpExManager();
                    }

                    return instance;
                }
            }
        }

        static MsRdpExManager()
        {
            coreApi = LoadCoreApi();
        }
    }
}
