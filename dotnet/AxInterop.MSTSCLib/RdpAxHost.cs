
using System;
using System.IO;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Reflection;
using MSTSCLib;

[assembly: ComVisible(false)]

namespace AxMSTSCLib {

    internal static partial class ComHelper
    {
        private static Guid IID_IUnknown = new Guid(0x00000000, 0x0000, 0x0000, 0xc0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);
        private static Guid IID_IClassFactory = new Guid(0x00000001, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);

        private delegate int DllGetClassObject(
            ref Guid clsid,
            ref Guid iid,
            /*[Out, MarshalAs(UnmanagedType.Interface)] out IClassFactory*/ out nint classFactory);

        internal static object CreateInstance(LibraryModule libraryModule, Guid clsid)
        {
            object obj;
            var classFactory = GetClassFactory(libraryModule, clsid);
            classFactory.CreateInstance(null, ref IID_IUnknown, out obj);
            return obj;
        }

        internal static unsafe IClassFactory GetClassFactory(LibraryModule libraryModule, Guid clsid)
        {
            IntPtr ptr = libraryModule.GetProcAddress("DllGetClassObject");
            var callback = (DllGetClassObject) Marshal.GetDelegateForFunctionPointer(ptr, typeof(DllGetClassObject));

            nint classFactory;
            var hr = callback(ref clsid, ref IID_IClassFactory, out classFactory);

            if (hr != 0)
            {
                throw new Win32Exception(hr, "Cannot create class factory");
            }

#if NET8_0_OR_GREATER
            try { return ComInterfaceMarshaller<IClassFactory>.ConvertToManaged((void*)classFactory); }
            finally { ComInterfaceMarshaller<IClassFactory>.Free((void*)classFactory); }
#else
            try { return (IClassFactory)Marshal.GetObjectForIUnknown(classFactory); }
            finally { Marshal.Release(classFactory); }
#endif
        }
    }

    [Guid("00000001-0000-0000-c000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
#if NET8_0_OR_GREATER
    [GeneratedComInterface]
#else
    [ComImport]
#endif
    internal partial interface IClassFactory
    {
        void CreateInstance(
            [MarshalAs(UnmanagedType.Interface)] object pUnkOuter,
            ref Guid riid,
            [MarshalAs(UnmanagedType.Interface)] out object ppvObject);
        void LockServer([MarshalAs(UnmanagedType.Bool)] bool fLock);
    }

    internal class LibraryModule
    {
        private readonly IntPtr _handle;
        private readonly string _filePath;

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        public static LibraryModule LoadModule(string filePath)
        {
            // Since LoadLibrary is ref-counted; we don't have an explicit need to call FreeLibrary
            var libraryModule = new LibraryModule(LoadLibrary(filePath), filePath);

            if (libraryModule._handle == IntPtr.Zero)
            {
                int error = Marshal.GetLastWin32Error();
                throw new Win32Exception(error, "Cannot load library: " + filePath);
            }

            return libraryModule;
        }

        private LibraryModule(IntPtr handle, string filePath)
        {
            _filePath = filePath;
            _handle = handle;
        }

        public IntPtr GetProcAddress(string name)
        {
            IntPtr ptr = GetProcAddress(_handle, name);

            if (ptr == IntPtr.Zero)
            {
                int error = Marshal.GetLastWin32Error();
                string message = string.Format("Cannot find proc {0} in {1}", name, _filePath);
                throw new Win32Exception(error, message);
            }

            return ptr;
        }

        public string FilePath
        {
            get { return _filePath; }
        }

        public IntPtr Module
        {
            get { return _handle; }
        }
    }

    public class AxHostEx : System.Windows.Forms.AxHost
    {
        public string axName = "mstsc";
        public string rdpExDll = string.Empty;
        private static object loadLock = new object();

        public static string RdpGetAxDllPath(string axName)
        {
            string mstscax = Environment.ExpandEnvironmentVariables("%SystemRoot%\\System32\\mstscax.dll");
            string rdclientax = Environment.ExpandEnvironmentVariables("%ProgramFiles%\\Remote Desktop\\rdclientax.dll");
            string rdclientax_local = Environment.ExpandEnvironmentVariables("%LocalAppData%\\Apps\\Remote Desktop\\rdclientax.dll");

            if (!File.Exists(rdclientax) && File.Exists(rdclientax_local))
            {
                rdclientax = rdclientax_local;
            }

            if (axName.Equals("mstsc") || axName.Equals("mstscax"))
            {
                return mstscax;
            }

            if (axName.Equals("msrdc") || axName.Equals("rdclientax"))
            {
                return rdclientax;
            }

            if (axName.EndsWith(".dll") && File.Exists(axName))
            {
                return axName;
            }

            return mstscax;
        }

        public static object RdpGetClassObject(Guid clsid, string axName, string rdpExDll)
        {
            object obj = null;

            lock (loadLock)
            {
                if (!string.IsNullOrEmpty(rdpExDll))
                {
                    Environment.SetEnvironmentVariable("MSRDPEX_AXNAME", axName);
                    LibraryModule libraryModule = LibraryModule.LoadModule(rdpExDll);
                    obj = ComHelper.CreateInstance(libraryModule, clsid);
                }
                else
                {
                    string axDllPath = RdpGetAxDllPath(axName);
                    LibraryModule libraryModule = LibraryModule.LoadModule(axDllPath);
                    obj = ComHelper.CreateInstance(libraryModule, clsid);
                }
            }

            return obj;
        }

        public object RdpCreateInstance(Guid clsid)
        {
            return RdpGetClassObject(clsid, this.axName, this.rdpExDll);
        }

        protected override object CreateInstanceCore(Guid clsid)
        {
#if NET8_0_OR_GREATER
            if (ComWrappers.TryGetComInstance(RdpCreateInstance(clsid), out var pUnkPtr))
            {
                try { return Marshal.GetObjectForIUnknown(pUnkPtr); }
                finally { Marshal.Release(pUnkPtr); }
            }

            return new InvalidOperationException("Could not obtain IUnknown pointer from COM object.");
#else
            return RdpCreateInstance(clsid);
#endif
        }

        public AxHostEx(string clsid): base(clsid) { }

        public new object GetOcx()
        {
            return ProxyObject.Pack(base.GetOcx());
        }
    }
}
