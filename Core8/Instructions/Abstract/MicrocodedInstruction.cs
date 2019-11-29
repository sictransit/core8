using Core8.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core8.Instructions.Abstract
{
    public abstract class MicrocodedInstruction : InstructionBase
    {
        public MicrocodedInstruction() : base((uint)InstructionName.Microcoded)
        {

        }
    }
}
