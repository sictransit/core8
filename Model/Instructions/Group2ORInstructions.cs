using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;

namespace Core8.Model.Instructions
{
    public class Group2ORInstructions : Group2InstructionsBase
    {
        private const int CLA_MASK = 1 << 7;
        private const int SMA_MASK = 1 << 6;
        private const int SZA_MASK = 1 << 5;
        private const int SNL_MASK = 1 << 4;

        public Group2ORInstructions(ICPU cpu) : base(cpu)
        {
        }

        protected override string OpCodeText =>
            (Data & Masks.GROUP_2_AND_OR_FLAGS) == 0
            ? base.OpCodeText
            : string.Join(' ', ((Group2OROpCodes)(Data & Masks.GROUP_2_AND_OR_FLAGS)).ToString(), base.OpCodeText);

        public override void Execute()
        {
            var skip = false;

            if ((Data & SMA_MASK) != 0)
            {
                skip = (AC.Accumulator & Masks.AC_SIGN) != 0;
            }

            if (!skip && (Data & SZA_MASK) != 0)
            {
                skip = AC.Accumulator == 0;
            }

            if (!skip && (Data & SNL_MASK) != 0)
            {
                skip = AC.Link != 0;
            }

            if (skip)
            {
                PC.Increment();
            }

            if ((Data & CLA_MASK) != 0)
            {
                AC.ClearAccumulator();
            }

            base.Execute();
        }

        [Flags]
        private enum Group2OROpCodes
        {
            CLA = CLA_MASK,
            SMA = SMA_MASK,
            SZA = SZA_MASK,
            SNL = SNL_MASK,
        }

    }
}
