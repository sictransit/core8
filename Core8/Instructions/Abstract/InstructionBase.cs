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

        public void Execute(ICore core)
        {
            if (core is null)
            {
                throw new System.ArgumentNullException(nameof(core));
            }

            ExecuteInternal(core);
        }

        protected abstract void ExecuteInternal(ICore core);
    }
}
