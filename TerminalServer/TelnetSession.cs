using NetCoreServer;
using Serilog;
using System.Net.Sockets;

namespace Core8
{
    internal class TelnetSession : TcpSession
    {
        private readonly TelnetServer server;

        public TelnetSession(TelnetServer server) : base(server)
        {
            this.server = server;

            this.server.Teleprinter.RegisterPrintCallback(Print);
        }

        private void Print(byte c)
        {
            SendAsync(new[] { c });
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            for (long i = offset; i < offset + size; i++)
            {
                server.Keyboard.Type(buffer[i]);
            }
            
            //SendAsync(buffer, offset, size);
        }

        protected override void OnError(SocketError error)
        {
            Log.Error($"Error caught in session: {error}");
        }
    }
}
