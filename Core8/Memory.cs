using Core8.Model;
using Core8.Model.Extensions;
using Core8.Model.Interfaces;
using Serilog;

namespace Core8
{
    public class Memory : IMemory
    {
        private readonly uint[][] ram;

        public Memory(uint fields = 1)
        {
            ram = new uint[fields][];

            for (int f = 0; f < fields; f++)
            {
                ram[f] = new uint[4096];
            }

            Fields = fields;
        }

        public uint Size { get; private set; }

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

            return MB;
        }

        public void Write(uint field, uint address, uint data)
        {
            MB = data & Masks.MEM_WORD;

            ram[field][address] = MB;

            Log.Debug($"[Write]: {address.ToOctalString()}:{MB.ToOctalString()}");
        }

        public override string ToString()
        {
            return string.Format($"[RAM] {MB.ToOctalString()}");
        }
    }
}
