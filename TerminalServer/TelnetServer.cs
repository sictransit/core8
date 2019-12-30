using NetCoreServer;
using NetMQ.Sockets;
using Serilog;
using System.Net;
using System.Net.Sockets;

namespace Core8
{
    internal class TelnetServer : TcpServer
    {
        private PublisherSocket publisher;
        private SubscriberSocket subscriber;

        public TelnetServer(IPAddress address, int port, PublisherSocket publisher, SubscriberSocket subscriber) : base(address, port)
        {
            this.publisher = publisher;
            this.subscriber = subscriber;
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
