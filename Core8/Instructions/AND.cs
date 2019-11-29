using System;
using System.Collections.Generic;
using System.Text;
using Core8.Interfaces;

namespace Core8.Instructions
{
    public class AND : MemoryReferenceInstruction
    {
        public AND(ushort address) : base(0b_000, address)
        {

        }

        public override void Execute(ICore core)
        {
            var memory = core.Memory.Read(Address);

            var ac = core.Registers.LINK_AC.Accumulator;

            core.Registers.LINK_AC.SetAccumulator((ushort)(memory & ac));
        }
    }
}
