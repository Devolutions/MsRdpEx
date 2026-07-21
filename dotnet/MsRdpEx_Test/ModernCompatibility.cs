extern alias ModernInterop;

using System.Reflection;
using System.Runtime.InteropServices;

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
        public void CompatibilityCollectionAccessorsUseModernMethodNames()
        {
            var extensions = typeof(ModernInterop::MSTSCLib.MSTSCLibExtensions);

            Assert.NotNull(extensions.GetMethod(
                nameof(ModernInterop::MSTSCLib.MSTSCLibExtensions.GetDriveByIndex),
                [typeof(ModernInterop::MSTSCLib.IMsRdpDriveCollection), typeof(uint)]));
            Assert.NotNull(extensions.GetMethod(
                nameof(ModernInterop::MSTSCLib.MSTSCLibExtensions.GetDeviceByIndex),
                [typeof(ModernInterop::MSTSCLib.IMsRdpDeviceCollection), typeof(uint)]));
            Assert.NotNull(extensions.GetMethod(
                nameof(ModernInterop::MSTSCLib.MSTSCLibExtensions.GetDeviceById),
                [typeof(ModernInterop::MSTSCLib.IMsRdpDeviceCollection), typeof(ModernInterop::MsRdpEx.Interop.BinaryString)]));
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

    }
}
