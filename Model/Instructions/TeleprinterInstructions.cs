using Core8.Model.Enums;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;

namespace Core8.Model.Instructions
{
    public class TeleprinterInstructions : InstructionsBase
    {
        private readonly ITeleprinter teleprinter;

        public TeleprinterInstructions(IRegisters registers, ITeleprinter teleprinter) : base(registers)
        {
            this.teleprinter = teleprinter;
        }

        protected override string OpCodeText => OpCode.ToString();

        private TeleprinterOpCode OpCode => (TeleprinterOpCode)(Data & Masks.IO_OPCODE);

        protected override void Execute()
        {
            switch (OpCode)
            {
                case TeleprinterOpCode.TLS:
                    TLS();
                    break;
                case TeleprinterOpCode.TSF:
                    TSF();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void TLS()
        {
            var c = Registers.LINK_AC.Accumulator & Masks.TELEPRINTER_BUFFER_MASK;

            teleprinter.Type((byte)c);

            teleprinter.ClearFlag();
        }

        private void TSF()
        {
            if (teleprinter.IsFlagSet)
            {
                Registers.IF_PC.Increment();
            }
        }
    }
}
