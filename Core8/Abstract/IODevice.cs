using Core8.Model;
using Core8.Model.Enums;
using Core8.Model.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Core8.Abstract
{
    public abstract class IODevice : IIODevice
    {
        private readonly ManualResetEvent flag = new ManualResetEvent(false);

        private Action irqHook;

        private IODeviceControls deviceControls;

        protected ConcurrentQueue<byte> Queue { get; } = new ConcurrentQueue<byte>();

        public bool IsFlagSet => flag.WaitOne(TimeSpan.Zero);

        public abstract void Tick();

        public virtual void SetDeviceControls(uint data)
        {
            deviceControls = (IODeviceControls)(data & Masks.IO_DEVICE_CONTROL_MASK);
        }

        public void SetFlag()
        {
            flag.Set();

            if (deviceControls.HasFlag(IODeviceControls.InterruptEnable))
            { 
                irqHook?.Invoke(); 
            }
        }

        public void ClearFlag()
        {
            flag.Reset();
        }

        public void Clear()
        {
            ClearFlag();

            Queue.Clear();
        }

        public virtual void Type(byte c)
        {
            Queue.Enqueue(c);
        }

        public void Type(byte[] chars)
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

        public void SetIRQHook(Action irq)
        {
            irqHook = irq;
        }

    }
}
