using MsRdpEx.Interop;

namespace MsRdpEx.Tests
{
    public class BinaryStringTests
    {
        [Fact]
        public void ReadOnlyBinaryStringRef_Conversions()
        {
            Assert.True(default == BinaryStringRef.Null);

            Assert.True(default(string) == BinaryStringRef.Null);
            Assert.True(string.Empty == BinaryStringRef.Empty);

            Assert.True((BinaryStringRef)default(BinaryString) == BinaryStringRef.Null);
            Assert.True((BinaryStringRef)BinaryString.Empty == BinaryStringRef.Empty);

            Assert.True(Array.Empty<byte>() == BinaryStringRef.Empty);
            Assert.True(Array.Empty<char>() == BinaryStringRef.Empty);

            Assert.True(default(Span<byte>) == BinaryStringRef.Null);
            Assert.True(default(Span<char>) == BinaryStringRef.Null);

            Assert.True(default == BinaryStringRef.Null.AsByteSpan());
            Assert.False(default == BinaryStringRef.Empty.AsByteSpan());

            Assert.True(default == BinaryStringRef.Null.AsTextSpan());
            Assert.False(default == BinaryStringRef.Empty.AsTextSpan());
        }

        [Fact]
        public void ReadOnlyBinaryStringRef_DifferentiatesBetweenNullAndEmpty()
        {
            Assert.True(BinaryStringRef.Null.IsNull);
            Assert.True(BinaryStringRef.Null.IsEmpty);

            Assert.False(BinaryStringRef.Empty.IsNull);
            Assert.True(BinaryStringRef.Empty.IsEmpty);

            Assert.True(BinaryStringRef.Null == BinaryStringRef.Null);
            Assert.True(BinaryStringRef.Empty == BinaryStringRef.Empty);

            Assert.False(BinaryStringRef.Null == BinaryStringRef.Empty);
            Assert.False(BinaryStringRef.Empty == BinaryStringRef.Null);
        }

        /*
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
        */
    }
}
