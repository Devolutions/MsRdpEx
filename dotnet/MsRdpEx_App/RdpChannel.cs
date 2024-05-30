using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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
        public string channelName;
        private IWTSVirtualChannel wtsChannel;

        public event EventHandler OnChannelClose;

        public RdpDvcClient(string name, IWTSVirtualChannel wtsChannel)
        {
            this.channelName = name;
            this.wtsChannel = wtsChannel;
        }

        public void SendRawBuffer(uint cbSize, IntPtr pBuffer)
        {
            wtsChannel?.Write(cbSize, pBuffer, null);
        }

        void IWTSVirtualChannelCallback.OnDataReceived(uint cbSize, IntPtr pBuffer)
        {

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

        public RdpDvcListener(string name, int maxCount)
        {
            this.channelName = name;
            this.maxCount = maxCount;
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

            RdpDvcClient client = new RdpDvcClient(channelName, pChannel);
            pAccept = true;
            pCallback = client;

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

        private Dictionary<string, RdpDvcListener> listeners = new Dictionary<string, RdpDvcListener>();

        void IWTSPlugin.Initialize(IWTSVirtualChannelManager pChannelMgr)
        {
            string channelName = "DvcSample";
            RdpDvcListener listener = new RdpDvcListener(channelName, -1);

            pChannelMgr.CreateListener(listener.channelName,
                0, listener, out listener.wtsListener);

            listeners[listener.channelName] = listener;
            this.OnConnected += listener.OnConnected;
            this.OnDisconnected += listener.OnDisconnected;
            this.OnTerminated += listener.OnTerminated;
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
