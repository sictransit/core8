using Core8.Enums;
using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions.Microcoded
{
    public class G2O : MicrocodedInstruction
    {
        public G2O(uint data) : base(data)
        { }

        public GroupTwoOrFlags Flags => (GroupTwoOrFlags)(Data & Masks.GROUP_2_OR_FLAGS);

        protected override string FlagString => Flags.ToString();

        protected override void ExecuteInternal(ICore core)
        {
            if (core is null)
            {
                throw new System.ArgumentNullException(nameof(core));
            }

            bool result = false;

            result |= Flags.HasFlag(GroupTwoOrFlags.SMA) && ((core.Registers.LINK_AC.Accumulator & Masks.AC_SIGN) != 0);
            result |= Flags.HasFlag(GroupTwoOrFlags.SZA) && (core.Registers.LINK_AC.Accumulator == 0);
            result |= Flags.HasFlag(GroupTwoOrFlags.SNL) && (core.Registers.LINK_AC.Link != 0);

            if (result)
            {
                core.Registers.IF_PC.Increment();
            }

            if (Flags.HasFlag(GroupTwoOrFlags.CLA))
            {
                core.Registers.LINK_AC.Clear();
            }
        }
    }
}
