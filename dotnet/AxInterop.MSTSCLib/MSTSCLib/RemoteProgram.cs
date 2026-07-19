using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace MsRdpEx.Interop
{
    [GeneratedComInterface]
    [Guid("FDD029F9-467A-4C49-8529-64B521DBD1B4")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface ITSRemoteProgram : IDispatch
    {
        void SetRemoteProgramMode([MarshalAs(UnmanagedType.VariantBool)] bool value);
        [return: MarshalAs(UnmanagedType.VariantBool)] bool GetRemoteProgramMode();
        void ServerStartProgram(BinaryStringRef ExecutablePath, BinaryStringRef FilePath, BinaryStringRef WorkingDirectory, [MarshalAs(UnmanagedType.VariantBool)] bool ExpandEnvVarInWorkingDirectoryOnServer, BinaryStringRef Arguments, [MarshalAs(UnmanagedType.VariantBool)] bool ExpandEnvVarInArgumentsOnServer);
    }

    [GeneratedComInterface]
    [Guid("92C38A7D-241A-418C-9936-099872C9AF20")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface ITSRemoteProgram2 : ITSRemoteProgram
    {
        void SetRemoteApplicationName(BinaryStringRef value);
        void SetRemoteApplicationProgram(BinaryStringRef value);
        void SetRemoteApplicationArgs(BinaryStringRef value);
    }

    [GeneratedComInterface]
    [Guid("4B84EA77-ACEA-418C-881A-4A8C28AB1510")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface ITSRemoteProgram3 : ITSRemoteProgram2
    {
        void ServerStartApp(BinaryStringRef AppUserModelId, BinaryStringRef Arguments, [MarshalAs(UnmanagedType.VariantBool)] bool ExpandEnvVarInArgumentsOnServer);
    }
}
