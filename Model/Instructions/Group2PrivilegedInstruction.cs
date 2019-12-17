using Core8.Model.Enums;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;

namespace Core8.Model.Instructions
{
    public class Group2PrivilegedInstruction : InstructionBase
    {
        public Group2PrivilegedInstruction(uint address, uint data) : base(address, data)
        {
        }

        protected override string OpCodeText => OpCodes.ToString();

        private Group2PrivilegedOpCodes OpCodes => (Group2PrivilegedOpCodes)(Data & Masks.PRIVILEGED_GROUP_2_FLAGS);

        public override void Execute(IHardware hardware)
        {
            if (OpCodes.HasFlag(Group2PrivilegedOpCodes.OSR))
            {
                hardware.Registers.LINK_AC.SetAccumulator(hardware.Registers.LINK_AC.Accumulator | hardware.Registers.Switch.Get);
            }

            if (OpCodes.HasFlag(Group2PrivilegedOpCodes.HLT))
            {
                hardware.Processor.Halt();
            }
        }
    }
}
