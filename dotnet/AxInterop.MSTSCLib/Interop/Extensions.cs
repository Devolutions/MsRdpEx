using MsRdpEx.Interop;

namespace MSTSCLib
{
    public static class MSTSCLibExtensions
    {
        public static void set_Property(this IMsRdpExtendedSettings settings, BinaryString bstrPropertyName, object pValue)
        {
            settings.set_Property(bstrPropertyName, ref pValue);
        }
    }
}
