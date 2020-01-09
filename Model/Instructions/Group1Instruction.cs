using Core8.Model.Enums;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;

namespace Core8.Model.Instructions
{
    public class Group1Instruction : InstructionBase
    {
        private readonly IRegisters registers;

        public Group1Instruction(uint address, uint data, IRegisters registers) : base(address, data)
        {
            this.registers = registers;
        }

        protected override string OpCodeText => OpCodes.ToString();

        private Group1OpCodes OpCodes => (Group1OpCodes)(Data & Masks.GROUP_1_FLAGS);

        public override void Execute()
        {
            void RotateAccumulatorRight()
            {
                var acc = registers.LINK_AC.Data;

                var result = (acc >> 1) & Masks.AC;
                result += (acc & Masks.FLAG) << 12;

                registers.LINK_AC.Set(result);
            }

            void RotateAccumulatorLeft()
            {
                var acc = registers.LINK_AC.Data;

                var result = (acc << 1) & (Masks.AC | Masks.LINK);
                result += (acc & Masks.LINK) >> 12;

                registers.LINK_AC.Set(result);
            }

            if (OpCodes.HasFlag(Group1OpCodes.CLA))
            {
                registers.LINK_AC.SetAccumulator(0);
            }

            if (OpCodes.HasFlag(Group1OpCodes.CLL))
            {
                registers.LINK_AC.SetLink(0);
            }

            if (OpCodes.HasFlag(Group1OpCodes.CMA))
            {
                registers.LINK_AC.SetAccumulator(registers.LINK_AC.Accumulator ^ Masks.AC);
            }

            if (OpCodes.HasFlag(Group1OpCodes.CML))
            {
                registers.LINK_AC.SetLink(registers.LINK_AC.Link ^ Masks.FLAG);
            }

            if (OpCodes.HasFlag(Group1OpCodes.IAC))
            {
                registers.LINK_AC.Set(registers.LINK_AC.Accumulator + 1);
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
                var acc = registers.LINK_AC.Accumulator;

                var result = (acc & Masks.AC_HIGH) >> 6 | (acc & Masks.AC_LOW) << 6;

                registers.LINK_AC.SetAccumulator(result);
            }
        }
    }
}
