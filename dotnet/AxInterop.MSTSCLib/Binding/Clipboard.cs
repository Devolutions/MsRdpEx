﻿using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace MsRdpEx.Interop
{
    [GeneratedComInterface]
    [Guid("2E769EE8-00C7-43DC-AFD9-235D75B72A40")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpClipboard
    {
        VariantBool CanSyncLocalClipboardToRemoteSession();
        void SyncLocalClipboardToRemoteSession();
        VariantBool CanSyncRemoteClipboardToLocalSession();
        void SyncRemoteClipboardToLocalSession();
    }
}
