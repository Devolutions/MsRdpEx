using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace MsRdpEx_App
{
    internal class NowProtoPipeTransport: IDisposable, IAsyncDisposable
    {
        public NowProtoPipeTransport(NamedPipeServerStream pipe)
        {
            _pipe = pipe;
        }

        public async Task Write(byte[] data)
        {
            await _pipe.WriteAsync(data);
            await _pipe.FlushAsync();
        }

        public async Task<byte[]> Read()
        {
            var bytesRead = await _pipe.ReadAsync(_buffer, 0, _buffer.Length);

            if (bytesRead == 0)
            {
                throw new EndOfStreamException("End of stream reached (DVC)");
            }

            return _buffer[..bytesRead];
        }

        public void Dispose()
        {
            _pipe?.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            if (_pipe != null) await _pipe.DisposeAsync();
        }

        private readonly NamedPipeServerStream _pipe;
        // 64K message buffer
        private readonly byte[] _buffer = new byte[64 * 1024];
    }
}
