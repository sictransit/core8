using NetCoreServer;
using Serilog;
using System.Net.Sockets;

namespace Core8
{
    internal class EchoSession : TcpSession
    {
        public EchoSession(TcpServer server) : base(server)
        { }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            // Resend the message back to the client
            SendAsync(buffer, offset, size);
        }

        protected override void OnError(SocketError error)
        {
            Log.Error($"Error caught in session: {error}");
        }
    }
}
