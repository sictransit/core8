using Core8.Interfaces;

namespace Core8.Instructions.Abstract
{
    public abstract class InstructionBase
    {
        protected InstructionBase(IRegisters registers)
        {
            Registers = registers;
        }

        protected IRegisters Registers { get; }

        public abstract void Execute(uint data);
    }
}
