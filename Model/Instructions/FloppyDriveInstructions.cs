using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using Serilog;
using System;

namespace Core8.Model.Instructions
{
    internal class FloppyDriveInstructions : PrivilegedInstructionsBase
    {
        private const int SEL_MASK = 0b_000;
        private const int LCD_MASK = 0b_001;
        private const int XDR_MASK = 0b_010;
        private const int STR_MASK = 0b_011;
        private const int SER_MASK = 0b_100;
        private const int SDN_MASK = 0b_101;
        private const int INTR_MASK = 0b_110;
        private const int INIT_MASK = 0b_111;

        public FloppyDriveInstructions(ICPU cpu) : base(cpu)
        {

        }

        private IFloppyDrive FloppyDrive => CPU.FloppyDrive;

        protected override string OpCodeText => ((FloppyDriveOpCode)(Data & Masks.FLOPPY_OPCODE)).ToString();

        protected override void PrivilegedExecute()
        {
            switch (Data & Masks.IO_OPCODE)
            {
                case SEL_MASK:
                    SEL();
                    break;
                case LCD_MASK:
                    LCD();
                    break;
                case XDR_MASK:
                    XDR();
                    break;
                case STR_MASK:
                    STR();
                    break;
                case SER_MASK:
                    SER();
                    break;
                case SDN_MASK:
                    SDN();
                    break;
                case INTR_MASK:
                    INTR();
                    break;
                case INIT_MASK:
                    INIT();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void INIT()
        {
            FloppyDrive.Initialize();
        }

        private void INTR()
        {
            FloppyDrive.SetInterrupts(AC.Accumulator);
        }

        private void SDN()
        {
            if (FloppyDrive.SkipNotDone())
            {
                PC.Increment();
            }
        }

        private void SER()
        {
            if (FloppyDrive.SkipError())
            {
                PC.Increment();
            }
        }

        private void STR()
        {
            if (FloppyDrive.SkipTransferRequest())
            {
                PC.Increment();
            }
        }

        private void XDR()
        {
            AC.SetAccumulator(FloppyDrive.TransferDataRegister(AC.Accumulator));
        }

        private void LCD()
        {
            FloppyDrive.LoadCommandRegister(AC.Accumulator);

            AC.ClearAccumulator();
        }

        private void SEL()
        {
            Log.Warning("SEL is currently no-op:");
            Log.Warning(AC.ToString());
        }

        private enum FloppyDriveOpCode
        {
            SEL = SEL_MASK,
            LCD = LCD_MASK,
            XDR = XDR_MASK,
            STR = STR_MASK,
            SER = SER_MASK,
            SDN = SDN_MASK,
            INTR = INTR_MASK,
            INIT = INIT_MASK,
        }
    }
}
