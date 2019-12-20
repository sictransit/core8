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

        protected ManualResetEvent Flag { get; } = new ManualResetEvent(false);

        protected ConcurrentQueue<byte> Queue { get; } = new ConcurrentQueue<byte>();

        public bool IsFlagSet => Flag.WaitOne(TimeSpan.Zero) ? true : false;

        public abstract void Tick();

        public void ClearFlag()
        {
            Flag.Reset();
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
