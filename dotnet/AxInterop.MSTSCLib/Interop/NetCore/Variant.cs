using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using MSTSCLib;

#nullable enable

namespace MsRdpEx.Interop
{
    [Flags]
    public enum VariantType : ushort // typedef unsigned short VARTYPE
    {
        Empty = 0, // VT_EMPTY = 0,
        DBNull = 1, // VT_NULL = 1,
        Int16 = 2, // VT_I2 = 2,
        Int32 = 3, // VT_I4 = 3,
        Float32 = 4, // VT_R4 = 4,
        Float64 = 5, // VT_R8 = 5,
        Currency = 6, // VT_CY = 6,
        DateTime = 7, // VT_DATE = 7,
        BinaryString = 8, // VT_BSTR = 8,
        IDispatch = 9, // VT_DISPATCH = 9,
        Error = 10, // VT_ERROR = 10,
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
        OtherInt32 = 22, // VT_INT = 22,
        OtherUInt32 = 23, // VT_UINT = 23,
        // VT_VOID = 24,
        // VT_HRESULT = 25,
        // VT_PTR = 26,
        // VT_SAFEARRAY = 27,
        // VT_CARRAY = 28,
        // VT_USERDEFINED = 29,
        // VT_LPSTR = 30,
        // VT_LPWSTR = 31,
        // VT_RECORD = 36,
        IntPtr = 37, // VT_IntPtr = 37,
        UIntPtr = 38, // VT_UIntPtr = 38,
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

        // VT_VECTOR = 0x1000,
        ArrayModifier = 0x2000, // VT_ARRAY = 0x2000,
        ByRefModifier = 0x4000, // VT_BYREF = 0x4000,
        // VT_RESERVED = 0x8000,

        // VT_ILLEGAL = 0xffff,
        // VT_ILLEGALMASKED = 0xfff,
        // VT_TYPEMASK = 0xfff,
    }

    public struct NativeVariant
    {
        public VariantType Type;
        public ushort Header1;
        public ushort Header2;
        public ushort Header3;
        public nint Content1;
        public nint Content2;

        public NativeVariant(VariantType Type)
        {
            this.Type = Type;
        }
    }

    [CustomMarshaller(typeof(object), MarshalMode.Default, typeof(VariantMarshaller))]
    public static unsafe class VariantMarshaller
    {
        public static object? ConvertToManaged(NativeVariant data)
        {
            switch (data.Type)
            {
                case VariantType.Empty: return null;
                case VariantType.DBNull: return DBNull.Value;
                case VariantType.Boolean: return (bool)Unsafe.As<nint, VariantBool>(ref data.Content1);
                case VariantType.Int8: return Unsafe.As<nint, sbyte>(ref data.Content1);
                case VariantType.Int16: return Unsafe.As<nint, short>(ref data.Content1);
                case VariantType.Int32: return Unsafe.As<nint, int>(ref data.Content1);
                case VariantType.Int64: return Unsafe.As<nint, long>(ref data.Content1);
                case VariantType.IntPtr: return Unsafe.As<nint, nint>(ref data.Content1);
                case VariantType.UInt8: return  Unsafe.As<nint, byte>(ref data.Content1);
                case VariantType.UInt16: return Unsafe.As<nint, ushort>(ref data.Content1);
                case VariantType.UInt32: return Unsafe.As<nint, uint>(ref data.Content1);
                case VariantType.UInt64: return Unsafe.As<nint, ulong>(ref data.Content1);
                case VariantType.UIntPtr: return Unsafe.As<nint, nuint>(ref data.Content1);
                case VariantType.Float32: return Unsafe.As<nint, float>(ref data.Content1);
                case VariantType.Float64: return Unsafe.As<nint, double>(ref data.Content1);
                case VariantType.Currency: return decimal.FromOACurrency(Unsafe.As<nint, long>(ref data.Content1));
                case VariantType.DateTime: return DateTime.FromOADate(Unsafe.As<nint, double>(ref data.Content1));
                case VariantType.OtherInt32: goto case VariantType.Int32;
                case VariantType.OtherUInt32: goto case VariantType.UInt32;

                case VariantType.Decimal:
                    // .NET decimal has the same layout as VARIANT except that the type field is left empty
                    var variantWithoutType = data;
                    variantWithoutType.Type = VariantType.Empty;
                    return Unsafe.As<NativeVariant, decimal>(ref variantWithoutType);

                case VariantType.BinaryString:
                    // If we have a NULL pointer .NET would convert this to a NULL string reference
                    if (data.Content1 == 0)
                        return null;

                    var length = ((uint*)data.Content1)[-1];
                    if (length == 0)
                        return string.Empty;

                    return Marshal.PtrToStringBSTR(data.Content1);

                case VariantType.IUnknown:
                case VariantType.IDispatch:
                    // If we have a NULL pointer .NET would convert this to a NULL object reference
                    if (data.Content1 == 0)
                        return null;

                    return ProxyObject.Pack(ComInterfaceMarshaller<object>.ConvertToManaged((void*)data.Content1));

                case VariantType.ByRefModifier | VariantType.Error: return *(int*)data.Content1;
                case VariantType.ByRefModifier | VariantType.Boolean: return *(short*)data.Content1;
                case VariantType.ByRefModifier | VariantType.Int8: return *(sbyte*)data.Content1;
                case VariantType.ByRefModifier | VariantType.Int16: return *(short*)data.Content1;
                case VariantType.ByRefModifier | VariantType.Int32: return *(int*)data.Content1;
                case VariantType.ByRefModifier | VariantType.Int64: return *(long*)data.Content1;
                case VariantType.ByRefModifier | VariantType.IntPtr: return *(nint*)data.Content1;
                case VariantType.ByRefModifier | VariantType.UInt8: return *(byte*)data.Content1;
                case VariantType.ByRefModifier | VariantType.UInt16: return *(ushort*)data.Content1;
                case VariantType.ByRefModifier | VariantType.UInt32: return *(uint*)data.Content1;
                case VariantType.ByRefModifier | VariantType.UInt64: return *(ulong*)data.Content1;
                case VariantType.ByRefModifier | VariantType.UIntPtr: return *(nuint*)data.Content1;
                case VariantType.ByRefModifier | VariantType.Float32: return *(float*)data.Content1;
                case VariantType.ByRefModifier | VariantType.Float64: return *(double*)data.Content1;
                case VariantType.ByRefModifier | VariantType.Currency: return *(long*)data.Content1;
                case VariantType.ByRefModifier | VariantType.DateTime: return *(double*)data.Content1;
                case VariantType.ByRefModifier | VariantType.OtherInt32: goto case VariantType.ByRefModifier | VariantType.Int32;
                case VariantType.ByRefModifier | VariantType.OtherUInt32: goto case VariantType.ByRefModifier | VariantType.UInt32;

                case VariantType.ByRefModifier | VariantType.Variant:
                    {
                        var pVariant = (NativeVariant*)data.Content1;
                        if (pVariant == null)
                            return null;

                        if ((pVariant->Type & VariantType.ByRefModifier) != 0)
                            throw new InvalidOperationException("Double indirection is not allowed.");

                        return ConvertToManaged(*pVariant);
                    }

                default:
                    throw new NotSupportedException($"Variant Type {(ushort)data.Type} is not supported.");
            }
        }

