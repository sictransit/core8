using Core8.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core8.Instructions
{
    public abstract class MemoryReferenceInstruction : InstructionBase
    {
        protected MemoryReferenceInstruction(uint opCode, uint address) : base((((opCode << 9) & Masks.OP_CODE) | (address & Masks.ADDRESS_WORD)))
        {
            
        }

        public uint OpCode => ((Data & Masks.OP_CODE) >> 9);

        public uint I => ((Data & Masks.I_MODE) >> 8);

        public uint Z => ((Data & Masks.Z_MODE) >> 7);

        public uint Address => (Data & Masks.ADDRESS_WORD);

    }

}
