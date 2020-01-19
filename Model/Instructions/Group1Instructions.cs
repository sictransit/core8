using Core8.Model.Enums;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;

namespace Core8.Model.Instructions
{
    public class Group1Instructions : InstructionsBase
    {
        internal Group1Instructions(IRegisters registers) : base(registers)
        {
        }

        protected override string OpCodeText => OpCodes.ToString();

        private Group1OpCodes OpCodes => (Group1OpCodes)(Data & Masks.GROUP_1_FLAGS);

        public override void Execute()
        {
            if (OpCodes.HasFlag(Group1OpCodes.CLA))
            {
                Registers.LINK_AC.ClearAccumulator();
            }

            if (OpCodes.HasFlag(Group1OpCodes.CLL))
            {
                Registers.LINK_AC.ClearLink();
            }

            if (OpCodes.HasFlag(Group1OpCodes.CMA))
            {
                Registers.LINK_AC.ComplementAccumulator();
            }

            if (OpCodes.HasFlag(Group1OpCodes.CML))
            {
                Registers.LINK_AC.ComplementLink();
            }

            if (OpCodes.HasFlag(Group1OpCodes.IAC))
            {
                Registers.LINK_AC.IncrementWithCarry();
            }

            if (OpCodes.HasFlag(Group1OpCodes.RAR))
            {
                Registers.LINK_AC.RAR();

                if (OpCodes.HasFlag(Group1OpCodes.BSW))
                {
                    Registers.LINK_AC.RAR();
                }
            }

            if (OpCodes.HasFlag(Group1OpCodes.RAL))
            {
                Registers.LINK_AC.RAL();

                if (OpCodes.HasFlag(Group1OpCodes.BSW))
                {
                    Registers.LINK_AC.RAL();
                }
            }

            if (OpCodes.HasFlag(Group1OpCodes.BSW) && !OpCodes.HasFlag(Group1OpCodes.RAR) && !OpCodes.HasFlag(Group1OpCodes.RAL))
            {
                Registers.LINK_AC.ByteSwap();
            }
        }
    }
}
