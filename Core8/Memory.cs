using Core8.Model;
using Core8.Model.Extensions;
using Core8.Model.Interfaces;
using Serilog;

namespace Core8
{
    public class Memory : IMemory
    {
        private readonly uint[][] ram;

        public Memory(uint fields = 8)
        {
            ram = new uint[fields][];

            for (int f = 0; f < fields; f++)
            {
                ram[f] = new uint[4096];
            }

            Fields = fields;
        }

        public uint MB { get; private set; }

        public uint Fields { get; }

        public uint Examine(uint field, uint address)
        {
            return ram[field][address];
        }

        public uint Read(uint field, uint address, bool indirect = false)
        {
            if (indirect && (address >= 8) && (address <= 15))
            {
                Write(field, address, Examine(field, address) + 1);
            }

            MB = Examine(field, address);

            if (Log.IsEnabled(Serilog.Events.LogEventLevel.Verbose))
            {
                Log.Verbose($"[Read]: ({field.ToOctalString()}){address.ToOctalString()}:{MB.ToOctalString()}");
            }

            return MB;
        }

        public void Write(uint field, uint address, uint data)
        {
            MB = data & Masks.MEM_WORD;

            ram[field][address] = MB;

            if (Log.IsEnabled(Serilog.Events.LogEventLevel.Verbose))
            {
                Log.Verbose($"[Write]: ({field.ToOctalString()}){address.ToOctalString()}:{MB.ToOctalString()}");
            }
        }

        public override string ToString()
        {
            return string.Format($"[RAM] {MB.ToOctalString()}");
        }
    }
}
