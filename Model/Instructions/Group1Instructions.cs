using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;

namespace Core8.Model.Instructions
{
    internal class Group1Instructions : InstructionsBase
    {
        private const int IAC_MASK = 1 << 0;
        private const int BSW_MASK = 1 << 1;
        private const int RAL_MASK = 1 << 2;
        private const int RAR_MASK = 1 << 3;
        private const int CML_MASK = 1 << 4;
        private const int CMA_MASK = 1 << 5;
        private const int CLL_MASK = 1 << 6;
        private const int CLA_MASK = 1 << 7;

        public Group1Instructions(ICPU cpu) : base(cpu)
        {
        }

        protected override string OpCodeText => ((Group1OpCodes)(Data & Masks.GROUP_1_FLAGS)).ToString();

        public override void Execute()
        {
            if ((Data & CLA_MASK) != 0)
            {
                AC.ClearAccumulator();
            }

            if ((Data & CLL_MASK) != 0)
            {
                AC.ClearLink();
            }

            if ((Data & CMA_MASK) != 0)
            {
                AC.ComplementAccumulator();
            }

            if ((Data & CML_MASK) != 0)
            {
                AC.ComplementLink();
            }

            if ((Data & IAC_MASK) != 0)
            {
                AC.AddWithCarry(1);
            }

            switch (Data & (RAL_MASK | RAR_MASK | BSW_MASK))
            {
                case RAR_MASK:
                    AC.RAR();
                    break;
                case RAR_MASK | BSW_MASK:
                    AC.RAR();
                    AC.RAR();
                    break;
                case RAL_MASK:
                    AC.RAL();
                    break;
                case RAL_MASK | BSW_MASK:
                    AC.RAL();
                    AC.RAL();
                    break;
                case RAL_MASK | RAR_MASK:
                    AC.ANDAccumulator(Data);
                    break;
                case RAL_MASK | RAR_MASK | BSW_MASK:
                    AC.SetAccumulator(Page | Word);
                    break;
                case BSW_MASK:
                    AC.ByteSwap();
                    break;
            }
        }

        [Flags]
        private enum Group1OpCodes
        {
            NOP = 0,
            IAC = IAC_MASK,
            BSW = BSW_MASK,
            RAL = RAL_MASK,
            RAR = RAR_MASK,
            CML = CML_MASK,
            CMA = CMA_MASK,
            CLL = CLL_MASK,
            CLA = CLA_MASK,
        }
    }
}
