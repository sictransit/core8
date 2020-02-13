using Core8.Model.Extensions;
using Core8.Model.Interfaces;

namespace Core8.Model.Instructions.Abstract
{
    public abstract class InstructionsBase : IInstruction
    {
        protected InstructionsBase(ICPU cpu)
        {
            CPU = cpu;
        }

        public uint Address { get; private set; }

        public uint Data { get; private set; }

        public abstract void Execute();

        protected abstract string OpCodeText { get; }

        protected ICPU CPU { get; }

        protected IRegisters Registers => CPU.Registers;

        protected IInterrupts Interrupts => CPU.Interrupts;

        public IInstruction Load(uint address, uint data)
        {
            Address = address;
            Data = data;

            return this;
        }

        public override string ToString()
        {
            return $"{Address.ToOctalString(5)}:{Data.ToOctalString()} {OpCodeText}";
        }
    }
}
