using System;
using System.Collections.Generic;
using System.Text;
using Core8.Enum;
using Core8.Interfaces;

namespace Core8.Instructions
{
    public class CLA : MicrocodedInstruction
    {
        public CLA() : base((uint)InstructionName.Microcoded)
        {

        }

        public override void Execute(ICore core)
        {
            core.Registers.LINK_AC.SetAccumulator(0);
        }
    }
}
