﻿using Core8.Extensions;
using Core8.Model;
using Core8.Model.Interfaces;
using NetMQ;
using NetMQ.Sockets;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Core8.Peripherals.Teletype
{
    public class ASR33 : ITeletype
    {
        private readonly List<char> paper = new List<char>();

        private int ticks;

        private const int TickDelay = 100;

        private readonly ConcurrentQueue<byte> reader = new ConcurrentQueue<byte>();

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

        private char? OutputBuffer { get; set; }

        public void SetDeviceControl(int data)
        {
            deviceControl = data & INTERRUPT_ENABLE;
        }

        public void Clear()
        {
            SetDeviceControl(Masks.IO_DEVICE_CONTROL_MASK);

            ClearInputFlag();
            ClearOutputFlag();

            OutputBuffer = null;

            reader.Clear();
        }

        public void ClearInputFlag() => InputFlag = false;

        private void SetInputFlag() => InputFlag = true;

        public void ClearOutputFlag() => OutputFlag = false;

        public void SetOutputFlag() => OutputFlag = true;

        public void Type(byte c)
        {
            if (OutputBuffer == null)
            {
                OutputBuffer = (char)c;

                ticks = 0;

                Log.Debug($"Paper: {c.ToPrintableAscii()}");
            }
            else
            {
                Log.Warning($"Type with char in buffer: {c.ToPrintableAscii()}");
            }
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

        public string Printout => new string(paper.ToArray());

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

            if (!HandleOutput())
            {
                HandleInput();
            }            
        }

        private bool HandleOutput()
        {
            if (OutputBuffer.HasValue)
            {
                paper.Add(OutputBuffer.Value);

                if (!publisherSocket.TrySendFrame(new[] { (byte)OutputBuffer }))
                {
                    Log.Warning("Failed to send 0MQ frame.");
                }

                OutputBuffer = null;

                SetOutputFlag();

                return true;
            }

            return false;
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
                Log.Debug($"Keyboard: {b.ToPrintableAscii()}");

                InputBuffer = b;

                SetInputFlag();
            }            
        }
    }
}
