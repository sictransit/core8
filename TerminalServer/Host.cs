using NetMQ;
using NetMQ.Sockets;
using Serilog;
using System;
using System.Net;
using System.Threading;

namespace Core8.Peripherals;

public class Host : IDisposable
{
    private readonly int port;
    private readonly ManualResetEvent running = new(false);
    private bool isDisposed;

    private Thread hostThread;

    public Host(int port = 23)
    {
        this.port = port;

        Console.CancelKeyPress += (_, _) => running.Reset();
    }

    public void Start()
    {
        hostThread = new Thread(Run)
        {
            IsBackground = true,
            Priority = ThreadPriority.Normal
        };

        hostThread.Start();
    }

    private void Run()
    {
        using PublisherSocket publisher = new();

        publisher.Bind(@"tcp://127.0.0.1:17232");

        using SubscriberSocket subscriber = new();
        subscriber.Bind(@"tcp://127.0.0.1:17233");
        subscriber.SubscribeToAnyTopic();

        running.Set();

        using TelnetServer server = new(IPAddress.Any, port, publisher)
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
        if (!isDisposed)
        {
            if (disposing)
            {
                running?.Dispose();
            }

            isDisposed = true;
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
