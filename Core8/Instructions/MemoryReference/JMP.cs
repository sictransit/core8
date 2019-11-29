using System;
using System.Collections.Generic;
using System.Text;
using Core8.Enum;
using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions.MemoryReference
{
    public class JMP : MemoryReferenceInstruction
    {
        public JMP(uint address) : base((uint)InstructionName.JMP, address)
        {

        }

        public override void Execute(ICore core)
        {
            core.Registers.IF_PC.Jump(core.Memory.Read(Address));
        }
    }
}
