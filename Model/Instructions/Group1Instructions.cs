using Core8.Model.Enums;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;

namespace Core8.Model.Instructions
{
    public class Group1Instructions : InstructionsBase
    {
        public Group1Instructions(IRegisters registers) : base(registers)
        {
        }

        protected override string OpCodeText => OpCodes.ToString();

        private Group1OpCodes OpCodes => (Group1OpCodes)(Data & Masks.GROUP_1_FLAGS);

        protected override void Execute()
        {
            void RotateAccumulatorRight()
            {
                var acc = Registers.LINK_AC.Data;

                var result = (acc >> 1) & Masks.AC;
                result += (acc & Masks.FLAG) << 12;

                Registers.LINK_AC.Set(result);
            }

            void RotateAccumulatorLeft()
            {
                var acc = Registers.LINK_AC.Data;

                var result = (acc << 1) & (Masks.AC_LINK);
                result += (acc & Masks.LINK) >> 12;

                Registers.LINK_AC.Set(result);
            }

            if (OpCodes.HasFlag(Group1OpCodes.CLA))
            {
                Registers.LINK_AC.SetAccumulator(0);
            }

            if (OpCodes.HasFlag(Group1OpCodes.CLL))
            {
                Registers.LINK_AC.SetLink(0);
            }

            if (OpCodes.HasFlag(Group1OpCodes.CMA))
            {
                Registers.LINK_AC.SetAccumulator(Registers.LINK_AC.Accumulator ^ Masks.AC);
            }

            if (OpCodes.HasFlag(Group1OpCodes.CML))
            {
                Registers.LINK_AC.SetLink(Registers.LINK_AC.Link ^ Masks.FLAG);
            }

            if (OpCodes.HasFlag(Group1OpCodes.IAC))
            {
                Registers.LINK_AC.Set(Registers.LINK_AC.Accumulator + 1);
            }

            if (OpCodes.HasFlag(Group1OpCodes.RAR))
            {
                RotateAccumulatorRight();

                if (OpCodes.HasFlag(Group1OpCodes.BSW))
                {
                    RotateAccumulatorRight();
                }
            }

            if (OpCodes.HasFlag(Group1OpCodes.RAL))
            {
                RotateAccumulatorLeft();

                if (OpCodes.HasFlag(Group1OpCodes.BSW))
                {
                    RotateAccumulatorLeft();
                }
            }

            if (OpCodes.HasFlag(Group1OpCodes.BSW) && !OpCodes.HasFlag(Group1OpCodes.RAR) && !OpCodes.HasFlag(Group1OpCodes.RAL))
            {
                var acc = Registers.LINK_AC.Accumulator;

                var result = (acc & Masks.AC_HIGH) >> 6 | (acc & Masks.AC_LOW) << 6;

                Registers.LINK_AC.SetAccumulator(result);
            }
        }
    }
}
