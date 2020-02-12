using NetMQ;
using NetMQ.Sockets;
using Serilog;
using System;
using System.Net;
using System.Threading;

namespace Core8
{
    public class Host : IDisposable
    {
        private readonly int port;
        private readonly ManualResetEvent running = new ManualResetEvent(false);
        private bool disposedValue = false;

        private Thread hostThread;

        public Host(int port = 23)
        {
            this.port = port;

            Console.CancelKeyPress += (sender, e) => running.Reset();
        }

        public void Start()
        {
            hostThread = new Thread(Run)
            {
                IsBackground = true,
                Priority = ThreadPriority.AboveNormal
            };

            hostThread.Start();
        }

        private void Run()
        {
            using var publisher = new PublisherSocket();

            publisher.Bind(@"tcp://127.0.0.1:17232");

            using var subscriber = new SubscriberSocket();
            subscriber.Bind(@"tcp://127.0.0.1:17233");
            subscriber.SubscribeToAnyTopic();

            running.Set();

            using var server = new TelnetServer(IPAddress.Any, port, publisher, subscriber)
            {
                OptionReuseAddress = true
            };

            Log.Information("Server starting ...");

            server.Start();

            while (running.WaitOne(TimeSpan.Zero))
            {
                if (subscriber.TryReceiveFrameBytes(TimeSpan.FromMilliseconds(100), out var frame))
                {
                    server.Multicast(frame);
                }
            }

            server.Stop();

            Log.Information("Server stopped.");

        }

        public void Stop()
        {
            running.Reset();

            hostThread.Join();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (running != null)
                    {
                        running.Dispose();
                    }
                }

                disposedValue = true;
            }
        }

        ~Host()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }
    }
}
