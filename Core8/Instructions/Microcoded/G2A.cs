using Core8.Enum;
using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions.Microcoded
{
    public class G2A : MicrocodedInstruction
    {
        public G2A(uint data) : base(data)
        { }

        public GroupTwoAndFlags Flags => (GroupTwoAndFlags)(Data & Masks.GROUP_2_AND_FLAGS);

        protected override string FlagString => Flags.ToString();

        public override void Execute(ICore core)
        {
            bool result = true;

            result &= Flags.HasFlag(GroupTwoAndFlags.SPA) && ((core.Registers.LINK_AC.Accumulator & Masks.AC_SIGN) == 0);
            result &= Flags.HasFlag(GroupTwoAndFlags.SNA) && (core.Registers.LINK_AC.Accumulator != 0);
            result &= Flags.HasFlag(GroupTwoAndFlags.SZL) && (core.Registers.LINK_AC.Link == 0);

            if (result)
            {
                core.Registers.IF_PC.Increment();
            }

            if (Flags.HasFlag(GroupTwoAndFlags.CLA))
            {
                core.Registers.LINK_AC.Clear();
            }
        }
    }
}
