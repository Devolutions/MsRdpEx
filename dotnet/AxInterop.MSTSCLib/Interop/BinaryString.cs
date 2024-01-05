using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Threading;

#nullable enable

namespace MsRdpEx.Interop
{
    [NativeMarshalling(typeof(Marshaller))]
    public unsafe readonly ref struct ReadOnlyBinaryStringRef
    {
        [CustomMarshaller(typeof(ReadOnlyBinaryStringRef), MarshalMode.Default, typeof(Marshaller))]
        public static class Marshaller
        {
            public static ReadOnlyBinaryStringRef ConvertToManaged(nint bstr)
            {
                return new(bstr);
            }

            public static nint ConvertToUnmanaged(ReadOnlyBinaryStringRef value)
            {
                fixed (byte* pointer = value.content)
                    return BinaryStringInterop.AllocateByteBuffer(pointer, value.content.Length);
            }

            public static void Free(nint bstr)
            {
                if (bstr != 0)
                    Marshal.FreeBSTR(bstr);
            }
        }

        public static implicit operator string?(ReadOnlyBinaryStringRef bstr) => bstr.ToString();
        public static implicit operator ReadOnlySpan<char>(ReadOnlyBinaryStringRef bstr) => bstr.AsTextSpan();
        public static implicit operator ReadOnlySpan<byte>(ReadOnlyBinaryStringRef bstr) => bstr.AsByteSpan();

        public static implicit operator ReadOnlyBinaryStringRef(string? value) => new(value);
        public static implicit operator ReadOnlyBinaryStringRef(char[] value) => new(value);
        public static implicit operator ReadOnlyBinaryStringRef(byte[] value) => new(value);
        public static implicit operator ReadOnlyBinaryStringRef(Span<char> value) => new(value);
        public static implicit operator ReadOnlyBinaryStringRef(Span<byte> value) => new(value);
        public static implicit operator ReadOnlyBinaryStringRef(ReadOnlySpan<char> value) => new(value);
        public static implicit operator ReadOnlyBinaryStringRef(ReadOnlySpan<byte> value) => new(value);

        public static ReadOnlyBinaryStringRef Null => default;
        public static ReadOnlyBinaryStringRef Empty => new(string.Empty);

        private readonly ReadOnlySpan<byte> content;

        private ReadOnlyBinaryStringRef(nint bstr)
        {
            if (bstr != 0)
            {
                var length = ((uint*)bstr)[-1];
                content = new((void*)bstr, checked((int)length));
            }
        }

        public ReadOnlyBinaryStringRef(ReadOnlySpan<byte> data)
        {
            content = data;
        }

        public ReadOnlyBinaryStringRef(string? text)
        {
            content = MemoryMarshal.AsBytes(text.AsSpan());
        }

        public ReadOnlyBinaryStringRef(ReadOnlySpan<char> text)
        {
            content = MemoryMarshal.AsBytes(text);
        }

        public int ByteLength => content.Length;
        public int TextLength => content.Length / 2;
        public bool IsEmpty => content.IsEmpty;
        public bool IsNull => Unsafe.IsNullRef(ref MemoryMarshal.GetReference(content));

        public ReadOnlySpan<byte> AsByteSpan() => content;
        public ReadOnlySpan<char> AsTextSpan() => MemoryMarshal.Cast<byte, char>(content);
        public override string? ToString() => IsNull ? null : new(AsTextSpan());
        public override int GetHashCode()
        {
            if (IsNull)
                return 0;

            var hash = new HashCode();
            hash.AddBytes(content);
            return hash.ToHashCode();
        }
        public override bool Equals([NotNullWhen(true)] object? obj) => false; // cannot store ref-struct in a boxed object
        public bool Equals(ReadOnlyBinaryStringRef other)
        {
            ref var thisContent = ref MemoryMarshal.GetReference(this.content);
            ref var otherContent = ref MemoryMarshal.GetReference(other.content);

            if (Unsafe.AreSame(ref thisContent, ref otherContent))
                return true;

            if (Unsafe.IsNullRef(ref thisContent) || Unsafe.IsNullRef(ref otherContent))
                return false;

            return this.content.SequenceEqual(other.content);
        }
        public static bool operator ==(ReadOnlyBinaryStringRef lhs, ReadOnlyBinaryStringRef rhs) => lhs.Equals(rhs);
        public static bool operator !=(ReadOnlyBinaryStringRef lhs, ReadOnlyBinaryStringRef rhs) => !lhs.Equals(rhs);
    }

