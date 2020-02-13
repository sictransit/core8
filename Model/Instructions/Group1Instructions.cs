using Core8.Model.Enums;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;

namespace Core8.Model.Instructions
{
    public class Group1Instructions : InstructionsBase
    {
        public Group1Instructions(ICPU cpu) : base(cpu)
        {
        }

        protected override string OpCodeText => OpCodes.ToString();

        private Group1OpCodes OpCodes => (Group1OpCodes)(Data & Masks.GROUP_1_FLAGS);

        public override void Execute()
        {
            if (OpCodes.HasFlag(Group1OpCodes.CLA))
            {
                Registers.AC.ClearAccumulator();
            }

            if (OpCodes.HasFlag(Group1OpCodes.CLL))
            {
                Registers.AC.ClearLink();
            }

            if (OpCodes.HasFlag(Group1OpCodes.CMA))
            {
                Registers.AC.ComplementAccumulator();
            }

            if (OpCodes.HasFlag(Group1OpCodes.CML))
            {
                Registers.AC.ComplementLink();
            }

            if (OpCodes.HasFlag(Group1OpCodes.IAC))
            {
                Registers.AC.IncrementWithCarry();
            }

            if (OpCodes.HasFlag(Group1OpCodes.RAR))
            {
                Registers.AC.RAR();

                if (OpCodes.HasFlag(Group1OpCodes.BSW))
                {
                    Registers.AC.RAR();
                }
            }

            if (OpCodes.HasFlag(Group1OpCodes.RAL))
            {
                Registers.AC.RAL();

                if (OpCodes.HasFlag(Group1OpCodes.BSW))
                {
                    Registers.AC.RAL();
                }
            }

            if (OpCodes.HasFlag(Group1OpCodes.BSW) && !OpCodes.HasFlag(Group1OpCodes.RAR) && !OpCodes.HasFlag(Group1OpCodes.RAL))
            {
                Registers.AC.ByteSwap();
            }
        }
    }
}
