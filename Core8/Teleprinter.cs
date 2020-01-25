using Core8.Model;
using Core8.Model.Enums;
using Core8.Model.Interfaces;
using NetMQ;
using NetMQ.Sockets;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Text;

namespace Core8
{
    public class Teleprinter : ITeleprinter
    {
        private readonly StringBuilder paper = new StringBuilder();

        private readonly PublisherSocket publisherSocket;
        private readonly SubscriberSocket subscriberSocket;

        private volatile uint buffer;

        private ConcurrentQueue<byte> inputQueue { get; } = new ConcurrentQueue<byte>();
        private ConcurrentQueue<byte> outputQueue { get; } = new ConcurrentQueue<byte>();

        private Action<bool> irqHook = null;

        private IODeviceControls deviceControls;

        public Teleprinter()
        {
            publisherSocket = new PublisherSocket();
            publisherSocket.Connect(@"tcp://127.0.0.1:17233");

            subscriberSocket = new SubscriberSocket();
            subscriberSocket.Connect(@"tcp://127.0.0.1:17232");
            subscriberSocket.SubscribeToAnyTopic();
        }

        public bool InputFlag { get; private set; }

        public bool OutputFlag { get; private set; }

        public void SetDeviceControls(uint data)
        {
            deviceControls = (IODeviceControls)(data & Masks.IO_DEVICE_CONTROL_MASK);
        }

        public void ClearInputFlag()
        {
            irqHook?.Invoke(false);

            InputFlag = false;
        }

        public void ClearOutputFlag()
        {
            irqHook?.Invoke(false);

            OutputFlag = false;
        }

        public void Clear()
        {
            SetDeviceControls(Masks.IO_DEVICE_CONTROL_MASK);

            ClearInputFlag();
            ClearOutputFlag();

            inputQueue.Clear();
            outputQueue.Clear();
        }

        public void Type(byte c)
        {
            outputQueue.Enqueue(c);
        }

        private void Type(byte[] chars)
        {
            if (chars is null)
            {
                throw new ArgumentNullException(nameof(chars));
            }

            foreach (var c in chars)
            {
                Type(c);
            }
        }

        public void Read(byte[] chars)
        {
            if (chars is null)
            {
                throw new ArgumentNullException(nameof(chars));
            }

            foreach (var c in chars)
            {
                Read(c);
            }
        }

        public void Read(byte c)
        {
            inputQueue.Enqueue(c);
        }

        public void SetIRQHook(Action<bool> irq)
        {
            irqHook = irq;
        }

        public bool IsTapeLoaded => !inputQueue.IsEmpty;

        public uint GetBuffer()
        {
            var value = buffer & Masks.KEYBOARD_BUFFER_MASK;

            return buffer;
        }

        public string Printout => paper.ToString();

        public void SetInputFlag()
        {
            if (deviceControls.HasFlag(IODeviceControls.InterruptEnable))
            {
                irqHook?.Invoke(true);
            }

            InputFlag = true;
        }

        public void SetOutputFlag()
        {
            if (deviceControls.HasFlag(IODeviceControls.InterruptEnable))
            {
                irqHook?.Invoke(true);
            }

            OutputFlag = true;
        }

        public void FormFeed()
        {
            paper.Clear();
        }

        public void Tick()
        {
            if (!InputFlag)
            {
                if (subscriberSocket.TryReceiveFrameBytes(TimeSpan.Zero, out var frame))
                {
                    Read(frame);
                }

                if (inputQueue.TryDequeue(out var input))
                {
                    buffer = input;

                    SetInputFlag();

                    if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
                    {
                        Log.Debug($"Reader queue: {inputQueue.Count}");
                    }
                }
            }

            if (!OutputFlag && outputQueue.TryDequeue(out var output))
            {
                var data = new[] { output };

                if (!publisherSocket.TrySendFrame(data))
                {
                    Log.Warning("Failed to send frame.");
                }

                var c = Encoding.ASCII.GetChars(data)[0];
                paper.Append(c);

                SetOutputFlag();
            }
        }

    }
}
