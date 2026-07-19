using System.Reflection;
using System.Runtime.InteropServices;
using MsRdpEx.Interop;

namespace MsRdpEx.Tests
{
    public class CompatibilityTests
    {
        private static Lazy<Assembly> interopLib { get; } = new(() => LoadEmbeddedAssembly("Interop.MSTSCLib.dll"));
        private static Lazy<Assembly> msRdpExLib { get; } = new(() => typeof(IMsRdpClient).Assembly);
        private static string GeneratedComInterfaceNamespace { get; } = typeof(IMsRdpClient).Namespace + ".";

        private static Assembly LoadEmbeddedAssembly(string EmbeddedFileName)
        {
            using var stream = typeof(CompatibilityTests).Assembly.GetManifestResourceStream(EmbeddedFileName);

            if (stream is null)
                throw new InvalidOperationException($"Could not find embedded {EmbeddedFileName}.");

            var buffer = new byte[stream.Length];
            stream.ReadExactly(buffer);
            return Assembly.Load(buffer);
        }

        #region MemberDataSource

        public static IEnumerable<object[]> GetExpectedInterfaceTypes()
        {
            foreach (var type in interopLib.Value.GetTypes())
                if (type.IsInterface && type.IsPublic && type.IsImport && type.GetCustomAttribute<CoClassAttribute>() is null)
                    yield return [type];
        }

        public static IEnumerable<object[]> GetEnumPairs()
        {
            var lib = msRdpExLib.Value;
            foreach (var refType in interopLib.Value.GetTypes())
                if (refType.IsEnum && refType.IsPublic && !refType.Name.StartsWith("_"))
                    yield return [refType, lib.GetType(refType.FullName!)!];
        }

        public static IEnumerable<object[]> GetGeneratedComInterfaces()
        {
            var lib = msRdpExLib.Value;
            foreach (var refType in GetExpectedInterfaceTypes().Unpack<Type>())
                if (lib.GetType(GeneratedComInterfaceNamespace + refType.Name) is { } libType && libType.IsInterface)
                    yield return [refType, libType];
        }

        public static IEnumerable<object[]> GetMethodPairs()
        {
            foreach (var (refType, libType) in GetGeneratedComInterfaces().Unpack<Type, Type>())
            {
                var refMethods = refType.GetMethods().Except(refType.GetInterfaces().SelectMany(x => x.GetMethods()), MethodNameComparer.Instance).ToArray();
                var libMethods = libType.GetMethods().Except(libType.GetInterfaces().SelectMany(x => x.GetMethods()), MethodNameComparer.Instance).ToArray();

                var count = Math.Min(refMethods.Length, libMethods.Length);
                for (int i = 0; i < count; i++)
                    yield return [new MethodDesc(refMethods[i]), new MethodDesc(libMethods[i])];
            }
        }

        #endregion

        #region Helper Methods

        private static string AdjustMethodName(string expectedName)
        {
            // adjust for naming conventions
            if (expectedName.StartsWith("set_"))
                expectedName = "Set" + char.ToUpperInvariant(expectedName[4]) + expectedName.Substring(5);
            else if (expectedName.StartsWith("get_"))
                expectedName = "Get" + char.ToUpperInvariant(expectedName[4]) + expectedName.Substring(5);
            //else
            //    expectedName = char.ToUpper(expectedName[0]) + expectedName.Substring(1);

            return expectedName;
        }

        private static void ConvertRefToPointer(ParameterDesc info)
        {
            if (info.Type.IsByRef)
                info.Type = info.Type.GetElementType()!.MakePointerType();
        }

        private static void AdjustExpectedType(ParameterDesc info)
        {
            info.Type = AdjustExpectedType(info, info.Type);
        }

