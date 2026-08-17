using System.Runtime.InteropServices;

namespace Devolutions.MsRdpEx.Avalonia;

/// <summary>
/// A minimal, WinForms-free projection of the native IMsRdpExInstance methods
/// required by the Avalonia sample.
/// </summary>
internal sealed unsafe class MsRdpExInstanceHandle : IDisposable
{
    private static readonly Guid InterfaceId = new("94CDA65A-EFDF-4453-B8B2-2493A12D31C7");
    private readonly object syncRoot = new();
    private readonly OutputMirrorGetFrameVersion? getFrameVersion;
    private nint instance;

    public MsRdpExInstanceHandle(
        nint rdpControl,
        OutputMirrorGetFrameVersion? getFrameVersion)
    {
        this.getFrameVersion = getFrameVersion;
        Guid interfaceId = InterfaceId;
        int hr = Marshal.QueryInterface(rdpControl, ref interfaceId, out instance);
        Marshal.ThrowExceptionForHR(hr);
    }

    public void SetOutputMirrorEnabled(bool enabled)
    {
        lock (syncRoot)
        {
            ObjectDisposedException.ThrowIf(instance == 0, this);
            nint* vtable = *(nint**)instance;
            var setEnabled = (delegate* unmanaged[Stdcall]<nint, byte, int>)vtable[8];
            int hr = setEnabled(instance, enabled ? (byte)1 : (byte)0);
            Marshal.ThrowExceptionForHR(hr);
        }
    }

    public nint GetInputWindow()
    {
        lock (syncRoot)
        {
            ObjectDisposedException.ThrowIf(instance == 0, this);
            nint* vtable = *(nint**)instance;
            var getInputWindow = (delegate* unmanaged[Stdcall]<nint, nint*, int>)vtable[16];
            nint window;
            int hr = getInputWindow(instance, &window);
            Marshal.ThrowExceptionForHR(hr);
            return window;
        }
    }

    public bool TryGetShadowBitmapFrameVersion(out uint version)
    {
        lock (syncRoot)
        {
            version = 0;
            if (instance == 0 || getFrameVersion is null)
                return false;

            nint* vtable = *(nint**)instance;
            var getOutputMirror = (delegate* unmanaged[Stdcall]<nint, nint*, int>)vtable[5];
            nint outputMirror;
            int hr = getOutputMirror(instance, &outputMirror);
            if (hr < 0 || outputMirror == 0)
                return false;

            version = getFrameVersion(outputMirror);
            return true;
        }
    }

    public bool WithShadowBitmap(Action<nint, int, int, int> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        lock (syncRoot)
        {
            ObjectDisposedException.ThrowIf(instance == 0, this);
            nint* vtable = *(nint**)instance;
            var getShadowBitmap = (delegate* unmanaged[Stdcall]<nint, nint*, nint*, nint*, uint*, uint*, uint*, byte>)vtable[23];
            var lockShadowBitmap = (delegate* unmanaged[Stdcall]<nint, void>)vtable[24];
            var unlockShadowBitmap = (delegate* unmanaged[Stdcall]<nint, void>)vtable[25];

            nint dc;
            nint bitmap;
            nint data;
            uint width;
            uint height;
            uint stride;

            if (getShadowBitmap(instance, &dc, &bitmap, &data, &width, &height, &stride) == 0 ||
                data == 0 || width == 0 || height == 0 || stride == 0)
            {
                return false;
            }

            lockShadowBitmap(instance);

            try
            {
                // The DIB can be replaced during a resize. Resolve it again while
                // holding the native lock before copying any pixels.
                if (getShadowBitmap(instance, &dc, &bitmap, &data, &width, &height, &stride) == 0 ||
                    data == 0 || width == 0 || height == 0 || stride == 0)
                {
                    return false;
                }

                action(data, checked((int)width), checked((int)height), checked((int)stride));
                return true;
            }
            finally
            {
                unlockShadowBitmap(instance);
            }
        }
    }

    public void Dispose()
    {
        lock (syncRoot)
        {
            if (instance == 0)
                return;

            Marshal.Release(instance);
            instance = 0;
        }
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate uint OutputMirrorGetFrameVersion(nint outputMirror);
}
