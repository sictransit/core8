using Core8.Abstract;
using Core8.Interfaces;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Core8
{
    public class Keyboard : IODevice, IKeyboard
    {
        private readonly ManualResetEvent flag = new ManualResetEvent(false);

        private volatile uint buffer;

        private readonly ConcurrentQueue<uint> queue = new ConcurrentQueue<uint>();

        public Keyboard(uint id) : base(id)
        { }

        public uint Buffer => buffer & Masks.KEYBOARD_BUFFER_MASK;

        public uint Flag => flag.WaitOne(TimeSpan.Zero) ? 1u : 0u;

        public bool IsFlagSet => Flag == 1u;

        public bool IsTapeLoaded => !queue.IsEmpty;

        public override void Tick()
        {
            if (!IsFlagSet && queue.TryDequeue(out var item))
            {
                buffer = item;

                flag.Set();

                Log.Information($"Reader queue: {queue.Count}");
            }
        }

        public void ClearFlag()
        {
            flag.Reset();
        }

        public void Load(byte[] data)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            foreach (var item in data)
            {
                queue.Enqueue(item);
            }
        }
    }
}
