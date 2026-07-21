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

        public static IMsRdpDrive GetDriveByIndex(this IMsRdpDriveCollection drives, uint index)
        {
            return drives.get_DriveByIndex(index);
        }

        public static IMsRdpDevice GetDeviceByIndex(this IMsRdpDeviceCollection devices, uint index)
        {
            return devices.get_DeviceByIndex(index);
        }

        public static IMsRdpDevice GetDeviceById(this IMsRdpDeviceCollection devices, BinaryString deviceInstanceId)
        {
            return devices.get_DeviceById(deviceInstanceId);
        }

        public static void set_Property(this IMsRdpExtendedSettings settings, BinaryString bstrPropertyName, object pValue)
        {
            settings.set_Property(bstrPropertyName, ref pValue);
        }
    }
}
