using System.Reflection;
using System.Runtime.InteropServices;

namespace MsRdpEx.Tests
{
    public class RdpInstancePluginTests
    {
        [Fact]
        public void WTSPlugin_WhenRegistrationFails_ReleasesOwnedReference()
        {
            var plugin = new object();
            IntPtr reference = Marshal.GetIUnknownForObject(plugin);
            int initialCount = GetReferenceCount(reference);
            try
            {
                var failure = new COMException("Plugin registration failed.");
                int calls = 0;
                var instance = DispatchProxy.Create<IMsRdpExInstance, PluginInstanceProxy>();
                ((PluginInstanceProxy)instance).Register = pointer =>
                {
                    Assert.Equal(reference, pointer);
                    calls++;
                    throw failure;
                };

                Assert.Same(failure, Assert.Throws<COMException>(() => new RdpInstance(instance).WTSPlugin = plugin));
                Assert.Equal(1, calls);
                Assert.Equal(initialCount, GetReferenceCount(reference));
            }
            finally
            {
                if (GetReferenceCount(reference) > initialCount)
                    Marshal.Release(reference);
                Marshal.Release(reference);
            }
        }

        [Fact]
        public void WTSPlugin_WhenRegistrationSucceeds_TransfersOwnedReference()
        {
            var plugin = new object();
            IntPtr reference = Marshal.GetIUnknownForObject(plugin);
            int initialCount = GetReferenceCount(reference);
            IntPtr transferred = IntPtr.Zero;
            try
            {
                var instance = DispatchProxy.Create<IMsRdpExInstance, PluginInstanceProxy>();
                ((PluginInstanceProxy)instance).Register = pointer => transferred = pointer;

                new RdpInstance(instance).WTSPlugin = plugin;

                Assert.Equal(reference, transferred);
                Assert.Equal(initialCount + 1, GetReferenceCount(reference));
            }
            finally
            {
                if (transferred != IntPtr.Zero && GetReferenceCount(reference) > initialCount)
                    Marshal.Release(transferred);
                Marshal.Release(reference);
            }
        }

        private static int GetReferenceCount(IntPtr pointer)
        {
            int count = Marshal.AddRef(pointer);
            Marshal.Release(pointer);
            return count;
        }

        public class PluginInstanceProxy : DispatchProxy
        {
            public Action<IntPtr>? Register { get; set; }

            protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
            {
                if (targetMethod?.Name != nameof(IMsRdpExInstance.SetWTSPluginObject))
                    throw new NotSupportedException(targetMethod?.Name);

                Register!((IntPtr)args![0]!);
                return null;
            }
        }
    }
}
