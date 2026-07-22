extern alias ModernInterop;
extern alias GeneratedWinFormsHost;

using System.Reflection;
using System.Runtime.InteropServices;
using MsRdpEx;
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
            var tryGetInterfaceId = proxyMetadata.GetMethod(
                "TryGetInterfaceId",
                BindingFlags.Public | BindingFlags.Static)!;

            foreach (var compatibilityInterface in GetModernInterfaces())
            {
                var arguments = new object?[] { compatibilityInterface.TypeHandle, null };
                Assert.True((bool)tryGetImplementation.Invoke(null, arguments)!);
                Assert.NotNull(arguments[1]);

                arguments = [compatibilityInterface.TypeHandle, null];
                Assert.True((bool)tryGetInterfaceId.Invoke(null, arguments)!);
                var proxyGuidAttribute = compatibilityInterface.GetCustomAttributes()
                    .Single(attribute => attribute.GetType().Name == "ProxyGuidAttribute");
                var proxyGuid = (Guid)proxyGuidAttribute.GetType()
                    .GetProperty("IID")!
                    .GetValue(proxyGuidAttribute)!;
                Assert.Equal(proxyGuid, (Guid)arguments[1]!);
            }
        }

        [Fact]
        public void ModernInteropExposesActivationAndEventApis()
        {
            Assert.NotNull(modernInteropLib.GetType("MSTSCLib.RdpClientFactory"));
            Assert.NotNull(modernInteropLib.GetType("MSTSCLib.RdpClientEventSubscription"));
            Assert.NotNull(modernInteropLib.GetType("MSTSCLib.RdpClientEvents"));
            Assert.NotNull(typeof(MSTSCLib.GeneratedRdpClientHostOptions));
            Assert.NotNull(typeof(MSTSCLib.GeneratedRdpClientHost));
        }

        [Fact]
        public void SeparateGeneratedWinFormsHostExposesItsPublicApi()
        {
            var optionsType = typeof(GeneratedWinFormsHost::MsRdpEx.WinForms.GeneratedRdpClientHostOptions);
            var hostType = typeof(GeneratedWinFormsHost::MsRdpEx.WinForms.GeneratedRdpClientHost);

            Assert.Equal("Interop.MSTSCLib.Generated.WinForms", hostType.Assembly.GetName().Name);
            Assert.Equal(typeof(Guid), optionsType.GetProperty(nameof(GeneratedWinFormsHost::MsRdpEx.WinForms.GeneratedRdpClientHostOptions.ClassId))?.PropertyType);
            Assert.NotNull(hostType.GetMethod(nameof(GeneratedWinFormsHost::MsRdpEx.WinForms.GeneratedRdpClientHost.GetClient)));
        }

        [Fact]
        public void RdpInstanceRequiresAValidMsRdpExInterface()
        {
            Assert.False(Bindings.TryGetRdpInstance(new object(), out var instance));
            Assert.Null(instance);
            Assert.Throws<ArgumentNullException>(() => new RdpInstance(null!));
            Assert.Throws<InvalidOperationException>(() => RdpInstance.FromOcx(new object()));
        }

        [Fact]
        public void EventSubscriptionSupportsRemoteDesktopClientEvents()
        {
            var subscriptionType = typeof(ModernInterop::MSTSCLib.RdpClientEventSubscription);
            var sinkType = subscriptionType.GetNestedType("RdpClientEventSink", BindingFlags.NonPublic)!;

            Assert.Contains(typeof(ModernInterop::MsRdpEx.Interop.IRemoteDesktopClientEvents), sinkType.GetInterfaces());
            Assert.NotNull(typeof(ModernInterop::MSTSCLib.RdpClientEvents).GetMethod(
                nameof(ModernInterop::MSTSCLib.RdpClientEvents.Subscribe),
                [typeof(ModernInterop::MSTSCLib.IRemoteDesktopClient)]));

            foreach (string eventName in new[]
            {
                "DisconnectedWithDetails",
                "StatusChanged",
                "AutoReconnecting",
                "DialogDisplaying",
                "DialogDismissed",
                "NetworkStatusChanged",
                "AdminMessageReceived",
                "KeyCombinationPressed",
                "TouchPointerCursorMoved",
            })
            {
                Assert.NotNull(subscriptionType.GetEvent(eventName));
            }
        }

        [Fact]
        public void RdpClientFactoryExposesCompatibleModernControlFactories()
        {
            AssertFactoryReturnType("CreateClient9", typeof(MSTSCLib.IMsRdpClient9));
            AssertFactoryReturnType("CreateClient10", typeof(MSTSCLib.IMsRdpClient10));
            AssertFactoryReturnType("CreateClient11", typeof(MSTSCLib.IMsRdpClient10));
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
        public void EventSubscriptionWritesLegacyByRefCallbacks()
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

            subscription.ConfirmClose += (_, args) => args.AllowClose = false;
            subscription.TSPublicKeyReceived += (_, args) => args.ContinueLogon = false;
            subscription.LegacyAutoReconnecting += (_, args) => args.ContinueStatus = (ModernInterop::MSTSCLib.AutoReconnectContinueState)2;

            nint publicKey = Marshal.StringToBSTR("certificate");
            try
            {
                unsafe
                {
                    short allowClose = -1;
                    short continueLogon = -1;
                    int continueStatus = 0;
                    var arguments = stackalloc ModernInterop::MsRdpEx.Interop.NativeVariant[3];
                    var parameters = new DispatchParameters
                    {
                        Arguments = (nint)arguments,
                        ArgumentCount = 1,
                    };

                    arguments[0] = new(ModernInterop::MsRdpEx.Interop.VariantType.ByRefModifier | ModernInterop::MsRdpEx.Interop.VariantType.Boolean)
                    {
                        Content1 = (nint)(&allowClose),
                    };
                    invoke.Invoke(sink, [15, nint.Zero, 0, (short)0, (nint)(&parameters), nint.Zero, nint.Zero, nint.Zero]);

                    parameters.ArgumentCount = 2;
                    arguments[0] = new(ModernInterop::MsRdpEx.Interop.VariantType.ByRefModifier | ModernInterop::MsRdpEx.Interop.VariantType.Boolean)
                    {
                        Content1 = (nint)(&continueLogon),
                    };
                    arguments[1] = new(ModernInterop::MsRdpEx.Interop.VariantType.BinaryString)
                    {
                        Content1 = publicKey,
                    };
                    invoke.Invoke(sink, [16, nint.Zero, 0, (short)0, (nint)(&parameters), nint.Zero, nint.Zero, nint.Zero]);

                    parameters.ArgumentCount = 3;
                    arguments[0] = new(ModernInterop::MsRdpEx.Interop.VariantType.ByRefModifier | ModernInterop::MsRdpEx.Interop.VariantType.Int32)
                    {
                        Content1 = (nint)(&continueStatus),
                    };
                    arguments[1] = new(ModernInterop::MsRdpEx.Interop.VariantType.Int32)
                    {
                        Content1 = 3,
                    };
                    arguments[2] = new(ModernInterop::MsRdpEx.Interop.VariantType.Int32)
                    {
                        Content1 = 4,
                    };
                    invoke.Invoke(sink, [17, nint.Zero, 0, (short)0, (nint)(&parameters), nint.Zero, nint.Zero, nint.Zero]);

                    Assert.Equal(0, allowClose);
                    Assert.Equal(0, continueLogon);
                    Assert.Equal(2, continueStatus);
                }
            }
            finally
            {
                Marshal.FreeBSTR(publicKey);
            }
        }

        [Fact]
        public void EventSubscriptionDispatchesLegacyGappedDispIds()
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

            (bool Displayed, nint Handle, ModernInterop::MSTSCLib.RemoteWindowDisplayedAttribute Attribute)? remoteWindow = null;
            ModernInterop::MSTSCLib.RdpClientAutoReconnectingEventArgs? reconnect = null;
            bool devicesButtonPressed = false;
            subscription.RemoteWindowDisplayed += (displayed, handle, attribute) => remoteWindow = (displayed, handle, attribute);
            subscription.AutoReconnecting += (_, args) => reconnect = args;
            subscription.DevicesButtonPressed += () => devicesButtonPressed = true;

            unsafe
            {
                var arguments = stackalloc ModernInterop::MsRdpEx.Interop.NativeVariant[4];
                var parameters = new DispatchParameters
                {
                    Arguments = (nint)arguments,
                    ArgumentCount = 3,
                };

                arguments[0] = new(ModernInterop::MsRdpEx.Interop.VariantType.Int32) { Content1 = 1 };
                arguments[1] = new(ModernInterop::MsRdpEx.Interop.VariantType.IntPtr) { Content1 = 0x1234 };
                arguments[2] = new(ModernInterop::MsRdpEx.Interop.VariantType.Boolean) { Content1 = -1 };
                invoke.Invoke(sink, [29, nint.Zero, 0, (short)0, (nint)(&parameters), nint.Zero, nint.Zero, nint.Zero]);

                parameters.ArgumentCount = 4;
                arguments[0] = new(ModernInterop::MsRdpEx.Interop.VariantType.Int32) { Content1 = 5 };
                arguments[1] = new(ModernInterop::MsRdpEx.Interop.VariantType.Int32) { Content1 = 4 };
                arguments[2] = new(ModernInterop::MsRdpEx.Interop.VariantType.Boolean) { Content1 = -1 };
                arguments[3] = new(ModernInterop::MsRdpEx.Interop.VariantType.Int32) { Content1 = 3 };
                invoke.Invoke(sink, [34, nint.Zero, 0, (short)0, (nint)(&parameters), nint.Zero, nint.Zero, nint.Zero]);

                parameters.ArgumentCount = 0;
                invoke.Invoke(sink, [35, nint.Zero, 0, (short)0, (nint)(&parameters), nint.Zero, nint.Zero, nint.Zero]);
            }

            Assert.Equal((true, (nint)0x1234, (ModernInterop::MSTSCLib.RemoteWindowDisplayedAttribute)1), remoteWindow);
            Assert.NotNull(reconnect);
            Assert.Equal(3, reconnect.DisconnectReason);
            Assert.Equal(4, reconnect.AttemptCount);
            Assert.Equal(5, reconnect.MaxAttemptCount);
            Assert.True(reconnect.NetworkAvailable);
            Assert.True(devicesButtonPressed);
        }

        [Fact]
        public void EventSubscriptionDispatchesRemoteDesktopClientCallbacks()
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

            ModernInterop::MSTSCLib.RdpClientDisconnectedEventArgs? disconnect = null;
            ModernInterop::MSTSCLib.RdpClientAutoReconnectingEventArgs? reconnect = null;
            subscription.DisconnectedWithDetails += (_, args) => disconnect = args;
            subscription.AutoReconnecting += (_, args) => reconnect = args;

            nint disconnectMessage = Marshal.StringToBSTR("The server disconnected the session.");
            nint reconnectMessage = Marshal.StringToBSTR("Network transition.");
            try
            {
                unsafe
                {
                    var arguments = stackalloc ModernInterop::MsRdpEx.Interop.NativeVariant[6];
                    var parameters = new DispatchParameters
                    {
                        Arguments = (nint)arguments,
                        ArgumentCount = 3,
                    };

                    arguments[0] = new(ModernInterop::MsRdpEx.Interop.VariantType.BinaryString) { Content1 = disconnectMessage };
                    arguments[1] = new(ModernInterop::MsRdpEx.Interop.VariantType.Int32) { Content1 = 1001 };
                    arguments[2] = new(ModernInterop::MsRdpEx.Interop.VariantType.Int32) { Content1 = 42 };
                    invoke.Invoke(sink, [753, nint.Zero, 0, (short)0, (nint)(&parameters), nint.Zero, nint.Zero, nint.Zero]);

                    parameters.ArgumentCount = 6;
                    arguments[0] = new(ModernInterop::MsRdpEx.Interop.VariantType.Int32) { Content1 = 5 };
                    arguments[1] = new(ModernInterop::MsRdpEx.Interop.VariantType.Int32) { Content1 = 2 };
                    arguments[2] = new(ModernInterop::MsRdpEx.Interop.VariantType.Boolean) { Content1 = -1 };
                    arguments[3] = new(ModernInterop::MsRdpEx.Interop.VariantType.BinaryString) { Content1 = reconnectMessage };
                    arguments[4] = new(ModernInterop::MsRdpEx.Interop.VariantType.Int32) { Content1 = 1002 };
                    arguments[5] = new(ModernInterop::MsRdpEx.Interop.VariantType.Int32) { Content1 = 43 };
                    invoke.Invoke(sink, [755, nint.Zero, 0, (short)0, (nint)(&parameters), nint.Zero, nint.Zero, nint.Zero]);
                }
            }
            finally
            {
                Marshal.FreeBSTR(disconnectMessage);
                Marshal.FreeBSTR(reconnectMessage);
            }

            Assert.NotNull(disconnect);
            Assert.Equal(42, disconnect.DisconnectReason);
            Assert.Equal(1001, disconnect.ExtendedDisconnectReason);
            Assert.Equal("The server disconnected the session.", disconnect.DisconnectErrorMessage);
            Assert.NotNull(reconnect);
            Assert.Equal(43, reconnect.DisconnectReason);
            Assert.Equal(1002, reconnect.ExtendedDisconnectReason);
            Assert.Equal("Network transition.", reconnect.DisconnectErrorMessage);
            Assert.True(reconnect.NetworkAvailable);
            Assert.Equal(2, reconnect.AttemptCount);
            Assert.Equal(5, reconnect.MaxAttemptCount);
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
                nameof(ModernInterop::MSTSCLib.MSTSCLibExtensions.SetPublisherCertificateChain),
                typeof(ModernInterop::MSTSCLib.IMsRdpClientNonScriptable4), typeof(object));
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
            AssertExtension(extensions,
                nameof(ModernInterop::MSTSCLib.MSTSCLibExtensions.SetLoadBalanceInfoBytes),
                typeof(ModernInterop::MSTSCLib.IMsRdpClientAdvancedSettings), typeof(ReadOnlySpan<byte>));
            AssertExtension(extensions,
                nameof(ModernInterop::MSTSCLib.MSTSCLibExtensions.GetLoadBalanceInfoBytes),
                typeof(ModernInterop::MSTSCLib.IMsRdpClientAdvancedSettings));
        }

        [Fact]
        public void CompatibilityExtensionsRejectNonComObjects()
        {
            Assert.False(ModernInterop::MSTSCLib.MSTSCLibExtensions.TryGetInterface<ModernInterop::MSTSCLib.IMsRdpClient>(
                new object(),
                out _));
        }

        [Fact]
        public void ProxyObjectTryPackRejectsUnsupportedInterfaces()
        {
            var proxy = (ModernInterop::MSTSCLib.ProxyObject)RuntimeHelpers.GetUninitializedObject(
                typeof(ModernInterop::MSTSCLib.ProxyObject));

            Assert.False(ModernInterop::MSTSCLib.ProxyObject.TryPack<IDisposable>(proxy, out _));
            Assert.False(ModernInterop::MSTSCLib.ProxyObject.TryPack<ModernInterop::MSTSCLib.IMsRdpClient>(proxy, out _));
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

        private static void AssertFactoryReturnType(string name, Type returnType)
        {
            var factory = typeof(MSTSCLib.RdpClientFactory);
            Assert.Equal(returnType, factory.GetMethod(name, Type.EmptyTypes)?.ReturnType);
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
