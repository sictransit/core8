using Core8.Enum;
using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions.Microcoded
{
    public class G1 : MicrocodedInstruction
    {
        public G1(uint data) : base(data)
        { }

        public GroupOneFlags Flags => (GroupOneFlags)(Data & Masks.GROUP_1_FLAGS);

        protected override string FlagString => Flags.ToString();

        public override void Execute(ICore core)
        {
            if (Flags.HasFlag(GroupOneFlags.CLA))
            {
                core.Registers.LINK_AC.SetAccumulator(0);
            }

            if (Flags.HasFlag(GroupOneFlags.CLL))
            {
                core.Registers.LINK_AC.SetLink(0);
            }

            if (Flags.HasFlag(GroupOneFlags.CMA))
            {
                core.Registers.LINK_AC.SetAccumulator(core.Registers.LINK_AC.Accumulator % Masks.AC);
            }

            if (Flags.HasFlag(GroupOneFlags.CML))
            {
                core.Registers.LINK_AC.SetLink(core.Registers.LINK_AC.Link % Masks.FLAG);
            }

            if (Flags.HasFlag(GroupOneFlags.IAC))
            {
                core.Registers.LINK_AC.Set(core.Registers.LINK_AC.Data + 1);
            }

            if (Flags.HasFlag(GroupOneFlags.RAR))
            {
                RotateAccumulatorRight(core.Registers);

                if (Flags.HasFlag(GroupOneFlags.BSW))
                {
                    RotateAccumulatorRight(core.Registers);
                }
            }

            if (Flags.HasFlag(GroupOneFlags.RAL))
            {
                RotateAccumulatorLeft(core.Registers);

                if (Flags.HasFlag(GroupOneFlags.BSW))
                {
                    RotateAccumulatorLeft(core.Registers);
                }
            }

            if (Flags.HasFlag(GroupOneFlags.BSW) && !Flags.HasFlag(GroupOneFlags.RAR) && !Flags.HasFlag(GroupOneFlags.RAL))
            {
                ByteSwap(core.Registers);
            }
        }

        private void ByteSwap(IRegisters registers)
        {
            var acc = registers.LINK_AC.Accumulator;

            var result = (acc & Masks.AC_HIGH) >> 6 | (acc & Masks.AC_LOW) << 6;

            registers.LINK_AC.SetAccumulator(result);
        }

        private void RotateAccumulatorRight(IRegisters registers)
        {
            var acc = registers.LINK_AC.Data;

            var result = acc >> 1 | (acc & Masks.FLAG) << 12;

            registers.LINK_AC.Set(result);
        }

        private void RotateAccumulatorLeft(IRegisters registers)
        {
            var acc = registers.LINK_AC.Data;

            var result = acc << 1 | (acc & Masks.LINK) >> 12;

            registers.LINK_AC.Set(result);
        }

    }
}
