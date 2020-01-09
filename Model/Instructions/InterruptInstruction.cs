using Core8.Model.Enums;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;

namespace Core8.Model.Instructions
{
    public class InterruptInstruction : InstructionBase
    {
        private readonly IProcessor processor;
        private readonly IRegisters registers;

        public InterruptInstruction(uint address, uint data, IProcessor processor, IRegisters registers) : base(address, data)
        {
            this.processor = processor;
            this.registers = registers;
        }

        protected override string OpCodeText => OpCode.ToString();

        private InterruptOpCode OpCode => (InterruptOpCode)(Data & Masks.IO_OPCODE);

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
                default:
                    break;
            }
        }

        private void SKON()
        {
            if (processor.InterruptsEnabled)
            {
                registers.IF_PC.Increment();
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
    }
}
