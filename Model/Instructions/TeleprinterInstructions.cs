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

        public override void Execute()
        {
            switch (OpCode)
            {
                case TeleprinterOpCode.TFL:
                    TFL();
                    break;
                case TeleprinterOpCode.TLS:
                    TLS();
                    break;
                case TeleprinterOpCode.TCF:
                    TCF();
                    break;
                case TeleprinterOpCode.TPC:
                    TPC();
                    break;
                case TeleprinterOpCode.TSF:
                    TSF();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void TFL()
        {
            teleprinter.SetOutputFlag();
        }

        private void TLS()
        {
            var c = Registers.LINK_AC.Accumulator & Masks.TELEPRINTER_BUFFER_MASK;

            teleprinter.Type((byte)c);

            teleprinter.ClearOutputFlag();

            teleprinter.InitiateOutput();
        }

        private void TCF()
        {
            teleprinter.ClearOutputFlag();
        }

        private void TPC()
        {
            var c = Registers.LINK_AC.Accumulator & Masks.TELEPRINTER_BUFFER_MASK;

            teleprinter.Type((byte)c);

            teleprinter.InitiateOutput();
        }

        private void TSF()
        {
            if (teleprinter.OutputFlag)
            {
                Registers.IF_PC.Increment();
            }
        }
    }
}
