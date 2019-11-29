using System;
using System.Collections.Generic;
using System.Text;
using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions.Microcoded
{
    public class HLT : MicrocodedInstruction
    {
        public override void Execute(ICore core)
        {
            core.Processor.Halt();
        }
    }
}
