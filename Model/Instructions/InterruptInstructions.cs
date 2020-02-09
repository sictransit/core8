using Core8.Model.Enums;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;

namespace Core8.Model.Instructions
{
    public class InterruptInstructions : PrivilegedInstructionsBase
    {
        public InterruptInstructions(IProcessor processor) : base(processor)
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
                Register.PC.Increment();
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
                Register.PC.Increment();
            }
        }

        private void GTF()
        {
            var acc = Register.AC.Link << 11;
            acc |= (uint)(Interrupts.Requested ? 1 : 0) << 9;
            acc |= (uint)(Interrupts.Pending ? 1 : 0) << 7;
            acc |= Register.SF.Data;

            Register.AC.SetAccumulator(acc);
        }

        private void SGT()
        {
            // TODO: EAE support
        }

        private void RTF()
        {
            var acc = Register.AC.Accumulator;

            Register.AC.SetLink((acc >> 11) & Masks.FLAG);

            Register.IB.SetIB(acc >> 3);
            Register.DF.SetDF(acc);
            Register.UB.SetUB(acc >> 6);

            Interrupts.Enable(false);
            Interrupts.Suspend();
        }

        private void CAF()
        {
            Processor.Clear();
        }
    }
}
