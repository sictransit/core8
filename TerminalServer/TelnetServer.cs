using NetCoreServer;
using NetMQ.Sockets;
using Serilog;
using System.Net;
using System.Net.Sockets;

namespace Core8
{
    internal class TelnetServer : TcpServer
    {
        private readonly PublisherSocket publisher;        

        public TelnetServer(IPAddress address, int port, PublisherSocket publisher) : base(address, port)
        {
            this.publisher = publisher;            
        }

        protected override TcpSession CreateSession()
        {
            return new TelnetSession(this, publisher);
        }

        protected override void OnError(SocketError error)
        {
            Log.Error($"Error caught in server: {error}");
        }
    }
}
