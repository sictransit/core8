using Core8.Extensions;
using Core8.Interfaces;

namespace Core8.Instructions.Abstract
{
    public abstract class InstructionBase
    {
        protected InstructionBase(uint address, uint data)
        {
            Address = address;
            Data = data;
        }

        public uint Address { get; }

        public uint Data { get; }

        public abstract void Execute(IHardware hardware);

        protected abstract string OpCodeText { get; }

        public override string ToString()
        {
            return $"{Address.ToOctalString()}: {OpCodeText} ({Data.ToOctalString()})";
        }
    }
}
