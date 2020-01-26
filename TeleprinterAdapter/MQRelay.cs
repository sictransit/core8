using Core8.Model.Interfaces;
using NetMQ;
using NetMQ.Sockets;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Core8
{
    public class MQRelay
    {
        private readonly ITeleprinter teleprinter;

        private readonly PublisherSocket publisherSocket;
        private readonly SubscriberSocket subscriberSocket;

        private readonly Thread publisherThread;
        private readonly Thread subscriberThread;


        public MQRelay(ITeleprinter teleprinter)
        {
            this.teleprinter = teleprinter;

            publisherSocket = new PublisherSocket();
            subscriberSocket = new SubscriberSocket();

            publisherThread = new Thread(Publish)
            {
                IsBackground = true
            };

            subscriberThread = new Thread(Subscribe)
            {
                IsBackground = true
            };

            publisherThread.Start();
            subscriberThread.Start();
        }

        private void Publish()
        {
            publisherSocket.Connect(@"tcp://127.0.0.1:17233");

            while (true)
            {
                if (teleprinter.OutputAvailable.WaitOne(TimeSpan.FromMilliseconds(200)))
                {
                    if (!publisherSocket.TrySendFrame(new[] { (byte)teleprinter.OutputBuffer}))
                    {
                        Log.Debug("Failed to send frame.");
                    }
                }
            }
        }

        private void Subscribe()
        {
            subscriberSocket.Connect(@"tcp://127.0.0.1:17232");
            subscriberSocket.SubscribeToAnyTopic();

            while (true)
            {
                if (subscriberSocket.TryReceiveFrameBytes(TimeSpan.FromMilliseconds(200), out var frame))
                {
                    teleprinter.Read(frame[^1]);
                }

            }
        }

    }
}
