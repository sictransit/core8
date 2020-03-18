using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;

namespace Core8.Model.Instructions
{
    public class FloppyDriveInstructions : PrivilegedInstructionsBase
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
            FloppyDrive.SetInterrupts(Registers.AC.Accumulator);
        }

        private void SDN()
        {
            if (FloppyDrive.SkipNotDone())
            {
                Registers.PC.Increment();
            }
        }

        private void SER()
        {
            if (FloppyDrive.SkipError())
            {
                Registers.PC.Increment();
            }
        }

        private void STR()
        {
            if (FloppyDrive.SkipTransferRequest())
            {
                CPU.Registers.PC.Increment();
            }
        }

        private void XDR()
        {
            Registers.AC.SetAccumulator(FloppyDrive.TransferDataRegister(Registers.AC.Accumulator));
        }

        private void LCD()
        {
            Registers.AC.SetAccumulator(FloppyDrive.LoadCommandRegister(Registers.AC.Accumulator));
        }

        private void SEL()
        {
            throw new NotImplementedException();
        }

        private enum FloppyDriveOpCode : int
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
