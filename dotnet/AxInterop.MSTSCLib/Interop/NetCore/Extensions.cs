using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace MsRdpEx.Interop
{
    public static unsafe class MsRdpExInteropExtensions
    {
        public static void NotifyRedirectDeviceChange(this IMsRdpClientNonScriptable client, ulong wParam, long lParam)
        {
            client.NotifyRedirectDeviceChange((nuint)wParam, (nint)lParam);
        }

        public static unsafe void SendKeys(this IMsRdpClientNonScriptable client, int numKeys, bool pbArrayKeyUp, int plKeyData)
        {
            switch (numKeys)
            {
                case 0:
                    client.SendKeys(0, null, null);
                    break;

                case 1:
                    VariantBool _pbArrayKeyUp = pbArrayKeyUp;
                    client.SendKeys(1, &_pbArrayKeyUp, &plKeyData);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(numKeys));
            }
        }

        public static void SendKeys(this IMsRdpClientNonScriptable client, ReadOnlySpan<bool> keyUp, ReadOnlySpan<int> keyData)
        {
            if (keyUp.Length != keyData.Length)
                throw new InvalidOperationException();

            // Documentation says 20 is the maximum number of keys this API can send, so we also use it as the safety limit for stackalloc.
            // If the documentation is wrong and the library supports more inputs this can be removed but the stackalloc needs a soft limit
            // and fall back to array allocation when exceeding it:
            //
            // Span<VariantBool> tempKeyUp = keyUp.Length <= 512 ? stackalloc VariantBool[keyUp.Length] : new VariantBool[keyUp.Length];
            //
            if (keyUp.Length > 20)
                throw new InvalidOperationException();

            Span<VariantBool> keyUpBuffer = stackalloc VariantBool[keyUp.Length];
            for (int i = 0; i < keyUp.Length; i++)
                keyUpBuffer[i] = keyUp[i];

            fixed (VariantBool* pKeyUp = keyUpBuffer)
            fixed (int* pKeyData = keyData)
                client.SendKeys(keyUp.Length, pKeyUp, pKeyData);
        }

        public static void SendKeys(this IMsRdpClientNonScriptable client, ReadOnlySpan<VariantBool> keyUp, ReadOnlySpan<int> keyData)
        {
            if (keyUp.Length != keyData.Length)
                throw new InvalidOperationException();

            fixed (VariantBool* pKeyUp = keyUp)
            fixed (int* pKeyData = keyData)
                client.SendKeys(keyUp.Length, pKeyUp, pKeyData);
        }
    }

    // this is just for compatibility until we can generate the proper redirection
    internal static class RedirectionHelpers
    {
        #region Prefix Support

        // TODO: adapter should drop the 'in' keyword in the call
        internal static unsafe void SendKeys(this IMsRdpClientNonScriptable client, int numKeys, in bool pbArrayKeyUp, in int plKeyData)
        {
            client.SendKeys(numKeys, pbArrayKeyUp, plKeyData);
        }

        #endregion

        #region Spelling Support

        // TODO: adapter should use the right spelling
        internal static void set_ConnectWithEndpoint(this IMsRdpClientAdvancedSettings settings, in object A_1)
        {
            settings.SetConnectWithEndpoint(in A_1);
        }

        // TODO: adapter should use the right spelling
        internal static void set_PublisherCertificateChain(this IMsRdpClientNonScriptable4 client, in object pVarCert)
        {
            client.SetPublisherCertificateChain(in pVarCert);
        }

        // TODO: adapter should use the right spelling
        internal static void set_Property(this IMsRdpExtendedSettings settings, BinaryString bstrPropertyName, in object pValue)
        {
            settings.SetProperty(bstrPropertyName, in pValue);
        }

        // TODO: adapter should use the right spelling
        internal static IMsRdpDevice get_DeviceByIndex(this IMsRdpDeviceCollection devices, uint index)
        {
            return devices.GetDeviceByIndex(index);
        }

        // TODO: adapter should use the right spelling
        internal static IMsRdpDevice get_DeviceById(this IMsRdpDeviceCollection devices, BinaryString devInstanceId)
        {
            return devices.GetDeviceById(devInstanceId);
        }

        // TODO: adapter should use the right spelling
        internal static IMsRdpDrive get_DriveByIndex(this IMsRdpDriveCollection drives, uint index)
        {
            return drives.GetDriveByIndex(index);
        }

        // TODO: adapter should use the right spelling
        internal static IMsRdpCameraRedirConfig get_ByIndex(this IMsRdpCameraRedirConfigCollection redirections, uint index)
        {
            return redirections.GetByIndex(index);
        }

        // TODO: adapter should use the right spelling
        internal static IMsRdpCameraRedirConfig get_BySymbolicLink(this IMsRdpCameraRedirConfigCollection redirections, BinaryString SymbolicLink)
        {
            return redirections.GetBySymbolicLink(SymbolicLink);
        }

        // TODO: adapter should use the right spelling
        internal static IMsRdpCameraRedirConfig get_ByInstanceId(this IMsRdpCameraRedirConfigCollection redirections, BinaryString InstanceId)
        {
            return redirections.GetByInstanceId(InstanceId);
        }

        // TODO: adapter should use the right spelling
        internal static object get_Property(this IMsRdpExtendedSettings settings, BinaryString bstrPropertyName)
        {
            return settings.GetProperty(bstrPropertyName);
        }

        #endregion

        #region IDispatch Support

        private static unsafe IDispatch GetIDispatch(object callback)
        {
            if (callback is null)
                return null;

            if (callback is MSTSCLib.ProxyObject)
                return MSTSCLib.ProxyObject.Unpack<IDispatch>(callback);

            if (callback is ComObject)
                return (IDispatch)callback;

            if (Marshal.IsComObject(callback))
            {
                var pUnk = Marshal.GetIUnknownForObject(callback);
                try { return ComInterfaceMarshaller<IDispatch>.ConvertToManaged((void*)pUnk); }
                finally { Marshal.Release(pUnk); }
            }

            throw new NotSupportedException();
        }

        internal static void attachEvent(this IMsRdpClient9 client, BinaryString eventName, object callback)
        {
            client.attachEvent(eventName, GetIDispatch(callback));
        }

        internal static void detachEvent(this IMsRdpClient9 client, BinaryString eventName, object callback)
        {
            client.detachEvent(eventName, GetIDispatch(callback));
        }

        internal static void attachEvent(this IRemoteDesktopClient client, BinaryString eventName, object callback)
        {
            client.attachEvent(eventName, GetIDispatch(callback));
        }

        internal static void detachEvent(this IRemoteDesktopClient client, BinaryString eventName, object callback)
        {
            client.detachEvent(eventName, GetIDispatch(callback));
        }

        #endregion
    }
}
