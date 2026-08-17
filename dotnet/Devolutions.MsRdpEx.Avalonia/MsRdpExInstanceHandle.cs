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
    private readonly OutputMirrorCaptureApi? captureApi;
    private readonly OutputMirrorGetFrameVersion? getFrameVersion;
    private nint capture;
    private nint instance;

    public MsRdpExInstanceHandle(
        nint rdpControl,
        OutputMirrorCaptureApi? captureApi,
        OutputMirrorGetFrameVersion? getFrameVersion)
    {
        this.captureApi = captureApi;
        this.getFrameVersion = getFrameVersion;
        Guid interfaceId = InterfaceId;
        int hr = Marshal.QueryInterface(rdpControl, ref interfaceId, out instance);
        Marshal.ThrowExceptionForHR(hr);

        try
        {
            if (captureApi is not null)
            {
                hr = captureApi.Create(instance, out capture);
                Marshal.ThrowExceptionForHR(hr);
            }
        }
        catch
        {
            Marshal.Release(instance);
            instance = 0;
            throw;
        }
    }

    public bool CanCaptureOffThread => capture != 0;

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
            if (instance == 0)
                return false;

            if (capture != 0 && captureApi is not null)
            {
                version = captureApi.GetFrameVersion(capture);
                return true;
            }

            if (getFrameVersion is null)
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

            if (capture != 0 && captureApi is not null)
                return WithNativeCapture(action, captureApi);

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

            if (capture != 0)
            {
                captureApi!.Release(capture);
                capture = 0;
            }

            Marshal.Release(instance);
            instance = 0;
        }
    }

    private bool WithNativeCapture(
        Action<nint, int, int, int> action,
        OutputMirrorCaptureApi api)
    {
        if (!api.GetShadowBitmap(
            capture,
            out _,
            out _,
            out nint data,
            out uint width,
            out uint height,
            out uint stride) ||
            data == 0 || width == 0 || height == 0 || stride == 0)
        {
            return false;
        }

        api.Lock(capture);
        try
        {
            if (!api.GetShadowBitmap(
                capture,
                out _,
                out _,
                out data,
                out width,
                out height,
                out stride) ||
                data == 0 || width == 0 || height == 0 || stride == 0)
            {
                return false;
            }

            action(data, checked((int)width), checked((int)height), checked((int)stride));
            return true;
        }
        finally
        {
            api.Unlock(capture);
        }
    }

    internal sealed record OutputMirrorCaptureApi(
        OutputMirrorCaptureCreate Create,
        OutputMirrorCaptureGetFrameVersion GetFrameVersion,
        OutputMirrorCaptureGetShadowBitmap GetShadowBitmap,
        OutputMirrorCaptureLock Lock,
        OutputMirrorCaptureUnlock Unlock,
        OutputMirrorCaptureRelease Release);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate uint OutputMirrorGetFrameVersion(nint outputMirror);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate int OutputMirrorCaptureCreate(nint instance, out nint capture);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate uint OutputMirrorCaptureGetFrameVersion(nint capture);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal delegate bool OutputMirrorCaptureGetShadowBitmap(
        nint capture,
        out nint dc,
        out nint bitmap,
        out nint data,
        out uint width,
        out uint height,
        out uint stride);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate void OutputMirrorCaptureLock(nint capture);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate void OutputMirrorCaptureUnlock(nint capture);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate void OutputMirrorCaptureRelease(nint capture);
}
