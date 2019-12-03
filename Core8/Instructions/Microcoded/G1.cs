using Core8.Enums;
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

        protected override void ExecuteInternal(IEnvironment environment)
        {
            if (environment is null)
            {
                throw new System.ArgumentNullException(nameof(environment));
            }

            if (Flags.HasFlag(GroupOneFlags.CLA))
            {
                environment.Registers.LINK_AC.SetAccumulator(0);
            }

            if (Flags.HasFlag(GroupOneFlags.CLL))
            {
                environment.Registers.LINK_AC.SetLink(0);
            }

            if (Flags.HasFlag(GroupOneFlags.CMA))
            {
                environment.Registers.LINK_AC.SetAccumulator(environment.Registers.LINK_AC.Accumulator % Masks.AC);
            }

            if (Flags.HasFlag(GroupOneFlags.CML))
            {
                environment.Registers.LINK_AC.SetLink(environment.Registers.LINK_AC.Link % Masks.FLAG);
            }

            if (Flags.HasFlag(GroupOneFlags.IAC))
            {
                environment.Registers.LINK_AC.Set(environment.Registers.LINK_AC.Data + 1);
            }

            if (Flags.HasFlag(GroupOneFlags.RAR))
            {
                RotateAccumulatorRight(environment.Registers);

                if (Flags.HasFlag(GroupOneFlags.BSW))
                {
                    RotateAccumulatorRight(environment.Registers);
                }
            }

            if (Flags.HasFlag(GroupOneFlags.RAL))
            {
                RotateAccumulatorLeft(environment.Registers);

                if (Flags.HasFlag(GroupOneFlags.BSW))
                {
                    RotateAccumulatorLeft(environment.Registers);
                }
            }

            if (Flags.HasFlag(GroupOneFlags.BSW) && !Flags.HasFlag(GroupOneFlags.RAR) && !Flags.HasFlag(GroupOneFlags.RAL))
            {
                ByteSwap(environment.Registers);
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

            var result = (acc >> 1) | ((acc & Masks.FLAG) << 12);

            registers.LINK_AC.Set(result);
        }

        private void RotateAccumulatorLeft(IRegisters registers)
        {
            var acc = registers.LINK_AC.Data;

            var result = (acc << 1) | ((acc & Masks.LINK) >> 12);

            registers.LINK_AC.Set(result);
        }

    }
}
