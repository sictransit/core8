using Core8.Enums;
using Core8.Instructions.Abstract;
using Core8.Interfaces;
using System;

namespace Core8.Instructions
{
    public class Teleprinter : InstructionBase
    {
        private readonly ITeleprinter teleprinter;

        public Teleprinter(IRegisters registers, ITeleprinter teleprinter) : base(registers)
        {
            this.teleprinter = teleprinter;
        }

        public override void Execute(uint data)
        {
            var opCode = (TeleprinterInstruction)(data & Masks.OP_CODE);

            switch (opCode)
            {
                case TeleprinterInstruction.TLS:
                    TLS();
                    break;
                case TeleprinterInstruction.TPC:
                    TPC();
                    break;
                case TeleprinterInstruction.TSF:
                    TSF();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public void TLS()
        {
            var c = Registers.LINK_AC.Accumulator & Masks.TELEPRINTER_BUFFER_MASK;

            teleprinter.Print((byte)c);

            teleprinter.ClearFlag();
        }

        public void TPC()
        {
            var c = Registers.LINK_AC.Accumulator & Masks.TELEPRINTER_BUFFER_MASK;

            teleprinter.Print((byte)c);
        }

        public void TSF()
        {
            if (teleprinter.IsFlagSet)
            {
                Registers.IF_PC.Increment();
            }
        }
    }
}
