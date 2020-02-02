using Core8.Model.Extensions;
using Core8.Model.Interfaces;

namespace Core8.Model.Instructions.Abstract
{
    public abstract class InstructionsBase : IInstruction
    {
        protected InstructionsBase(IRegisters registers, bool privileged = false)
        {
            Registers = registers;
            Privileged = privileged;
        }

        public uint Field { get; private set; }

        public uint Address { get; private set; }

        public uint Data { get; private set; }

        public virtual bool Privileged { get; }

        public abstract void Execute();

        protected abstract string OpCodeText { get; }

        protected IRegisters Registers { get; }

        public void Load(uint field, uint address, uint data)
        {
            Field = field;
            Address = address;
            Data = data;
        }

        public override string ToString()
        {
            return $"({Field.ToOctalString()}){Address.ToOctalString()}: {Data.ToOctalString()} {OpCodeText}";
        }
    }
}
