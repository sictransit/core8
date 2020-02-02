using Core8.Model.Enums;
using Core8.Model.Interfaces;

namespace Core8.Model.Instructions.Abstract
{
    public abstract class Group2InstructionsBase : InstructionsBase
    {
        private readonly IProcessor processor;

        internal Group2InstructionsBase(IProcessor processor, IRegisters registers) : base(registers)
        {
            this.processor = processor;
        }

        protected override string OpCodeText => OpCodes != 0 ? OpCodes.ToString() : string.Empty;

        private Group2PrivilegedOpCodes OpCodes => (Group2PrivilegedOpCodes)(Data & Masks.PRIVILEGED_GROUP_2_FLAGS);

        public override bool Privileged => OpCodes.HasFlag(Group2PrivilegedOpCodes.OSR) || OpCodes.HasFlag(Group2PrivilegedOpCodes.HLT);

        public override void Execute()
        {
            if (OpCodes.HasFlag(Group2PrivilegedOpCodes.OSR))
            {
                Registers.LINK_AC.ORAccumulator(Registers.SR.Get);
            }

            if (OpCodes.HasFlag(Group2PrivilegedOpCodes.HLT))
            {
                processor.Halt();
            }
        }
    }
}
