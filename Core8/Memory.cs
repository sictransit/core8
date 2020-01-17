using Core8.Model;
using Core8.Model.Extensions;
using Core8.Model.Interfaces;
using Serilog;

namespace Core8
{
    public class Memory : IMemory
    {
        private readonly uint[] ram;

        public Memory()
        {
            ram = new uint[4096];
        }

        public uint Size { get; private set; }

        public uint MB { get; private set; }

        public uint Examine(uint address)
        {
            return ram[address];
        }

        public uint Read(uint address, bool indirect = false)
        {
            if (indirect && (address >= 8) && (address <= 15))
            {
                Write(address, Examine(address) + 1);
            }

            MB = Examine(address);

            return MB;
        }

        public void Write(uint address, uint data)
        {
            MB = data & Masks.MEM_WORD;

            ram[address] = MB;

            Log.Debug($"[Write]: {address.ToOctalString()}:{MB.ToOctalString()}");
        }

        public override string ToString()
        {
            return string.Format($"[RAM] {MB.ToOctalString()}");
        }
    }
}
