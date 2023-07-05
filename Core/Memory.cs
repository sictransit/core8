using Core8.Extensions;
using Core8.Model.Interfaces;
using Serilog;

namespace Core8.Core
{
    public class Memory : IMemory
    {
        private readonly int[] ram;

        public Memory(int fields = 8)
        {
            Size = fields * 4096;

            ram = new int[Size];
        }

        public int Size { get; }

        public int Read(int address, bool indirect = false)
        {
            if (indirect && (address & 0b_111_111_111_000) == 0b_001_000)
            {
                return Write(address, ram[address] + 1);
            }

            if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
            {
                Log.Debug($"[MR] {address.ToOctalString()}:{ram[address].ToOctalString()}");
            }

            return ram[address];
        }

        public int Write(int address, int data)
        {
            var value = data & 0b_111_111_111_111;

            ram[address] = value;

            if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
            {
                Log.Debug($"[MW] {address.ToOctalString()}:{value.ToOctalString()}");
            }

            return value;
        }
    }
}
