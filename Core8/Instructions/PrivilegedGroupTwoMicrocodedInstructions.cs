using Core8.Enums;
using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions
{
    public class PrivilegedGroupTwoMicrocodedInstructions : MicrocodedInstructionsBase
    {
        private readonly IProcessor processor;

        public PrivilegedGroupTwoMicrocodedInstructions(IProcessor processor, IRegisters registers) : base(registers)
        {
            this.processor = processor;
        }

        public override void Execute(uint data)
        {
            var flags = (PrivilegedGroupTwoFlags)(data & Masks.PRIVILEGED_GROUP_2_FLAGS);

            if (flags.HasFlag(PrivilegedGroupTwoFlags.OSR))
            {
                Registers.LINK_AC.SetAccumulator(Registers.LINK_AC.Accumulator | Registers.Switch.Get);
            }

            if (flags.HasFlag(PrivilegedGroupTwoFlags.HLT))
            {
                processor.Halt();
            }
        }
    }
}
