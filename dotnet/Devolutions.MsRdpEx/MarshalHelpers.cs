using System;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MsRdpEx
{
    public class MarshalHelpers
    {
        // Replacement for Marshal.PtrToStringUTF8
        public static string PtrToStringUTF8(IntPtr ptr)
        {
            int length = 0;
            while (Marshal.ReadByte(ptr, length) != 0)
            {
                length++;
            }

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
    }
}
