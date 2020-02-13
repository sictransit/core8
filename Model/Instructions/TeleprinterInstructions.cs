using Core8.Model.Enums;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;

namespace Core8.Model.Instructions
{
    public class TeleprinterInstructions : TeleptypeInstructionsBase
    {
        public TeleprinterInstructions(ICPU cpu) : base(cpu)
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
            Teletype.SetOutputFlag();
        }

        private void TLS()
        {
            var c = Registers.AC.Accumulator & Masks.TELEPRINTER_BUFFER_MASK;

            Teletype.Type((byte)c);

            Teletype.ClearOutputFlag();

            Teletype.InitiateOutput();
        }

        private void TCF()
        {
            Teletype.ClearOutputFlag();
        }

        private void TPC()
        {
            var c = Registers.AC.Accumulator & Masks.TELEPRINTER_BUFFER_MASK;

            Teletype.Type((byte)c);

            Teletype.InitiateOutput();
        }

        private void TSF()
        {
            if (Teletype.OutputFlag)
            {
                Registers.PC.Increment();
            }
        }
    }
}
