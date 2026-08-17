namespace Devolutions.MsRdpEx.Avalonia;

/// <summary>
/// Connection values used by <see cref="RdpClientView.Connect(RdpConnectionSettings)"/>.
/// Passwords are passed directly to the RDP control and are not persisted by the view.
/// </summary>
public sealed record RdpConnectionSettings(
    string HostName,
    string UserName,
    string Password,
    string Domain,
    int DesktopWidth = 0,
    int DesktopHeight = 0,
    string RdpFileContents = "",
    bool RedirectClipboard = true);
