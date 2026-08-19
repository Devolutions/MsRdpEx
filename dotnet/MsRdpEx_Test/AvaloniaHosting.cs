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

        // The OLE scope state is process-global, and xunit runs these tests in
        // a random order, so each case resets the counter and asserts absolute
        // rules: a non-owner never uninitializes, and an owner uninitializes
        // only when it leaves last.
        [Fact]
        public void OleScopeUninitializesOnlyOwnedLastSession()
        {
            RdpActiveXSession.ResetOleScopeForTesting();
            RdpActiveXSession.EnterOleScope(); // session A, owns its S_OK initialization
            RdpActiveXSession.EnterOleScope(); // session B, got S_FALSE

            Assert.False(RdpActiveXSession.ExitOleScope(false)); // B leaves first
            Assert.True(RdpActiveXSession.ExitOleScope(true)); // A is last and owned OLE
        }

        [Fact]
        public void OleScopeNeverUninitializesForeignOle()
        {
            RdpActiveXSession.ResetOleScopeForTesting();
            RdpActiveXSession.EnterOleScope();

            // S_FALSE path: OLE was initialized by another component (for
            // example Avalonia's OleContext) and must never be torn down here,
            // not even when the session leaves last.
            Assert.False(RdpActiveXSession.ExitOleScope(false));
        }

        [Fact]
        public void OleScopeKeepsOleWhenOwnerDisposedEarly()
        {
            RdpActiveXSession.ResetOleScopeForTesting();
            RdpActiveXSession.EnterOleScope(); // owner
            RdpActiveXSession.EnterOleScope(); // non-owner

            // The owning session leaves while another session remains, so the
            // uninitialize is deferred to the last surviving session.
            Assert.False(RdpActiveXSession.ExitOleScope(true));
            Assert.False(RdpActiveXSession.ExitOleScope(false));
        }

        [Fact]
        public void OleScopeDefersOwnerUninitializeWhileSessionsRemain()
        {
            // The deferral used when a session hands its balance to an older
            // S_FALSE session: the owner leaving first must not uninitialize,
            // and the last surviving session must.
            RdpActiveXSession.ResetOleScopeForTesting();
            RdpActiveXSession.EnterOleScope(); // A, got S_FALSE
            RdpActiveXSession.EnterOleScope(); // B, got S_OK and hands its balance to A

            Assert.False(RdpActiveXSession.ExitOleScope(true)); // B leaves first, no uninitialize
            Assert.False(RdpActiveXSession.ExitOleScope(false)); // A defers its own S_FALSE balance

            // A now runs the inherited balance as the last session.
            RdpActiveXSession.EnterOleScope();
            Assert.True(RdpActiveXSession.ExitOleScope(true));
        }
    }
}
