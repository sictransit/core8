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

        protected void RRB(IEnvironment environment)
        {
            var acc = environment.Registers.LINK_AC.Accumulator;

            environment.Registers.LINK_AC.SetAccumulator(environment.Reader.Buffer | acc);
        }

        protected void RFC(IEnvironment environment)
        {
            if (environment is null)
            {
                throw new System.ArgumentNullException(nameof(environment));
            }

            environment.Reader.ClearReaderFlag();
        }
    }
}
