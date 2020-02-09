using Core8.Model.Enums;
using Core8.Model.Interfaces;

namespace Core8.Model.Instructions.Abstract
{
    public abstract class Group2InstructionsBase : PrivilegedInstructionsBase
    {
        internal Group2InstructionsBase(IProcessor processor) : base(processor)
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
                Processor.Registers.AC.ORAccumulator(Processor.Registers.SR.Get);
            }

            if (OpCodes.HasFlag(Group2PrivilegedOpCodes.HLT))
            {
                Processor.Halt();
            }
        }
    }
}
