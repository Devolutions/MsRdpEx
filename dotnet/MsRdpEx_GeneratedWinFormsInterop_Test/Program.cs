using AxMSTSCLib;
using MSTSCLib;

var confirmClose = new RdpClientConfirmCloseEventArgs();
confirmClose.pfAllowClose = false;
if (confirmClose.AllowClose)
    throw new InvalidOperationException("pfAllowClose did not update AllowClose.");

var publicKey = new RdpClientPublicKeyEventArgs(null);
publicKey.pfContinueLogon = false;
if (publicKey.ContinueLogon)
    throw new InvalidOperationException("pfContinueLogon did not update ContinueLogon.");

var reconnect = new RdpClientLegacyAutoReconnectingEventArgs(42, 3);
reconnect.pArcContinueStatus = AutoReconnectContinueState.autoReconnectContinueStop;
if (reconnect.ContinueStatus != AutoReconnectContinueState.autoReconnectContinueStop)
    throw new InvalidOperationException("pArcContinueStatus did not update ContinueStatus.");

if (typeof(RdpClientConfirmCloseEventArgs).Assembly.GetName().Name != "Interop.MSTSCLib.Generated")
    throw new InvalidOperationException("Generated WinForms resolved duplicate event-argument types.");

Console.WriteLine(typeof(AxMsRdpClient9NotSafeForScripting).Assembly.FullName);
Console.WriteLine(typeof(RdpClientConfirmCloseEventArgs).Assembly.FullName);
Console.WriteLine("Generated WinForms legacy aliases succeeded.");
