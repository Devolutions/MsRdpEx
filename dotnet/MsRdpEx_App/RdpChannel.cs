using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Channels;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;

using System.Threading.Tasks;
using MSTSCLib;

namespace MsRdpEx_App
{
    [ComImport]
    [Guid("a1230207-d6a7-11d8-b9fd-000bdbd1f198")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IWTSVirtualChannel
    {
        void Write(uint cbSize, IntPtr pBuffer,
            [MarshalAs(UnmanagedType.IUnknown)] object pReserved);
        void Close();
    }

    [ComImport]
    [Guid("a1230204-d6a7-11d8-b9fd-000bdbd1f198")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IWTSVirtualChannelCallback
    {
        void OnDataReceived(uint cbSize, IntPtr pBuffer);
        void OnClose();
    }

    [ComImport]
    [Guid("a1230206-9a39-4d58-8674-cdb4dff4e73b")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IWTSListener
    {
        void GetConfiguration(
            [MarshalAs(UnmanagedType.IUnknown)] out object ppPropertyBag);
    }

    [ComImport]
    [Guid("a1230203-d6a7-11d8-b9fd-000bdbd1f198")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IWTSListenerCallback
    {
        void OnNewChannelConnection(
            IWTSVirtualChannel pChannel,
            [MarshalAs(UnmanagedType.BStr)] string data,
            [MarshalAs(UnmanagedType.Bool)] out bool pAccept,
            out IWTSVirtualChannelCallback pCallback);
    }

    [ComImport]
    [Guid("a1230205-d6a7-11d8-b9fd-000bdbd1f198")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IWTSVirtualChannelManager
    {
        void CreateListener(
            [MarshalAs(UnmanagedType.LPStr)] string pszChannelName,
            uint uFlags,
            [MarshalAs(UnmanagedType.Interface)] IWTSListenerCallback pListenerCallback,
            [MarshalAs(UnmanagedType.Interface)] out IWTSListener ppListener);
    }

    [ComImport]
    [Guid("a1230201-1439-4e62-a414-190d0ac3d40e")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IWTSPlugin
    {
        void Initialize([MarshalAs(UnmanagedType.Interface)] IWTSVirtualChannelManager pChannelMgr);
        void Connected();
        void Disconnected(uint dwDisconnectCode);
        void Terminated();
    }

    public class RdpDvcClient : IWTSVirtualChannelCallback
    {
        private NowProtoPipeTransport transport = null;

        public string channelName;
        private IWTSVirtualChannel wtsChannel;

        public event EventHandler OnChannelClose;

        private RdpView rdpView = null;


        public RdpDvcClient(string name, IWTSVirtualChannel wtsChannel, RdpView rdpView)
        {
            this.channelName = name;
            this.wtsChannel = wtsChannel;
        }

        public void SendRawBuffer(uint cbSize, IntPtr pBuffer)
        {
            Debug.WriteLine($"DVC DATA sent: {cbSize}");

            wtsChannel?.Write(cbSize, pBuffer, null);
        }

        void WriteDvc(byte[] data)
        {
            GCHandle? pinnedArray = null;
            try
            {
                // Marshall without additional allocations, only pin before call.
                pinnedArray = GCHandle.Alloc(data, GCHandleType.Pinned);
                SendRawBuffer((uint)data.Length, pinnedArray.Value.AddrOfPinnedObject());
            }
            finally
            {
                pinnedArray?.Free();
            }

        }

        void IWTSVirtualChannelCallback.OnDataReceived(uint cbSize, IntPtr pBuffer)
        {
            Debug.WriteLine($"DVC DATA received: {cbSize}");

            var buffer = new byte[cbSize];
            Marshal.Copy(pBuffer, buffer, 0, (int)cbSize);

            Task.Run(async () =>
            {
                await transport.Write(buffer);
            });
        }

        public void OnConnected()
        {
            var pipe = StartPipeServer();
            Task.Run(async () =>
            {
                await pipe.WaitForConnectionAsync();
                Debug.WriteLine("DVC pipe connected");
                transport = new NowProtoPipeTransport(pipe);

                // DVC -> pipe IO loop
                while (true)
                {
                    // Transport manually closed
                    if (transport is null)
                    {
                        break;
                    }

                    // loop could stop on EOF from the pipe
                    var data = await transport.Read();
                    WriteDvc(data);
                }
            });
        }

        public void OnClose()
        {
            Debug.WriteLine("DVC pipe disconnected");

            transport.Dispose();
            transport = null;
        }

        private void StartDvcAppIfPresent(string uniqueId)
        {
            // find exe besides app
            var defaultDvcAppPath = Path.Combine(
                Directory.GetParent(System.Environment.ProcessPath).FullName ?? "",
                "Devolutions.NowDvcApp.exe"
            );
            var dvcAppVar = Environment.GetEnvironmentVariable("MSRDPEX_DVC_APP");

            if (string.IsNullOrEmpty(dvcAppVar))
            {
                Trace.WriteLine(
                    $"`DEVOLUTIONS_DVC_APP` env var is not set, using default dvc app path `{defaultDvcAppPath}`"
                );
            }

            var dvcApp = dvcAppVar ?? defaultDvcAppPath;

            if (!File.Exists(dvcApp))
            {
                // Silently disable DVC functionality if the app is not found
                if (dvcApp == defaultDvcAppPath)
                {
                    Trace.WriteLine($"Default DVC app not found at `{dvcApp}`. DVC functionality is disabled.");
                    return;
                }

                // Show explicit message box in async manner if environment was configured wrong.
                Task.Run(() =>
                {
                    MessageBox.Show(
                        $"DVC app not found at `{dvcApp}`. Remove or adjust your `DEVOLUTIONS_DVC_APP` env var.",
                        "Warning",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                });
            }

            try
            {
                Process.Start(dvcApp, new string[] { uniqueId });
            } catch (Exception ex)
            {
                Trace.WriteLine($"Failed to start DVC app: {ex}");
                MessageBox.Show(
                    $"Failed to start DVC app: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private NamedPipeServerStream StartPipeServer()
        {
            // GUID in 00000000-0000-0000-0000-000000000000 format
            var uniqueId = Guid.NewGuid().ToString("D");
            var startApp = true;

            // If there is debug-mode client running on the machine, use it instead of
            // launching a new one. Useful for debugging purposes (Launch app under debugger
            // manually and then connect to it).
            {
                const string globalInstanceLockName = $"Global\\now-proto-client-GLOBAL";

                using var mutex = new Mutex(
                    false,
                    globalInstanceLockName,
                    out var createdNew
                );

                // Mutex already exist, therefore debug version of DVC client is already running
                if (!createdNew)
                {
                    // override uniqueId with the global instance name
                    uniqueId = "GLOBAL";
                    startApp = false;
                }
            }

            var pipeName = $"now-proto-{uniqueId}";

            if (startApp)
            {
                // Start the DVC app if it is not already running
                StartDvcAppIfPresent(uniqueId);
            }

            Trace.WriteLine($"Starting DVC pipe server at `\\\\.\\pipe\\{pipeName}`");

            // DVC transport is message based, therefore pipe is set to
            // PipeTransmissionMode.Message mode to avoid fragmentation.
            var server = new NamedPipeServerStream(
                pipeName,
                PipeDirection.InOut, 
                1,
                PipeTransmissionMode.Message,
                PipeOptions.Asynchronous
            );

            return server;
        }

        void IWTSVirtualChannelCallback.OnClose()
        {
            OnChannelClose?.Invoke(this, EventArgs.Empty);
        }
    }

    public class RdpDvcListener : IWTSListenerCallback
    {
        public int maxCount;
        public string channelName;
        public IWTSListener wtsListener;

        private List<RdpDvcClient> clients = new List<RdpDvcClient>();
        public List<RdpDvcClient> Clients => clients;

        private RdpView rdpView;

        public RdpDvcListener(string name, int maxCount, RdpView rdpView)
        {
            channelName = name;
            this.maxCount = maxCount;

            this.rdpView = rdpView;
        }

        void IWTSListenerCallback.OnNewChannelConnection(IWTSVirtualChannel pChannel,
            [MarshalAs(UnmanagedType.BStr)] string data,
            [MarshalAs(UnmanagedType.Bool)] out bool pAccept,
            out IWTSVirtualChannelCallback pCallback)
        {
            if ((maxCount != -1) && (clients.Count >= maxCount))
            {
                pAccept = false;
                pCallback = null;
                return;
            }

            RdpDvcClient client = new RdpDvcClient(channelName, pChannel, rdpView);
            pAccept = true;
            pCallback = client;

            client.OnConnected();

            client.OnChannelClose += OnChannelClose;
            clients.Add(client);
        }

        private void OnChannelClose(object sender, EventArgs e)
        {
            RdpDvcClient client = sender as RdpDvcClient;
            client?.OnClose();
            clients.Remove(client);
        }

        public void OnConnected(object sender, EventArgs e)
        {

        }

        public void OnDisconnected(object sender, EventArgs e)
        {

        }

        public void OnTerminated(object sender, EventArgs e)
        {

        }
    }

    public class RdpDvcPlugin : IWTSPlugin
    {
        public event EventHandler OnConnected;
        public event EventHandler OnDisconnected;
        public event EventHandler OnTerminated;

        private RdpView rdpView = null;

        private Dictionary<string, RdpDvcListener> listeners = new Dictionary<string, RdpDvcListener>();

        public RdpDvcPlugin(RdpView rdpView)
        {
            this.rdpView = rdpView;
        }

        void IWTSPlugin.Initialize(IWTSVirtualChannelManager pChannelMgr)
        {
            // Allow setting custom `MSRDPEX_DVC_CHANNEL_NAME` for testing purposes
            // (Implementing arbitrary DVC channel via pipe server)
            string channelName =
                Environment.GetEnvironmentVariable("MSRDPEX_DVC_CHANNEL_NAME")
                ?? "Devolutions::Now::Agent";

            RdpDvcListener listener = new RdpDvcListener(channelName, -1, rdpView);

            pChannelMgr.CreateListener(listener.channelName,
                0, listener, out listener.wtsListener);

            listeners[listener.channelName] = listener;
            this.OnConnected += listener.OnConnected;
            this.OnDisconnected += listener.OnDisconnected;
            this.OnTerminated += listener.OnTerminated;

            // After initialization, we don't need the reference to the RdpView anymore
            rdpView = null;
        }

        void IWTSPlugin.Connected()
        {
            OnConnected?.Invoke(this, EventArgs.Empty);
        }

        void IWTSPlugin.Disconnected(uint disconnectCode)
        {
            OnDisconnected?.Invoke(this, EventArgs.Empty);
        }

        void IWTSPlugin.Terminated()
        {
            OnTerminated?.Invoke(this, EventArgs.Empty);
        }
    }
}
