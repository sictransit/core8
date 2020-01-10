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

        public uint Address { get; private set; }

        public uint Data { get; private set; }

        protected abstract void Execute();

        protected abstract string OpCodeText { get; }

        protected IRegisters Registers { get; }

        public void Load(uint address, uint data)
        {
            Address = address;
            Data = data;
        }

        public void LoadAndExecute(uint address, uint data)
        {
            Load(address, data);

            Execute();
        }

        public override string ToString()
        {
            return $"{Address.ToOctalString()}:{Data.ToOctalString()} {OpCodeText}";
        }
    }
}
