using System;
using System.Collections.Generic;
using System.Text;
using Core8.Enum;
using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions.MemoryReference
{
    public class ISZ : MemoryReferenceInstruction
    {
        public ISZ(uint address) : base((uint)InstructionName.ISZ, address)
        {

        }

        public override void Execute(ICore core)
        {
            var value = core.Memory.Read(Address);

            value = value + 1 & Masks.MEM_WORD;

            core.Memory.Write(Address, value);

            if (value == 0)
            {
                core.Registers.IF_PC.Increment();
            }
        }
    }
}