    [NativeMarshalling(typeof(OwnershipTransfer))]
    public sealed unsafe partial class BinaryString : IDisposable, IEquatable<BinaryString>
    {
        [CustomMarshaller(typeof(BinaryString), MarshalMode.Default, typeof(OwnershipTransfer))]
        public ref struct OwnershipTransfer
        {
            private nint pointer;

            public void FromManaged(BinaryString? value)
            {
                Free();

                // receive ownership from managed code
                if (value is not null)
                {
                    pointer = value.pointer;
                    value.pointer = 0;

                    // we are using zero pointers in non-null BinaryString as a cheap representation of an empty BinaryString
                    // however if we need to pass ownership of an empty string to native code we actually have to allocate one
                    if (pointer == 0 && (pointer = BinaryStringInterop.AllocateByteBuffer(null, 0)) == 0)
                        throw new OutOfMemoryException();
                }
            }

            public void FromUnmanaged(nint value)
            {
                Free();

                // receive ownership from unmanaged code
                pointer = value;
            }

            public nint ToUnmanaged()
            {
                // transfer ownership to unmanaged code
                var result = pointer;
                pointer = 0;
                return result;
            }

            public BinaryString? ToManaged()
            {
                if (pointer == 0)
                    return null;

                // make sure we don't construct a BinaryString larger than .NET can manage
                // (it's safe to throw here, the marshaller retains ownership and frees it)
                if (((int*)pointer)[-1] < 0)
                    throw new OverflowException();

                // transfer ownership to managed code
                var result = new BinaryString(pointer);
                pointer = 0;
                return result;
            }

            public void Free()
            {
                if (pointer != 0)
                {
                    Marshal.FreeBSTR(pointer);
                    pointer = 0;
                }
            }
        }

        public static implicit operator ReadOnlyBinaryStringRef(BinaryString? bstr) => bstr is null ? default : new(bstr.AsByteSpan());
        public static implicit operator string?(BinaryString? bstr) => bstr?.ToString();
        public static implicit operator ReadOnlySpan<char>(BinaryString? bstr) => bstr is null ? default : bstr.AsTextSpan();
        public static implicit operator ReadOnlySpan<byte>(BinaryString? bstr) => bstr is null ? default : bstr.AsByteSpan();

        public static implicit operator BinaryString?(string? text) => text is null ? null : new(text);
        public static implicit operator BinaryString(Span<char> text) => new(text);
        public static implicit operator BinaryString(Span<byte> data) => new(data);
        public static implicit operator BinaryString(ReadOnlySpan<char> text) => new(text);
        public static implicit operator BinaryString(ReadOnlySpan<byte> data) => new(data);

        public static BinaryString Empty { get; } = new(0);

        public static BinaryString AllocateByteLength(int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            if (length == 0)
                return Empty;

            var pointer = BinaryStringInterop.AllocateByteBuffer(null, length);
            if (pointer == 0)
                throw new OutOfMemoryException();

            return new(pointer);
        }

        public static BinaryString AllocateTextLength(int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            if (length == 0)
                return Empty;

            var pointer = BinaryStringInterop.AllocateTextBuffer(null, length);
            if (pointer == 0)
                throw new OutOfMemoryException();

            return new(pointer);
        }

        private nint pointer;

        private BinaryString(nint pointer)
        {
            if ((this.pointer = pointer) == 0)
                GC.SuppressFinalize(this);
        }

        public BinaryString(ReadOnlySpan<byte> data)
        {
            if (data.IsEmpty)
            {
                GC.SuppressFinalize(this);
            }
            else
            {
                fixed (byte* dataPointer = data)
                    if ((this.pointer = BinaryStringInterop.AllocateByteBuffer(dataPointer, data.Length)) == 0)
                        throw new OutOfMemoryException();
            }
        }

        public BinaryString(ReadOnlySpan<char> text)
        {
            if (text.IsEmpty)
            {
                GC.SuppressFinalize(this);
            }
            else
            {
                fixed (char* textPointer = text)
                    if ((this.pointer = BinaryStringInterop.AllocateTextBuffer(textPointer, text.Length)) == 0)
                        throw new OutOfMemoryException();
            }
        }

        public BinaryString(string? text)
        {
            if (string.IsNullOrEmpty(text))
                GC.SuppressFinalize(this);
            else
                pointer = Marshal.StringToBSTR(text);
        }

        ~BinaryString()
        {
            Dispose();
        }

        public void Dispose()
        {
            var pointer = Interlocked.Exchange(ref this.pointer, 0);
            if (pointer != 0)
            {
                GC.SuppressFinalize(this);
                Marshal.FreeBSTR(pointer);
            }
        }

        public int ByteLength => pointer == 0 ? 0 : ((int*)pointer)[-1];
        public int TextLength => ByteLength / 2;
        public bool IsEmpty => ByteLength == 0;
        public bool IsDisposed => pointer == 0;

        public Span<byte> AsByteSpan() => pointer == 0 ? new(Array.Empty<byte>()) : new((void*)pointer, ByteLength);
        public Span<char> AsTextSpan() => pointer == 0 ? new(Array.Empty<char>()) : new((void*)pointer, TextLength);
        public override string ToString() => new((char*)pointer, 0, TextLength);
        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.AddBytes(AsByteSpan());
            return hash.ToHashCode();
        }
        public override bool Equals(object? obj) => Equals(obj as BinaryString);
        public bool Equals(BinaryString? other) => other is not null && (this.pointer == other.pointer || this.AsByteSpan().SequenceEqual(other.AsByteSpan()));
        public static bool operator ==(BinaryString lhs, BinaryString rhs) => lhs is null ? rhs is null : lhs.Equals(rhs);
        public static bool operator !=(BinaryString lhs, BinaryString rhs) => !(lhs == rhs);
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
