using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;

namespace Core8.Model.Instructions
{
    public class InterruptInstructions : PrivilegedInstructionsBase
    {
        private const int SKON_MASK = 0b_000;
        private const int ION_MASK = 0b_001;
        private const int IOF_MASK = 0b_010;
        private const int SRQ_MASK = 0b_011;
        private const int GTF_MASK = 0b_100;
        private const int RTF_MASK = 0b_101;
        private const int SGT_MASK = 0b_110;
        private const int CAF_MASK = 0b_111;

        public InterruptInstructions(ICPU cpu) : base(cpu)
        {
        }

        protected override string OpCodeText => ((InterruptOpCode)(Data & Masks.INTERRUPT_FLAGS)).ToString();

        protected override void PrivilegedExecute()
        {
            switch (Data & Masks.INTERRUPT_FLAGS)
            {
                case SKON_MASK:
                    SKON();
                    break;
                case ION_MASK:
                    ION();
                    break;
                case IOF_MASK:
                    IOF();
                    break;
                case SRQ_MASK:
                    SRQ();
                    break;
                case GTF_MASK:
                    GTF();
                    break;
                case RTF_MASK:
                    RTF();
                    break;
                case SGT_MASK:
                    SGT();
                    break;
                case CAF_MASK:
                    CAF();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void SKON()
        {
            if (Interrupts.Enabled)
            {
                Registers.PC.Increment();
            }

            Interrupts.Disable();
        }

        private void ION()
        {
            Interrupts.Enable();
        }

        private void IOF()
        {
            Interrupts.Disable();
        }

        private void SRQ()
        {
            if (Interrupts.Requested)
            {
                Registers.PC.Increment();
            }
        }

        private void GTF()
        {
            var acc = (Registers.AC.Link << 11) | ((Interrupts.Requested ? 1 : 0) << 9) | ((Interrupts.Pending ? 1 : 0) << 7) | Registers.SF.Content;

            Registers.AC.SetAccumulator(acc);
        }

        private void SGT()
        {
            // TODO: EAE support

            throw new NotImplementedException();
        }

        private void RTF()
        {
            var acc = Registers.AC.Accumulator;

            Registers.AC.SetLink((acc >> 11) & Masks.FLAG);

            Registers.IB.SetIB(acc >> 3);
            Registers.DF.SetDF(acc);
            Registers.UB.SetUB(acc >> 6);

            Interrupts.Enable(false);
            Interrupts.Inhibit();
        }

        private void CAF()
        {
            CPU.Clear();
        }

        private enum InterruptOpCode
        {
            SKON = SKON_MASK,
            ION = ION_MASK,
            IOF = IOF_MASK,
            SRQ = SRQ_MASK,
            GTF = GTF_MASK,
            RTF = RTF_MASK,
            SGT = SGT_MASK,
            CAF = CAF_MASK,
        }
    }
}
