using System;
using System.Collections.Generic;
using System.Text;
using Core8.Interfaces;

namespace Core8.Instructions
{
    public class HLT : MicrocodedInstruction
    {
        public HLT() : base(0b_1111_000000_10)
        {

        }

        public override void Execute(ICore core)
        {
            core.Processor.Halt();
        }
    }
}
