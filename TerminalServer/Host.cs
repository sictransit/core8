using Core8.Model.Interfaces;
using Serilog;
using System;
using System.Net;
using System.Threading;

namespace Core8
{
    public class Host
    {
        private readonly IInputDevice input;
        private readonly IOutputDevice output;

        private readonly ManualResetEvent running = new ManualResetEvent(false);

        private Thread hostThread;

        public Host(IInputDevice input, IOutputDevice output)
        {
            this.input = input;
            this.output = output;
        }

        public void Start()
        {
            hostThread = new Thread(Run);

            hostThread.Start();
        }

        private void Run()        
        {
            running.Set();

            var server = new EchoServer(IPAddress.Any, 2222)
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
    }
}
