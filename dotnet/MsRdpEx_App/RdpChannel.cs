using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Channels;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;

using System.Threading.Tasks;
using MSTSCLib;
using Devolutions.NowClient;

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

    public class RdpDvcClient : IWTSVirtualChannelCallback, INowTransport
    {
        private const int RxChannelCapacity = 1024;

        private DvcDialog dialog = null;

        public string channelName;
        private IWTSVirtualChannel wtsChannel;

        public event EventHandler OnChannelClose;

        private RdpView rdpView = null;

        private Channel<byte[]> rxChannel = Channel.CreateBounded<byte[]>(RxChannelCapacity);

        Task INowTransport.Write(byte[] data)
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

            return Task.CompletedTask;
        }

        Task<byte[]> INowTransport.Read()
        {
            return rxChannel.Reader.ReadAsync().AsTask();
        }

        public RdpDvcClient(string name, IWTSVirtualChannel wtsChannel, RdpView rdpView)
        {
            this.channelName = name;
            this.wtsChannel = wtsChannel;

            // Start DVC dialog form
            if (rdpView.InvokeRequired)
            {
                rdpView.Invoke(() => {
                    dialog = rdpView.StartDvcDialog();
                });
            }
            else
            {
                dialog = rdpView.StartDvcDialog();
            }
        }

        public void SendRawBuffer(uint cbSize, IntPtr pBuffer)
        {
            Debug.WriteLine($"SEND raw buffer: {cbSize}");
            wtsChannel?.Write(cbSize, pBuffer, null);
        }

        void IWTSVirtualChannelCallback.OnDataReceived(uint cbSize, IntPtr pBuffer)
        {
            Debug.WriteLine($"DATA received: {cbSize}");

            var buffer = new byte[cbSize];
            Marshal.Copy(pBuffer, buffer, 0, (int)cbSize);

            if (!rxChannel.Writer.TryWrite(buffer))
            {
                throw new InvalidOperationException("Failed to write to channel");
            }
        }

        public void OnConnected()
        {
            dialog.OnDvcConnected(this);
        }

        void IWTSVirtualChannelCallback.OnClose()
        {
            OnChannelClose?.Invoke(this, EventArgs.Empty);
        }
    }

    public class RdpDvcListener : IWTSListenerCallback
    {
        // Define On connected delegate
        public delegate void OnConnectedDelegate(INowTransport transport);

        public int maxCount;
        public string channelName;
        public IWTSListener wtsListener;

        private List<RdpDvcClient> clients = new List<RdpDvcClient>();
        public List<RdpDvcClient> Clients => clients;

        private OnConnectedDelegate onConnected;

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
            string channelName = "Devolutions::Now::Agent";
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
