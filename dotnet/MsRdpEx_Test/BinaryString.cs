using MsRdpEx.Interop;

namespace MsRdpEx.Tests
{
    public class BinaryStringTests
    {
        [Fact]
        public void ReadOnlyBinaryStringRef_Conversions()
        {
            Assert.True(default == ReadOnlyBinaryStringRef.Null);

            Assert.True(default(string) == ReadOnlyBinaryStringRef.Null);
            Assert.True(string.Empty == ReadOnlyBinaryStringRef.Empty);

            Assert.True(default(BinaryString) == ReadOnlyBinaryStringRef.Null);
            Assert.True(BinaryString.Empty == ReadOnlyBinaryStringRef.Empty);

            Assert.True(Array.Empty<byte>() == ReadOnlyBinaryStringRef.Empty);
            Assert.True(Array.Empty<char>() == ReadOnlyBinaryStringRef.Empty);

            Assert.True(default(Span<byte>) == ReadOnlyBinaryStringRef.Null);
            Assert.True(default(Span<char>) == ReadOnlyBinaryStringRef.Null);

            Assert.True(default == ReadOnlyBinaryStringRef.Null.AsByteSpan());
            Assert.False(default == ReadOnlyBinaryStringRef.Empty.AsByteSpan());

            Assert.True(default == ReadOnlyBinaryStringRef.Null.AsTextSpan());
            Assert.False(default == ReadOnlyBinaryStringRef.Empty.AsTextSpan());
        }

        [Fact]
        public void ReadOnlyBinaryStringRef_DifferentiatesBetweenNullAndEmpty()
        {
            Assert.True(ReadOnlyBinaryStringRef.Null.IsNull);
            Assert.True(ReadOnlyBinaryStringRef.Null.IsEmpty);

            Assert.False(ReadOnlyBinaryStringRef.Empty.IsNull);
            Assert.True(ReadOnlyBinaryStringRef.Empty.IsEmpty);

            Assert.True(ReadOnlyBinaryStringRef.Null == ReadOnlyBinaryStringRef.Null);
            Assert.True(ReadOnlyBinaryStringRef.Empty == ReadOnlyBinaryStringRef.Empty);

            Assert.False(ReadOnlyBinaryStringRef.Null == ReadOnlyBinaryStringRef.Empty);
            Assert.False(ReadOnlyBinaryStringRef.Empty == ReadOnlyBinaryStringRef.Null);
        }

        [Fact]
        public void DisposedBinaryStringIsEmptyString()
        {
            const string text = "Hello World";
            using var bstr = new BinaryString(text);

            Assert.False(bstr.IsDisposed);
            Assert.False(bstr.IsEmpty);
            Assert.Equal(text.Length, bstr.TextLength);
            Assert.Equal(text.Length * sizeof(char), bstr.ByteLength);

            Assert.False(bstr == ReadOnlyBinaryStringRef.Empty);
            Assert.False(bstr == ReadOnlyBinaryStringRef.Null);

            bstr.Dispose();

            Assert.True(bstr.IsDisposed);
            Assert.True(bstr.IsEmpty);
            Assert.Equal(0, bstr.ByteLength);
            Assert.Equal(0, bstr.TextLength);

            Assert.True(bstr == ReadOnlyBinaryStringRef.Empty);
            Assert.False(bstr == ReadOnlyBinaryStringRef.Null);
        }
    }
}
