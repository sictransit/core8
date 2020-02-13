using Core8.Model.Enums;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;

namespace Core8.Model.Instructions
{
    public class InterruptInstructions : PrivilegedInstructionsBase
    {
        public InterruptInstructions(ICPU cpu) : base(cpu)
        {
        }

        protected override string OpCodeText => OpCode.ToString();

        private InterruptOpCode OpCode => (InterruptOpCode)(Data & Masks.INTERRUPT_FLAGS);

        protected override void PrivilegedExecute()
        {
            switch (OpCode)
            {
                case InterruptOpCode.SKON:
                    SKON();
                    break;
                case InterruptOpCode.ION:
                    ION();
                    break;
                case InterruptOpCode.IOF:
                    IOF();
                    break;
                case InterruptOpCode.SRQ:
                    SRQ();
                    break;
                case InterruptOpCode.GTF:
                    GTF();
                    break;
                case InterruptOpCode.RTF:
                    RTF();
                    break;
                case InterruptOpCode.SGT:
                    SGT();
                    break;
                case InterruptOpCode.CAF:
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
            var acc = Registers.AC.Link << 11;
            acc |= (Interrupts.Requested ? 1 : 0) << 9;
            acc |= (Interrupts.Pending ? 1 : 0) << 7;
            acc |= Registers.SF.Content;

            Registers.AC.SetAccumulator(acc);
        }

        private void SGT()
        {
            // TODO: EAE support
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
    }
}
