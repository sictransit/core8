using Core8.Extensions;

namespace Core8.Instructions.Abstract
{
    public abstract class MicrocodedInstruction : InstructionBase
    {
        public MicrocodedInstruction(uint data) : base(data)
        {
        }
        protected abstract string FlagString { get; }

        public override string ToString()
        {
            return $"{Data.ToOctal()} {FlagString}";
        }

    }
}
