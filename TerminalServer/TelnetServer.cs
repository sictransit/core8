using Core8.Model.Interfaces;
using NetCoreServer;
using Serilog;
using System.Net;
using System.Net.Sockets;

namespace Core8
{
    internal class TelnetServer : TcpServer
    {
        private readonly IKeyboard keyboard;

        public TelnetServer(IPAddress address, int port, IKeyboard keyboard, ITeleprinter teleprinter) : base(address, port)
        {
            teleprinter.RegisterPrintCallback(Print);

            this.keyboard = keyboard;
        }

        private void Print(byte c)
        {
            Multicast(new[] { c });
        }

        protected override TcpSession CreateSession()
        {
            return new TelnetSession(this, keyboard);
        }

        protected override void OnError(SocketError error)
        {
            Log.Error($"Error caught in server: {error}");
        }
    }
}
