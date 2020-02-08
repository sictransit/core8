using Core8.Model.Enums;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;

namespace Core8.Model.Instructions
{
    public class TeleprinterInstructions : TeleprinterInstructionsBase
    {
        public TeleprinterInstructions(IProcessor processor) : base(processor)
        {
        }

        protected override string OpCodeText => OpCode.ToString();

        private TeleprinterOpCode OpCode => (TeleprinterOpCode)(Data & Masks.IO_OPCODE);

        protected override void PrivilegedExecute()
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
            Teleprinter.SetOutputFlag();
        }

        private void TLS()
        {
            var c = Register.LINK_AC.Accumulator & Masks.TELEPRINTER_BUFFER_MASK;

            Teleprinter.Type((byte)c);

            Teleprinter.ClearOutputFlag();

            Teleprinter.InitiateOutput();
        }

        private void TCF()
        {
            Teleprinter.ClearOutputFlag();
        }

        private void TPC()
        {
            var c = Register.LINK_AC.Accumulator & Masks.TELEPRINTER_BUFFER_MASK;

            Teleprinter.Type((byte)c);

            Teleprinter.InitiateOutput();
        }

        private void TSF()
        {
            if (Teleprinter.OutputFlag)
            {
                Register.IF_PC.Increment();
            }
        }
    }
}
