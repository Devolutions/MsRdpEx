using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace MsRdpEx.Interop
{
    [GeneratedComInterface]
    [Guid("C9D65442-A0F9-45B2-8F73-D61D2DB8CBB6")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsTscSecuredSettings : IDispatch
    {
        void SetStartProgram(ReadOnlyBinaryStringRef pStartProgram);
        BinaryString GetStartProgram();
        void SetWorkDir(ReadOnlyBinaryStringRef pWorkDir);
        BinaryString GetWorkDir();
        void SetFullScreen(int pfFullScreen);
        int GetFullScreen();
    }

    [GeneratedComInterface]
    [Guid("605BEFCF-39C1-45CC-A811-068FB7BE346D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientSecuredSettings : IMsTscSecuredSettings
    {
        void SetKeyboardHookMode(int pkeyboardHookMode);
        int GetKeyboardHookMode();
        void SetAudioRedirectionMode(int pAudioRedirectionMode);
        int GetAudioRedirectionMode();
    }

    [GeneratedComInterface]
    [Guid("25F2CE20-8B1D-4971-A7CD-549DAE201FC0")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClientSecuredSettings2 : IMsRdpClientSecuredSettings
    {
        BinaryString GetPCB();
        void SetPCB(ReadOnlyBinaryStringRef bstrPCB);
    }
}
