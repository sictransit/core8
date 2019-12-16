using Core8.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
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
    }
}
