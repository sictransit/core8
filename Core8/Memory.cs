using Core8.Model;
using Core8.Model.Extensions;
using Core8.Model.Interfaces;
using Serilog;
using System;

namespace Core8
{
    public class Memory : IMemory
    {
        private readonly uint[] ram;

        public Memory(uint size)
        {
            if (size < 4 * 1024 || size > 32 * 1024)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            ram = new uint[size];
            Size = size;
        }

        public uint Size { get; private set; }

        public uint MB { get; private set; }

        public uint Examine(uint address)
        {
            return ram[address] & Masks.MEM_WORD;
        }

        public uint Read(uint address, bool indirect = false)
        {
            if (address > Size)
            {
                throw new ArgumentOutOfRangeException(nameof(address));
            }

            if (indirect && (address >= 8) && (address <= 15))
            {
                Write(address, Examine(address) + 1);
            }

            MB = Examine(address);

            return MB;
        }

        public void Write(uint address, uint data)
        {
            if (address > Size)
            {
                throw new ArgumentOutOfRangeException(nameof(address));
            }

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
