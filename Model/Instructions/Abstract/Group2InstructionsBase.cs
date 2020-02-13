using Core8.Model.Enums;
using Core8.Model.Interfaces;

namespace Core8.Model.Instructions.Abstract
{
    public abstract class Group2InstructionsBase : PrivilegedInstructionsBase
    {
        internal Group2InstructionsBase(ICPU cpu) : base(cpu)
        {
        }

        protected override string OpCodeText => OpCodes != 0 ? OpCodes.ToString() : string.Empty;

        private Group2PrivilegedOpCodes OpCodes => (Group2PrivilegedOpCodes)(Data & Masks.PRIVILEGED_GROUP_2_FLAGS);

        public override void Execute()
        {
            if (OpCodes != 0)
            {
                base.Execute();
            }
        }

        protected override void PrivilegedExecute()
        {
            if (OpCodes.HasFlag(Group2PrivilegedOpCodes.OSR))
            {
                CPU.Registers.AC.ORAccumulator(CPU.Registers.SR.Content);
            }

            if (OpCodes.HasFlag(Group2PrivilegedOpCodes.HLT))
            {
                CPU.Halt();
            }
        }
    }
}
