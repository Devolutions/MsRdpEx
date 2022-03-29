using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MsRdpEx
{
    public enum MsRdpEx_LogLevel : uint
    {
        Trace = 0,
        Debug = 1,
        Info = 2,
        Warn = 3,
        Error = 4,
        Fatal = 5,
        Off = 6
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("13F6E86F-EE7D-44D1-AA94-1136B784441D")]
    public interface IMsRdpExCoreApi
    {
        void Load();
        void Unload();

        [MethodImpl(MethodImplOptions.PreserveSig)]
        IntPtr GetMsRdpExDllPath();

        [MethodImpl(MethodImplOptions.PreserveSig)]
        void SetLogEnabled([MarshalAs(UnmanagedType.U1)] bool logEnabled);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        void SetLogLevel([MarshalAs(UnmanagedType.U4)] MsRdpEx_LogLevel logLevel);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        void SetLogFilePath([MarshalAs(UnmanagedType.LPStr)] string logFilePath);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        void SetPcapEnabled([MarshalAs(UnmanagedType.U1)] bool pcapEnabled);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        void SetPcapFilePath([MarshalAs(UnmanagedType.LPStr)] string pcapFilePath);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        void SetAxHookEnabled([MarshalAs(UnmanagedType.U1)] bool axHookEnabled);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        [return: MarshalAs(UnmanagedType.U1)]
        bool QueryInstanceByWindowHandle(IntPtr hWnd, [MarshalAs(UnmanagedType.IUnknown)] out object rdpInstance);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        [return: MarshalAs(UnmanagedType.U1)]
        bool OpenInstanceForWindowHandle(IntPtr hWnd, [MarshalAs(UnmanagedType.IUnknown)] out object rdpInstance);
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("338784B3-3363-45A2-8ECD-80A65DBAF636")]
    public interface IMsRdpExProcess
    {
        [MethodImpl(MethodImplOptions.PreserveSig)]
        void SetFileName([MarshalAs(UnmanagedType.LPStr)] string filename);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        void SetWorkingDirectory([MarshalAs(UnmanagedType.LPStr)] string workingDirectory);

        void Start(int argc, ref IntPtr[] argv,
            [MarshalAs(UnmanagedType.LPStr)] string appName,
            [MarshalAs(UnmanagedType.LPStr)] string axName);
        void Stop(UInt32 exitCode);
        void Wait(UInt32 milliseconds);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        uint GetProcessId();

        [MethodImpl(MethodImplOptions.PreserveSig)]
        uint GetExitCode();
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("94CDA65A-EFDF-4453-B8B2-2493A12D31C7")]
    public interface IMsRdpExInstance
    {
        void GetRdpClient([MarshalAs(UnmanagedType.IUnknown)] out object rdpClient);

        void GetOutputMirrorObject(out IntPtr outputMirror);

        void SetOutputMirrorObject(IntPtr outputMirror);

        void GetOutputMirrorEnabled([MarshalAs(UnmanagedType.U1)] out bool outputMirrorEnabled);

        void SetOutputMirrorEnabled([MarshalAs(UnmanagedType.U1)] bool outputMirrorEnabled);

        void GetVideoRecordingEnabled([MarshalAs(UnmanagedType.U1)] out bool videoRecordingEnabled);

        void SetVideoRecordingEnabled([MarshalAs(UnmanagedType.U1)] bool videoRecordingEnabled);

        void GetDumpBitmapUpdates([MarshalAs(UnmanagedType.U1)] out bool dumpBitmapUpdates);

        void SetDumpBitmapUpdates([MarshalAs(UnmanagedType.U1)] bool dumpBitmapUpdates);

        void GetCorePropsRawPtr(out IntPtr pCorePropsRaw);

        void SetCorePropsRawPtr(IntPtr pCorePropsRaw);

        void AttachOutputWindow(IntPtr hOutputWnd, IntPtr pUserData);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        [return: MarshalAs(UnmanagedType.U1)]
        bool GetShadowBitmap(ref IntPtr phDC, ref IntPtr phBitmap, ref UInt32 pWidth, ref UInt32 pHeight);
    }

    public static class Bindings
    {
        private static Guid IID_IMsRdpExCoreApi = new Guid(0x13F6E86F, 0xEE7D, 0x44D1,0xAA, 0x94, 0x11, 0x36, 0xB7, 0x84, 0x44, 0x1D);
        private static Guid IID_IMsRdpExProcess = new Guid(0x338784B3, 0x3363, 0x45A2, 0x8E, 0xCD, 0x80, 0xA6, 0x5D, 0xBA, 0xF6, 0x36);
        private static Guid IID_IMsRdpExInstance = new Guid(0x94CDA65A, 0xEFDF, 0x4453, 0xB8, 0xB2, 0x24, 0x93, 0xA1, 0x2D, 0x31, 0xC7);

        public static IMsRdpExCoreApi GetCoreApi()
        {
            object instance = null;
            MsRdpEx_CreateInstance(ref IID_IMsRdpExCoreApi, out instance);
            return (IMsRdpExCoreApi)instance;
        }

        public static IMsRdpExProcess StartProcess(string[] args, string appName, string axName)
        {
            object instance = null;
            MsRdpEx_CreateInstance(ref IID_IMsRdpExProcess, out instance);

            IMsRdpExProcess process = (IMsRdpExProcess) instance;

            int argc = args.Length;
            IntPtr[] argv = new IntPtr[argc];

            for (int i = 0; i < argc; i++)
            {
                argv[i] = Marshal.StringToCoTaskMemAnsi(args[i]);
            }

            process.Start(argc, ref argv, appName, axName);

            for (int i = 0; i < argv.Length; i++)
            {
                Marshal.FreeCoTaskMem(argv[i]);
            }

            return process;
        }

        [DllImport("MsRdpEx.dll")]
        public static extern uint MsRdpEx_CreateInstance(ref Guid riid,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppvObject);
    }
}
