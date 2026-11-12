using Devolutions.MsRdpEx.Avalonia;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Platform;

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

        [Fact]
        public void RdpClientViewReusesDetachedNativeControl()
        {
            using TestableRdpClientView view = new();
            TestNativeControlHandle control = new();

            view.DetachNativeControl(control);

            Assert.False(control.IsDestroyed);
            Assert.Same(control, view.AttachNativeControl(new TestPlatformHandle()));
            Assert.True(view.IsSurfaceRefreshQueuedForTesting);

            view.Dispose();
            view.DetachNativeControl(control);
            Assert.True(control.IsDestroyed);
        }

        [Fact]
        public void RdpClientViewDisposeDestroysDetachedNativeControl()
        {
            TestableRdpClientView view = new();
            TestNativeControlHandle control = new();

            view.DetachNativeControl(control);
            view.Dispose();

            Assert.True(control.IsDestroyed);
        }

        [Fact]
        public void RdpClientViewCoalescesSurfaceRefreshRequests()
        {
            using TestableRdpClientView view = new();
            int postCount = 0;
            view.SurfaceRefreshPostForTesting = _ => postCount++;

            view.RequestSurfaceRefreshForTesting();
            view.RequestSurfaceRefreshForTesting();

            Assert.True(view.IsSurfaceRefreshQueuedForTesting);
            Assert.Equal(1, postCount);

            view.RunSurfaceRefreshForTesting();

            Assert.False(view.IsSurfaceRefreshQueuedForTesting);
        }

        [Fact]
        public void RdpClientViewQueuedSurfaceRefreshIsSafeAfterDispose()
        {
            TestableRdpClientView view = new();

            view.RequestSurfaceRefreshForTesting();
            view.Dispose();

            view.RunSurfaceRefreshForTesting();

            Assert.False(view.IsSurfaceRefreshQueuedForTesting);
        }

        [Fact]
        public void RdpClientViewRequestsSurfaceRefreshOnWindowActivated()
        {
            using TestableRdpClientView view = new();
            int postCount = 0;
            view.SurfaceRefreshPostForTesting = _ => postCount++;

            view.RaiseWindowActivatedForTesting();

            Assert.True(view.IsSurfaceRefreshQueuedForTesting);
            Assert.Equal(1, postCount);
        }

        [Fact]
        public void RdpClientViewRequestsSurfaceRefreshOnRestoreFromMinimized()
        {
            using TestableRdpClientView view = new();
            int postCount = 0;
            view.SurfaceRefreshPostForTesting = _ => postCount++;

            view.RaiseWindowStateChangedForTesting(WindowState.Normal, WindowState.Minimized);
            view.RaiseWindowStateChangedForTesting(WindowState.Normal, WindowState.Maximized);

            Assert.False(view.IsSurfaceRefreshQueuedForTesting);
            Assert.Equal(0, postCount);

            view.RaiseWindowStateChangedForTesting(WindowState.Minimized, WindowState.Normal);

            Assert.True(view.IsSurfaceRefreshQueuedForTesting);
            Assert.Equal(1, postCount);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(0, 600)]
        [InlineData(800, 0)]
        [InlineData(-1, 600)]
        [InlineData(800, -1)]
        public void RdpClientViewRejectsInvalidNativeSurfaceSizes(int width, int height)
        {
            using TestableRdpClientView view = new();
            int attempts = 0;
            view.NativeSurfaceResizeForTesting = _ => { attempts++; return true; };

            Assert.False(view.TryResizeNativeSurface(new PixelSize(width, height)));
            Assert.Equal(0, attempts);
        }

        [Fact]
        public void RdpClientViewDoesNotCacheAnUnavailableNativeResize()
        {
            using TestableRdpClientView view = new();
            PixelSize size = new(1500, 1050);

            Assert.False(view.TryResizeNativeSurface(size));

            int attempts = 0;
            view.NativeSurfaceResizeForTesting = _ => { attempts++; return true; };
            Assert.True(view.TryResizeNativeSurface(size));
            Assert.True(view.TryResizeNativeSurface(size));
            Assert.Equal(1, attempts);
        }

        [Fact]
        public void RdpClientViewRejectsNativeResizesAfterDisposeEvenForACachedSize()
        {
            using TestableRdpClientView view = new();
            PixelSize size = new(1500, 1050);
            int attempts = 0;
            view.NativeSurfaceResizeForTesting = _ => { attempts++; return true; };
            Assert.True(view.TryResizeNativeSurface(size));

            view.Dispose();

            Assert.False(view.TryResizeNativeSurface(size));
            Assert.False(view.TryResizeNativeSurface(new PixelSize(1800, 1200)));
            Assert.Equal(1, attempts);
        }

        [Fact]
        public void RdpClientViewSkipsOnlyTheCurrentSuccessfulNativeSurfaceSize()
        {
            using TestableRdpClientView view = new();
            PixelSize first = new(1500, 1050);
            PixelSize second = new(1800, 1200);
            List<PixelSize> attempts = [];
            view.NativeSurfaceResizeForTesting = size => { attempts.Add(size); return true; };

            Assert.True(view.TryResizeNativeSurface(first));
            Assert.True(view.TryResizeNativeSurface(first));
            Assert.True(view.TryResizeNativeSurface(second));
            Assert.True(view.TryResizeNativeSurface(second));
            Assert.True(view.TryResizeNativeSurface(first));
            Assert.Equal(new[] { first, second, first }, attempts);
        }

        [Fact]
        public void RdpClientViewRetriesTheSameSizeAfterAFailedNativeResize()
        {
            using TestableRdpClientView view = new();
            PixelSize size = new(1500, 1050);
            List<PixelSize> attempts = [];
            view.NativeSurfaceResizeForTesting = requested =>
            {
                attempts.Add(requested);
                return attempts.Count > 1;
            };

            Assert.False(view.TryResizeNativeSurface(size));
            Assert.True(view.TryResizeNativeSurface(size));
            Assert.True(view.TryResizeNativeSurface(size));
            Assert.Equal(new[] { size, size }, attempts);
        }

        [Fact]
        public void RdpClientViewKeepsItsLastSuccessfulSizeAfterAFailedNativeResize()
        {
            using TestableRdpClientView view = new();
            PixelSize first = new(1500, 1050);
            PixelSize second = new(1800, 1200);
            List<PixelSize> attempts = [];
            view.NativeSurfaceResizeForTesting = requested =>
            {
                attempts.Add(requested);
                return attempts.Count != 2;
            };

            Assert.True(view.TryResizeNativeSurface(first));
            Assert.False(view.TryResizeNativeSurface(second));
            Assert.True(view.TryResizeNativeSurface(first));
            Assert.True(view.TryResizeNativeSurface(second));
            Assert.Equal(new[] { first, second, second }, attempts);
        }

        private sealed class TestableRdpClientView : RdpClientView
        {
            public IPlatformHandle AttachNativeControl(IPlatformHandle parent)
            {
                return base.CreateNativeControlCore(parent);
            }

            public void DetachNativeControl(IPlatformHandle control)
            {
                base.DestroyNativeControlCore(control);
            }
        }

        private class TestPlatformHandle : IPlatformHandle
        {
            public nint Handle => 1;

            public string HandleDescriptor => "TEST";
        }

        private sealed class TestNativeControlHandle : TestPlatformHandle, INativeControlHostDestroyableControlHandle
        {
            public bool IsDestroyed { get; private set; }

            public void Destroy()
            {
                IsDestroyed = true;
            }
        }
    }
}
