using Core8.Model.Enums;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;

namespace Core8.Model.Instructions
{
    public class TeleprinterInstruction : InstructionBase
    {
        private readonly IRegisters registers;
        private readonly ITeleprinter teleprinter;

        public TeleprinterInstruction(uint address, uint data, IRegisters registers, ITeleprinter teleprinter) : base(address, data)
        {
            this.registers = registers;
            this.teleprinter = teleprinter;
        }

        protected override string OpCodeText => OpCode.ToString();

        private TeleprinterOpCode OpCode => (TeleprinterOpCode)(Data & Masks.IO_OPCODE);

        public override void Execute()
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
            var c = registers.LINK_AC.Accumulator & Masks.TELEPRINTER_BUFFER_MASK;

            teleprinter.Type((byte)c);

            teleprinter.ClearFlag();
        }

        private void TSF()
        {
            if (teleprinter.IsFlagSet)
            {
                registers.IF_PC.Increment();
            }
        }
    }
}
