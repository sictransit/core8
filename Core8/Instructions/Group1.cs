using Core8.Enums;
using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions
{
    public class Group1 : InstructionBase
    {
        public Group1(IRegisters registers) : base(registers)
        { }

        public override void Execute(uint data)
        {
            var flags = (GroupOneInstructions)(data & Masks.GROUP_1_FLAGS);

            if (flags.HasFlag(GroupOneInstructions.CLA))
            {
                Registers.LINK_AC.SetAccumulator(0);
            }

            if (flags.HasFlag(GroupOneInstructions.CLL))
            {
                Registers.LINK_AC.SetLink(0);
            }

            if (flags.HasFlag(GroupOneInstructions.CMA))
            {
                Registers.LINK_AC.SetAccumulator(Registers.LINK_AC.Accumulator % Masks.AC);
            }

            if (flags.HasFlag(GroupOneInstructions.CML))
            {
                Registers.LINK_AC.SetLink(Registers.LINK_AC.Link % Masks.FLAG);
            }

            if (flags.HasFlag(GroupOneInstructions.IAC))
            {
                Registers.LINK_AC.Set(Registers.LINK_AC.Data + 1);
            }

            if (flags.HasFlag(GroupOneInstructions.RAR))
            {
                RotateAccumulatorRight();

                if (flags.HasFlag(GroupOneInstructions.BSW))
                {
                    RotateAccumulatorRight();
                }
            }

            if (flags.HasFlag(GroupOneInstructions.RAL))
            {
                RotateAccumulatorLeft();

                if (flags.HasFlag(GroupOneInstructions.BSW))
                {
                    RotateAccumulatorLeft();
                }
            }

            if (flags.HasFlag(GroupOneInstructions.BSW) && !flags.HasFlag(GroupOneInstructions.RAR) && !flags.HasFlag(GroupOneInstructions.RAL))
            {
                ByteSwap();
            }
        }

        private void ByteSwap()
        {
            var acc = Registers.LINK_AC.Accumulator;

            var result = (acc & Masks.AC_HIGH) >> 6 | (acc & Masks.AC_LOW) << 6;

            Registers.LINK_AC.SetAccumulator(result);
        }

        private void RotateAccumulatorRight()
        {
            var acc = Registers.LINK_AC.Data;

            var result = acc >> 1 | (acc & Masks.FLAG) << 12;

            Registers.LINK_AC.Set(result);
        }

        private void RotateAccumulatorLeft()
        {
            var acc = Registers.LINK_AC.Data;

            var result = acc << 1 | (acc & Masks.LINK) >> 12;

            Registers.LINK_AC.Set(result);
        }
    }
}
