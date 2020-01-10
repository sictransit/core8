using Core8.Model.Enums;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;

namespace Core8.Model.Instructions
{
    public class Group2ANDInstruction : Group2InstructionBase
    {
        public Group2ANDInstruction(uint address, uint data, IProcessor processor, IRegisters registers) : base(address, data, processor, registers)
        {
        }

        protected override string OpCodeText => OpCodes == 0 ? base.OpCodeText : string.Join(' ', OpCodes.ToString(), base.OpCodeText);

        private Group2ANDOpCodes OpCodes => (Group2ANDOpCodes)(Data & Masks.GROUP_2_AND_OR_FLAGS);

        public override void Execute()
        {
            bool result = true;

            if (OpCodes.HasFlag(Group2ANDOpCodes.SPA))
            {
                result &= (Registers.LINK_AC.Accumulator & Masks.AC_SIGN) == 0;
            }

            if (OpCodes.HasFlag(Group2ANDOpCodes.SNA))
            {
                result &= Registers.LINK_AC.Accumulator != 0;
            }

            if (OpCodes.HasFlag(Group2ANDOpCodes.SZL))
            {
                result &= Registers.LINK_AC.Link == 0;
            }

            if (result)
            {
                Registers.IF_PC.Increment();
            }

            if (OpCodes.HasFlag(Group2ANDOpCodes.CLA))
            {
                Registers.LINK_AC.SetAccumulator(0);
            }

            base.Execute();
        }
    }
}
