using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

#nullable enable

namespace MsRdpEx.Interop
{
    [NativeMarshalling(typeof(Marshaller))]
    public unsafe readonly ref struct BinaryStringRef
    {
        [CustomMarshaller(typeof(BinaryStringRef), MarshalMode.Default, typeof(Marshaller))]
        public static class Marshaller
        {
            public static BinaryStringRef ConvertToManaged(nint bstr)
            {
                return new(bstr);
            }

            public static nint ConvertToUnmanaged(BinaryStringRef value)
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

        public static implicit operator string?(BinaryStringRef bstr) => bstr.ToString();
        public static implicit operator ReadOnlySpan<char>(BinaryStringRef bstr) => bstr.AsTextSpan();
        public static implicit operator ReadOnlySpan<byte>(BinaryStringRef bstr) => bstr.AsByteSpan();

        public static implicit operator BinaryStringRef(string? value) => new(value);
        public static implicit operator BinaryStringRef(ImmutableArray<byte> value) => new(value.AsSpan());
        public static implicit operator BinaryStringRef(BinaryString? value) => value is null ? default : new(value.AsByteSpan());
        public static implicit operator BinaryStringRef(char[] value) => new(value);
        public static implicit operator BinaryStringRef(byte[] value) => new(value);
        public static implicit operator BinaryStringRef(Span<char> value) => new(value);
        public static implicit operator BinaryStringRef(Span<byte> value) => new(value);
        public static implicit operator BinaryStringRef(ReadOnlySpan<char> value) => new(value);
        public static implicit operator BinaryStringRef(ReadOnlySpan<byte> value) => new(value);

        public static BinaryStringRef Null => default;
        public static BinaryStringRef Empty => new(string.Empty);

        private readonly ReadOnlySpan<byte> content;

        private BinaryStringRef(nint bstr)
        {
            if (bstr != 0)
            {
                var length = ((uint*)bstr)[-1];
                content = new((void*)bstr, checked((int)length));
            }
        }

        public BinaryStringRef(ReadOnlySpan<byte> data)
        {
            content = data;
        }

        public BinaryStringRef(string? text)
        {
            content = MemoryMarshal.AsBytes(text.AsSpan());
        }

        public BinaryStringRef(ReadOnlySpan<char> text)
        {
            content = MemoryMarshal.AsBytes(text);
        }

        public int ByteLength => content.Length;
        public int TextLength => content.Length / 2;
        public bool IsEmpty => content.IsEmpty;
        public bool IsNull => Unsafe.IsNullRef(ref MemoryMarshal.GetReference(content));

        public ReadOnlySpan<byte> AsByteSpan() => content;
        public ImmutableArray<byte> ToImmutableArray() => ImmutableArray.Create(content);
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
        public bool Equals(BinaryStringRef other)
        {
            ref var thisContent = ref MemoryMarshal.GetReference(this.content);
            ref var otherContent = ref MemoryMarshal.GetReference(other.content);

            if (Unsafe.AreSame(ref thisContent, ref otherContent))
                return true;

            if (Unsafe.IsNullRef(ref thisContent) || Unsafe.IsNullRef(ref otherContent))
                return false;

            return this.content.SequenceEqual(other.content);
        }
        public static bool operator ==(BinaryStringRef lhs, BinaryStringRef rhs) => lhs.Equals(rhs);
        public static bool operator !=(BinaryStringRef lhs, BinaryStringRef rhs) => !lhs.Equals(rhs);
    }
}
