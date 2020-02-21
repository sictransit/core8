using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

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
                    TLS();
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
            throw new NotImplementedException();
        }

        private void INTR()
        {
            throw new NotImplementedException();
        }

        private void SDN()
        {
            throw new NotImplementedException();
        }

        private void SER()
        {
            throw new NotImplementedException();
        }

        private void STR()
        {
            throw new NotImplementedException();
        }

        private void XDR()
        {
            throw new NotImplementedException();
        }

        private void TLS()
        {
            throw new NotImplementedException();
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
            INT = INTR_MASK,
            INI = INIT_MASK,
        }
    }
}
