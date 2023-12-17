using System;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

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

    [GeneratedComInterface]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("13F6E86F-EE7D-44D1-AA94-1136B784441D")]
    public partial interface IMsRdpExCoreApi
    {
        void Load();
        void Unload();

        [MethodImpl(MethodImplOptions.PreserveSig)]
        IntPtr GetMsRdpExDllPath();

        [MethodImpl(MethodImplOptions.PreserveSig)]
        void SetLogEnabled([MarshalAs(UnmanagedType.U1)] bool logEnabled);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        void SetLogLevel(MsRdpEx_LogLevel logLevel);

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
        bool QueryInstanceByWindowHandle(IntPtr hWnd, [MarshalAs(UnmanagedType.Interface)] out object rdpInstance);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        [return: MarshalAs(UnmanagedType.U1)]
        bool OpenInstanceForWindowHandle(IntPtr hWnd, [MarshalAs(UnmanagedType.Interface)] out object rdpInstance);
    }

    [GeneratedComInterface]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("338784B3-3363-45A2-8ECD-80A65DBAF636")]
    public partial interface IMsRdpExProcess
    {
        [MethodImpl(MethodImplOptions.PreserveSig)]
        void SetFileName([MarshalAs(UnmanagedType.LPStr)] string filename);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        void SetArguments([MarshalAs(UnmanagedType.LPStr)] string arguments);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        void SetArgumentBlock([MarshalAs(UnmanagedType.LPStr)] string argumentBlock);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        void SetEnvironmentBlock([MarshalAs(UnmanagedType.LPStr)] string environmentBlock);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        void SetWorkingDirectory([MarshalAs(UnmanagedType.LPStr)] string workingDirectory);

        void StartWithInfo();

        void Start(int argc, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] ref IntPtr[] argv,
            [MarshalAs(UnmanagedType.LPStr)] string appName,
            [MarshalAs(UnmanagedType.LPStr)] string axName);

        void Stop(UInt32 exitCode);

        void Wait(UInt32 milliseconds);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        uint GetProcessId();

        [MethodImpl(MethodImplOptions.PreserveSig)]
        uint GetExitCode();
    }

    [GeneratedComInterface]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("94CDA65A-EFDF-4453-B8B2-2493A12D31C7")]
    public partial interface IMsRdpExInstance
    {
        void GetSessionId(out Guid sessionId);

        void GetRdpClient([MarshalAs(UnmanagedType.Interface)] out object rdpClient);

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

        void AttachInputWindow(IntPtr hInputWnd, IntPtr pUserData);

        void AttachOutputWindow(IntPtr hOutputWnd, IntPtr pUserData);

        void AttachExtendedSettings(IntPtr pExtendedSettings);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        [return: MarshalAs(UnmanagedType.U1)]
        bool GetExtendedSettings(out IntPtr ppExtendedSettings);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        [return: MarshalAs(UnmanagedType.U1)]
        bool GetShadowBitmap(ref IntPtr phDC, ref IntPtr phBitmap, ref IntPtr pBitmapData,
            ref UInt32 pBitmapWidth, ref UInt32 pBitmapHeight, ref UInt32 pBitmapStep);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        void LockShadowBitmap();

        [MethodImpl(MethodImplOptions.PreserveSig)]
        void UnlockShadowBitmap();

        [MethodImpl(MethodImplOptions.PreserveSig)]
        void GetLastMousePosition(ref Int32 posX, ref Int32 posY);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        void SetLastMousePosition(Int32 posX, Int32 posY);
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

        public static IMsRdpExProcess CreateProcess()
        {
            object instance = null;
            MsRdpEx_CreateInstance(ref IID_IMsRdpExProcess, out instance);
            IMsRdpExProcess process = (IMsRdpExProcess)instance;
            return process;
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

        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("MsRdpEx.dll")]
        public static extern uint MsRdpEx_CreateInstance(ref Guid riid,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppvObject);

        [DllImport("MsRdpEx.dll")]
        public static extern uint MsRdpEx_GetClaimsToken(
            [MarshalAs(UnmanagedType.BStr)] string clientAddress,
            [MarshalAs(UnmanagedType.BStr)] string claimsHint,
            [MarshalAs(UnmanagedType.BStr)] string userNameHint,
            [MarshalAs(UnmanagedType.BStr)] string userDomainHint,
            uint uiSilentRetrievalMode,
            [MarshalAs(UnmanagedType.Bool)] bool allowCredPrompt,
            IntPtr parentWindow,
            [MarshalAs(UnmanagedType.BStr)] ref string claimsToken,
            [MarshalAs(UnmanagedType.BStr)] ref string actualAuthority,
            [MarshalAs(UnmanagedType.BStr)] ref string actualUserName,
            ref RECT position,
            [MarshalAs(UnmanagedType.BStr)] string windowTitle,
            [MarshalAs(UnmanagedType.BStr)] string logonCertAuthority,
            [MarshalAs(UnmanagedType.BStr)] ref string resultMsg,
            [MarshalAs(UnmanagedType.BStr)] string avdActivityId,
            [MarshalAs(UnmanagedType.Bool)] ref bool isAcquiredSilently,
            [MarshalAs(UnmanagedType.Bool)] ref bool isRetriableError,
            [MarshalAs(UnmanagedType.Bool)] bool invalidateCache,
            [MarshalAs(UnmanagedType.BStr)] string resourceAppId);

        [DllImport("MsRdpEx.dll")]
        public static extern uint MsRdpEx_LogoffClaimsToken(
            [MarshalAs(UnmanagedType.BStr)] string claimsHint,
            [MarshalAs(UnmanagedType.BStr)] string clientId,
            [MarshalAs(UnmanagedType.BStr)] string username);

        [DllImport("MsRdpEx.dll")]
        public static extern void MsRdpEx_CancelAuthentication();

        [DllImport("MsRdpEx.dll")]
        public static extern uint MsRdpEx_DeleteSavedCreds(
            [MarshalAs(UnmanagedType.BStr)] string workspaceId,
            [MarshalAs(UnmanagedType.BStr)] string username);

        [DllImport("MsRdpEx.dll")]
        public static extern uint MsRdpEx_PreCleanUp();

        public enum SilentRetrievalMode
        {
            None,                     // 0
            All,                      // 1
            GetDefaultAccount,        // 2
            GetTokenFromUsername,     // 3
            GetTokenFromWebAccount    // 4
        }

        public static uint GetClaimsToken(
            string feedAddress,
            string claimsHint,
            string userName,
            string domain,
            SilentRetrievalMode silentRetrievalMode,
            bool allowCredPrompt,
            IntPtr mainWindowHandle,
            string credentialWindowMessage = "",
            bool invalidateCache = false,
            string resourceAppId = "")
        {
            uint hr;
            string claimsToken = "";
            string actualUserName = "";
            string actualAuthority = "";
            RECT position = default(RECT);
            string resultMsg = "";
            string avdActivityId = "";
            bool isAcquiredSilently = false;
            bool isRetriableError = false;

            hr = MsRdpEx_GetClaimsToken(
                feedAddress,
                claimsHint,
                userName,
                domain,
                (uint)silentRetrievalMode,
                allowCredPrompt,
                mainWindowHandle,
                ref claimsToken,
                ref actualAuthority,
                ref actualUserName,
                ref position,
                credentialWindowMessage,
                "",
                ref resultMsg,
                avdActivityId,
                ref isAcquiredSilently,
                ref isRetriableError,
                invalidateCache,
                resourceAppId);

            return hr;
        }
    }
}
