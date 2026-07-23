namespace MSTSCLib
{
    public enum AutoReconnectContinueState
    {
        #region AutoReconnectContinueState
        autoReconnectContinueAutomatic = 0,
        autoReconnectContinueStop = 1,
        autoReconnectContinueManual = 2,
        #endregion
    }
    public enum RemoteProgramResult
    {
        #region RemoteProgramResult
        remoteAppResultOk = 0,
        remoteAppResultLocked = 1,
        remoteAppResultProtocolError = 2,
        remoteAppResultNotInWhitelist = 3,
        remoteAppResultNetworkPathDenied = 4,
        remoteAppResultFileNotFound = 5,
        remoteAppResultFailure = 6,
        remoteAppResultHookNotLoaded = 7,
        #endregion
    }
    public enum RemoteWindowDisplayedAttribute
    {
        #region RemoteWindowDisplayedAttribute
        remoteAppWindowNone = 0,
        remoteAppWindowDisplayed = 1,
        remoteAppShellIconDisplayed = 2,
        #endregion
    }
    public enum ExtendedDisconnectReasonCode
    {
        #region ExtendedDisconnectReasonCode
        exDiscReasonNoInfo = 0,
        exDiscReasonAPIInitiatedDisconnect = 1,
        exDiscReasonAPIInitiatedLogoff = 2,
        exDiscReasonServerIdleTimeout = 3,
        exDiscReasonServerLogonTimeout = 4,
        exDiscReasonReplacedByOtherConnection = 5,
        exDiscReasonOutOfMemory = 6,
        exDiscReasonServerDeniedConnection = 7,
        exDiscReasonServerDeniedConnectionFips = 8,
        exDiscReasonServerInsufficientPrivileges = 9,
        exDiscReasonServerFreshCredsRequired = 10,
        exDiscReasonRpcInitiatedDisconnectByUser = 11,
        exDiscReasonLogoffByUser = 12,
        exDiscReasonShutdown = 25,
        exDiscReasonReboot = 26,
        exDiscReasonLicenseInternal = 256,
        exDiscReasonLicenseNoLicenseServer = 257,
        exDiscReasonLicenseNoLicense = 258,
        exDiscReasonLicenseErrClientMsg = 259,
        exDiscReasonLicenseHwidDoesntMatchLicense = 260,
        exDiscReasonLicenseErrClientLicense = 261,
        exDiscReasonLicenseCantFinishProtocol = 262,
        exDiscReasonLicenseClientEndedProtocol = 263,
        exDiscReasonLicenseErrClientEncryption = 264,
        exDiscReasonLicenseCantUpgradeLicense = 265,
        exDiscReasonLicenseNoRemoteConnections = 266,
        exDiscReasonLicenseCreatingLicStoreAccDenied = 267,
        exDiscReasonRdpEncInvalidCredentials = 768,
        exDiscReasonProtocolRangeStart = 4096,
        exDiscReasonProtocolRangeEnd = 32767,
        #endregion
    }
    public enum ControlCloseStatus
    {
        #region ControlCloseStatus
        controlCloseCanProceed = 0,
        controlCloseWaitForEvents = 1,
        #endregion
    }
    public enum RedirectionWarningType
    {
        #region RedirectionWarningType
        RedirectionWarningTypeDefault = 0,
        RedirectionWarningTypeUnsigned = 1,
        RedirectionWarningTypeUnknown = 2,
        RedirectionWarningTypeUser = 3,
        RedirectionWarningTypeThirdPartySigned = 4,
        RedirectionWarningTypeTrusted = 5,
        RedirectionWarningTypeMax = 5,
        #endregion
    }
    public enum RemoteSessionActionType
    {
        #region RemoteSessionActionType
        RemoteSessionActionCharms = 0,
        RemoteSessionActionAppbar = 1,
        RemoteSessionActionSnap = 2,
        RemoteSessionActionStartScreen = 3,
        RemoteSessionActionAppSwitch = 4,
        RemoteSessionActionActionCenter = 5,
        #endregion
    }
    public enum ControlReconnectStatus
    {
        #region ControlReconnectStatus
        controlReconnectStarted = 0,
        controlReconnectBlocked = 1,
        #endregion
    }
    public enum ClientSpec
    {
        #region ClientSpec
        FullMode = 0,
        ThinClientMode = 1,
        SmallCacheMode = 2,
        #endregion
    }
    public enum RemoteActionType
    {
        #region RemoteActionType
        RemoteActionCharms = 0,
        RemoteActionAppbar = 1,
        RemoteActionSnap = 2,
        RemoteActionStartScreen = 3,
        RemoteActionAppSwitch = 4,
        #endregion
    }
    public enum SnapshotEncodingType
    {
        #region SnapshotEncodingType
        SnapshotEncodingDataUri = 0,
        #endregion
    }
    public enum SnapshotFormatType
    {
        #region SnapshotFormatType
        SnapshotFormatPng = 0,
        SnapshotFormatJpeg = 1,
        SnapshotFormatBmp = 2,
        #endregion
    }
    public enum CameraRedirEncodingQuality
    {
        #region CameraRedirEncodingQuality
        encodingQualityLow = 0,
        encodingQualityMedium = 1,
        encodingQualityHigh = 2,
        #endregion
    }
}
