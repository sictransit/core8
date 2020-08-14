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

        private readonly ConcurrentQueue<byte> reader = new ConcurrentQueue<byte>();

        private readonly PublisherSocket publisherSocket;
        private readonly SubscriberSocket subscriberSocket;

        private int deviceControl;

        private bool outputPending;
        private DateTime outputPendingAt;
        private DateTime inputPendingAt;

        private const int INTERRUPT_ENABLE = 1 << 0;
        private const int STATUS_ENABLE = 1 << 1;

        private bool inputIRQ;

        private bool outputIRQ;



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

        public byte InputBuffer { get; private set; }

        public byte OutputBuffer { get; private set; }

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

        public void ClearInputFlag()
        {
            InputFlag = inputIRQ = false;
        }

        private void SetInputFlag()
        {
            InputFlag = true;
            inputIRQ = (deviceControl & INTERRUPT_ENABLE) != 0;
        }

        public void ClearOutputFlag()
        {
            OutputFlag = outputIRQ = false;
        }

        public void SetOutputFlag()
        {
            OutputFlag = true;
            outputIRQ = (deviceControl & INTERRUPT_ENABLE) != 0;
        }

        public void Type(byte c)
        {
            OutputBuffer = c;

            outputPending = true;
            outputPendingAt = DateTime.UtcNow.AddMilliseconds(100);

            Log.Information($"Paper: {c.ToPrintableAscii()}");
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

        public bool InterruptRequested => inputIRQ || outputIRQ;

        public override string ToString()
        {
            return $"[TT] if={(InputFlag ? 1 : 0)} ib={InputBuffer} of={(OutputFlag ? 1 : 0)} ob={OutputBuffer} irq/in={(inputIRQ ? 1 : 0)} irq/out={(outputIRQ ? 1 : 0)} (tq= {reader.Count})";
        }

        public void Tick()
        {
            if (!InputFlag && DateTime.UtcNow > inputPendingAt)
            {
                while (subscriberSocket.TryReceiveFrameBytes(TimeSpan.Zero, out var frame))
                {
                    foreach (var key in frame)
                    {
                        reader.Enqueue(key);
                    }

                    inputPendingAt = DateTime.UtcNow.AddMilliseconds(100);
                }

                if (reader.TryDequeue(out var b))
                {
                    Log.Debug($"Keyboard: {b.ToPrintableAscii()}");

                    InputBuffer = b;

                    SetInputFlag();                    
                }                
            }

            if (outputPending && DateTime.UtcNow > outputPendingAt)
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
