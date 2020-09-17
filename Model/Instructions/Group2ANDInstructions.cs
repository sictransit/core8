using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;

namespace Core8.Model.Instructions
{
    public class Group2ANDInstructions : Group2InstructionsBase
    {
        private const int CLA_MASK = 1 << 7;
        private const int SPA_MASK = 1 << 6;
        private const int SNA_MASK = 1 << 5;
        private const int SZL_MASK = 1 << 4;
        private const int SKP_MASK = 1 << 3;

        public Group2ANDInstructions(ICPU cpu) : base(cpu)
        {
        }

        protected override string OpCodeText =>
            (Data & Masks.GROUP_2_AND_OR_FLAGS) == 0
            ? base.OpCodeText
            : string.Join(' ', ((Group2ANDOpCodes)(Data & Masks.GROUP_2_AND_OR_FLAGS)).ToString(), base.OpCodeText);

        public override void Execute()
        {
            var skip = true;

            if ((Data & SPA_MASK) != 0)
            {
                skip = (Registers.AC.Accumulator & Masks.AC_SIGN) == 0;
            }

            if (skip && (Data & SNA_MASK) != 0)
            {
                skip = Registers.AC.Accumulator != 0;
            }

            if (skip && (Data & SZL_MASK) != 0)
            {
                skip = Registers.AC.Link == 0;
            }

            if (skip)
            {
                Registers.PC.Increment();
            }

            if ((Data & CLA_MASK) != 0)
            {
                Registers.AC.ClearAccumulator();
            }

            base.Execute();
        }

        [Flags]
        private enum Group2ANDOpCodes
        {
            CLA = CLA_MASK,
            SPA = SPA_MASK,
            SNA = SNA_MASK,
            SZL = SZL_MASK,
            SKP = SKP_MASK,
        }

    }
}
