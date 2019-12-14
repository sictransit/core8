using Core8.Enums;
using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions
{
    public class Group2Priv : InstructionBase
    {
        private readonly IProcessor processor;

        public Group2Priv(IRegisters registers, IProcessor processor) : base(registers)
        {
            this.processor = processor;
        }

        public override void Execute(uint data)
        {
            var flags = (PrivilegedGroupTwoInstructions)(data & Masks.PRIVILEGED_GROUP_2_FLAGS);

            if (flags.HasFlag(PrivilegedGroupTwoInstructions.OSR))
            {
                Registers.LINK_AC.SetAccumulator(Registers.LINK_AC.Accumulator | Registers.Switch.Get);
            }

            if (flags.HasFlag(PrivilegedGroupTwoInstructions.HLT))
            {
                processor.Halt();
            }
        }
    }
}
