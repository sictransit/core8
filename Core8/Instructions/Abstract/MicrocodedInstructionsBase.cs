using Core8.Extensions;
using Core8.Interfaces;

namespace Core8.Instructions.Abstract
{
    public abstract class MicrocodedInstructionsBase : InstructionBase
    {
        public MicrocodedInstructionsBase(IRegisters registers)  
        {
            Registers = registers;
        }

        protected IRegisters Registers { get; }

        public abstract void Execute(uint data);
    }
}
