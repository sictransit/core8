﻿using Core8.Model.Interfaces;
using NetMQ;
using NetMQ.Sockets;
using Serilog;
using System;
using System.Threading;

namespace Core8
{
    public class MQRelay
    {
        private readonly ITeletype teletype;

        private readonly PublisherSocket publisherSocket;
        private readonly SubscriberSocket subscriberSocket;

        private readonly Thread publisherThread;
        private readonly Thread subscriberThread;

        private bool publishing;
        private bool subscribing;

        public MQRelay(ITeletype teletype)
        {
            this.teletype = teletype;

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
        }

        public void Start()
        {
            publisherThread.Start();
            subscriberThread.Start();
        }

        public void Stop()
        {
            publishing = false;
            subscribing = false;

            publisherThread.Join();
            subscriberThread.Join();
        }

        private void Publish()
        {
            publishing = true;

            publisherSocket.Connect(@"tcp://127.0.0.1:17233");

            while (publishing)
            {
                if (teletype.CachedDataAvailableEvent.WaitOne(TimeSpan.FromMilliseconds(200)))
                {
                    if (!publisherSocket.TrySendFrame(teletype.GetCachedOutput()))
                    {
                        Log.Debug("Failed to send frame.");
                    }
                }
            }
        }

        private void Subscribe()
        {
            subscribing = true;

            subscriberSocket.Connect(@"tcp://127.0.0.1:17232");
            subscriberSocket.SubscribeToAnyTopic();

            while (subscribing)
            {
                if (subscriberSocket.TryReceiveFrameBytes(TimeSpan.FromMilliseconds(100), out var frame))
                {
                    teletype.Read(frame[^1]);
                }

            }
        }

    }
}
