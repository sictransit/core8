﻿using Core8.Model;
using Core8.Model.Extensions;
using Core8.Model.Interfaces;
using Serilog;

namespace Core8
{
    public class Memory : IMemory
    {
        private readonly uint[] ram;

        public Memory(uint fields = 8)
        {
            Size = fields * 4096;

            ram = new uint[Size];
        }

        public uint Size { get; private set; }

        public uint Examine(uint address)
        {
            var content = ram[address];

            if (Log.IsEnabled(Serilog.Events.LogEventLevel.Verbose))
            {
                Log.Verbose($"[Read]: {address.ToOctalString(5)}:{content.ToOctalString()}");
            }

            return content;
        }

        public uint Read(uint address, bool indirect = false)
        {
            if (indirect && ((address & Masks.MEM_WORD) >= 8) && ((address & Masks.MEM_WORD) <= 15))
            {
                return Write(address, Examine(address) + 1);
            }

            return Examine(address);
        }

        public uint Write(uint address, uint data)
        {
            var value = data & Masks.MEM_WORD;

            ram[address] = value;

            if (Log.IsEnabled(Serilog.Events.LogEventLevel.Verbose))
            {
                Log.Verbose($"[Write]: {address.ToOctalString(5)}:{value.ToOctalString()}");
            }

            return value;
        }
    }
}
