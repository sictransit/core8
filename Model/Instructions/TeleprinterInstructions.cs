using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;

namespace Core8.Model.Instructions
{
    public class TeleprinterInstructions : TeleptypeInstructionsBase
    {
        private const int TFL_MASK = 0b_000;
        private const int TSF_MASK = 0b_001;
        private const int TCF_MASK = 0b_010;
        private const int TPC_MASK = 0b_100;
        private const int TLS_MASK = 0b_110;

        public TeleprinterInstructions(ICPU cpu) : base(cpu)
        {
        }

        protected override string OpCodeText => ((TeleprinterOpCode)(Data & Masks.IO_OPCODE)).ToString();

        protected override void PrivilegedExecute()
        {
            switch (Data & Masks.IO_OPCODE)
            {
                case TFL_MASK:
                    TFL();
                    break;
                case TLS_MASK:
                    TLS();
                    break;
                case TCF_MASK:
                    TCF();
                    break;
                case TPC_MASK:
                    TPC();
                    break;
                case TSF_MASK:
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

        private enum TeleprinterOpCode : int
        {
            TFL = TFL_MASK,
            TSF = TSF_MASK,
            TCF = TCF_MASK,
            TPC = TPC_MASK,
            TLS = TLS_MASK
        }
    }
}
