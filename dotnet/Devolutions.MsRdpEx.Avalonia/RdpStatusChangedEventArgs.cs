namespace Devolutions.MsRdpEx.Avalonia;

/// <summary>
/// Describes a human-readable RDP session status update.
/// </summary>
public sealed class RdpStatusChangedEventArgs(string message) : EventArgs
{
    public string Message { get; } = message;
}