        public static NativeVariant ConvertToUnmanaged(object? data)
        {
            NativeVariant result = default;
            switch (data)
            {
                case null:
                    result.Type = VariantType.Empty;
                    break;

                case DBNull _:
                    result.Type = VariantType.DBNull;
                    break;

                case ErrorWrapper content:
                    result.Type = VariantType.Error;
                    Unsafe.As<nint, int>(ref result.Content1) = content.ErrorCode;
                    break;

                case bool content:
                    result.Type = VariantType.Boolean;
                    Unsafe.As<nint, VariantBool>(ref result.Content1) = content;
                    break;

                case sbyte content:
                    result.Type = VariantType.Int8;
                    Unsafe.As<nint, sbyte>(ref result.Content1) = content;
                    break;

                case short content:
                    result.Type = VariantType.Int16;
                    Unsafe.As<nint, short>(ref result.Content1) = content;
                    break;

                case int content:
                    result.Type = VariantType.Int32;
                    Unsafe.As<nint, int>(ref result.Content1) = content;
                    break;

                case long content:
                    result.Type = VariantType.Int64;
                    Unsafe.As<nint, long>(ref result.Content1) = content;
                    break;

                case nint content:
                    result.Type = VariantType.IntPtr;
                    Unsafe.As<nint, nint>(ref result.Content1) = content;
                    break;

                case byte content:
                    result.Type = VariantType.UInt8;
                    Unsafe.As<nint, byte>(ref result.Content1) = content;
                    break;

                case ushort content:
                    result.Type = VariantType.UInt16;
                    Unsafe.As<nint, ushort>(ref result.Content1) = content;
                    break;

                case uint content:
                    result.Type = VariantType.UInt32;
                    Unsafe.As<nint, uint>(ref result.Content1) = content;
                    break;

                case ulong content:
                    result.Type = VariantType.UInt64;
                    Unsafe.As<nint, ulong>(ref result.Content1) = content;
                    break;

                case nuint content:
                    result.Type = VariantType.UIntPtr;
                    Unsafe.As<nint, nuint>(ref result.Content1) = content;
                    break;

                case float content:
                    result.Type = VariantType.Float32;
                    Unsafe.As<nint, float>(ref result.Content1) = content;
                    break;

                case double content:
                    result.Type = VariantType.Float64;
                    Unsafe.As<nint, double>(ref result.Content1) = content;
                    break;


                case decimal content:
                    // Decimal overwrites the whole thing, need to set the type afterwards.
                    Unsafe.As<NativeVariant, decimal>(ref result) = content;
                    result.Type = VariantType.Decimal;
                    break;

                case CurrencyWrapper content:
                    result.Type = VariantType.Currency;
                    Unsafe.As<nint, long>(ref result.Content1) = decimal.ToOACurrency(content.WrappedObject);
                    break;

                case string content:
                    result.Type = VariantType.BinaryString;
                    result.Content1 = BinaryString.Marshaller.ConvertToUnmanaged(content);
                    break;

                case BinaryString content:
                    result.Type = VariantType.BinaryString;
                    result.Content1 = BinaryString.Marshaller.ConvertToUnmanaged(content);
                    break;

                case BStrWrapper content:
                    result.Type = content.WrappedObject is null ? VariantType.Empty : VariantType.BinaryString;
                    result.Content1 = BinaryString.Marshaller.ConvertToUnmanaged(content.WrappedObject);
                    break;

                default:
                    throw new NotSupportedException();
            }
            return result;
        }

        public static void Free(NativeVariant data)
        {
            if (data.Type != VariantType.Empty)
                VariantInterop.VariantClear(&data);
        }
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
        public static partial int VariantClear(NativeVariant* value);
#else
        [DllImport("oleaut32", EntryPoint = "VariantClear", SetLastError = false, CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern int VariantClear(NativeVariant* value);
#endif
    }
}
