using Core8.Enums;
using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions
{
    public class Group2OR : InstructionBase
    {
        public Group2OR(IRegisters registers) : base(registers)
        { }

        public override void Execute(uint data)
        {
            var flags = (GroupTwoOrInstructions)(data & Masks.GROUP_2_OR_FLAGS);

            bool result = false;

            if (flags.HasFlag(GroupTwoOrInstructions.SMA))
            {
                result |= ((Registers.LINK_AC.Accumulator & Masks.AC_SIGN) != 0);
            }

            if (flags.HasFlag(GroupTwoOrInstructions.SZA))
            {
                result |= (Registers.LINK_AC.Accumulator == 0);
            }

            if (flags.HasFlag(GroupTwoOrInstructions.SNL))
            {
                result |= (Registers.LINK_AC.Link != 0);
            }

            if (result)
            {
                Registers.IF_PC.Increment();
            }

            if (flags.HasFlag(GroupTwoOrInstructions.CLA))
            {
                Registers.LINK_AC.Clear();
            }
        }
    }
}
