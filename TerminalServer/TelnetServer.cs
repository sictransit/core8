using Core8.Model.Interfaces;
using NetCoreServer;
using Serilog;
using System.Net;
using System.Net.Sockets;

namespace Core8
{
    internal class TelnetServer : TcpServer
    {
        public TelnetServer(IPAddress address, int port, IKeyboard keyboard, ITeleprinter teleprinter) : base(address, port)
        {
            Keyboard = keyboard;
            Teleprinter = teleprinter;
        }

        public IKeyboard Keyboard { get; }

        public ITeleprinter Teleprinter { get; }

        protected override TcpSession CreateSession()
        {
            return new TelnetSession(this);
        }

        protected override void OnError(SocketError error)
        {
            Log.Error($"Error caught in server: {error}");
        }
    }
}
