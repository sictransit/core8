using Core8.Model.Enums;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;

namespace Core8.Model.Instructions
{
    public class Group2ANDInstructions : Group2InstructionsBase
    {
        public Group2ANDInstructions(ICPU cpu) : base(cpu)
        {
        }

        protected override string OpCodeText => OpCodes == 0 ? base.OpCodeText : string.Join(' ', OpCodes.ToString(), base.OpCodeText);

        private Group2ANDOpCodes OpCodes => (Group2ANDOpCodes)(Data & Masks.GROUP_2_AND_OR_FLAGS);

        public override void Execute()
        {
            bool result = true;

            if (OpCodes.HasFlag(Group2ANDOpCodes.SPA))
            {
                result &= (Registers.AC.Accumulator & Masks.AC_SIGN) == 0;
            }

            if (OpCodes.HasFlag(Group2ANDOpCodes.SNA))
            {
                result &= Registers.AC.Accumulator != 0;
            }

            if (OpCodes.HasFlag(Group2ANDOpCodes.SZL))
            {
                result &= Registers.AC.Link == 0;
            }

            if (result)
            {
                Registers.PC.Increment();
            }

            if (OpCodes.HasFlag(Group2ANDOpCodes.CLA))
            {
                Registers.AC.ClearAccumulator();
            }

            base.Execute();
        }
    }
}
