using Core8.Enums;
using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions
{
    public class Group1Instruction : InstructionBase
    {
        public Group1Instruction(uint address, uint data) : base(address, data)
        {
        }

        protected override string OpCodeText => OpCodes.ToString();

        private Group1OpCodes OpCodes => (Group1OpCodes)(Data & Masks.GROUP_1_FLAGS);

        public override void Execute(IHardware hardware)
        {
            void RotateAccumulatorRight()
            {
                var acc = hardware.Registers.LINK_AC.Data;

                var result = acc >> 1 | (acc & Masks.FLAG) << 12;

                hardware.Registers.LINK_AC.Set(result);
            }

            void RotateAccumulatorLeft()
            {
                var acc = hardware.Registers.LINK_AC.Data;

                var result = acc << 1 | (acc & Masks.LINK) >> 12;

                hardware.Registers.LINK_AC.Set(result);
            }

            if (OpCodes.HasFlag(Group1OpCodes.CLA))
            {
                hardware.Registers.LINK_AC.SetAccumulator(0);
            }

            if (OpCodes.HasFlag(Group1OpCodes.CLL))
            {
                hardware.Registers.LINK_AC.SetLink(0);
            }

            if (OpCodes.HasFlag(Group1OpCodes.CMA))
            {
                hardware.Registers.LINK_AC.SetAccumulator(hardware.Registers.LINK_AC.Accumulator % Masks.AC);
            }

            if (OpCodes.HasFlag(Group1OpCodes.CML))
            {
                hardware.Registers.LINK_AC.SetLink(hardware.Registers.LINK_AC.Link % Masks.FLAG);
            }

            if (OpCodes.HasFlag(Group1OpCodes.IAC))
            {
                hardware.Registers.LINK_AC.Set(hardware.Registers.LINK_AC.Data + 1);
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
                var acc = hardware.Registers.LINK_AC.Accumulator;

                var result = (acc & Masks.AC_HIGH) >> 6 | (acc & Masks.AC_LOW) << 6;

                hardware.Registers.LINK_AC.SetAccumulator(result);
            }
        }
    }
}
