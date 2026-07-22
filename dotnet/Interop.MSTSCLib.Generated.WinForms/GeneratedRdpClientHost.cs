using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Windows.Forms;
using MSTSCLib;

namespace MsRdpEx.WinForms;

/// <summary>
/// Describes how <see cref="GeneratedRdpClientHost"/> activates an RDP ActiveX control.
/// </summary>
public sealed class GeneratedRdpClientHostOptions
{
    /// <summary>The CLSID of the RDP ActiveX control.</summary>
    public Guid ClassId { get; init; }

    /// <summary>
    /// Selects the RDP ActiveX DLL when <see cref="RdpExDll"/> is not specified.
    /// Supported values include <c>mstsc</c> and <c>msrdc</c>.
    /// </summary>
    public string AxName { get; init; } = "mstsc";

    /// <summary>
    /// Optional path to MsRdpEx.dll. When specified, activation uses its class factory.
    /// </summary>
    public string? RdpExDll { get; init; }
}

/// <summary>
/// Hosts a source-generated RDP ActiveX proxy in a WinForms control.
/// </summary>
public sealed partial class GeneratedRdpClientHost : AxHost
{
    private static readonly object LoadLock = new();
    private readonly string axName;
    private readonly string rdpExDll;

    public GeneratedRdpClientHost(Guid classId)
        : this(new GeneratedRdpClientHostOptions { ClassId = classId })
    {
    }

    public GeneratedRdpClientHost(GeneratedRdpClientHostOptions options)
        : base(ValidateOptions(options).ClassId.ToString("B"))
    {
        axName = options.AxName;
        rdpExDll = options.RdpExDll ?? string.Empty;
    }

    /// <summary>Gets the generated COM proxy associated with this hosted control.</summary>
    /// <typeparam name="T">A generated RDP interface implemented by the control.</typeparam>
    /// <exception cref="InvalidOperationException">The ActiveX control has not been created or does not implement <typeparamref name="T"/>.</exception>
    public T GetClient<T>() where T : class
    {
        if (ProxyObject.TryPack(base.GetOcx(), out T? client))
            return client;

        throw new InvalidOperationException($"The hosted RDP control does not implement {typeof(T).FullName}.");
    }

    protected override object CreateInstanceCore(Guid classId)
    {
        object instance = CreateInstance(classId);
        if (!ComWrappers.TryGetComInstance(instance, out nint unknown))
            throw new InvalidOperationException("Could not obtain an IUnknown pointer from the RDP ActiveX control.");

        try
        {
            return Marshal.GetObjectForIUnknown(unknown);
        }
        finally
        {
            Marshal.Release(unknown);
        }
    }

    private object CreateInstance(Guid classId)
    {
        lock (LoadLock)
        {
            string libraryPath = string.IsNullOrEmpty(rdpExDll)
                ? GetActiveXLibraryPath(axName)
                : rdpExDll;

            if (!string.IsNullOrEmpty(rdpExDll))
                Environment.SetEnvironmentVariable("MSRDPEX_AXNAME", axName);

            return ComClassFactory.CreateInstance(LibraryModule.Load(libraryPath), classId);
        }
    }

    private static GeneratedRdpClientHostOptions ValidateOptions(GeneratedRdpClientHostOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        if (options.ClassId == Guid.Empty)
            throw new ArgumentException("An RDP ActiveX CLSID is required.", nameof(options));
        if (string.IsNullOrWhiteSpace(options.AxName))
            throw new ArgumentException("An ActiveX DLL name is required.", nameof(options));

        return options;
    }

    private static string GetActiveXLibraryPath(string axName)
    {
        string mstscax = Environment.ExpandEnvironmentVariables("%SystemRoot%\\System32\\mstscax.dll");
        string rdclientax = Environment.ExpandEnvironmentVariables("%ProgramFiles%\\Remote Desktop\\rdclientax.dll");
        string localRdclientax = Environment.ExpandEnvironmentVariables("%LocalAppData%\\Apps\\Remote Desktop\\rdclientax.dll");

        if (!File.Exists(rdclientax) && File.Exists(localRdclientax))
            rdclientax = localRdclientax;

        if (axName.Equals("mstsc", StringComparison.OrdinalIgnoreCase) ||
            axName.Equals("mstscax", StringComparison.OrdinalIgnoreCase))
            return mstscax;

        if (axName.Equals("msrdc", StringComparison.OrdinalIgnoreCase) ||
            axName.Equals("rdclientax", StringComparison.OrdinalIgnoreCase))
            return rdclientax;

        return axName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) && File.Exists(axName)
            ? axName
            : mstscax;
    }

    private static class ComClassFactory
    {
        private static readonly Guid IidIUnknown = new("00000000-0000-0000-C000-000000000046");
        private static readonly Guid IidIClassFactory = new("00000001-0000-0000-C000-000000000046");

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DllGetClassObject(ref Guid classId, ref Guid interfaceId, out nint classFactory);

        internal static object CreateInstance(LibraryModule libraryModule, Guid classId)
        {
            var getClassObject = Marshal.GetDelegateForFunctionPointer<DllGetClassObject>(
                libraryModule.GetProcAddress("DllGetClassObject"));
            var classFactoryIid = IidIClassFactory;
            int hr = getClassObject(ref classId, ref classFactoryIid, out nint factoryPointer);
            Marshal.ThrowExceptionForHR(hr);

            unsafe
            {
                try
                {
                    var unknownIid = IidIUnknown;
                    nint instancePointer;
                    nint* vtable = *(nint**)factoryPointer;
                    var createInstance = (delegate* unmanaged[Stdcall]<nint, nint, Guid*, nint*, int>)vtable[3];
                    hr = createInstance(factoryPointer, 0, &unknownIid, &instancePointer);
                    Marshal.ThrowExceptionForHR(hr);

                    try
                    {
                        return Marshal.GetObjectForIUnknown(instancePointer);
                    }
                    finally
                    {
                        Marshal.Release(instancePointer);
                    }
                }
                finally
                {
                    Marshal.Release(factoryPointer);
                }
            }
        }
    }

    private sealed class LibraryModule
    {
        private readonly nint handle;
        private readonly string path;

        private LibraryModule(nint handle, string path)
        {
            this.handle = handle;
            this.path = path;
        }

        internal static LibraryModule Load(string path)
        {
            nint handle = LoadLibraryW(path);
            if (handle == 0)
                throw new Win32Exception(Marshal.GetLastWin32Error(), $"Cannot load library: {path}");

            return new LibraryModule(handle, path);
        }

        internal nint GetProcAddress(string name)
        {
            nint address = GetProcAddress(handle, name);
            if (address == 0)
                throw new Win32Exception(Marshal.GetLastWin32Error(), $"Cannot find export '{name}' in '{path}'.");

            return address;
        }

        [DllImport("kernel32.dll", EntryPoint = "LoadLibraryW", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        private static extern nint LoadLibraryW(string path);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern nint GetProcAddress(nint module, string name);
    }
}
