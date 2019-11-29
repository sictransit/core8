using Core8.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core8.Instructions
{
    public abstract class InstructionBase
    {
        public InstructionBase(ushort data)
        {
            Data = data;
        }

        protected ushort Data { get; private set; }

        public ushort Content
        {
            get
            {
                return (ushort)(Data & Masks.INSTRUCTION);
            }
        }

        public abstract void Execute(ICore core);
    }
}
