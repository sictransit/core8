using Core8.Model;
using Core8.Model.Enums;
using Core8.Model.Interfaces;
using NetMQ;
using NetMQ.Sockets;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;

namespace Core8
{
    public class Teleprinter : ITeleprinter
    {
        private readonly StringBuilder paper = new StringBuilder();

        private readonly PublisherSocket publisherSocket;
        private readonly SubscriberSocket subscriberSocket;

        private volatile uint buffer;

        private readonly ManualResetEvent inputFlag = new ManualResetEvent(false);
        private readonly ManualResetEvent outputFlag = new ManualResetEvent(false);

        private ConcurrentQueue<byte> inputQueue { get; } = new ConcurrentQueue<byte>();
        private ConcurrentQueue<byte> outputQueue { get; } = new ConcurrentQueue<byte>();

        private Action irqHook = null;

        private IODeviceControls deviceControls;

        public Teleprinter()
        {
            publisherSocket = new PublisherSocket();
            publisherSocket.Connect(@"tcp://127.0.0.1:17233");

            subscriberSocket = new SubscriberSocket();
            subscriberSocket.Connect(@"tcp://127.0.0.1:17232");
            subscriberSocket.SubscribeToAnyTopic();
        }

        public bool IsInputFlagSet => inputFlag.WaitOne(TimeSpan.Zero);

        public bool IsOutputFlagSet => outputFlag.WaitOne(TimeSpan.Zero);

        public void SetDeviceControls(uint data)
        {
            deviceControls = (IODeviceControls)(data & Masks.IO_DEVICE_CONTROL_MASK);
        }

        public void ClearInputFlag()
        {
            inputFlag.Reset();
        }

        public void ClearOutputFlag()
        {
            outputFlag.Reset();
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


        public void SetIRQHook(Action irq)
        {
            irqHook = irq;
        }

        public bool IsTapeLoaded => !inputQueue.IsEmpty;

        public uint GetBuffer()
        {
            var value = buffer & Masks.KEYBOARD_BUFFER_MASK;

            Log.Debug($"Keyboard buffer: {value:00} (dec)");

            return buffer;
        }

        public string Printout => paper.ToString();

        public void SetInputFlag()
        {
            if (deviceControls.HasFlag(IODeviceControls.InterruptEnable))
            {
                irqHook?.Invoke();
            }

            inputFlag.Set();
        }

        public void SetOutputFlag()
        {
            if (deviceControls.HasFlag(IODeviceControls.InterruptEnable))
            {
                irqHook?.Invoke();
            }

            outputFlag.Set();
        }

        public void FormFeed()
        {
            paper.Clear();
        }

        public void Tick()
        {
            if (!IsInputFlagSet)
            {
                if (subscriberSocket.TryReceiveFrameBytes(TimeSpan.Zero, out var frame))
                {
                    Type(frame);
                }

                if (inputQueue.TryDequeue(out var input))
                {
                    buffer = input;

                    SetInputFlag();

                    Log.Debug($"Reader queue: {inputQueue.Count}");
                }
            }

            if (!IsOutputFlagSet && outputQueue.TryDequeue(out var output))
            {
                var data = new[] { output };

                if (!publisherSocket.TrySendFrame(data))
                {
                    Log.Debug("Failed to send frame.");
                }


                var c = Encoding.ASCII.GetChars(data)[0];
                paper.Append(c);

                SetOutputFlag();
            }
        }

    }
}
