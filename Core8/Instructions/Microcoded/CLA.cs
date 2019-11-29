using System;
using System.Collections.Generic;
using System.Text;
using Core8.Enum;
using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions.Microcoded
{
    public class CLA : MicrocodedInstruction
    {
        public override void Execute(ICore core)
        {
            core.Registers.LINK_AC.SetAccumulator(0);
        }
    }
}
