using Core8.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core8.Instructions
{
    public abstract class InstructionBase
    {
        public InstructionBase(uint data)
        {
            Data = data;
        }

        protected uint Data { get; private set; }

        public uint Content
        {
            get
            {
                return (Data & Masks.INSTRUCTION);
            }
        }

        public abstract void Execute(ICore core);
    }
}
