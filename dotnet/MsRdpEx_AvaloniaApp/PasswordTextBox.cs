using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;
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
        return new PasswordTextBoxAutomationPeer(this);
    }

    private sealed class PasswordTextBoxAutomationPeer(PasswordTextBox owner)
        : ControlAutomationPeer(owner), IValueProvider
    {
        bool IValueProvider.IsReadOnly => owner.IsReadOnly;

        string IValueProvider.Value => string.Empty;

        void IValueProvider.SetValue(string value)
        {
            if (owner.IsReadOnly)
                throw new InvalidOperationException("The password field is read-only.");

            owner.Text = value;
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Edit;
        }

        protected override string GetClassNameCore()
        {
            return nameof(TextBox);
        }
    }
}
