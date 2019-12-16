using Core8.Abstract;
using Core8.Interfaces;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;

namespace Core8
{
    public class Teleprinter : IODevice, ITeleprinter
    {
        private readonly ManualResetEvent flag = new ManualResetEvent(false);

        private readonly ConcurrentQueue<byte> queue = new ConcurrentQueue<byte>();

        private readonly StringBuilder paper = new StringBuilder();

        public Teleprinter(uint id) : base(id)
        { }

        public string Printout => paper.ToString();

        public uint Flag => flag.WaitOne(TimeSpan.Zero) ? 1u : 0u;

        public bool IsFlagSet => Flag == 1u;

        public override void Tick()
        {
            if (!IsFlagSet && queue.TryDequeue(out var item))
            {
                var c = Encoding.ASCII.GetChars(new[] { item })[0];

                paper.Append(c);

                flag.Set();

                Log.Information(Printout);
            }
        }

        public void ClearFlag()
        {
            flag.Reset();
        }

        public void Print(byte c)
        {
            queue.Enqueue(c);
        }
    }
}
