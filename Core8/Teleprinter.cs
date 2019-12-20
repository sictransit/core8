using Core8.Abstract;
using Core8.Model.Interfaces;
using System;
using System.Text;

namespace Core8
{
    public class Teleprinter : IODevice, ITeleprinter
    {
        private readonly StringBuilder paper = new StringBuilder();

        private Action<byte> callback;

        public Teleprinter(uint id) : base(id)
        { }

        public string Printout => paper.ToString();

        public void RegisterPrintCallback(Action<byte> callback)
        {
            this.callback = callback;
        }

        public override void Tick()
        {
            if (!IsFlagSet && Queue.TryDequeue(out var item))
            {
                var c = Encoding.ASCII.GetChars(new[] { item })[0];

                paper.Append(c);

                base.Flag.Set();
            }
        }

        public override void Type(byte c)
        {
            callback?.Invoke(c);

            base.Type(c);
        }
    }
}
