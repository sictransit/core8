using Core8.Model.Enums;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;

namespace Core8.Model.Instructions
{
    public class MemoryManagementInstructions : InstructionsBase
    {
        private readonly IMemory memory;

        public MemoryManagementInstructions(IMemory memory, IRegisters registers) : base(registers)
        {
            this.memory = memory;
        }

        protected override string OpCodeText => OpCodes.ToString();

        private MemoryManagementOpCodes OpCodes => (MemoryManagementOpCodes)(Data & Masks.GROUP_1_FLAGS);

        public override void Execute()
        {
            if (OpCodes.HasFlag(MemoryManagementOpCodes.CDF))
            {
            }

            if (OpCodes.HasFlag(MemoryManagementOpCodes.CIF))
            { 
            }

        }

    }

}
