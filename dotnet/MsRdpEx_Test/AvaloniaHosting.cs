using Devolutions.MsRdpEx.Avalonia;

namespace MsRdpEx.Tests
{
    public class AvaloniaHostingTests
    {
        [Fact]
        public void AxNameEnvironmentRestoresPreviousValue()
        {
            const string marker = "test-previous-axname";
            Environment.SetEnvironmentVariable("MSRDPEX_AXNAME", marker);

            try
            {
                string observed = RdpActiveXSession.WithAxNameEnvironment(
                    "mstsc",
                    () => Environment.GetEnvironmentVariable("MSRDPEX_AXNAME")!);

                Assert.Equal("mstsc", observed);
                Assert.Equal(marker, Environment.GetEnvironmentVariable("MSRDPEX_AXNAME"));
            }
            finally
            {
                Environment.SetEnvironmentVariable("MSRDPEX_AXNAME", null);
            }
        }

        [Fact]
        public void AxNameEnvironmentRestoresAfterException()
        {
            const string marker = "test-previous-axname";
            Environment.SetEnvironmentVariable("MSRDPEX_AXNAME", marker);

            try
            {
                Assert.Throws<InvalidOperationException>(() =>
                    RdpActiveXSession.WithAxNameEnvironment<object>("mstsc", () =>
                        throw new InvalidOperationException()));
                Assert.Equal(marker, Environment.GetEnvironmentVariable("MSRDPEX_AXNAME"));
            }
            finally
            {
                Environment.SetEnvironmentVariable("MSRDPEX_AXNAME", null);
            }
        }

        [Fact]
        public void AxNameEnvironmentClearsWhenPreviouslyUnset()
        {
            Environment.SetEnvironmentVariable("MSRDPEX_AXNAME", null);

            RdpActiveXSession.WithAxNameEnvironment("mstsc", () => 0);

            Assert.Null(Environment.GetEnvironmentVariable("MSRDPEX_AXNAME"));
        }

        [Theory]
        // Keys are trapped and forwarded only while the view is focused in an
        // active window with a live connection (focused-but-disconnected must
        // pass keys through for local focus traversal).
        [InlineData(false, true, true, true, true)]
        [InlineData(true, true, true, true, false)]
        [InlineData(false, false, true, true, false)]
        [InlineData(false, true, false, true, false)]
        [InlineData(false, true, true, false, false)]
        public void ShouldForwardKeysTruthTable(
            bool isViewOnly, bool focusWithin, bool windowActive, bool connectionActive, bool expected)
        {
            Assert.Equal(
                expected,
                RdpClientView.ShouldForwardKeys(isViewOnly, focusWithin, windowActive, connectionActive));
        }

        [Fact]
        public void OleScopeUninitializesOnlyOwnedLastSession()
        {
            RdpActiveXSession.EnterOleScope(); // session A, owns its S_OK initialization
            RdpActiveXSession.EnterOleScope(); // session B, got S_FALSE

            Assert.False(RdpActiveXSession.ExitOleScope(false)); // B leaves first
            Assert.True(RdpActiveXSession.ExitOleScope(true)); // A is last and owned OLE
        }

        [Fact]
        public void OleScopeNeverUninitializesForeignOle()
        {
            RdpActiveXSession.EnterOleScope();

            // S_FALSE path: OLE was initialized by another component (for
            // example Avalonia's OleContext) and must never be torn down here.
            Assert.False(RdpActiveXSession.ExitOleScope(false));
        }

        [Fact]
        public void OleScopeKeepsOleWhenOwnerDisposedEarly()
        {
            RdpActiveXSession.EnterOleScope(); // owner
            RdpActiveXSession.EnterOleScope(); // non-owner

            // The owning session leaves while another session remains, so OLE
            // intentionally stays initialized for the process lifetime.
            Assert.False(RdpActiveXSession.ExitOleScope(true));
            Assert.False(RdpActiveXSession.ExitOleScope(false));
        }
    }
}
