
using System;
using System.IO;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace AxMSTSCLib {

    internal static class ComHelper
    {
        private static Guid IID_IUnknown = new Guid(0x00000000, 0x0000, 0x0000, 0xc0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);
        private static Guid IID_IClassFactory = new Guid(0x00000001, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);

        private delegate int DllGetClassObject(
            ref Guid clsid,
            ref Guid iid,
            [Out, MarshalAs(UnmanagedType.Interface)] out IClassFactory classFactory);

        internal static object CreateInstance(LibraryModule libraryModule, Guid clsid)
        {
            object obj;
            var classFactory = GetClassFactory(libraryModule, clsid);
            classFactory.CreateInstance(null, ref IID_IUnknown, out obj);
            return obj;
        }

        internal static IClassFactory GetClassFactory(LibraryModule libraryModule, Guid clsid)
        {
            IntPtr ptr = libraryModule.GetProcAddress("DllGetClassObject");
            var callback = (DllGetClassObject) Marshal.GetDelegateForFunctionPointer(ptr, typeof(DllGetClassObject));

            IClassFactory classFactory;
            var hr = callback(ref clsid, ref IID_IClassFactory, out classFactory);

            if (hr != 0)
            {
                throw new Win32Exception(hr, "Cannot create class factory");
            }

            return classFactory;
        }
    }

    [Guid("00000001-0000-0000-c000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    internal interface IClassFactory
    {
        void CreateInstance(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnkOuter,
            ref Guid riid,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppvObject);
        void LockServer(bool fLock);
    }

    internal class LibraryModule : IDisposable
    {
        private readonly IntPtr _handle;
        private readonly string _filePath;

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);

        public static LibraryModule LoadModule(string filePath)
        {
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

        ~LibraryModule()
        {
            if (_handle != IntPtr.Zero)
            {
                FreeLibrary(_handle);
            }
        }

        public void Dispose()
        {
            if (_handle != IntPtr.Zero)
            {
                FreeLibrary(_handle);
            }

            GC.SuppressFinalize(this);
        }

        public IntPtr GetProcAddress(string name)
        {
            IntPtr ptr = GetProcAddress(_handle, "DllGetClassObject");

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
    }

    public class AxHostEx : System.Windows.Forms.AxHost
    {
        public static string RdpGetAxDllPath(string axName)
        {
            string mstscax = Environment.ExpandEnvironmentVariables("%SystemRoot%\\System32\\mstscax.dll");
            string rdclientax = Environment.ExpandEnvironmentVariables("%ProgramFiles%\\Remote Desktop\\rdclientax.dll");

            if (axName.Equals("latest"))
            {
                if (File.Exists(rdclientax))
                {
                    return rdclientax;
                }
                else
                {
                    return mstscax;
                }
            }

            if (axName.Equals("mstscax") || axName.Equals("mstscax.dll") ||
                axName.Equals("mstsc") || axName.Equals("mstsc.exe"))
            {
                return mstscax;
            }

            if (axName.Equals("rdclientax") || axName.Equals("rdclientax.dll") ||
                axName.Equals("msrdc") || axName.Equals("msrdc.exe"))
            {
                return rdclientax;
            }

            if (axName.EndsWith(".dll") && File.Exists(axName))
            {
                return axName;
            }

            return mstscax;
        }

        public static object RdpGetClassObject(Guid clsid, string axName)
        {
            object obj = null;
            string libraryName = RdpGetAxDllPath(axName);
            LibraryModule libraryModule = LibraryModule.LoadModule(libraryName);
            obj = ComHelper.CreateInstance(libraryModule, clsid);
            return obj;
        }

        public static object RdpCreateInstance(Guid clsid)
        {
            return RdpGetClassObject(clsid, "latest");
        }

        protected override object CreateInstanceCore(Guid clsid)
        {
            return RdpCreateInstance(clsid);
        }

        public AxHostEx(string clsid): base(clsid) { }
    }
}
