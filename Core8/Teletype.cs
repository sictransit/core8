using Core8.Model;
using Core8.Model.Extensions;
using Core8.Model.Interfaces;
using NetMQ;
using NetMQ.Sockets;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Core8
{
    public class Teletype : ITeletype
    {
        private readonly List<byte> paper = new List<byte>();

        private readonly ConcurrentQueue<byte> tapeQueue = new ConcurrentQueue<byte>();

        private readonly PublisherSocket publisherSocket;
        private readonly SubscriberSocket subscriberSocket;

        private int deviceControl;

        private int outputPending;

        private const int INTERRUPT_ENABLE = 1 << 0;
        private const int STATUS_ENABLE = 1 << 1;

        public Teletype(string inputAddress, string outputAddress)
        {
            publisherSocket = new PublisherSocket();
            publisherSocket.Connect(outputAddress);

            subscriberSocket = new SubscriberSocket();
            subscriberSocket.Connect(inputAddress);
            subscriberSocket.SubscribeToAnyTopic();
        }

        public bool InputFlag { get; set; }

        public bool OutputFlag { get; set; }

        public bool InterruptRequested => (InputFlag || OutputFlag) && ((deviceControl & INTERRUPT_ENABLE) != 0);

        public byte InputBuffer { get; private set; }

        public byte OutputBuffer { get; private set; }

        public void SetDeviceControl(int data)
        {
            deviceControl = data & (INTERRUPT_ENABLE | STATUS_ENABLE);
        }

        public void Clear()
        {
            SetDeviceControl(Masks.IO_DEVICE_CONTROL_MASK);

            InputFlag = OutputFlag = false;

            outputPending = 0;

            tapeQueue.Clear();
        }

        public void Type(byte c)
        {
            OutputBuffer = c;

            outputPending = 100;
        }

        public void MountPaperTape(byte[] chars)
        {
            if (chars is null)
            {
                throw new ArgumentNullException(nameof(chars));
            }

            tapeQueue.Clear();

            foreach (var c in chars)
            {
                tapeQueue.Enqueue(c);
            }
        }

        public string Printout => Encoding.ASCII.GetString(paper.ToArray());

        public override string ToString()
        {
            return $"[TT] if={(InputFlag ? 1 : 0)} ib={InputBuffer} of={(OutputFlag ? 1 : 0)} ob={OutputBuffer} irq={(InterruptRequested ? 1 : 0)} (tq: {tapeQueue.Count})";
        }

        public void Tick()
        {
            if (!InputFlag)
            {
                while (subscriberSocket.TryReceiveFrameBytes(TimeSpan.Zero, out var frame))
                {
                    foreach (var key in frame)
                    {
                        tapeQueue.Enqueue(key);
                    }
                }

                if (tapeQueue.TryDequeue(out var b))
                {
                    Log.Debug($"Keyboard: {b.ToPrintableAscii()}");

                    InputBuffer = b;

                    InputFlag = true;
                }
            }

            if (outputPending>0 && (--outputPending == 0))
            {
                paper.Add(OutputBuffer);

                if (!publisherSocket.TrySendFrame(new[] { OutputBuffer }))
                {
                    Log.Warning("Failed to send 0MQ frame.");
                }               

                OutputFlag = true;
            }
        }
    }
}
