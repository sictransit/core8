using System;
using System.Collections.Generic;
using System.Text;
using Core8.Enum;
using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions.MemoryReference
{
    public class AND : MemoryReferenceInstruction
    {
        public AND(uint address) : base((uint)InstructionName.AND, address)
        {

        }

        public override void Execute(ICore core)
        {
            var memory = core.Memory.Read(Address);

            var ac = core.Registers.LINK_AC.Accumulator;

            core.Registers.LINK_AC.SetAccumulator(memory & ac);
        }
    }
}
