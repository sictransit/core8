using Core8.Enums;
using Core8.Extensions;

namespace Core8.Instructions.Abstract
{
    public abstract class TeleprinterInstruction : InstructionBase
    {
        protected TeleprinterInstruction(uint data) : base(data)
        {

        }

        public InstructionName OpCode => (InstructionName)Data;

        public override string ToString()
        {
            return $"{Data.ToOctalString()} {OpCode}";
        }
    }
}
