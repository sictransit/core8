using System;
using System.Collections.Generic;
using System.Text;
using Core8.Interfaces;

namespace Core8.Instructions
{
    public class TAD : MemoryReferenceInstruction
    {
        public TAD(ushort address) : base(0b_001, address)
        {

        }

        public override void Execute(ICore core)
        {
            var memory = core.Memory.Read(Address);

            var ac = core.Registers.LINK_AC.Data;

            core.Registers.LINK_AC.Set((ushort)(ac + memory));
        }
    }
}
