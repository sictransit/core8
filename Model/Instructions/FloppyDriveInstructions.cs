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

        protected override string OpCodeText => ((FloppyDriveOpCode)(Data & Masks.FLOPPY_OPCODE)).ToString();

        protected override void PrivilegedExecute()
        {
            var floppy = CPU.GetFloppyDrive((Data & Masks.IO) >> 3);

            switch (Data & Masks.IO_OPCODE)
            {
                case SEL_MASK:
                    SEL(floppy);
                    break;
                case LCD_MASK:
                    TLS(floppy);
                    break;
                case XDR_MASK:
                    XDR(floppy);
                    break;
                case STR_MASK:
                    STR(floppy);
                    break;
                case SER_MASK:
                    SER(floppy);
                    break;
                case SDN_MASK:
                    SDN(floppy);
                    break;
                case INTR_MASK:
                    INTR(floppy);
                    break;
                case INIT_MASK:
                    INIT(floppy);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void INIT(IFloppyDrive floppy)
        {
            throw new NotImplementedException();
        }

        private void INTR(IFloppyDrive floppy)
        {
            throw new NotImplementedException();
        }

        private void SDN(IFloppyDrive floppy)
        {
            if (floppy.Done)
            {
                floppy.ClearDone();

                CPU.Registers.PC.Increment();
            }
        }

        private void SER(IFloppyDrive floppy)
        {
            throw new NotImplementedException();
        }

        private void STR(IFloppyDrive floppy)
        {
            if (floppy.TransferRequest)
            {
                floppy.ClearTransferRequest();

                CPU.Registers.PC.Increment();
            }
        }

        private void XDR(IFloppyDrive floppy)
        {
            throw new NotImplementedException();
        }

        private void TLS(IFloppyDrive floppy)
        {
            throw new NotImplementedException();
        }

        private void SEL(IFloppyDrive floppy)
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
