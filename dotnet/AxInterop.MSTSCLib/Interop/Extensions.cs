using MsRdpEx.Interop;

namespace MSTSCLib
{
    public static class MSTSCLibExtensions
    {
        public static void SetProperty(this IMsRdpExtendedSettings settings, BinaryString bstrPropertyName, object pValue)
        {
            settings.set_Property(bstrPropertyName, ref pValue);
        }

        public static object GetProperty(this IMsRdpExtendedSettings settings, BinaryString bstrPropertyName)
        {
            return settings.get_Property(bstrPropertyName);
        }

        public static T GetProperty<T>(this IMsRdpExtendedSettings settings, BinaryString bstrPropertyName) where T : class
        {
            return ProxyObject.Pack<T>(settings.GetProperty(bstrPropertyName));
        }

        public static void set_Property(this IMsRdpExtendedSettings settings, BinaryString bstrPropertyName, object pValue)
        {
            settings.set_Property(bstrPropertyName, ref pValue);
        }
    }
}
