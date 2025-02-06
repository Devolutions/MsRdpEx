/// <summary>
/// Bindings for the native `dvc_client.dll` library (written in rust).
/// </summary>

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace MsRdpEx_App
{
    internal class DvcClientFfiException : Exception
    {
        public DvcClientFfiException(string message) : base(message) {}
    }

    internal class DvcClientError : Exception
    {
        public DvcClientError(string message) : base(message) { }
    }

    internal enum DvcClientResponseKind
    {
        UpdateUi,
        SendMessage,
        Error,
        None,
    }

    internal class DvcClientResponse
    {
        public DvcClientResponse(IntPtr result)
        {
            self = result;

            var kind_ptr = DvcClientLib.dvcc_response_get_kind(self);
            if (kind_ptr != IntPtr.Zero)
            {
                var kind_str = Marshal.PtrToStringAnsi(kind_ptr);
                switch (kind_str)
                {
                    case "update_ui":
                        kind = DvcClientResponseKind.UpdateUi;
                        break;
                    case "send_message":
                        kind = DvcClientResponseKind.SendMessage;
                        break;
                    case "error":
                        kind = DvcClientResponseKind.Error;

                        var data = GetData();
                        var message = Encoding.UTF8.GetString(data);
                        throw new DvcClientError(message);
                    default:
                        throw new DvcClientFfiException("Unknown response kind");
                }
            }
            else
            {
                kind = DvcClientResponseKind.None;
            }
        }

        public DvcClientResponseKind Kind
        {
            get
            {
                return kind;
            }
        }

        public string AsUiUpdate()
        {
            if (kind != DvcClientResponseKind.UpdateUi)
            {
                throw new DvcClientFfiException("Response is not an update_ui");
            }

            var data = GetData();
            return Encoding.UTF8.GetString(data);
        }

        public IntPtr GetDataPtr()
        {
            if (kind == DvcClientResponseKind.None)
            {
                throw new DvcClientFfiException("Response has no data");
            }

            var data_ptr = DvcClientLib.dvcc_response_get_data(self);

            if (data_ptr == IntPtr.Zero)
            {
                throw new DvcClientFfiException("Response data is NULL");
            }

            return data_ptr;
        }

        public uint GetDataLen()
        {
            var data_len = DvcClientLib.dvcc_response_get_data_len(self);

            return (uint)data_len;
        }

        private byte[] GetData()
        {
            if (kind == DvcClientResponseKind.None)
            {
                throw new DvcClientFfiException("Response has no data");
            }

            var data_ptr = DvcClientLib.dvcc_response_get_data(self);
            var data_len = DvcClientLib.dvcc_response_get_data_len(self);

            if (data_ptr == IntPtr.Zero)
            {
                throw new DvcClientFfiException("Response data is NULL");
            }

            var data = new byte[data_len];
            Marshal.Copy(data_ptr, data, 0, (int)data_len);
            return data;
        }

        ~DvcClientResponse()
        {
            DvcClientLib.dvcc_response_destroy(self);
        }

        private IntPtr self;
        private DvcClientResponseKind kind;
    }

    internal class DvcClientCtx
    {
        public DvcClientCtx()
        {
            self = DvcClientLib.dvcc_init();
        }

        public DvcClientResponse HandleUi(string request)
        {
            // String to byte[]
            var req = Encoding.UTF8.GetBytes(request);

            var result = DvcClientLib.dvcc_handle_ui(self, req, req.Length);
            return new DvcClientResponse(result);
        }

        public DvcClientResponse HandleData(byte[] data)
        {
            var result = DvcClientLib.dvcc_handle_data(self, data, data.Length);
            return new DvcClientResponse(result);
        }

        ~DvcClientCtx()
        {
            // TODO: IDisposable?
            DvcClientLib.dvcc_destroy(self);
        }

        IntPtr self;
    }

    internal class DvcClientLib
    {
        [DllImport("dvc_client.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr dvcc_init();

        [DllImport("dvc_client.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void dvcc_destroy(IntPtr ctx);

        [DllImport("dvc_client.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr dvcc_handle_ui(IntPtr ctx, byte[] req, nint req_len);

        [DllImport("dvc_client.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr dvcc_handle_data(IntPtr ctx, byte[] data, nint data_len);

        [DllImport("dvc_client.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void dvcc_response_destroy(IntPtr result);

        [DllImport("dvc_client.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr dvcc_response_get_kind(IntPtr result);

        [DllImport("dvc_client.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr dvcc_response_get_data(IntPtr result);

        [DllImport("dvc_client.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern nint dvcc_response_get_data_len(IntPtr result);
    }
}
