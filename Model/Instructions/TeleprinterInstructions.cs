using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;

namespace Core8.Model.Instructions
{
    public class TeleprinterInstructions : TeleptypeInstructionsBase
    {
        private const int TFL_MASK = 0;
        private const int TSF_MASK = 1 << 0;
        private const int TCF_MASK = 1 << 1;
        private const int TPC_MASK = 1 << 2;
        private const int TSK_MASK = 1 << 2 | 1 << 0;
        private const int TLS_MASK = TCF_MASK | TPC_MASK;

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
                    TCF();
                    TPC();
                    break;
                case TCF_MASK:
                    TCF();
                    break;
                case TPC_MASK:
                    TPC();
                    break;
                case TSK_MASK:
                    TSK();
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

        private void TCF()
        {
            Teletype.ClearOutputFlag();
        }

        private void TPC()
        {
            var c = Registers.AC.Accumulator & Masks.TELEPRINTER_BUFFER_MASK;

            Teletype.Type((byte)c);
        }

        private void TSK()
        {
            if (Teletype.OutputFlag || Teletype.InputFlag)
            {
                Registers.PC.Increment();
            }
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
            TSK = TSK_MASK,
            TLS = TLS_MASK
        }
    }
}
