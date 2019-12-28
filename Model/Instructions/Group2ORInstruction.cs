using Core8.Model.Enums;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;

namespace Core8.Model.Instructions
{
    public class Group2ORInstruction : InstructionBase
    {
        public Group2ORInstruction(uint address, uint data) : base(address, data)
        {
        }

        protected override string OpCodeText => OpCodes.ToString();

        private Group2OROpCodes OpCodes => (Group2OROpCodes)(Data & Masks.GROUP_2_AND_OR_FLAGS);

        public override void Execute(IHardware hardware)
        {
            bool result = false;

            if (OpCodes.HasFlag(Group2OROpCodes.SMA))
            {
                result |= (hardware.Registers.LINK_AC.Accumulator & Masks.AC_SIGN) != 0;
            }

            if (OpCodes.HasFlag(Group2OROpCodes.SZA))
            {
                result |= hardware.Registers.LINK_AC.Accumulator == 0;
            }

            if (OpCodes.HasFlag(Group2OROpCodes.SNL))
            {
                result |= hardware.Registers.LINK_AC.Link != 0;
            }

            if (result)
            {
                hardware.Registers.IF_PC.Increment();
            }

            if (OpCodes.HasFlag(Group2OROpCodes.CLA))
            {
                hardware.Registers.LINK_AC.SetAccumulator(0);
            }
        }
    }
}
