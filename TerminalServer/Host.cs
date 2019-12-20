using Core8.Model.Interfaces;
using Serilog;
using System;
using System.Net;
using System.Threading;

namespace Core8
{
    public class Host
    {
        private readonly IKeyboard keyboard;
        private readonly ITeleprinter teleprinter;
        private readonly int port;
        private readonly ManualResetEvent running = new ManualResetEvent(false);

        private Thread hostThread;

        public Host(IKeyboard keyboard, ITeleprinter teleprinter, int port = 23)
        {
            this.keyboard = keyboard;
            this.teleprinter = teleprinter;
            this.port = port;
        }

        public void Start()
        {
            hostThread = new Thread(Run)
            {
                Priority = ThreadPriority.AboveNormal
            };

            hostThread.Start();
        }

        private void Run()
        {
            running.Set();

            var server = new TelnetServer(IPAddress.Any, port, keyboard, teleprinter)
            {
                OptionReuseAddress = true
            };

            Log.Information("Server starting ...");

            server.Start();

            while (running.WaitOne(TimeSpan.Zero))
            {
                Thread.Sleep(200);
            }

            server.Stop();

            Log.Information("Server stopped.");
        }

        public void Stop()
        {
            running.Reset();

            hostThread.Join();
        }

        public void Display(byte key)
        {
            throw new NotImplementedException();
        }
    }
}
