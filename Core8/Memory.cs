using Core8.Interfaces;
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

        public uint Read(uint address)
        {
            if (address > Size)
            {
                throw new ArgumentOutOfRangeException(nameof(address));
            }

            return ram[address] & Masks.MEM_WORD;
        }

        public void Write(uint address, uint data)
        {
            if (address > Size)
            {
                throw new ArgumentOutOfRangeException(nameof(address));
            }

            if (data > Masks.MEM_WORD)
            {
                throw new ArgumentOutOfRangeException(nameof(data));
            }

            ram[address] = data;
        }
    }
}
