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

namespace Core8.Peripherals.Teletype
{
    public class ASR33 : ITeletype
    {
        private readonly List<byte> paper = new List<byte>();

        private int ticks;

        private const int TickDelay = 100;

        private readonly ConcurrentQueue<byte> reader = new ConcurrentQueue<byte>();

        private readonly PublisherSocket publisherSocket;
        private readonly SubscriberSocket subscriberSocket;

        private int deviceControl;

        private bool outputPending;

        private const int INTERRUPT_ENABLE = 1 << 0;
        private const int STATUS_ENABLE = 1 << 1;

        private bool InterruptEnable => ((deviceControl & INTERRUPT_ENABLE) != 0);

        private bool InputIRQ => InputFlag && InterruptEnable;

        private bool OutputIRQ => OutputFlag && InterruptEnable;

        public ASR33(string inputAddress, string outputAddress)
        {
            publisherSocket = new PublisherSocket();
            publisherSocket.Connect(outputAddress);

            subscriberSocket = new SubscriberSocket();
            subscriberSocket.Connect(inputAddress);
            subscriberSocket.SubscribeToAnyTopic();
        }

        public bool InputFlag { get; private set; }

        public bool OutputFlag { get; private set; }

        public byte InputBuffer { get; private set; }

        private byte OutputBuffer { get; set; }

        public void SetDeviceControl(int data)
        {
            deviceControl = data & (INTERRUPT_ENABLE | STATUS_ENABLE);
        }

        public void Clear()
        {
            SetDeviceControl(Masks.IO_DEVICE_CONTROL_MASK);

            ClearInputFlag();
            ClearOutputFlag();

            outputPending = false;

            reader.Clear();
        }

        public void ClearInputFlag() => InputFlag = false;

        private void SetInputFlag() => InputFlag = true;

        public void ClearOutputFlag() => OutputFlag = false;

        public void SetOutputFlag() => OutputFlag = true;

        public void Type(byte c)
        {
            OutputBuffer = c;

            outputPending = true;

            ticks = 0;

            Log.Debug($"Paper: {c.ToPrintableAscii()}");
        }

        public void MountPaperTape(byte[] chars)
        {
            if (chars is null)
            {
                throw new ArgumentNullException(nameof(chars));
            }

            reader.Clear();

            foreach (var c in chars)
            {
                reader.Enqueue(c);
            }
        }

        public string Printout => Encoding.ASCII.GetString(paper.ToArray());

        public bool InterruptRequested => InputIRQ || OutputIRQ;

        public override string ToString()
        {
            return $"[TT] if={(InputFlag ? 1 : 0)} ib={InputBuffer} of={(OutputFlag ? 1 : 0)} ob={OutputBuffer} irq/in={(InputIRQ ? 1 : 0)} irq/out={(OutputIRQ ? 1 : 0)} (tq= {reader.Count})";
        }

        public void Tick()
        {
            if (ticks++ < TickDelay)
            {
                return;
            }

            ticks = 0;

            if (!InputFlag)
            {
                while (subscriberSocket.TryReceiveFrameBytes(TimeSpan.Zero, out var frame))
                {
                    foreach (var key in frame)
                    {
                        reader.Enqueue(key);
                    }
                }

                if (reader.TryDequeue(out var b))
                {
                    Log.Debug($"Keyboard: {b.ToPrintableAscii()}");

                    InputBuffer = b;

                    SetInputFlag();
                }
            }

            if (outputPending)
            {
                paper.Add(OutputBuffer);

                if (!publisherSocket.TrySendFrame(new[] { OutputBuffer }))
                {
                    Log.Warning("Failed to send 0MQ frame.");
                }

                outputPending = false;

                SetOutputFlag();
            }
        }
    }
}
