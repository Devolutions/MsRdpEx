using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace MsRdpEx.Interop
{
    [GeneratedComInterface]
    [Guid("302D8188-0052-4807-806A-362B628F9AC5")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsRdpExtendedSettings
    {
        void SetProperty(ReadOnlyBinaryStringRef PropertyName, in Variant value);
        void GetProperty(ReadOnlyBinaryStringRef PropertyName, out Variant value);
    }

    public static unsafe partial class InteropExtensions
    {
        public static object GetProperty(this IMsRdpExtendedSettings settings, ReadOnlyBinaryStringRef PropertyName)
        {
            Variant variant = default; // VT_EMPTY
            try
            {
                settings.GetProperty(PropertyName, out variant);

                switch (variant.Type)
                {
                    case VariantType.Boolean:
                        return variant.Content1 != 0;
                    case VariantType.IUnknown:
#if NET8_0_OR_GREATER
                        return ComInterfaceMarshaller<object>.ConvertToManaged((void*)variant.Content1);
#else
                        return Marshal.GetObjectForIUnknown(variant.Content1);
#endif
                    default:
                        throw new NotSupportedException();
                }
            }
            finally
            {
                int hr;
                if ((hr = VariantInterop.VariantClear(&variant)) < 0)
                    Environment.FailFast("Fatal interop failure.", Marshal.GetExceptionForHR(hr));
            }
        }

        public static void SetProperty(this IMsRdpExtendedSettings settings, ReadOnlyBinaryStringRef PropertyName, bool value)
        {
            Variant variant = default;
            variant.Type = VariantType.Boolean;
            Unsafe.As<nint, VariantBool>(ref variant.Content1) = value;
            settings.SetProperty(PropertyName, variant);
        }

        public static void SetProperty(this IMsRdpExtendedSettings settings, ReadOnlyBinaryStringRef PropertyName, int value)
        {
            Variant variant = default;
            variant.Type = VariantType.Int32;
            Unsafe.As<nint, int>(ref variant.Content1) = value;
            settings.SetProperty(PropertyName, variant);
        }

        public static void SetProperty(this IMsRdpExtendedSettings settings, ReadOnlyBinaryStringRef PropertyName, uint value)
        {
            Variant variant = default;
            variant.Type = VariantType.UInt32;
            Unsafe.As<nint, uint>(ref variant.Content1) = value;
            settings.SetProperty(PropertyName, variant);
        }

        public static void SetProperty(this IMsRdpExtendedSettings settings, ReadOnlyBinaryStringRef PropertyName, string value)
        {
            Variant variant = default;
            variant.Type = VariantType.BinaryString;
            variant.Content1 = Marshal.StringToBSTR(value);
            try { settings.SetProperty(PropertyName, variant); }
            finally { Marshal.FreeBSTR(variant.Content1); }
        }
    }
}
