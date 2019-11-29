using Core8.Instructions;
using Core8.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core8
{
    public class Memory : IMemory
    {
        private readonly uint size;
        private readonly ushort[] ram;

        public Memory(uint size)
        {
            if (size < 4 * 1024 || size > 32 * 1024)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            ram = new ushort[size];
            this.size = size;
        }

        public void Load(uint address, InstructionBase instruction)
        {
            Write(address, instruction.Content);
        }

        public ushort Read(uint address)
        {
            if (address > size)
            {
                throw new ArgumentOutOfRangeException(nameof(address));
            }

            return (ushort)(ram[address] & Masks.MEM_WORD);
        }

        public void Write(uint address, ushort data)
        {
            if (address > size)
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
