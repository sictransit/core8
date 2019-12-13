using Core8.Instructions.Abstract;
using Core8.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core8.Instructions
{
    public class TeleprinterInstructions : InstructionBase
    {
        private readonly IRegisters registers;
        private readonly ITeleprinter teleprinter;

        public TeleprinterInstructions(IRegisters registers, ITeleprinter teleprinter) 
        {
            this.registers = registers;
            this.teleprinter = teleprinter;
        }

        public void TLS()
        {
            var c = registers.LINK_AC.Accumulator & Masks.TELEPRINTER_BUFFER_MASK;

            teleprinter.Print((byte)c);

            teleprinter.ClearFlag();
        }

        public void TPC()
        {
            var c = registers.LINK_AC.Accumulator & Masks.TELEPRINTER_BUFFER_MASK;

            teleprinter.Print((byte)c);
        }

        public void TSF()
        {
            if (teleprinter.IsFlagSet)
            {
                registers.IF_PC.Increment();
            }
        }
    }
}
