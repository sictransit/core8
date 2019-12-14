using Core8.Enums;
using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions
{
    public class Group2AND : InstructionBase
    {
        public Group2AND(IRegisters registers) : base(registers)
        { }

        public override void Execute(uint data)
        {
            var flags = (GroupTwoAndInstructions)(data & Masks.GROUP_2_AND_FLAGS);

            bool result = true;

            if (flags.HasFlag(GroupTwoAndInstructions.SPA))
            {
                result &= (Registers.LINK_AC.Accumulator & Masks.AC_SIGN) == 0;
            }

            if (flags.HasFlag(GroupTwoAndInstructions.SNA))
            {
                result &= (Registers.LINK_AC.Accumulator != 0);
            }

            if (flags.HasFlag(GroupTwoAndInstructions.SZL))
            {
                result &= (Registers.LINK_AC.Link == 0);
            }

            if (result)
            {
                Registers.IF_PC.Increment();
            }

            if (flags.HasFlag(GroupTwoAndInstructions.CLA))
            {
                Registers.LINK_AC.Clear();
            }
        }
    }
}
