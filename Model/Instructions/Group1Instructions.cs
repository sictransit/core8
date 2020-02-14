using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;

namespace Core8.Model.Instructions
{
    public class Group1Instructions : InstructionsBase
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
                Registers.AC.ClearAccumulator();
            }

            if ((Data & CLL_MASK) != 0)
            {
                Registers.AC.ClearLink();
            }

            if ((Data & CMA_MASK) != 0)
            {
                Registers.AC.ComplementAccumulator();
            }

            if ((Data & CML_MASK) != 0)
            {
                Registers.AC.ComplementLink();
            }

            if ((Data & IAC_MASK) != 0)
            {
                Registers.AC.IncrementWithCarry();
            }

            if ((Data & RAR_MASK) != 0)
            {
                Registers.AC.RAR();

                if ((Data & BSW_MASK) != 0)
                {
                    Registers.AC.RAR();
                }
            }

            if ((Data & RAL_MASK) != 0)
            {
                Registers.AC.RAL();

                if ((Data & BSW_MASK) != 0)
                {
                    Registers.AC.RAL();
                }
            }

            if ((Data & (BSW_MASK | RAR_MASK | RAL_MASK)) == BSW_MASK)
            {
                Registers.AC.ByteSwap();
            }
        }

        [Flags]
        private enum Group1OpCodes : int
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
