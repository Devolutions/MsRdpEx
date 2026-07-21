using System;
using MsRdpEx.Interop;

namespace MSTSCLib
{
    public static class MSTSCLibExtensions
    {
        public static T GetValue<T>(this IMsRdpExtendedSettings settings, BinaryString bstrPropertyName) where T : struct
        {
            object value = settings.GetProperty(bstrPropertyName);
            return value is T result
                ? result
                : throw new InvalidCastException($"The '{bstrPropertyName}' extended setting is not a {typeof(T).Name}.");
        }

        public static void SetProperty(this IMsRdpExtendedSettings settings, BinaryString bstrPropertyName, object pValue)
        {
            settings.set_Property(bstrPropertyName, ref pValue);
        }

        public static void SetConnectWithEndpoint(this IMsRdpClientAdvancedSettings settings, object endpoint)
        {
            settings.set_ConnectWithEndpoint(ref endpoint);
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

        public static IMsRdpCameraRedirConfig GetCameraByIndex(this IMsRdpCameraRedirConfigCollection cameras, uint index)
        {
            return cameras.get_ByIndex(index);
        }

        public static IMsRdpCameraRedirConfig GetCameraBySymbolicLink(this IMsRdpCameraRedirConfigCollection cameras, BinaryString symbolicLink)
        {
            return cameras.get_BySymbolicLink(symbolicLink);
        }

        public static IMsRdpCameraRedirConfig GetCameraByInstanceId(this IMsRdpCameraRedirConfigCollection cameras, BinaryString instanceId)
        {
            return cameras.get_ByInstanceId(instanceId);
        }

        public static void set_Property(this IMsRdpExtendedSettings settings, BinaryString bstrPropertyName, object pValue)
        {
            settings.set_Property(bstrPropertyName, ref pValue);
        }
    }
}
