using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Threading;

#nullable enable

namespace MsRdpEx.Interop
{
    /// <summary>Contains either text or binary data.</summary>
    /// <remarks>Data is stored in immutable managed form and does not need to be disposed.</remarks>
    [NativeMarshalling(typeof(Marshaller))]
    public sealed class BinaryString : IEquatable<BinaryString>
    {
        [CustomMarshaller(typeof(BinaryString), MarshalMode.Default, typeof(Marshaller))]
        public static unsafe class Marshaller
        {
            public static BinaryString? ConvertToManaged(nint data)
            {
                if (data == 0)
                    return null;

                var length = ((int*)data)[-1];
                if (length < 0)
                    throw new OutOfMemoryException();

                if (length == 0)
                    return Empty;

                // Prefer storing as string for even lengths since that is the common case and even if we want
                // binary data we are more likely to process it as a span than asking for an ImmutableArray.
                if ((length & 1) == 0)
                    return new BinaryString(new ReadOnlySpan<char>((char*)data, length / 2));

                // If the length is odd we store it as a byte array so we don't lose the last byte.
                return new BinaryString(new ReadOnlySpan<byte>((byte*)data, length));
            }

            public static nint ConvertToUnmanaged(BinaryString? data)
            {
                if (data is null)
                    return 0;

                nint result;
                if (data.mTextData is not null)
                {
                    fixed (char* pTextData = data.mTextData)
                        result = BinaryStringInterop.AllocateTextBuffer(pTextData, data.mTextData.Length);
                }
                else if (!data.mByteData.IsDefault)
                {
                    fixed (byte* pByteData = data.mByteData.AsSpan())
                        result = BinaryStringInterop.AllocateByteBuffer(pByteData, data.mByteData.Length);
                }
                else throw new UnreachableException();

                if (result == 0)
                    throw new OutOfMemoryException();

                return result;
            }

            public static void Free(nint data)
            {
                if (data != 0)
                    Marshal.FreeBSTR(data);
            }
        }

        public static implicit operator string?(BinaryString? bstr) => bstr?.ToString();
        public static implicit operator ImmutableArray<byte>(BinaryString? bstr) => bstr is null ? default : bstr.ToImmutableArray();
        public static implicit operator ReadOnlySpan<char>(BinaryString? bstr) => bstr is null ? default : bstr.AsTextSpan();
        public static implicit operator ReadOnlySpan<byte>(BinaryString? bstr) => bstr is null ? default : bstr.AsByteSpan();

        public static implicit operator BinaryString?(string? text) => text is null ? null : new(text);
        public static implicit operator BinaryString?(ImmutableArray<byte> data) => data.IsDefault ? null : new(data);
        public static implicit operator BinaryString?(BinaryStringRef bstr) => bstr.IsNull ? null : new(bstr.AsByteSpan());
        public static implicit operator BinaryString?(char[] text) => text is null ? null : new(text);
        public static implicit operator BinaryString?(byte[] data) => data is null ? null : new(data);
        public static implicit operator BinaryString(Span<char> text) => new(text);
        public static implicit operator BinaryString(Span<byte> data) => new(data);
        public static implicit operator BinaryString(ReadOnlySpan<char> text) => new(text);
        public static implicit operator BinaryString(ReadOnlySpan<byte> data) => new(data);

        public static BinaryString Empty { get; } = new BinaryString("", []);

        public static bool IsNullOrEmpty(BinaryString? bstr) => bstr is null || bstr.IsEmpty;

        private string? mTextData;
        private ImmutableArray<byte> mByteData;

        private BinaryString(string? text, ImmutableArray<byte> data)
        {
            if (text is null && data.IsDefault)
                throw new ArgumentException("A null string must be represented by a null BinaryString.");

            mTextData = text;
            mByteData = data;
        }

        public BinaryString(string text)
        {
            mTextData = text ?? throw new ArgumentNullException(nameof(text), "A null string must be represented by a null BinaryString.");
        }

        public BinaryString(ReadOnlySpan<char> text)
        {
            mTextData = new string(text);
        }

        public BinaryString(ImmutableArray<byte> data)
        {
            if (data.IsDefault)
                throw new ArgumentNullException(nameof(data), "A null string must be represented by a null BinaryString.");

            mByteData = data;
        }

        public BinaryString(ReadOnlySpan<byte> data)
        {
            mByteData = data.ToImmutableArray();
        }

        public bool IsEmpty => (mTextData?.Length ?? mByteData.Length) == 0;
        public int TextLength => mTextData?.Length ?? (mByteData.Length / 2);
        public int ByteLength => mByteData.IsDefault ? mTextData!.Length * 2 : mByteData.Length;

        private ReadOnlySpan<byte> GetByteSpanSlow()
        {
            Debug.Assert(mTextData is not null);
            return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<char, byte>(ref Unsafe.AsRef(in mTextData.GetPinnableReference())), mTextData.Length * 2);
        }

        public ReadOnlySpan<byte> AsByteSpan() => mByteData.IsDefault ? GetByteSpanSlow() : mByteData.AsSpan();

        private ImmutableArray<byte> GetImmutableArraySlow()
        {
            var data = GetByteSpanSlow().ToImmutableArray();
            var other = ImmutableInterlocked.InterlockedCompareExchange(ref mByteData, data, default);
            return other.IsDefault ? data : other;
        }

        public ImmutableArray<byte> ToImmutableArray() => mByteData.IsDefault ? GetImmutableArraySlow() : mByteData;

        private ReadOnlySpan<char> GetTextSpanSlow()
        {
            Debug.Assert(!mByteData.IsDefault);
            return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<byte, char>(ref MemoryMarshal.GetReference(mByteData.AsSpan())), mByteData.Length / 2);
        }

        public ReadOnlySpan<char> AsTextSpan() => mTextData is null ? GetTextSpanSlow() : mTextData;

        private string GetStringSlow()
        {
            var text = new string(GetTextSpanSlow());
            return Interlocked.CompareExchange(ref mTextData, text, null) ?? text;
        }

        public override string ToString() => mTextData is null ? GetStringSlow() : mTextData;

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.AddBytes(AsByteSpan());
            return hash.ToHashCode();
        }

        public override bool Equals(object? obj) => Equals(obj as BinaryString);
        public bool Equals(BinaryString? other) => other is not null && other.AsByteSpan().SequenceEqual(AsByteSpan());
        public static bool operator ==(BinaryString? lhs, BinaryString? rhs) => lhs is null ? rhs is null : lhs.Equals(rhs);
        public static bool operator !=(BinaryString? lhs, BinaryString? rhs) => !(lhs == rhs);
    }

    internal static unsafe partial class BinaryStringInterop
    {
        [LibraryImport("oleaut32", EntryPoint = "SysAllocStringLen", SetLastError = false, StringMarshalling = StringMarshalling.Utf16)]
        public static partial nint AllocateTextBuffer( // WINOLEAUTAPI_(_Ret_writes_maybenull_z_(ui+1) BSTR) SysAllocStringLen
            char* buffer, // _In_reads_opt_(ui) const OLECHAR * strIn
            int length); // UINT ui

        [LibraryImport("oleaut32", EntryPoint = "SysAllocStringByteLen", SetLastError = false, StringMarshalling = StringMarshalling.Utf16)]
        public static partial nint AllocateByteBuffer( // WINOLEAUTAPI_(BSTR) SysAllocStringByteLen
            byte* buffer, // _In_opt_z_ LPCSTR psz
            int length); // _In_ UINT len
    }
}
