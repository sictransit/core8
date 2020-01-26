using Core8.Model;
using Core8.Model.Enums;
using Core8.Model.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Core8
{
    public class Teleprinter : ITeleprinter
    {
        private readonly List<byte> paper = new List<byte>();

        private readonly ConcurrentQueue<byte> tapeQueue = new ConcurrentQueue<byte>();

        private readonly ConcurrentQueue<byte> outputCache = new ConcurrentQueue<byte>();

        private IODeviceControls deviceControls;

        public Teleprinter()
        {
            CachedDataAvailableEvent = new AutoResetEvent(false);
        }

        public bool InputFlag { get; private set; }

        public bool OutputFlag { get; private set; }

        public bool InterruptRequested { get; private set; }

        public byte InputBuffer { get; private set; }

        public byte OutputBuffer { get; private set; }

        public AutoResetEvent CachedDataAvailableEvent { get; private set; }

        private void RequestInterrupt()
        {
            if (deviceControls.HasFlag(IODeviceControls.InterruptEnable))
            {
                InterruptRequested = true;
            }
        }

        public byte[] GetCachedOutput()
        {
            var buffer = new List<byte>();

            while (outputCache.TryDequeue(out var b))
            {
                buffer.Add(b);
            }

            return buffer.ToArray();
        }

        public void SetDeviceControls(uint data)
        {
            deviceControls = (IODeviceControls)(data & Masks.IO_DEVICE_CONTROL_MASK);
        }

        public void ClearInputFlag()
        {
            InputFlag = InterruptRequested = false;

            if (tapeQueue.TryDequeue(out var b))
            {
                Read(b);
            }
        }

        public void ClearOutputFlag()
        {
            OutputFlag = InterruptRequested = false;
        }

        public void Clear()
        {
            SetDeviceControls(Masks.IO_DEVICE_CONTROL_MASK);

            ClearInputFlag();
            ClearOutputFlag();

            tapeQueue.Clear();

            CachedDataAvailableEvent.Reset();
        }

        public void Type(byte c)
        {
            OutputBuffer = c;
        }

        public void InitiateOutput()
        {
            paper.Add(OutputBuffer);

            outputCache.Enqueue(OutputBuffer);

            CachedDataAvailableEvent.Set();

            SetOutputFlag();
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

        public void Read(byte c)
        {
            if (!InputFlag)
            {
                InputBuffer = (byte)(c & Masks.KEYBOARD_BUFFER_MASK);

                SetInputFlag();
            }
        }

        public string Printout => Encoding.ASCII.GetString(paper.ToArray());

        public void SetInputFlag()
        {
            InputFlag = true;

            RequestInterrupt();
        }

        public void SetOutputFlag()
        {
            OutputFlag = true;

            RequestInterrupt();
        }
    }
}
