using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;
using System.Linq;

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
            (Data & 0b_000_011_111_000) == 0
            ? base.OpCodeText
            : string.Join(' ', new[] { SplitOpCodes((Group2OROpCodes)(Data & 0b_000_011_111_000)), base.OpCodeText }.Where(s => !string.IsNullOrEmpty(s)));

        public override void Execute()
        {
            var skip = false;

            if ((Data & SMA_MASK) != 0)
            {
                skip = (AC.Accumulator & 0b_0_100_000_000_000) != 0;
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
            SMA = SMA_MASK,
            SZA = SZA_MASK,
            SNL = SNL_MASK,
            CLA = CLA_MASK,
        }

    }
}
