using System;
using System.Collections.Generic;
using System.Text;
using Core8.Enum;
using Core8.Interfaces;

namespace Core8.Instructions
{
    public class CMA : MicrocodedInstruction
    {
        public CMA() : base((uint)InstructionName.Microcoded)
        {

        }

        public override void Execute(ICore core)
        {
            var acc = core.Registers.LINK_AC.Accumulator;

            core.Registers.LINK_AC.SetAccumulator(acc % Masks.AC);
        }
    }
}