        private static Type AdjustExpectedType(ParameterDesc info, Type type)
        {
            switch (type.Name)
            {
                case "Object" when info.GetCustomAttribute<MarshalAsAttribute>()?.Value == UnmanagedType.IDispatch:
                    // COM source generator doesn't support IDispatch yet, manually map it to the interface
                    return typeof(IDispatch);

                case "String" when info.Mode == ParameterMode.Input && info.GetCustomAttribute<MarshalAsAttribute>()?.Value == UnmanagedType.BStr:
                    // BSTR supports binary data, some APIs require this. Going through a string type prevents using that
                    // feature and makes those APIs unusable. Using a custom type to support both strings and spans.
                    return typeof(BinaryStringRef);

                case "String" when info.Mode == ParameterMode.Output && info.GetCustomAttribute<MarshalAsAttribute>()?.Value == UnmanagedType.BStr:
                    // BSTR supports binary data, some APIs require this. Going through a string type prevents using that
                    // feature and makes those APIs unusable. Using a custom type to support both strings and spans.
                    return typeof(BinaryString);

                case "UInt64" when info.GetCustomAttribute<ComAliasNameAttribute>()?.Value == "MSTSCLib.UINT_PTR":
                    // https://learn.microsoft.com/en-us/windows/win32/termserv/imsrdpclientnonscriptable-notifyredirectdevicechange
                    // fixing a bug in the assembly (it was generated from a 64 bit TLB, the 32 bit TLB uses 32 bit integers - proper AnyCpu code should use IntPtr)
                    return typeof(nuint);

                case "Int64" when info.GetCustomAttribute<ComAliasNameAttribute>()?.Value == "MSTSCLib.LONG_PTR":
                    // https://learn.microsoft.com/en-us/windows/win32/termserv/imsrdpclientnonscriptable-notifyredirectdevicechange
                    // fixing a bug in the assembly (it was generated from a 64 bit TLB, the 32 bit TLB uses 32 bit integers - proper AnyCpu code should use IntPtr)
                    return typeof(nint);

                case "_RemotableHandle&" when info.GetCustomAttribute<ComAliasNameAttribute>()?.Value == "MSTSCLib.wireHWND":
                    // https://learn.microsoft.com/en-us/windows/win32/termserv/imstscaxevents-onremotewindowdisplayed
                    // https://learn.microsoft.com/en-us/windows/win32/termserv/imsrdpclientnonscriptable2-uiparentwindowhandle
                    // Documentation says this should be a HWND, i.e. an opaque pointer.
                    // Some methods in the assembly do this, some don't, fix those that don't.
                    return typeof(nint);
            }

            if (type.IsByRef)
                return AdjustExpectedType(info, type.GetElementType()!).MakeByRefType();

            if (type.IsPointer)
                return AdjustExpectedType(info, type.GetElementType()!).MakePointerType();

            return type;
        }

        #endregion

        [Theory]
        [MemberData(nameof(GetExpectedInterfaceTypes))]
        public void InterfaceExists(Type refType)
        {
            var libType = msRdpExLib.Value.GetType(GeneratedComInterfaceNamespace + refType.Name);

            Assert.NotNull(libType);
            Assert.True(libType.IsInterface);
        }

        [Theory]
        [MemberData(nameof(GetEnumPairs))]
        public void EnumValues(Type expected, Type? generated)
        {
            Assert.NotNull(generated);
            Assert.Equal(expected.Name, generated.Name);

            var refFields = expected.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
            var libFields = generated.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            Assert.Equal(refFields.Length, libFields.Length);

            for (int i = 0; i < refFields.Length; i++)
            {
                Assert.Equal(refFields[i].Name, libFields[i].Name);
                Assert.Equal(refFields[i].GetRawConstantValue(), libFields[i].GetRawConstantValue());
            }
        }

        [Theory]
        [MemberData(nameof(GetGeneratedComInterfaces))]
        public void VTableSize(Type expected, Type generated)
        {
            var refMethods = expected.GetMethods().Except(expected.GetInterfaces().SelectMany(x => x.GetMethods()), MethodNameComparer.Instance).Count();
            var libMethods = generated.GetMethods().Except(generated.GetInterfaces().SelectMany(x => x.GetMethods()), MethodNameComparer.Instance).Count();

            Assert.Equal(refMethods, libMethods);
        }

        [Theory]
        [MemberData(nameof(GetMethodPairs))]
        public void MethodSignatures(MethodDesc expected, MethodDesc generated)
        {
            Assert.Equal(AdjustMethodName(expected.Name), generated.Name);

            var refParameters = expected.Parameters;
            var libParameters = generated.Parameters;

            // report mismatch in parameter counts first
            Assert.Equal(refParameters.Count, libParameters.Count);

            // apply workarounds to fix unit tests where things intentionally differ
            #region Known Differences

            if (generated.Type == typeof(IMsRdpClientNonScriptable) && generated.Name == nameof(IMsRdpClientNonScriptable.SendKeys))
            {
                Assert.Equal(3, libParameters.Count);
                Assert.Equal(typeof(int), libParameters[0].Type);
                Assert.Equal(typeof(VariantBool*), libParameters[1].Type);
                Assert.Equal(typeof(int*), libParameters[2].Type);
                Assert.Equal(ParameterMode.Input, libParameters[0].Mode);
                Assert.Equal(ParameterMode.Reference, libParameters[1].Mode);
                Assert.Equal(ParameterMode.Reference, libParameters[2].Mode);

                // These parameters are not output but we cannot annotate that properly with the COM source generator.
                // For correctness it doesn't matter since they are pointers so no marshalling is required.
                libParameters[1].Mode = ParameterMode.Input;
                libParameters[2].Mode = ParameterMode.Input;

                // The reference implementation is broken here and expects a boolean reference instead of an array/pointer.
                libParameters[1].Type = typeof(bool).MakeByRefType();
            }

            #endregion

            for (int i = 0; i < refParameters.Count; i++)
            {
                AdjustExpectedType(refParameters[i]);
                ConvertRefToPointer(refParameters[i]);
                ConvertRefToPointer(libParameters[i]);
                Assert.Equal(refParameters[i].Mode, libParameters[i].Mode);
                Assert.Equal(refParameters[i].TypeName, libParameters[i].TypeName);
            }
        }
    }
}
