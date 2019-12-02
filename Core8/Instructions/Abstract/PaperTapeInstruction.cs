using Core8.Enums;
using Core8.Extensions;
using Core8.Interfaces;

namespace Core8.Instructions.Abstract
{
    public abstract class PaperTapeInstruction : InstructionBase
    {
        protected PaperTapeInstruction(uint data) : base(data)
        {

        }

        public InstructionName OpCode => (InstructionName)Data;

        public override string ToString()
        {
            return $"{Data.ToOctalString()} {OpCode}";
        }

        protected void RFC(ICore core)
        {
            if (core is null)
            {
                throw new System.ArgumentNullException(nameof(core));
            }

            core.Reader.ClearReaderFlag();
        }
    }
}
