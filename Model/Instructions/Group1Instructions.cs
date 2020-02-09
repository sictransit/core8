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
                Register.AC.ClearAccumulator();
            }

            if (OpCodes.HasFlag(Group1OpCodes.CLL))
            {
                Register.AC.ClearLink();
            }

            if (OpCodes.HasFlag(Group1OpCodes.CMA))
            {
                Register.AC.ComplementAccumulator();
            }

            if (OpCodes.HasFlag(Group1OpCodes.CML))
            {
                Register.AC.ComplementLink();
            }

            if (OpCodes.HasFlag(Group1OpCodes.IAC))
            {
                Register.AC.IncrementWithCarry();
            }

            if (OpCodes.HasFlag(Group1OpCodes.RAR))
            {
                Register.AC.RAR();

                if (OpCodes.HasFlag(Group1OpCodes.BSW))
                {
                    Register.AC.RAR();
                }
            }

            if (OpCodes.HasFlag(Group1OpCodes.RAL))
            {
                Register.AC.RAL();

                if (OpCodes.HasFlag(Group1OpCodes.BSW))
                {
                    Register.AC.RAL();
                }
            }

            if (OpCodes.HasFlag(Group1OpCodes.BSW) && !OpCodes.HasFlag(Group1OpCodes.RAR) && !OpCodes.HasFlag(Group1OpCodes.RAL))
            {
                Register.AC.ByteSwap();
            }
        }
    }
}
