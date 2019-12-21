using Core8.Model.Interfaces;
using NetCoreServer;
using Serilog;
using System.Net.Sockets;

namespace Core8
{
    internal class TelnetSession : TcpSession
    {
        private readonly IKeyboard keyboard;

        public TelnetSession(TelnetServer server, IKeyboard keyboard) : base(server)
        {
            this.keyboard = keyboard;
        }

        protected override void OnConnected()
        {
            SendAsync("WELCOME TO THE PDP-8 TERMINAL SERVER\r\n");

        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            for (long i = offset; i < offset + size; i++)
            {
                keyboard.Type(buffer[i]);
            }

            //SendAsync(buffer, offset, size);
        }

        protected override void OnError(SocketError error)
        {
            Log.Error($"Error caught in session: {error}");
        }
    }
}
