using Core8.Model.Extensions;
using Core8.Model.Interfaces;

namespace Core8.Model.Instructions.Abstract
{
    public abstract class InstructionsBase : IInstruction
    {
        protected InstructionsBase(IRegisters registers)
        {
            Registers = registers;
        }

        public uint Field { get; private set; }

        public uint Address { get; private set; }

        public uint Data { get; private set; }

        public abstract void Execute();

        protected bool UserMode => Registers.UF.Data != 0;

        public bool UserModeInterrupt { get; protected set; }

        protected abstract string OpCodeText { get; }

        protected IRegisters Registers { get; }

        public IInstruction Load(uint field, uint address, uint data)
        {
            Field = field;
            Address = address;
            Data = data;

            return this;
        }

        public override string ToString()
        {
            var irq = UserModeInterrupt ? " (privileged, interrupt)" : string.Empty;

            return $"({Field.ToOctalString(1)}){Address.ToOctalString()}:{Data.ToOctalString()} {OpCodeText}{irq}";
        }
    }
}
