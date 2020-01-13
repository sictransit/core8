using Core8.Model.Enums;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;

namespace Core8.Model.Instructions
{
    public class InterruptInstructions : InstructionsBase
    {
        private readonly IProcessor processor;

        internal InterruptInstructions(IProcessor processor, IRegisters registers) : base(registers)
        {
            this.processor = processor;
        }

        protected override string OpCodeText => OpCode.ToString();

        private InterruptOpCode OpCode => (InterruptOpCode)(Data & Masks.INTERRUPT_FLAGS);

        public override void Execute()
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
                case InterruptOpCode.CAF:
                    CAF();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void SKON()
        {
            if (processor.InterruptsEnabled)
            {
                Registers.IF_PC.Increment();
            }
        }

        private void ION()
        {
            processor.EnableInterrupts();
        }

        private void IOF()
        {
            processor.DisableInterrupts();
        }

        private void SRQ()
        {
            if (processor.InterruptRequested)
            {
                Registers.IF_PC.Increment();
            }
        }

        private void CAF()
        {
            processor.Clear();
        }

    }
}
