using Avalonia.Automation.Peers;
using Avalonia.Controls;

namespace MsRdpEx_AvaloniaApp;

/// <summary>
/// Prevents password text from being exposed through the UI Automation value
/// provider while retaining the standard Avalonia TextBox editing behavior.
/// </summary>
public sealed class PasswordTextBox : TextBox
{
    protected override Type StyleKeyOverride => typeof(TextBox);

    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new NoneAutomationPeer(this);
    }
}
