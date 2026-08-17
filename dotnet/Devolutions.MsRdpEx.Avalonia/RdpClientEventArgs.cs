namespace Devolutions.MsRdpEx.Avalonia;

/// <summary>
/// Reports the remote desktop size acknowledged by the RDP ActiveX control.
/// </summary>
public sealed class RdpRemoteDesktopSizeChangedEventArgs(int width, int height) : EventArgs
{
    public int Width { get; } = width;
    public int Height { get; } = height;
}

/// <summary>
/// Reports that the RDP control released keyboard focus at an edge of its tab order.
/// </summary>
public sealed class RdpFocusReleasedEventArgs(int direction) : EventArgs
{
    public int Direction { get; } = direction;
}
