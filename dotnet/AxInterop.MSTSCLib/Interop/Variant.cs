using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

#nullable enable

namespace MsRdpEx.Interop
{
    [Flags]
    public enum VariantType : ushort // typedef unsigned short VARTYPE
    {
        Empty = 0, // VT_EMPTY = 0,
        Null = 1, // VT_NULL = 1,
        Int16 = 2, // VT_I2 = 2,
        Int32 = 3, // VT_I4 = 3,
        Float32 = 4, // VT_R4 = 4,
        Float64 = 5, // VT_R8 = 5,
        Currency = 6, // VT_CY = 6,
        DateTime = 7, // VT_DATE = 7,
        BinaryString = 8, // VT_BSTR = 8,
        IDispatch = 9, // VT_DISPATCH = 9,
        // VT_ERROR = 10,
        Boolean = 11, // VT_BOOL = 11,
        Variant = 12, // VT_VARIANT = 12,
        IUnknown = 13, // VT_UNKNOWN = 13,
        Decimal = 14, // VT_DECIMAL = 14,
        Int8 = 16, // VT_I1 = 16,
        UInt8 = 17, // VT_UI1 = 17,
        UInt16 = 18, // VT_UI2 = 18,
        UInt32 = 19, // VT_UI4 = 19,
        Int64 = 20, // VT_I8 = 20,
        UInt64 = 21, // VT_UI8 = 21,
        // VT_INT = 22,
        // VT_UINT = 23,
        // VT_VOID = 24,
        // VT_HRESULT = 25,
        // VT_PTR = 26,
        // VT_SAFEARRAY = 27,
        // VT_CARRAY = 28,
        // VT_USERDEFINED = 29,
        AnsiString = 30, // VT_LPSTR = 30,
        UnicodeString = 31, // VT_LPWSTR = 31,
        // VT_RECORD = 36,
        // VT_INT_PTR = 37,
        // VT_UINT_PTR = 38,
        // VT_FILETIME = 64,
        // VT_BLOB = 65,
        // VT_STREAM = 66,
        // VT_STORAGE = 67,
        // VT_STREAMED_OBJECT = 68,
        // VT_STORED_OBJECT = 69,
        // VT_BLOB_OBJECT = 70,
        // VT_CF = 71,
        // VT_CLSID = 72,
        // VT_VERSIONED_STREAM = 73,
        // VT_BSTR_BLOB = 0xfff,

        VectorModifier = 0x1000, // VT_VECTOR = 0x1000,
        ArrayModifier = 0x2000, // VT_ARRAY = 0x2000,
        ByRefModifier = 0x4000, // VT_BYREF = 0x4000,
        // VT_RESERVED = 0x8000,

        // VT_ILLEGAL = 0xffff,
        // VT_ILLEGALMASKED = 0xfff,
        // VT_TYPEMASK = 0xfff,
    }

    public struct Variant
    {
        public VariantType Type;
        public ushort Header1;
        public ushort Header2;
        public ushort Header3;
        public nint Content1;
        public nint Content2;
    }

    public struct VariantBool
    {
        public static implicit operator VariantBool(bool value) => new VariantBool(value);
        public static implicit operator bool(VariantBool value) => value.Value != 0;

        public static VariantBool True => new(TrueValue);
        public static VariantBool False => new(FalseValue);

        public const short TrueValue = -1;
        public const short FalseValue = 0;

        public short Value;

        public VariantBool(short value) => Value = value;
        public VariantBool(bool value) => Value = value ? TrueValue : FalseValue;
    }

    internal static unsafe partial class VariantInterop
    {
#if NET8_0_OR_GREATER
        [LibraryImport("oleaut32", EntryPoint = "VariantClear", SetLastError = false, StringMarshalling = StringMarshalling.Utf16)]
        public static partial int VariantClear(Variant* value);
#else
        [DllImport("oleaut32", EntryPoint = "VariantClear", SetLastError = false, CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern int VariantClear(Variant* value);
#endif
    }
}
