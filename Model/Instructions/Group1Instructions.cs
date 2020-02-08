using Core8.Model.Enums;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;

namespace Core8.Model.Instructions
{
    public class Group1Instructions : InstructionsBase
    {
        public Group1Instructions(IProcessor processor) : base(processor)
        {
        }

        protected override string OpCodeText => OpCodes.ToString();

        private Group1OpCodes OpCodes => (Group1OpCodes)(Data & Masks.GROUP_1_FLAGS);

        public override void Execute()
        {
            if (OpCodes.HasFlag(Group1OpCodes.CLA))
            {
                Register.LINK_AC.ClearAccumulator();
            }

            if (OpCodes.HasFlag(Group1OpCodes.CLL))
            {
                Register.LINK_AC.ClearLink();
            }

            if (OpCodes.HasFlag(Group1OpCodes.CMA))
            {
                Register.LINK_AC.ComplementAccumulator();
            }

            if (OpCodes.HasFlag(Group1OpCodes.CML))
            {
                Register.LINK_AC.ComplementLink();
            }

            if (OpCodes.HasFlag(Group1OpCodes.IAC))
            {
                Register.LINK_AC.IncrementWithCarry();
            }

            if (OpCodes.HasFlag(Group1OpCodes.RAR))
            {
                Register.LINK_AC.RAR();

                if (OpCodes.HasFlag(Group1OpCodes.BSW))
                {
                    Register.LINK_AC.RAR();
                }
            }

            if (OpCodes.HasFlag(Group1OpCodes.RAL))
            {
                Register.LINK_AC.RAL();

                if (OpCodes.HasFlag(Group1OpCodes.BSW))
                {
                    Register.LINK_AC.RAL();
                }
            }

            if (OpCodes.HasFlag(Group1OpCodes.BSW) && !OpCodes.HasFlag(Group1OpCodes.RAR) && !OpCodes.HasFlag(Group1OpCodes.RAL))
            {
                Register.LINK_AC.ByteSwap();
            }
        }
    }
}
