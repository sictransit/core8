using Core8.Extensions;
using Core8.Model.Interfaces;
using NetMQ;
using NetMQ.Sockets;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Core8.Peripherals.Teletype
{
    public class ASR33 : ITeletype
    {
        private readonly ConcurrentQueue<byte> output = new();

        private int ticks;

        private const int TICK_DELAY = 100;

        private readonly ConcurrentQueue<byte> reader = new();

        private readonly PublisherSocket publisherSocket;
        private readonly SubscriberSocket subscriberSocket;

        private int deviceControl;

        private const int INTERRUPT_ENABLE = 1 << 0;

        private bool InterruptEnable => (deviceControl & INTERRUPT_ENABLE) != 0;

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

        private byte? OutputBuffer { get; set; }

        public IReadOnlyCollection<byte> Output => output.ToArray();

        public void SetDeviceControl(int data)
        {
            deviceControl = data & INTERRUPT_ENABLE;
        }

        public void Clear()
        {
            SetDeviceControl(0b_000_000_000_011);

            ClearInputFlag();
            ClearOutputFlag();

            //TODO: Clear InputBuffer as well?
            OutputBuffer = null;

            //TODO: Clear Output as well?
            reader.Clear();

            ticks = 0;
        }

        public void ClearInputFlag() => InputFlag = false;

        private void SetInputFlag() => InputFlag = true;

        public void ClearOutputFlag() => OutputFlag = false;

        public void SetOutputFlag() => OutputFlag = true;

        public void Print(byte c)
        {
            if (OutputBuffer == null)
            {
                OutputBuffer = c;

                ticks = 0;

                Log.Debug($"Output: {c.ToPrintableAscii()}");
            }
            else
            {
                Log.Warning($"Type with char in buffer: {c.ToPrintableAscii()}");
            }
        }

        public void Type(byte c)
        {
            reader.Enqueue(c);
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

        public void RemovePaperTape()
        {
            reader.Clear();
        }

        public string Printout => Encoding.ASCII.GetString(output.ToArray());

        public bool InterruptRequested => InputIRQ || OutputIRQ;

        public override string ToString()
        {
            return $"[TT] if={(InputFlag ? 1 : 0)} ib={InputBuffer} of={(OutputFlag ? 1 : 0)} ob={OutputBuffer} irq/in={(InputIRQ ? 1 : 0)} irq/out={(OutputIRQ ? 1 : 0)} (tq= {reader.Count})";
        }

        public void Tick()
        {
            if (ticks++ < TICK_DELAY)
            {
                return;
            }

            ticks = 0;

            HandleInput();

            HandleOutput();
        }

        private void HandleOutput()
        {
            if (!OutputBuffer.HasValue)
            {
                return;
            }

            output.Enqueue(OutputBuffer.Value);

            if (!publisherSocket.TrySendFrame(new[] { (byte)OutputBuffer }))
            {
                Log.Warning("Failed to send 0MQ frame.");
            }

            OutputBuffer = null;

            SetOutputFlag();
        }

        private void HandleInput()
        {
            if (InputFlag)
            {
                return;
            }

            while (subscriberSocket.TryReceiveFrameBytes(TimeSpan.Zero, out var frame))
            {
                foreach (var key in frame)
                {
                    reader.Enqueue(key);
                }
            }

            if (reader.TryDequeue(out var b))
            {
                Log.Debug($"Input: {b.ToPrintableAscii()}");

                InputBuffer = b;

                SetInputFlag();
            }
        }
    }
}
