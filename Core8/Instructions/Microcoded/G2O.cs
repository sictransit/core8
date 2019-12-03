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

        protected override void ExecuteInternal(IEnvironment environment)
        {
            if (environment is null)
            {
                throw new System.ArgumentNullException(nameof(environment));
            }

            bool result = false;

            if (Flags.HasFlag(GroupTwoOrFlags.SMA))
            {
                result |=  ((environment.Registers.LINK_AC.Accumulator & Masks.AC_SIGN) != 0);
            }

            if (Flags.HasFlag(GroupTwoOrFlags.SZA))
            {
                result |= (environment.Registers.LINK_AC.Accumulator == 0);
            }

            if (Flags.HasFlag(GroupTwoOrFlags.SNL))
            { 
                result |= (environment.Registers.LINK_AC.Link != 0); 
            }

            if (result)
            {
                environment.Registers.IF_PC.Increment();
            }

            if (Flags.HasFlag(GroupTwoOrFlags.CLA))
            {
                environment.Registers.LINK_AC.Clear();
            }
        }
    }
}
