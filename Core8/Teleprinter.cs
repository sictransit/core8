using Core8.Abstract;
using Core8.Model.Interfaces;
using Serilog;
using System.Text;

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

        public void Output(byte data)
        {
            Queue.Enqueue(data);
        }
    }
}
