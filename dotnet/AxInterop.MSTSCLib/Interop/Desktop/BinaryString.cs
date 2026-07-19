using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

#nullable enable

namespace MsRdpEx.Interop
{
    /// <summary>Contains either text or binary data.</summary>
    /// <remarks>Data is stored in immutable managed form and does not need to be disposed.</remarks>
    public unsafe sealed class BinaryString : IEquatable<BinaryString>
    {
        public static class Marshaller
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
                    return new BinaryString(new string((char*)data, 0, length / 2));

                // If the length is odd we store it as a byte array so we don't lose the last byte.
                var result = new byte[length];
                Marshal.Copy(data, result, 0, length);
                return new BinaryString(null, result);
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
                else
                {
                    fixed (byte* pByteData = data.mByteData)
                        result = BinaryStringInterop.AllocateByteBuffer(pByteData, data.mByteData!.Length);
                }

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
        //public static implicit operator ImmutableArray<byte>(BinaryString? bstr) => bstr is null ? default : bstr.ToImmutableArray();
        //public static implicit operator ReadOnlySpan<char>(BinaryString? bstr) => bstr is null ? default : bstr.AsTextSpan();
        //public static implicit operator ReadOnlySpan<byte>(BinaryString? bstr) => bstr is null ? default : bstr.AsByteSpan();

        public static implicit operator BinaryString?(string? text) => text is null ? null : new(text);
        //public static implicit operator BinaryString?(ImmutableArray<byte> data) => data.IsDefault ? null : new(data);
        //public static implicit operator BinaryString?(BinaryStringRef bstr) => bstr.IsNull ? null : new(bstr.AsByteSpan());
        public static implicit operator BinaryString?(char[] text) => text is null ? null : new(text);
        public static implicit operator BinaryString?(byte[] data) => data is null ? null : new(data);
        //public static implicit operator BinaryString(Span<char> text) => new(text);
        //public static implicit operator BinaryString(Span<byte> data) => new(data);
        //public static implicit operator BinaryString(ReadOnlySpan<char> text) => new(text);
        //public static implicit operator BinaryString(ReadOnlySpan<byte> data) => new(data);

        public static BinaryString Empty { get; } = new BinaryString("", []);

        public static bool IsNullOrEmpty(BinaryString? bstr) => bstr is null || bstr.IsEmpty;

        private string? mTextData;
        private byte[]? mByteData;

        private BinaryString(string? text, byte[] data)
        {
            if (text is null && data is null)
                throw new ArgumentException("A null string must be represented by a null BinaryString.");

            mTextData = text;
            mByteData = data;
        }

        public BinaryString(string text)
        {
            mTextData = text ?? throw new ArgumentNullException(nameof(text), "A null string must be represented by a null BinaryString.");
        }

        public BinaryString(char[] text)
        {
            if (text is null)
                throw new ArgumentNullException(nameof(text), "A null string must be represented by a null BinaryString.");

            mTextData = new string(text);
        }

        public BinaryString(byte[] data)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data), "A null string must be represented by a null BinaryString.");

            mByteData = (byte[])data.Clone();
        }

        public bool IsEmpty => (mTextData?.Length ?? mByteData!.Length) == 0;
        public int TextLength => mTextData?.Length ?? (mByteData!.Length / 2);
        public int ByteLength => mByteData is null ? mTextData!.Length * 2 : mByteData.Length;

        private byte[] ToArraySlow()
        {
            Debug.Assert(mTextData is not null);
            var data = new byte[mTextData!.Length * 2];
            fixed (char* pTextData = mTextData)
                Marshal.Copy((nint)pTextData, data, 0, data.Length);
            var other = Interlocked.CompareExchange(ref mByteData, data, default);
            return other is null ? data : other;
        }

        public byte[] ToArray() => (byte[])(mByteData is null ? ToArraySlow() : mByteData).Clone();

        private string GetStringSlow()
        {
            Debug.Assert(mByteData is not null);
            var data = new char[mByteData!.Length / 2];
            Buffer.BlockCopy(mByteData, 0, data, 0, data.Length * 2);
            var text = new string(data);
            return Interlocked.CompareExchange(ref mTextData, text, null) ?? text;
        }

        public override string ToString() => mTextData is null ? GetStringSlow() : mTextData;

        public override int GetHashCode()
        {
            if (mTextData is not null)
            {
                fixed (char* pTextData = mTextData)
                    return GetHashCode((byte*)pTextData, mTextData.Length * 2);
            }
            else
            {
                fixed (byte* pByteData = mByteData)
                    return GetHashCode(pByteData, mByteData!.Length);
            }
        }

        private static int GetHashCode(byte* pData, int length)
        {
            int hash = length;
            for (int i = 0; i < length; i++)
                hash = (hash * 33) + pData[i];
            return hash;
        }

        private bool Equals(string other)
        {
            fixed (char* pOther = other)
                return Equals((byte*)pOther, other.Length * 2);
        }

        private bool Equals(byte[] other)
        {
            fixed (byte* pOther = other)
                return Equals(pOther, other.Length);
        }

        private bool Equals(byte* pOther, int length)
        {
            if (mTextData is not null)
            {
                if (mTextData.Length * 2 != length)
                    return false;

                fixed (char* pTextData = mTextData)
                    return Equals((byte*)pTextData, length);
            }
            else
            {
                if (mByteData!.Length != length)
                    return false;

                fixed (byte* pByteData = mByteData)
                    return Equals(pByteData, length);
            }
        }

        private static bool Equals(byte* pData, byte* pOther, int length)
        {
            for (int i = 0; i < length; i++)
                if (pData[i] != pOther[i])
                    return false;

            return true;
        }

        public override bool Equals(object? obj) => Equals(obj as BinaryString);
        public bool Equals(BinaryString? other) => other is not null && (other.mTextData is not null ? Equals(other.mTextData) : Equals(other.mByteData!));
        public static bool operator ==(BinaryString? lhs, BinaryString? rhs) => lhs is null ? rhs is null : lhs.Equals(rhs);
        public static bool operator !=(BinaryString? lhs, BinaryString? rhs) => !(lhs == rhs);
    }

    internal static unsafe class BinaryStringInterop
    {
        [DllImport("oleaut32", EntryPoint = "SysAllocStringLen", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Unicode)]
        public static extern nint AllocateTextBuffer( // WINOLEAUTAPI_(_Ret_writes_maybenull_z_(ui+1) BSTR) SysAllocStringLen
            char* buffer, // _In_reads_opt_(ui) const OLECHAR * strIn
            int length); // UINT ui

        [DllImport("oleaut32", EntryPoint = "SysAllocStringByteLen", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Unicode)]
        public static extern nint AllocateByteBuffer( // WINOLEAUTAPI_(BSTR) SysAllocStringByteLen
            byte* buffer, // _In_opt_z_ LPCSTR psz
            int length); // _In_ UINT len
    }
}
