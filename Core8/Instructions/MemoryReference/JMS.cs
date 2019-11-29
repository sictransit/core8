using System;
using System.Collections.Generic;
using System.Text;
using Core8.Enum;
using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions.MemoryReference
{
    public class JMS : MemoryReferenceInstruction
    {
        public JMS(uint address) : base((uint)InstructionName.JMS, address)
        {

        }

        public override void Execute(ICore core)
        {
            core.Memory.Write(Address, core.Registers.IF_PC.Address);
        }
    }
}
