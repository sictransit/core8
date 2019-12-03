using Core8.Enums;
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

        protected override void ExecuteInternal(IEnvironment environment)
        {
            if (environment is null)
            {
                throw new System.ArgumentNullException(nameof(environment));
            }

            bool result = true;

            if (Flags.HasFlag(GroupTwoAndFlags.SPA))
            {
                result &= (environment.Registers.LINK_AC.Accumulator & Masks.AC_SIGN) == 0;
            }

            if (Flags.HasFlag(GroupTwoAndFlags.SNA))
            {
                result &= (environment.Registers.LINK_AC.Accumulator != 0);
            }

            if (Flags.HasFlag(GroupTwoAndFlags.SZL))
            {
                result &= (environment.Registers.LINK_AC.Link == 0);
            }

            if (result)
            {
                environment.Registers.IF_PC.Increment();
            }

            if (Flags.HasFlag(GroupTwoAndFlags.CLA))
            {
                environment.Registers.LINK_AC.Clear();
            }
        }
    }
}
