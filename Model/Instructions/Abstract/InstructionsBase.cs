using Core8.Model.Extensions;
using Core8.Model.Interfaces;

namespace Core8.Model.Instructions.Abstract
{
    public abstract class InstructionsBase : IInstruction
    {
        protected InstructionsBase(IProcessor processor)
        {
            Processor = processor;
        }

        public uint Address { get; private set; }

        public uint Data { get; private set; }

        public abstract void Execute();

        protected abstract string OpCodeText { get; }

        protected IProcessor Processor { get; }

        protected IRegisters Register => Processor.Registers;

        protected IInterrupts Interrupts => Processor.Interrupts;

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
