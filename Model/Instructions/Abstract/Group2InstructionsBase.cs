using Core8.Model.Interfaces;
using System;

namespace Core8.Model.Instructions.Abstract
{
    internal abstract class Group2InstructionsBase : PrivilegedInstructionsBase
    {
        private const int HLT_MASK = 1 << 1;
        private const int OSR_MASK = 1 << 2;

        protected Group2InstructionsBase(ICPU cpu) : base(cpu)
        {
        }

        protected override string OpCodeText =>
            (Data & 0b_000_000_000_110) != 0
            ? ((Group2PrivilegedOpCodes)(Data & 0b_000_000_000_110)).ToString()
            : string.Empty;

        public override void Execute()
        {
            if ((Data & 0b_000_000_000_110) != 0)
            {
                base.Execute();
            }
        }

        protected override void PrivilegedExecute()
        {
            if ((Data & OSR_MASK) != 0)
            {
                AC.ORAccumulator(SR.Content);
            }

            if ((Data & HLT_MASK) != 0)
            {
                CPU.Halt();
            }
        }

        [Flags]
        private enum Group2PrivilegedOpCodes
        {
            HLT = HLT_MASK,
            OSR = OSR_MASK,
        }
    }
}
