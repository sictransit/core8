using Core8.Model;
using Core8.Model.Interfaces;

namespace Core8
{
    public class Memory : IMemory
    {
        private readonly int[] ram;

        public Memory(int fields = 8)
        {
            Size = fields * 4096;

            ram = new int[Size];
        }

        public int Size { get; private set; }

        public int Read(int address, bool indirect = false)
        {
            if (indirect && ((address & Masks.MEM_WORD) <= 15) && ((address & Masks.MEM_WORD) >= 8))
            {
                return Write(address, ram[address] + 1);
            }

            return ram[address];
        }

        public int Write(int address, int data)
        {
            var value = data & Masks.MEM_WORD;

            ram[address] = value;

            return value;
        }
    }
}
