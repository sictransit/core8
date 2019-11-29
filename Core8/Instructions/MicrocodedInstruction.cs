using System;
using System.Collections.Generic;
using System.Text;

namespace Core8.Instructions
{
    public abstract class MicrocodedInstruction : InstructionBase
    {
        public MicrocodedInstruction(uint data) : base(data)
        {

        }
    }
}
