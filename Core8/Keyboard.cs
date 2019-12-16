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
        private volatile uint buffer;
        
        public Keyboard(uint id) : base(id)
        { }

        public uint Buffer => buffer & Masks.KEYBOARD_BUFFER_MASK;

        public bool IsTapeLoaded => !Queue.IsEmpty;

        public override void Tick()
        {
            if (!IsFlagSet && Queue.TryDequeue(out var item))
            {
                buffer = item;

                Flag.Set();

                Log.Information($"Reader queue: {Queue.Count}");
            }
        }        

        public void Load(byte[] data)
        {
            foreach (var item in data)
            {
                Queue.Enqueue(item);
            }
        }
    }
}
