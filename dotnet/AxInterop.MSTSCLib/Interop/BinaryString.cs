using System;
using System.Runtime.InteropServices;

namespace MsRdpEx.Interop
{
    public unsafe sealed class BinaryStringMarshaler : ICustomMarshaler
    {
        private static readonly BinaryStringMarshaler SharedInstance = new BinaryStringMarshaler();
        public static ICustomMarshaler GetInstance(string cookie) => SharedInstance;
        public object MarshalNativeToManaged(IntPtr pointer) => BinaryString.Marshaller.ConvertToManaged(pointer);
        public void CleanUpManagedData(object ManagedObj) { }
        public IntPtr MarshalManagedToNative(object ManagedObj) => BinaryString.Marshaller.ConvertToUnmanaged((BinaryString)ManagedObj);
        public void CleanUpNativeData(IntPtr pNativeData) => BinaryString.Marshaller.Free(pNativeData);
        public int GetNativeDataSize() => throw new NotSupportedException();
    }
}
