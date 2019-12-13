using Core8.Enums;
using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions
{
    public class GroupTwoAndMicrocodedInstructions : MicrocodedInstructionsBase
    {
        public GroupTwoAndMicrocodedInstructions(IRegisters registers) : base(registers)
        { }

        public override void Execute(uint data)
        {
            var flags = (GroupTwoAndFlags)(data & Masks.GROUP_2_AND_FLAGS);

            bool result = true;

            if (flags.HasFlag(GroupTwoAndFlags.SPA))
            {
                result &= (Registers.LINK_AC.Accumulator & Masks.AC_SIGN) == 0;
            }

            if (flags.HasFlag(GroupTwoAndFlags.SNA))
            {
                result &= (Registers.LINK_AC.Accumulator != 0);
            }

            if (flags.HasFlag(GroupTwoAndFlags.SZL))
            {
                result &= (Registers.LINK_AC.Link == 0);
            }

            if (result)
            {
                Registers.IF_PC.Increment();
            }

            if (flags.HasFlag(GroupTwoAndFlags.CLA))
            {
                Registers.LINK_AC.Clear();
            }
        }
    }
}
