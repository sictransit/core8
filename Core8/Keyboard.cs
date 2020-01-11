using Core8.Abstract;
using Core8.Model;
using Core8.Model.Interfaces;
using NetMQ;
using NetMQ.Sockets;
using Serilog;
using System;

namespace Core8
{
    public class Keyboard : IODevice, IKeyboard
    {
        private volatile uint buffer;
        private readonly SubscriberSocket subscriberSocket;

        public Keyboard()
        {
            subscriberSocket = new SubscriberSocket();

            subscriberSocket.Connect(@"tcp://127.0.0.1:17232");
            subscriberSocket.SubscribeToAnyTopic();
        }

        public uint GetBuffer()
        {
            var value = buffer & Masks.KEYBOARD_BUFFER_MASK;

            Log.Debug($"Keyboard buffer: {value:00} (dec)");

            return buffer;
        }

        public bool IsTapeLoaded => !Queue.IsEmpty;

        public override void Tick()
        {
            if (!IsFlagSet)
            {
                if (subscriberSocket.TryReceiveFrameBytes(TimeSpan.Zero, out var frame))
                {
                    Type(frame);
                }

                if (Queue.TryDequeue(out var item))
                {
                    buffer = item;

                    SetFlag();

                    Log.Debug($"Reader queue: {Queue.Count}");
                }
            }
        }
    }
}
