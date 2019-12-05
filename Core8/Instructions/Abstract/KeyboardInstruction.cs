using Core8.Enums;
using Core8.Extensions;
using Core8.Interfaces;

namespace Core8.Instructions.Abstract
{
    public abstract class KeyboardInstruction : InstructionBase
    {
        protected KeyboardInstruction(uint data) : base(data)
        {

        }

        public InstructionName OpCode => (InstructionName)Data;

        public override string ToString()
        {
            return $"{Data.ToOctalString()} {OpCode}";
        }
    }
}
