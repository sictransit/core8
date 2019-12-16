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
        private readonly StringBuilder paper = new StringBuilder();

        public Teleprinter(uint id) : base(id)
        { }

        public string Printout => paper.ToString();        

        public override void Tick()
        {
            if (!IsFlagSet && Queue.TryDequeue(out var item))
            {
                var c = Encoding.ASCII.GetChars(new[] { item })[0];

                paper.Append(c);

                base.Flag.Set();

                Log.Information(Printout);
            }
        }

        public void Print(byte c)
        {
            Queue.Enqueue(c);
        }
    }
}
