using Core8.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core8.Instructions.Abstract
{
    public abstract class MicrocodedInstruction : InstructionBase
    {
        public MicrocodedInstruction(uint group, uint data) : base((((uint)InstructionName.Microcoded << 9) & Masks.OP_CODE) | ((group & Masks.FLAG)<<8) | (data &Masks.GROUP_1 ))
        {

        }
    }
}
