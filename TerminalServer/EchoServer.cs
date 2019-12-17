using NetCoreServer;
using Serilog;
using System.Net;
using System.Net.Sockets;

namespace Core8
{
    internal class EchoServer : TcpServer
    {

        public EchoServer(IPAddress address, int port) : base(address, port)
        {
        }

        protected override TcpSession CreateSession()
        {
            return new EchoSession(this);
        }

        protected override void OnError(SocketError error)
        {
            Log.Error($"Error caught in server: {error}");
        }
    }
}
