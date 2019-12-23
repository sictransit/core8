using Core8.Model.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Core8.Abstract
{
    public abstract class IODevice : IIODevice
    {
        protected IODevice(uint id)
        {
            Id = id;
        }

        public uint Id { get; }

        private readonly ManualResetEvent flag = new ManualResetEvent(false);

        private readonly ManualResetEvent interruptRequested = new ManualResetEvent(false);

        protected ConcurrentQueue<byte> Queue { get; } = new ConcurrentQueue<byte>();

        public bool IsFlagSet => flag.WaitOne(TimeSpan.Zero);

        public bool InterruptRequested => interruptRequested.WaitOne(TimeSpan.Zero);

        public abstract void Tick();

        protected void SetFlag()
        {
            flag.Set();

            interruptRequested.Set();
        }

        public void ClearFlag()
        {
            flag.Reset();

            interruptRequested.Reset();
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
            foreach (var c in chars)
            {
                Type(c);
            }
        }
    }
}
