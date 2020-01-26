using Core8.Model;
using Core8.Model.Enums;
using Core8.Model.Interfaces;
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

        private readonly ConcurrentQueue<byte> tapeQueue = new ConcurrentQueue<byte>();

        private Action<bool> irqHook = null;

        private IODeviceControls deviceControls;

        public Teleprinter()
        {
            OutputAvailable = new AutoResetEvent(false);
        }

        public bool InputFlag { get; private set; }

        public bool OutputFlag { get; private set; }

        public byte InputBuffer { get; private set; }

        public byte OutputBuffer { get; private set; }

        public AutoResetEvent OutputAvailable { get; private set; }

        public void SetDeviceControls(uint data)
        {
            deviceControls = (IODeviceControls)(data & Masks.IO_DEVICE_CONTROL_MASK);
        }

        public void ClearInputFlag()
        {
            irqHook?.Invoke(false);

            InputFlag = false;

            if (tapeQueue.TryDequeue(out var b))
            {
                Read(b);
            }
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

            tapeQueue.Clear();

            OutputAvailable.Reset();
        }

        public void Type(byte c)
        {
            paper.Append((char)c);

            OutputBuffer = c;

            OutputAvailable.Set();

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

        public void SetIRQHook(Action<bool> irq)
        {
            irqHook = irq;
        }

        public bool IsTapeLoaded => !tapeQueue.IsEmpty;

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
    }
}
