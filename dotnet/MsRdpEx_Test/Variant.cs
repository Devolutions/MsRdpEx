using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using MsRdpEx.Interop;

namespace MsRdpEx.Tests
{
    public unsafe class VariantTests
    {
        public static IEnumerable<object?[]> GetVariantValueTestData()
        {
            return GetTestData().Select(x => new object?[] { x.Item1, x.Item2, x.Item3 });

            static IEnumerable<(object?, VariantType, byte[])> GetTestData()
            {
                yield return (null, VariantType.Empty, []);
                yield return (false, VariantType.Boolean, [0x00, 0x00]);
                yield return (true, VariantType.Boolean, [0xFF, 0xFF]);
                yield return (DBNull.Value, VariantType.DBNull, []);
                yield return ((sbyte)0x12, VariantType.Int8, [0x12]);
                yield return ((byte)0x87, VariantType.UInt8, [0x87]);
                yield return ((short)0x1234, VariantType.Int16, [0x34, 0x12]);
                yield return ((ushort)0x8765, VariantType.UInt16, [0x65, 0x87]);
                yield return ((int)0x1234_5678, VariantType.Int32, [0x78, 0x56, 0x34, 0x12]);
                yield return ((uint)0x8765_4321, VariantType.UInt32, [0x21, 0x43, 0x65, 0x87]);
                yield return ((long)0x1234_5678_1234_5678, VariantType.Int64, [0x78, 0x56, 0x34, 0x12, 0x78, 0x56, 0x34, 0x12]);
                yield return ((ulong)0x8765_4321_8765_4321, VariantType.UInt64, [0x21, 0x43, 0x65, 0x87, 0x21, 0x43, 0x65, 0x87]);
                yield return ((float)1.2f, VariantType.Float32, BitConverter.GetBytes((float)1.2f));
                yield return ((double)1.2d, VariantType.Float64, BitConverter.GetBytes((double)1.2d));
                yield return ((decimal)1.2m, VariantType.Decimal, [0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00]);
            }
        }

        private static bool IsLargeVariant(VariantType vt) => vt == VariantType.Decimal;

        private struct ManagedBoxedVariant
        {
            [MarshalAs(UnmanagedType.Struct)]
            public object? Value;
        }

        [Theory]
        [MemberData(nameof(GetVariantValueTestData))]
        public void ClassicMarshalTest(object? value, VariantType expectedType, byte[] expectedData)
        {
            var managed = new ManagedBoxedVariant { Value = value };
            var native = new NativeVariant();
            Marshal.StructureToPtr(managed, (nint)(&native), false);
            try
            {
                Assert.Equal(expectedType, native.Type);

                if (IsLargeVariant(native.Type))
                {
                    var contentSpan = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref native, 1))[sizeof(VariantType)..];
                    Assert.True(expectedData.Length <= contentSpan.Length);
                    Assert.Equal(expectedData, contentSpan[..expectedData.Length].ToArray());
                }
                else
                {
                    var contentSpan = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref native.Content1, 2));
                    Assert.True(expectedData.Length <= contentSpan.Length);
                    Assert.Equal(expectedData, contentSpan[..expectedData.Length].ToArray());
                }
            }
            finally
            {
                Marshal.DestroyStructure<ManagedBoxedVariant>((nint)(&native));
            }
        }

#if NET9_0_OR_GREATER
        [Theory]
        [MemberData(nameof(GetVariantValueTestData))]
        public void ModernMarshalTest(object? value, VariantType expectedType, byte[] expectedData)
        {
            var native = ComVariantMarshaller.ConvertToUnmanaged(value);
            try
            {
                Assert.Equal(expectedType, native.VarType);

                if (IsLargeVariant(native.VarType))
                {
                    var contentSpan = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref native, 1))[sizeof(VariantType)..];
                    Assert.True(expectedData.Length <= contentSpan.Length);
                    Assert.Equal(expectedData, contentSpan[..expectedData.Length].ToArray());
                }
                else
                {
                    var contentSpan = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref native.GetRawDataRef<nint>(), 2));
                    Assert.True(expectedData.Length <= contentSpan.Length);
                    Assert.Equal(expectedData, contentSpan[..expectedData.Length].ToArray());
                }
            }
            finally
            {
                ComVariantMarshaller.Free(native);
            }
        }
#endif
    }
}
