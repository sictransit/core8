﻿using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions.Microcoded
{
    public class HLT : MicrocodedInstruction
    {
        public HLT() : base(1, Masks.GROUP_2_HLT)
        {

        }

        public override void Execute(ICore core)
        {
            core.Processor.Halt();
        }
    }
}
