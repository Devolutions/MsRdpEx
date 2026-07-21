using System;
using System.Collections.Generic;

namespace MSTSCLib
{
    internal static class ProxyMetadata
    {
        private static readonly Dictionary<RuntimeTypeHandle, RuntimeTypeHandle> implementations = new()
        {
            [typeof(IMsTscAx_Redist).TypeHandle] = typeof(IMsTscAx_RedistProxy).TypeHandle,
            [typeof(IMsTscAx).TypeHandle] = typeof(IMsTscAxProxy).TypeHandle,
            [typeof(IMsTscSecuredSettings).TypeHandle] = typeof(IMsTscSecuredSettingsProxy).TypeHandle,
            [typeof(IMsTscAdvancedSettings).TypeHandle] = typeof(IMsTscAdvancedSettingsProxy).TypeHandle,
            [typeof(IMsTscDebug).TypeHandle] = typeof(IMsTscDebugProxy).TypeHandle,
            [typeof(IMsTscAxEvents).TypeHandle] = typeof(IMsTscAxEventsProxy).TypeHandle,
            [typeof(IMsRdpClient).TypeHandle] = typeof(IMsRdpClientProxy).TypeHandle,
            [typeof(IMsRdpClientAdvancedSettings).TypeHandle] = typeof(IMsRdpClientAdvancedSettingsProxy).TypeHandle,
            [typeof(IMsRdpClientSecuredSettings).TypeHandle] = typeof(IMsRdpClientSecuredSettingsProxy).TypeHandle,
            [typeof(IMsTscNonScriptable).TypeHandle] = typeof(IMsTscNonScriptableProxy).TypeHandle,
            [typeof(IMsRdpClientNonScriptable).TypeHandle] = typeof(IMsRdpClientNonScriptableProxy).TypeHandle,
            [typeof(IMsRdpClient2).TypeHandle] = typeof(IMsRdpClient2Proxy).TypeHandle,
            [typeof(IMsRdpClientAdvancedSettings2).TypeHandle] = typeof(IMsRdpClientAdvancedSettings2Proxy).TypeHandle,
            [typeof(IMsRdpClient3).TypeHandle] = typeof(IMsRdpClient3Proxy).TypeHandle,
            [typeof(IMsRdpClientAdvancedSettings3).TypeHandle] = typeof(IMsRdpClientAdvancedSettings3Proxy).TypeHandle,
            [typeof(IMsRdpClient4).TypeHandle] = typeof(IMsRdpClient4Proxy).TypeHandle,
            [typeof(IMsRdpClientAdvancedSettings4).TypeHandle] = typeof(IMsRdpClientAdvancedSettings4Proxy).TypeHandle,
            [typeof(IMsRdpClientNonScriptable2).TypeHandle] = typeof(IMsRdpClientNonScriptable2Proxy).TypeHandle,
            [typeof(IMsRdpClient5).TypeHandle] = typeof(IMsRdpClient5Proxy).TypeHandle,
            [typeof(IMsRdpClientTransportSettings).TypeHandle] = typeof(IMsRdpClientTransportSettingsProxy).TypeHandle,
            [typeof(IMsRdpClientAdvancedSettings5).TypeHandle] = typeof(IMsRdpClientAdvancedSettings5Proxy).TypeHandle,
            [typeof(ITSRemoteProgram).TypeHandle] = typeof(ITSRemoteProgramProxy).TypeHandle,
            [typeof(IMsRdpClientShell).TypeHandle] = typeof(IMsRdpClientShellProxy).TypeHandle,
            [typeof(IMsRdpClientNonScriptable3).TypeHandle] = typeof(IMsRdpClientNonScriptable3Proxy).TypeHandle,
            [typeof(IMsRdpDeviceCollection).TypeHandle] = typeof(IMsRdpDeviceCollectionProxy).TypeHandle,
            [typeof(IMsRdpDriveCollection).TypeHandle] = typeof(IMsRdpDriveCollectionProxy).TypeHandle,
            [typeof(IMsRdpClient6).TypeHandle] = typeof(IMsRdpClient6Proxy).TypeHandle,
            [typeof(IMsRdpClientAdvancedSettings6).TypeHandle] = typeof(IMsRdpClientAdvancedSettings6Proxy).TypeHandle,
            [typeof(IMsRdpClientTransportSettings2).TypeHandle] = typeof(IMsRdpClientTransportSettings2Proxy).TypeHandle,
            [typeof(IMsRdpClientNonScriptable4).TypeHandle] = typeof(IMsRdpClientNonScriptable4Proxy).TypeHandle,
            [typeof(IMsRdpClient7).TypeHandle] = typeof(IMsRdpClient7Proxy).TypeHandle,
            [typeof(IMsRdpClientAdvancedSettings7).TypeHandle] = typeof(IMsRdpClientAdvancedSettings7Proxy).TypeHandle,
            [typeof(IMsRdpClientTransportSettings3).TypeHandle] = typeof(IMsRdpClientTransportSettings3Proxy).TypeHandle,
            [typeof(IMsRdpClientSecuredSettings2).TypeHandle] = typeof(IMsRdpClientSecuredSettings2Proxy).TypeHandle,
            [typeof(ITSRemoteProgram2).TypeHandle] = typeof(ITSRemoteProgram2Proxy).TypeHandle,
            [typeof(IMsRdpClientNonScriptable5).TypeHandle] = typeof(IMsRdpClientNonScriptable5Proxy).TypeHandle,
            [typeof(IMsRdpPreferredRedirectionInfo).TypeHandle] = typeof(IMsRdpPreferredRedirectionInfoProxy).TypeHandle,
            [typeof(IMsRdpExtendedSettings).TypeHandle] = typeof(IMsRdpExtendedSettingsProxy).TypeHandle,
            [typeof(IMsRdpClient8).TypeHandle] = typeof(IMsRdpClient8Proxy).TypeHandle,
            [typeof(IMsRdpClientAdvancedSettings8).TypeHandle] = typeof(IMsRdpClientAdvancedSettings8Proxy).TypeHandle,
            [typeof(IMsRdpClient9).TypeHandle] = typeof(IMsRdpClient9Proxy).TypeHandle,
            [typeof(IMsRdpClientTransportSettings4).TypeHandle] = typeof(IMsRdpClientTransportSettings4Proxy).TypeHandle,
            [typeof(IMsRdpClient10).TypeHandle] = typeof(IMsRdpClient10Proxy).TypeHandle,
            [typeof(ITSRemoteProgram3).TypeHandle] = typeof(ITSRemoteProgram3Proxy).TypeHandle,
            [typeof(IMsRdpClientNonScriptable6).TypeHandle] = typeof(IMsRdpClientNonScriptable6Proxy).TypeHandle,
            [typeof(IMsRdpClientNonScriptable7).TypeHandle] = typeof(IMsRdpClientNonScriptable7Proxy).TypeHandle,
            [typeof(IMsRdpCameraRedirConfigCollection).TypeHandle] = typeof(IMsRdpCameraRedirConfigCollectionProxy).TypeHandle,
            [typeof(IMsRdpClipboard).TypeHandle] = typeof(IMsRdpClipboardProxy).TypeHandle,
            [typeof(IRemoteDesktopClient).TypeHandle] = typeof(IRemoteDesktopClientProxy).TypeHandle,
            [typeof(IRemoteDesktopClientSettings).TypeHandle] = typeof(IRemoteDesktopClientSettingsProxy).TypeHandle,
            [typeof(IRemoteDesktopClientActions).TypeHandle] = typeof(IRemoteDesktopClientActionsProxy).TypeHandle,
            [typeof(IRemoteDesktopClientTouchPointer).TypeHandle] = typeof(IRemoteDesktopClientTouchPointerProxy).TypeHandle,
            [typeof(IRemoteDesktopClientEvents).TypeHandle] = typeof(IRemoteDesktopClientEventsProxy).TypeHandle,
            [typeof(IMsRdpDevice).TypeHandle] = typeof(IMsRdpDeviceProxy).TypeHandle,
            [typeof(IMsRdpDrive).TypeHandle] = typeof(IMsRdpDriveProxy).TypeHandle,
            [typeof(IMsRdpCameraRedirConfig).TypeHandle] = typeof(IMsRdpCameraRedirConfigProxy).TypeHandle,
        };

        public static bool TryGetImplementation(RuntimeTypeHandle interfaceType, out RuntimeTypeHandle implementationType)
        {
            return implementations.TryGetValue(interfaceType, out implementationType);
        }
    }
}
