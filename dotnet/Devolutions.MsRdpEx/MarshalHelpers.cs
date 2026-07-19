using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;

[assembly: ComVisible(false)]

namespace MsRdpEx
{
    public class MarshalHelpers
    {
        // Replacement for native strlen() function
        public static unsafe int PtrStringLength(IntPtr ptr)
        {
            byte* p = (byte*)ptr;
            int length = 0;

            while (*p != 0)
            {
                p++;
                length++;
            }

            return length;
        }

        // Replacement for Marshal.PtrToStringUTF8
        public static string PtrToStringUTF8(IntPtr ptr)
        {
            int length = PtrStringLength(ptr);
            byte[] buffer = new byte[length];
            Marshal.Copy(ptr, buffer, 0, length);
            return System.Text.Encoding.UTF8.GetString(buffer);
        }

        // Replacement for Marshal.StringToCoTaskMemUTF8
        public static IntPtr StringToCoTaskMemUTF8(string str)
        {
            if (str == null)
            {
                return IntPtr.Zero;
            }

            byte[] utf8Bytes = System.Text.Encoding.UTF8.GetBytes(str);
            IntPtr unmanagedMemory = Marshal.AllocCoTaskMem(utf8Bytes.Length + 1);
            Marshal.Copy(utf8Bytes, 0, unmanagedMemory, utf8Bytes.Length);
            Marshal.WriteByte(unmanagedMemory, utf8Bytes.Length, 0);

            return unmanagedMemory;
        }

        // Replacement for [MarshalAs(UnmanagedType.LPStr)]
        // [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(MarshalHelpers.LPUTF8Str))]

#if NET8_0_OR_GREATER
        [CustomMarshaller(typeof(string), MarshalMode.Default, typeof(LPUTF8Str))]
        public static class LPUTF8Str
        {
            public static string ConvertToManaged(IntPtr pNativeData)
            {
                return MarshalHelpers.PtrToStringUTF8(pNativeData);
            }

            public static IntPtr ConvertToUnmanaged(string ManagedObj)
            {
                return MarshalHelpers.StringToCoTaskMemUTF8(ManagedObj);
            }

            public static void Free(IntPtr pNativeData)
            {
                Marshal.FreeCoTaskMem(pNativeData);
            }
        }
#else
        public class LPUTF8Str : ICustomMarshaler
        {
            public void CleanUpManagedData(object ManagedObj)
            {

            }

            public void CleanUpNativeData(IntPtr pNativeData)
            {
                Marshal.FreeCoTaskMem(pNativeData);
            }

            public int GetNativeDataSize()
            {
                throw new NotImplementedException();
            }

            public IntPtr MarshalManagedToNative(object ManagedObj)
            {
                return MarshalHelpers.StringToCoTaskMemUTF8((string)ManagedObj);
            }

            public object MarshalNativeToManaged(IntPtr pNativeData)
            {
                return MarshalHelpers.PtrToStringUTF8(pNativeData);
            }

            public static ICustomMarshaler GetInstance(string cookie) => new LPUTF8Str();
        }
#endif
    }
}
