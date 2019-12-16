using Core8.Enums;
using Core8.Instructions.Abstract;
using Core8.Interfaces;
using System.Diagnostics;

namespace Core8.Instructions
{
    public class Group2ANDInstruction : InstructionBase
    {
        public Group2ANDInstruction(uint address, uint data) : base(address, data)
        {
        }

        protected override string OpCodeText => OpCodes.ToString();

        private Group2ANDOpCodes OpCodes => (Group2ANDOpCodes)(Data & Masks.GROUP_2_AND_OR_FLAGS);

        public override void Execute(IHardware hardware)
        {
            bool result = true;

            if (OpCodes.HasFlag(Group2ANDOpCodes.SPA))
            {
                result &= (hardware.Registers.LINK_AC.Accumulator & Masks.AC_SIGN) == 0;
            }

            if (OpCodes.HasFlag(Group2ANDOpCodes.SNA))
            {
                result &= (hardware.Registers.LINK_AC.Accumulator != 0);
            }

            if (OpCodes.HasFlag(Group2ANDOpCodes.SZL))
            {
                result &= (hardware.Registers.LINK_AC.Link == 0);
            }

            if (result)
            {
                hardware.Registers.IF_PC.Increment();
            }

            if (OpCodes.HasFlag(Group2ANDOpCodes.CLA))
            {
                hardware.Registers.LINK_AC.Clear();
            }
        }
    }
}
