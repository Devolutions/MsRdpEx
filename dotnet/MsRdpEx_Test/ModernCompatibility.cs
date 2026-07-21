extern alias ModernInterop;

using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace MsRdpEx.Tests
{
    public class ModernCompatibilityTests
    {
        private static Lazy<Assembly> legacyInteropLib { get; } = new(LoadLegacyInteropAssembly);
        private static Assembly modernInteropLib { get; } = typeof(ModernInterop::MSTSCLib.ProxyObject).Assembly;

        private static Assembly LoadLegacyInteropAssembly()
        {
            using var stream = typeof(ModernCompatibilityTests).Assembly.GetManifestResourceStream("Interop.MSTSCLib.dll");

            if (stream is null)
                throw new InvalidOperationException("Could not find embedded Interop.MSTSCLib.dll.");

            var buffer = new byte[stream.Length];
            stream.ReadExactly(buffer);
            return Assembly.Load(buffer);
        }

        private static IEnumerable<Type> GetLegacyInterfaces()
        {
            return legacyInteropLib.Value.GetTypes()
                .Where(type => type.IsInterface && type.IsPublic && type.IsImport && type.GetCustomAttribute<CoClassAttribute>() is null)
                .OrderBy(type => type.FullName);
        }

        private static IEnumerable<Type> GetModernInterfaces()
        {
            return modernInteropLib.GetExportedTypes()
                .Where(type => type.IsInterface && type.Namespace == "MSTSCLib")
                .OrderBy(type => type.FullName);
        }

        [Fact]
        public void CompatibilityInterfacesMatchLegacyNames()
        {
            Assert.Equal(
                GetLegacyInterfaces().Select(type => type.FullName),
                GetModernInterfaces().Select(type => type.FullName));
        }

        [Fact]
        public void CompatibilityInterfacesHaveStaticProxyMappings()
        {
            var proxyMetadata = modernInteropLib.GetType("MSTSCLib.ProxyMetadata", throwOnError: true)!;
            var tryGetImplementation = proxyMetadata.GetMethod(
                "TryGetImplementation",
                BindingFlags.Public | BindingFlags.Static)!;

            foreach (var compatibilityInterface in GetModernInterfaces())
            {
                var arguments = new object?[] { compatibilityInterface.TypeHandle, null };
                Assert.True((bool)tryGetImplementation.Invoke(null, arguments)!);
                Assert.NotNull(arguments[1]);
            }
        }

        [Fact]
        public void ModernInteropExposesActivationAndEventApis()
        {
            Assert.NotNull(modernInteropLib.GetType("MSTSCLib.RdpClientFactory"));
            Assert.NotNull(modernInteropLib.GetType("MSTSCLib.RdpClientEventSubscription"));
            Assert.NotNull(modernInteropLib.GetType("MSTSCLib.RdpClientEvents"));
        }

        [Fact]
        public void EventSubscriptionDispatchesIDispatchCallbacks()
        {
            var subscription = (ModernInterop::MSTSCLib.RdpClientEventSubscription)RuntimeHelpers.GetUninitializedObject(
                typeof(ModernInterop::MSTSCLib.RdpClientEventSubscription));
            var sinkType = typeof(ModernInterop::MSTSCLib.RdpClientEventSubscription).GetNestedType(
                "RdpClientEventSink",
                BindingFlags.NonPublic)!;
            var sink = Activator.CreateInstance(
                sinkType,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                binder: null,
                args: [subscription],
                culture: null)!;
            var invoke = sinkType.GetMethod("Invoke", BindingFlags.Instance | BindingFlags.Public)!;

            int? disconnectedReason = null;
            (int Width, int Height)? remoteDesktopSize = null;
            subscription.Disconnected += reason => disconnectedReason = reason;
            subscription.RemoteDesktopSizeChanged += (width, height) => remoteDesktopSize = (width, height);

            unsafe
            {
                var arguments = stackalloc ModernInterop::MsRdpEx.Interop.NativeVariant[2];
                arguments[0] = new ModernInterop::MsRdpEx.Interop.NativeVariant(
                    ModernInterop::MsRdpEx.Interop.VariantType.Int32) { Content1 = 600 };
                arguments[1] = new ModernInterop::MsRdpEx.Interop.NativeVariant(
                    ModernInterop::MsRdpEx.Interop.VariantType.Int32) { Content1 = 800 };
                var parameters = new DispatchParameters
                {
                    Arguments = (nint)arguments,
                    ArgumentCount = 2,
                };

                invoke.Invoke(sink, [12, nint.Zero, 0, (short)0, (nint)(&parameters), nint.Zero, nint.Zero, nint.Zero]);
                parameters.ArgumentCount = 1;
                arguments[0].Content1 = 42;
                invoke.Invoke(sink, [4, nint.Zero, 0, (short)0, (nint)(&parameters), nint.Zero, nint.Zero, nint.Zero]);
            }

            Assert.Equal((800, 600), remoteDesktopSize);
            Assert.Equal(42, disconnectedReason);
        }

        [Fact]
        public void CompatibilityExtensionsProvideModernAccessors()
        {
            var extensions = typeof(ModernInterop::MSTSCLib.MSTSCLibExtensions);

            AssertExtension(extensions,
                nameof(ModernInterop::MSTSCLib.MSTSCLibExtensions.SetProperty),
                typeof(ModernInterop::MSTSCLib.IMsRdpExtendedSettings), typeof(ModernInterop::MsRdpEx.Interop.BinaryString), typeof(object));
            AssertExtension(extensions,
                nameof(ModernInterop::MSTSCLib.MSTSCLibExtensions.GetProperty),
                typeof(ModernInterop::MSTSCLib.IMsRdpExtendedSettings), typeof(ModernInterop::MsRdpEx.Interop.BinaryString));
            AssertExtension(extensions,
                nameof(ModernInterop::MSTSCLib.MSTSCLibExtensions.SetConnectWithEndpoint),
                typeof(ModernInterop::MSTSCLib.IMsRdpClientAdvancedSettings), typeof(object));
            AssertExtension(extensions,
                nameof(ModernInterop::MSTSCLib.MSTSCLibExtensions.GetDriveByIndex),
                typeof(ModernInterop::MSTSCLib.IMsRdpDriveCollection), typeof(uint));
            AssertExtension(extensions,
                nameof(ModernInterop::MSTSCLib.MSTSCLibExtensions.GetDeviceByIndex),
                typeof(ModernInterop::MSTSCLib.IMsRdpDeviceCollection), typeof(uint));
            AssertExtension(extensions,
                nameof(ModernInterop::MSTSCLib.MSTSCLibExtensions.GetDeviceById),
                typeof(ModernInterop::MSTSCLib.IMsRdpDeviceCollection), typeof(ModernInterop::MsRdpEx.Interop.BinaryString));
            AssertExtension(extensions,
                nameof(ModernInterop::MSTSCLib.MSTSCLibExtensions.GetCameraByIndex),
                typeof(ModernInterop::MSTSCLib.IMsRdpCameraRedirConfigCollection), typeof(uint));
            AssertExtension(extensions,
                nameof(ModernInterop::MSTSCLib.MSTSCLibExtensions.GetCameraBySymbolicLink),
                typeof(ModernInterop::MSTSCLib.IMsRdpCameraRedirConfigCollection), typeof(ModernInterop::MsRdpEx.Interop.BinaryString));
            AssertExtension(extensions,
                nameof(ModernInterop::MSTSCLib.MSTSCLibExtensions.GetCameraByInstanceId),
                typeof(ModernInterop::MSTSCLib.IMsRdpCameraRedirConfigCollection), typeof(ModernInterop::MsRdpEx.Interop.BinaryString));
        }

        [Fact]
        public void CompatibilityEnumsMatchLegacyValues()
        {
            var legacyEnums = legacyInteropLib.Value.GetTypes()
                .Where(type => type.IsEnum && type.IsPublic && !type.Name.StartsWith("_"))
                .OrderBy(type => type.FullName)
                .ToArray();
            var modernEnums = modernInteropLib.GetExportedTypes()
                .Where(type => type.IsEnum && type.Namespace == "MSTSCLib")
                .OrderBy(type => type.FullName)
                .ToArray();

            Assert.Equal(
                legacyEnums.Select(type => type.FullName),
                modernEnums.Select(type => type.FullName));

            foreach (var legacy in legacyEnums)
            {
                var modern = modernEnums.Single(type => type.FullName == legacy.FullName);
                Assert.Equal(
                    legacy.GetFields(BindingFlags.Public | BindingFlags.Static).Select(field => (field.Name, field.GetRawConstantValue())),
                    modern.GetFields(BindingFlags.Public | BindingFlags.Static).Select(field => (field.Name, field.GetRawConstantValue())));
            }
        }

        private static void AssertExtension(Type extensions, string name, params Type[] parameterTypes)
        {
            Assert.NotNull(extensions.GetMethods().SingleOrDefault(method =>
                method.Name == name &&
                !method.IsGenericMethod &&
                method.GetParameters().Select(parameter => parameter.ParameterType).SequenceEqual(parameterTypes)));
        }

        private unsafe struct DispatchParameters
        {
            public nint Arguments;
            public nint NamedArguments;
            public uint ArgumentCount;
            public uint NamedArgumentCount;
        }

    }
}
