using Core8.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core8.Instructions
{
    public abstract class MemoryReferenceInstruction : InstructionBase
    {
        protected MemoryReferenceInstruction(ushort opCode, ushort address) : base((ushort)(((opCode << 9) & Masks.OP_CODE) | (address & Masks.ADDRESS_WORD)))
        {

        }

        public ushort OpCode => (ushort)((Data & Masks.OP_CODE) >> 9);

        public ushort I => (ushort)((Data & Masks.I_MODE) >> 8);

        public ushort Z => (ushort)((Data & Masks.Z_MODE) >> 7);

        public ushort Address => (ushort)(Data & Masks.ADDRESS_WORD);

    }

}
