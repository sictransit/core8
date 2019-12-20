using Core8.Abstract;
using Core8.Model;
using Core8.Model.Interfaces;
using Serilog;

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
    }
}
