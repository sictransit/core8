using Core8.Model.Enums;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;

namespace Core8.Model.Instructions
{
    public class Group2ANDInstructions : Group2InstructionsBase
    {
        public Group2ANDInstructions(IProcessor processor) : base(processor)
        {
        }

        protected override string OpCodeText => OpCodes == 0 ? base.OpCodeText : string.Join(' ', OpCodes.ToString(), base.OpCodeText);

        private Group2ANDOpCodes OpCodes => (Group2ANDOpCodes)(Data & Masks.GROUP_2_AND_OR_FLAGS);

        public override void Execute()
        {
            bool result = true;

            if (OpCodes.HasFlag(Group2ANDOpCodes.SPA))
            {
                result &= (Register.AC.Accumulator & Masks.AC_SIGN) == 0;
            }

            if (OpCodes.HasFlag(Group2ANDOpCodes.SNA))
            {
                result &= Register.AC.Accumulator != 0;
            }

            if (OpCodes.HasFlag(Group2ANDOpCodes.SZL))
            {
                result &= Register.AC.Link == 0;
            }

            if (result)
            {
                Register.PC.Increment();
            }

            if (OpCodes.HasFlag(Group2ANDOpCodes.CLA))
            {
                Register.AC.ClearAccumulator();
            }

            base.Execute();
        }
    }
}
