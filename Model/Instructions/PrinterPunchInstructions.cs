using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;

namespace Core8.Model.Instructions
{
    public class PrinterPunchInstructions : PrivilegedInstructionsBase
    {
        private const int TFL_MASK = 0;
        private const int TSF_MASK = 1 << 0;
        private const int TCF_MASK = 1 << 1;
        private const int TPC_MASK = 1 << 2;
        private const int TSK_MASK = TPC_MASK | TSF_MASK;
        private const int TLS_MASK = TCF_MASK | TPC_MASK;

        public PrinterPunchInstructions(ICPU cpu) : base(cpu)
        {
        }

        protected override string OpCodeText => ((TeleprinterOpCode)(Data & 0b_111)).ToString();

        private IPrinterPunch Device => CPU.PrinterPunch;

        protected override void PrivilegedExecute()
        {
            switch (Data & 0b_111)
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
            Device.SetOutputFlag();
        }

        private void TCF()
        {
            Device.ClearOutputFlag();
        }

        private void TPC()
        {
            var c = AC.Accumulator & 0b_000_011_111_111;

            Device.Print((byte)c);
        }

        private void TSK()
        {
            if (Device.OutputFlag)
            {
                PC.Increment();
            }
        }

        private void TSF()
        {
            if (Device.OutputFlag)
            {
                PC.Increment();
            }
        }

        private enum TeleprinterOpCode
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
