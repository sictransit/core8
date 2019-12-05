using Core8.Instructions.Abstract;
using Core8.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core8.Instructions
{
    public class NOP : InstructionBase
    {
        public NOP(uint data) : base(data)
        {

        }

        protected override void ExecuteInternal(IEnvironment environment)
        {
            // NOP
        }
    }
}
