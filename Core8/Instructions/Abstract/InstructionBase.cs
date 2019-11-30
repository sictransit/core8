using Core8.Interfaces;

namespace Core8.Instructions.Abstract
{
    public abstract class InstructionBase
    {
        public InstructionBase(uint data)
        {
            Data = data;
        }

        protected uint Data { get; private set; }

        public uint Content
        {
            get
            {
                return Data & Masks.INSTRUCTION;
            }
        }

        public abstract void Execute(ICore core);
    }
}
