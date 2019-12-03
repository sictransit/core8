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

            result &= Flags.HasFlag(GroupTwoAndFlags.SPA) && ((environment.Registers.LINK_AC.Accumulator & Masks.AC_SIGN) == 0);
            result &= Flags.HasFlag(GroupTwoAndFlags.SNA) && (environment.Registers.LINK_AC.Accumulator != 0);
            result &= Flags.HasFlag(GroupTwoAndFlags.SZL) && (environment.Registers.LINK_AC.Link == 0);

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
